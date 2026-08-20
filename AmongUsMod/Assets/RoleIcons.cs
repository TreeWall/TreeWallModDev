using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TreeWallMod.Assets
{
    public static class RoleIcons
    {
        private const string ShortPath = "TreeWallMod.Resources.RoleIcons";

        // crew
        public static LoadableAsset<Sprite> Runner { get; } = new LoadableResourceAsset($"{ShortPath}.Runner.png");
        public static LoadableAsset<Sprite> Psychic { get; } = new LoadableResourceAsset($"{ShortPath}.Psychic.png");
        public static LoadableAsset<Sprite> Syringe { get; } = new LoadableResourceAsset($"{ShortPath}.Syringe.png");

        // neutral
        public static LoadableAsset<Sprite> Marksman { get; } = new LoadableResourceAsset($"{ShortPath}.Marksman.png");
    }
}