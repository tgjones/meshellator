using SlimDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering.Effects
{
	public class EffectParameter
	{
		private readonly Effect _effect;
		private readonly EffectHandle _handle;

		internal EffectParameter(Effect effect, string name)
		{
			_effect = effect;
			_handle = effect.GetParameter(null, name);
		}

		public T GetValue<T>()
			where T : struct
		{
			return _effect.GetValue<T>(_handle);
		}

		public T[] GetValue<T>(int count)
			where T : struct
		{
			return _effect.GetValue<T>(_handle, count);
		}

		public BaseTexture GetTexture()
		{
			return _effect.GetTexture(_handle);
		}

		public void SetValue<T>(T value)
			where T : struct
		{
			_effect.SetValue(_handle, value);
		}

		public void SetValue<T>(T[] values)
			where T : struct
		{
			_effect.SetValue(_handle, values);
		}

		public void SetTexture(BaseTexture texture)
		{
			_effect.SetTexture(_handle, texture);
		}
	}
}