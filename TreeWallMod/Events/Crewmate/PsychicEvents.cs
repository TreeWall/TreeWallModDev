using TreeWallMod.Roles.Crewmate;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;

namespace TreeWallMod.Events.Crewmate
{
    public static class PsychicEvents
    {
        [RegisterEvent]
        public static void AfterMurderEventHandler(AfterMurderEvent @event)
        {
            var psychic = PlayerControl.LocalPlayer.GetRole<PsychicRole>();

            if (psychic == null)
            {
                return;
            }

            psychic.AddKiller(@event.Source);

            Message($"Added {@event.Source.name} to List");
        }
    }
}
