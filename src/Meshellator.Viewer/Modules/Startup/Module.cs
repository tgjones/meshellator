using System.Collections.Generic;
using System.Linq;
using Caliburn.PresentationFramework;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Framework.Ribbon;
using Gemini.Framework.Services;
using Meshellator.Viewer.Framework.Rendering;
using Meshellator.Viewer.Framework.Results;
using Meshellator.Viewer.Framework.Scenes;
using Meshellator.Viewer.Framework.Services;
using Meshellator.Viewer.Modules.ModelEditor.ViewModels;
using Nexus;

namespace Meshellator.Viewer.Modules.Startup
{
	public class Module : ModuleBase
	{
		protected override void Initialize()
		{
			IShell shell = Container.GetInstance<IShell>();

			shell.Title = "Meshellator Viewer";

			Scene torusScene = Meshellator.CreateFromTorus(10, 1, 20);
			Scene teapotScene = Meshellator.CreateFromTeapot(15, 20);
			teapotScene.Materials[0].DiffuseColor = ColorsRgbF.Green;
			Scene planeScene = Meshellator.CreateFromPlane(40, 40);
			planeScene.Materials[0].DiffuseColor = ColorsRgbF.Gray;

			torusScene.Meshes.Add(teapotScene.Meshes[0]);
			torusScene.Meshes.Add(planeScene.Meshes[0]);

			shell.OpenDocument(new ModelEditorViewModel(new SceneViewModel(torusScene), "[New Shape]"));

			shell.Ribbon.AddBackstageItems(
				new RibbonButton("New Sphere", NewSphere),
				new RibbonButton("New Teapot", NewTeapot));

			shell.Ribbon.Tabs
				.First(t => t.Name == "Home")
				.Add(new RibbonGroup("Fill Mode", new List<IRibbonItem>
				{
					new RibbonToggleButton("Solid", SetFillModeSolid, ChangeRenderStateCanExecute, "FillMode") { IsChecked = true },
					new RibbonToggleButton("Wireframe", SetFillModeWireframe, ChangeRenderStateCanExecute, "FillMode"),
					new RibbonToggleButton("Point", SetFillModePoint, ChangeRenderStateCanExecute, "FillMode")
				}));

			shell.Ribbon.Tabs
				.First(t => t.Name == "Home")
				.Add(new RibbonGroup("Show", new List<IRibbonItem>
				{
					new RibbonCheckBox("Normals", ToggleNormals, ChangeRenderStateCanExecute),
					new RibbonCheckBox("Shadows", ToggleShadows, ChangeRenderStateCanExecute) { IsChecked = true }
				}));

			shell.Ribbon.Tabs
				.First(t => t.Name == "Home")
				.Add(new RibbonGroup("Render Options", new List<IRibbonItem>
				{
					new RibbonCheckBox("Anti-Aliasing", ToggleAntiAliasing, ChangeRenderStateCanExecute) { IsChecked = true },
					new RibbonCheckBox("No Specular", ToggleSpecular, ChangeRenderStateCanExecute)
				}));
		}

		private static IEnumerable<IResult> NewSphere()
		{
			yield return Show.Document(new ModelEditorViewModel(new SceneViewModel(Meshellator.CreateFromSphere(10, 10)), "[New Sphere]"));
		}

		private static IEnumerable<IResult> NewTeapot()
		{
			yield return Show.Document(new ModelEditorViewModel(new SceneViewModel(Meshellator.CreateFromTeapot(10, 10)), "[New Teapot]"));
		}

		private static IEnumerable<IResult> SetFillModeSolid(bool isChecked)
		{
			return SetFillMode(isChecked, FillMode.Solid);
		}

		private static IEnumerable<IResult> SetFillModeWireframe(bool isChecked)
		{
			return SetFillMode(isChecked, FillMode.Wireframe);
		}

		private static IEnumerable<IResult> SetFillModePoint(bool isChecked)
		{
			return SetFillMode(isChecked, FillMode.Point);
		}

		private static IEnumerable<IResult> SetFillMode(bool isChecked, FillMode fillMode)
		{
			if (!isChecked)
				yield break;
			yield return new RenderSettingsResult(e => e.RenderParameters.FillMode = fillMode);
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
			return (Container.GetInstance<IShell>().CurrentPresenter is IModelEditor);
		}
	}
}