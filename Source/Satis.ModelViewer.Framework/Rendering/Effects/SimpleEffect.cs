using Nexus;
using SlimDX;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Framework.Rendering.Effects
{
	public class SimpleEffect : EffectWrapperBase
	{
		private Matrix3D _view;

		public Matrix3D World
		{
			get { return GetValue<Matrix>("World").ToMatrix3D(); }
			set { SetValue("World", value.ToSlimDxMatrix()); }
		}

		public Matrix3D View
		{
			get { return _view; }
			set
			{
				_view = value;
				Matrix3D matrix = Matrix3D.Invert(value);
				SetValue("EyePosition", matrix.Translation);
			}
		}

		public Matrix3D Projection
		{
			get;
			set;
		}

		public Vector3D LightDirection
		{
			get { return GetValue<Vector3D>("LightDirection"); }
			set { SetValue("LightDirection", value); }
		}

		public Matrix3D LightViewProjection
		{
			get { return GetValue<Matrix>("LightViewProjection").ToMatrix3D(); }
			set { SetValue("LightViewProjection", value.ToSlimDxMatrix()); }
		}

		public ColorRgbF AmbientLightColor
		{
			get { return GetValue<ColorRgbF>("AmbientLightColor"); }
			set { SetValue("AmbientLightColor", value); }
		}

		public ColorRgbF DiffuseColor
		{
			get { return GetValue<ColorRgbF>("DiffuseColor"); }
			set { SetValue("DiffuseColor", value); }
		}

		public ColorRgbF SpecularColor
		{
			get { return GetValue<ColorRgbF>("SpecularColor"); }
			set { SetValue("SpecularColor", value); }
		}

		public float SpecularPower
		{
			get { return GetValue<float>("SpecularPower"); }
			set { SetValue("SpecularPower", value); }
		}

		public bool ShadowsEnabled
		{
			get { return GetValue<bool>("ShadowsEnabled"); }
			set { SetValue("ShadowsEnabled", value); }
		}

		public BaseTexture ShadowMap
		{
			get { return GetTexture("ShadowMap"); }
			set { SetTexture("ShadowMap", value); }
		}

		public float ShadowMapSize
		{
			get { return GetValue<float>("ShadowMapSize"); }
			set { SetValue("ShadowMapSize", value); }
		}

		public bool SpecularEnabled
		{
			get { return GetValue<bool>("SpecularEnabled"); }
			set { SetValue("SpecularEnabled", value); }
		}

		public SimpleEffect(Device device)
			: base(EffectUtility.FromResource(device, "Rendering/Effects/SimpleEffect.fx"))
		{
			
		}

		protected override void OnBegin()
		{
			SetValue("WorldViewProjection", (World * View * Projection).ToSlimDxMatrix());
		}
	}
}