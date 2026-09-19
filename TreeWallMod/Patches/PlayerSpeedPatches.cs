using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfUs.Options.Maps;
using TownOfUs.Patches;
using TreeWallMod.Modifiers.Crewmate;
using TreeWallMod.Modules;
using TreeWallMod.Options.Roles.Crewmate;
using TreeWallMod.Roles.Crewmate;

namespace TreeWallMod.Patches
{
    [HarmonyPatch(typeof(LogicOptions), nameof(LogicOptions.GetPlayerSpeedMod))]
    public static class PlayerSpeedPatch
    {
        public static void Postfix(PlayerControl pc, ref float __result)
        {
            if (pc == null || MeetingHud.Instance)
            {
                return;
            }

            var runner = pc.GetRole<RunnerRole>();

            if (runner != null && runner.SpeedActive)
            {
                __result *= runner.SpeedMultiplier;
            }
        }
    }
}