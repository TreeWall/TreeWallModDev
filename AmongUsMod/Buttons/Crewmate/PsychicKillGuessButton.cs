using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.Localization;
using TownOfUs.Networking;
using TownOfUs.Utilities;
using TreeWallMod.Modifiers.Crewmate;
using TreeWallMod.Options.Roles.Crewmate;
using TreeWallMod.Roles.Crewmate;
using UnityEngine;

namespace TreeWallMod.Buttons.Crewmate
{
	public sealed class PsychicKillGuessButton : TownOfUsRoleButton<PsychicRole>
	{
		public override string Name => "Guess Killer";
		public override Color TextOutlineColor => new Color32(165, 231, 89, 255);
		public override float Cooldown => OptionGroupSingleton<PsychicOptions>.Instance.PsychicGuessCd;
		public override float InitialCooldown => OptionGroupSingleton<PsychicOptions>.Instance.PsychicGuessCd;
		public override ButtonLocation Location => ButtonLocation.BottomLeft;
		public override LoadableAsset<Sprite> Sprite => Assets.CrewAssets.PsychicKillGuessSprite;

		public override bool UsableInDeath => false;
		public override float EffectDuration => 3.0f;

		private PlayerControl Victim;

		public override void ClickHandler()
		{
			if (!CanClick())
			{
				return;
			}

			OnClick();
		}

		protected override void OnClick()
		{
			var targetPlayer = PlayerControl.LocalPlayer;
			targetPlayer.NetTransform.Halt();

			if (Minigame.Instance)
			{
				return;
			}

			var player1Menu = CustomPlayerMenu.Create();
			player1Menu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material =
				PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
			player1Menu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material =
				PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;

			player1Menu.Begin(
				plr => !(plr.HasDied() || (plr.TryGetModifier<DisabledModifier>(out var mod) && (!mod.IsConsideredAlive || !mod.CanBeInteractedWith))),
				plr =>
				{
					player1Menu.ForceClose();

					if (plr == null || plr.Data.IsDead || plr.Data.Disconnected)
					{
						return;
					}

					Victim = plr;
					EffectActive = true;
					Timer = EffectDuration;
				});
			foreach (var panel in player1Menu.potentialVictims)
			{
				if (panel.NameText.text != PlayerControl.LocalPlayer.Data.PlayerName)
				{
					panel.NameText.color = Color.white;
				}
			}
		}

		public override void OnEffectEnd()
		{
			if (Victim.HasDied() || (Victim.TryGetModifier<DisabledModifier>(out var mod) && (!mod.IsConsideredAlive || !mod.CanBeInteractedWith)))
			{
				return;
			}

			var targetPlayer = PlayerControl.LocalPlayer;
			if (targetPlayer.TryGetModifier<PsychicStorageModifier>(out var psychic) && psychic.HasKiller(Victim))
			{
				try
				{
					targetPlayer.RpcSpecialMurder(Victim, MeetingCheck.OutsideMeeting, true, teleportMurderer: false, showKillAnim: true, playKillSound: false, causeOfDeath: "PsychicGuess");
				}
				catch
				{
					targetPlayer.RpcSpecialMurder(Victim, MeetingCheck.OutsideMeeting, true, teleportMurderer: false, showKillAnim: false, playKillSound: false, causeOfDeath: "PsychicGuess");
				}
			}
			else if (OptionGroupSingleton<PsychicOptions>.Instance.WrongGuessToggle)
			{
				try
				{
					targetPlayer.RpcSpecialMurder(targetPlayer, MeetingCheck.OutsideMeeting, true, teleportMurderer: false, showKillAnim: true, playKillSound: false, causeOfDeath: "PsychicMisguess");
				}
				catch
				{
					targetPlayer.RpcSpecialMurder(targetPlayer, MeetingCheck.OutsideMeeting, true, teleportMurderer: false, showKillAnim: false, playKillSound: false, causeOfDeath: "PsychicMisguess");
				}
			}
		}
	}
}
