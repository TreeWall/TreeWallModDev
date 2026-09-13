using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TreeWallMod.Assets
{
    public static class CrewAssets
    {
        private const string ShortPath = "TreeWallMod.Resources.CrewButtons";
        public static LoadableAsset<Sprite> RunnerCaffeineSprite { get; } = new LoadableResourceAsset($"{ShortPath}.Caffeine.png");
        public static LoadableAsset<Sprite> PsychicKillGuessSprite { get; } = new LoadableResourceAsset($"{ShortPath}.PsychicActivate.png");
        public static LoadableAsset<Sprite> SyringeInjectSprite { get; } = new LoadableResourceAsset($"{ShortPath}.Inject.png");
    }
}