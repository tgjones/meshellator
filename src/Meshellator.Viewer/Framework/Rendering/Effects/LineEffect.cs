using Nexus;
using SlimDX;
using SlimDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering.Effects
{
	public class LineEffect : EffectWrapperBase
	{
		public Matrix3D WorldViewProjection
		{
			get { return GetValue<Matrix>("WorldViewProjection").ToMatrix3D(); }
			set { SetValue("WorldViewProjection", value.ToSlimDxMatrix()); }
		}

		public LineEffect(Device device)
			: base(EffectUtility.FromResource(device, "Framework/Rendering/Effects/LineEffect.fx"))
		{
		}
	}
}