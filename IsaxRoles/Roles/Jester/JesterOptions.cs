using MiraAPI.GameOptions;
using System;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace IsaxRoles;

public class JesterOptions : AbstractOptionGroup<Jester>
{ 
    public override string GroupName => "Jester";
    public override Color GroupColor => Color.magenta;

    public ModdedToggleOption JesterCanVent { get; } = new("Jester Can Vent", false);
    public ModdedToggleOption JesterHasImpVision { get; } = new("Has Impostor vision", true);
}