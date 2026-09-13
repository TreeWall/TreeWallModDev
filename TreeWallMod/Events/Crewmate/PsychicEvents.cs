using TreeWallMod.Buttons.Crewmate;
using TreeWallMod.Modifiers.Crewmate;
using TreeWallMod.Roles.Crewmate;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Utilities;

namespace TreeWallMod.Events.Crewmate
{
    public static class PsychicEvents
    {
        [RegisterEvent]
        public static void AfterMurderEventHandler(AfterMurderEvent @event)
        {
            if (!PlayerControl.LocalPlayer.IsRole<PsychicRole>())
            {
                return;
            }

            if (PlayerControl.LocalPlayer.TryGetModifier<PsychicStorageModifier>(out var psychic))
            {
                psychic.AddKiller(@event.Source);
            }

            Logger<TreeWallMod.TreeWallModPlugin>.Instance.LogMessage($"Added {@event.Source.name} to List");
        }
    }
}
