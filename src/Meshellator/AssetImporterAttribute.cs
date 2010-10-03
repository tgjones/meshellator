using System;
using System.ComponentModel.Composition;

namespace Meshellator
{
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class AssetImporterAttribute : ExportAttribute, IAssetImporterMetadata
	{
		public string Extension { get; set; }
		public string Name { get; set; }

		public AssetImporterAttribute(string extension, string name)
			: base(typeof(IAssetImporter))
		{
			Extension = extension;
			Name = name;
		}
	}
}