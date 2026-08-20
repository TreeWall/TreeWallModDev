using AmongUs.Data;
using Assets.CoreScripts;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs;
using TownOfUs.Events;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modules;
using TownOfUs.Networking;
using TownOfUs.Roles;
using TreeWallMod.Modifiers.Crewmate;
using TreeWallMod.Modifiers.Neutral;
using UnityEngine;
using static Rewired.Demos.CustomPlatform.MyPlatformControllerExtension;
using static UnityEngine.GraphicsBuffer;

namespace TreeWallMod.Modules
{
	public static class StupidRolesRpc
	{
		[MethodRpc((uint)StupidRolesRpcEnum.ChangeAnimation)]
		public static void RpcChangeAnimation(this PlayerControl pc, PlayerAnimationClips pac, StoredAnimationClips animation, bool playIdleAnim = false)
		{
			Setter[(int)pac](pc.MyPhysics.Animations.group, LoadedAnimationClips[(int)animation]);
			if (playIdleAnim) pc.MyPhysics.Animations.PlayIdleAnimation();
		}

		static readonly Action<PlayerAnimationGroup, AnimationClip>[] Setter =
		{
			(g, a) => g.IdleAnim = a,
			(g, a) => g.RunAnim = a
		};

		public enum PlayerAnimationClips
		{
			Idle = 0,
			Run
		}

		public enum StoredAnimationClips
		{
			ogIdle = 0,
			ogWalk,
			headlessIdleAnim,
			headlessWalkAnim
		}

		static List<AnimationClip> LoadedAnimationClips = new List<AnimationClip>
		{
			PlayerControl.LocalPlayer.MyPhysics.Animations.group.IdleAnim,
			PlayerControl.LocalPlayer.MyPhysics.Animations.group.RunAnim,
			Assets.Assets.headlessIdleAnim.LoadAsset(),
			Assets.Assets.headlessWalkAnim.LoadAsset()
		};


		[MethodRpc((uint)StupidRolesRpcEnum.CosmeticControl)]
		public static void RpcCosmeticControl(this PlayerControl pc, bool active)
		{
			pc.cosmetics.gameObject.SetActive(active);
		}

		[MethodRpc((uint)StupidRolesRpcEnum.SurpassChecksDie)]
		public static void RpcSurpassChecksDie(this PlayerControl player, PlayerControl killer, string? deathReason = null)
		{
			string cod = "Killer";
			if (deathReason != null)
			{
				cod = deathReason;
			}

			if (!player.HasModifier<DeathHandlerModifier>())
			{
				DeathHandlerModifier.UpdateDeathHandlerImmediate(player, TouLocale.Get($"DiedTo{cod}"),
				DeathEventHandlers.CurrentRound,
				(!MeetingHud.Instance && !ExileController.Instance)
					? DeathHandlerOverride.SetTrue
					: DeathHandlerOverride.SetFalse,
				TouLocale.GetParsed("DiedByStringBasic").Replace("<player>", killer.Data.PlayerName),
				lockInfo: DeathHandlerOverride.SetTrue);
			}

			player.Die(DeathReason.Kill, false);
			var @event = new AfterMurderEvent(killer, player, null);
			MiraEventManager.InvokeEvent(@event);
		}

		[MethodRpc((uint)StupidRolesRpcEnum.RemovePlayerSyringeInject)]
		public static void RpcRemovePlayerSyringeInject(this PlayerControl injected, PlayerControl syringe)
		{
			if (!injected.TryGetModifier<SyringeInjectedModifier>(out var syringeInjectedMod))
			{
				return;
			}

			syringeInjectedMod.RemovePlayer(syringe);
			Message($"Removed {syringe.name} from {syringeInjectedMod.Player.name}");
		}

		[MethodRpc((uint)StupidRolesRpcEnum.AddPlayerSyringeInject)]
		public static void RpcAddPlayerSyringeInject(this PlayerControl injected, PlayerControl syringe)
		{
			if (!injected.TryGetModifier<SyringeInjectedModifier>(out var syringeInjectedMod))
			{
				Message("Doesnt Have modifier!");
				return;
			}

			syringeInjectedMod.AddPlayer(syringe);
			Message($"Added {syringe.name} to {syringeInjectedMod.Player.name}");
		}

		[MethodRpc((uint)StupidRolesRpcEnum.MarksmanSuppressedComplete)]
		public static void RpcMarksmanSuppressedComplete(this PlayerControl p, MarksmanSuppressedModifier marksmanSuppressedMod)
		{
			//if (!p.TryGetModifier<MarksmanSuppressedModifier>(out var marksmanSuppressedMod))
			//{
			//	return;
			//}

			if (!marksmanSuppressedMod.murderResultFlags.HasFlag(MurderResultFlags.Succeeded))
			{
				return;
			}

            DeathHandlerModifier.UpdateDeathHandlerImmediate(p, TouLocale.Get($"DiedToMarksman"),
                DeathEventHandlers.CurrentRound,
                (!MeetingHud.Instance && !ExileController.Instance)
                    ? DeathHandlerOverride.SetTrue
                    : DeathHandlerOverride.SetFalse,
                TouLocale.GetParsed("DiedByStringBasic").Replace("<player>", marksmanSuppressedMod.Killer.Data.PlayerName),
                lockInfo: DeathHandlerOverride.SetTrue);

            p.Die(DeathReason.Kill, true);

            var afterMurderEvent = new AfterMurderEvent(marksmanSuppressedMod.Killer, p, null);
            MiraEventManager.InvokeEvent(afterMurderEvent);			

			// Dont FUCKING know why its like this
			if (PlayerControl.LocalPlayer.IsHost())
			{
				marksmanSuppressedMod.VoteArea.Overlay?.gameObject?.SetActive(false);
				marksmanSuppressedMod.VoteArea.XMark?.gameObject?.SetActive(false);
			}
			else
			{
                marksmanSuppressedMod.VoteArea.SetDead(marksmanSuppressedMod.VoteArea.DidReport, false);
            }

			if (PlayerControl.LocalPlayer.IsHost())
            {
                p.RpcRemoveModifier<MarksmanSuppressedModifier>();
            }
        }
	}
}
