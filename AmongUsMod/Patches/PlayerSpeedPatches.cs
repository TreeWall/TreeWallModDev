using TreeWallMod.Modifiers.Crewmate;
using TreeWallMod.Options.Roles.Crewmate;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;

namespace TreeWallMod.Patches
{
    [HarmonyPatch(typeof(LogicOptions), nameof(LogicOptions.GetPlayerSpeedMod))]
    public static class PlayerSpeedPatch
    {
        public static void Postfix(PlayerControl pc, ref float __result)
        {
            if (pc == null) return;

            if (pc.TryGetModifier<RunnerSpeedModifier>(out var runner) && (runner.active || OptionGroupSingleton<RunnerOptions>.Instance.PermanentSpeed))
            {
                __result *= runner.speedMultiplier;
            }
        }
    }
}