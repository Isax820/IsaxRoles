using MiraAPI.GameOptions;
using System;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;

namespace IsaxRoles;

public class ArsonistOptions : AbstractOptionGroup<Arsonist>
{ 
    public override string GroupName => "Arsonist";
    public override Color GroupColor => new Color(1f, 0.3f, 0f);

    [ModdedNumberOption("Douse Cooldown", 5, 60, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DouseCooldown { get; set; } = 30f;

    [ModdedNumberOption("Maximum Players Doused", 1, 15, 1f)]
    public float MaxDousedPlayers { get; set; } = 3f;

    public ModdedToggleOption ArsonistHasImpVision { get; } = new("Has Impostor vision", false);
}