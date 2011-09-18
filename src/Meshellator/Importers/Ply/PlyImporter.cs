using System;
using System.IO;
using Nexus;

namespace Meshellator.Importers.Ply
{
	[AssetImporter(".ply", "Polygon File Format")]
	public class PlyImporter : AssetImporterBase
	{
		public override Scene ImportFile(FileStream fileStream, string fileName)
		{
			var scene = new Scene();
			var mesh = new Mesh();
			scene.Meshes.Add(mesh);

			// Create record for this object.
			var plyFile = new PlyFile();

			// Read and parse the file's header.
			var reader = new StreamReader(fileName);
			string originalLine;
			string[] words = GetWords(reader, out originalLine);
			if (words == null || words[0] != "ply")
				throw new Exception("Missing header");

			ParseHeader(reader, plyFile, words, originalLine);

			if (plyFile.FileType != PlyFileType.Ascii)
				throw new Exception("Binary files not currently supported");

			// Go through each kind of element that we learned is in the file
			// and read them.
			foreach (var element in plyFile.Elements)
			{
				for (int i = 0; i < element.Num; i++)
				{
					words = GetWords(reader, out originalLine);
					if (words == null)
						throw new Exception("Unexpected end of file");

					var elementValue = new PlyElementValue();
					int whichWord = 0;
					foreach (var property in element.Properties)
					{
						int intVal;
						uint uintVal;
						double doubleVal;
						if (property.IsList)
						{
							GetAsciiItem(words[whichWord++], property.CountExternal, out intVal, out uintVal, out doubleVal);
							var listCount = intVal;
							for (int j = 0; j < listCount; j++)
							{
								var propertyValue = GetAsciiItem(words[whichWord++], property.ExternalType, out intVal, out uintVal, out doubleVal);
								elementValue.PropertyValues.Add(propertyValue);
							}
						}
						else
						{
							var propertyValue = GetAsciiItem(words[whichWord++], property.ExternalType, out intVal, out uintVal, out doubleVal);
							elementValue.PropertyValues.Add(propertyValue);
						}
					}
					element.ElementValues.Add(elementValue);
				}
			}

			// Now read vertex and face information.
			foreach (var element in plyFile.Elements)
			{
				switch (element.Name)
				{
					case "vertex" :
						foreach (var elementValue in element.ElementValues)
						{
							var position = new Point3D(
								(float) elementValue.PropertyValues[0],
								(float) elementValue.PropertyValues[1],
								(float) elementValue.PropertyValues[2]);
							mesh.Positions.Add(position);
						}
						break;
					case "face" :
						foreach (var elementValue in element.ElementValues)
						{
							if (elementValue.PropertyValues.Count != 3)
								throw new Exception("Only triangle faces are currently supported");

							mesh.Indices.Add((int) elementValue.PropertyValues[0]);
							mesh.Indices.Add((int) elementValue.PropertyValues[1]);
							mesh.Indices.Add((int) elementValue.PropertyValues[2]);
						}
						break;
				}
			}

			reader.Close();

			return scene;
		}

		private static void ParseHeader(StreamReader reader, PlyFile plyFile, string[] words, string originalLine)
		{
			while (words != null)
			{
				// Parse words.
				switch (words[0])
				{
					case "format":
						if (words.Length != 3)
							throw new Exception("Invalid header: format.");
						switch (words[1])
						{
							case "ascii":
								plyFile.FileType = PlyFileType.Ascii;
								break;
							case "binary_little_endian":
								plyFile.FileType = PlyFileType.BinaryLittleEndian;
								break;
							case "binary_big_endian":
								plyFile.FileType = PlyFileType.BinaryBigEndian;
								break;
							default:
								throw new Exception("Invalid header: format.");
						}
						plyFile.Version = Convert.ToSingle(words[2]);
						break;
					case "element":
						AddElement(plyFile, words);
						break;
					case "property":
						AddProperty(plyFile, words);
						break;
					case "comment":
						AddComment(plyFile, originalLine);
						break;
					case "obj_info":
						AddObjInfo(plyFile, originalLine);
						break;
					case "end_header":
						return;
				}

				words = GetWords(reader, out originalLine);
			}
		}

		/// <summary>
		/// Adds an element to a PLY file descriptor.
		/// </summary>
		/// <param name="plyFile"></param>
		/// <param name="words"></param>
		private static void AddElement(PlyFile plyFile, string[] words)
		{
			plyFile.Elements.Add(new PlyElement
			{
				Name = words[1],
				Num = Convert.ToInt32(words[2])
			});
		}

		/// <summary>
		/// Adds a property to a PLY file descriptor.
		/// </summary>
		/// <param name="plyFile"></param>
		/// <param name="words"></param>
		private static void AddProperty(PlyFile plyFile, string[] words)
		{
			var property = new PlyProperty();

			if (words[1] == "list")
			{
				property.CountExternal = GetPropertyType(words[2]);
				property.ExternalType = GetPropertyType(words[3]);
				property.Name = words[4];
				property.IsList = true;
			}
			else
			{
				property.ExternalType = GetPropertyType(words[1]);
				property.Name = words[2];
				property.IsList = false;
			}

			// Add this property to the list of properties of the current element.
			plyFile.Elements[plyFile.Elements.Count - 1].Properties.Add(property);
		}

		/// <summary>
		/// Adds a comment to a PLY file descriptor.
		/// </summary>
		/// <param name="plyFile"></param>
		/// <param name="line"></param>
		private static void AddComment(PlyFile plyFile, string line)
		{
			// Skip over 'comment' and leading spaces and tabs.
			int i = 7;
			while (line[i] == ' ' || line[i] == '\t')
				i++;

			plyFile.Comments.Add(line.Substring(i));
		}

		/// <summary>
		/// Adds some object information to a PLY file descriptor.
		/// </summary>
		/// <param name="plyFile"></param>
		/// <param name="line"></param>
		private static void AddObjInfo(PlyFile plyFile, string line)
		{
			// Skip over 'obj_info' and leading spaces and tabs.
			int i = 8;
			while (line[i] == ' ' || line[i] == '\t')
				i++;

			plyFile.ObjectInformationItems.Add(line.Substring(i));
		}

		#region Helper methods

		/// <summary>
		/// Gets a text line from a file and break it up into words.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="originalLine"></param>
		/// <returns></returns>
		private static string[] GetWords(StreamReader reader, out string originalLine)
		{
			if (reader.EndOfStream)
			{
				originalLine = null;
				return null;
			}

			// Read in a line.
			originalLine = reader.ReadLine();
			if (originalLine == null)
				return null;

			// Split by spaces.
			string[] result = originalLine.Split(new[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries);

			return result;
		}

		private static PlyPropertyType GetPropertyType(string typeName)
		{
			switch (typeName)
			{
				case "char":
					return PlyPropertyType.Char;
				case "short":
					return PlyPropertyType.Short;
				case "int":
				case "int32" :
					return PlyPropertyType.Int;
				case "uchar":
				case "uint8":
					return PlyPropertyType.UChar;
				case "ushort":
					return PlyPropertyType.UShort;
				case "uint":
					return PlyPropertyType.UInt;
				case "float":
				case "float32":
					return PlyPropertyType.Float;
				case "double":
					return PlyPropertyType.Double;
				default:
					throw new ArgumentException("Unknown type: " + typeName);
			}
		}

		private static object GetAsciiItem(string word, PlyPropertyType type,
			out int intVal, out uint uintVal, out double doubleVal)
		{
			switch (type)
			{
				case PlyPropertyType.Char:
				case PlyPropertyType.UChar:
				case PlyPropertyType.Short:
				case PlyPropertyType.UShort:
				case PlyPropertyType.Int:
					intVal = Convert.ToInt32(word);
					uintVal = (uint)intVal;
					doubleVal = intVal;
					break;
				case PlyPropertyType.UInt:
					uintVal = Convert.ToUInt32(word);
					intVal = (int)uintVal;
					doubleVal = uintVal;
					break;
				case PlyPropertyType.Float:
				case PlyPropertyType.Double:
					doubleVal = Convert.ToDouble(word);
					intVal = (int)doubleVal;
					uintVal = (uint)doubleVal;
					break;
				default:
					throw new ArgumentException("Unknown type: " + type);
			}

			switch (type)
			{
				case PlyPropertyType.Char:
					return Convert.ToByte(word);
				case PlyPropertyType.Double:
					return Convert.ToDouble(word);
				case PlyPropertyType.Float:
					return Convert.ToSingle(word);
				case PlyPropertyType.Int:
					return Convert.ToInt32(word);
				case PlyPropertyType.Short:
					return Convert.ToInt16(word);
				case PlyPropertyType.UChar:
					return Convert.ToByte(word);
				case PlyPropertyType.UInt:
					return Convert.ToUInt32(word);
				case PlyPropertyType.UShort:
					return Convert.ToUInt16(word);
				default:
					throw new ArgumentException("Unknown type: " + type);
			}
		}

		#endregion
	}
}