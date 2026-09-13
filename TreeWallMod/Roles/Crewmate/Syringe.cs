using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
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
using TownOfUs.Interfaces;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TreeWallMod.Assets;
using TreeWallMod.Modifiers.Crewmate;
using TreeWallMod.Modules;
using UnityEngine;

namespace TreeWallMod.Roles.Crewmate
{
	public sealed class SyringeRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IWikiDiscoverable, IDoomable, ILoyalCrewmate
	{
        //public string LocaleKey => "Syringe";
        //      public string RoleName => MiraLocaleManager.Get($"TreeWallMod{LocaleKey}");
        //      public string RoleDescription => MiraLocaleManager.GetParsed($"TreeWallMod{LocaleKey}IntroBlurb", [], string.Empty);
        //      public string RoleLongDescription => MiraLocaleManager.GetParsed($"TreeWallMod{LocaleKey}TabDescription", [], string.Empty);

        public string IdPart => "Syringe";
		string ICustomRole.IdPrefix => "TreeWallMod.Role";

        public bool IsPowerCrew => false;
		public Color RoleColor => Colors.Syringe;
		public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
		public RoleAlignment RoleAlignment => RoleAlignment.CrewmatePower;
		public DoomableType DoomHintType => DoomableType.Fearmonger;

		public bool CanBeTraitor => false;
		public bool CanBeCrewpostor => false;
		public bool CanBeEgotist => false;
		public bool CanBeOtherEvil => false;

		public string GetAdvancedDescription()
		{
			return
                MiraLocaleManager.Get($"TreeWallMod.Role.{IdPart}.WikiDescription") +
				MiscUtils.AppendOptionsText(GetType());
		}

		[HideFromIl2Cpp]
		public List<CustomButtonWikiDescription> Abilities
		{
			get
			{
				return new List<CustomButtonWikiDescription>
				{
					new(MiraLocaleManager.Get($"TreeWallMod.Role.{IdPart}Inject", "Inject"),
                        MiraLocaleManager.Get($"TreeWallMod.Role.{IdPart}Inject.WikiDescription"),
						CrewAssets.SyringeInjectSprite),
				};
			}
		}

		public CustomRoleConfiguration Configuration => new(this)
		{
			Icon = RoleIcons.Syringe
		};

		public override void OnDeath(DeathReason reason)
		{
			RoleBehaviourStubs.OnDeath(this, reason);

			Clear();
		}

		public override void Deinitialize(PlayerControl targetPlayer)
		{
			RoleBehaviourStubs.Deinitialize(this, targetPlayer);

			Clear();
		}

		public void Clear()
		{
			if (Player.AmOwner)
			{
				var players = ModifierUtils.GetPlayersWithModifier<SyringeInjectedModifier>(x => x.SyringeItems.Any(x => x.Syringe == Player));

				foreach (var plr in players)
				{
					if (plr != null && plr.TryGetModifier<SyringeInjectedModifier>(out var injected))
					{
						string outputString = "";
						foreach (var p in injected.SyringeItems)
						{
							outputString += p.Syringe.name + " ";
						}
						Message($"{plr.name} Injected Modifier Players: {outputString}");

						plr.RpcRemovePlayerSyringeInject(Player);

						outputString = "";
						foreach (var p in injected.SyringeItems)
						{
							outputString += p.Syringe.name + " ";
						}
						Message($"{plr.name} Injected Modifier Players: {outputString}");
					}
				}
			}
		}
	}
}
