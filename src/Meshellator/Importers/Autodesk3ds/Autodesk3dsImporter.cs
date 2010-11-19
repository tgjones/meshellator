using System.Diagnostics;
using System.IO;
using System.Linq;
using Nexus;

namespace Meshellator.Importers.Autodesk3ds
{
	[AssetImporter(".3ds", "Autodesk 3DS")]
	public class Autodesk3dsImporter : AssetImporterBase
	{
		private const bool FLIPYZ = false;
		private const bool FLIPV = false;

		public override Scene ImportFile(FileStream fileStream, string fileName)
		{
			Scene3ds scene3ds = new Scene3ds(fileStream, null, Scene3ds.DECODE_ALL);

			Scene result = new Scene();

			// Compute normals for our scene
			computeNormals(scene3ds, result);

			Material defaultMaterial = new Material
			{
				Name = "Default",
				FileName = fileName,
				DiffuseColor = ColorsRgbF.Gray,
				SpecularColor = ColorsRgbF.White,
				Shininess = 64
			};

			for (int i = 0; i < scene3ds.Materials.Count(); ++i)
			{
				Material3ds material3ds = scene3ds.Materials.ElementAt(i);
				Material material = new Material
				{
					Name = material3ds.name(),
					FileName = fileName,
					DiffuseColor = material3ds.diffuse().ToColorRgbF(),
					AmbientColor = material3ds.ambient().ToColorRgbF(),
					SpecularColor = material3ds.specular().ToColorRgbF(),
					Transparency = material3ds.transparency()
				};
				if ((int) material3ds._shininess != 0)
					material.Shininess = (int) material3ds._shininess;
				result.Materials.Add(material);
			}

			for (int i = 0; i < scene3ds.Meshes.Count(); ++i)
			{
				Mesh3ds mesh3ds = scene3ds.Meshes.ElementAt(i);

				Mesh resultMesh = result.Meshes[i];

				//resultMesh.Material = defaultMaterial;

				// TODO: Split by face materials so that each mesh only has one material.
				Debug.Assert(mesh3ds.faceMats() == 1);
				for (int fm = 0; fm < mesh3ds.faceMats(); ++fm)
				{
					// Get current material's face.
					FaceMat3ds fmat = mesh3ds.faceMat(fm);
					Material material = result.Materials[fmat.material()];
					resultMesh.Material = material;

					for (int fi = 0; fi < fmat.faces(); ++fi)
					{
						int idx = fmat.face(fi);
						Face3ds f = mesh3ds.face(idx);


						resultMesh.Indices.Add(f.P0);
						resultMesh.Indices.Add(f.P1);
						resultMesh.Indices.Add(f.P2);
					}
				}
			}

			return result;
		}

		//
		// Compute scene vertex normals
		//
		public void computeNormals(Scene3ds scene, Scene result)
		{
			Point3D vcenter = Point3D.Zero;
			float vcounter = 0.0f;
			for (int i = 0; i < scene.Meshes.Count(); i++)
			{
				Mesh3ds m = scene.Meshes.ElementAt(i);
				// Alloc memory
				Mesh mesh = new Mesh
				{
					Name = m.Name
				};
				result.Meshes.Add(mesh);

				//mesh._numTexCoords = 0;

				Vector3D[] tmpFaceNormals = new Vector3D[m.faces()];

				// Compute face normals
				for (int fi = 0; fi < m.faces(); fi++)
				{
					Face3ds f = m.face(fi);
					Vertex3ds p0 = m.vertex(f.P0);
					Vertex3ds p1 = m.vertex(f.P1);
					Vertex3ds p2 = m.vertex(f.P2);

					/*// Compute face middle point
					mesh.faceMiddlePoint[fi] = new PVector();
					mesh.faceMiddlePoint[fi].x = (p0.X + p1.X + p2.X) / 3.0f;
					mesh.faceMiddlePoint[fi].y = (p0.Y + p1.Y + p2.Y) / 3.0f;
					mesh.faceMiddlePoint[fi].z = (p0.Z + p1.Z + p2.Z) / 3.0f;*/

					Point3D v0 = new Point3D(p0.X, p0.Y, p0.Z);
					Point3D v1 = new Point3D(p1.X, p1.Y, p1.Z);
					Point3D v2 = new Point3D(p2.X, p2.Y, p2.Z);

					Vector3D e0 = v1 - v0;
					Vector3D e1 = v2 - v0;

					//mesh.faceNormals[fi] = e1.cross(e0);

					// save a copy of the unnormalized face normal. used for average vertex normals
					tmpFaceNormals[fi] = Vector3D.Cross(e1, e0);

					// normalize face normal
					//mesh.faceNormals[fi].normalize();
				}

				//
				// Compute vertex normals.Take average from adjacent face normals.find coplanar faces or get weighted normals.
				// One could also use the smooth groups from 3ds to compute normals, we'll see about that. 
				//
				//PVector v = new PVector();
				TexCoord3ds tc = new TexCoord3ds(0, 0);
				for (int vi = 0; vi < m.vertices(); vi++)
				{
					Vertex3ds p = m.vertex(vi);
					vcenter += new Point3D(p.X, p.Y, p.Z);
					vcounter++;
					if (m.texCoords() > 0) tc = m.texCoord(vi);
					Vector3D n = Vector3D.Zero;
					float num = 0;
					for (int fi = 0; fi < m.faces(); fi++)
					{
						Face3ds f = m.face(fi);
						//        Vertex3ds p0 = m.vertex(f.P0);
						//        Vertex3ds p1 = m.vertex(f.P1);
						//        Vertex3ds p2 = m.vertex(f.P2);
						if (vi == f.P0 || vi == f.P1 || vi == f.P2)
						{
							num++;
							n += tmpFaceNormals[fi]; //mesh.faceNormals[fi] );
						}
					}
					if (num > 0) n *= 1.0f/(float) num;
					n.Normalize();
					mesh.Normals.Add(n);

					if (FLIPYZ)
					{
						Vector3D tmp = mesh.Normals[vi];
						mesh.Normals[vi] = new Vector3D(tmp.X, -tmp.Z, tmp.Y);
					}
					// Save vertex data      
					if (FLIPYZ) mesh.Positions.Add(new Point3D(p.X, -p.Z, p.Y));
					else mesh.Positions.Add(new Point3D(p.X, p.Y, p.Z));

					// Save texcoord data
					//mesh._numTexCoords = m.texCoords();
					if (m.texCoords() > 0)
					{
						if (FLIPV) mesh.TextureCoordinates.Add(new Point3D(tc.U, 1.0f - tc.V, 0));
						else mesh.TextureCoordinates.Add(new Point3D(tc.U, tc.V, 0));
					}
				}
			}

			if (vcounter > 0.0) vcenter /= vcounter;
		}
	}
}