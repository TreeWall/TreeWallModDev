using AmongUs.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Translation;
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
using TreeWallMod.Assets;
using TreeWallMod.Events.Crewmate;
using TreeWallMod.Modifiers.Crewmate;
using TreeWallMod.Modules;
using UnityEngine;

namespace TreeWallMod.Roles.Crewmate
{
	public sealed class PsychicRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IWikiDiscoverable, IDoomable
	{
        //public string LocaleKey => "Psychic";
        //      public string RoleName => MiraLocaleManager.Get($"TreeWallMod{LocaleKey}");
        //      public string RoleDescription => MiraLocaleManager.GetParsed($"TreeWallMod{LocaleKey}IntroBlurb", [], string.Empty);
        //      public string RoleLongDescription => MiraLocaleManager.GetParsed($"TreeWallMod{LocaleKey}TabDescription", [], string.Empty);

        public string IdPart => "Psychic";
        string ICustomRole.IdPrefix => "TreeWallMod.Role";

        public bool IsPowerCrew => false;
		public Color RoleColor => Colors.Psychic;
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
