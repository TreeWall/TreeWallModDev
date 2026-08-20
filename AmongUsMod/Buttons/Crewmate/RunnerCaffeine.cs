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
	public sealed class RunnerCaffeine : TownOfUsRoleButton<RunnerRole>
	{
		public override string Name => "Caffeine";
		public override BaseKeybind Keybind => Keybinds.PrimaryAction;
		public override Color TextOutlineColor => TreeWallMod.Colors.Runner;
		public override float Cooldown => OptionGroupSingleton<RunnerOptions>.Instance.CaffeineCooldown;
		public override float EffectDuration => OptionGroupSingleton<RunnerOptions>.Instance.CaffeineDuration;
		public override LoadableAsset<Sprite> Sprite => CrewAssets.RunnerCaffeineSprite;

		private static PlayerControl targetPlayer => PlayerControl.LocalPlayer;
		
		protected override void OnClick()
		{
			if (!targetPlayer.TryGetModifier<RunnerSpeedModifier>(out var runner))
			{
				Logger<TreeWallModPlugin>.Instance.LogError("Does not have Runner Modifier! Something went wrong!");
				return;
			}

            runner.active = true;

            if (OptionGroupSingleton<RunnerOptions>.Instance.CaffeineStack && 
				runner.speedMultiplier + OptionGroupSingleton<RunnerOptions>.Instance.SpeedMultiplier > OptionGroupSingleton<RunnerOptions>.Instance.SpeedLimit.Value && 
				OptionGroupSingleton<RunnerOptions>.Instance.SpeedLimit.Value != 0)
			{
				return;
			}

            Message($"Start {targetPlayer.MyPhysics.Speed} Internal: {runner.speedMultiplier} Limit: {OptionGroupSingleton<RunnerOptions>.Instance.SpeedLimit.Value}");

			runner.speedMultiplier += OptionGroupSingleton<RunnerOptions>.Instance.SpeedMultiplier; 
        }


        public override void OnEffectEnd()
		{
			if (targetPlayer.TryGetModifier<RunnerSpeedModifier>(out var runner))
			{
                runner.active = false;
                if (!OptionGroupSingleton<RunnerOptions>.Instance.CaffeineStack) runner.speedMultiplier = 1.0f;
            }

            float re = 0;
            if (targetPlayer.TryGetModifier<RunnerSpeedModifier>(out var e)) re = e.speedMultiplier;
            Message($"End {targetPlayer.MyPhysics.Speed} Internal: {re}");
        }

	}
}