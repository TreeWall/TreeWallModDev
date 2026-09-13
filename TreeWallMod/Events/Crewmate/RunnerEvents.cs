using TreeWallMod.Roles.Crewmate;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using TownOfUs.Roles.Impostor;
using TownOfUs.Utilities;

namespace TreeWallMod.Events.Crewmate
{
    public static class RunnerEvents
    {
        [RegisterEvent]
        public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
        {
            if (!@event.Target.AmOwner || !@event.Target.IsRole<RunnerRole>() || MeetingHud.Instance)
            {
                return;
            }

            if (@event.Target.MyPhysics.Velocity.magnitude != 0)
            {
                Message("Prevent kill!");
                @event.Cancel();
                Message($"{(@event.Target.Data.IsDead ? "Dead" : "Alive")}");
            }
        }
    }
}