using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using UnityEngine;

namespace IsaxRoles;

public class Arsonist : CrewmateRole, ICustomRole
{
    public string RoleName => "Arsonist";
    public string RoleDescription => "Douse everyone and ignite them";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => new Color(1f, 0.3f, 0f);
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        CanUseVent = false,
        TasksCountForProgress = false,
        CanGetKilled = true,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        OptionsScreenshot = ArsonistAssets.ArsonistBanner,
        MaxRoleCount = 1
    };

    public HashSet<byte> dousedPlayers = new HashSet<byte>();

    public int DousedAlive => dousedPlayers.Count(id =>
    {
        var player = Utils.PlayerById(id);
        return player != null
            && player.Data != null
            && !player.Data.IsDead
            && !player.Data.Disconnected;
    });

    public (bool Douse, bool Ignite) ResetTimer = (false, false); // Douse = reset du bouton Douse, Ignite = reset du bouton Ignite

    public Arsonist()
    {
        this.SetCanDoTasks(false);
    }

    public void HudUpdate(HudManager hudManager)
    {
        foreach (var playerId in dousedPlayers)
        {
            var player = Utils.PlayerById(playerId);
            var data = player?.Data;

            if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead) continue;

            player.myRend().material.SetColor("_VisorColor", RoleColor);
            player.cosmetics.nameText.color = Color.black;
        }

        if (MeetingHud.Instance != null)
        {
            foreach (var state in MeetingHud.Instance.playerStates)
            {
                if (!VoteAreaHelper.TryGetPlayerId(state, out var targetId)) continue;

                var playerData = Utils.PlayerById(targetId)?.Data;
                if (playerData == null || playerData.Disconnected)
                {
                    dousedPlayers.Remove(targetId);
                    continue;
                }

                if (dousedPlayers.Contains(targetId))
                    state.NameText.color = Color.black;
            }
        }
    }

    public void Ignite()
    {
        foreach (var playerId in dousedPlayers)
        {
            var player = Utils.PlayerById(playerId);
            if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected) continue;

            PlayerControl.LocalPlayer.RpcCustomMurder(player, teleportMurderer: false);
        }

        dousedPlayers.Clear();
    }

    public bool GameEnd(LogicGameFlowNormal __instance)
    {
        if (Player.Data.IsDead || Player.Data.Disconnected) return true;

        var alive = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => x.Data != null && !x.Data.IsDead && !x.Data.Disconnected)
            .ToArray();

        if (alive.Length <= 2 && alive.Count(x => x.Data.Role.IsImpostor || x.Data.Role is Arsonist) == 1)
        {
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReasonsEnum.ArsonistWins, false);
            return false;
        }

        return false;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return gameOverReason == (GameOverReason)CustomGameOverReasonsEnum.ArsonistWins;
    }
}

internal static class VoteAreaHelper
{
    private static readonly string[] IdNames = { "TargetPlayerId", "targetPlayerId", "PlayerId", "playerId" };
    private static PropertyInfo idProperty;
    private static FieldInfo idField;
    private static bool resolved;

    private static void Resolve()
    {
        if (resolved) return;
        resolved = true;

        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        foreach (var name in IdNames)
        {
            idProperty = typeof(PlayerVoteArea).GetProperty(name, flags);
            if (idProperty != null) return;

            idField = typeof(PlayerVoteArea).GetField(name, flags);
            if (idField != null) return;
        }
    }

    public static bool TryGetPlayerId(PlayerVoteArea voteArea, out byte id)
    {
        id = 0;
        if (voteArea == null) return false;

        Resolve();

        object rawId = null;
        if (idProperty != null)
            rawId = idProperty.GetValue(voteArea);
        else if (idField != null)
            rawId = idField.GetValue(voteArea);

        if (rawId != null)
        {
            id = unchecked((byte)System.Convert.ToInt32(rawId));
            return true;
        }

        if (voteArea.NameText == null) return false;
        var displayed = voteArea.NameText.text;

        var match = PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(p => p.Data != null && p.Data.PlayerName == displayed);

        if (match == null) return false;

        id = match.PlayerId;
        return true;
    }
}