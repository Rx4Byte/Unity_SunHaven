using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandExtension.Models;
using HarmonyLib;
using QFSW.QC;
using QFSW.QC.Utilities;
using UnityEngine;
using Wish;

namespace CommandExtension
{
    public class Patches
    {
        // Pre-patch Player.SendChatMessage(string, string) method.
        // Routes command messages into the command handler and suppresses normal chat when appropriate.
        [HarmonyPatch(typeof(Player), nameof(Player.SendChatMessage), new[] { typeof(string), typeof(string) })]
        class Patch_PlayerSendChatMessage
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
            static bool Prefix(string characterName, string message)
            {
                // Only intercept commands from the tracked player.
                // If CheckIfCommandSendChatMessage identifies this input as a command,
                // suppress the normal chat send.
                if (characterName == CommandMethodes.playerNameForCommands
                    && IsCommand(message, true)) // && characterName != playerNameForCommandsFirst
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
        class Patch_PlayerDisplayChatBubble
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
            static bool Prefix(ref string text)
            {
                if (IsCommand(text, false))
                {
                    // This input is a command - Prevent the chat bubble from showing.
                    return false;
                }

                // Normal chat—allow bubble display.
                return true;
            }
        }

        private static int ranOnceOnPlayerSpawn = 0;
        // Pre-patch to hook into Player.Initialize for welcome messages, 
        // in-game console setup and debug helper activation.
        [HarmonyPatch(typeof(Player), nameof(Player.Initialize))]
        class Patch_PlayerInitialize
        {
            /// <summary>
            /// Runs immediately after Player.Initialize completes.
            /// Sends a greeting once per spawn, enables debug helpers if requested,
            /// and sets up the quantum console for commands.
            /// </summary>
            /// <param name="__instance">The Player instance that was initialized.</param>
            static void Postfix(Player __instance)
            {
                // Only proceed when the feedback-disabled command is deactivated,
                // meaning command feedback is allowed.
                int feedbackCmdIndex = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdFeedbackDisabled);

                if (Commands.GeneratedCommands[feedbackCmdIndex].State == CommandState.Deactivated)
                {
                    // On the third initialize call, display a welcome message.
                    if (ranOnceOnPlayerSpawn < 2)
                    {
                        ranOnceOnPlayerSpawn++;
                    }
                    else if (ranOnceOnPlayerSpawn == 2)
                    {
                        ranOnceOnPlayerSpawn++;
                        GreetPlayerInChat();

                        // In debug mode, turn on test helpers and notify the player.
                        if (CommandExtension.debug)
                        {
                            CommandMethodes.MessageToChat("debug: enable cheat commands".ColorText(Color.magenta));
                            CommandMethodes.CommandFunction_Jumper();
                            CommandMethodes.CommandFunction_InfiniteMana();
                            CommandMethodes.CommandFunction_InfiniteAirSkips();
                            CommandMethodes.CommandFunction_Pause();
                        }
                    }

                    // Enable in-game command feature 
                    if (Player.Instance != null && QuantumConsole.Instance)
                    {
                        // Temporarily enable cheat registration for console initialization.
                        QuantumConsole.Instance.GenerateCommands = Settings.EnableCheats = true;
                        QuantumConsole.Instance.Initialize();

                        // Revert the cheat-enable flag after setup.
                        Settings.EnableCheats = false;
                    }
                }
            }
        }

        // Post-patch GameSave.LoadCharacter.
        // Captures the loaded character name.
        [HarmonyPatch(typeof(GameSave), nameof(GameSave.LoadCharacter))]
        class Patch_GameSaveLoadCharacter
        {
            /// <summary>
            /// Called after loading a character save slot.
            /// Stores the characterName for use in chat‐command processing.
            /// </summary>
            /// <param name="characterNumber">The index of the character that was loaded.</param>
            /// <param name="__instance">The GameSave instance containing the loaded data.</param>
            static void Postfix(int characterNumber, GameSave __instance)
            {
                CommandMethodes.playerNameForCommands = CommandMethodes.playerNameForCommandsFirst = __instance.CurrentSave.characterData.characterName;
            }
        }

        // Pre-patch to override the buggy Year calculation.
        // Applies a custom formula: (DayCycle.Day - 1) / 112 + 1.
        [HarmonyPatch(typeof(DayCycle))]
        [HarmonyPatch("Year", MethodType.Getter)]
        class Patch_DayCycleYear
        {
            public static bool Prefix(ref int __result)
            {
                __result = (DayCycle.Day - 1) / 112 + 1;
                return !CommandMethodes.yearFix;
            }
        }

        // Pre-patch Player.AirSkipsUsed setter.
        // When infinite air skips are enabled, this prefix prevents the game from reducing the count.

        [HarmonyPatch(typeof(Player), nameof(Player.AirSkipsUsed), MethodType.Setter)]
        class Patch_PlayerAirSkipsUsed
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
            static bool Prefix(int value)
            {
                return !CommandMethodes.infAirSkips;
            }
        }

        // Post-patch to modify Rigidbody behavior each frame when “jump-over” mode is active.
        // While jumpOver is true and noclip is disabled, the player:
        //  - Uses Dynamic bodyType when grounded (standard physics)  
        //  - Switches to Kinematic in the air (ignoring collisions)
        [HarmonyPatch(typeof(Player), "Update")]
        class Patch_PlayerUpdate
        {
            /// <summary>
            /// Postfix runs immediately after Player.Update().
            /// Adjusts the player’s Rigidbody2D.bodyType based on grounded state
            /// when jump-over mode is enabled.
            /// </summary>
            /// <param name="__instance">The Player instance being updated.</param>
            static void Postfix(ref Player __instance)
            {
                // Apply only when jump-over is turned on and noclip is off
                if (CommandMethodes.jumpOver && !CommandMethodes.noclip) //ignore the wrong turned boolean!
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
        [HarmonyPatch(typeof(Player), nameof(Player.UseMana), new[] { typeof(float) })]
        class Patch_PlayerUseMana
        {
            /// <summary>
            /// Prefix runs before Player.UseMana is called.
            /// Returns false to cancel the original method when infinite mana is active,
            /// so the player's mana remains unchanged.
            /// </summary>
            static bool Prefix(float mana)
            {
                // When infMana is true, skip the original UseMana call
                return !CommandMethodes.infMana;
            }
        }

        // Pre-patch to override the game’s day‐night cycle speed.
        // Applies pause or custom speed when the corresponding commands are active.
        [HarmonyPatch(typeof(Settings))]
        [HarmonyPatch("DaySpeedMultiplier", MethodType.Getter)]
        class Patch_DayCycleDaySpeedMultiplier
        {
            /// <summary>
            /// Prefix called before Settings.DaySpeedMultiplier returns its value.
            /// - If the pause command is active, sets the multiplier to 0 (freezing time).
            /// - If the custom‐speed command is active, uses the configured timeMultiplier.
            /// - Otherwise, lets the original getter run and return the default speed.
            /// </summary>
            static bool Prefix(ref float __result)
            {
                // Check for “pause” command activation
                int pauseCmdIndex = Array.FindIndex(Commands.GeneratedCommands, cmd => cmd.Name == Commands.CmdPrefix + Commands.CmdPause);
                if (Commands.GeneratedCommands[pauseCmdIndex].State == CommandState.Activated)
                {
                    __result = 0f;      // freeze time
                    return false;       // skip original getter
                }

                // Check for “custom day speed” command activation
                int customSpeedIndex = Array.FindIndex(Commands.GeneratedCommands, cmd => cmd.Name == Commands.CmdPrefix + Commands.CmdCustomDaySpeed);
                if (Commands.GeneratedCommands[customSpeedIndex].State == CommandState.Activated)
                {
                    __result = CommandMethodes.timeMultiplier;      // apply custom speed multiplier time  
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
        //[HarmonyPatch]
        //class Patch_ItemGetToolTip
        //{
        //    /// <summary>
        //    /// Targets GetToolTip methods on all relevant item classes.
        //    /// </summary>
        //    static IEnumerable<MethodBase> TargetMethods()
        //    {
        //        Type[] itemTypes = new Type[] { typeof(NormalItem), typeof(ArmorItem), typeof(FoodItem), typeof(FishItem), typeof(CropItem), typeof(WateringCanItem), typeof(AnimalItem), typeof(PetItem), typeof(ToolItem) };
        //        foreach (Type itemType in itemTypes)
        //            yield return AccessTools.Method(itemType, "GetToolTip", new[] { typeof(Tooltip), typeof(int), typeof(bool) });
        //    }
        //
        //    /// <summary>
        //    /// Prefix runs before each GetToolTip call.
        //    /// - If printOnHover is enabled, sends the item ID and name to chat.
        //    /// - Prepends or removes a colored ID label in the in-game tooltip description
        //    ///   based on appendItemDescWithId.
        //    /// </summary>
        //    /// <param name="__instance">The item instance whose tooltip is being generated.</param>
        //    static void Prefix(Item __instance)
        //    {
        //        // Fetch the item ID and its data record
        //        int id = __instance.ID();
        //        // TODO: update
        //        ItemData itemData = ItemDatabaseWrapper.ItemDatabase.GetItemData(id);
        //
        //        // Print the ID and name in chat when hovering, if enabled
        //        if (CommandMethodes.printOnHover)
        //        {
        //            CommandMethodes.MessageToChat($"{id} : {itemData.name}");
        //        }
        //
        //        string idLabel = "ID: ".ColorText(Color.magenta) + id.ToString().ColorText(Color.magenta) + "\"\n\"";
        //
        //        if (CommandMethodes.appendItemDescWithId)
        //        {
        //            // Add the label if it’s not already present
        //            if (!itemData.description.Contains(idLabel))
        //            {
        //                itemData.description = idLabel + itemData.description;
        //            }
        //        }
        //        // Remove the label if it exists when feature is off
        //        if (itemData.description.Contains(idLabel))
        //        {
        //            itemData.description = itemData.description.Replace(idLabel, "");
        //        }
        //
        //    }
        //}

        // Pre-patch to display the raw item ID as the formatted description when in debug mode.
        //[HarmonyPatch(typeof(ItemData))]
        //[HarmonyPatch("FormattedDescription", MethodType.Getter)]
        //class Patch_ItemDataFormattedDescription
        //{
        //    /// <summary>
        //    /// Postfix runs after ItemData.FormattedDescription getter.
        //    /// When debug is enabled, replaces the description with the item’s ID in magenta.
        //    /// </summary>
        //    /// <param name="__result">
        //    /// The string result returned by the original getter.
        //    /// </param>
        //    /// <param name="__instance">
        //    /// The ItemData instance whose description is being fetched.
        //    /// </param>
        //    static void Postfix(ref string __result, ItemData __instance)
        //    {
        //        if (CommandExtension.debug)
        //        {
        //            __result = __instance.id.ToString().ColorText(Color.magenta) + "\"\n\"";
        //        }
        //    }
        //}

        // Post-patch to auto-fill museum bundles when the appropriate fill commands are active.
        //[HarmonyPatch(typeof(HungryMonster))]
        //[HarmonyPatch("SetMeta")]
        //class Patch_HungryMonsterSetMeta
        //{
        //    /// <summary>
        //    /// Postfix runs after HungryMonster.SetMeta executes.
        //    /// If this bundle is a museum bundle and either the cheat-fill or auto-fill command is activated,
        //    /// it will top up the bundle slots, either directly (cheat mode) or by transferring items
        //    /// from the player’s inventory (auto-fill mode), then refresh all museum visuals.
        //    /// </summary>
        //    /// <param name="__instance">The HungryMonster instance being configured.</param>
        //    /// <param name="decorationData">DecorationPositionData passed into SetMeta (unused).</param>
        //    static void Postfix(HungryMonster __instance, DecorationPositionData decorationData)
        //    {
        //        // Only proceed for actual museum bundles
        //        if (__instance.bundleType != BundleType.MuseumBundle)
        //        {
        //            return;
        //        }
        //
        //        // Determine if either fill command is active
        //        bool cheatFill = Commands.GeneratedCommands.Any(cmd => cmd.Name == Commands.CmdPrefix + Commands.CmdCheatFillMuseum && cmd.State == CommandState.Activated);
        //        bool autoFill = Commands.GeneratedCommands.Any(cmd => cmd.Name == Commands.CmdPrefix + Commands.CmdAutoFillMuseum && cmd.State == CommandState.Activated);
        //        if (!cheatFill && !autoFill)
        //            return;
        //
        //        // Ensure a valid player context for auto-fill
        //        Player player = CommandMethodes.GetPlayerForCommand();
        //        if (player == null)
        //            return;
        //
        //        var monster = __instance;
        //        var inventory = monster.sellingInventory;
        //        if (inventory == null)
        //            return;
        //
        //        // Cheat-fill: force all slots to max regardless of player inventory
        //        if (cheatFill)
        //        {
        //            foreach (var slot in inventory.Items)
        //            {
        //                if (slot.item == null ||
        //                    slot.slot.numberOfItemToAccept == 0 ||
        //                    slot.amount >= slot.slot.numberOfItemToAccept)
        //                    continue;
        //
        //                bool isMoneyBundle = monster.name.ToLower().Contains("money");
        //                int needed = slot.slot.numberOfItemToAccept - slot.amount;
        //
        //                if (isMoneyBundle
        //                    && slot.slot.itemToAccept.id >= 60000 && slot.slot.itemToAccept.id <= 60002)
        //                {
        //                    inventory.AddItem(item: slot.slot.itemToAccept.id,
        //                        amount: needed,
        //                        slot: slot.slotNumber,
        //                        sendNotification: false,
        //                        specialItem: false);
        //                }
        //                else if (!isMoneyBundle)
        //                {
        //                    var itemObj = ItemDatabaseWrapper.ItemDatabase.GetItemData(slot.slot.itemToAccept.id).GetItem();
        //                    inventory.AddItem(itemObj, needed, slot.slotNumber, false);
        //                }
        //
        //                monster.UpdateFullness();
        //            }
        //        }
        //        // Auto-fill: move matching items from the player’s inventory
        //        else
        //        {
        //            var playerInv = player.Inventory;
        //            if (playerInv == null)
        //                return;
        //
        //            foreach (var slot in inventory.Items)
        //            {
        //                if (slot.slot.numberOfItemToAccept == 0 || slot.amount >= slot.slot.numberOfItemToAccept)
        //                    continue;
        //
        //                if (monster.name.ToLower().Contains("money"))
        //                    continue; // skip money bundles here
        //
        //                foreach (var pItem in playerInv.Items)
        //                {
        //                    if (pItem.id != slot.slot.itemToAccept.id)
        //                        continue;
        //
        //                    int transfer = Math.Min(pItem.amount, slot.slot.numberOfItemToAccept - slot.amount
        //                    );
        //                    if (transfer <= 0)
        //                        continue;
        //
        //                    var itemObj = ItemDatabaseWrapper.ItemDatabase.GetItemData(pItem.id).GetItem();
        //                    inventory.AddItem(item: itemObj,
        //                        amount: transfer,
        //                        slot: slot.slotNumber,
        //                        sendNotification: false);
        //
        //                    CommandMethodes.CommandFunction_PrintToChat($"transferred: {transfer.ToString().ColorText(Color.white)} * "
        //                        + $"{ItemDatabaseWrapper.ItemDatabase.GetItemData(pItem.id).name.ColorText(Color.white)}"
        //                    );
        //                    playerInv.RemoveItem(pItem.item, transfer);
        //                    monster.UpdateFullness();
        //                }
        //            }
        //        }
        //
        //        // Refresh all museum bundle visuals so changes appear immediately
        //        //Array.ForEach(FindObjectsOfType<MuseumBundleVisual>(), vPodium => typeof(MuseumBundleVisual).GetMethod("OnSaveInventory", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(vPodium, null));
        //        foreach (var visual in UnityEngine.Object.FindObjectsOfType<MuseumBundleVisual>())
        //        {
        //            typeof(MuseumBundleVisual).GetMethod("OnSaveInventory", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(visual, null);
        //        }
        //
        //
        //        //if (Commands[Array.FindIndex(Commands, command => command.Name == ExtensionCommands.CmdCheatFillMuseum)].State == ExtensionCommands.CommandState.Activated
        //        //    ||
        //        //    Commands[Array.FindIndex(Commands, command => command.Name == ExtensionCommands.CmdAutoFillMuseum)].State == ExtensionCommands.CommandState.Activated)
        //        //{
        //        //    Player player = GetPlayerForCommand();
        //        //    if (player == null)
        //        //        return;
        //        //    HungryMonster monster = __instance;
        //        //    if (monster.sellingInventory != null) // && monster.sellingInventory.Items != null && monster.sellingInventory.Items.Count >= 1
        //        //    {
        //        //        if (Commands.Any(command => command.Name == ExtensionCommands.CmdCheatFillMuseum && command.State == ExtensionCommands.CommandState.Activated))
        //        //        {
        //        //            foreach (SlotItemData slotItemData in monster.sellingInventory.Items)
        //        //            {
        //        //                if (slotItemData.item == null || slotItemData.slot.numberOfItemToAccept == 0 || slotItemData.amount == slotItemData.slot.numberOfItemToAccept)
        //        //                    continue;
        //        //                if (!monster.name.ToLower().Contains("money"))
        //        //                    monster.sellingInventory.AddItem(ItemDatabaseWrapper.ItemDatabase.GetItemData(slotItemData.slot.itemToAccept.id).GetItem(), slotItemData.slot.numberOfItemToAccept - slotItemData.amount, slotItemData.slotNumber, false);
        //        //                else if (monster.name.ToLower().Contains("money"))
        //        //                {
        //        //                    if (slotItemData.slot.itemToAccept.id >= 60000 && slotItemData.slot.itemToAccept.id <= 60002)
        //        //                        monster.sellingInventory.AddItem(slotItemData.slot.itemToAccept.id, slotItemData.slot.numberOfItemToAccept - slotItemData.amount, slotItemData.slotNumber, false, false);
        //        //                    //if (slotItemData.slot.itemToAccept.id == 60000)
        //        //                    //    monster.sellingInventory.AddItem(slotItemData.slot.itemToAccept.id, slotItemData.slot.numberOfItemToAccept - slotItemData.amount, slotItemData.slotNumber, false, false);
        //        //                    //else if (slotItemData.slot.itemToAccept.id == 60001)
        //        //                    //    monster.sellingInventory.AddItem(slotItemData.slot.itemToAccept.id, slotItemData.slot.numberOfItemToAccept - slotItemData.amount, slotItemData.slotNumber, false, false);
        //        //                    //else if (slotItemData.slot.itemToAccept.id == 60002)
        //        //                    //    monster.sellingInventory.AddItem(slotItemData.slot.itemToAccept.id, slotItemData.slot.numberOfItemToAccept - slotItemData.amount, slotItemData.slotNumber, false, false);
        //        //                }
        //        //                monster.UpdateFullness();
        //        //            }
        //        //        }
        //        //        else
        //        //        {
        //        //            foreach (SlotItemData slotItemData in monster.sellingInventory.Items)
        //        //            {
        //        //                if (!monster.name.ToLower().Contains("money") && slotItemData.item != null && player.Inventory != null)
        //        //                {
        //        //                    if (slotItemData.slot.numberOfItemToAccept == 0 || slotItemData.amount == slotItemData.slot.numberOfItemToAccept)
        //        //                        continue;
        //        //                    Inventory pInventory = player.Inventory;
        //        //                    foreach (var pItem in pInventory.Items)
        //        //                    {
        //        //                        if (pItem.id == slotItemData.slot.itemToAccept.id)
        //        //                        {
        //        //                            int amount = Math.Min(pItem.amount, slotItemData.slot.numberOfItemToAccept - slotItemData.amount);
        //        //                            monster.sellingInventory.AddItem(ItemDatabaseWrapper.ItemDatabase.GetItemData(slotItemData.slot.itemToAccept.id).GetItem(), amount, slotItemData.slotNumber, false);
        //        //                            CommandFunction_PrintToChat($"transferred: {amount.ToString().ColorText(Color.white)} * {ItemDatabaseWrapper.ItemDatabase.GetItemData(pItem.id).name.ColorText(Color.white)}");
        //        //                            player.Inventory.RemoveItem(pItem.item, amount);
        //        //                            monster.UpdateFullness();
        //        //                        }
        //        //                    }
        //        //
        //        //                }
        //        //            }
        //        //        }
        //        //
        //        //    }
        //        //    Array.ForEach(FindObjectsOfType<MuseumBundleVisual>(), vPodium => typeof(MuseumBundleVisual).GetMethod("OnSaveInventory", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(vPodium, null));
        //        //}
        //    }
        //}


        // ============================================================
        // Utility
        // ============================================================

        /// <summary>
        /// Outputs a styled welcome message to chat,
        /// informing the player that the command extension is active.
        /// </summary>
        private static void GreetPlayerInChat()
        {
            CommandMethodes.MessageToChat("> Command Extension Active!".ColorText(new Color(1F, 0.66F, 0.0F)));
            CommandMethodes.MessageToChat("     type '!help' for a list of commands.".ColorText(new Color(0.7F, 0.44F, 0.0F)));
            //CommandMethodes.CommandFunction_PrintToChat("\n------------------------------------------".ColorText(Color.black));
        }

        /// <summary>
        /// Determines if the entered chat text is a command and, if so, suppresses the normal chat bubble.
        /// Utility for DisplayChatBubble pre-patch.
        /// </summary>
        /// <param name="inputText">The raw chat input to evaluate for command syntax.</param>
        /// <returns>
        /// True when the text is a recognized command (so no chat bubble should show);  
        /// false when it’s regular chat (bubble displays normally).
        /// </returns>
        private static bool IsCommand(string commandInput, bool isMessage = false)
        {
            // Normalize to lowercase for case-insensitive prefix detection
            commandInput = commandInput.ToLower();

            // Command syntax: must start with '!' but not '!!' (latter is an escape for literal '!')
            if (!(commandInput.Length >= 2 && commandInput[0] == '!' && commandInput[1] != '!'))
            {
                return false;
            }

            if (CommandMethodes.GetPlayerForCommand() == null)
            {
                // Verify there’s a valid player context before executing commands.
                // Under normal conditions this should never be null.
                // TODO: log a error and notify the player if it is null.
            }

            if (isMessage)
            {
                Commands.ProcessCommands(commandInput);
            }

            return true;
        }
    }
}
