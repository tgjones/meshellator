namespace Satis.Importers.Autodesk3ds
{
	/**
 * Base class for Kochanek-Bartels spline track keys containing Frame, TBC and 
 * Ease parameters.
 * These parameters are used in all 3D Studio keyframer tracks 
 * (except the {@link mri.v3ds.HideTrack3ds HideTrack3ds}).
 * <br>
 * <br>
 * When Tension = Bias = Continuity = 0.0 then this is Catmul-Rom<br>
 * When Tension = 1.0 and Bias = Continuity = 0.0 then this is Simple Cubic<br>
 * When Tension = Bias = 0.0 and Continuity = -1.0 then this is Linear interpolation<br>
 */
	public class SplineKey3ds
	{
		/**
		 * Frame number.
		 */
		public int Frame = 0;

		/**
		 * Spline Tension parameter 1.0 (tight) to -1.0 (round).
		 */
		public float Tension = 0.0f;

		/**
		 * Spline Bias parameter 1.0 (post shoot) to -1.0 (pre shoot).
		 */
		public float Bias = 0.0f;

		/**
		 * Spline Continuity parameter 1.0 (inverted corners) to -1.0 (box corners).
		 */
		public float Continuity = 0.0f;

		/**
		 * Spline ease-to parameter 0.0 to 1.0 (speed, coming to key).
		 */
		public float EaseTo = 0.0f;

		/**
		 * Spline ease-from parameter 0.0 to 1.0 (speed, leaving key).
		 */
		public float EaseFrom = 0.0f;
	}
}