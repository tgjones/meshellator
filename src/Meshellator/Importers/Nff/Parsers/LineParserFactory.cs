using System;
using System.Collections.Generic;

namespace Meshellator.Importers.Nff.Parsers
{
	public static class LineParserFactory
	{
		private static readonly Dictionary<string, LineParser> Parsers;

		static LineParserFactory()
		{
			Parsers = new Dictionary<string, LineParser>
			{
				{ "s", new SphereParser() },
				{ "x-sphere", new SphereParser() },
				{ "x-teapot", new TeapotParser() },
				{ "x-cube", new CubeParser() },
				{ "x-cylinder", new CylinderParser() },
				{ "x-plane", new PlaneParser() },
				{ "x-torus", new TorusParser() },
				{ "f", new MaterialParser() },
				{ "#", new CommentParser() },
				{ "tess", new TessellationParser() }
			};
		}

		public static LineParser GetParser(string[] words)
		{
			if (words.Length == 0)
				return new DefaultParser();

			if (!Parsers.ContainsKey(words[0]))
				throw new InvalidOperationException("Cannot find parser for line starting with '" + words[0] + "'");

			return Parsers[words[0]];
		}
	}
}