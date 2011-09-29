using System.IO;
using System.Reflection;
using Caliburn.Micro;
using Gemini.Framework.Services;
using SlimDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering.Effects
{
	public static class EffectUtility
	{
		public static Effect FromResource(Device device, string resourcePath)
		{
			IResourceManager resourceManager = IoC.Get<IResourceManager>();
			Stream effectStream = resourceManager.GetStream(resourcePath, Assembly.GetExecutingAssembly().GetAssemblyName());
			return Effect.FromStream(device, effectStream, ShaderFlags.None);
		}
	}
}