using System;

namespace Satis.Importers.Autodesk3ds
{
	class Utils3ds
	{
		static string mDF = "0.0000";

		static string mSpaces = "                                ";
		static string mZeroes = "00000000000000000000000000000000";


		public static string floatToString(float val, int width)
		{
			string str = val.ToString(mDF);
			if (str.Length >= width)
			{
				return str;
			}
			return mSpaces.Substring(0, width - str.Length) + str;
		}

		public static string intToString(int val, int width)
		{
			string str = val.ToString();
			if (str.Length >= width)
			{
				return str;
			}
			return mSpaces.Substring(0, width - str.Length) + str;
		}

		public static string intToHexString(int val, int width)
		{
			string str = val.ToString("X");
			if (str.Length >= width)
			{
				return str;
			}
			return mZeroes.Substring(0, width - str.Length) + str;
		}

		public static string intToBinString(int val, int width)
		{
			string str = Convert.ToString(val, 2);
			if (str.Length >= width)
			{
				return str;
			}
			return mZeroes.Substring(0, width - str.Length) + str;
		}
	}
}