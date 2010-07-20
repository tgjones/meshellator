/******************************************************************************

Copyright NVIDIA Corporation 2008
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY
LOSS) ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF
NVIDIA HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.

YOU ("DEVELOPER") SHALL HAVE THE RIGHT TO MODIFY AND CREATE DERIVATIVE WORKS 
WITH THE SOURCE CODE. DEVELOPER SHALL OWN ANY DERIVATIVE WORKS ("DERIVATIVES") 
IT CREATES TO THE SOURCE CODE, PROVIDED THAT DEVELOPER USES THE MATERIALS IN 
ACCCORDANCE WITH THE TERMS OF THIS AGREEMENT. DEVELOPER MAY DISTRIBUTE THE 
DERIVATIVES, PROVIDED THAT ALL NVIDIA COPYRIGHT NOTICES AND TRADEMARKS ARE 
USED PROPERLY AND THE DERIVATIVES INCLUDE THE FOLLOWING STATEMENT: 
"This software contains source code provided by NVIDIA Corporation."  

DEVELOPER AGREES NOT TO DISASSEMBLE, DECOMPILE OR REVERSE ENGINEER THE OBJECT 
CODE VERSIONS OF ANY OF THE PROVIDED MATERIALS. DEVELOPER ACKNOWLEDGES THAT 
CERTAIN OF THE MATERIALS PROVIDED IN OBJECT CODE VERSION MAY CONTAIN THIRD PARTY 
COMPONENTS THAT MAY BE SUBJECT TO RESTRICTIONS, AND EXPRESSLY AGREES NOT TO
ATTEMPT TO MODIFY OR DISTRIBUTE SUCH MATERIALS WITHOUT FIRST RECEIVING CONSENT 
FROM NVIDIA.

To learn more about working with the FX Composer SDK, visit the NVIDIA FX Composer 
Forums at: http://developer.nvidia.com/forums/

******************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Nexus;

namespace Satis.Importers.Autodesk3ds.Autodesk3ds
{
	/// <summary>
	/// A Chunk3DS is the basic unit for containing information in a 3DS file.
	/// It contains an identifier and length (length includes the id).
	/// </summary>
	internal struct Chunk3DS
	{
		public UInt16 id;       // ID of this chunk
		public int length;      // Length of this chunk
		public long start;      // The starting byte of this chunk
		public long end;        // The ending byte of this chunk

		/// <summary>
		/// Obtain the numbers of bytes read from this chunk.
		/// </summary>
		/// <param name="binaryStreamPos">Position of the binary read stream.</param>
		/// <returns>The number of bytes read into this chunk so far.</returns>
		public int GetBytesRead(long binaryStreamPos)
		{
			if (binaryStreamPos <= this.start)
			{
				return 0;
			}
			return (int) (binaryStreamPos - this.start);
		}
	}

	/// <summary>
	/// Holds a byte-based r,g,b colour as read from a 3DS file.
	/// </summary>
	internal struct ByteColour3DS
	{
		public byte r, g, b;
		public ByteColour3DS(byte r, byte g, byte b)
		{
			this.r = r; this.g = g; this.b = b;
		}
		public override string ToString()
		{
			return "(" + r + ", " + g + ", " + b + ")";
		}
		public Color ToColor()
		{
			return new Color(r, g, b);
		}
		public static float ConvertToFloatColour(byte colour)
		{
			return (float) (colour) / (float) (255);
		}
	}

	/// <summary>
	/// Holds a float-based r,g,b colour as read from a 3DS file.
	/// </summary>
	internal struct FloatColour3DS
	{
		public float r, g, b;
		public FloatColour3DS(float r, float g, float b)
		{
			this.r = r; this.g = g; this.b = b;
		}
		public override string ToString()
		{
			return "(" + r + ", " + g + ", " + b + ")";
		}

		public static ByteColour3DS FloatToByteColour(FloatColour3DS floatColour)
		{
			return new ByteColour3DS((byte) (255.0f * floatColour.r), (byte) (255.0f * floatColour.g), (byte) (255.0f * floatColour.b));
		}
	}

	/// <summary>
	/// Holds a vertex as read from a 3DS file.
	/// </summary>
	internal struct Vertex3DS
	{
		internal const float EPSILON = 0.000001f;
		public float x, y, z;
		public Vertex3DS(float x, float y, float z)
		{
			this.x = x; this.y = y; this.z = z;
		}
		/// <summary>
		/// Figures out if a given vertex is equal to this with a margin of
		/// floating point error.
		/// </summary>
		/// <param name="v">Vertex we are checking for equivalence.</param>
		/// <returns>True if equal, false otherwise.</returns>
		public bool EqualToWithEpsilon(Vertex3DS v)
		{
			if (Math.Abs(v.x - this.x) < EPSILON && Math.Abs(v.y - this.y) < EPSILON && Math.Abs(v.z - this.z) < EPSILON)
			{
				return true;
			}
			return false;
		}
		public override string ToString()
		{
			return "(" + x + ", " + y + ", " + z + ")";
		}
		/// <summary>
		/// Cross Product of two 3D verticies/vectors.
		/// </summary>
		/// <param name="lhs">Left hand side vector.</param>
		/// <param name="rhs">Right hand side vector.</param>
		/// <returns>Cross product of lhs x rhs.</returns>
		public static Vertex3DS Cross(Vertex3DS lhs, Vertex3DS rhs)
		{
			Vertex3DS v = new Vertex3DS(lhs.y * rhs.z - lhs.z * rhs.y,
																	lhs.z * rhs.x - lhs.x * rhs.z,
																	lhs.x * rhs.y - lhs.y * rhs.x);
			return v;
		}
		/// <summary>
		/// Normalizes this vertex.
		/// </summary>
		public static Vertex3DS Normalize(Vertex3DS v)
		{
			float norm = (float) Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
			if (norm == 0.0f)
				return new Vertex3DS(0, 0, 0);

			Vertex3DS newV = new Vertex3DS();
			newV.x = v.x / norm;
			if (Math.Abs(newV.x) < EPSILON)
			{
				newV.x = 0.0f;
			}

			newV.y = v.y / norm;
			if (Math.Abs(newV.y) < EPSILON)
			{
				newV.y = 0.0f;
			}

			newV.z = v.z / norm;
			if (Math.Abs(newV.z) < EPSILON)
			{
				newV.z = 0.0f;
			}

			return newV;
		}
		public static Vertex3DS operator -(Vertex3DS lhs, Vertex3DS rhs)
		{
			Vertex3DS v = new Vertex3DS(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
			return v;
		}
		public static Vertex3DS operator +(Vertex3DS lhs, Vertex3DS rhs)
		{
			Vertex3DS v = new Vertex3DS(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
			return v;
		}
	}

	/// <summary>
	/// Holds texture coordinates as read from a 3DS file.
	/// </summary>
	internal struct UVCoord3DS
	{
		public float u, v;
		public override string ToString()
		{
			return "(" + u + ", " + v + ")";
		}
	}

	/// <summary>
	/// Holds a triangle face as read from a 3DS file.
	/// </summary>
	internal struct TriFace3DS
	{
		public ushort[] vertInd;   // Indices of vertices that make up this face
		public ushort flag;
		//public long smoothGrp;      // The smoothing group of this face

		public TriFace3DS(ushort v1, ushort v2, ushort v3, ushort flag)
		{
			this.vertInd = new ushort[3];
			this.vertInd[0] = v1;
			this.vertInd[1] = v2;
			this.vertInd[2] = v3;
			this.flag = flag;
		}
		public override string ToString()
		{
			return "V1: " + this.vertInd[0] + ", V2: " + this.vertInd[1] + ", V3: " + this.vertInd[2];
		}
	}


	/// <summary>
	/// Holds the assets of a 3DS file in internal structs/classes for later interpretation.
	/// </summary>
	internal class Assets3DS
	{
		public int versionNum;
		public float masterScale;
		private List<Material3DS> _materials = new List<Material3DS>();
		private List<TriMeshObject3DS> _triMeshes = new List<TriMeshObject3DS>();
		private List<LightObject3DS> _lights = new List<LightObject3DS>();
		private List<CameraObject3DS> _cameras = new List<CameraObject3DS>();

		public List<Material3DS> materials
		{
			get { return this._materials; }
		}
		public List<TriMeshObject3DS> triMeshes
		{
			get { return this._triMeshes; }
		}
		public List<LightObject3DS> lights
		{
			get { return this._lights; }
		}
		public List<CameraObject3DS> cameras
		{
			get { return this._cameras; }
		}

		public Assets3DS()
		{
			// Add the default material
			this._materials.Add(Material3DS.GetDefaultMaterial());
		}

		public override string ToString()
		{
			string materialsList = "";
			string triMeshList = "";
			string lightList = "";
			string camList = "";

			int count = 0;
			foreach (Material3DS m in this._materials)
			{
				materialsList += "---Material #" + count + "---\n" + m + "\n";
				count++;
			}
			materialsList += "Total Material Count: " + count + "\n";
			count = 0;
			foreach (TriMeshObject3DS t in this._triMeshes)
			{
				triMeshList += "---TriMesh #" + count + "---\n" + t + "\n";
				count++;
			}
			triMeshList += "Total Geometry Count: " + count + "\n";
			count = 0;
			foreach (LightObject3DS l in this._lights)
			{
				lightList += "---Light #" + count + "---\n" + l + "\n";
				count++;
			}
			lightList += "Total Light Count: " + count + "\n";
			count = 0;
			foreach (CameraObject3DS c in this._cameras)
			{
				camList += "---Camera #" + count + "---\n" + c + "\n";
				count++;
			}
			camList += "Total Camera Count: " + count + "\n";

			return "****** 3DS File Assets ******\n" +
							"File Version: " + this.versionNum + "\n" +
							"Master Scale: " + this.masterScale + "\n\n" +
							"Material Listing:\n" + materialsList + "\n" +
							"Light Listing:\n" + lightList + "\n" +
							"Camera Listing:\n" + camList + "\n" +
							"Geometry Listing:\n" + triMeshList;
		}

		/// <summary>
		/// Add a new material to the list of materials in the assets.
		/// </summary>
		/// <param name="mat">A new material.</param>
		public void AddMaterial(Material3DS mat)
		{
			// Make sure the material is unique
			foreach (Material3DS m in this._materials)
			{
				if (m.name.Equals(mat.name))
					throw new InvalidDataException("Duplicate material name - names must be unique.");
			}
			this._materials.Add(mat);
		}
		/// <summary>
		/// Adds a tri-mesh object to the list of tri meshes in the assets.
		/// </summary>
		/// <param name="tmObj">A new triangluar mesh.</param>
		public void AddTriMeshObject(TriMeshObject3DS tmObj)
		{
			// Make sure the object is unique
			foreach (TriMeshObject3DS o in this._triMeshes)
			{
				if (o.name.Equals(tmObj.name))
					throw new InvalidDataException("Duplicate triangluar mesh name - names must be unique.");
			}
			this._triMeshes.Add(tmObj);
		}
		/// <summary>
		/// Adds a light object to the list of lights in the assets.
		/// </summary>
		/// <param name="lightObj">A new light.</param>
		public void AddLightObject(LightObject3DS lightObj)
		{
			this._lights.Add(lightObj);
		}
		/// <summary>
		/// Adds a camera object to the list of cameras in the assets.
		/// </summary>
		/// <param name="cameraObj">A new camera.</param>
		public void AddCameraObject(CameraObject3DS cameraObj)
		{
			this._cameras.Add(cameraObj);
		}
	}

	/// <summary>
	/// Holds texture mapping information derived from the 3DS format.
	/// </summary>
	internal class TextureMap3DS
	{
		public string fileName;         // File name for the texture
		public float uScale, vScale;    // UV scaling
		public float uOffset, vOffset;	// UV offsets
		public float rotAngle;					// Rotation angle (in degrees)

		public TextureMap3DS()
		{
			this.fileName = "";
			this.uScale = 1;
			this.vScale = 1;
			this.uOffset = 0;
			this.vOffset = 0;
			this.rotAngle = 0;
		}

		public override string ToString()
		{
			return this.fileName;
		}
	}

	/// <summary>
	/// Holds the attributes of a 3DS Material as read from a 3DS file.
	/// </summary>
	internal class Material3DS
	{
		public const string DEFAULT_MAT_NAME = "Default_Material";

		public enum ShadingStyle
		{
			WireFrame = 0,
			Flat = 1,
			Gouraud = 2,
			Phong = 3,
			Metal = 4
		}

		public string name;                      // Name of the material
		public ByteColour3DS ambient;            // Ambient colour
		public ByteColour3DS diffuse;            // Diffuse colour
		public ByteColour3DS specular;           // Specular colour
		public short shininessRatio;
		public short transparencyRatio;
		public ShadingStyle shading;
		public TextureMap3DS diffuseMap;
		public TextureMap3DS specularMap;

		public Material3DS()
		{
			this.name = "";
			this.ambient = new ByteColour3DS(255, 255, 255);
			this.diffuse = new ByteColour3DS(255, 255, 255);
			this.shininessRatio = 100;
			this.transparencyRatio = 0;
			this.shading = ShadingStyle.Phong;
			this.diffuseMap = null;
			this.specularMap = null;
		}

		/// <summary>
		/// Creates a default material and returns it.
		/// </summary>
		/// <returns>A default material.</returns>
		public static Material3DS GetDefaultMaterial()
		{
			Material3DS defaultMat = new Material3DS();
			defaultMat.name = Material3DS.DEFAULT_MAT_NAME;
			return defaultMat;
		}

		public override string ToString()
		{
			return "Name: " + this.name + "\n" +
						 "Ambient: " + this.ambient + " Diffuse: " + this.diffuse + " Specular: " + this.specular +
						 "\nShininess: " + shininessRatio + " Transparency: " + this.transparencyRatio + " Shading Style: " + this.shading +
						 "\nDiffuse Texture: " + this.diffuseMap + " Specular Texture: " + this.specularMap;
		}

		/// <summary>
		/// Converts a value in range 0 to 100 into range 0 to 1.
		/// </summary>
		/// <param name="val">Value to convert.</param>
		/// <returns>Converted value.</returns>
		public static float ConvertPctToFloat(short val)
		{
			return (float) (val) / 100.0f;
		}

		/// <summary>
		/// Converts a value in the range 0 to 100 to a value in the range 0 to 128.
		/// </summary>
		/// <param name="shiny">Value to convert.</param>
		/// <returns>Converted value.</returns>
		public static int ConvertPctShininessToPhong(short shiny)
		{
			return (int) (((float) shiny) * 1.28f);
		}
	}

	/// <summary>
	/// Represents a set of faces associated with a particular material.
	/// </summary>
	internal struct MaterialGroup3DS
	{
		public Material3DS material;			// Reference to the material that will be applied
		public List<ushort> faceIndices;	// The indices of faces to apply the material to

		public MaterialGroup3DS(Assets3DS assets, string matName)
		{
			Debug.Assert(assets != null);
			this.material = null;
			List<Material3DS> materials = assets.materials;
			foreach (Material3DS mat in materials)
			{
				if (mat.name == matName)
				{
					this.material = mat;
				}
			}
			if (this.material == null)
			{
				throw new InvalidDataException("Unknown material name referenced - " +
																			 "materials referenced in meshes must be previously defined.");
			}
			this.faceIndices = new List<ushort>();
		}
		public MaterialGroup3DS(Material3DS material)
		{
			this.material = material;
			this.faceIndices = new List<ushort>();
		}
		public override string ToString()
		{
			return this.material.name;
		}
	}


	/// <summary>
	/// Represents a triangular mesh geometry derived from the 3DS format.
	/// </summary>
	internal class TriMeshObject3DS
	{
		public string name;                 // Name of the object

		public List<Vertex3DS> verts;       // List of vertices (in global coords)
		public List<UVCoord3DS> texCoords;  // List of texture coordinates for each vertex
		public List<Vertex3DS> normals;     // List of normals for each vertex (Generated using smoothing groups)

		public List<TriFace3DS> faces;      // List of faces (each face holds indices of verts)
		public List<long> smoothingGrps;    // Smoothing group number for each face

		public List<MaterialGroup3DS> materialGrps;	// What materials to apply to what faces

		public float[,] orientMatrix;       // Orientation matrix (3x4)

		public TriMeshObject3DS(string name)
		{
			this.name = name;
			this.verts = new List<Vertex3DS>();
			this.faces = new List<TriFace3DS>();
			this.texCoords = new List<UVCoord3DS>();
			this.normals = new List<Vertex3DS>();
			this.materialGrps = new List<MaterialGroup3DS>();
			this.smoothingGrps = new List<long>();
			this.orientMatrix = new float[3, 4];
		}

		public override string ToString()
		{
			string materialGrpList = "Material Groups: ";
			foreach (MaterialGroup3DS matGrp in this.materialGrps)
			{
				materialGrpList += matGrp.material.name + " ";
			}

			return "Name: " + this.name + ", Number of Vertices: " + this.verts.Count +
						 ", Number of Faces: " + this.faces.Count + "\n" + materialGrpList;
		}


		/// <summary>
		/// This will ensure that the material groups on this mesh cover all the faces.
		/// In cases where they don't already, the default material is applied.
		/// </summary>
		public void FinalizeMaterials(Assets3DS assets)
		{
			// Track which faces have a material group associated with them
			bool[] hasMatGrp = new bool[this.faces.Count];
			for (ushort i = 0; i < hasMatGrp.Length; i++)
			{
				hasMatGrp[i] = false;
			}

			foreach (MaterialGroup3DS matGrp in this.materialGrps)
			{
				foreach (ushort faceIndex in matGrp.faceIndices)
				{
					hasMatGrp[faceIndex] = true;
				}
			}

			// For all faces without a material group assign a new default
			// material group to that face

			bool needsDefaultMat = false;
			for (ushort i = 0; i < hasMatGrp.Length; i++)
			{
				if (!hasMatGrp[i])
				{
					needsDefaultMat = true;
					break;
				}
			}
			if (!needsDefaultMat)
			{
				return;
			}
			MaterialGroup3DS defaultMatGrp;
			defaultMatGrp = new MaterialGroup3DS(assets, Material3DS.DEFAULT_MAT_NAME);
			this.materialGrps.Add(defaultMatGrp);
			for (ushort i = 0; i < hasMatGrp.Length; i++)
			{
				if (!hasMatGrp[i])
				{
					defaultMatGrp.faceIndices.Add(i);
				}
			}

		}
	}

	/// <summary>
	/// Represents a light as derived from the 3DS file format.
	/// </summary>
	internal class LightObject3DS
	{
		public string name;							// Name of the light
		public Vertex3DS position;      // Position of the light
		public Vertex3DS pointsAt;      // Direction, only applies if this is a spotlight
		public FloatColour3DS colour;   // Colour of the light
		public bool isSpotlight;        // Is this light a spotlight?
		public float cutoffAngle;				// Only applies to spotlight

		public LightObject3DS(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return "Name: " + this.name + ", Position: " + this.position + ", Is Spotlight: " + this.isSpotlight;
		}

	}

	internal class CameraObject3DS
	{
		public string name;						// Name of the camera
		public Vertex3DS position;		// Position of the camera
		public Vertex3DS pointsAt;		// Target of the camera

		public CameraObject3DS(string name)
		{
			this.name = name;
			this.position = new Vertex3DS(0, 0, 0);
			this.pointsAt = new Vertex3DS(0, 0, 0);
		}

		public override string ToString()
		{
			return "Name: " + this.name + ", Position: " + this.position + "\nPoints At: " + this.pointsAt;
		}
	}
}