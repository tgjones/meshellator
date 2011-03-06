using System;
using System.Collections.Generic;

namespace Meshellator.Importers.LightwaveObj.Objects
{
	public class Group
	{
		public string Name { get; private set; }
		public Vertex Min { get; private set; }
		public Material Material { get; set; }
		public List<Face> Faces { get; private set; }

		public List<int> Indices = new List<int>();
		public List<Vertex> Vertices = new List<Vertex>();
		public List<Vertex> Normals = new List<Vertex>();
		public List<TextureCoordinate> TextureCoordinates = new List<TextureCoordinate>();
		public int IndexCount;

		public Group()
			: this("default")
		{
			
		}

		public Group(string name)
		{
			Faces = new List<Face>();
			IndexCount = 0;
			Name = name;
		}

		public void AddFace(Face face)
		{
			Faces.Add(face);
		}

		public void Pack()
		{
			float minX = 0;
			float minY = 0;
			float minZ = 0;
			foreach (Face face in Faces)
			{
				foreach (Vertex vertex in face.Vertices)
				{
					if (Math.Abs(vertex.X) > minX)
						minX = Math.Abs(vertex.X);
					if (Math.Abs(vertex.Y) > minY)
						minY = Math.Abs(vertex.Y);
					if (Math.Abs(vertex.Z) > minZ)
						minZ = Math.Abs(vertex.Z);
				}
			}

			Min = new Vertex(minX, minY, minZ);
		}
	}
}