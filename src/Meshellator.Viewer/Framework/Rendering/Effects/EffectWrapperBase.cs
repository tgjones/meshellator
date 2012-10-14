using System.Collections.Generic;
using SharpDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering.Effects
{
	public abstract class EffectWrapperBase
	{
		private readonly Effect _effect;
		private readonly Dictionary<string, EffectParameter> _parameters;

		public string CurrentTechnique
		{
			get { return _effect.GetTechniqueDescription(_effect.Technique).Name; }
			set { _effect.Technique = _effect.GetTechnique(value); }
		}

		protected EffectWrapperBase(Effect effect)
		{
			_effect = effect;
			_parameters = new Dictionary<string, EffectParameter>();
		}

		protected T GetValue<T>(string name)
			where T : struct
		{
			return GetEffectParameter(name).GetValue<T>();
		}

		protected T[] GetValue<T>(string name, int count)
			where T : struct
		{
			return GetEffectParameter(name).GetValue<T>(count);
		}

		protected BaseTexture GetTexture(string name)
		{
			return GetEffectParameter(name).GetTexture();
		}

		protected void SetValue(string name, bool value)
		{
			GetEffectParameter(name).SetValue(value);
		}

		protected void SetValue<T>(string name, T value)
			where T : struct
		{
			GetEffectParameter(name).SetValue(value);
		}

		protected void SetValue<T>(string name, T[] value)
			where T : struct
		{
			GetEffectParameter(name).SetValue(value);
		}

		protected void SetTexture(string name, BaseTexture value)
		{
			GetEffectParameter(name).SetTexture(value);
		}

		private EffectParameter GetEffectParameter(string name)
		{
			if (!_parameters.ContainsKey(name))
				_parameters.Add(name, new EffectParameter(_effect, name));
			return _parameters[name];
		}

		public int Begin()
		{
			OnBegin();
			return _effect.Begin();
		}

		protected virtual void OnBegin()
		{
			
		}

		public void BeginPass(int pass)
		{
			_effect.BeginPass(pass);
		}

		public void EndPass()
		{
			_effect.EndPass();
		}

		public void End()
		{
			_effect.End();
		}
	}
}