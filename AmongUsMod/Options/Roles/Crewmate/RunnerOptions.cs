using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TreeWallMod.Roles.Crewmate;

namespace TreeWallMod.Options.Roles.Crewmate
{
	public sealed class RunnerOptions : AbstractOptionGroup<RunnerRole>
	{
		public override string GroupName => "Runner";

		[ModdedNumberOption("Runner Caffeine Cooldown", 5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
		public float CaffeineCooldown { get; set; } = 20f;

		[ModdedNumberOption("Runner Caffeine Duration", 5f, 25f, 1f, MiraNumberSuffixes.Seconds)]
		public float CaffeineDuration { get; set; } = 7f;

		[ModdedNumberOption("Runner Caffeine Speed Multiplier", 1f, 10f, 0.1f, MiraNumberSuffixes.Multiplier)]
		public float SpeedMultiplier { get; set; } = 2f;

		[ModdedToggleOption("Stack Caffeine")]
		public bool CaffeineStack { get; set; } = false;

        [ModdedToggleOption("Permanent Speed Buff")]
        public bool PermanentSpeed { get; set; } = false;

        public ModdedNumberOption SpeedLimit { get; set; } = new ModdedNumberOption(
												"Runner Caffeine Speed Limit",
												20f, 0f, 100f, 1f,
												MiraNumberSuffixes.Multiplier,
												zeroInfinity: true)
		{
			Visible = () => OptionGroupSingleton<RunnerOptions>.Instance.CaffeineStack
		};

	}
}