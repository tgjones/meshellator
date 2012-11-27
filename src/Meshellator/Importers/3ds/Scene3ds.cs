using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nexus;

namespace Meshellator.Importers.Autodesk3ds
{
	/**
 * Toplevel class for parsing a 3ds-file and containing a 3D Studio vector animation.
 *
 * The Scene3ds class constructors takes 3ds-file data as input,
 * parses the data and builds a memory image of the relevant 3D vector 
 * animation data.
 * <br>
 * <br>
 * A successfully created Scene3ds object contains the following "global" data:<br>
 * - An array of meshes<br>
 * - An array of materials<br>
 * - An array of cameras<br>
 * - Start end end frame number of animation<br>
 *
 */
	public class Scene3ds
	{
		/**
		 * Decode level constant. Decode all information. Unused parameters
		 * as hex bytes.
		 */
		public const int DECODE_ALL = 3;

		/**
		 * Decode level constant. Decode all information except unused parameters.
		 */
		public const int DECODE_USED_PARAMS = 2;

		/**
		 * Decode level constant. Decode all information except unused parameters
		 * and chunks.
		 */
		public const int DECODE_USED_PARAMS_AND_CHUNKS = 1;


		// Array of meshes
		List<Mesh3ds> _meshes = new List<Mesh3ds>(5);

		// Array of cameras
		private readonly List<Camera3ds> _cameras = new List<Camera3ds>(5);

		// Array of materials
		private readonly List<Material3ds> _materials = new List<Material3ds>(5);

		// Array of lights
		private readonly List<Light3ds> _lights = new List<Light3ds>(5);

		// Parameters from the keyframer chunks:

		// Start frame
		int mStartFrame = 0;

		// End frame
		int mEndFrame = 0;



		// Add mesh at end of mesh array
		void addMesh(Mesh3ds m)
		{
			_meshes.Add(m);
		}

		// Add camera at end of camera array
		void addCamera(Camera3ds c)
		{
			_cameras.Add(c);
		}

		// Add material at end of material array
		private void addMaterial(Material3ds m)
		{
			_materials.Add(m);
		}

		// Add light at end of light array
		void addLight(Light3ds l)
		{
			_lights.Add(l);
		}

		/// <summary>
		/// Returns all the meshes in this scene.
		/// </summary>
		public IEnumerable<Mesh3ds> Meshes
		{
			get { return _meshes; }
		}

		/// <summary>
		/// Returns all the cameras in this scene.
		/// </summary>
		public IEnumerable<Camera3ds> Cameras
		{
			get { return _cameras; }
		}

		/// <summary>
		/// Returns all the materials in this scene.
		/// </summary>
		public IEnumerable<Material3ds> Materials
		{
			get { return _materials; }
		}

		/// <summary>
		/// Returns all the lights in this scene.
		/// </summary>
		public IEnumerable<Light3ds> Lights
		{
			get { return _lights; }
		}

		/// <summary>
		/// Get animation start frame.
		/// </summary>
		public int StartFrame
		{
			get { return mStartFrame; }
		}

		/**
		 * Get animation end frame.
		 *
		 * @return animation end frame
		 */
		public int endFrame()
		{
			return mEndFrame;
		}

		/**
		 * Construct (and decode) a Scene3ds object from a local 3ds-file.
		 *
		 * @param file      3ds-file
		 * @param decode    destination object for text decode
		 * @param level     decode level (DECODE_ALL, DECODE_USED_PARAMS,    
		 *                  DECODE_USED_PARAMS_AND_CHUNKS)
		 * @ in case of I/O or parsing problems
		 */
		public Scene3ds(FileStream fileStream, TextDecode3ds decode, int level)
		{
			if (decode != null)
			{
				mDecode = new Decode3ds(decode, level);
			}

			try
			{
				mFileData = new BinaryReader(fileStream);
				//mFileLength = mFileData.length;
				//mFilePos    = 0;

				Read3ds();
			}
			catch (IOException ex)
			{
				throw new Exception3ds("I/O problems: " + ex.Message, ex);
			}
			catch (Exception3ds ex)
			{
				throw new Exception3ds("3DS-parser: " + ex.Message, ex);
			}
			finally
			{
				mFileData.Close();
				mDecode = null;
			}
		}


		private BinaryReader mFileData = null;
		//private int    mFileLength = 0;
		//private int    mFilePos    = 0;

		private Decode3ds mDecode = null;


		private class Head
		{
			public int id;
			public int length;

			public Head(int id, int length)
			{
				this.id = id;
				this.length = length;
			}
		}

		private long filePos()
		{
			return mFileData.BaseStream.Position;
		}

		private byte ReadByte()
		{
			try
			{
				return mFileData.ReadByte();
			}
			catch (EndOfStreamException ex)
			{
				throw new Exception3ds("Read out of bounds! File is probably corrupt.", ex);
			}
		}

		private int ReadUnsignedShort()
		{
			try
			{
				return mFileData.ReadUInt16();
			}
			catch (EndOfStreamException ex)
			{
				throw new Exception3ds("Read out of bounds! File is probably corrupt.", ex);
			}
		}

		private int ReadInt()
		{
			try
			{
				return mFileData.ReadInt32();
			}
			catch (EndOfStreamException ex)
			{
				throw new Exception3ds("Read out of bounds! File is probably corrupt.", ex);
			}
		}

		private float ReadFloat()
		{
			try
			{
				return mFileData.ReadSingle();
			}
			catch (EndOfStreamException ex)
			{
				throw new Exception3ds("Read out of bounds! File is probably corrupt.", ex);
			}
		}

		private int SkipBytes(int n)
		{
			if (n < 0)
			{
				throw new Exception3ds("Negative chunk size! File is probably corrupt.");
			}
			else if (mFileData.BaseStream.Position + n > mFileData.BaseStream.Length)
			{
				throw new Exception3ds("Read out of bounds! File is probably corrupt.");
			}
			if (mDecode != null)
			{
				mDecode.printBytes(mFileData, n);
			}
			mFileData.BaseStream.Position += n;
			return n;
		}

		private void SkipChunk(int chunk_len)
		{
			if (mDecode != null)
			{
				mDecode.enter();
			}
			SkipBytes(chunk_len);
			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private Head read_HEAD()
		{
			int id = ReadUnsignedShort();
			int length = ReadInt();

			if (mDecode != null)
			{
				mDecode.printHead(id, length);
			}

			return new Head(id, length);
		}

		private string ReadName(int length = 32)
		{
			byte[] buf = new byte[length];
			bool terminated = false;
			int n;
			string name;

			for (n = 0; n < length; n++)
			{
				if ((buf[n] = ReadByte()) == 0)
				{
					terminated = true;
					break;
				}
			}
			if (terminated == false)
			{
				throw new Exception3ds("Name not terminated! File is probably corrupt.");
			}
			name = Encoding.UTF8.GetString(buf, 0, n);

			if (mDecode != null)
			{
				mDecode.enter();
				mDecode.println("Name: \"" + name + "\"");
				mDecode.leave();
			}

			return name;
		}

		private void Read3ds()
		{
			Head head = read_HEAD();

			if (head.id != CHUNK_M3DMAGIC)
				throw new Exception3ds("Bad signature! This is not a 3D Studio R4 .3ds file.");

			read_M3DMAGIC(head.length - 6);
		}

		private void read_M3DMAGIC(int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			if (mDecode != null)
			{
				mDecode.enter();
			}

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_MDATA:
						read_MDATA(head.length - 6);
						break;
					case CHUNK_KFDATA:
						read_KFDATA(head.length - 6);
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private void read_MDATA(int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			if (mDecode != null)
			{
				mDecode.enter();
			}

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_NAMED_OBJECT:
						read_NAMED_OBJECT(head.length - 6);
						break;
					case CHUNK_MAT_ENTRY:
						read_MAT_ENTRY(head.length - 6);
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private Color3ds readColor(int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			if (mDecode != null)
			{
				mDecode.enter();
			}

			Color3ds lvColor = new Color3ds();

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_COL_RGB:
						lvColor = readRGBColor();
						break;
					case CHUNK_COL_TRU:
						lvColor = readTrueColor();
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}

			return lvColor;

		}


		private float readPercentage(int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			if (mDecode != null)
			{
				mDecode.enter();
			}

			float val = 0.0f;

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_PERCENTW:
						//lvColor = readRGBColor();
						int trans = ReadUnsignedShort();
						val = (float)(trans / 100.0);
						//System.out.println( "short: " + trans + "  float: " + val );

						break;
					case CHUNK_PERCENTF:
						val = ReadFloat();
						//System.out.println( "just float: " + val );
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}

			return val;

		}


		private void read_MAT_ENTRY(int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			Material3ds mat = new Material3ds();
			addMaterial(mat);

			if (mDecode != null)
			{
				mDecode.enter();
			}

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_MAT_NAME:
						mat.mName = ReadName();
						break;
					case CHUNK_MAT_AMBIENT:
						mat.mAmbient = readColor(head.length - 6);
						break;
					case CHUNK_MAT_SPECULAR:
						mat.mSpecular = readColor(head.length - 6);
						break;
					case CHUNK_MAT_DIFFUSE:
						mat.mDiffuse = readColor(head.length - 6);
						break;
					case CHUNK_MAT_MAP:
						mat.DiffuseTexture = ParseTextureChunk(head.length - 6);
						break;
					//case CHUNK_MAT_SHININESS:
					//mat._shininess = readFloat();
					//break;
					case CHUNK_MAT_TRANSPARENCY:
						mat._transparency = readPercentage(head.length - 6);
						mat._transparency = 1.0f - mat._transparency;	// values come form 3ds inverted, so we need to invert it here again. 0 is full transparent, 1 is opaque.
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private Texture ParseTextureChunk(int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			Texture result = new Texture();

			if (mDecode != null)
			{
				mDecode.enter();
			}

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_MAT_MAPNAME:
						result.MapName = ReadName();
						break;
					case CHUNK_MAT_MAP_USCALE:
						// Texture coordinate scaling in the U direction
						result.ScaleU = ReadFloat();
						break;
					case CHUNK_MAT_MAP_VSCALE:
						// Texture coordinate scaling in the V direction
						result.ScaleV = ReadFloat();
						break;
					case CHUNK_MAT_MAP_UOFFSET:
						// Texture coordinate offset in the U direction
						result.OffsetU = -ReadFloat();
						break;

					case CHUNK_MAT_MAP_VOFFSET:
						// Texture coordinate offset in the V direction
						result.OffsetV = ReadFloat();
						break;

					case CHUNK_MAT_MAP_ANG:
						// Texture coordinate rotation, CCW in DEGREES
						result.Rotation = -MathUtility.ToRadians((ReadFloat()));
						break;

					case CHUNK_MAT_MAP_TILING:
					{
						int iFlags = ReadUnsignedShort();

						// Get the mapping mode (for both axes)
						if ((iFlags & 0x2u) != 0)
							result.MapMode = TextureMapMode.Mirror;
						else if ((iFlags & 0x10u) != 0)
							result.MapMode = TextureMapMode.Decal;
						else // wrapping in all remaining cases
							result.MapMode = TextureMapMode.Wrap;

						break;
					}
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}

			return result;
		}

		private void read_NAMED_OBJECT(int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			string name = ReadName();

			if (mDecode != null)
			{
				mDecode.enter();
			}

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_N_TRI_OBJECT:
						read_N_TRI_OBJECT(name, head.length - 6);
						break;
					case CHUNK_N_LIGHT:
						read_N_LIGHT(name, head.length - 6);
						break;
					case CHUNK_N_CAMERA:
						read_N_CAMERA(name, head.length - 6);
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}


		private void readSpotChunk(Light3ds pLight, int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			pLight.mTarget.X = ReadFloat();
			pLight.mTarget.Z = ReadFloat();
			pLight.mTarget.Y = ReadFloat();

			pLight.mHotspot = ReadFloat();
			pLight.mFalloff = ReadFloat();

			if (mDecode != null)
			{
				mDecode.println("Target: " + pLight.mTarget);
				mDecode.println("Hotspot: " + Utils3ds.floatToString(pLight.mHotspot, 12));
				mDecode.println("Falloff: " + Utils3ds.floatToString(pLight.mFalloff, 12));
			}

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_LIT_RAYSHAD:
						//        pLight.mRayShadows = (readUnsignedShort() > 0);
						pLight.mRayShadows = true;
						break;
					case CHUNK_LIT_SHADOWED:
						//        pLight.mShadowed = (readUnsignedShort() > 0);
						pLight.mShadowed = true;
						break;
					case CHUNK_LIT_LOCAL_SHADOW:
						ReadFloat();
						ReadFloat();
						ReadFloat();
						break;
					case CHUNK_LIT_LOCAL_SHADOW2:
						pLight.mShadowBias = ReadFloat();
						pLight.mShadowFilter = ReadFloat();
						pLight.mShadowSize = ReadFloat();
						break;
					case CHUNK_LIT_SEE_CONE:
						//        pLight.mCone = (readUnsignedShort() > 0);
						pLight.mCone = true;
						break;
					case CHUNK_LIT_SPOT_RECTANGULAR:
						//        pLight.mRectangular = (readUnsignedShort() > 0);
						pLight.mRectangular = true;
						break;
					case CHUNK_LIT_SPOT_OVERSHOOT:
						//        pLight.mOvershoot = (readUnsignedShort() > 0);
						pLight.mOvershoot = true;
						break;
					case CHUNK_LIT_SPOT_PROJECTOR:
						//        pLight.mProjector = (readUnsignedShort() > 0);
						pLight.mProjector = true;
						pLight.mProjectorName = ReadName(64);
						break;
					case CHUNK_LIT_SPOT_RANGE:
						ReadFloat();
						break;
					case CHUNK_LIT_SPOT_ROLL:
						pLight.mRoll = ReadFloat();
						break;
					case CHUNK_LIT_SPOT_ASPECT:
						pLight.mAspect = ReadFloat();
						break;
					case CHUNK_LIT_RAY_BIAS:
						pLight.mRayBias = ReadFloat();
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private void read_N_LIGHT(string name, int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			Light3ds lit = new Light3ds();
			lit.mName = name;

			addLight(lit);

			lit.mPosition.X = ReadFloat();
			lit.mPosition.Z = ReadFloat();
			lit.mPosition.Y = ReadFloat();

			if (mDecode != null)
			{
				mDecode.enter();
				mDecode.println("Position: " + lit.mPosition);
			}


			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_LIT_OFF:
						lit.mOff = (ReadUnsignedShort() > 0);
						break;
					case CHUNK_LIT_SPOT:
						readSpotChunk(lit, head.length - 6);
						break;
					case CHUNK_COL_RGB:
					case CHUNK_COL_LINRGB:
						lit.mColor = readRGBColor();
						break;
					case CHUNK_COL_TRU:
					case CHUNK_COL_LINTRU:
						lit.mColor = readTrueColor();
						break;
					case CHUNK_LIT_ATTENUATE:
						lit.mAttenuation = ReadFloat();
						break;
					case CHUNK_LIT_INNER_RANGE:
						lit.mInnerRange = ReadFloat();
						break;
					case CHUNK_LIT_OUTER_RANGE:
						lit.mOuterRange = ReadFloat();
						break;
					case CHUNK_LIT_MULTIPLIER:
						lit.mMultiplexer = ReadFloat();
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private Color3ds readRGBColor()
		{
			Color3ds lvColor = new Color3ds();

			lvColor.mRed = ReadFloat();
			lvColor.mGreen = ReadFloat();
			lvColor.mBlue = ReadFloat();

			return lvColor;
		}

		private Color3ds readTrueColor()
		{
			Color3ds lvColor = new Color3ds();

			lvColor.mRed = (float)(ReadByte() & 0xff) / 255;
			lvColor.mGreen = (float)(ReadByte() & 0xff) / 255;
			lvColor.mBlue = (float)(ReadByte() & 0xff) / 255;

			return lvColor;
		}

		private void read_N_CAMERA(string name, int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			Camera3ds cam = new Camera3ds();
			cam.mName = name;

			addCamera(cam);

			cam.mPosition.X = ReadFloat();
			cam.mPosition.Z = ReadFloat();
			cam.mPosition.Y = ReadFloat();
			cam.mTarget.X = ReadFloat();
			cam.mTarget.Z = ReadFloat();
			cam.mTarget.Y = ReadFloat();
			cam.mRoll = ReadFloat();
			cam.mLens = ReadFloat();

			if (mDecode != null)
			{
				mDecode.enter();
				mDecode.println("Position: " + cam.mPosition);
				mDecode.println("Target:   " + cam.mTarget);
				mDecode.println("Roll: " + Utils3ds.floatToString(cam.mRoll, 12));
				mDecode.println("Lens: " + Utils3ds.floatToString(cam.mLens, 12));
			}

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_CAM_RANGES:
						read_CAM_RANGES(cam);
						break;
					case CHUNK_CAM_SEE_CONE:
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private void read_CAM_RANGES(Camera3ds cam)
		{
			cam.mNearPlane = ReadFloat();
			cam.mFarPlane = ReadFloat();

			if (mDecode != null)
			{
				mDecode.enter();
				mDecode.println("Near plane:" + Utils3ds.floatToString(cam.mNearPlane, 14));
				mDecode.println("Far plane: " + Utils3ds.floatToString(cam.mFarPlane, 14));
				mDecode.leave();
			}
		}


		private void read_N_TRI_OBJECT(string name, int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			Mesh3ds mes = new Mesh3ds();
			mes.Name = name;

			addMesh(mes);

			if (mDecode != null)
			{
				mDecode.enter();
			}

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_POINT_ARRAY:
						mes.mVertex = read_POINT_ARRAY();
						break;
					case CHUNK_TEX_VERTS:
						mes.mTexCoord = read_TEX_VERTS();
						break;
					case CHUNK_MESH_TEXTURE_INFO:
						read_MESH_TEXTURE_INFO(mes);
						break;
					case CHUNK_MESH_MATRIX:
						readMatrix(mes.mLocalSystem);
						break;
					case CHUNK_FACE_ARRAY:
						read_FACE_ARRAY(mes, head.length - 6);
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private Vertex3ds[] read_POINT_ARRAY()
		{
			int verts = ReadUnsignedShort();
			Vertex3ds[] v = new Vertex3ds[verts];

			for (int n = 0; n < verts; n++)
			{
				float x = ReadFloat();
				float z = ReadFloat();
				float y = ReadFloat();
				v[n] = new Vertex3ds(x, y, z);
			}

			if (mDecode != null)
			{
				mDecode.enter();
				mDecode.println("Vertices: " + verts);
				for (int i = 0; i < verts; i++)
				{
					mDecode.println(" " + Utils3ds.intToString(i, 4) + ":  " + v[i]);
				}
				mDecode.leave();
			}

			return v;
		}

		private TexCoord3ds[] read_TEX_VERTS()
		{
			int coords = ReadUnsignedShort();
			TexCoord3ds[] tc = new TexCoord3ds[coords];

			for (int n = 0; n < coords; n++)
			{
				float u = ReadFloat();
				float v = ReadFloat();
				// Set unset u,v coordinates to 0
				if (u < -100.0f || u > 100.0f) u = 0.0f;
				if (v < -100.0f || v > 100.0f) v = 0.0f;
				tc[n] = new TexCoord3ds(u, v);
			}

			if (mDecode != null)
			{
				mDecode.enter();
				mDecode.println("Coords: " + coords);
				for (int i = 0; i < coords; i++)
				{
					mDecode.println(" " + Utils3ds.intToString(i, 4) + ":  " + tc[i]);
				}
				mDecode.leave();
			}

			return tc;
		}

		private void read_MESH_TEXTURE_INFO(Mesh3ds mes)
		{
			mes.TextureMappingMode = (TextureMappingMode3ds) ReadUnsignedShort();
			mes.mTexUTile = ReadFloat();
			mes.mTexVTile = ReadFloat();

			if (mDecode != null)
			{
				mDecode.enter();
				string type = "";
				switch (mes.TextureMappingMode)
				{
					case TextureMappingMode3ds.PlanarMap:
						type = "PLANAR";
						break;
					case TextureMappingMode3ds.CylindricalMap:
						type = "CYLINDRICAL";
						break;
					case TextureMappingMode3ds.SphericalMap:
						type = "SPHERICAL";
						break;
					default:
						type = "" + mes.TextureMappingMode;
						break;
				}
				mDecode.println("Texture mapping type: " + type);
				mDecode.println("Texture U tiling: " + Utils3ds.floatToString(mes.mTexUTile, 9));
				mDecode.println("Texture V tiling: " + Utils3ds.floatToString(mes.mTexVTile, 9));
			}

			SkipBytes(4 * 4 + (3 * 4 + 3) * 4);

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private void readMatrix(float[,] mtx)
		{
			mtx[0, 0] = ReadFloat();
			mtx[0, 2] = ReadFloat();
			mtx[0, 1] = ReadFloat();

			mtx[2, 0] = ReadFloat();
			mtx[2, 2] = ReadFloat();
			mtx[2, 1] = ReadFloat();

			mtx[1, 0] = ReadFloat();
			mtx[1, 2] = ReadFloat();
			mtx[1, 1] = ReadFloat();

			mtx[0, 3] = ReadFloat();
			mtx[2, 3] = ReadFloat();
			mtx[1, 3] = ReadFloat();

			if (mDecode != null)
			{
				mDecode.enter();
				for (int n = 0; n < 3; n++)
				{
					mDecode.println("" + Utils3ds.floatToString(mtx[n, 0], 13) +
													" " + Utils3ds.floatToString(mtx[n, 1], 13) +
													" " + Utils3ds.floatToString(mtx[n, 2], 13) +
													" " + Utils3ds.floatToString(mtx[n, 3], 13));
				}
				mDecode.leave();
			}
		}

		private void read_FACE_ARRAY(Mesh3ds mes, int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			int faces = ReadUnsignedShort();
			mes.mFace = new Face3ds[faces];

			for (int n = 0; n < faces; n++)
			{
				int p0 = ReadUnsignedShort();
				int p1 = ReadUnsignedShort();
				int p2 = ReadUnsignedShort();
				int flags = ReadUnsignedShort();
				mes.mFace[n] = new Face3ds(p0, p1, p2, flags);
			}

			if (mDecode != null)
			{
				mDecode.enter();
				mDecode.println("Faces: " + faces);
				for (int i = 0; i < faces; i++)
				{
					mDecode.println(" " + Utils3ds.intToString(i, 4) + ":  " + mes.mFace[i]);
				}
			}

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_MSH_MAT_GROUP:
						read_MSH_MAT_GROUP(mes);
						break;
					case CHUNK_SMOOTH_GROUP:
						read_SMOOTH_GROUP(mes, head.length - 6);
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private void read_MSH_MAT_GROUP(Mesh3ds mes)
		{
			string name = ReadName();

			FaceMat3ds fm = new FaceMat3ds();
			mes.addFaceMat(fm);

			for (int i = 0; i < Materials.Count(); i++)
			{
				if (Materials.ElementAt(i).name().Equals(name) == true)
				{
					fm.mMatIndex = i;
					break;
				}
			}

			int indexes = ReadUnsignedShort();
			fm.mFaceIndex = new int[indexes];

			for (int n = 0; n < indexes; n++)
			{
				fm.mFaceIndex[n] = ReadUnsignedShort();
			}

			if (mDecode != null)
			{
				mDecode.enter();
				mDecode.println("Faces: " + indexes);
				for (int t = 0; t < indexes; t++)
				{
					mDecode.println("  face: " + fm.mFaceIndex[t]);
				}
				mDecode.leave();
			}
		}

		private void read_SMOOTH_GROUP(Mesh3ds mes, int chunk_len)
		{
			int entrys = chunk_len / 4;
			mes.mSmoothGroup = new int[entrys];

			for (int n = 0; n < entrys; n++)
			{
				mes.mSmoothGroup[n] = ReadInt();
			}

			if (mDecode != null)
			{
				mDecode.enter();
				mDecode.println("Entrys: " + entrys);
				for (int i = 0; i < entrys; i++)
				{
					mDecode.println(Utils3ds.intToString(i, 4) + ": " + Utils3ds.intToBinString(mes.mSmoothGroup[i], 32));
				}
				mDecode.leave();
			}

			if (entrys != mes.faces())
			{
				throw new Exception3ds("SMOOTH_GROUP entrys != faces. File is probably corrupt!");
			}
		}

		private void read_KFDATA(int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			if (mDecode != null)
			{
				mDecode.enter();
			}

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_KFSEG:
						mStartFrame = ReadInt();
						mEndFrame = ReadInt();
						if (mDecode != null)
						{
							mDecode.enter();
							mDecode.println("Start frame: " + mStartFrame);
							mDecode.println("End frame:   " + mEndFrame);
							mDecode.leave();
						}
						break;
					case CHUNK_OBJECT_NODE_TAG:
						read_OBJECT_NODE_TAG(head.length - 6);
						break;
					case CHUNK_TARGET_NODE_TAG:
						read_TARGET_NODE_TAG(head.length - 6);
						break;
					case CHUNK_CAMERA_NODE_TAG:
						read_CAMERA_NODE_TAG(head.length - 6);
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private void read_OBJECT_NODE_TAG(int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			if (mDecode != null)
			{
				mDecode.enter();
			}

			int node_id = 0;
			string name = "";
			Mesh3ds mes = null;

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_NODE_ID:
						node_id = read_NODE_ID();
						break;
					case CHUNK_NODE_HDR:
						name = ReadName();
						mes = Meshes.FirstOrDefault(m => m.Name == name);
						if (mes == null) // TODO: This doesn't seem right.
							mes = Meshes.First();
						mes.mNodeId = node_id;
						mes.mNodeFlags = ReadInt();
						mes.mParentNodeId = ReadUnsignedShort();
						if (mDecode != null)
						{
							mDecode.enter();
							mDecode.println("Node flags: 0x" + Utils3ds.intToHexString(mes.mNodeFlags, 8));
							mDecode.println("Parent node id: " + mes.mParentNodeId);
							mDecode.leave();
						}
						// BUG: Build hierarchy here...
						break;
					case CHUNK_PIVOT:
						mes.mPivot.X = ReadFloat();
						mes.mPivot.Z = ReadFloat();
						mes.mPivot.Y = ReadFloat();
						if (mDecode != null)
						{
							mDecode.enter();
							mDecode.println(mes.mPivot.ToString());
							mDecode.leave();
						}
						break;
					case CHUNK_POS_TRACK_TAG:
						read_POS_TRACK_TAG(mes.mPositionTrack);
						break;
					case CHUNK_ROT_TRACK_TAG:
						read_ROT_TRACK_TAG(mes.mRotationTrack);
						break;
					case CHUNK_SCL_TRACK_TAG:
						read_POS_TRACK_TAG(mes.mScaleTrack);
						break;
					case CHUNK_MORPH_TRACK_TAG:
						read_MORPH_TRACK_TAG(mes.mMorphTrack);
						break;
					case CHUNK_HIDE_TRACK_TAG:
						read_HIDE_TRACK_TAG(mes.mHideTrack);
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private void read_TARGET_NODE_TAG(int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			if (mDecode != null)
			{
				mDecode.enter();
			}

			int target_node_id = 0;
			string name = "";
			Camera3ds cam = null;

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_NODE_ID:
						target_node_id = read_NODE_ID();
						break;
					case CHUNK_NODE_HDR:
						name = ReadName();
						cam = Cameras.FirstOrDefault(c => c.name() == name);
						if (cam == null) // TODO: This doesn't seem right.
							cam = Cameras.First();
						cam.mTargetNodeId = target_node_id;
						cam.mTargetNodeFlags = ReadInt();
						cam.mTargetParentNodeId = ReadUnsignedShort();
						if (mDecode != null)
						{
							mDecode.enter();
							mDecode.println("Target node flags: 0x" + Utils3ds.intToHexString(cam.mTargetNodeFlags, 8));
							mDecode.println("Target parent node id: " + cam.mTargetParentNodeId);
							mDecode.leave();
						}
						// BUG: Build hierarchy here...
						break;
					case CHUNK_POS_TRACK_TAG:
						read_POS_TRACK_TAG(cam.mTargetTrack);
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private void read_CAMERA_NODE_TAG(int chunk_len)
		{
			long chunk_end = filePos() + chunk_len;

			if (mDecode != null)
			{
				mDecode.enter();
			}

			int position_node_id = 0;
			string name = "";
			Camera3ds cam = null;

			while (filePos() < chunk_end)
			{
				Head head = read_HEAD();
				switch (head.id)
				{
					case CHUNK_NODE_ID:
						position_node_id = read_NODE_ID();
						break;
					case CHUNK_NODE_HDR:
						name = ReadName();
						cam = Cameras.First(c => c.name() == name);
						cam.mPositionNodeId = position_node_id;
						cam.mPositionNodeFlags = ReadInt();
						cam.mPositionParentNodeId = ReadUnsignedShort();
						if (mDecode != null)
						{
							mDecode.enter();
							mDecode.println("Position node flags: 0x" + Utils3ds.intToHexString(cam.mPositionNodeFlags, 8));
							mDecode.println("Position parent node id: " + cam.mPositionParentNodeId);
							mDecode.leave();
						}
						// BUG: Build hierarchy here...
						break;
					case CHUNK_POS_TRACK_TAG:
						read_POS_TRACK_TAG(cam.mPositionTrack);
						break;
					case CHUNK_FOV_TRACK_TAG:
						readPTrack(cam.mFovTrack);
						break;
					case CHUNK_ROLL_TRACK_TAG:
						readPTrack(cam.mRollTrack);
						break;
					default:
						SkipChunk(head.length - 6);
						break;
				}
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private int read_NODE_ID()
		{
			int id = ReadUnsignedShort();

			if (mDecode != null)
			{
				mDecode.enter();
				mDecode.println("Node id: " + id);
				mDecode.leave();
			}

			return id;
		}

		private int readTrackHead(Track3ds track)
		{
			int keys, flags;

			track.mFlags = flags = ReadUnsignedShort();

			if (mDecode != null)
			{
				string loop;
				switch (track.loopType())
				{
					case Track3ds.SINGLE: loop = "SINGLE"; break;
					case Track3ds.REPEAT: loop = "REPEAT"; break;
					case Track3ds.LOOP: loop = "LOOP"; break;
					default: loop = "" + track.loopType(); break;
				}
				mDecode.println("Loop type: " + loop);
				mDecode.println("Flags: lock " +
						(((flags & Track3ds.LOCK_X) == Track3ds.LOCK_X) ? "X " : "- ") +
						(((flags & Track3ds.LOCK_Y) == Track3ds.LOCK_Y) ? "Y " : "- ") +
						(((flags & Track3ds.LOCK_Z) == Track3ds.LOCK_Z) ? "Z " : "- ") +
						"  unlink " +
						(((flags & Track3ds.UNLINK_X) == Track3ds.UNLINK_X) ? "X " : "- ") +
						(((flags & Track3ds.UNLINK_Y) == Track3ds.UNLINK_Y) ? "Y " : "- ") +
						(((flags & Track3ds.UNLINK_Z) == Track3ds.UNLINK_Z) ? "Z " : "- "));
			}

			SkipBytes(2 * 4);

			keys = ReadInt();

			if (mDecode != null)
			{
				mDecode.println("Keys: " + keys);
			}

			return keys;
		}

		private void readSplineParams(SplineKey3ds key)
		{
			int flags = ReadUnsignedShort();

			if (flags != 0)
			{
				if ((flags & 0x01) != 0)
				{
					key.Tension = ReadFloat();
					if (mDecode != null)
					{
						mDecode.println("    Tension:    " + Utils3ds.floatToString(key.Tension, 7));
					}
				}
				if ((flags & 0x02) != 0)
				{
					key.Bias = ReadFloat();
					if (mDecode != null)
					{
						mDecode.println("    Bias:       " + Utils3ds.floatToString(key.Bias, 7));
					}
				}
				if ((flags & 0x04) != 0)
				{
					key.Continuity = ReadFloat();
					if (mDecode != null)
					{
						mDecode.println("    Continuity: " + Utils3ds.floatToString(key.Continuity, 7));
					}
				}
				if ((flags & 0x08) != 0)
				{
					key.EaseTo = ReadFloat();
					if (mDecode != null)
					{
						mDecode.println("    Ease to:    " + Utils3ds.floatToString(key.EaseTo, 7));
					}
				}
				if ((flags & 0x10) != 0)
				{
					key.EaseFrom = ReadFloat();
					if (mDecode != null)
					{
						mDecode.println("    Ease from:  " + Utils3ds.floatToString(key.EaseFrom, 7));
					}
				}
			}
		}

		private void readPTrack(PTrack3ds track)
		{
			if (mDecode != null)
			{
				mDecode.enter();
			}

			int keys = readTrackHead(track);
			track.mKey = new PKey3ds[keys];

			for (int i = 0; i < keys; i++)
			{
				PKey3ds key = new PKey3ds();
				key.Frame = ReadInt();
				if (mDecode != null)
				{
					mDecode.println("  Frame: " + key.Frame);
				}
				readSplineParams(key);
				key.P = ReadFloat();
				if (mDecode != null)
				{
					mDecode.println("  " + Utils3ds.floatToString(key.P, 13));
				}
				track.mKey[i] = key;
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private void read_POS_TRACK_TAG(XYZTrack3ds track)
		{
			if (mDecode != null)
			{
				mDecode.enter();
			}

			int keys = readTrackHead(track);
			track.mKey = new XYZKey3ds[keys];

			for (int i = 0; i < keys; i++)
			{
				XYZKey3ds key = new XYZKey3ds();
				key.Frame = ReadInt();
				if (mDecode != null)
				{
					mDecode.println("  Frame: " + key.Frame);
				}
				readSplineParams(key);
				key.X = ReadFloat();
				key.Z = ReadFloat();
				key.Y = ReadFloat();
				if (mDecode != null)
				{
					mDecode.println("    X Y Z:" +
														Utils3ds.floatToString(key.X, 13) +
														Utils3ds.floatToString(key.Y, 13) +
														Utils3ds.floatToString(key.Z, 13));
				}
				track.mKey[i] = key;
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private void read_ROT_TRACK_TAG(RotationTrack3ds track)
		{
			if (mDecode != null)
			{
				mDecode.enter();
			}

			int keys = readTrackHead(track);
			track.mKey = new RotationKey3ds[keys];

			for (int i = 0; i < keys; i++)
			{
				RotationKey3ds key = new RotationKey3ds();
				key.Frame = ReadInt();
				if (mDecode != null)
				{
					mDecode.println("  Frame: " + key.Frame);
				}
				readSplineParams(key);
				key.A = ReadFloat();
				key.X = ReadFloat();
				key.Z = ReadFloat();
				key.Y = ReadFloat();
				if (mDecode != null)
				{
					mDecode.println("    A X Y Z:" +
														Utils3ds.floatToString(key.A, 12) +
														Utils3ds.floatToString(key.X, 9) +
														Utils3ds.floatToString(key.Y, 9) +
														Utils3ds.floatToString(key.Z, 9));
				}
				track.mKey[i] = key;
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private void read_MORPH_TRACK_TAG(MorphTrack3ds track)
		{
			if (mDecode != null)
			{
				mDecode.enter();
			}

			int keys = readTrackHead(track);
			track.mKey = new MorphKey3ds[keys];

			for (int i = 0; i < keys; i++)
			{
				MorphKey3ds key = new MorphKey3ds();
				key.Frame = ReadInt();
				if (mDecode != null)
				{
					mDecode.println("  Frame: " + key.Frame);
				}
				readSplineParams(key);
				string name = ReadName();

				for (int n = 0; n < Meshes.Count(); n++)
				{
					if (Meshes.ElementAt(n).Name == name)
					{
						key.Mesh = n;
						break;
					}
				}

				track.mKey[i] = key;
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}

		private void read_HIDE_TRACK_TAG(HideTrack3ds track)
		{
			if (mDecode != null)
			{
				mDecode.enter();
			}

			SplineKey3ds dummy = new SplineKey3ds();
			int keys = readTrackHead(track);
			track.mKey = new HideKey3ds[keys];

			for (int i = 0; i < keys; i++)
			{
				HideKey3ds key = new HideKey3ds();
				key.Frame = ReadInt();
				if (mDecode != null)
				{
					mDecode.println("  Frame: " + key.Frame);
				}
				readSplineParams(dummy);
				track.mKey[i] = key;
			}

			if (mDecode != null)
			{
				mDecode.leave();
			}
		}


		const int
			CHUNK_COL_RGB = 0x0010,
			CHUNK_COL_TRU = 0x0011,
			CHUNK_COL_LINRGB = 0x0012,
			CHUNK_COL_LINTRU = 0x0013,

			CHUNK_PERCENTW = 0x0030,		// int2   percentage
		CHUNK_PERCENTF = 0x0031,		// float4  percentage

			CHUNK_M3DMAGIC = 0x4D4D,
				CHUNK_MDATA = 0x3D3D,
					CHUNK_MAT_ENTRY = 0xAFFF,
						CHUNK_MAT_NAME = 0xA000,
						CHUNK_MAT_AMBIENT = 0xA010,
						CHUNK_MAT_DIFFUSE = 0xA020,
						CHUNK_MAT_SPECULAR = 0xA030,
						CHUNK_MAT_SHININESS = 0xA041,
						CHUNK_MAT_TRANSPARENCY = 0xA050,
						CHUNK_MAT_MAP = 0xA200,
						CHUNK_MAT_MAPNAME = 0xA300,

						// Scaling in U/V direction.
			// (need to gen separate UV coordinate set 
			// and do this by hand)
			CHUNK_MAT_MAP_USCALE = 0xA354,
			CHUNK_MAT_MAP_VSCALE = 0xA356,

			// Translation in U/V direction.
			// (need to gen separate UV coordinate set 
			// and do this by hand)
			CHUNK_MAT_MAP_UOFFSET = 0xA358,
			CHUNK_MAT_MAP_VOFFSET = 0xA35a,

			// UV-coordinates rotation around the z-axis
			// Assumed to be in radians.
			CHUNK_MAT_MAP_ANG = 0xA35C,

			// Tiling flags for 3DS files
			CHUNK_MAT_MAP_TILING = 0xa351,

					CHUNK_NAMED_OBJECT = 0x4000,
						CHUNK_N_TRI_OBJECT = 0x4100,
							CHUNK_POINT_ARRAY = 0x4110,
							CHUNK_TEX_VERTS = 0x4140,
							CHUNK_MESH_TEXTURE_INFO = 0x4170,
							CHUNK_MESH_MATRIX = 0x4160,
							CHUNK_MESH_COLOR = 0x4165,
							CHUNK_FACE_ARRAY = 0x4120,
								CHUNK_MSH_MAT_GROUP = 0x4130,
								CHUNK_SMOOTH_GROUP = 0x4150,
						CHUNK_N_LIGHT = 0x4600,
							CHUNK_LIT_SPOT = 0x4610,
							CHUNK_LIT_OFF = 0x4620,
							CHUNK_LIT_ATTENUATE = 0x4625,
							CHUNK_LIT_RAYSHAD = 0x4627,
							CHUNK_LIT_SHADOWED = 0x4630,
							CHUNK_LIT_LOCAL_SHADOW = 0x4640,
							CHUNK_LIT_LOCAL_SHADOW2 = 0x4641,
							CHUNK_LIT_SEE_CONE = 0x4650,
							CHUNK_LIT_SPOT_RECTANGULAR = 0x4651,
							CHUNK_LIT_SPOT_OVERSHOOT = 0x4652,
							CHUNK_LIT_SPOT_PROJECTOR = 0x4653,
							CHUNK_LIT_SPOT_RANGE = 0x4655,
							CHUNK_LIT_SPOT_ROLL = 0x4656,
							CHUNK_LIT_SPOT_ASPECT = 0x4657,
							CHUNK_LIT_RAY_BIAS = 0x4658,
							CHUNK_LIT_INNER_RANGE = 0x4659,
							CHUNK_LIT_OUTER_RANGE = 0x465A,
							CHUNK_LIT_MULTIPLIER = 0x465B,
						CHUNK_N_CAMERA = 0x4700,
							CHUNK_CAM_SEE_CONE = 0x4710,
							CHUNK_CAM_RANGES = 0x4720,
				CHUNK_KFDATA = 0xB000,
					CHUNK_KFSEG = 0xB008,
					CHUNK_OBJECT_NODE_TAG = 0xB002,
						CHUNK_NODE_ID = 0xB030,
						CHUNK_NODE_HDR = 0xB010,
						CHUNK_PIVOT = 0xB013,
						CHUNK_POS_TRACK_TAG = 0xB020,
						CHUNK_ROT_TRACK_TAG = 0xB021,
						CHUNK_SCL_TRACK_TAG = 0xB022,
						CHUNK_MORPH_TRACK_TAG = 0xB026,
						CHUNK_HIDE_TRACK_TAG = 0xB029,
					CHUNK_TARGET_NODE_TAG = 0xB004,
					CHUNK_CAMERA_NODE_TAG = 0xB003,
						CHUNK_FOV_TRACK_TAG = 0xB023,
						CHUNK_ROLL_TRACK_TAG = 0xB024;
		//        CHUNK_AMBIENT_NODE_TAG        = 0xB001;

	}
}