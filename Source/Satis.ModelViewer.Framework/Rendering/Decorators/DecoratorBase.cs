using System.Collections.Generic;

namespace Satis.ModelViewer.Framework.Rendering.Decorators
{
	internal abstract class DecoratorBase : IDecorator
	{
		public abstract bool IsActive(RenderSettings settings);

		public virtual void OnBeginDrawModel(Model model, RenderSettings renderSettings, IEnumerable<IDecorator> activeDecorators)
		{
			
		}

		public virtual void OnEndDrawModel(Model model, RenderSettings renderSettings)
		{
			
		}

		public virtual void OnBeginDrawMesh(ModelMesh mesh, RenderSettings renderSettings)
		{

		}

		public virtual void OnEndDrawMesh(ModelMesh mesh, RenderSettings renderSettings)
		{

		}
	}
}