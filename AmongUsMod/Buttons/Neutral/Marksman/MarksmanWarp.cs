using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs;
using TownOfUs.Buttons;
using TownOfUs.Roles.Crewmate;
using TreeWallMod.Options.Roles.Neutral;
using TreeWallMod.Roles.Neutral;
using UnityEngine;

namespace TreeWallMod.Buttons.Neutral.Marksman
{
	public sealed class MarksmanWarp : TownOfUsRoleButton<MarksmanRole, PlayerControl>
	{
		public override string Name => Marking ? "Mark" : "Warp";
		public override Color TextOutlineColor => Marking ? (Colors.Marksman) : (TownOfUsColors.Transporter);
		public override float Cooldown => Marking ? OptionGroupSingleton<MarksmanOptions>.Instance.WarpMarkCd : OptionGroupSingleton<MarksmanOptions>.Instance.WarpCd;
		public override ButtonLocation Location => ButtonLocation.BottomRight;
		public override LoadableAsset<Sprite> Sprite => Assets.RoleIcons.Marksman;

		public bool Marking { get; set; } = true;
		public PlayerControl? WarpMarkedPlayer { get; set; } = null;
		public bool Used { get; set; } = false;

		private bool markingDoClick = true;

		public override PlayerControl? GetTarget()
		{
			return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
		}

		public override bool IsTargetValid(PlayerControl? target)
		{
			return (base.IsTargetValid(target) && target != null);
		}

        public override bool CanUse()
        {
			if (Used)
			{
				return false;
			}
            else if (Marking)
			{
				return base.CanUse();
			}
			else
			{
				return true;
			}
        }

        public override void ClickHandler()
        {
            if (Marking)
            {
                WarpMarkedPlayer = Target;
                Marking = false;
            }
            else
            {
				Used = true;
            }
            base.ClickHandler();
        }

		protected override void OnClick()
		{
			if (markingDoClick)
			{
				markingDoClick = false;
				return;
			}

			if (WarpMarkedPlayer == null)
			{
				return;
			}
		}
	}
}
