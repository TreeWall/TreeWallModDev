using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI;
using MiraAPI.GameOptions;
using MiraAPI.PluginLoading;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TreeWallMod.Options;

namespace TreeWallMod
{
	[BepInAutoPlugin("com.treewall.TreeWallMod", "TreeWallMod")]
	[BepInProcess("Among Us.exe")]
	[BepInDependency(ReactorPlugin.Id)]
	[BepInDependency(MiraApiPlugin.Id)]
	[ReactorModFlags(ModFlags.RequireOnAllClients)]
	public partial class TreeWallModPlugin : BasePlugin, IMiraPlugin
	{
		public Harmony Harmony { get; } = new(Id);

		public string OptionsTitleText => "Among Us Mod :D";
        public static bool IsDevBuild => true;

        public ConfigFile GetConfigFile() => Config;

		public override void Load()
		{
            IL2CPPChainloader.Instance.Finished += Modules.ExtensionLocale.SearchInternalLocale;

            Harmony.PatchAll();

			Message("TreeWallMod Loaded!");

			var assembly = Assembly.GetExecutingAssembly();
			string[] resourceNames = assembly.GetManifestResourceNames();

            Message("--- LISTING ALL EMBEDDED RESOURCES ---");
			foreach (string name in resourceNames)
			{
				Message($"Found resource: {name}");
			}
			Message("-------------------------------------");

			Message("--- BUNDLE RESOURCES ---");
            foreach(var name in TreeWallMod.Assets.Assets.Bundle.GetAllAssetNames())
				Message("Bundle asset: " + name);

            Patches.ChangeSoundPatch.RegisterSwap("impostor_kill", () =>
            {
                int rand = UnityEngine.Random.RandomRangeInt(0, 3);

                switch (rand)
                {
                    case 0:
                        return Assets.Assets.FartKillSound1;

                    case 1:
                        return Assets.Assets.FartKillSound2;

                    case 2:
                        return Assets.Assets.FartKillSound3;

                    default:
                        return Assets.Assets.FartKillSound2;
                }
            },
            isEnabled: () => OptionGroupSingleton<GeneralOptions>.Instance.FartKill);
        }
	}
}
