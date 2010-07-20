using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using Nexus;
using Satis.Importers.Autodesk3ds.Autodesk3ds;
using Satis.Importers.Autodesk3ds.FileFormat;
using Satis.Logging;

namespace Satis.Importers.Autodesk3ds
{
	[AssetImporter(".3ds", "Autodesk 3DS")]
	//[Export(typeof(IAssetImporter))]
	public class Autodesk3dsImporter : AssetImporterBase
	{
		[Import(typeof(ILogger))]
		private ILogger Logger { get; set; }

		// TODO: Should be a dialog or something.
		private class ImportSettings
		{
			public bool ImportCameras { get; set; }
			public bool ImportLights { get; set; }
			public bool ImportMaterials { get; set; }
			public bool FlipYZ { get; set; }
			public float NormalAngleThreshold { get; set; }

			public ImportSettings()
			{
				ImportLights = true;
				ImportCameras = true;
				ImportMaterials = true;
				FlipYZ = true;
				NormalAngleThreshold = 30;
			}
		}

		public override Scene ImportFile(FileStream fileStream, string fileName)
		{
			Scene scene = new Scene { FileName = fileName };

			// Import the file given the options in the importDialog...
			Reader3ds reader = new Reader3ds(Logger);
			Assets3DS fileAssets = reader.Read(fileStream);
			// Make sure the file was read in OK
			if (fileAssets == null)
			{
				return scene;
			}

			ImportSettings importSettings = new ImportSettings();

			/*// Import cameras from the 3DS file as FXComposer persepective cams
			if (importSettings.ImportCameras)
			{
				this.ImportCameras(fileAssets.cameras);
			}*/

			/*// Import lights (spot and point)
			if (importSettings.ImportLights)
			{
				this.ImportLights(fileAssets.lights);
			}*/

			// Import materials from the 3DS file into FXComposer as Effects
			if (importSettings.ImportMaterials)
			{
				this.ImportEffects(fileAssets.materials, scene);
			}
			// Import geometry from the 3DS file into FXComposer
			this.ImportGeometry(fileAssets.triMeshes, importSettings, scene);

			return scene;
		}

		#region Private Members

		/// <summary>
		/// Imports materials from the 3DS file into FXComposer.
		/// </summary>
		/// <param name="materials">Listing of 3DS materials.</param>
		/// <param name="scene"></param>
		private void ImportEffects(List<Material3DS> materials, Scene scene)
		{
			foreach (Material3DS material in materials)
			{
				Material satisMaterial = new Material();

				// Determine the shading style and create a profile type based on it
				switch (material.shading)
				{
					case Material3DS.ShadingStyle.Phong:
						this.ImportPhongMaterial(material, satisMaterial);
						break;

					// TODO? Support for more shading models here...

					default:
						// By default make it a phong shading effect
						material.shading = Material3DS.ShadingStyle.Phong;
						this.ImportPhongMaterial(material, satisMaterial);
						break;
				}

				// TODO: More stuff??

				// Add the effect so that it can be inserted later
				scene.Materials.Add(satisMaterial);
			}
		}

		/// <summary>
		/// Imports a phong material into FXComposer.
		/// </summary>
		/// <param name="material">The material (must be phong).</param>
		/// <param name="satisMaterial"></param>
		private void ImportPhongMaterial(Material3DS material, Material satisMaterial)
		{
			Debug.Assert(material.shading == Material3DS.ShadingStyle.Phong);
			Debug.Assert(satisMaterial != null);
			Debug.Assert(material != null);

			// Import properties common to all materials
			this.ImportCommonMaterialProperties(material, satisMaterial);

			// Shininess
			satisMaterial.Shininess = Material3DS.ConvertPctShininessToPhong(material.shininessRatio);

			// Transparency 
			satisMaterial.Transparency = 1.0f - Material3DS.ConvertPctToFloat(material.transparencyRatio);
		}

		/// <summary>
		/// Import properties common to all material types.
		/// </summary>
		/// <param name="material">The material extracted from the 3DS file.</param>
		/// <param name="satisMaterial"></param>
		private void ImportCommonMaterialProperties(Material3DS material, Material satisMaterial)
		{
			Debug.Assert(satisMaterial != null);
			Debug.Assert(material != null);

			satisMaterial.Name = material.name;

			// Ambient colour
			satisMaterial.AmbientColor = material.ambient.ToColor();

			// Diffuse colour/texture
			if (material.diffuseMap == null)
				satisMaterial.DiffuseColor = material.diffuse.ToColor();
			else
				satisMaterial.DiffuseTextureName = material.diffuseMap.fileName;

			// Specular colour/texture
			if (material.specularMap == null)
				satisMaterial.SpecularColor = material.specular.ToColor();
			else
				satisMaterial.SpecularTextureName = material.specularMap.fileName;
		}

		/*/// <summary>
		/// Imports a texture image (brings a texture into an FXImage object).
		/// </summary>
		/// <param name="matName">Material name for the texture.</param>
		/// <param name="filepathTo3DS">Path to the original 3DS file.</param>
		/// <param name="texture">The texture obtained via reading in the 3DS file.</param>
		/// <returns></returns>
		private FXImage ImportTextureImage(string matName, string filepathTo3DS, TextureMap3DS texture)
		{
			FXDebug.Assert(texture != null);
			FXImage texImage = null;
			string path = Path.GetDirectoryName(filepathTo3DS);

			// Look for the texture in the typical directories...
			foreach (string prefix in FXAutodesk3DSReader.DefaultTextureDirs)
			{
				texImage = FxcImage.Create(matName + "_" + Path.GetFileNameWithoutExtension(texture.fileName),
																 Path.Combine(Path.Combine(path, prefix), texture.fileName));
				if (texImage != null && texImage.Loaded)
				{
					break;
				}
				else if (texImage != null)
				{
					FxcImage.Destroy(texImage);
				}
			}

			// If texture was not found, say so
			if (texImage == null || !texImage.Loaded)
			{
				FXAutodesk3DSReader.ShowWarning("Could not find texture: " + texture.fileName);
				return null;
			}
			return texImage;
		}*/

		/*/// <summary>
		/// Imports camera objects from the 3DS file into FXComposer.
		/// </summary>
		/// <param name="cameras">A list of 3DS camera objects.</param>
		private void ImportCameras(List<CameraObject3DS> cameras)
		{
			foreach (CameraObject3DS camera in cameras)
			{
				FXCameraPerspective fxCam = FxcCamera.CreatePerspectiveCamera(camera.name);
				FXNode camNode = FxcScene.AddCamera(scene, fxCam);

				FXVector3 pos = new FXVector3(camera.position.x, camera.position.y, camera.position.z);
				FXVector3 lookAt = new FXVector3(camera.pointsAt.x, camera.pointsAt.y, camera.pointsAt.z);
				if (FXAutodesk3DSReader.importDialog.flipYZ)
				{
					pos = new FXVector3(camera.position.x, camera.position.z, -camera.position.y);
					lookAt = new FXVector3(camera.pointsAt.x, camera.pointsAt.z, -camera.pointsAt.y);
				}
				FXVector3 direction = lookAt - pos;

				// Rotate camera into basic position in 3DS max (lookAt = (0,1,0), up = (0,0,1)
				FXQuaternion rotate = FXQuaternion.RotationAxis(new FXVector3(1, 0, 0), MathUtility.PI_OVER_2);

				// Since 3DS files don't give us an up vector for the camera we don't rotate along its axis
				// Rotate on the xy-plane (about the z-axis)
				FXVector3 projXY = new FXVector3(direction.X, direction.Y, 0);
				projXY.Normalize();
				FXVector3 horizAxis = FXVector3.Cross(new FXVector3(0, 1, 0), projXY);
				if (FXVectorsEqualWithEpsilon(horizAxis, FXVector3.Zero))
				{
					horizAxis.X = 0;
					horizAxis.Y = 0;
					horizAxis.Z = 1;
				}
				else
				{
					horizAxis.Normalize();
				}
				FXQuaternion horizRotate = FXQuaternion.RotationAxis(horizAxis, MathUtility.Acos(projXY.Y));
				rotate = horizRotate * rotate;

				// Rotate up or down to the direction
				direction.Normalize();
				FXVector3 vertAxis = FXVector3.Cross(projXY, direction);
				if (!FXVectorsEqualWithEpsilon(vertAxis, FXVector3.Zero))
				{
					vertAxis.Normalize();
					FXQuaternion vertRot = FXQuaternion.RotationAxis(vertAxis, MathUtility.Acos(direction.X * projXY.X + direction.Y * projXY.Y));
					rotate = vertRot * rotate;
				}
				camNode.Rotate = rotate;
				camNode.Translate = pos;
			}
		}*/

		/*/// <summary>
		/// Import lights from the 3DS file into FXComposer.
		/// </summary>
		/// <param name="lights">A list of 3DS lights.</param>
		private void ImportLights(List<LightObject3DS> lights)
		{
			foreach (LightObject3DS light in lights)
			{
				FXVector3 position = new FXVector3(light.position.x, light.position.y, light.position.z);
				if (FXAutodesk3DSReader.importDialog.flipYZ)
				{
					position = new FXVector3(light.position.x, light.position.z, -light.position.y);
				}

				// Add the light to the scene based on what type it is
				FXLight fxLight = null;
				FXNode lightNode = null;
				if (light.isSpotlight)
				{
					fxLight = FxcLight.CreateSpotLight(light.name);
					lightNode = FxcScene.AddLight(scene, fxLight);

					// Set the direction for the spot light
					FXVector3 pointsAt = new FXVector3(light.pointsAt.x, light.pointsAt.y, light.pointsAt.z);
					if (FXAutodesk3DSReader.importDialog.flipYZ)
					{
						pointsAt = new FXVector3(light.pointsAt.x, light.pointsAt.z, -light.pointsAt.y);
					}
					FXVector3 direction = pointsAt - position;

					// Rotate light into basic position in 3DS max (lookAt = (0,1,0), up = (0,0,1)
					FXQuaternion rotate = FXQuaternion.RotationAxis(new FXVector3(1, 0, 0), (float)Math.PI / 2.0f);

					FXVector3 projXY = new FXVector3(direction.X, direction.Y, 0);
					projXY.Normalize();
					FXVector3 horizAxis = FXVector3.Cross(new FXVector3(0, 1, 0), projXY);
					if (FXVectorsEqualWithEpsilon(horizAxis, FXVector3.Zero))
					{
						horizAxis.X = 0;
						horizAxis.Y = 0;
						horizAxis.Z = 1;
					}
					else
					{
						horizAxis.Normalize();
					}
					FXQuaternion horizRotate = FXQuaternion.RotationAxis(horizAxis, (float)Math.Acos(projXY.Y));
					rotate = horizRotate * rotate;

					// Rotate up or down to the direction
					direction.Normalize();
					FXVector3 vertAxis = FXVector3.Cross(projXY, direction);
					if (!FXVectorsEqualWithEpsilon(vertAxis, FXVector3.Zero))
					{
						vertAxis.Normalize();
						FXQuaternion vertRot = FXQuaternion.RotationAxis(vertAxis, (float)Math.Acos(direction.X * projXY.X + direction.Y * projXY.Y));
						rotate = vertRot * rotate;
					}

					lightNode.Rotate = rotate;
				}
				else
				{
					// Not a spotlight, just add a point light
					fxLight = FxcLight.CreatePointLight(light.name);
					lightNode = FxcScene.AddLight(scene, fxLight);
				}

				// Set position of the light
				lightNode.Translate = position;

				// Set colour of the light...
				FXMatrix lightColour = new FXMatrix(1, 4);
				lightColour[0, 0] = light.colour.r;
				lightColour[0, 1] = light.colour.g;
				lightColour[0, 2] = light.colour.b;
				lightColour[0, 3] = 1.0f;
				fxLight.Color = lightColour;
			}
		}

		/// <summary>
		/// Quick and dirty function for determining if two FXComposer vectors are equal (with an epsilon).
		/// </summary>
		/// <param name="lhs">Left hand side.</param>
		/// <param name="rhs">Right hand side.</param>
		/// <returns>True if equal, false otherwise.</returns>
		private static bool FXVectorsEqualWithEpsilon(FXVector3 lhs, FXVector3 rhs)
		{
			if (Math.Abs(lhs.X - rhs.X) < Vertex3DS.EPSILON && Math.Abs(lhs.Y - rhs.Y) < Vertex3DS.EPSILON && Math.Abs(lhs.Z - rhs.Z) < Vertex3DS.EPSILON)
			{
				return true;
			}
			return false;
		}*/

		/// <summary>
		/// Imports the geometry from the accumulated 3DS triangular meshes into FXComposer.
		/// </summary>
		/// <param name="geometry">Triangular mesh geometry from 3DS file.</param>
		private void ImportGeometry(List<TriMeshObject3DS> geometry, ImportSettings importSettings, Scene scene)
		{
			foreach (TriMeshObject3DS triMesh in geometry)
			{
				Mesh mesh = new Mesh();
				scene.Meshes.Add(mesh);

				// Obtain vertices from the triMesh and place into a stream
				for (int i = 0; i < triMesh.verts.Count; i++)
				{
					mesh.Positions.Add((importSettings.FlipYZ)
						? new Point3D(triMesh.verts[i].x, triMesh.verts[i].z, -triMesh.verts[i].y)
						: new Point3D(triMesh.verts[i].x, triMesh.verts[i].y, triMesh.verts[i].z));
				}

				/*//Seperate material groups by smoothing group into more material groups
				List<MaterialGroup3DS> newMatGrps = new List<MaterialGroup3DS>();
				foreach (MaterialGroup3DS materialGrp in triMesh.materialGrps)
				{
					Dictionary<long, MaterialGroup3DS> smoothAndMaterialGrps = new Dictionary<long, MaterialGroup3DS>();
					foreach (ushort s in materialGrp.faceIndices)
					{
						long smoothGrp = (s < 0 || s >= triMesh.smoothingGrps.Count) ? -1 : triMesh.smoothingGrps[s];
						MaterialGroup3DS newMatGrp;
						if (!smoothAndMaterialGrps.TryGetValue(smoothGrp, out newMatGrp))
						{
							newMatGrp = new MaterialGroup3DS(materialGrp.material);
							smoothAndMaterialGrps[smoothGrp] = newMatGrp;
							newMatGrps.Add(newMatGrp);
						}
						newMatGrp.faceIndices.Add(s);
					}
				}*/
				List<MaterialGroup3DS> newMatGrps = triMesh.materialGrps;

				// TODO: REMOVE
				if (newMatGrps.Count != 1)
					throw new System.NotImplementedException();

				// Establish polygon groups based on material groupings within the triangular mesh
				foreach (MaterialGroup3DS materialGrp in newMatGrps)
				{
					// Obtain texture coords from the triMesh and place into a stream
					// (These may be different depending on the material)

					// NOTE: We only look at the texture coordinate specifics of one of the textures
					// by default the diffuse texture is looked at.
					TextureMap3DS tempTexMap = materialGrp.material.diffuseMap ?? materialGrp.material.specularMap;

					for (int i = 0; i < triMesh.texCoords.Count; i++)
					{
						if (tempTexMap == null)
						{
							mesh.TextureCoordinates.Add(new Point3D(triMesh.texCoords[i].u, triMesh.texCoords[i].v, 0.0f));
						}
						else
						{
/*
							// Transform the texture coordinates based on those in tempTexMap
							Matrix2D xFormMatrix = Matrix2D.Identity;
							xFormMatrix = xFormMatrix * Matrix2D.CreateScale(tempTexMap.uScale, tempTexMap.vScale, 1.0f);
							xFormMatrix.Translate(tempTexMap.uOffset, tempTexMap.vOffset, 0.0f);
							FXMatrix rotateMatrix = new FXMatrix(3, 3, 0);
							double angleInRads = Trig.DegreesToRadians(tempTexMap.rotAngle);
							rotateMatrix[0, 0] = (float)Math.Cos(angleInRads);
							rotateMatrix[0, 1] = (float)-Math.Sin(angleInRads);
							rotateMatrix[1, 0] = (float)Math.Sin(angleInRads);
							rotateMatrix[1, 1] = rotateMatrix[0, 0];
							rotateMatrix[2, 2] = 1.0f;
							xFormMatrix = xFormMatrix * rotateMatrix;

							Point3D finalTexCoords = new Point3D(triMesh.texCoords[i].u, triMesh.texCoords[i].v, 1.0f);
							finalTexCoords = finalTexCoords * xFormMatrix;
							texCoordData[i] = finalTexCoords;
							*/
							throw new System.NotImplementedException();
						}
					}

					mesh.MaterialName = materialGrp.material.name;

					// Create all of the faces for the new mesh
					for (int i = 0; i < materialGrp.faceIndices.Count; i++)
					{
						TriFace3DS face = triMesh.faces[materialGrp.faceIndices[i]];
						for (int j = 0; j < 3; j++)
							mesh.Indices.Add(face.vertInd[j]);
					}
				}
			}
		}
		#endregion
	}
}