using TreeWallMod.Roles.Crewmate;
using MiraAPI.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Utilities;
using UnityEngine;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace TreeWallMod.Modifiers.Crewmate
{
	internal class PsychicStorageModifier : BaseModifier
	{
		public override string ModifierName => "PsychicStorageModifer";
		public override bool HideOnUi => true;
		public override bool ShowInFreeplay => false;

		private Dictionary<PlayerControl, float> killers = new Dictionary<PlayerControl, float>();
		private float time = 0.0f;
		private float totalDeltaTime = 0.0f;

		public void AddKiller(PlayerControl killer)
		{
			if (!killers.ContainsKey(killer)) killers.Add(killer, time);
		}

		public bool HasKiller(PlayerControl killer)
		{
			return killers.ContainsKey(killer);
		}

		public override void OnActivate()
		{
			base.OnActivate();

			time = 0.0f;
			totalDeltaTime = 0.0f;
		}

		public override void FixedUpdate()
		{
            base.FixedUpdate();

            if (!Player)
			{
				ModifierComponent?.RemoveModifier(this);
				return;
			}

			totalDeltaTime += Time.deltaTime;

			if (totalDeltaTime > 0.1)
			{
				time += totalDeltaTime;
				totalDeltaTime = 0.0f;

				var expiredKeys = killers.Keys.Where(key => killers[key] < (time - 15f)).ToList();

				foreach (var key in expiredKeys)
				{
					killers.Remove(key);
					Message($"Removed {key.name}");
				}
			}
		}
	}
}
