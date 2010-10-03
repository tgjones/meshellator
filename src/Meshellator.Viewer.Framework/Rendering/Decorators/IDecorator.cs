using System.Collections.Generic;

namespace Meshellator.Viewer.Framework.Rendering.Decorators
{
	internal interface IDecorator
	{
		bool IsActive(RenderSettings settings);

		void OnBeginDrawModel(Model model, RenderSettings renderSettings, IEnumerable<IDecorator> decorators);
		void OnEndDrawModel(Model model, RenderSettings renderSettings);

		void OnBeginDrawMesh(ModelMesh mesh, RenderSettings renderSettings);
		void OnEndDrawMesh(ModelMesh mesh, RenderSettings renderSettings);
	}
}