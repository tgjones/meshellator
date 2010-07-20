using System.IO;
using NUnit.Framework;
using Satis.Importers.Autodesk3ds;

namespace Satis.Tests
{
	[TestFixture]
	public class Simple3dsTest
	{
		[Test]
		public void CanImport()
		{
			FileStream fileStream = File.Open(@"C:\Users\Tim Jones\Documents\3D Models\85-nissan-fairlady.3DS", FileMode.Open);
			Autodesk3dsImporter importer = new Autodesk3dsImporter();
			Scene scene = importer.ImportFile(fileStream, null);
			fileStream.Close();
		}
	}
}