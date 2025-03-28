/**
IPluginを継承する何もしないプラグイン

実際には manju-summoner/YukkuriMovieMaker4PluginSamples を
参考に適切なプラグインベースを継承して作ってください。

**/
using System.Reflection;
using System.Windows;

using YukkuriMovieMaker.Plugin;

namespace Ymm4PluginSample;

//以下のサンプルを参考に作ってください
//[manju-summoner/YukkuriMovieMaker4PluginSamples: YMM4用プラグインのサンプル集です](https://github.com/manju-summoner/YukkuriMovieMaker4PluginSamples)

[PluginDetails(AuthorName = "", ContentId = "")]
public class Sample : IToolPlugin
{
	public Sample()
	{
		Console.WriteLine("Sample");
	}

	public Type ViewModelType => typeof(ToolViewModel);
	public Type ViewType => typeof(ToolView);

	public string Name => "プロジェクト情報の取得";
}