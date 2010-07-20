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
using System.IO;
using Satis.Importers.Autodesk3ds.Autodesk3ds;
using Satis.Logging;

namespace Satis.Importers.Autodesk3ds.FileFormat
{
	internal class Reader3ds
	{
		private readonly ILogger _logger;

		#region constants

		private const ushort PrimaryChunk = 0x4D4D;   // Identifies the chunk that contains all others
		private const ushort VersionChunk = 0x0002;   // Identifies the Version of the file
		private const ushort EditorChunk = 0x3D3D;   // Start of the editor config
		private const ushort KeyframerChunk = 0xB000;   // Start of the keyframer config
		private const ushort MasterScaleChunk = 0x0100;   // Scale factor for all geometry coords
		private const ushort MeshVersionChunk = 0x3D3E;

		// Keyframe chunks


		// Material chunks
		private const ushort MatChunk = 0xAFFF;
		private const ushort MatNameChunk = 0xA000;
		private const ushort MatAmbientChunk = 0xA010;
		private const ushort MatDiffuseChunk = 0xA020;
		private const ushort MatSpecularChunk = 0xA030;
		private const ushort MatShinRatioChunk = 0xA040;
		private const ushort MatShinStrChunk = 0xA041;
		private const ushort MatXPRatioChunk = 0xA050;
		private const ushort MatXPFalloffChunk = 0xA052;
		private const ushort MatRefBlurChunk = 0xA053;
		private const ushort MatShadingChunk = 0xA100;
		private const ushort MatUseRefBlurChunk = 0xA250;
		private const ushort MatSelfIllumChunk = 0xA080;
		private const ushort MatTwoSidedChunk = 0xA081;
		private const ushort MatDecalChunk = 0xA082;
		private const ushort MatAdditiveXPChunk = 0xA083;
		private const ushort MatWireframeChunk = 0xA085;
		//private const ushort MAT_FACEMAP_CHUNK      = 0xA088;
		//private const ushort MAT_PHONGSOFT_CHUNK    = 0xA08C;
		//private const ushort MAT_ABSWIREUNITS_CHUNK = 0xA08E;
		//private const ushort MAT_WIRESIZE_CHUNK = 0xA087;
		private const ushort MatTextureMap1Chunk = 0xA200;
		//private const ushort MAT_TEXMAP2_CHUNK = 0xA33A;
		private const ushort MatSpecularMapChunk = 0xA204;


		// Object Chunks
		private const ushort ObjChunk = 0x4000;
		private const ushort ObjTriMeshChunk = 0x4100;
		private const ushort ObjLightChunk = 0x4600;
		private const ushort ObjCameraChunk = 0x4700;


		// Tri-mesh Chunks
		private const ushort ObjTMVArrayChunk = 0x4110;   // Defines the list of vertices
		private const ushort ObjTMFlagArrayChunk = 0x4111;
		private const ushort ObjTMFaceArrayChunk = 0x4120;
		private const ushort ObjTMMatGrpChunk = 0x4130;
		private const ushort ObjTMSmoothGrpChunk = 0x4150;
		//private const ushort OBJ_TM_MATCUBE_CHUNK   = 0x4190;
		private const ushort ObjTMTexCoordsChunk = 0x4140;
		private const ushort ObjTMMatrixChunk = 0x4160;
		//private const ushort OBJ_TM_MESHCOL_CHUNK   = 0x4165; // NOT PERTINENT
		//private const ushort OBJ_TM_TEXINFO_CHUNK   = 0x4170; // NOT PERTINENT
		//private const ushort OBJ_TM_PROCNAME_CHUNK  = 0x4181; // NOT PERTINENT
		//private const ushort OBJ_TM_PROCDATA_CHUNK  = 0x4182; // NOT PERTINENT

		// Light Chunks
		private const ushort ObjLightOuterRangeChunk = 0x465A;
		private const ushort ObjLightInnerRangeChunk = 0x4659;
		private const ushort ObjLightMultiplierChunk = 0x465B;
		//private const ushort OBJ_LIGHT_EXCLUDE    = 0x4654; //...?
		private const ushort ObjLightIsSpotChunk = 0x4610;
		//private const ushort OBJ_LIGHT_ATTENUATE    = 0x4625; // ... has attenuation?
		// Spotlight stuff...


		// Texture Chunks
		private const ushort MatTexNameChunk = 0xA300;
		private const ushort MatTexTilingChunk = 0xA351;
		private const ushort MatTexBlurChunk = 0xA353;
		private const ushort MatTexUScaleChunk = 0xA354;
		private const ushort MatTexVScaleChunk = 0xA356;
		private const ushort MatTexUOffsetChunk = 0xA358;
		private const ushort MatTexVOffsetChunk = 0xA35A;
		private const ushort MatTexAngleChunk = 0xA35C;
		private const ushort MatTexColour1Chunk = 0xA360;
		private const ushort MatTexColour2Chunk = 0xA362;
		private const ushort MatTexRColourChunk = 0xA364;
		private const ushort MatTexGColourChunk = 0xA366;
		private const ushort MatTexBColourChunk = 0xA368;

		// Ubiquitous Chunks (levels 3+) ******
		private const ushort ColourFloatChunk = 0x0010;   // Chunk with 3 floats defining r,g,b
		private const ushort Colour24Chunk = 0x0011;   // Chunk with 3 bytes defining r,g,b
		private const ushort LinearColour24Chunk = 0x0012;   // Chunk with 3 bytes defining r,g,b
		private const ushort IntPercentageChunk = 0x0030;   // Chunk with a single short value
		#endregion

		#region attributes
		private BinaryReader binReader;     // Reads in the 3DS binary file stream
		private Assets3DS readAssets;       // Set of assets read in from the current 3DS file
		#endregion

		#region Constructor(s)
		/// <summary>
		/// Constructor for the 3DS Reader.
		/// </summary>
		public Reader3ds(ILogger logger)
		{
			_logger = logger;
			this.binReader = null;
			this.readAssets = new Assets3DS();
		}
		#endregion

		#region private functions
		/// <summary>
		/// Read in a chunk header (this includes the id and length).
		/// </summary>
		/// <returns>The chunk in which we will store the info.</returns>
		private Chunk3DS ReadChunkHeader()
		{
			Chunk3DS chunk;
			chunk.start = this.binReader.BaseStream.Position;
			chunk.id = this.binReader.ReadUInt16();
			chunk.length = this.binReader.ReadInt32();
			chunk.end = chunk.start + chunk.length;
			////Console.WriteLine("Chunk Read: ID = {0:X}\tLength = {1}\tEnd Byte = {2}", chunk.id, chunk.length, chunk.end);
			return chunk;
		}

		/// <summary>
		/// Reads an ascii zero terminated string from the current binary stream.
		/// </summary>
		/// <returns>The string extracted from the bin stream (no zero at the end).</returns>
		private string ReadASCIIZ()
		{
			string tempStr = "";
			while (true)
			{
				char tempChar = this.binReader.ReadChar();
				if (tempChar == 0)
				{
					break;
				}
				tempStr = tempStr + tempChar;
			}
            /*
			if (tempStr == "")
			{
				throw new InvalidDataException("Invalid 3DS string chunk - empty field.");
			}
            */
			return tempStr;
		}

		/// <summary>
		/// Reads a r,g,b colour format of ushorts from the binary stream.
		/// </summary>
		/// <returns>The colour read in.</returns>
		private ByteColour3DS ReadColour24()
		{
			Chunk3DS tempChunk = this.ReadChunkHeader();
			if (tempChunk.id != LinearColour24Chunk && tempChunk.id != Colour24Chunk)
			{
				throw new InvalidDataException("Invalid 3DS byte colour chunk - chunk header mismatch.");
			}

			ByteColour3DS newColour;
			newColour.r = this.binReader.ReadByte();
			newColour.g = this.binReader.ReadByte();
			newColour.b = this.binReader.ReadByte();
			return newColour;
		}

		/// <summary>
		/// Reads an r,g,b colour format of floats from the binary stream.
		/// </summary>
		/// <returns>The colour read in.</returns>
		private FloatColour3DS ReadColourF()
		{
			Chunk3DS tempChunk = this.ReadChunkHeader();
			if (tempChunk.id != ColourFloatChunk)
			{
				throw new InvalidDataException("Invalid 3DS float colour chunk - chunk header mismatch.");
			}

			FloatColour3DS newColour;
			newColour.r = this.binReader.ReadSingle();
			newColour.g = this.binReader.ReadSingle();
			newColour.b = this.binReader.ReadSingle();
			return newColour;
		}

		/// <summary>
		/// Reads in a short number from the binary stream.
		/// </summary>
		/// <returns>The number extracted.</returns>
		private short ReadIntPercentage()
		{
			Chunk3DS tempChunk = this.ReadChunkHeader();
			if (tempChunk.id != IntPercentageChunk)
			{
				throw new InvalidDataException("Invalid 3DS integer percentage chunk - chunk header mismatch.");
			}
			short percentage = this.binReader.ReadInt16();
			return percentage;
		}

		/// <summary>
		/// Processes the primary/main chunk for the 3DS file (called on the first chunk read in)
		/// Note that these are 2nd - 3rd level chunks.
		/// </summary>
		/// <param name="prevChunk">The chunk whose header was previously read in and whose contained
		/// chunks we will be examining.</param>
		private void ProcessChunk(Chunk3DS prevChunk)
		{
			Chunk3DS currentChunk;
			// Read though the previous chunk until we reach its end
			while (this.binReader.BaseStream.Position < prevChunk.end)
			{
				currentChunk = this.ReadChunkHeader();

				switch (currentChunk.id)
				{
					case VersionChunk:
						// We need to make sure this 3DS file is version 3.0 or higher
						this.readAssets.versionNum = binReader.ReadInt32();
						//Console.WriteLine("Found version chunk: Version # = " + this.readAssets.versionNum);
						if (this.readAssets.versionNum < 3)
						{
							// Throw an informative exception about the version being bad
							throw new InvalidDataException("Bad 3DS file version");
						}
						break;

					case EditorChunk:
						//Console.WriteLine("Found editor chunk");

						Chunk3DS tempChunk = this.ReadChunkHeader();
						// Ensure the mesh version is 3 or above
						int meshVerNum = binReader.ReadInt32();
						//Console.WriteLine("Found Mesh Version chunk: Mesh Version # = " + meshVerNum);
						if (tempChunk.id != MeshVersionChunk || meshVerNum < 3)
						{
							throw new InvalidDataException("Bad 3DS mesh version.");
						}
						this.ProcessChunk(currentChunk);
						break;

					case MatChunk:
						//Console.WriteLine("Found material chunk");
						this.ProcessMaterialChunk(currentChunk);
						break;

					case ObjChunk:
						//Console.WriteLine("Found object chunk");
						this.ProcessObjectChunk(currentChunk);
						break;

					case KeyframerChunk:
						//Console.WriteLine("Found keyframer chunk");
						// TODO ... complete this
						break;

					case MasterScaleChunk:
						this.readAssets.masterScale = this.binReader.ReadSingle();
						//Console.WriteLine("Master Scale = " + this.readAssets.masterScale);
						break;

					default:
						break;
				}
				this.binReader.ReadBytes(currentChunk.length - currentChunk.GetBytesRead(this.binReader.BaseStream.Position));
			}
		}

		/// <summary>
		/// Processes a 3DS object chunk.
		/// Note that these are 3+ level chunks.
		/// </summary>
		/// <param name="prevChunk">The previous chunk (i.e., object chunk).</param>
		private void ProcessObjectChunk(Chunk3DS prevChunk)
		{
			// Start by reading the name
			string objName = this.ReadASCIIZ();
			//Console.WriteLine("\tObject Name = " + objName);

			Chunk3DS currentChunk;

			// Read though the previous chunk until we reach its end
			while (this.binReader.BaseStream.Position < prevChunk.end)
			{
				currentChunk = this.ReadChunkHeader();

				switch (currentChunk.id)
				{
					case ObjTriMeshChunk:
						//Console.WriteLine("\tObject Type = Triangular Mesh");
						TriMeshObject3DS triMesh = new TriMeshObject3DS(objName);
						this.ProcessTriMeshChunk(currentChunk, triMesh);
						triMesh.FinalizeMaterials(this.readAssets);
						this.readAssets.AddTriMeshObject(triMesh);
						break;
					case ObjLightChunk:
						//Console.WriteLine("\tObject Type = Light");
						LightObject3DS light = new LightObject3DS(objName);
						this.ProcessLightChunk(currentChunk, light);
						this.readAssets.AddLightObject(light);
						break;
					case ObjCameraChunk:
						//Console.WriteLine("\tObject Type = Camera");
						CameraObject3DS camera = new CameraObject3DS(objName);
						this.ProcessCameraChunk(currentChunk, camera);						
						this.readAssets.AddCameraObject(camera);
						break;
					default:
						break;
				}
				this.binReader.ReadBytes(currentChunk.length - currentChunk.GetBytesRead(this.binReader.BaseStream.Position));
			}
		}

		/// <summary>
		/// Processes a Triangular Mesh chunk from the 3DS file.
		/// </summary>
		/// <param name="triMeshChunk">The tri-mesh chunk.</param>
		private void ProcessTriMeshChunk(Chunk3DS triMeshChunk, TriMeshObject3DS triMesh)
		{
			Chunk3DS currentChunk;
			// Read though the chunk until we reach its end
			while (this.binReader.BaseStream.Position < triMeshChunk.end)
			{
				currentChunk = this.ReadChunkHeader();
				switch (currentChunk.id)
				{
					case ObjTMVArrayChunk:
						// Read in the vertex set for the triMesh
						ushort numVerts = this.binReader.ReadUInt16();
						//Console.WriteLine("\tVertex Array found, #Vertices = " + numVerts);
						for (ushort i = 0; i < numVerts; i++)
						{
							Vertex3DS v = new Vertex3DS(this.binReader.ReadSingle(), this.binReader.ReadSingle(), this.binReader.ReadSingle());
							////Console.WriteLine("\t\tVertex #" + i + ": " + v);
							triMesh.verts.Add(v);
						}
						break;

					//case ObjTMFlagArrayChunk:
					//    break;

					case ObjTMFaceArrayChunk:
						this.ProcessFaceArrayChunk(currentChunk, triMesh);
						break;

					case ObjTMTexCoordsChunk:
						//Read in the texture coordinates for the triMesh
						//Console.WriteLine("Found texture coordinates chunk");
						ushort numTexCoords = this.binReader.ReadUInt16();
						if (numTexCoords != triMesh.verts.Count)
							throw new InvalidDataException("Invalid number of texture coordinates - must be the same as the number of vertices.");
						//Console.WriteLine("\tTexture coordinate array found, #Texture Coords = " + numTexCoords);
						for (ushort i = 0; i < numTexCoords; i++)
						{
							UVCoord3DS texCoord;
							texCoord.u = this.binReader.ReadSingle();
							texCoord.v = this.binReader.ReadSingle();
							////Console.WriteLine("\t\tTexture Coord #" + i + ": " + texCoord);
							triMesh.texCoords.Add(texCoord);
						}
						break;

					case ObjTMMatrixChunk:
						// Read the orientation matrix for the triMesh
						//Console.WriteLine("Found orientation matrix chunk");
						for (int i = 0; i < 3; i++)
						{
							for (int j = 0; j < 4; j++)
							{
								triMesh.orientMatrix[i, j] = this.binReader.ReadSingle();
								//Console.Write(triMesh.orientMatrix[i, j] + " ");
							}
							////Console.WriteLine();
						}
						break;

					default:
						break;
				}
				this.binReader.ReadBytes(currentChunk.length - currentChunk.GetBytesRead(this.binReader.BaseStream.Position));
			}
		}

		/// <summary>
		/// Processes an array of faces and puts it into the triMesh object.
		/// </summary>
		/// <param name="fArrayChunk">The previous chunk read in (the face array chunk).</param>
		/// <param name="triMesh">The triangular mesh that the faces belong to.</param>
		private void ProcessFaceArrayChunk(Chunk3DS fArrayChunk, TriMeshObject3DS triMesh)
		{
			// Read in the faces for the triMesh (these are indices into the vertex array)
			ushort numFaces = this.binReader.ReadUInt16();
			//Console.WriteLine("\tFace Array found, #Faces = " + numFaces);
			for (ushort i = 0; i < numFaces; i++)
			{
				TriFace3DS f = new TriFace3DS(this.binReader.ReadUInt16(), this.binReader.ReadUInt16(),
																			this.binReader.ReadUInt16(), this.binReader.ReadUInt16());
				// Ensure that the indices fall into the proper range
				if (f.vertInd[0] >= triMesh.verts.Count || f.vertInd[1] >= triMesh.verts.Count || 
					  f.vertInd[2] >= triMesh.verts.Count)
					throw new InvalidDataException("Invalid face index - face index must be less than total number of vertices.");
				////Console.WriteLine("\t\tFace #" + i + ": " + f);
				triMesh.faces.Add(f);
			}

			Chunk3DS currentChunk;
			// Read though the chunk until we reach its end
			while (this.binReader.BaseStream.Position < fArrayChunk.end)
			{
				currentChunk = this.ReadChunkHeader();
				switch (currentChunk.id)
				{
					case ObjTMSmoothGrpChunk:
						////Console.WriteLine("\t\tSmoothing group chunk found.");
						for (ushort i = 0; i < triMesh.faces.Count; i++)
						{
							triMesh.smoothingGrps.Add(this.binReader.ReadInt32());
							////Console.WriteLine("\t\tSmoothing Group for face #" + i + ": " + triMesh.smoothingGrps[i]);
						}
						break;

					case ObjTMMatGrpChunk:
						// Read in a material group to apply to a certain subset of faces in the triMesh
						MaterialGroup3DS newMatGrp = new MaterialGroup3DS(this.readAssets, this.ReadASCIIZ());
						//Console.WriteLine("\t\tMaterial group found: " + newMatGrp);
						ushort numFaceInd = this.binReader.ReadUInt16();
						for (ushort i = 0; i < numFaceInd; i++)
						{
							ushort index = this.binReader.ReadUInt16();
							// Index must refer to an existing face in the triMesh
							if (index >= triMesh.faces.Count || index < 0)
							{
								throw new InvalidDataException("Material group face index out of bounds.");
							}
							newMatGrp.faceIndices.Add(index);
						}
						triMesh.materialGrps.Add(newMatGrp);
						break;
					/*
					case OBJ_TM_MATCUBE_CHUNK:
					break;
					*/
					default:
						break;
				}
				this.binReader.ReadBytes(currentChunk.length - currentChunk.GetBytesRead(this.binReader.BaseStream.Position));
			}

		}

		/// <summary>
		/// Process a scene camera and place it into the approparite 3DS object.
		/// </summary>
		/// <param name="cameraChunk">The previous chunk read in (camera chunk).</param>
		/// <param name="camera">The camera object that represents a 3DS file camera.</param>
		private void ProcessCameraChunk(Chunk3DS cameraChunk, CameraObject3DS camera)
		{
			// Read in the position, target and bank angle of the camera
			camera.position = new Vertex3DS(this.binReader.ReadSingle(), this.binReader.ReadSingle(), this.binReader.ReadSingle());
			camera.pointsAt = new Vertex3DS(this.binReader.ReadSingle(), this.binReader.ReadSingle(), this.binReader.ReadSingle());
			//camera.bankAngle = this.binReader.ReadSingle();
			//Console.WriteLine("\t\tPosition = " + camera.position);
			//Console.WriteLine("\t\tDirection = " + camera.pointsAt);
			////Console.WriteLine("\t\tAngle = " + camera.bankAngle);
			this.binReader.ReadBytes(cameraChunk.length - cameraChunk.GetBytesRead(this.binReader.BaseStream.Position));
		}

		/// <summary>
		/// Process the light chunk in the 3DS file.
		/// </summary>
		/// <param name="lightChunk">The previous chunk read (the light chunk).</param>
		/// <param name="light">The light object that will store info.</param>
		private void ProcessLightChunk(Chunk3DS lightChunk, LightObject3DS light)
		{
			// Read in the position of the light
			light.position = new Vertex3DS(this.binReader.ReadSingle(), this.binReader.ReadSingle(), this.binReader.ReadSingle());
			//Console.WriteLine("Light Position: " + light.position);
			// Read in the colour of the light
			light.colour = this.ReadColourF();
			//Console.WriteLine("\tLight Colour = " + light.colour);

			Chunk3DS currentChunk;
			// Read though the previous chunk until we reach its end
			while (this.binReader.BaseStream.Position < lightChunk.end)
			{
				// The current chunk we will be looking at is the next chunk in the stream
				// obtain its info so that we can parse it
				currentChunk = this.ReadChunkHeader();

				// Read in various 3DS material properties
				switch (currentChunk.id)
				{
					/*
			case ObjLightOuterRangeChunk:
					break;
			case ObjLightInnerRangeChunk:
					break;
			case ObjLightMultiplierChunk:
					break;
			case OBJ_LIGHT_EXCLUDE:
					break;
					 */
					case ObjLightIsSpotChunk:
						// The light is a spotlight, read in the appropriate values
						// specific to spotlights
						light.isSpotlight = true;
						light.pointsAt = new Vertex3DS(this.binReader.ReadSingle(), this.binReader.ReadSingle(), this.binReader.ReadSingle());
						this.binReader.ReadSingle();											// Discard the hotspot angle
						light.cutoffAngle = this.binReader.ReadSingle();	// Only take the falloff angle (widest)
						//Console.WriteLine("\tSpotlight Found, Direction = " + light.pointsAt);
						//Console.WriteLine("\t                 Cutoff Angle = " + light.cutoffAngle);
						break;

					default:
						break;
				}
				this.binReader.ReadBytes(currentChunk.length - currentChunk.GetBytesRead(this.binReader.BaseStream.Position));
			}
		}

		/// <summary>
		/// Process the material chunk in the 3DS file.
		/// </summary>
		/// <param name="prevChunk">The previous chunk read (material chunk).</param>
		private void ProcessMaterialChunk(Chunk3DS prevChunk)
		{
			Material3DS newMaterial = new Material3DS();
			Chunk3DS currentChunk;

			// Read though the previous chunk until we reach its end
			while (this.binReader.BaseStream.Position < prevChunk.end)
			{
				// The current chunk we will be looking at is the next chunk in the stream
				// obtain its info so that we can parse it
				currentChunk = this.ReadChunkHeader();

				// Read in various 3DS material properties
				switch (currentChunk.id)
				{
					case MatNameChunk:
						newMaterial.name = this.ReadASCIIZ();
						
						// Material name must be unique
						foreach (Material3DS mat in this.readAssets.materials)
						{
							if (mat.name == newMaterial.name)
							{
								// Conflicting names!!
								throw new InvalidDataException("Conflicting material names - material names must be unique. " +
																							 "NOTE: A 3DS material cannot have the name: " + Material3DS.DEFAULT_MAT_NAME +
																							 " for importing purposes.");
							}
						}

						//Console.WriteLine("Material = " + newMaterial.name);
						break;

					case MatAmbientChunk:
						newMaterial.ambient = this.ReadColour24();
						//Console.WriteLine("\tAmbient = " + newMaterial.ambient);
						break;

					case MatDiffuseChunk:
						newMaterial.diffuse = this.ReadColour24();
						//Console.WriteLine("\tDiffuse = " + newMaterial.diffuse);
						break;

					case MatSpecularChunk:
						newMaterial.specular = this.ReadColour24();
						//Console.WriteLine("\tSpecular = " + newMaterial.specular);
						break;

					case MatShinRatioChunk:
						newMaterial.shininessRatio = this.ReadIntPercentage();
						//Console.WriteLine("\tShininess Ratio = " + newMaterial.shininessRatio);
						break;

					case MatXPRatioChunk:
						newMaterial.transparencyRatio = this.ReadIntPercentage();
						//Console.WriteLine("\tTransparency Ratio = " + newMaterial.transparencyRatio);
						break;

					case MatTextureMap1Chunk:
						//Console.WriteLine("\tFound diffuse texture for material " + newMaterial.name);
						newMaterial.diffuseMap = ProcessTextureMapChunk(currentChunk);
						break;

					case MatSpecularMapChunk:
						//Console.WriteLine("\tFound specular texture for material " + newMaterial.name);
						newMaterial.specularMap = ProcessTextureMapChunk(currentChunk);
						break;

					case MatShadingChunk:
						newMaterial.shading = (Material3DS.ShadingStyle)this.binReader.ReadInt16();
						//Console.WriteLine("\tShading type = " + newMaterial.shading);
						break;

					/*
		  case MatShinStrChunk:
					break;
		  case MatXPFalloffChunk:
					break;
			case MAT_TEXMAP2_CHUNK:
					//Console.WriteLine("\tFound second texture for material " + newMaterial.name);
					ProcessTextureMapChunk(currentChunk);
					break;
			case MatRefBlurChunk:
					break;
			case MatUseRefBlurChunk:
					break;
			case MatSelfIllumChunk:
					break;
			case MatTwoSidedChunk:
					break;
			case MatDecalChunk:
					break;
			case MatAdditiveXPChunk:
					break;
			case MatWireframeChunk:
					break;
			case MAT_ABSWIREUNITS_CHUNK:
					break;
			case MAT_WIRESIZE_CHUNK:
					break;
							// and more...
					 */
					default:
						break;
				}
				this.binReader.ReadBytes(currentChunk.length - currentChunk.GetBytesRead(this.binReader.BaseStream.Position));
			}

			// Add the new material to the set of accumulated assets
			this.readAssets.AddMaterial(newMaterial);
		}

		/// <summary>
		/// Processes a texture chunk's subchunks in a 3DS file.
		/// Note that these are 4th level chunks.
		/// </summary>
		/// <param name="prevChunk">The previous chunk read in.</param>
		/// <param name="mat">The material this texture is a part of.</param>
		private TextureMap3DS ProcessTextureMapChunk(Chunk3DS prevChunk)
		{
			Chunk3DS currentChunk;

			TextureMap3DS newTexMap = new TextureMap3DS();
			// Read the texture strength... (NOT USED)
			//newTexMap.strength = this.ReadIntPercentage();
			////Console.WriteLine("\t\tTexture Strength = " + newTexMap.strength);

			// Read though the previous chunk until we reach its end
			while (this.binReader.BaseStream.Position < prevChunk.end)
			{
				// The current chunk we will be looking at is the next chunk in the stream
				// obtain its info so that we can parse it
				currentChunk = this.ReadChunkHeader();

				// Read in various texture properties
				switch (currentChunk.id)
				{
					case MatTexNameChunk:
						newTexMap.fileName = this.ReadASCIIZ();
						//Console.WriteLine("\t\tTexture File = " + newTexMap.fileName);
						break;
					case MatTexUScaleChunk:
						newTexMap.uScale = this.binReader.ReadSingle();
						//Console.WriteLine("\t\tTexture UScale = " + newTexMap.uScale);
						break;
					case MatTexVScaleChunk:
						newTexMap.vScale = this.binReader.ReadSingle();
						//Console.WriteLine("\t\tTexture VScale = " + newTexMap.vScale);
						break;
					case MatTexUOffsetChunk:
						newTexMap.uOffset = this.binReader.ReadSingle();
						//Console.WriteLine("\t\tTexture UOffset = " + newTexMap.uOffset);
						break;
					case MatTexVOffsetChunk:
						newTexMap.vOffset = this.binReader.ReadSingle();
						//Console.WriteLine("\t\tTexture VOffset = " + newTexMap.vOffset);
						break;
				case MatTexAngleChunk:
						newTexMap.rotAngle = this.binReader.ReadSingle();
						if (newTexMap.rotAngle > 360.0f || newTexMap.rotAngle < -360.0f)
						{
							throw new InvalidDataException("Invalid texture rotation angle - angle must be between 360 and -360.");
						}
						//Console.WriteLine("\t\tTexture angle = " + newTexMap.rotAngle);
						break;
					/*
					case MatTexTilingChunk:
							this.binReader.ReadUInt16();
							break;
					case MatTexBlurChunk:
							this.binReader.ReadSingle();
							break;
					case MatTexColour1Chunk:
							this.binReader.ReadUInt16();
							this.binReader.ReadUInt16();
							this.binReader.ReadUInt16();
							break;
					case MatTexColour2Chunk:
							this.binReader.ReadUInt16();
							this.binReader.ReadUInt16();
							this.binReader.ReadUInt16();
							break;
					case MatTexRColourChunk:
							this.binReader.ReadUInt16();
							this.binReader.ReadUInt16();
							this.binReader.ReadUInt16();
							break;
					case MatTexGColourChunk:
							this.binReader.ReadUInt16();
							this.binReader.ReadUInt16();
							this.binReader.ReadUInt16();
							break;
					case MatTexBColourChunk:
							this.binReader.ReadUInt16();
							this.binReader.ReadUInt16();
							this.binReader.ReadUInt16();
							break;
					*/
					default:
						break;
				}
				this.binReader.ReadBytes(currentChunk.length - currentChunk.GetBytesRead(this.binReader.BaseStream.Position));
			}
			return newTexMap;
		}

		/// <summary>
		/// Cleans up all the information that
		/// </summary>
		private void CleanUp()
		{
			if (this.binReader != null)
			{
				this.binReader.Close();
			}
			this.binReader = null;
		}
		#endregion

		#region public functions
		/// <summary>
		/// Reads the 3ds file and builds the appropriate data structures
		/// to represent it.
		/// </summary>
		/// <returns>The assets accumulated from reading the file if success, null otherwise.</returns>
		public Assets3DS Read(FileStream fs)
		{
			// Make sure there are no assets from a previous read
			this.readAssets = new Assets3DS();

			this.binReader = new BinaryReader(fs);

			// This is where we actually process every chunk in the 3DS file
			// and assimilate it into the assets object (this.readAssets) of this reader
			try
			{
				// Start by reading the main chunk
				Chunk3DS mainChunk = ReadChunkHeader();
				if (mainChunk.id != PrimaryChunk)
					throw new InvalidDataException("Primary chunk not found - invalid 3DS format.");
				this.ProcessChunk(mainChunk);
			}
			catch (InvalidDataException e)
			{
				_logger.Error(e + "\nCould not load file\nFormat of 3DS file is incorrect.");
				this.CleanUp();
				return null;
			}
			catch (Exception e)
			{
				//Console.WriteLine(e);
				_logger.Error("Could not load file\nFormat of 3DS file is incorrect.");
				this.CleanUp();
				return null;
			}

			// Clean up and return the read assets
			return this.readAssets;
		}
		#endregion

	}
}
