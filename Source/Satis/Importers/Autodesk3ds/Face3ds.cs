namespace Satis.Importers.Autodesk3ds
{
	/**
 * Face definition for a single triangle.
 * <br>
 * <br>
 * The face definition contains three parameters (<code>P0, P1, P2</code>) which 
 * identify the three corners of a triangle. <code>P0, P1</code> and <code>P2</code> 
 * are simply indexes into the vertex and texture mapping arrays (accessible 
 * from the {@link mri.v3ds.Mesh3ds Mesh3ds} class). A face also contains the
 * <code>Flag</code> parameter which is a bitfield holding edge visibility 
 * and texture wrapping flags.
 */ 
public class Face3ds
{
	/**
	 * Bitmask for Flags parameter. If the flag is set, it means that
	 * the edge between P0 and P1 is visible.
	 */
	public const int AB_VISIBLE = 0x04;

	/**
	 * Bitmask for Flags parameter. If the flag is set, it means that
	 * the edge between P1 and P2 is visible.
	 */
	public const int BC_VISIBLE = 0x02;

	/**
	 * Bitmask for Flags parameter. If the flag is set, it means that
	 * the edge between P2 and P0 is visible.
	 */
	public const int CA_VISIBLE = 0x01;

	/**
	 * Bitmask for Flags parameter. If the flag is set, it means that
	 * the texture may have a wrap in the U direction. Actual wrapping 
	 * has to be derermined using a heuristic method.
	 */
	public const int U_WRAP = 0x08;

	/**
	 * Bitmask for Flags parameter. If the flag is set, it means that
	 * the texture may have a wrap in the V direction. Actual wrapping 
	 * has to be derermined using a heuristic method.
	 */
	public const int V_WRAP = 0x10;

	/**
	 * P0 index into vertex and texture mapping arrays.
	 */
	public int P0;

	/**
	 * P1 index into vertex and texture mapping arrays.
	 */
	public int P1;

	/**
	 * P2 index into vertex and texture mapping arrays.
	 */
	public int P2;

	/**
	 * Edge visibility and texture wrapping flags. Use the
	 * constants: AB_VISIBLE, BC_VISIBLE, CA_VISIBLE, U_WRAP and
	 * V_WRAP as bitmasks to access the flags.
	 */
	public int Flags;

	/**
	 * Constructor, initialising the (P0,P1,P2,Flags) parameters.
	 */
	public Face3ds(int p0, int p1, int p2, int flags)
	{
		P0 = p0;
		P1 = p1;
		P2 = p2;
		Flags = flags;
	}

	/**
	 * Returns a String object representing this Face3ds's value.
	 * 
	 * @return a string representation of this object.
	 */
	public override string ToString()
	{
		return "P0 P1 P2:" +
		       Utils3ds.intToString(P0, 6) + 
		       Utils3ds.intToString(P1, 6) +
		       Utils3ds.intToString(P2, 6) +
		       "  Flags: " + ((Flags & AB_VISIBLE) != 0 ? "AB" : "--") +
		       " "  +        ((Flags & BC_VISIBLE) != 0 ? "BC" : "--") +
		       " "  +        ((Flags & CA_VISIBLE) != 0 ? "CA" : "--") + 
			 "  "  +       ((Flags & U_WRAP) != 0 ? "U"  : "-") + 
			 " "  +        ((Flags & V_WRAP) != 0 ? "V"  : "-");
	}
}
}