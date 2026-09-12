using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using System;
using System.Linq;
using System.Reflection;
using TownOfUs;
using TownOfUs.Modules;
using TownOfUs.Options;
using TownOfUs.Patches.Options;
using TownOfUs.Patches.Roles;
using TownOfUs.Roles;
using TownOfUs.Roles.Other;
using TreeWallMod.Modifiers.Neutral;
using UnityEngine;

namespace TreeWallMod.Patches
{
    [HarmonyPatch]
    public static class ChatPatches
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
        public static bool FPrefix(ChatController __instance)
        {
            // this patch only works in dev builds
            if (!TreeWallModPlugin.IsDevBuild)
            {
                return true;
            }

            var text = __instance.freeChatField.Text.ToLower(TownOfUsPlugin.Culture);
            var textRegular = __instance.freeChatField.Text.WithoutRichText();

            var systemName = $"<color=#8BFDFD>{TouLocale.GetParsed("SystemChatTitle")}</color>";

            if (textRegular.StartsWith("/modifier ", StringComparison.OrdinalIgnoreCase))
            {
                string targetName = textRegular.Substring(10).Trim();
                var target = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.Data?.PlayerName.Equals(targetName, StringComparison.OrdinalIgnoreCase) == true);

                if (target == null)
                {
                    MiscUtils.AddSystemChat(PlayerControl.LocalPlayer.Data, systemName,
                        $"<color=#FF0000>Player \"{targetName}\" not found.</color>");
                    ClearChat(__instance);
                    return false;
                }

                string outputString = "";

                foreach (var modifier in target.GetModifiers<BaseModifier>())
                    outputString += $"{modifier.ModifierName} ";

                MiscUtils.AddSystemChat(PlayerControl.LocalPlayer.Data, systemName,
                        $"{outputString}");
                ClearChat(__instance);

                return false;
            }
            else if (textRegular.StartsWith("/debug", StringComparison.OrdinalIgnoreCase))
            {
                var cam = Camera.main;
                var z = -89f;
                var bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.WorldToScreenPoint(new Vector3(0, 0, z)).z));
                var topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.WorldToScreenPoint(new Vector3(0, 0, z)).z));

                string outputString = $"Orthographic: {cam.orthographic}, Size: {cam.orthographicSize}, Aspect: {cam.aspect}, Position: {cam.transform.position}, Bottom Left: {bottomLeft}, Top Right: {topRight}";

                //Message($"Orthographic: {cam.orthographic}, Size: {cam.orthographicSize}, Aspect: {cam.aspect}, Position: {cam.transform.position}, Bottom Left: {bottomLeft}, Top Right: {topRight}");
                Message(outputString);

                MiscUtils.AddSystemChat(PlayerControl.LocalPlayer.Data, systemName,
                        $"{outputString}");

                ClearChat(__instance);
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.UpdateChatMode))]
        public static void UpdateChatModePostfix(ChatController __instance)
        {
            if (PlayerControl.LocalPlayer.HasModifier<MarksmanSuppressedModifier>() && MeetingHud.Instance)
            {
                __instance.freeChatField.SetVisible(false);
                __instance.quickChatField.SetVisible(false);
            }

            if (PlayerControl.LocalPlayer.HasModifier<MarksmanSuppressedModifier>() && !MeetingHud.Instance && PlayerControl.LocalPlayer.Data.IsDead)
            {
                __instance.freeChatField.SetVisible(true);
                __instance.quickChatField.SetVisible(true);
            }
        }

        private static void ClearChat(ChatController chat)
        {
            chat.freeChatField.Clear();
            chat.quickChatMenu.Clear();
            chat.quickChatField.Clear();
            chat.UpdateChatMode();
        }
    }
}
