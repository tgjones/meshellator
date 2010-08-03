using System.IO;
using System.Reflection;
using Gemini.Framework.Services;
using Microsoft.Practices.ServiceLocation;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Framework.Rendering.Effects
{
	public static class EffectUtility
	{
		public static Effect FromResource(Device device, string resourcePath)
		{
			IResourceManager resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
			Stream effectStream = resourceManager.GetStream(resourcePath, Assembly.GetExecutingAssembly().GetAssemblyName());
			return Effect.FromStream(device, effectStream, ShaderFlags.None);
		}
	}
}