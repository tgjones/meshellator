namespace Meshellator.Importers.Obj.Objects.Parsers.Obj
{
	public class TextureCoordinateParser : LineParser
	{
		private TextureCoordinate _coordinate;

		public override void Parse()
		{
			_coordinate = new TextureCoordinate();
			if (Words.Length >= 2)
				_coordinate.U = float.Parse(Words[1]);

			if (Words.Length >= 3)
				_coordinate.V = 1 - float.Parse(Words[2]); // OBJ origin is at upper left, OpenGL origin is	 at lower left.

			if (Words.Length >= 4)
				_coordinate.W = float.Parse(Words[3]);

		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			wavefrontObject.Textures.Add(_coordinate);
		}
	}
}