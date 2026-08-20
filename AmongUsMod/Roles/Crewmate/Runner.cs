using AmongUs.GameOptions;
using TreeWallMod.Assets;
using TreeWallMod.Buttons.Crewmate;
using TreeWallMod.Modifiers.Crewmate;
using TreeWallMod.Modules;
using TreeWallMod.Options.Roles.Crewmate;
using Il2CppInterop.Runtime.Attributes;
using InnerNet;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.LocalSettings;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Mono.Cecil;
using Reactor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using UnityEngine;


namespace TreeWallMod.Roles.Crewmate
{
	public sealed class RunnerRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IWikiDiscoverable, IUnguessable, IDoomable
    {
		public string LocaleKey => "Runner";
		public string RoleName => TouLocale.Get($"TreeWallMod{LocaleKey}");
		public string RoleDescription => TouLocale.GetParsed($"TreeWallMod{LocaleKey}IntroBlurb");
		public string RoleLongDescription => TouLocale.GetParsed($"TreeWallMod{LocaleKey}TabDescription");

		public bool IsGuessable => false;
		public bool IsPowerCrew => true;
		public Color RoleColor => TreeWallMod.Colors.Runner;
        public RoleBehaviour AppearAs => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<MayorRole>());
		public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;
		public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
		public DoomableType DoomHintType => DoomableType.Trickster;

		public CustomRoleConfiguration Configuration => new(this)
		{
			Icon = RoleIcons.Runner
		};


		public override void Initialize(PlayerControl player)
		{
			RoleBehaviourStubs.Initialize(this, player);

            if (player.AmOwner)
            {
                if (player.HasModifier<RunnerSpeedModifier>()) player.RpcRemoveModifier<RunnerSpeedModifier>();
                player.RpcAddModifier<RunnerSpeedModifier>();
            }
        }

		public override void Deinitialize(PlayerControl targetPlayer)
		{
			RoleBehaviourStubs.Deinitialize(this, targetPlayer);

			Clear();
		}

        public override void OnDeath(DeathReason reason)
        {
            RoleBehaviourStubs.OnDeath(this, reason);

            Clear();
        }

        public void Clear()
        {
            if (Player.AmOwner && Player != null)
            {
                if (Player.HasModifier<RunnerSpeedModifier>()) Player.RpcRemoveModifier<RunnerSpeedModifier>();
            }
        }
    }
}