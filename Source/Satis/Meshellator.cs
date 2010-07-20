using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using Nexus;
using Satis.Primitives;

namespace Satis
{
	public class Meshellator
	{
		private static Meshellator _instance;
		private static Meshellator Instance
		{
			get { return _instance ?? (_instance = new Meshellator()); }
		}

		private readonly CompositionContainer _container;

		[ImportMany]
		private Lazy<IAssetImporter, IAssetImporterMetadata>[] AssetImporters { get; set; }

		private Meshellator()
		{
			AggregateCatalog catalog = new AggregateCatalog();
			catalog.Catalogs.Add(new DirectoryCatalog(".", "*.dll"));
			catalog.Catalogs.Add(new DirectoryCatalog(".", "*.exe"));

			_container = new CompositionContainer(catalog);
			_container.ComposeParts(this);
		}

		public static Scene ImportFromFile(string fileName)
		{
			// Look for importer which handles this file extension.
			string fileExtension = Path.GetExtension(fileName).ToUpper();
			IAssetImporter assetImporter = Instance.AssetImporters.SingleOrDefault(l => l.Metadata.Extension.ToUpper() == fileExtension).Value;
			if (assetImporter == null)
				throw new ArgumentException("Could not find importer for extension '" + fileExtension + "'");

			FileStream fileStream = File.OpenRead(fileName);
			return assetImporter.ImportFile(fileStream, fileName);
		}

		public static Scene CreateFromCube(float size)
		{
			return CreateFromPrimitive(new CubeTessellator(size));
		}

		public static Scene CreateFromCylinder(float radius, float height, int tessellationLevel)
		{
			return CreateFromPrimitive(new CylinderTessellator(radius, height, tessellationLevel));
		}

		public static Scene CreateFromSphere(float radius, int tessellationLevel)
		{
			return CreateFromPrimitive(new SphereTessellator(radius, tessellationLevel));
		}

		public static Scene CreateFromTeapot(float size, int tessellationLevel)
		{
			return CreateFromPrimitive(new TeapotTessellator(size, tessellationLevel));
		}

		public static Scene CreateFromTorus(float radius, float thickness, int tessellationLevel)
		{
			return CreateFromPrimitive(new TorusTessellator(radius, thickness, tessellationLevel));
		}

		private static Scene CreateFromPrimitive(BasicPrimitiveTessellator tessellator)
		{
			tessellator.Tessellate();
			// TODO: create Scene from tessellation
			throw new NotImplementedException();
		}
	}
}