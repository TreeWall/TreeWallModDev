using AmongUs.GameOptions;
using TreeWallMod.Assets;
using TreeWallMod.Events.Crewmate;
using TreeWallMod.Modifiers.Crewmate;
using TreeWallMod.Modules;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using UnityEngine;

namespace TreeWallMod.Roles.Crewmate
{
	public sealed class PsychicRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IWikiDiscoverable, IDoomable
	{
		public string LocaleKey => "Psychic";
		public string RoleName => TouLocale.Get($"TreeWallMod{LocaleKey}");
		public string RoleDescription => TouLocale.GetParsed($"TreeWallMod{LocaleKey}IntroBlurb");
		public string RoleLongDescription => TouLocale.GetParsed($"TreeWallMod{LocaleKey}TabDescription");

		public bool IsPowerCrew => false;
		public Color RoleColor => TreeWallMod.Colors.Psychic;
		public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
		public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;
		public DoomableType DoomHintType => DoomableType.Relentless;

		public CustomRoleConfiguration Configuration => new(this)
		{
			Icon = RoleIcons.Psychic
		};


		public override void Initialize(PlayerControl player)
		{
			RoleBehaviourStubs.Initialize(this, player);

			if (player.AmOwner)
			{
				if (player.HasModifier<PsychicStorageModifier>()) player.RpcRemoveModifier<PsychicStorageModifier>();
				player.RpcAddModifier<PsychicStorageModifier>();
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
				Message($"Client: {PlayerControl.LocalPlayer.name}");
				if (Player.HasModifier<PsychicStorageModifier>()) Player.RpcRemoveModifier<PsychicStorageModifier>();
			}
		}
	}
}
