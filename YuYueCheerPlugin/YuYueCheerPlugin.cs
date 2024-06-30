using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;

namespace YuYueCheerPlugin;

[MinimumApiVersion(80)]
public class YuYueCheerPlugin : BasePlugin, IPluginConfig<YuYueCheerPluginConfig>
{
    public override string ModuleName => "YuYueCheerPlugin";
    public override string ModuleVersion => "1.0.6";
    public override string ModuleAuthor => "YuYueCraft | GianniKoch(Special Thanks)";
    public override string ModuleDescription => "The meme plugin that emits laughter";
    public required YuYueCheerPluginConfig Config { get; set; }

    private Dictionary<CCSPlayerController, DateTimeOffset> LastCheer { get; } = new();

    public override void Load(bool hotReload)
    {
        AddCommand("cheer", "Cheer!", (commandPlayer, info) =>
        {
            if (Config.CheerSounds.Count == 0)
            {
                info.ReplyToCommand("{ChatColors.Grey}[系统] 笑声文件未设置!");
                return;
            }

            if (commandPlayer != null)
            {
                if (LastCheer.TryGetValue(commandPlayer, out var lastCheer) &&
                    DateTimeOffset.Now - lastCheer < TimeSpan.FromSeconds(Config.CooldownSeconds))
                {
                    info.ReplyToCommand($" {ChatColors.Grey}[系统] 冷却时间{Config.CooldownSeconds}秒!");
                    commandPlayer.PrintToCenter($" {ChatColors.Grey}冷却中,请稍后重试!");
                    return;
                }

                LastCheer[commandPlayer] = DateTimeOffset.UtcNow;
            }

            var song = Config.CheerSounds[Random.Shared.NextDistinct(Config.CheerSounds.Count)];
            foreach (var player in Utilities.GetPlayers())
            {
                player.ExecuteClientCommand($"play \"{song}\"");
            }

            var teamColor = commandPlayer?.Team switch
            {
                CsTeam.CounterTerrorist => ChatColors.Blue,
                CsTeam.Terrorist => ChatColors.Red,
                _ => ChatColors.White
            };
            Server.PrintToChatAll($" {ChatColors.Grey}[系统] {teamColor}{commandPlayer?.PlayerName ?? "Console"}{ChatColors.Grey} 笑嘻了!!!");
        });
    }

    public void OnConfigParsed(YuYueCheerPluginConfig config)
    {
        if (config.CheerSounds.Count == 0)
        {
            Server.PrintToConsole("笑声文件未设置!");
        }
        else
        {
            Server.PrintToConsole($"载入 {config.CheerSounds.Count} 个笑声文件!");
        }

        Config = config;
    }
}

public class YuYueCheerPluginConfig : BasePluginConfig
{
    public List<string> CheerSounds { get; set; } = [];
    public int CooldownSeconds { get; set; } = 3;
}