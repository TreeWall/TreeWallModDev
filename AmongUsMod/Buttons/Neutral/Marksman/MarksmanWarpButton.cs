using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities.Assets;
using System;
using TownOfUs;
using TownOfUs.Buttons;
using TreeWallMod.Options.Roles.Neutral;
using TreeWallMod.Roles.Neutral;
using UnityEngine;

namespace TreeWallMod.Buttons.Neutral.Marksman
{
    public sealed class MarksmanWarpButton : TownOfUsRoleButton<MarksmanRole>
    {
        public override string Name => "Warp";
        public override Color TextOutlineColor => TownOfUsColors.Transporter;
        public override float Cooldown => Math.Clamp(OptionGroupSingleton<MarksmanOptions>.Instance.WarpCd + MapCooldown, 5f, 120f);
        public override ButtonLocation Location => ButtonLocation.BottomLeft;
        public override LoadableAsset<Sprite> Sprite => TouRoleIcons.Transporter;

        public override bool Enabled(RoleBehaviour? role)
        {
            var marksman = PlayerControl.LocalPlayer.GetRole<MarksmanRole>()!;

            return base.Enabled(role) && marksman.WarpMarking == MarksmanWarpState.Warp && marksman.UnlockedAbilities.Contains(MarksmanAbility.Warp);
        }

        protected override void OnClick()
        {
            var marksman = PlayerControl.LocalPlayer.GetRole<MarksmanRole>()!;

            marksman.WarpMarking = MarksmanWarpState.Used;
            if (marksman.WarpMarkedPlayer == null)
            {
                return;
            }
            MarksmanRole.RpcWarp(PlayerControl.LocalPlayer, marksman.WarpMarkedPlayer.PlayerId);
        }
    }
}
