using System;
using System.IO;
using Meshellator.Importers.LightwaveObj.Objects;
using Nexus;

namespace Meshellator.Importers.LightwaveObj
{
	[AssetImporter(".obj", "Lightwave OBJ")]
	public class LightwaveObjImporter : AssetImporterBase
	{
		public override Scene ImportFile(FileStream fileStream, string fileName)
		{
			WavefrontObject wavefrontObject = new WavefrontObject(fileName);

			Material defaultMaterial = new Material
			{
				Name = "Default",
				DiffuseColor = ColorsRgbF.Gray,
				SpecularColor = ColorsRgbF.White,
				Shininess = 64
			};

			Scene scene = new Scene();

			foreach (Group g in wavefrontObject.Groups)
			{
				Mesh mesh = new Mesh();
				scene.Meshes.Add(mesh);

				Objects.Material gm = g.Material;

				Material material;
				if (gm != null)
				{
					material = new Material
					{
						Name = gm.Name,
						FileName = gm.FileName
					};
					// material.Ambient

					if (!string.IsNullOrEmpty(gm.TextureName))
						material.DiffuseTextureName = gm.TextureName;
					else
						material.DiffuseColor = (gm.Kd != null)
							? new ColorRgbF(gm.Kd.X, gm.Kd.Y, gm.Kd.Z)
							: ColorsRgbF.Gray;

					material.SpecularColor = (gm.Ks != null)
						? new ColorRgbF(gm.Ks.X, gm.Ks.Y, gm.Ks.Z)
						: ColorsRgbF.Black;
					material.Shininess = (int)gm.Shininess; // TODO: Check
				}
				else
				{
					material = defaultMaterial;
				}
				mesh.Material = material;
				if (!scene.Materials.Contains(material))
					scene.Materials.Add(material);

				// Currently this is not utilising indices.
				int counter = 0;
				foreach (Face f in g.Faces)
				{
					// Copy normals.
					foreach (int normalIndex in f.NormalIndices)
					{
						Vertex n = wavefrontObject.Normals[normalIndex];
						mesh.Normals.Add(new Vector3D(n.X, n.Y, n.Z));
					}

					// Copy normals.
					foreach (int textureIndex in f.TextureIndices)
					{
						TextureCoordinate tc = wavefrontObject.Textures[textureIndex];
						mesh.TextureCoordinates.Add(new Point3D(tc.U, tc.V, tc.W));
					}

					switch (f.Type)
					{
						case FaceType.Triangles:
							// Copy positions.
							foreach (int vertexIndex in f.VertexIndices)
							{
								Vertex v = wavefrontObject.Vertices[vertexIndex];
								mesh.Positions.Add(new Point3D(v.X, v.Y, v.Z));
							}
							mesh.Indices.Add(counter++);
							mesh.Indices.Add(counter++);
							mesh.Indices.Add(counter++);
							//mesh.Indices.AddRange(f.VertexIndices);
							break;
						default:
							throw new NotSupportedException();
					}
				}

				/*foreach (Vertex v in wavefrontObject.Vertices)
					mesh.Positions.Add(new Point3D(v.X, v.Y, v.Z));

				foreach (Vertex v in wavefrontObject.Normals)
					mesh.Normals.Add(new Vector3D(v.X, v.Y, v.Z));

				foreach (TextureCoordinate tc in wavefrontObject.Textures)
					mesh.TextureCoordinates.Add(new Point3D(tc.U, tc.V, 0));*/
			}

			return scene;
		}
	}
}