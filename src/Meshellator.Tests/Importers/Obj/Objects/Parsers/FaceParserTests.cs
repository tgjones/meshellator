using Meshellator.Importers.Obj.Objects;
using Meshellator.Importers.Obj.Objects.Parsers.Obj;
using NUnit.Framework;

namespace Meshellator.Tests.Importers.Obj.Objects.Parsers
{
	/// <summary>
	/// Based on information from https://github.com/roastedamoeba/meshellator/issues/1.
	/// </summary>
	[TestFixture]
	public class FaceParserTests
	{
		[Test]
		public void CanParseStyle1()
		{
			// Arrange.
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vertex());
			wavefrontObject.Normals.Add(new Vertex());
			const string line = "f 1/1/1 1/1/1 1/1/1";
			FaceParser parser = (FaceParser) new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			// Act.
			parser.Parse();
		}

		[Test]
		public void CanParseStyle2()
		{
			// Arrange.
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vertex());
			wavefrontObject.Normals.Add(new Vertex());
			const string line = "f 1/1/1 1/1/1 1/1/1 1/1/1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			// Act.
			parser.Parse();
		}

		[Test]
		public void CanParseStyle3()
		{
			// Arrange.
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vertex());
			wavefrontObject.Normals.Add(new Vertex());
			const string line = "f 1//1 1//1 1//1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			// Act.
			parser.Parse();
		}

		[Test]
		public void CanParseStyle4()
		{
			// Arrange.
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vertex());
			wavefrontObject.Normals.Add(new Vertex());
			const string line = "f 1//1 1//1 1//1 1//1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			// Act.
			parser.Parse();
		}

		[Test]
		public void CanParseStyle5()
		{
			// Arrange.
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vertex());
			wavefrontObject.Normals.Add(new Vertex());
			const string line = "f 1/1 1/1 1/1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			// Act.
			parser.Parse();
		}

		[Test]
		public void CanParseStyle6()
		{
			// Arrange.
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vertex());
			wavefrontObject.Normals.Add(new Vertex());
			const string line = "f 1/1 1/1 1/1 1/1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			// Act.
			parser.Parse();
		}

		[Test]
		public void CanParseStyle7()
		{
			// Arrange.
			WavefrontObject wavefrontObject = new WavefrontObject();
			wavefrontObject.Vertices.Add(new Vertex());
			wavefrontObject.Normals.Add(new Vertex());
			const string line = "f 1 1 1";
			FaceParser parser = (FaceParser)new ObjLineParserFactory(wavefrontObject).GetLineParser(line);

			// Act.
			parser.Parse();
		}
	}
}