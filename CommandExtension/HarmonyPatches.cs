using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using PSS;
using QFSW.QC;
using QFSW.QC.Utilities;
using UnityEngine;
using Wish;

namespace CommandExtension
{
	public static class HarmonyPatches
	{
#pragma warning disable IDE0060 // Remove unused parameter
		// Pre-patch Player.SendChatMessage(string, string) method.
		// Routes command messages into the command handler and suppresses normal chat when appropriate.
		[HarmonyPatch(typeof(Player), nameof(Player.SendChatMessage), [typeof(string), typeof(string)])]
		public static class Patch_PlayerSendChatMessage
		{
			/// <summary>
			/// Prefix called before Player.SendChatMessage executes.
			/// Determines whether the message is a chat command and, if so, prevents it 
			/// from being sent as regular chat.
			/// </summary>
			/// <returns>
			/// False to cancel the original SendChatMessage (command handled);
			/// True to allow normal chat processing.
			/// </returns>
			/// <param name="characterName">Name of the player invoking SendChatMessage.</param>
			/// <param name="message">The chat text or command to evaluate.</param>
			public static bool Prefix(string characterName, string message)
			{
				// Only intercept commands from the tracked player.
				// If CheckIfCommandSendChatMessage identifies this input as a command,
				// suppress the normal chat send.
				if (characterName == CommandMethodes.PlayerNameForCommands
					&& CommandMethodes.IsCommand(message, true)) // && characterName != playerNameForCommandsFirst
				{
					// Suppress the chat send
					return false;
				}

				// Regular chat from the active player—allow it to proceed
				return true;
			}
		}

		// Pre-patch Player.DisplayChatBubble method.
		// Suppresses the chat bubble when the text is recognized as a command.
		[HarmonyPatch(typeof(Player), nameof(Player.DisplayChatBubble))]
		public static class Patch_PlayerDisplayChatBubble
		{
			/// <summary>
			/// Prefix called before Player.DisplayChatBubble executes.
			/// Suppresses the chat bubble when the text is recognized as a command.
			/// </summary>
			/// <returns>
			/// Returns false (skips original) when the message is a command, suppressing the chat bubble.
			/// Returns true to allow normal processing for regular chat.
			/// </returns>
			/// <param name="text">The chat text about to be displayed.</param>
			public static bool Prefix(ref string text)
			{
				if (CommandMethodes.IsCommand(text, false))
				{
					// This input is a command - Prevent the chat bubble from showing.
					return false;
				}

				// Normal chat—allow bubble display.
				return true;
			}
		}

		// Pre-patch to hook into Player.Initialize for welcome messages, 
		// in-game console setup and debug helper activation.
		[HarmonyPatch(typeof(Player), nameof(Player.Initialize))]
		public static class Patch_PlayerInitialize
		{
			/// <summary>
			/// Runs immediately after Player.Initialize completes.
			/// Sends a greeting once per spawn, enables debug helpers if requested,
			/// and sets up the quantum console for commands.
			/// </summary>
			/// <param name="__instance">The Player instance that was initialized.</param>
			public static void Postfix(Player __instance)
			{
				// On the third initialize call, display a welcome message.
				if (CommandMethodes.PlayerInitializationCount < CommandExtension.PLAYER_INITIALIZATION_COUNT_WANTED)
				{
					CommandMethodes.PlayerInitializationCount++;
				}
				else if (CommandMethodes.PlayerInitializationCount == CommandExtension.PLAYER_INITIALIZATION_COUNT_WANTED)
				{
					CommandMethodes.PlayerInitializationCount++;

					// Enable in-game command feature 
					if (Player.Instance != null && QuantumConsole.Instance)
					{
						// Temporarily enable cheats for console initialization.
						QuantumConsole.Instance.GenerateCommands = Settings.EnableCheats = true;
						QuantumConsole.Instance.Initialize();

						// Revert the cheat-enable flag after setup.
						Settings.EnableCheats = false;
					}
					
					if (CommandExtension.DEBUG)
					{
						CommandExtension.GreetDebug();
					}
					else
					{
						CommandExtension.GreetNormal();
					}
				}
			}
		}

		// Post-patch GameSave.LoadCharacter.
		// Captures the loaded character name.
		[HarmonyPatch(typeof(GameSave), nameof(GameSave.LoadCharacter))]
		public static class Patch_GameSaveLoadCharacter
		{
			/// <summary>
			/// Called after loading a character save slot.
			/// Stores the characterName for use in chat‐command processing.
			/// </summary>
			/// <param name="characterNumber">The index of the character that was loaded.</param>
			/// <param name="__instance">The GameSave instance containing the loaded data.</param>
			public static void Postfix(int characterNumber, GameSave __instance)
			{
				CommandMethodes.PlayerNameForCommands = CommandMethodes.PlayerNameForCommandsFirst = __instance.CurrentSave.characterData.characterName;
			}
		}

		// Pre-patch to override the buggy Year calculation.
		// Applies a custom formula: (DayCycle.Day - 1) / 112 + 1.
		/*
		[HarmonyPatch(typeof(DayCycle))]
		[HarmonyPatch("Year", MethodType.Getter)]
		class Patch_DayCycleYear
		{
			public bool Prefix(ref int __result)
			{
				__result = (DayCycle.Day - 1) / 112 + 1;
				return !CommandMethodes.yearFix;
			}
		}
		*/

		// Pre-patch Player.AirSkipsUsed setter.
		// When infinite air skips are enabled, this prefix prevents the game from reducing the count.
		[HarmonyPatch(typeof(Player), nameof(Player.AirSkipsUsed), MethodType.Setter)]
		public static class Patch_PlayerAirSkipsUsed
		{
			/// <summary>
			/// Called before the game’s AirSkipsUsed setter runs.
			/// When infinite air skips are enabled, this prefix prevents the game from reducing the count.
			/// </summary>
			/// <returns>
			/// False to cancel the original setter when infinite air skips are active;
			/// True to allow normal behavior otherwise.
			/// </returns>
			/// <param name="value">
			/// The new air‐skip count the game is trying to apply.
			/// </param>
			public static bool Prefix(int value)
			{
				return !Commands.IsCommandActive(Commands.CmdKeyDashInfinite);
			}
		}

		// Post-patch to modify Rigidbody behavior each frame when “jump-over” mode is active.
		// While jumpOver is true and noclip is disabled, the player ignores collision when in the air
		[HarmonyPatch(typeof(Player), "Update")]
		public static class Patch_PlayerUpdate
		{
			/// <summary>
			/// Postfix runs immediately after Player.Update().
			/// Adjusts the player’s Rigidbody2D.bodyType based on grounded state
			/// when jump-over mode is enabled.
			/// </summary>
			/// <param name="__instance">The Player instance being updated.</param>
			public static void Postfix(ref Player __instance)
			{
				// Apply only when jump-over is turned on and noclip is off
				if (Commands.IsCommandActive(Commands.CmdKeyJumper) && !Commands.IsCommandActive(Commands.CmdKeyNoclip)) //ignore the wrong turned boolean!
				{
					if (__instance.Grounded)
					{
						// On ground: enable standard physics so the player can move normally
						__instance.rigidbody.bodyType = RigidbodyType2D.Dynamic;
					}
					else
					{
						// In air: switch to kinematic to “jump over” obstacles without collision
						__instance.rigidbody.bodyType = RigidbodyType2D.Kinematic;
					}
				}
			}
		}

		// Pre-patch Player.UseMana(float) method.
		// Prevents mana from being consumed when infinite-mana mode is enabled.
		[HarmonyPatch(typeof(Player), nameof(Player.UseMana), [typeof(float)])]
		public static class Patch_PlayerUseMana
		{
			/// <summary>
			/// Prefix runs before Player.UseMana is called.
			/// Returns false to cancel the original method when infinite mana is active,
			/// so the player's mana remains unchanged.
			/// </summary>
			public static bool Prefix(float mana)
			{
				// When infMana is true, skip the original UseMana call
				return !Commands.IsCommandActive(Commands.CmdKeyManaInfinite);
			}
		}

		// Pre-patch to override the game’s day‐night cycle speed.
		// Applies pause or custom speed when the corresponding commands are active.
		[HarmonyPatch(typeof(Settings), nameof(Settings.DaySpeedMultiplier), MethodType.Getter)]
		public static class Patch_DayCycleDaySpeedMultiplier
		{
			/// <summary>
			/// Prefix called before Settings.DaySpeedMultiplier returns its value.
			/// - If the pause command is active, sets the multiplier to 0 (freezing time).
			/// - If the custom‐speed command is active, uses the configured timeMultiplier.
			/// - Otherwise, lets the original getter run and return the default speed.
			/// </summary>
			public static bool Prefix(ref float __result)
			{
				if (Commands.IsCommandActive(Commands.CmdKeyPause))
				{
					__result = 0f;      // freeze time
					return false;       // skip original getter
				}

				if (Commands.IsCommandActive(Commands.CmdKeyTimespeed))
				{
					__result = CommandMethodes.TimeMultiplier;      // apply custom speed multiplier time  
					return false;                   // skip original getter
				}

				// No command override, proceed with the game’s default multiplier
				return true;

				//if (Commands[Array.FindIndex(Commands, command => command.Name == ExtensionCommands.CmdPause)].State == ExtensionCommands.CommandState.Activated)
				//    __result = 0f;
				//else if (Commands[Array.FindIndex(Commands, command => command.Name == ExtensionCommands.CmdCustomDaySpeed)].State == ExtensionCommands.CommandState.Activated)
				//    __result = timeMultiplier;
				//else
				//    return true;  // vanilla mulitplier
				//return false;  // custom mulitplier
			}
		}

		// Pre-patch to inject item IDs into tooltips and optionally print them on hover.
		// Applies to GetToolTip(Tooltip, int, bool) across multiple item types.
		[HarmonyPatch]
		public static class Patch_ItemGetToolTip
		{
			/// <summary>
			/// Targets GetToolTip methods on all relevant item classes.
			/// </summary>
			public static IEnumerable<MethodBase> TargetMethods()
			{
				Type[] itemTypes = [
					typeof(NormalItem), typeof(ArmorItem), typeof(FoodItem),
					typeof(FishItem), typeof(CropItem), typeof(WateringCanItem),
					typeof(AnimalItem), typeof(PetItem), typeof(ToolItem)];

				foreach (Type itemType in itemTypes)
				{
					yield return AccessTools.Method(itemType, "GetToolTip", [typeof(Tooltip), typeof(int), typeof(bool)]);
				}
			}

			/// <summary>
			/// Prefix runs before each GetToolTip call.
			/// - If printOnHover is enabled, sends the item ID and name to chat.
			/// - Prepends or removes a colored ID label in the in-game tooltip description
			///   based on appendItemDescWithId.
			/// </summary>
			/// <param name="__instance">The item instance whose tooltip is being generated.</param>
			public static void Prefix(Item __instance)
			{
				// Fetch the item ID and its data recordDatabase.GetData(itemId, delegate (ItemData data) 
				Database.GetData(__instance.ID(), delegate (ItemData data)
				{
					// Print the ID and name in chat when hovering, if enabled
					if (Commands.IsCommandActive(Commands.CmdKeyShowItemInfoOnHover))
					{
						CommandMethodes.MessageToChat($"{data.id} : {data.name}");
					}

					string text = "ID: ".ColorText(Color.magenta) + data.id.ToString().ColorText(Color.magenta) + "\"\n\"";
					if (Commands.IsCommandActive(Commands.CmdKeyShowItemInfoOnTooltip))
					{
						if (!data.description.Contains(text))
						{
							data.description = text + data.description;
						}
					}
					else if (data.description.Contains(text))
					{
						data.description = data.description.Replace(text, "");
					}

				});
			}
		}

		// Pre-patch to display the raw item ID as the formatted description when in debug mode.
		/*
		[HarmonyPatch(typeof(ItemData))]
		[HarmonyPatch("FormattedDescription", MethodType.Getter)]
		class Patch_ItemDataFormattedDescription
		{
			/// <summary>
			/// Postfix runs after ItemData.FormattedDescription getter.
			/// When debug is enabled, replaces the description with the item’s ID in magenta.
			/// </summary>
			/// <param name="__result">
			/// The string result returned by the original getter.
			/// </param>
			/// <param name="__instance">
			/// The ItemData instance whose description is being fetched.
			/// </param>
			static void Postfix(ref string __result, ItemData __instance)
			{
				if (CommandExtension.debug)
				{
					__result = __instance.id.ToString().ColorText(Color.magenta) + "\"\n\"";
				}
			}
		}
		*/

		// Post-patch to auto-fill museum bundles when the appropriate fill commands are active.
		[HarmonyPatch(typeof(HungryMonster), nameof(HungryMonster.SetMeta))]
		public static class Patch_HungryMonsterSetMeta
		{
			/// <summary>
			/// Postfix runs after HungryMonster.SetMeta executes.
			/// If this bundle is a museum bundle and either the cheat-fill or auto-fill command is activated,
			/// it will top up the bundle slots, either directly (cheat mode) or by transferring items
			/// from the player’s inventory (auto-fill mode), then refresh all museum visuals.
			/// </summary>
			/// <param name="__instance">The HungryMonster instance being configured.</param>
			public static void Postfix(HungryMonster __instance)
			{
				// Only proceed for actual museum bundles
				if (__instance.bundleType != BundleType.MuseumBundle)
				{
					return;
				}

				// Determine if either fill command is active
				bool cheatFill = Commands.IsCommandActive(Commands.CmdKeyCheatFillMuseum);
				bool autoFill = Commands.IsCommandActive(Commands.CmdKeyAutoFillMuseum);
				if (!cheatFill && !autoFill)
				{
					return;
				}

				// Ensure a valid player context for auto-fill
				Player player = Player.Instance;
				if (player.Inventory == null || player.Inventory.Items == null)
				{
					return;
				}

				Inventory playerInventory = player.Inventory;


				HungryMonster monster = __instance;
				if (monster.sellingInventory == null || monster.sellingInventory.Items == null)
				{
					return;
				}

				Inventory monsterInventory = monster.sellingInventory;

				// Cheat-fill: force all slots to max regardless of player inventory
				if (cheatFill)
				{
					foreach (SlotItemData slot in monsterInventory.Items)
					{
						if (slot.slot.numberOfItemToAccept == 0 || slot.amount >= slot.slot.numberOfItemToAccept)
						{
							continue;
						}

						bool isMoneyBundle = monster.name.ToLower().Contains("money");
						int needed = slot.slot.numberOfItemToAccept - slot.amount;

						if (isMoneyBundle && slot.slot.itemToAccept.id >= 60000 && slot.slot.itemToAccept.id <= 60002)
						{
							monsterInventory.AddItem(item: slot.slot.itemToAccept.id,
								amount: needed,
								slot: slot.slotNumber,
								sendNotification: false,
								specialItem: false);
						}
						else if (!isMoneyBundle)
						{
							monsterInventory.AddItem(slot.slot.serializedItemToAccept.id, needed, slot.slotNumber, false);
						}

						monster.SaveMeta();
						monster.SendNewMeta(monster.meta);
						monster.UpdateFullness(true);
					}

				}
				// Auto-fill: move matching items from the player’s inventory
				else
				{
					foreach (SlotItemData moonsterItem in monsterInventory.Items)
					{
						if (moonsterItem.slot.numberOfItemToAccept == 0 || moonsterItem.amount >= moonsterItem.slot.numberOfItemToAccept)
						{
							continue;
						}


						if (monster.name.ToLower().Contains("money"))
						{
							continue;
						}

						foreach (SlotItemData playerItem in playerInventory.Items)
						{
							if (playerItem.id != moonsterItem.slot.serializedItemToAccept.id)
							{
								continue;
							}

							int amount = Math.Min(playerItem.amount, moonsterItem.slot.numberOfItemToAccept - moonsterItem.amount);

							monsterInventory.AddItem(item: playerItem.id,
								amount: amount,
								slot: moonsterItem.slotNumber,
								sendNotification: false);

							string itemName = "unkown";
							if (ItemInfoDatabase.Instance.allItemSellInfos.TryGetValue(playerItem.id, out ItemSellInfo itemSellInfo))
							{
								itemName = itemSellInfo.name;
							}

							CommandMethodes.MessageToChat($"added: {amount.ToString().ColorText(Color.white)} * "
								+ $"{itemName.ColorText(Color.green)}"
								+ " to the museum!");
							_ = playerInventory.RemoveItem(playerItem.item, amount);

							monster.SaveMeta();
							monster.SendNewMeta(monster.meta);
							monster.UpdateFullness(true);
						}
					}
				}

				Array.ForEach(UnityEngine.Object.FindObjectsOfType<MuseumBundleVisual>(), vPodium => typeof(MuseumBundleVisual).GetMethod("OnSaveInventory", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(vPodium, null));
			}
		}
#pragma warning restore IDE0060 // Remove unused parameter
	}
}
