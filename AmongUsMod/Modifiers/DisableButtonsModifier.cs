using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Modifiers;

namespace TreeWallMod.Modifiers
{
	public sealed class DisableButtonsModifier : DisabledModifier
	{
		public override string ModifierName => "Disable Buttons and Abilities";
		public override bool HideOnUi => true;
		public override bool AutoStart => false;

		public override bool CanBeInteractedWith => false;
		public override bool CanReport => false;
		public override bool IsConsideredAlive => false;
		public override bool CanUseAbilities => false;
        public override bool CanUseConsoles => true;
        public override bool CanOpenMap => true;
	}
}
