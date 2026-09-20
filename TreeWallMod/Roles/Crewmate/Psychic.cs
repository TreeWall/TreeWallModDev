using MiraAPI.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Roles;
using TreeWallMod.Assets;
using UnityEngine;

namespace TreeWallMod.Roles.Crewmate
{
	public sealed class PsychicRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IWikiDiscoverable, IDoomable
	{
		public string IdPart => "Psychic";
		string ICustomRole.IdPrefix => "TreeWallMod.Role";

		public bool IsPowerCrew => false;
		public Color RoleColor => Colors.Psychic;
		public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
		public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;
		public DoomableType DoomHintType => DoomableType.Relentless;

		private List<PsychicKillerInfo> killers = new();

		float elapsed = 0f;
		float totalDeltaTime;

		public CustomRoleConfiguration Configuration => new(this)
		{
			Icon = RoleIcons.Psychic
		};

		public void FixedUpdate()
		{
			if (Player == null || !Player.AmOwner)
			{
				return;
			}

			totalDeltaTime += Time.deltaTime;

			if (totalDeltaTime > 0.1)
			{
				elapsed += totalDeltaTime;
				totalDeltaTime = 0.0f;

				for (int i = 0; i < killers.Count; i++)
				{
					if (killers[i].Time < elapsed - 15f)
					{
						Message($"Removed {MiscUtils.PlayerById(killers[i].KillerId).name}, current count: {killers.Count-1}");
						killers.RemoveAt(i);
					}
				}
			}
		}

		public void AddKiller(PlayerControl killer)
		{
			killers.Add(new PsychicKillerInfo(killer.PlayerId, elapsed));
		}

		public bool HasKiller(byte killerId)
		{
			return killers.Any(x => x.KillerId == killerId);
		}
	}

	public readonly struct PsychicKillerInfo(byte killerId, float time)
	{
		public byte KillerId { get; } = killerId;
		public float Time { get; } = time;
	}
}
