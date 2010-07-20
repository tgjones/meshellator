using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using Gemini.Contracts;
using Gemini.Contracts.Application.Startup;
using Gemini.Contracts.Gui.Layout;
using Gemini.Contracts.Services.ExtensionService;
using Gemini.Host;
using Microsoft.Win32;
using Satis.ModelViewer.Workbench.Documents;

namespace Satis.ModelViewer.Application.StartupCommands
{
	/// <summary>
	/// This just makes sure that when we startup, the Pin Ball Table is always visible
	/// </summary>
	[Export(ContractNames.ExtensionPoints.Host.StartupCommands, typeof(IExecutableCommand))]
	public class StartupCommand : AbstractExtension, IExecutableCommand
	{
		[Import(ContractNames.CompositionPoints.Host.MainWindow, typeof(Window))]
		private Lazy<Window> MainWindow { get; set; }

		[Import(ContractNames.Services.Layout.LayoutManager, typeof(ILayoutManager))]
		private Lazy<ILayoutManager> LayoutManager { get; set; }

		[Import(ContractNames.CompositionPoints.Workbench.ViewModel, typeof(Gemini.Workbench.Workbench))]
		private Lazy<Gemini.Workbench.Workbench> Workbench { get; set; }

		/*[Import(DemoContractNames.CompositionPoints.PinBall.PinBallTable, typeof(DemoDocument.DemoDocument))]
		private Lazy<DemoDocument.DemoDocument> demoDocument { get; set; }

		[Import(ContractNames.Services.PropertyGrid.PropertyGridService, typeof(IPropertyGridService))]
		private Lazy<IPropertyGridService> propertyGridService { get; set; }

		[Import(ContractNames.Services.Output.OutputService, typeof(IOutputService))]
		private Lazy<IOutputService> OutputService { get; set; }*/

		public void Run(params object[] args)
		{
			MainWindow.Value.Title = "Satis Model Viewer";

			Workbench.Value.AddCommandBinding(ApplicationCommands.Open, OnOpenExecuted, OnOpenCanExecute);

			//layoutManager.Value.ShowDocument(demoDocument.Value, string.Empty); // Makes sure it's shown every time

			//propertyGridService.Value.SelectedObject = demoDocument.Value.Camera;

			//OutputService.Value.Append("Started up");

			/*// Sorry, this is a really hacky way of setting the icon on the
			// main window, and only because I can't seem to convert from
			// a PNG to an icon any other way.
			Dummy dummy = new Dummy();
			mainWindow.Value.Icon = dummy.Icon;
			dummy.Close();*/
		}

		private void OnOpenCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void OnOpenExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == true)
			{
				var document = ((GeminiApplication) System.Windows.Application.Current).Container.GetExportedValue<ModelDocument>(SatisContractNames.CompositionPoints.Workbench.Documents.ModelDocument);
				document.Scene = Meshellator.ImportFromFile(dialog.FileName);
				LayoutManager.Value.ShowDocument(document, string.Empty);
			}
		}
	}
}