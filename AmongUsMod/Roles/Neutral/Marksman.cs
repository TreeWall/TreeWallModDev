using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Rewired.Utils.Classes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs;
using TownOfUs.Buttons;
using TownOfUs.Extensions;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game.Assailant;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TreeWallMod.Assets;
using TreeWallMod.Buttons.Neutral.Marksman;
using TreeWallMod.Events.Crewmate;
using TreeWallMod.Modifiers.Crewmate;
using TreeWallMod.Modifiers.Neutral;
using TreeWallMod.Modules;
using TreeWallMod.Options.Roles.Neutral;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TreeWallMod.Roles.Neutral
{
	public sealed class MarksmanRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, ICrewVariant
	{
		public string LocaleKey => "Marksman";
		public string RoleName => TouLocale.Get($"TreeWallMod{LocaleKey}");
		public string RoleDescription => TouLocale.GetParsed($"TreeWallMod{LocaleKey}IntroBlurb");
		public string RoleLongDescription => TouLocale.GetParsed($"TreeWallMod{LocaleKey}TabDescription");

		public RoleBehaviour CrewVariant => throw new NotImplementedException();
		public Color RoleColor => TreeWallMod.Colors.Marksman;
		public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
		public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
		public DoomableType DoomHintType => DoomableType.Insight;

		public CustomRoleConfiguration Configuration => new(this)
		{
			MaxRoleCount = 1,
            IconTmp = TmpSpriteUtils.CreateSpriteAsset(RoleIcons.Marksman.LoadAsset(), "TreeWallMod.Roles.Neutral.Marksman", 1.45f),
			Icon = RoleIcons.Marksman,
            OptionsScreenshot = TouBanners.NeutralRoleBanner,
            GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
        };

		public readonly List<MarksmanAbility> LockedAbilities   = Enum.GetValues<MarksmanAbility>().ToList();
        public readonly List<MarksmanAbility> UnlockedAbilities = new();

		private MeetingMenu meetingMenu;

		public override void Initialize(PlayerControl player)
		{
			RoleBehaviourStubs.Initialize(this, player);

			if (Player.AmOwner)
			{
				meetingMenu = new MeetingMenu(
					this,
					ClickGuess,
					MeetingAbilityType.Click,
					TouAssets.Guess,
					null!,
					IsExempt);
			}
		}

		public override void Deinitialize(PlayerControl targetPlayer)
		{
			RoleBehaviourStubs.Deinitialize(this, targetPlayer);
			TouRoleUtils.ClearTaskHeader(Player);

			if (Player.AmOwner)
			{
				meetingMenu?.Dispose();
				meetingMenu = null!;
			}

			if (!Player.HasModifier<BasicGhostModifier>())
			{
				Player.AddModifier<BasicGhostModifier>();
			}
		}

		public int IncorrectGuesses { get; set; } = 0;
		public override void OnMeetingStart()
		{
			IncorrectGuesses = 0;
			RoleBehaviourStubs.OnMeetingStart(this);

			var meeting = MeetingHud.Instance;
			if (Player.AmOwner && meeting != null)
			{
				meetingMenu.GenButtons(meeting,
					Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>());

				var firstTarget = CustomButtonSingleton<MarksmanDiscover>.Instance.FirstTarget;
                var secondTarget = CustomButtonSingleton<MarksmanDiscover>.Instance.SecondTarget;

                if (firstTarget == null && secondTarget == null)
				{
					MiscUtils.AddFakeChat(Player.Data, "Marksman Info", "No player was Marked", false, true);
				}

				if (firstTarget != null)
				{
					MiscUtils.AddFakeChat(Player.Data, "Marksman Info", GenReport(firstTarget), false, true);
				}

                if (secondTarget != null)
                {
                    MiscUtils.AddFakeChat(Player.Data, "Marksman Info", GenReport(secondTarget), false, true);
                }
            }
		}

		public override void OnVotingComplete()
		{
			RoleBehaviourStubs.OnVotingComplete(this);

			if (Player.AmOwner)
			{
				meetingMenu.HideButtons();

				CustomButtonSingleton<MarksmanDiscover>.Instance.FirstTarget = null;
                CustomButtonSingleton<MarksmanDiscover>.Instance.SecondTarget = null;
            }
		}

		private string GenReport(PlayerControl target)
		{
			string ret = "";

			MarksmanRoleHintEnum marksmanRoleHint = OptionGroupSingleton<MarksmanOptions>.Instance.RoleHint;

			Info($"Generating Marksman report");

			var reportBuilder = new StringBuilder();

			if (marksmanRoleHint == MarksmanRoleHintEnum.Subalignment)
			{
				var role = target.Data.Role;
				var unGuessableType = role as IUnguessable;

				if (target.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole) is ICachedRole cachedMod)
				{
					role = cachedMod.CachedRole;
				}

				if (unGuessableType != null)
				{
					role = unGuessableType.AppearAs;
				}

				string alignment = role.GetRoleAlignment() switch
				{
					RoleAlignment.NeutralEvil or RoleAlignment.NeutralKilling or RoleAlignment.NeutralOutlier => "Evil",
					_ => "Crew"
				};

				if (role.IsImpostor()) alignment = "Evil";

				reportBuilder.AppendLine($"{target.name} is {alignment}");
			}
			else if (marksmanRoleHint == MarksmanRoleHintEnum.ListRoles)
			{
				var role = target.Data.Role;
				var unGuessableType = role as IUnguessable;

				if (target.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole) is ICachedRole cachedMod)
				{
					role = cachedMod.CachedRole;
				}

				if (unGuessableType != null)
				{
					role = unGuessableType.AppearAs;
				}

				reportBuilder.AppendLine($"{target.name} can be:");

				var roles = MiscUtils.GetPotentialRoles().Where(x => (x is not IUnguessable) && !x.IsDead && CustomRoleUtils.CanSpawnOnCurrentMode(x) && x != role).ToList();

                var allRoles = MiscUtils.AllRoles.Where(x => (x is not IUnguessable) && !x.IsDead && CustomRoleUtils.CanSpawnOnCurrentMode(x)).Where(x => x is IGuessable && !roles.Contains(x)).ToList();
                if (allRoles.Count > 0)
                {
                    foreach (var addedRole in allRoles)
                    {
                        if (addedRole is IGuessable guessable && guessable.CanBeGuessed)
                        {
                            roles.Add(addedRole);
                        }
                    }
                }
                roles = roles.OrderBy(x => x.GetRoleName()).ToList();

                int chosenIndex = UnityEngine.Random.RandomRangeInt(
					0, (roles.Count() > OptionGroupSingleton<MarksmanOptions>.Instance.RoleAmount) ? OptionGroupSingleton<MarksmanOptions>.Instance.RoleAmount : roles.Count());

				for (int i = 0; i < OptionGroupSingleton<MarksmanOptions>.Instance.RoleAmount; i++)
				{
					if (i == chosenIndex)
					{
                        reportBuilder.AppendLine(TownOfUsPlugin.Culture,
                                $"{MiscUtils.GetHyperlinkText(role)}");
						continue;
                    }

					int index = UnityEngine.Random.RandomRangeInt(0, roles.Count());
                    reportBuilder.AppendLine(TownOfUsPlugin.Culture,
								$"{     MiscUtils.GetHyperlinkText(roles[index])}");
                    roles.RemoveAt(index);

					if (roles.Count() == 0)
					{
						break;
					}
				}
			}
			else
			{
				var role = target.Data.Role;
				var doomableRole = role as IDoomable;
				var undoomableRole = role as IUnguessable;
				var hintType = DoomableType.Default;

				if (target.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole) is ICachedRole cachedMod)
				{
					role = cachedMod.CachedRole;
					doomableRole = role as IDoomable;
				}

				if (undoomableRole != null)
				{
					role = undoomableRole.AppearAs;
					doomableRole = role as IDoomable;
				}

				if (doomableRole != null)
				{
					hintType = doomableRole.DoomHintType;
				}

				var fallback = TouLocale.GetParsed("TouRoleDoomsayerRoleHintDefault");
				var hint = TouLocale.GetParsed($"TouRoleDoomsayerRoleHint{hintType}");

				if (hint.Contains("STRMISS"))
				{
					reportBuilder.AppendLine(TownOfUsPlugin.Culture,
						$"{fallback.Replace("<player>", target.name)}\n");
				}
				else
				{
					reportBuilder.AppendLine(TownOfUsPlugin.Culture, $"{hint.Replace("<player>", target.name)}\n");
				}

				var roles = MiscUtils.GetPotentialRoles().Where(x => (x is IDoomable doomRole && doomRole.DoomHintType == DoomableType.Default &&
					x is not IUnguessable || x is not IDoomable) && !x.IsDead && CustomRoleUtils.CanSpawnOnCurrentMode(x)).ToList();

				var allRoles = MiscUtils.AllRoles.Where(x => (x is IDoomable doomRole && doomRole.DoomHintType == DoomableType.Default &&
					x is not IUnguessable || x is not IDoomable) && !x.IsDead && CustomRoleUtils.CanSpawnOnCurrentMode(x)).Where(x => x is IGuessable && !roles.Contains(x)).ToList();

				if (allRoles.Count > 0)
				{
					foreach (var addedRole in allRoles)
					{
						if (addedRole is IGuessable guessable && guessable.CanBeGuessed)
						{
							roles.Add(addedRole);
						}
					}
				}
				roles = roles.OrderBy(x => x.GetRoleName()).ToList();
				var lastRole = roles[^1];

				if (hintType != DoomableType.Default)
				{
					roles = MiscUtils.GetPotentialRoles().Where(x => x is IDoomable doomRole && doomRole.DoomHintType == hintType &&
						x is not IUnguessable && !x.IsDead && CustomRoleUtils.CanSpawnOnCurrentMode(x)).ToList();

					allRoles = MiscUtils.AllRoles.Where(x => x is IDoomable doomRole && doomRole.DoomHintType == hintType &&
						x is not IUnguessable && !x.IsDead && CustomRoleUtils.CanSpawnOnCurrentMode(x)).Where(x => x is IGuessable && !roles.Contains(x)).ToList();
					if (allRoles.Count > 0)
					{
						foreach (var addedRole in allRoles)
						{
							if (addedRole is IGuessable guessable && guessable.CanBeGuessed)
							{
								roles.Add(addedRole);
							}
						}
					}
					roles = roles.OrderBy(x => x.GetRoleName()).ToList();
					lastRole = roles[^1];
				}

				if (roles.Count != 0)
				{
					reportBuilder.Append(TownOfUsPlugin.Culture, $"(");
					foreach (var role2 in roles)
					{
						if (role2 == lastRole)
						{
							reportBuilder.Append(TownOfUsPlugin.Culture,
								$"{MiscUtils.GetHyperlinkText(lastRole)})");
						}
						else
						{
							reportBuilder.Append(TownOfUsPlugin.Culture,
								$"{MiscUtils.GetHyperlinkText(role2)}, ");
						}
					}
				}
			}
			ret = reportBuilder.ToString();

			return ret;
		}

		public bool IsExempt(PlayerVoteArea voteArea)
		{
			return voteArea.TargetPlayerId == Player.PlayerId ||
				   Player.Data.IsDead || voteArea.AmDead ||
				   voteArea.GetPlayer()?.HasModifier<JailedModifier>() == true ||
				   (voteArea.GetPlayer()?.Data.Role is MayorRole mayor && mayor.Revealed) ||
				   (Player.IsLover() && voteArea.GetPlayer()?.IsLover() == true);
		}

		private static bool IsRoleValid(RoleBehaviour role)
		{
			var unguessableRole = role as IUnguessable;
			if (role.IsDead || role is IGhostRole || (unguessableRole != null && !unguessableRole.IsGuessable))
			{
				return false;
			}

			return true;
		}

		public void ClickGuess(PlayerVoteArea voteArea, MeetingHud meetingHud)
		{
			if (meetingHud.state == MeetingHud.VoteStates.Discussion)
			{
				return;
			}

			if (Minigame.Instance)
			{
				return;
			}

			var player = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;

			var shapeMenu = GuesserMenu.Create();
			shapeMenu.Begin(IsRoleValid, ClickRoleHandle);

			void ClickRoleHandle(RoleBehaviour role)
			{
				var realRole = player.Data.Role;


				var pickVictim = role.Role == realRole.Role;
				if (player.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole) is ICachedRole cachedMod)
				{
					pickVictim = cachedMod.GuessMode switch
					{
						// Checks for the role the player is at the moment
						CacheRoleGuess.ActiveRole => role.Role == realRole.Role,
						// Checks for the cached role itself (like Imitator or Traitor)
						CacheRoleGuess.CachedRole => role.Role == cachedMod.CachedRole.Role,
						// Checks if it's the cached or active role
						_ => role.Role == cachedMod.CachedRole.Role || role.Role == realRole.Role,
					};
				}
				var victim = pickVictim ? player : Player;

				ClickHandler(victim, voteArea.TargetPlayerId);
			}

			void ClickHandler(PlayerControl victim, byte targetId)
			{
				var opts = OptionGroupSingleton<MarksmanOptions>.Instance;

				var playersAlive = PlayerControl.AllPlayerControls.ToArray()
				.Count(x => !x.HasDied() && !x.IsJailed() && x != Player);

				if (victim == Player)
				{
					IncorrectGuesses++;

					if (IncorrectGuesses - (opts.MisguessAvailable ? 1 : 0) > 0)
					{
						var notif1 = Helpers.CreateAndShowNotification(
						"That was an incorrect Guess my boi NOW DIE",
						Color.red, new Vector3(0f, 1f, -20f), spr: RoleIcons.Marksman.LoadAsset());

						notif1.AdjustNotification();

						Player.RpcMeetingMurder(victim, MeetingAnimation.PlayerNameplateAnimation, CustomTouMurderRpcs.GetRandomMeetingAnim(DeathAnimType.Nameplate),
							causeOfDeath: "MarksmanMisguess");

                        shapeMenu.Close();
                        return;
                    }

					var notif2 = Helpers.CreateAndShowNotification(
						"That was an incorrect Guess my boi",
						Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Marksman.LoadAsset());

					notif2.AdjustNotification();
				}
				else
				{
					if (victim != Player && victim.TryGetModifier<OracleBlessedModifier>(out var oracleMod))
					{
						OracleRole.RpcOracleBlessNotify(PlayerControl.LocalPlayer, oracleMod.Oracle, victim);

						MeetingMenu.Instances.Do(x => x.HideSingle(victim.PlayerId));

						meetingMenu?.HideSingle(targetId);

						shapeMenu.Close();
						return;
					}

					if (UnlockedAbilities.Contains(MarksmanAbility.Supressor) || true)
					{
                        victim.RpcAddModifier<MarksmanSuppressedModifier>(Player);
                    }
					else
					{
						Player.RpcMeetingMurder(victim, MeetingAnimation.PlayerNameplateAnimation, CustomTouMurderRpcs.GetRandomMeetingAnim(DeathAnimType.Nameplate),
							causeOfDeath: "Marksman");
					}
					AddAbility(victim);

                    meetingMenu?.HideSingle(targetId);
                }

				shapeMenu.Close();
			}
		}

		public void AddAbility(PlayerControl target)
		{
			//int index = UnityEngine.Random.RandomRangeInt(0, LockedAbilities.Count());
			int index = 5;
			var opts = OptionGroupSingleton<MarksmanOptions>.Instance;

			if (index > LockedAbilities.Count())
			{
				return;
			}

            switch (LockedAbilities[index])
			{
				case MarksmanAbility.SharpenedBlade:
				{
                    var notif1 = Helpers.CreateAndShowNotification(
                        $"Sharpened Blade was Unlocked! Your Kill Cd has been decreased by {opts.SharpenedBladeKillCdReduction} seconds.",
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Marksman.LoadAsset());

                    notif1.AdjustNotification();

                    break;
				}

                case MarksmanAbility.Supressor:
				{
                    var notif1 = Helpers.CreateAndShowNotification(
                        $"Supressor was Unlocked! You can now guess people without anyone else knowing.",
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Marksman.LoadAsset());

                    notif1.AdjustNotification();

                    break;
                }

                case MarksmanAbility.Dualscover:
				{
                    var notif1 = Helpers.CreateAndShowNotification(
                        $"Dualscover was Unlocked! You can now use Discover twice.",
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Marksman.LoadAsset());

                    notif1.AdjustNotification();

					break;
                }
            }

			UnlockedAbilities.Add(LockedAbilities[index]);
			LockedAbilities.RemoveAt(index);
        }

        public bool IsModifierApplicable(BaseModifier modifier)
        {
            return modifier is not AssassinModifier;
        }
	}

	public enum MarksmanAbility
	{
		SharpenedBlade,
		SmokeBomb,
		Warp,
		Vanish,
		Dismantle,
		Supressor,
		Dualscover
	}
}
