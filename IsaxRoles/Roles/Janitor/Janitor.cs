using MiraAPI.GameOptions;
using MiraAPI.Roles;
using UnityEngine;

namespace IsaxRoles;

public class Janitor : ImpostorRole, ICustomRole
{
    public string RoleName => "Janitor";
    public string RoleDescription => "Clean up bodies";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => Color.black;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        CanUseVent = OptionGroupSingleton<JanitorOptions>.Instance.JanitorCanVent.Value,
        OptionsScreenshot = JanitorAssets.JanitorBanner,
    };

    public string GetCustomEjectionMessage(NetworkedPlayerInfo player)
    {
        if (ExileController.Instance.initData.confirmImpostor) return $"Looks like {player.PlayerName} got cleaned up!";
        return DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.ExileTextNonConfirm, (Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Il2CppSystem.Object>)System.Array.Empty<object>());
    }
}