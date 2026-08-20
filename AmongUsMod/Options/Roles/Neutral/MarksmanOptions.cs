using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeWallMod.Roles.Crewmate;
using TreeWallMod.Roles.Neutral;
using UnityEngine;

namespace TreeWallMod.Options.Roles.Neutral
{
	public sealed class MarksmanOptions : AbstractOptionGroup<MarksmanRole>
	{
		public override string GroupName => "Marksman";

		[ModdedNumberOption("Kill Cooldown", 10f, 60f, 10f, MiraNumberSuffixes.Seconds)]
		public float KillCd { get; set; } = 15f;

		[ModdedNumberOption("Discover Cooldown", 5f, 30f, 5f, MiraNumberSuffixes.Seconds)]
		public float DiscoverCd { get; set; } = 15f;

		public ModdedNumberOption InitialDiscoverUses { get; } = new("Starting Discover Uses", 0, 0, 15, 1, MiraNumberSuffixes.None, zeroInfinity: true);

		public ModdedNumberOption NewDiscoverKillsRequired { get; } = new("Kills/Guesses Required for new discover", 2, 1, 3, 1, MiraNumberSuffixes.None)
		{
			Visible = () => OptionGroupSingleton<MarksmanOptions>.Instance.InitialDiscoverUses != 0
		};

		public ModdedEnumOption<MarksmanRoleHintEnum> RoleHint { get; } = new("Role hint", MarksmanRoleHintEnum.Subalignment);

		public ModdedNumberOption RoleAmount { get; } = new("Amount of Roles revealed in Role Hint", 7, 2, 7, 1, MiraNumberSuffixes.None)
		{
			Visible = () => OptionGroupSingleton<MarksmanOptions>.Instance.RoleHint == MarksmanRoleHintEnum.ListRoles
		};

		[ModdedToggleOption("Gets Misguess")]
		public bool MisguessAvailable { get; set; } = true;

		[ModdedNumberOption("Abilities obtained from each guess", 1, 7, 1, MiraNumberSuffixes.None)]
		public float AbilitiesEachGuess { get; set; } = 1;

		[ModdedNumberOption("Ability Cap", 1, 7, 1, MiraNumberSuffixes.None)]
		public float AbilityCap { get; set; } = 7;

		[ModdedNumberOption("Sharpened Blade Reduces Kill cooldown by", 5, 10, 0.5f, MiraNumberSuffixes.Seconds)]
		public float SharpenedBladeKillCdReduction { get; set; } = 7;

		[ModdedNumberOption("Warp mark cooldown", 5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
		public float WarpMarkCd { get; set; } = 25f;

		[ModdedNumberOption("Warp Cooldown after Marking", 5, 20, 2.5f, MiraNumberSuffixes.Seconds)]
		public float WarpCd { get; set; } = 15f;
    }

	public enum MarksmanRoleHintEnum
	{
        Subalignment,
        ListRoles,
		DoomHint
	}
}
