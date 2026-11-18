using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using UnityEngine;

namespace IsaxRoles;

public class Jester : CrewmateRole, ICustomRole
{
    public string RoleName => "Jester";
    public string RoleDescription => "Get voted out to win";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => Color.magenta;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        OptionsScreenshot = JesterAssets.JesterBanner,
        TasksCountForProgress = false,
        CanUseVent = OptionGroupSingleton<JesterOptions>.Instance.JesterCanVent.Value,
        CanGetKilled = true,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        MaxRoleCount = 1
    };

    public Jester()
    {
        this.SetCanDoTasks(false);
    }

    public string GetCustomEjectionMessage(NetworkedPlayerInfo player)
    {
        return $"The joke's on you! {player.PlayerName} fooled everyone!";
    }

    public override void OnDeath(DeathReason reason)
    {
        if (reason == DeathReason.Exile)
        {
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReasonsEnum.JesterByVote, false);
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return gameOverReason == (GameOverReason)CustomGameOverReasonsEnum.JesterByVote;
    }
}