using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Buttons;
using TreeWallMod.Modifiers.Crewmate;
using TreeWallMod.Options.Roles.Crewmate;
using TreeWallMod.Options.Roles.Neutral;
using TreeWallMod.Roles.Neutral;
using UnityEngine;

namespace TreeWallMod.Buttons.Neutral.Marksman
{
	public sealed class MarksmanDiscover : TownOfUsRoleButton<MarksmanRole, PlayerControl>
	{
		public override string Name => "Discover";
		public override Color TextOutlineColor => Colors.Marksman;
		public override float Cooldown => OptionGroupSingleton<MarksmanOptions>.Instance.DiscoverCd;
		public override int MaxUses => (int)OptionGroupSingleton<MarksmanOptions>.Instance.InitialDiscoverUses;
		public override ButtonLocation Location => ButtonLocation.BottomRight;
		public override LoadableAsset<Sprite> Sprite => Assets.RoleIcons.Marksman;

		public override bool UsableInDeath => false;

		public PlayerControl? FirstTarget { get; set; } = null;
		public PlayerControl? SecondTarget { get; set; } = null;

		int Kills = 0;
		public void KilledPlayer()
		{
			Kills += 1;
			Kills %= OptionGroupSingleton<MarksmanOptions>.Instance.NewDiscoverKillsRequired;

			Message($"Killed someone");
			if (Kills == 0 && OptionGroupSingleton<MarksmanOptions>.Instance.InitialDiscoverUses != 0)
			{
				++UsesLeft;
				SetUses(UsesLeft);
				Message($"Added a use as killed");
			}
		}

		public override PlayerControl? GetTarget()
		{
			return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
		}

		public override bool IsTargetValid(PlayerControl? target)
		{
			var marksman = PlayerControl.LocalPlayer.GetRole<MarksmanRole>()!;

			return (
				base.IsTargetValid(target) && target != null &&
				(FirstTarget == null || (marksman.UnlockedAbilities.Contains(MarksmanAbility.Dualscover) && SecondTarget == null)));
		}

		public override bool CanUse()
		{
			var marksman = PlayerControl.LocalPlayer.GetRole<MarksmanRole>()!;
			return (base.CanUse() && (FirstTarget == null || (marksman.UnlockedAbilities.Contains(MarksmanAbility.Dualscover) && SecondTarget == null)));
		}

		protected override void OnClick()
		{
			var marksman = PlayerControl.LocalPlayer.GetRole<MarksmanRole>();
			if (FirstTarget == null)
			{
				FirstTarget = Target;
			}
			else if (marksman.UnlockedAbilities.Contains(MarksmanAbility.Dualscover) && SecondTarget == null)
			{
				SecondTarget = Target;
			}
		}
	}
}
