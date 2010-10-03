using System;
using System.IO;

namespace Meshellator.Importers.Autodesk3ds
{
	class Decode3ds
	{
		private TextDecode3ds mDecCont = null;
		private int mDecLevel = 0;
		private int mLevel = 0;
		private string mNL = "\n";

		public Decode3ds(TextDecode3ds decode, int level)
		{
			mDecCont = decode;
			mDecLevel = level;
			mDecCont.clear();
			mNL = Environment.NewLine;
		}

		public void enter()
		{
			mLevel++;
		}

		public void leave()
		{
			mLevel--;
		}

		public void println(string str)
		{
			for (int i = 0; i < mLevel; i++)
			{
				mDecCont.mText.Append("  ");
			}
			mDecCont.mText.Append(str + mNL);
		}

		public void printBytes(BinaryReader reader, int n)
		{
			if (mDecLevel >= Scene3ds.DECODE_ALL)
			{
				while (n > 0)
				{
					for (int i = 0; i < mLevel; i++)
					{
						mDecCont.mText.Append("  ");
					}
					int run = n;
					if (run > 20)
					{
						run = 20;
					}
					for (int i = 0; i < run; i++)
					{
						mDecCont.mText.Append(intToHex(reader.ReadByte(), 2) + " ");
					}
					n -= run;
					mDecCont.mText.Append(mNL);
				}
			}
		}

		public static string intToHex(int val, int digits)
		{
			char[] buf = new char[8];
			char[] lut = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

			int shift = (digits - 1) * 4;
			for (int i = 0; i < digits; i++)
			{
				buf[i] = lut[(val >> shift) & 0xf];
				shift -= 4;
			}
			return new string(buf, 0, digits);
		}

		public void printHead(int id, int length)
		{
			string name = "UNKNOWN";
			bool known = false;
			for (int i = 0; i < mChunkInfo.Length; i++)
			{
				if (mChunkInfo[i].id == id)
				{
					name = mChunkInfo[i].name;
					known = mChunkInfo[i].known;
					break;
				}
			}
			if ((known == true) || (mDecLevel >= Scene3ds.DECODE_USED_PARAMS))
			{
				println(name + "  id=0x" + intToHex(id, 4) + " length=" + length);
			}
		}


		private class ChunkInfo
		{
			internal bool known;
			internal int id;
			internal string name;

			public ChunkInfo(bool known, int id, string name)
			{
				this.known = known;
				this.id = id;
				this.name = name;
			}
		}

		private enum ChunkType
		{
			CHUNK_M3DMAGIC = 0x4D4D,
			CHUNK_MDATA = 0x3D3D,
			CHUNK_MAT_ENTRY = 0xAFFF,
			CHUNK_MAT_NAME = 0xA000,
			CHUNK_NAMED_OBJECT = 0x4000,
			CHUNK_N_TRI_OBJECT = 0x4100,
			CHUNK_POINT_ARRAY = 0x4110,
			CHUNK_TEX_VERTS = 0x4140,
			CHUNK_MESH_TEXTURE_INFO = 0x4170,
			CHUNK_MESH_MATRIX = 0x4160,
			//		        CHUNK_MESH_COLOR          = 0x4165,
			CHUNK_FACE_ARRAY = 0x4120,
			CHUNK_MSH_MAT_GROUP = 0x4130,
			CHUNK_SMOOTH_GROUP = 0x4150,
			CHUNK_N_CAMERA = 0x4700,
			//		        CHUNK_CAM_SEE_CONE        = 0x4710,
			CHUNK_CAM_RANGES = 0x4720,
			CHUNK_KFDATA = 0xB000,
			CHUNK_KFSEG = 0xB008,
			CHUNK_OBJECT_NODE_TAG = 0xB002,
			CHUNK_NODE_ID = 0xB030,
			CHUNK_NODE_HDR = 0xB010,
			CHUNK_PIVOT = 0xB013,
			CHUNK_POS_TRACK_TAG = 0xB020,
			CHUNK_ROT_TRACK_TAG = 0xB021,
			CHUNK_SCL_TRACK_TAG = 0xB022,
			CHUNK_MORPH_TRACK_TAG = 0xB026,
			CHUNK_HIDE_TRACK_TAG = 0xB029,
			CHUNK_TARGET_NODE_TAG = 0xB004,
			CHUNK_CAMERA_NODE_TAG = 0xB003,
			CHUNK_FOV_TRACK_TAG = 0xB023,
			CHUNK_ROLL_TRACK_TAG = 0xB024
			//		    CHUNK_AMBIENT_NODE_TAG        = 0xB001;
		}

		private static readonly ChunkInfo[] mChunkInfo =
	{ 
		// 3DS File Chunk IDs

		new ChunkInfo(true,  (int) ChunkType.CHUNK_M3DMAGIC,         "M3DMAGIC"              ),
		new ChunkInfo(false, 0x2D2D,                 "SMAGIC"                ),
		new ChunkInfo(false, 0x2D3D,                 "LMAGIC"                ),
		new ChunkInfo(false, 0x3DAA,                 "MLIBMAGIC"             ),
		new ChunkInfo(false, 0x3DFF,                 "MATMAGIC"              ),
		new ChunkInfo(false, 0x0002,                 "M3D_VERSION"           ),
		new ChunkInfo(false, 0x0005,                 "M3D_KFVERSION"         ),
	
		// Mesh Chunk Ids

		new ChunkInfo(true, (int) ChunkType.CHUNK_MDATA,             "MDATA"                 ),
		new ChunkInfo(false, 0x3D3E,                  "MESH_VERSION"          ),
		new ChunkInfo(false, 0x0010,                  "COLOR_F"               ),
		new ChunkInfo(false, 0x0011,                  "COLOR_24"              ),
		new ChunkInfo(false, 0x0012,                  "LIN_COLOR_24"          ),
		new ChunkInfo(false, 0x0013,                  "LIN_COLOR_F"           ),
		new ChunkInfo(false, 0x0030,                  "INT_PERCENTAGE"        ),
		new ChunkInfo(false, 0x0031,                  "FLOAT_PERCENTAGE"      ),

		new ChunkInfo(false, 0x0100,                  "MASTER_SCALE"          ),

		new ChunkInfo(false, 0x1100,                  "BIT_MAP"               ),
		new ChunkInfo(false, 0x1101,                  "USE_BIT_MAP"           ),
		new ChunkInfo(false, 0x1200,                  "SOLID_BGND"            ),
		new ChunkInfo(false, 0x1201,                  "USE_SOLID_BGND"        ),
		new ChunkInfo(false, 0x1300,                  "V_GRADIENT"            ),
		new ChunkInfo(false, 0x1301,                  "USE_V_GRADIENT"        ),

		new ChunkInfo(false, 0x1400,                  "LO_SHADOW_BIAS"        ),
		new ChunkInfo(false, 0x1410,                  "HI_SHADOW_BIAS"        ),
		new ChunkInfo(false, 0x1420,                  "SHADOW_MAP_SIZE"       ),
		new ChunkInfo(false, 0x1430,                  "SHADOW_SAMPLES"        ),
		new ChunkInfo(false, 0x1440,                  "SHADOW_RANGE"          ),
		new ChunkInfo(false, 0x1450,                  "SHADOW_FILTER"         ),
		new ChunkInfo(false, 0x1460,                  "RAY_BIAS"              ),
 
		new ChunkInfo(false, 0x1500,                  "O_CONSTS"              ),
 
		new ChunkInfo(false, 0x2100,                  "AMBIENT_LIGHT"         ),

		new ChunkInfo(false, 0x2200,                  "FOG"                   ),
		new ChunkInfo(false, 0x2201,                  "USE_FOG"               ),
		new ChunkInfo(false, 0x2210,                  "FOG_BGND"              ),
		new ChunkInfo(false, 0x2300,                  "DISTANCE_CUE"          ),
		new ChunkInfo(false, 0x2301,                  "USE_DISTANCE_CUE"      ),
		new ChunkInfo(false, 0x2302,                  "LAYER_FOG"             ),
		new ChunkInfo(false, 0x2303,                  "USE_LAYER_FOG"         ),
		new ChunkInfo(false, 0x2310,                  "DCUE_BGND"             ),

		new ChunkInfo(false, 0x3000,                  "DEFAULT_VIEW"          ),
		new ChunkInfo(false, 0x3010,                  "VIEW_TOP"              ),
		new ChunkInfo(false, 0x3020,                  "VIEW_BOTTOM"           ),
		new ChunkInfo(false, 0x3030,                  "VIEW_LEFT"             ),
		new ChunkInfo(false, 0x3040,                  "VIEW_RIGHT"            ),
		new ChunkInfo(false, 0x3050,                  "VIEW_FRONT"            ),
		new ChunkInfo(false, 0x3060,                  "VIEW_BACK"             ),
		new ChunkInfo(false, 0x3070,                  "VIEW_USER"             ),
		new ChunkInfo(false, 0x3080,                  "VIEW_CAMERA"           ),
		new ChunkInfo(false, 0x3090,                  "VIEW_WINDOW"           ),

		new ChunkInfo(true, (int) ChunkType.CHUNK_NAMED_OBJECT,      "NAMED_OBJECT"          ),
		new ChunkInfo(false, 0x4010,                  "OBJ_HIDDEN"            ),
		new ChunkInfo(false, 0x4011,                  "OBJ_VIS_LOFTER"        ),
		new ChunkInfo(false, 0x4012,                  "OBJ_DOESNT_CAST"       ),
		new ChunkInfo(false, 0x4013,                  "OBJ_MATTE"             ),
		new ChunkInfo(false, 0x4014,                  "OBJ_FAST"              ),
		new ChunkInfo(false, 0x4015,                  "OBJ_PROCEDURAL"        ),
		new ChunkInfo(false, 0x4016,                  "OBJ_FROZEN"            ),
		new ChunkInfo(false, 0x4017,                  "OBJ_DONT_RCVSHADOW"    ),

		new ChunkInfo(true, (int) ChunkType.CHUNK_N_TRI_OBJECT,      "N_TRI_OBJECT"          ),

		new ChunkInfo(true, (int) ChunkType.CHUNK_POINT_ARRAY,       "POINT_ARRAY"           ),
		new ChunkInfo(false, 0x4111,                  "POINT_FLAG_ARRAY"      ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_FACE_ARRAY,        "FACE_ARRAY"            ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_MSH_MAT_GROUP,     "MSH_MAT_GROUP"         ),
		new ChunkInfo(false, 0x4131,                  "OLD_MAT_GROUP"         ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_TEX_VERTS,         "TEX_VERTS"             ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_SMOOTH_GROUP,      "SMOOTH_GROUP"          ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_MESH_MATRIX,       "MESH_MATRIX"           ),
		new ChunkInfo(false, 0x4165,                  "MESH_COLOR"            ),
//		new ChunkInfo(true, (int) ChunkType.CHUNK_MESH_COLOR,        "MESH_COLOR"            ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_MESH_TEXTURE_INFO, "MESH_TEXTURE_INFO"     ),
		new ChunkInfo(false, 0x4181,                  "PROC_NAME"             ),
		new ChunkInfo(false, 0x4182,                  "PROC_DATA"             ),
		new ChunkInfo(false, 0x4190,                  "MSH_BOXMAP"            ),

		new ChunkInfo(false, 0x4400,                  "N_D_L_OLD"             ),

		new ChunkInfo(false, 0x4500,                  "N_CAM_OLD"             ),

		new ChunkInfo(false, 0x4600,                  "N_DIRECT_LIGHT"        ),
		new ChunkInfo(false, 0x4610,                  "DL_SPOTLIGHT"          ),
		new ChunkInfo(false, 0x4620,                  "DL_OFF"                ),
		new ChunkInfo(false, 0x4625,                  "DL_ATTENUATE"          ),
		new ChunkInfo(false, 0x4627,                  "DL_RAYSHAD"            ),
		new ChunkInfo(false, 0x4630,                  "DL_SHADOWED"           ),
		new ChunkInfo(false, 0x4640,                  "DL_LOCAL_SHADOW"       ),
		new ChunkInfo(false, 0x4641,                  "DL_LOCAL_SHADOW2"      ),
		new ChunkInfo(false, 0x4650,                  "DL_SEE_CONE"           ),
		new ChunkInfo(false, 0x4651,                  "DL_SPOT_RECTANGULAR"   ),
		new ChunkInfo(false, 0x4652,                  "DL_SPOT_OVERSHOOT"     ),
		new ChunkInfo(false, 0x4653,                  "DL_SPOT_PROJECTOR"     ),
		new ChunkInfo(false, 0x4654,                  "DL_EXCLUDE"            ),
		new ChunkInfo(false, 0x4655,                  "DL_RANGE"              ),
		new ChunkInfo(false, 0x4656,                  "DL_SPOT_ROLL"          ),
		new ChunkInfo(false, 0x4657,                  "DL_SPOT_ASPECT"        ),
		new ChunkInfo(false, 0x4658,                  "DL_RAY_BIAS"           ),
		new ChunkInfo(false, 0x4659,                  "DL_INNER_RANGE"        ),
		new ChunkInfo(false, 0x465A,                  "DL_OUTER_RANGE"        ),
		new ChunkInfo(false, 0x465B,                  "DL_MULTIPLIER"         ),

		new ChunkInfo(false, 0x4680,                  "N_AMBIENT_LIGHT"       ),

		new ChunkInfo(true, (int) ChunkType.CHUNK_N_CAMERA,          "N_CAMERA"              ),
		new ChunkInfo(false, 0x4710,                  "CAM_SEE_CONE"          ),
//		new ChunkInfo(true, (int) ChunkType.CHUNK_CAM_SEE_CONE,      "CAM_SEE_CONE"          ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_CAM_RANGES,        "CAM_RANGES"            ),

		new ChunkInfo(false, 0x4F00,                  "HIERARCHY"             ),
		new ChunkInfo(false, 0x4F10,                  "PARENT_OBJECT"         ),
		new ChunkInfo(false, 0x4F20,                  "PIVOT_OBJECT"          ),
		new ChunkInfo(false, 0x4F30,                  "PIVOT_LIMITS"          ),
		new ChunkInfo(false, 0x4F40,                  "PIVOT_ORDER"           ),
		new ChunkInfo(false, 0x4F50,                  "XLATE_RANGE"           ),

		new ChunkInfo(false, 0x5000,                  "POLY_2D"               ),

		// Flags in shaper file that tell whether polys make up an ok shape 

		new ChunkInfo(false, 0x5010,                  "SHAPE_OK"              ),
		new ChunkInfo(false, 0x5011,                  "SHAPE_NOT_OK"          ),

		new ChunkInfo(false, 0x5020,                  "SHAPE_HOOK"            ),

		new ChunkInfo(false, 0x6000,                  "PATH_3D"               ),
		new ChunkInfo(false, 0x6005,                  "PATH_MATRIX"           ),
		new ChunkInfo(false, 0x6010,                  "SHAPE_2D"              ),
		new ChunkInfo(false, 0x6020,                  "M_SCALE"               ),
		new ChunkInfo(false, 0x6030,                  "M_TWIST"               ),
		new ChunkInfo(false, 0x6040,                  "M_TEETER"              ),
		new ChunkInfo(false, 0x6050,                  "M_FIT"                 ),
		new ChunkInfo(false, 0x6060,                  "M_BEVEL"               ),
		new ChunkInfo(false, 0x6070,                  "XZ_CURVE"              ),
		new ChunkInfo(false, 0x6080,                  "YZ_CURVE"              ),
		new ChunkInfo(false, 0x6090,                  "INTERPCT"              ),
		new ChunkInfo(false, 0x60A0,                  "DEFORM_LIMIT"          ),

		// Flags for Modeler options

		new ChunkInfo(false, 0x6100,                  "USE_CONTOUR"           ),
		new ChunkInfo(false, 0x6110,                  "USE_TWEEN"             ),
		new ChunkInfo(false, 0x6120,                  "USE_SCALE"             ),
		new ChunkInfo(false, 0x6130,                  "USE_TWIST"             ),
		new ChunkInfo(false, 0x6140,                  "USE_TEETER"            ),
		new ChunkInfo(false, 0x6150,                  "USE_FIT"               ),
		new ChunkInfo(false, 0x6160,                  "USE_BEVEL"             ),

		// Viewport description chunks

		new ChunkInfo(false, 0x7000,                  "VIEWPORT_LAYOUT_OLD"   ),
		new ChunkInfo(false, 0x7010,                  "VIEWPORT_DATA_OLD"     ),
		new ChunkInfo(false, 0x7001,                  "VIEWPORT_LAYOUT"       ),
		new ChunkInfo(false, 0x7011,                  "VIEWPORT_DATA"         ),
		new ChunkInfo(false, 0x7012,                  "VIEWPORT_DATA_3"       ),
		new ChunkInfo(false, 0x7020,                  "VIEWPORT_SIZE"         ),
		new ChunkInfo(false, 0x7030,                  "NETWORK_VIEW"          ),

		// External Application Data

		new ChunkInfo(false, 0x8000,                  "XDATA_SECTION"         ),
		new ChunkInfo(false, 0x8001,                  "XDATA_ENTRY"           ),
		new ChunkInfo(false, 0x8002,                  "XDATA_APPNAME"         ),
		new ChunkInfo(false, 0x8003,                  "XDATA_STRING"          ),
		new ChunkInfo(false, 0x8004,                  "XDATA_FLOAT"           ),
		new ChunkInfo(false, 0x8005,                  "XDATA_DOUBLE"          ),
		new ChunkInfo(false, 0x8006,                  "XDATA_SHORT"           ),
		new ChunkInfo(false, 0x8007,                  "XDATA_LONG"            ),
		new ChunkInfo(false, 0x8008,                  "XDATA_VOID"            ),
		new ChunkInfo(false, 0x8009,                  "XDATA_GROUP"           ),
		new ChunkInfo(false, 0x800A,                  "XDATA_RFU6"            ),
		new ChunkInfo(false, 0x800B,                  "XDATA_RFU5"            ),
		new ChunkInfo(false, 0x800C,                  "XDATA_RFU4"            ),
		new ChunkInfo(false, 0x800D,                  "XDATA_RFU3"            ),
		new ChunkInfo(false, 0x800E,                  "XDATA_RFU2"            ),
		new ChunkInfo(false, 0x800F,                  "XDATA_RFU1"            ),


		new ChunkInfo(false, 0x80F0,                  "PARENT_NAME"           ),

		// Material Chunk IDs

		new ChunkInfo(true, (int) ChunkType.CHUNK_MAT_ENTRY,         "MAT_ENTRY"             ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_MAT_NAME,          "MAT_NAME"              ),
		new ChunkInfo(false, 0xA010,                  "MAT_AMBIENT"           ),
		new ChunkInfo(false, 0xA020,                  "MAT_DIFFUSE"           ),
		new ChunkInfo(false, 0xA030,                  "MAT_SPECULAR"          ),
		new ChunkInfo(false, 0xA040,                  "MAT_SHININESS"         ),
		new ChunkInfo(false, 0xA041,                  "MAT_SHIN2PCT"          ),
		new ChunkInfo(false, 0xA042,                  "MAT_SHIN3PCT"          ),
		new ChunkInfo(false, 0xA050,                  "MAT_TRANSPARENCY"      ),
		new ChunkInfo(false, 0xA052,                  "MAT_XPFALL"            ),
		new ChunkInfo(false, 0xA053,                  "MAT_REFBLUR"           ),

		new ChunkInfo(false, 0xA080,                  "MAT_SELF_ILLUM"        ),
		new ChunkInfo(false, 0xA081,                  "MAT_TWO_SIDE"          ),
		new ChunkInfo(false, 0xA082,                  "MAT_DECAL"             ),
		new ChunkInfo(false, 0xA083,                  "MAT_ADDITIVE"          ),
		new ChunkInfo(false, 0xA084,                  "MAT_SELF_ILPCT"        ),
		new ChunkInfo(false, 0xA085,                  "MAT_WIRE"              ),
		new ChunkInfo(false, 0xA086,                  "MAT_SUPERSMP"          ),
		new ChunkInfo(false, 0xA087,                  "MAT_WIRESIZE"          ),
		new ChunkInfo(false, 0xA088,                  "MAT_FACEMAP"           ),
		new ChunkInfo(false, 0xA08A,                  "MAT_XPFALLIN"          ),
		new ChunkInfo(false, 0xA08C,                  "MAT_PHONGSOFT"         ),
		new ChunkInfo(false, 0xA08E,                  "MAT_WIREABS"           ),

		new ChunkInfo(false, 0xA100,                  "MAT_SHADING"           ),

		new ChunkInfo(false, 0xA200,                  "MAT_TEXMAP"            ),
		new ChunkInfo(false, 0xA210,                  "MAT_OPACMAP"           ),
		new ChunkInfo(false, 0xA220,                  "MAT_REFLMAP"           ),
		new ChunkInfo(false, 0xA230,                  "MAT_BUMPMAP"           ),
		new ChunkInfo(false, 0xA204,                  "MAT_SPECMAP"           ),
		new ChunkInfo(false, 0xA240,                  "MAT_USE_XPFALL"        ),
		new ChunkInfo(false, 0xA250,                  "MAT_USE_REFBLUR"       ),
		new ChunkInfo(false, 0xA252,                  "MAT_BUMP_PERCENT"      ),

		new ChunkInfo(false, 0xA300,                  "MAT_MAPNAME"           ),
		new ChunkInfo(false, 0xA310,                  "MAT_ACUBIC"            ),

		new ChunkInfo(false, 0xA320,                  "MAT_SXP_TEXT_DATA"     ), 
		new ChunkInfo(false, 0xA321,                  "MAT_SXP_TEXT2_DATA"    ), 
		new ChunkInfo(false, 0xA322,                  "MAT_SXP_OPAC_DATA"     ), 
		new ChunkInfo(false, 0xA324,                  "MAT_SXP_BUMP_DATA"     ), 
		new ChunkInfo(false, 0xA325,                  "MAT_SXP_SPEC_DATA"     ), 
		new ChunkInfo(false, 0xA326,                  "MAT_SXP_SHIN_DATA"     ), 
		new ChunkInfo(false, 0xA328,                  "MAT_SXP_SELFI_DATA"    ), 
		new ChunkInfo(false, 0xA32A,                  "MAT_SXP_TEXT_MASKDATA" ), 
		new ChunkInfo(false, 0xA32C,                  "MAT_SXP_TEXT2_MASKDATA"), 
		new ChunkInfo(false, 0xA32E,                  "MAT_SXP_OPAC_MASKDATA" ), 
		new ChunkInfo(false, 0xA330,                  "MAT_SXP_BUMP_MASKDATA" ), 
		new ChunkInfo(false, 0xA332,                  "MAT_SXP_SPEC_MASKDATA" ), 
		new ChunkInfo(false, 0xA334,                  "MAT_SXP_SHIN_MASKDATA" ), 
		new ChunkInfo(false, 0xA336,                  "MAT_SXP_SELFI_MASKDATA"), 
		new ChunkInfo(false, 0xA338,                  "MAT_SXP_REFL_MASKDATA" ), 
		new ChunkInfo(false, 0xA33A,                  "MAT_TEX2MAP"           ), 
		new ChunkInfo(false, 0xA33C,                  "MAT_SHINMAP"           ), 
		new ChunkInfo(false, 0xA33D,                  "MAT_SELFIMAP"          ), 
		new ChunkInfo(false, 0xA33E,                  "MAT_TEXMASK"           ), 
		new ChunkInfo(false, 0xA340,                  "MAT_TEX2MASK"          ), 
		new ChunkInfo(false, 0xA342,                  "MAT_OPACMASK"          ), 
		new ChunkInfo(false, 0xA344,                  "MAT_BUMPMASK"          ), 
		new ChunkInfo(false, 0xA346,                  "MAT_SHINMASK"          ), 
		new ChunkInfo(false, 0xA348,                  "MAT_SPECMASK"          ), 
		new ChunkInfo(false, 0xA34A,                  "MAT_SELFIMASK"         ), 
		new ChunkInfo(false, 0xA34C,                  "MAT_REFLMASK"          ), 
		new ChunkInfo(false, 0xA350,                  "MAT_MAP_TILINGOLD"     ), 
		new ChunkInfo(false, 0xA351,                  "MAT_MAP_TILING"        ), 
		new ChunkInfo(false, 0xA352,                  "MAT_MAP_TEXBLUR_OLD"   ), 
		new ChunkInfo(false, 0xA353,                  "MAT_MAP_TEXBLUR"       ), 
		new ChunkInfo(false, 0xA354,                  "MAT_MAP_USCALE"        ), 
		new ChunkInfo(false, 0xA356,                  "MAT_MAP_VSCALE"        ), 
		new ChunkInfo(false, 0xA358,                  "MAT_MAP_UOFFSET"       ), 
		new ChunkInfo(false, 0xA35A,                  "MAT_MAP_VOFFSET"       ), 
		new ChunkInfo(false, 0xA35C,                  "MAT_MAP_ANG"           ), 
		new ChunkInfo(false, 0xA360,                  "MAT_MAP_COL1"          ), 
		new ChunkInfo(false, 0xA362,                  "MAT_MAP_COL2"          ), 
		new ChunkInfo(false, 0xA364,                  "MAT_MAP_RCOL"          ), 
		new ChunkInfo(false, 0xA366,                  "MAT_MAP_GCOL"          ), 
		new ChunkInfo(false, 0xA368,                  "MAT_MAP_BCOL"          ), 

		// Keyframe Chunk IDs

		new ChunkInfo(true, (int) ChunkType.CHUNK_KFDATA,            "KFDATA"                ),
		new ChunkInfo(false, 0xB00A,                  "KFHDR"                 ),
		new ChunkInfo(false, 0xB001,                  "AMBIENT_NODE_TAG"      ),
		// new ChunkInfo(true, (int) ChunkType.CHUNK_AMBIENT_NODE_TAG,  "AMBIENT_NODE_TAG"      ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_OBJECT_NODE_TAG,   "OBJECT_NODE_TAG"       ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_CAMERA_NODE_TAG,   "CAMERA_NODE_TAG"       ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_TARGET_NODE_TAG,   "TARGET_NODE_TAG"       ),
		new ChunkInfo(false, 0xB005,                  "LIGHT_NODE_TAG"        ),
		new ChunkInfo(false, 0xB006,                  "L_TARGET_NODE_TAG"     ),
		new ChunkInfo(false, 0xB007,                  "SPOTLIGHT_NODE_TAG"    ),

		new ChunkInfo(true, (int) ChunkType.CHUNK_KFSEG,             "KFSEG"                 ),
		new ChunkInfo(false, 0xB009,                  "KFCURTIME"             ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_NODE_HDR,          "NODE_HDR"              ),
		new ChunkInfo(false, 0xB011,                  "INSTANCE_NAME"         ),
		new ChunkInfo(false, 0xB012,                  "PRESCALE"              ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_PIVOT,             "PIVOT"                 ),
		new ChunkInfo(false, 0xB014,                  "BOUNDBOX"              ),
		new ChunkInfo(false, 0xB015,                  "MORPH_SMOOTH"          ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_POS_TRACK_TAG,     "POS_TRACK_TAG"         ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_ROT_TRACK_TAG,     "ROT_TRACK_TAG"         ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_SCL_TRACK_TAG,     "SCL_TRACK_TAG"         ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_FOV_TRACK_TAG,     "FOV_TRACK_TAG"         ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_ROLL_TRACK_TAG,    "ROLL_TRACK_TAG"        ),
		new ChunkInfo(false, 0xB025,                  "COL_TRACK_TAG"         ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_MORPH_TRACK_TAG,   "MORPH_TRACK_TAG"       ),
		new ChunkInfo(false, 0xB027,                  "HOT_TRACK_TAG"         ),
		new ChunkInfo(false, 0xB028,                  "FALL_TRACK_TAG"        ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_HIDE_TRACK_TAG,    "HIDE_TRACK_TAG"        ),
		new ChunkInfo(true, (int) ChunkType.CHUNK_NODE_ID,           "NODE_ID"               )
	};
	}
}