using MiraAPI.GameOptions;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using UnityEngine;

namespace TreeWallMod.Options.Modifiers
{
    public sealed class UniversalModifierOptions : AbstractOptionGroup
    {
        public override string GroupName => "Headless Modifiers";
        public override MenuCategory ParentMenu => MenuCategory.Modifiers;
        public override Color GroupColor => Colors.HeadlessModifier;

        public ModdedNumberOption HeadlessAmount { get; } =
            new("Headless Amount", 0, 0, 15, 1, MiraNumberSuffixes.None);

        public ModdedNumberOption HeadlessChance { get; } =
            new("Headless Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
            {
                Visible = () => OptionGroupSingleton<UniversalModifierOptions>.Instance.HeadlessAmount > 0
            };
    }
}
