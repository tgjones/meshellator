using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;

namespace Satis
{
	public class Importer
	{
		private static Importer _instance;
		private static Importer Instance
		{
			get { return _instance ?? (_instance = new Importer()); }
		}

		private readonly CompositionContainer _container;

		[ImportMany]
		private Lazy<IAssetImporter, IAssetImporterMetadata>[] AssetImporters { get; set; }

		private Importer()
		{
			AggregateCatalog catalog = new AggregateCatalog();
			catalog.Catalogs.Add(new DirectoryCatalog(".", "*.dll"));
			catalog.Catalogs.Add(new DirectoryCatalog(".", "*.exe"));

			_container = new CompositionContainer(catalog);
			_container.ComposeParts(this);
		}

		public static Scene ImportFile(string fileName)
		{
			// Look for importer which handles this file extension.
			string fileExtension = Path.GetExtension(fileName).ToUpper();
			IAssetImporter assetImporter = Instance.AssetImporters.SingleOrDefault(l => l.Metadata.Extension.ToUpper() == fileExtension).Value;
			if (assetImporter == null)
				throw new ArgumentException("Could not find importer for extension '" + fileExtension + "'");

			FileStream fileStream = File.OpenRead(fileName);
			return assetImporter.ImportFile(fileStream, fileName);
		}
	}
}