using TreeWallMod.Buttons.Crewmate;
using TreeWallMod.Modules;
using TreeWallMod.Options.Roles.Crewmate;
using TreeWallMod.Roles.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Utilities;

namespace TreeWallMod.Modifiers.Crewmate
{
	public sealed class RunnerSpeedModifier() : BaseModifier
	{
		public override string ModifierName => "RunnerSpeedModifer";
		public override bool HideOnUi => true;
		public override bool Unique => true;

		public float speedMultiplier = 1.0f;
		public bool active = false;

		public override void FixedUpdate()
		{
			if (!Player)
			{
                ModifierComponent?.RemoveModifier(this);
				return;
            }

			base.FixedUpdate();
		}
	}
}
