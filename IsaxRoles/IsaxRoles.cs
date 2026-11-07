using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI;
using MiraAPI.PluginLoading;
using MiraAPI.Translation;
using Reactor;
using Reactor.Utilities;
using Reactor.Networking;
using Reactor.Networking.Attributes;

namespace IsaxRoles;

[BepInAutoPlugin("com.isax.roles", "IsaxRoles")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class IsaxRoles : BasePlugin, IMiraPlugin
{
    public const string DisplayVersion = "2026.11.7";
    public const string Author = "Isax820";
    public IsaxRoles()
    {
        MiraLocaleManager.Register("com.isax.roles");
    }

    public Harmony Harmony { get; } = new(Id);
    public string OptionsTitleText { get; } = "Mira API\nIsax Roles";
    public string CustomOptionMenuNameTwo { get; } = "IsaxRoles Options";

    public ConfigFile GetConfigFile()
    {
        return Config;
    }

    public override void Load()
    {
        ReactorCredits.Register(
            "IsaxRoles",
            DisplayVersion,
            false,
            ReactorCredits.AlwaysShow
        );

        Harmony.PatchAll();

        Log.LogMessage("IsaxRoles initialized.");
    }
}