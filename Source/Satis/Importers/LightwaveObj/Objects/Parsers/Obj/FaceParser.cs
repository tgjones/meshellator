namespace Satis.Importers.LightwaveObj.Objects.Parsers.Obj
{
	public class FaceParser : LineParser
	{
		private Face face;
		public int[] vindices;
		public int[] nindices;
		public int[] tindices;
		private Vertex[] vertices;
		private Vertex[] normals;
		private TextureCoordinate[] textures;
		private readonly WavefrontObject _object = null;

		public FaceParser(WavefrontObject @object)
		{
			_object = @object;
		}

		public override void Parse()
		{
			face = new Face();
			switch (Words.Length)
			{
				case 4:
					ParseTriangles();
					break;
				case 5:
					ParseQuad();
					break;
				default:
					break;
			}
		}

		private void ParseTriangles()
		{
			face.Type = FaceType.Triangles;
			ParseLine(3);
		}

		private void ParseLine(int vertexCount)
		{
			string[] rawFaces = null;

			vindices = new int[vertexCount];
			nindices = new int[vertexCount];
			tindices = new int[vertexCount];
			vertices = new Vertex[vertexCount];
			normals = new Vertex[vertexCount];
			textures = new TextureCoordinate[vertexCount];

			for (int i = 1; i <= vertexCount; i++)
			{
				rawFaces = Words[i].Split('/');

				// v
				int currentValue = int.Parse(rawFaces[0]);
				vindices[i - 1] = currentValue - 1;
				// save vertex
				vertices[i - 1] = _object.Vertices[currentValue - 1];	// -1 because references starts at 1

				if (rawFaces.Length == 1)
				{
					continue;
				}

				// save texcoords
				if (!string.IsNullOrEmpty(rawFaces[1]))
				{
					currentValue = int.Parse(rawFaces[1]);
					if (currentValue <= _object.Textures.Count)  // This is to compensate the fact that if no texture is in the obj file, sometimes '1' is put instead of 'blank' (we find coord1/1/coord3 instead of coord1//coord3 or coord1/coord3)
					{
						tindices[i - 1] = currentValue - 1;
						textures[i - 1] = _object.Textures[currentValue - 1]; // -1 because references starts at 1
					}
				}

				// save normal
				currentValue = int.Parse(rawFaces[2]);

				nindices[i - 1] = currentValue - 1;
				normals[i - 1] = _object.Normals[currentValue - 1]; 	// -1 because references starts at 1
			}
		}


		private void ParseQuad()
		{
			face.Type = FaceType.Quads;
			ParseLine(4);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			Group group = wavefrontObject.CurrentGroup;

			if (group == null)
			{
				group = new Group("Default created by loader");
				wavefrontObject.Groups.Add(group);
				wavefrontObject.GroupsDirectAccess.Add(group.Name, group);
				wavefrontObject.CurrentGroup = group;
			}

			if (vertices.Length == 3)
			{
				// Add list of vertex/normal/texcoord to current group
				// Each object keeps a list of its own data, apart from the global list
				group.Vertices.Add(this.vertices[0]);
				group.Vertices.Add(this.vertices[1]);
				group.Vertices.Add(this.vertices[2]);
				group.Normals.Add(this.normals[0]);
				group.Normals.Add(this.normals[1]);
				group.Normals.Add(this.normals[2]);
				group.TextureCoordinates.Add(this.textures[0]);
				group.TextureCoordinates.Add(this.textures[1]);
				group.TextureCoordinates.Add(this.textures[2]);
				group.Indices.Add(group.IndexCount++);
				group.Indices.Add(group.IndexCount++);
				group.Indices.Add(group.IndexCount++);	// create index list for current object
			}
			else
			{
				// Add list of vertex/normal/texcoord to current group
				// Each object keeps a list of its own data, apart from the global list
				group.Vertices.Add(this.vertices[0]);
				group.Vertices.Add(this.vertices[1]);
				group.Vertices.Add(this.vertices[2]);
				group.Vertices.Add(this.vertices[3]);
				group.Normals.Add(this.normals[0]);
				group.Normals.Add(this.normals[1]);
				group.Normals.Add(this.normals[2]);
				group.Normals.Add(this.normals[3]);
				group.TextureCoordinates.Add(this.textures[0]);
				group.TextureCoordinates.Add(this.textures[1]);
				group.TextureCoordinates.Add(this.textures[2]);
				group.TextureCoordinates.Add(this.textures[3]);
				group.Indices.Add(group.IndexCount++);
				group.Indices.Add(group.IndexCount++);
				group.Indices.Add(group.IndexCount++);
				group.Indices.Add(group.IndexCount++);	// create index list for current object
			}

			face.VertexIndices = vindices;
			face.NormalIndices = nindices;
			face.TextureIndices = tindices;
			face.Normals = normals;
			face.Vertices = vertices;
			face.TextureCoordinates = textures;

			wavefrontObject.CurrentGroup.AddFace(face);
		}
	}
}