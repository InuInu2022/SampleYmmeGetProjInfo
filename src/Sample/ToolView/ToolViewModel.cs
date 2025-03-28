using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

using Epoxy;
using Reactive.Bindings;

namespace Ymm4PluginSample;

[ViewModel]
public class ToolViewModel
{
	// Loadedイベントを受信するためのCommandプロパティの定義
	// ※サンプルではEpoxy使ってますが、他にも色んな方法があります
	public Command? Ready { get; private set; }

	public ReadOnlyReactivePropertySlim<string>? ProjectName { get; set; }
	public ReadOnlyReactivePropertySlim<int>? CurrentFrame { get; private set; }
	public ReadOnlyReactivePropertySlim<int>? TotalFrame { get; private set; }
	public ReadOnlyReactivePropertySlim<System.TimeSpan>? CurrentTime { get; private set; }
	public ReadOnlyReactivePropertySlim<System.TimeSpan>? TotalTime { get; private set; }
	public ReadOnlyReactivePropertySlim<string>? VideoInfo { get; private set; }

	public ToolViewModel()
	{
		// Loadedイベントが発生した場合の処理を記述
		this.Ready = Command.Factory.Create<EventArgs>(_ =>
		{
			var toolWin = Application.Current.MainWindow;
			toolWin.Title = "YMM4のプロジェクト情報取得ツール";
			GetMainWindow();
			return default;
		});
	}

	void GetMainWindow()
	{
		// メインウィンドウを取得
		Window mainWindow = GetYmmMainWindow();

		if (mainWindow != null)
		{
			// メインウィンドウのDataContext（ViewModel）を取得
			dynamic viewModel = mainWindow.DataContext;

			/// メインウィンドウのViewModelから色々情報が取れます

			Debug.WriteLine($"viewModel.Title: {GetProp(viewModel, nameof(viewModel.Title))}");
			Debug.WriteLine($"""
				viewModel.WindowState:
					x: {GetProp(viewModel, nameof(viewModel.WindowState.X))}
					y: {GetProp(viewModel, nameof(viewModel.WindowState.Y))}
					width: {GetProp(viewModel, nameof(viewModel.WindowState.Width))}
					height: {GetProp(viewModel, nameof(viewModel.WindowState.Height))}
					IsMaximized: {GetProp(viewModel, nameof(viewModel.WindowState.IsMaximized))}
				""");
			Debug.WriteLine($"viewModel.IsEmptyProject: {GetProp(viewModel, nameof(viewModel.IsEmptyProject))}");
			Debug.WriteLine($"viewModel.IsSaved: {GetProp(viewModel, nameof(viewModel.IsSaved))}");
			Debug.WriteLine($"viewModel.KeepProjectPath: {GetProp(viewModel, nameof(viewModel.KeepProjectPath))}");
			Debug.WriteLine($"viewModel.ProjectFilePath: {GetProp(viewModel, nameof(viewModel.ProjectFilePath))}");

			/// StatusBarからプロジェクト情報を取得
			/// ReactiveProperty化されているのでBindするだけで自動で反映されます
			/// StatusBarは非表示でも情報取れます

			var sb = GetProp(viewModel, nameof(viewModel.StatusBarViewModel));
			var val = GetProp(sb, nameof(viewModel.StatusBarViewModel.Value));

			Debug.WriteLine($"viewModel.StatusBarViewModel.Value.ProjectName :{GetProp(val, nameof(val.ProjectName))}");

			/// 取得したプロパティをBindする
			/// ProjectName 以外はプロジェクトを読み込み直すとBindが途切れるようです
			/// TODO: ProjectNameが変更されたらReBindするような処理が必要

			ProjectName = GetProp(val, nameof(val.ProjectName));
			CurrentFrame = GetProp(val, nameof(val.CurrentFrame));
			TotalFrame = GetProp(val, nameof(val.TotalFrame));
			CurrentTime = GetProp(val, nameof(val.CurrentTime));
			TotalTime = GetProp(val, nameof(val.TotalTime));
			VideoInfo = GetProp(val, nameof(val.VideoInfo));

			/// 情報として無理やりタイムライン上の情報も取れます

			var tla = GetProp(viewModel, nameof(viewModel.TimelineAreaViewModel));
			var talv = GetProp(tla, nameof(viewModel.TimelineAreaViewModel.ViewModel));
			//タイミングによって取得できないのでnullチェックが必要
			if (talv is null) return;
			var talvv = GetProp(talv, nameof(viewModel.TimelineAreaViewModel.Value));
			if (talvv is null) return;
			var timeline = GetField(talvv, nameof(viewModel.TimelineAreaViewModel.Value.timeline), true);

		}
		else
		{
			Console.WriteLine("No Main Window found.");
		}
	}

	static Window GetYmmMainWindow()
	{
		List<dynamic> windows = [.. Application.Current.Windows];
		return windows
			.OfType<Window>()
			.First(w => string.Equals(w.GetType().FullName, "YukkuriMovieMaker.Views.MainView", StringComparison.Ordinal));
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "<保留中>")]
	static dynamic? GetProp(dynamic vm, string propName, bool isPrivate = false)
	{
		Type vmType = vm.GetType();
		var propertyInfo = vmType.GetProperty(
			propName,
			bindingAttr: isPrivate
				? BindingFlags.NonPublic | BindingFlags.Instance
				: BindingFlags.Public | BindingFlags.Instance
			);
		if (propertyInfo is null) return default;
		dynamic val = propertyInfo.GetValue(vm);
		return val;
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Major Code Smell",
		"S3011:Reflection should not be used to increase accessibility of classes, methods, or fields",
		Justification = "<保留中>"
	)]
	static dynamic? GetField(dynamic vm, string fieldName, bool isPrivate = false)
	{
		Type vmType = vm.GetType();
		var fieldInfo = vmType.GetField(
			fieldName,
			bindingAttr: isPrivate
				? BindingFlags.NonPublic | BindingFlags.Instance
				: BindingFlags.Public | BindingFlags.Instance
		);
		if (fieldInfo is null)
			return default;
		dynamic val = fieldInfo.GetValue(vm);
		return val;
	}
}