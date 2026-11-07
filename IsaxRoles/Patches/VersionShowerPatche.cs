using HarmonyLib;

namespace IsaxRoles;

[HarmonyPatch(typeof(VersionShower))]
public static class VersionShowerPatches
{
    [HarmonyPatch(nameof(VersionShower.Start))]
    [HarmonyPostfix]
    private static void StartPostfix(VersionShower __instance)
    {
        __instance.text.text += $"<size=80%> + <color=#5E4CA6FF>IsaxRoles v2026.11.7</color> by Isax820</size>";
    }
}