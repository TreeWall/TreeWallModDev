using TreeWallMod.Options.Modifiers;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using System.Collections.Generic;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules.Wiki;
using UnityEngine;
using Il2CppSystem;
using TownOfUs.Utilities;
using TreeWallMod.Buttons.Crewmate;
using TreeWallMod.Modules;
using TreeWallMod.Options.Roles.Crewmate;
using TreeWallMod.Roles.Crewmate;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWallMod.Modifiers.GameModifers
{
	public sealed class HeadlessModifier : UniversalGameModifier, IWikiDiscoverable
    {
		public override string ModifierName => "Headless";
		public override bool ShowInFreeplay => true;
		public override Color FreeplayFileColor => Colors.HeadlessModifier;

		public bool headlessState = false;
		public PlayerControl killer;
		public bool Die = false;

		public List<ActionButton> DisabledButtons = new List<ActionButton>();

		public override string GetDescription()
		{
			return "Turn into a headless torso when you die";
		}

		public string GetAdvancedDescription()
		{
			return "Turn into a headless torso when you die, this lasts untill the next round";
		}

		public override int GetAmountPerGame()
		{
			return (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.HeadlessAmount;
		}

		public override int GetAssignmentChance()
		{
			return (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.HeadlessChance;
		}

		public override void OnActivate()
		{
			killer = Player;
			base.OnActivate();
			//Message(ReInput.mapping.GetActionCategory("Default").id);

			//ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(false, ControllerType.Keyboard, 0);

			//if (HudManager.Instance != null && headlessState)
			//{
			//	var buttonsParent = HudManager.Instance.transform.Find("Buttons");

			//	if (buttonsParent != null)
			//	{
			//		var allButtons = buttonsParent.GetComponentsInChildren<ActionButton>(true);
			//		foreach (var button in allButtons)
			//		{
			//			if (button != HudManager.Instance.PetButton && button != HudManager.Instance.UseButton && button != HudManager.Instance.ReportButton)
			//			{
			//				DisabledButtons.Add(button);
			//			}
			//		}
			//	}
			//}
		}

		public override void Update()
		{
			base.Update();
			//ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(false, ControllerType.Keyboard);

			//hide buttons
			//if (HudManager.Instance != null && headlessState)
			//{
			//	var buttonsParent = HudManager.Instance.transform.Find("Buttons");

			//	if (buttonsParent != null)
			//	{
			//		var allButtons = buttonsParent.GetComponentsInChildren<ActionButton>(true);
			//		foreach (var button in allButtons)
			//		{
			//			if (button != HudManager.Instance.PetButton && button != HudManager.Instance.UseButton && button != HudManager.Instance.ReportButton)
			//			{
			//				button.gameObject.SetActive(false);
			//				button.SetDisabled();
			//			}
			//		}
			//	}
			//}
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			//foreach (var button in DisabledButtons)
			//{
			//	button.gameObject.SetActive(true);
			//	button.SetEnabled();
			//	Message($"Name: {button.name} Tag: {button.tag} Object Class: {button.ObjectClass}");
			//}
		}
	}
}