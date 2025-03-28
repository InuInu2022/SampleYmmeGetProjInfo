/**
IPluginを継承する何もしないプラグイン

実際には manju-summoner/YukkuriMovieMaker4PluginSamples を
参考に適切なプラグインベースを継承して作ってください。

**/
using System.Reflection;
using System.Windows;

using YukkuriMovieMaker.Plugin;

namespace Ymm4PluginSample;

[PluginDetails(AuthorName = "InuInu", ContentId = "")]
public class Sample : IToolPlugin
{
	public Type ViewModelType => typeof(ToolViewModel);
	public Type ViewType => typeof(ToolView);

	public PluginDetailsAttribute Details =>
		GetType().GetCustomAttribute<PluginDetailsAttribute>() ?? new();

	public string Name => "プロジェクト情報の取得";
}