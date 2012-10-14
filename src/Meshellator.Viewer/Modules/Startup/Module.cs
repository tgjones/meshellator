using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Framework.Results;
using Meshellator.Viewer.Framework.Rendering;
using Meshellator.Viewer.Framework.Results;
using Meshellator.Viewer.Framework.Scenes;
using Meshellator.Viewer.Framework.Services;
using Meshellator.Viewer.Modules.ModelEditor.ViewModels;
using Nexus.Graphics.Colors;

namespace Meshellator.Viewer.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		public override void Initialize()
		{
			Shell.Title = "Meshellator Viewer";

			Scene torusScene = MeshellatorLoader.CreateFromTorus(10, 1, 20);
			Scene teapotScene = MeshellatorLoader.CreateFromTeapot(15, 20);
			teapotScene.Materials[0].DiffuseColor = ColorsRgbF.Green;
			Scene planeScene = MeshellatorLoader.CreateFromPlane(40, 40);
			planeScene.Materials[0].DiffuseColor = ColorsRgbF.Gray;

			torusScene.Meshes.Add(teapotScene.Meshes[0]);
			torusScene.Meshes.Add(planeScene.Meshes[0]);

			Shell.OpenDocument(new ModelEditorViewModel(new SceneViewModel(torusScene), "[New Shape]"));

			var fileMenuItem = Shell.MainMenu.First(mi => mi.Name == "File");
			fileMenuItem.Children.Insert(0, new MenuItem("New Sphere", NewSphere));
			fileMenuItem.Children.Insert(1, new MenuItem("New Teapot", NewTeapot));
			fileMenuItem.Children.Insert(2, new MenuItemSeparator());

			var rendererMenu = new MenuItem("Renderer");
			Shell.MainMenu.Add(rendererMenu);
			rendererMenu.Add(
				new CheckableMenuItem("Wireframe", ToggleFillModeWireframe),
				MenuItemBase.Separator,
				new CheckableMenuItem("Show Normals", ToggleNormals, ChangeRenderStateCanExecute),
				new CheckableMenuItem("Show Shadows", ToggleShadows, ChangeRenderStateCanExecute) { IsChecked = true },
				MenuItemBase.Separator,
				new CheckableMenuItem("Anti-Aliasing", ToggleAntiAliasing, ChangeRenderStateCanExecute) { IsChecked = true },
				new CheckableMenuItem("No Specular", ToggleSpecular, ChangeRenderStateCanExecute));
		}

		private static IEnumerable<IResult> NewSphere()
		{
			yield return Show.Document(new ModelEditorViewModel(new SceneViewModel(MeshellatorLoader.CreateFromSphere(10, 10)), "[New Sphere]"));
		}

		private static IEnumerable<IResult> NewTeapot()
		{
			yield return Show.Document(new ModelEditorViewModel(new SceneViewModel(MeshellatorLoader.CreateFromTeapot(10, 10)), "[New Teapot]"));
		}

		private static IEnumerable<IResult> ToggleFillModeWireframe(bool isChecked)
		{
			yield return new RenderSettingsResult(e => e.RenderParameters.FillMode = (isChecked) ? FillMode.Wireframe : FillMode.Solid);
		}

		private static IEnumerable<IResult> ToggleNormals(bool isChecked)
		{
			yield return new RenderSettingsResult(e => e.RenderParameters.ShowNormals = isChecked);
		}

		private static IEnumerable<IResult> ToggleShadows(bool isChecked)
		{
			yield return new RenderSettingsResult(e => e.RenderParameters.ShowShadows = isChecked);
		}

		private static IEnumerable<IResult> ToggleAntiAliasing(bool isChecked)
		{
			yield return new RenderSettingsResult(e => e.RenderParameters.AntiAliasingEnabled = isChecked);
		}

		private static IEnumerable<IResult> ToggleSpecular(bool isChecked)
		{
			yield return new RenderSettingsResult(e => e.RenderParameters.NoSpecular = isChecked);
		}

		private bool ChangeRenderStateCanExecute()
		{
			return (Shell.ActiveItem is IModelEditor);
		}
	}
}