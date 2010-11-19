using System.Collections.Generic;
using System.IO;
using Meshellator.Importers.LightwaveObj.Objects.Parsers;
using Meshellator.Importers.LightwaveObj.Objects.Parsers.Obj;

namespace Meshellator.Importers.LightwaveObj.Objects
{
	public class WavefrontObject
	{
		public List<Vertex> Vertices {get;set;}
		public List<Vertex> Normals { get; set; }
		public List<TextureCoordinate> Textures { get; set; }
		public List<Group> Groups { get; set; }
		public Dictionary<string, Material> Materials { get; set; }
		public string FileName { get; set; }

		private ObjLineParserFactory _parserFactory;

		public Material CurrentMaterial { get; set; }
		public Group CurrentGroup { get; set; }

		public string Contextfolder { get; private set; }

		public float Radius { get; set; }

		public WavefrontObject(string fileName)
		{
			Vertices = new List<Vertex>();
			Normals = new List<Vertex>();
			Textures = new List<TextureCoordinate>();
			Groups = new List<Group>();
			Materials = new Dictionary<string, Material>();

			FileName = fileName;
			Contextfolder = Path.GetDirectoryName(fileName);
			Parse(fileName);
			CalculateRadius();
		}

		private void CalculateRadius()
		{
			foreach (Vertex vertex in Vertices)
			{
				float currentNorm = vertex.Length();
				if (currentNorm > Radius)
					Radius = currentNorm;
			}
		}

		private void Parse(string fileName)
		{
			_parserFactory = new ObjLineParserFactory(this);

			StreamReader reader = new StreamReader(fileName);
			string currentLine = null;
			while ((currentLine = reader.ReadLine()) != null)
				ParseLine(currentLine);
			reader.Close();
		}

		private void ParseLine(string currentLine)
		{
			if (string.IsNullOrEmpty(currentLine))
				return;

			LineParser parser = _parserFactory.GetLineParser(currentLine);
			parser.Parse();
			parser.IncorporateResults(this);
		}
	}
}