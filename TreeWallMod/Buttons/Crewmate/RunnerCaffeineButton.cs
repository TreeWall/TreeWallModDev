using TreeWallMod.Assets;
using TreeWallMod.Modifiers.Crewmate;
using TreeWallMod.Options.Roles.Crewmate;
using TreeWallMod.Roles.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking;
using Reactor.Utilities;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Utilities;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace TreeWallMod.Buttons.Crewmate
{
	public sealed class RunnerCaffeineButton : TownOfUsRoleButton<RunnerRole>
	{
		public override string Name => "Caffeine";
		public override BaseKeybind Keybind => Keybinds.PrimaryAction;
		public override Color TextOutlineColor => Colors.Runner;
		public override float Cooldown => OptionGroupSingleton<RunnerOptions>.Instance.CaffeineCooldown;
		public override float EffectDuration => OptionGroupSingleton<RunnerOptions>.Instance.CaffeineDuration.Value;
        public override int MaxUses => (int)OptionGroupSingleton<RunnerOptions>.Instance.CaffeineUses;
		public override LoadableAsset<Sprite> Sprite => CrewAssets.RunnerCaffeineSprite;
		public override bool ZeroIsInfinite { get; set; } = true;

		protected override void OnClick()
		{
			var runner = PlayerControl.LocalPlayer.GetRole<RunnerRole>()!;

			float speed = 0;

            if (OptionGroupSingleton<RunnerOptions>.Instance.CaffeineStack && 
				runner.SpeedMultiplier + OptionGroupSingleton<RunnerOptions>.Instance.SpeedMultiplier - 1 > OptionGroupSingleton<RunnerOptions>.Instance.SpeedLimit.Value && 
				OptionGroupSingleton<RunnerOptions>.Instance.SpeedLimit.Value != 0)
			{
				speed = runner.SpeedMultiplier;
			}
			else
			{
				speed = (runner.SpeedMultiplier == 1 ? 0 : runner.SpeedMultiplier) + OptionGroupSingleton<RunnerOptions>.Instance.SpeedMultiplier;
            }

			RunnerRole.RpcSetRunnerSpeed(runner.Player, speed, true);

			Message($"Current Multiplier: {runner.SpeedMultiplier}, Active: {runner.SpeedActive}");
        }

        public override void OnEffectEnd()
		{
            var runner = PlayerControl.LocalPlayer.GetRole<RunnerRole>()!;

			float speed = OptionGroupSingleton<RunnerOptions>.Instance.CaffeineStack ? runner.SpeedMultiplier : 1f;

			RunnerRole.RpcSetRunnerSpeed(runner.Player, speed, false);
        }

	}
}