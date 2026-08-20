using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Buttons;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Networking;
using TreeWallMod.Buttons.Neutral.Marksman;
using TreeWallMod.Modifiers.Neutral;
using TreeWallMod.Modules;
using TreeWallMod.Options.Roles.Neutral;
using TreeWallMod.Roles.Neutral;
using UnityEngine;
using static Rewired.Demos.CustomPlatform.MyPlatformControllerExtension;
using static UnityEngine.GraphicsBuffer;

namespace TreeWallMod.Events.Neutral
{
	public static class MarksmanEvents
	{
		[RegisterEvent]
		public static void AfterMurderEventHandler(AfterMurderEvent @event)
		{
			Message($"{@event.Target.name} got killed by {@event.Source.name}");

			if (!PlayerControl.LocalPlayer.IsRole<MarksmanRole>())
			{
				return;
			}

			var button = CustomButtonSingleton<MarksmanDiscover>.Instance;
			if ((button.FirstTarget != null) && @event.Target == button.FirstTarget && !MeetingHud.Instance)
			{
				++button.UsesLeft;
				if ((int)OptionGroupSingleton<MarksmanOptions>.Instance.InitialDiscoverUses != 0)
				{
					button.SetUses(button.UsesLeft);
					Message($"Added a use as {@event.Target} died");
				}
				button.FirstTarget = null;
			}

			if ((button.SecondTarget != null) && @event.Target == button.SecondTarget && !MeetingHud.Instance)
			{
				++button.UsesLeft;
				if ((int)OptionGroupSingleton<MarksmanOptions>.Instance.InitialDiscoverUses != 0)
				{
					button.SetUses(button.UsesLeft);
					Message($"Added a use as {@event.Target} died");
				}
				button.SecondTarget = null;
			}

			if (@event.Source.AmOwner)
			{
				button.KilledPlayer();
			}
		}

		[RegisterEvent]
		public static void StartMeetingEventHandler(StartMeetingEvent @event)
		{
			var marksman = PlayerControl.LocalPlayer.GetRole<MarksmanRole>();
			if (marksman != null)
			{
				marksman.WarpMarkedPlayer = null;
				marksman.WarpMarking = MarksmanWarpState.Marking;
			}
		}

		[RegisterEvent]
		public static void AfterMarksmanSuppresedModifierPlayerMurderHandler(AfterMurderEvent @event)
		{
			if (@event.Target.AmOwner && MeetingHud.Instance 
				&& @event.Target.TryGetModifier<MarksmanSuppressedModifier>(out var marksmanSuppressedModifier) &&
				marksmanSuppressedModifier.Killer != @event.Source)
			{
				PlayerControl.LocalPlayer.RpcRemoveModifier<MarksmanSuppressedModifier>();
			}
		}

		[RegisterEvent]
		public static void MarksmanSuppresedModifierVotingCompleteEventHandler(VotingCompleteEvent @event)
		{
			if (!PlayerControl.LocalPlayer.IsHost())
			{
				return;
			}

			var suppressedPlayers = PlayerControl.AllPlayerControls.ToArray()
				.Where(x => !x.Data.IsDead && x.HasModifier<MarksmanSuppressedModifier>());

			Message($"{suppressedPlayers.Count()} players are to be Suppressed");
			foreach (var plr in suppressedPlayers)
			{
				Message($"Deciding {plr.name}'s fate");
				if (!plr.TryGetModifier<MarksmanSuppressedModifier>(out var marksmanSuppressedMod))
				{
					Message("Doesnt have marksman mod");
					continue;
				}

				plr.RpcMarksmanSuppressedComplete(marksmanSuppressedMod);
			}
		}
	}
}
