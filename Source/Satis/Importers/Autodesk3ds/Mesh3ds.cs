using System.Collections.Generic;

namespace Satis.Importers.Autodesk3ds
{
	/**
 * Mesh object.
 * <br>
 * <br>
 * A mesh object in 3D Studio consists of a set of faces (triangles).
 * The faces are assigned materials and can be texture mapped.
 * The position, rotation, scaling, etc. are controlled by the
 * keyframer tracks.
 * <br>
 * <br>
 * The Mesh3ds class contains the following data that is read 
 * from the 3D-editor part of the 3ds-file:<br>
 * - Mesh name<br>
 * - An array of vertices<br>
 * - An array of texture mapping coordinates<br>
 * - Texture mapping U and V tiling parameter<br>
 * - Texture mapping type<br>
 * - An array of faces<br>
 * - An array of smmothing group parameters<br>
 * - Matrix for moving the mesh frown world to object space<br>
 * - A set of face material assignment objects<br>
 * <br>
 * The following data is from the keyframer:<br>
 * - Node id in the object hierarchy<br>
 * - Parent node id in the object hierarchy<br>
 * - Node flags<br>
 * - Position spline track<br>
 * - Rotation spline track<br>
 * - Scale spline track<br>
 * - Morph spline track<br>
 * - Hide track<br>
 * <br>
 * <br>
 */
public class Mesh3ds
{
	/// <summary>
	/// Gets or sets the name of this mesh.
	/// </summary>
	public string Name { get; internal set; }

	// Vertices
	internal Vertex3ds[] mVertex = new Vertex3ds[0];

	// Texture mapping coordinates
	internal TexCoord3ds[] mTexCoord = new TexCoord3ds[0];

	// Texture mapping U tiling
	internal float mTexUTile = 1.0f;

	// Texture mapping V tiling
	internal float mTexVTile = 1.0f;

	/// <summary>
	/// Get or sets texture mapping type used while mapping this mesh.
	/// </summary>
	public TextureMappingMode3ds TextureMappingMode { get; internal set; }

	// Face definitions
	internal Face3ds[] mFace = new Face3ds[0];

	// Smoothing groups
	internal int[] mSmoothGroup = new int[0];

	// Matrix for local coordinate system
	internal float[,] mLocalSystem = new float[3, 4];

	// Face materials
	internal List<FaceMat3ds> mFaceMat = new List<FaceMat3ds>(5);


	// From the keyframer

	// Node id
	internal int mNodeId = 0;

	// Parent node id
	internal int mParentNodeId = 0;

	// Node flags
	internal int mNodeFlags = 0;	

	// Pivot point
	internal Vertex3ds mPivot = new Vertex3ds(0.0f, 0.0f, 0.0f);

	// Position spline track
	internal XYZTrack3ds mPositionTrack = new XYZTrack3ds();

	// Rotation spline track
	internal RotationTrack3ds mRotationTrack = new RotationTrack3ds();

	// Scale spline track
	internal XYZTrack3ds mScaleTrack = new XYZTrack3ds();

	// Morph spline track
	internal MorphTrack3ds mMorphTrack = new MorphTrack3ds();

	// Hide spline track
	internal HideTrack3ds mHideTrack = new HideTrack3ds();

	public Mesh3ds()
	{
		Name = string.Empty;
	}

	internal void addFaceMat(FaceMat3ds fm)
	{
		mFaceMat.Add(fm);
	}

	/**
	 * Get number of vertices.
	 *
	 * @return number of vertices
	 */
	public int vertices() 
	{
		return mVertex.Length;
	}

	/**
	 * Acces a specific vertex in the vertex array.
	 *
	 * @param i index into vertex array [0 ... vertices()-1]
	 * @return the specified vertex
	 */
	public Vertex3ds vertex(int i)
	{
		return mVertex[i];
	}

	/** 
	 * Access the whole array of vertices.
	 *
	 * @return array of vertices
	 */
	public Vertex3ds[] vertexArray()
	{
		return mVertex;
	}

	/** 
	 * Get number of texture mapping coordinates.
	 *
	 * The number of texture mapping coordinates is always the same 
	 * as the number of vertices if the mesh has been texture mapped
	 * in 3D Studio, else it is 0.
	 *
	 * @return number of texture mapping coordinates.
	 */
	public int texCoords()
	{
		return mTexCoord.Length;
	}

	/** 
	 * Access a specific texture mapping coordinate.
	 *
	 * @param i index into texture mapping array [0 ... texCoords()-1]
	 * @return the specified texture mapping coordinate
	 */
	public TexCoord3ds texCoord(int i)
	{
		return mTexCoord[i];
	}

	/** 
	 * Access the whole array of texture mapping coordinates.
	 *
	 * @return array of texture mapping coordinates
	 */
	public TexCoord3ds[] texCoordArray()
	{
		return mTexCoord;
	}

	/** 
	 * Get texture mapping U tiling parameter.
	 *
	 * @return texture mapping U tiling
	 */
	public float texUTile()
	{
		return mTexUTile;
	}

	/** 
	 * Get texture mapping V tiling parameter.
	 *
	 * @return texture mapping V tiling
	 */
	public float texVTile()
	{
		return mTexVTile;
	}

	/** 
	 * Get number of faces.
	 *
	 * @return number of faces
	 */
	public int faces()
	{
		return mFace.Length;
	}

	/**
	 * Access a specific face.
	 *
	 * @param i index into face array [0 ... faces()-1]
	 * @return the specified face
	 */
	public Face3ds face(int i)
	{
		return mFace[i];
	}

	/** 
	 * Access the whole array of faces.
	 *
	 * @return array of faces
	 */
	public Face3ds[] faceArray()
	{
		return mFace;
	}

	/** 
	 * Get number of face materials.
	 *
	 * @return number of face materials
	 */
	public int faceMats()
	{
		return mFaceMat.Count;
	}

	/**
	 * Access a specific face material.
	 *
	 * @param i index into face material array [0 ... faceMats()-1]
	 * @return the specified face material
	 */
	public FaceMat3ds faceMat(int i)
	{
		return mFaceMat[i];
	}

	/** 
	 * Get number of smoothing group entrys.
	 *
	 * @return number of smoothing group entrys
	 */
	public int smoothEntrys()
	{
		return mSmoothGroup.Length;
	}

	/**
	 * Access a specific smoothing group entry.
	 *
	 * @param i index into smoothing group entry array [0 ... smoothEntrys()-1]
	 * @return the specified smoothing group entry
	 */
	public int smoothEntry(int i)
	{
		return mSmoothGroup[i];
	}

	/** 
	 * Access the whole array of smoothing group entrys.
	 *
	 * @return array of smoothing group entry
	 */
	public int[] smoothEntryArray()
	{
		return mSmoothGroup;
	}

	/**
	 * Get node id.
	 *
	 * @return node id
	 */
	public int nodeId()
	{	
		return mNodeId;
	}

	/**
	 * Get parent node id.
	 *
	 * @return parent node id
	 */
	public int parentNodeId()
	{	
		return mParentNodeId;
	}

	/**
	 * Get node flags.
	 *
	 * @return node flags
	 */
	public int nodeFlags()
	{	
		return mNodeFlags;
	}

	/**
	 * Get pivot point.
	 *
	 * @return pivot point
	 */
	public Vertex3ds pivot()
	{
		return mPivot;
	}

	/**
	 * Access position spline track.
	 *
	 * @return position spline track
	 */
	public XYZTrack3ds position()
	{
		return mPositionTrack;
	}

	/**
	 * Access rotation spline track.
	 *
	 * @return rotation spline track
	 */
	public RotationTrack3ds rotation()
	{
		return mRotationTrack;
	}

	/**
	 * Access scale spline track.
	 *
	 * @return scale spline track
	 */
	public XYZTrack3ds scale()
	{
		return mScaleTrack;
	}

	/**
	 * Access morph spline track.
	 *
	 * @return morph spline track
	 */
	public MorphTrack3ds morph()
	{
		return mMorphTrack;
	}

	/**
	 * Access hide track.
	 *
	 * @return hide track
	 */
	public HideTrack3ds hide()
	{
		return mHideTrack;
	}

}
}