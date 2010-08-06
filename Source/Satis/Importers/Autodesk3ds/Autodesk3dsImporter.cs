using System.IO;

namespace Satis.Importers.Autodesk3ds
{
	[AssetImporter(".3ds", "Autodesk 3DS")]
	public class Autodesk3dsImporter : AssetImporterBase
	{
		public override Scene ImportFile(FileStream fileStream, string fileName)
		{
			Scene3ds scene3ds = new Scene3ds(fileStream, null, Scene3ds.DECODE_ALL);

			// Compute normals for our scene
			//computeNormals();

			Scene result = new Scene();



			return result;
		}

		/*//
		// Compute scene vertex normals
		//
		public void computeNormals(Scene3ds scene, Scene result)
		{
			PVector vcenter = new PVector();
			float vcounter = 0.0f;
			for (int i = 0; i < scene.meshes(); i++)
			{
				Mesh3ds m = scene.mesh(i);
				// Alloc memory
				Mesh mesh = new Mesh();
				result.Meshes.Add(mesh);

				mesh.faceNormals = new PVector[m.faces()];
				mesh.faceMiddlePoint = new PVector[m.faces()];
				mesh.vertices = new PVector[m.vertices()];
				mesh.vertexNormals = new PVector[m.vertices()];
				mesh._numTexCoords = 0;
				mesh.texCoords = new PVector[m.vertices()];

				PVector[] tmpFaceNormals = new PVector[m.faces()];

				// Compute face normals
				for (int fi = 0; fi < m.faces(); fi++)
				{
					Face3ds f = m.face(fi);
					Vertex3ds p0 = m.vertex(f.P0);
					Vertex3ds p1 = m.vertex(f.P1);
					Vertex3ds p2 = m.vertex(f.P2);

					// Compute face middle point
					mesh.faceMiddlePoint[fi] = new PVector();
					mesh.faceMiddlePoint[fi].x = (p0.X + p1.X + p2.X) / 3.0f;
					mesh.faceMiddlePoint[fi].y = (p0.Y + p1.Y + p2.Y) / 3.0f;
					mesh.faceMiddlePoint[fi].z = (p0.Z + p1.Z + p2.Z) / 3.0f;

					PVector v0 = new PVector(p0.X, p0.Y, p0.Z);
					PVector v1 = new PVector(p1.X, p1.Y, p1.Z);
					PVector v2 = new PVector(p2.X, p2.Y, p2.Z);

					PVector e0 = PVector.sub(v1, v0);
					PVector e1 = PVector.sub(v2, v0);

					mesh.faceNormals[fi] = e1.cross(e0);

					// save a copy of the unnormalized face normal. used for average vertex normals
					tmpFaceNormals[fi] = mesh.faceNormals[fi].get();

					// normalize face normal
					mesh.faceNormals[fi].normalize();
				}

				//
				// Compute vertex normals.Take average from adjacent face normals.find coplanar faces or get weighted normals.
				// One could also use the smooth groups from 3ds to compute normals, we'll see about that. 
				//
				//PVector v = new PVector();
				PVector n = new PVector();
				TexCoord3ds tc = new TexCoord3ds(0, 0);
				for (int vi = 0; vi < m.vertices(); vi++)
				{
					Vertex3ds p = m.vertex(vi);
					vcenter.add(p.X, p.Y, p.Z);
					vcounter++;
					if (m.texCoords() > 0) tc = m.texCoord(vi);
					n.set(0, 0, 0);
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
							n.add(tmpFaceNormals[fi]); //mesh.faceNormals[fi] );
						}
					}
					if (num > 0) n.mult(1.0f / (float)num);
					n.normalize();
					mesh.vertexNormals[vi] = n.get();

					if (FLIPYZ)
					{
						float tmp = mesh.vertexNormals[vi].y;
						mesh.vertexNormals[vi].y = -mesh.vertexNormals[vi].z;
						mesh.vertexNormals[vi].z = tmp;
					}
					// Save vertex data      
					if (FLIPYZ) mesh.vertices[vi] = new PVector(p.X, -p.Z, p.Y);
					else mesh.vertices[vi] = new PVector(p.X, p.Y, p.Z);

					// Save texcoord data
					mesh._numTexCoords = m.texCoords();
					if (m.texCoords() > 0)
					{
						if (FLIPV) mesh.texCoords[vi] = new PVector(tc.U, 1.0f - tc.V, 0);
						else mesh.texCoords[vi] = new PVector(tc.U, tc.V, 0);
					}
				}

				tmpFaceNormals = null;

			}

			if (vcounter > 0.0) vcenter.div(vcounter);
		}*/
	}
}