using Nexus;
using SharpDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering.Effects
{
	public class GaussianBlurEffect : EffectWrapperBase
	{
		public Vector2D Scale
		{
			get { return GetValue<Vector2D>("Scale"); }
			set { SetValue("Scale", value); }
		}

		public BaseTexture Texture
		{
			get { return GetTexture("Texture"); }
			set { SetTexture("Texture", value); }
		}

		public GaussianBlurEffect(Device device)
			: base(EffectUtility.FromResource(device, "Framework/Rendering/Effects/GaussianBlurEffect.fx"))
		{
		}
	}
}