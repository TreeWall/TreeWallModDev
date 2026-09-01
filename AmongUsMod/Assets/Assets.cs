using MiraAPI.Utilities.Assets;
using MiraAPI.Utilities;
using Reactor.Utilities;
using UnityEngine;  

namespace TreeWallMod.Assets
{
    public static class Assets
    {
        private const string ShortPath = "TreeWallMod.Resources";

        public static readonly AssetBundle Bundle = AssetBundleManager.Load("rfsbundle");

        public static readonly LoadableBundleAsset<AnimationClip> HeadlessWalkAnim = new LoadableBundleAsset<AnimationClip>("walk.anim", Bundle);
        public static readonly LoadableBundleAsset<AnimationClip> HeadlessIdleAnim = new LoadableBundleAsset<AnimationClip>("idle_0.anim", Bundle);
        public static readonly LoadableBundleAsset<AnimationClip> VanishPoofAnim = new LoadableBundleAsset<AnimationClip>("Vanish_Poof.anim", Bundle);
        public static readonly LoadableBundleAsset<AnimationClip> VanishChargeAnim = new LoadableBundleAsset<AnimationClip>("Vanish_ChargeLoop.anim", Bundle);

        public static LoadableAudioResourceAsset FartKillSound1 { get; } = new LoadableAudioResourceAsset($"{ShortPath}.Audio.FartKill1.wav");
        public static LoadableAudioResourceAsset FartKillSound2 { get; } = new LoadableAudioResourceAsset($"{ShortPath}.Audio.FartKill2.wav");
        public static LoadableAudioResourceAsset FartKillSound3 { get; } = new LoadableAudioResourceAsset($"{ShortPath}.Audio.FartKill3.wav");
    }
}