using System.Collections.Generic;
using CommandExtension.Models;
using QFSW.QC.Utilities;
using UnityEngine;

namespace CommandExtension
{
    public static class Commands
	{
		//// Core 
		// prefix
		public const string CmdPrefix = "!";

		// Debug
		public const string CmdKeyDebug = "debug";
        public const string CmdDesDebug = "Debug Methode";
        public const string CmdUseDebug = CmdPrefix + CmdKeyDebug;

		// Help
		public const string CmdKeyHelp = "help";
        public const string CmdDesHelp = "Displays a list of all available commands.";
        public const string CmdUseHelp = CmdPrefix + CmdKeyHelp;

		// State
		public const string CmdKeyState = "state";
        public const string CmdDesState = "Shows which commands are currently activated.";
        public const string CmdUseState = CmdPrefix + CmdKeyState;

		// Fix/Clear Chat
		public const string CmdKeyClearChat = "clear";
		public const string CmdDesClearChat = "Clears chat (also fixes broken chat).";
		public const string CmdUseClearChat = CmdPrefix + CmdKeyClearChat;

		// Feedback toggle
		public const string CmdKeyFeedbackDisabled = "feedback";
        public const string CmdDesFeedbackDisabled = "Toggles chat feedback for command execution.";
        public const string CmdUseFeedbackDisabled = CmdPrefix + CmdKeyFeedbackDisabled;

		// Command target name
		public const string CmdKeyName = "name";
        public const string CmdDesName = "Sets or resets the player name that commands will target.";
        public const string CmdUseName = CmdPrefix + CmdKeyName + " [playerName]*";

		//// Mine commands
		// Minereset
		public const string CmdKeyMineReset = "minereset";
        public const string CmdDesMineReset = "Reset the current mine.";
        public const string CmdUseMineReset = CmdPrefix + CmdKeyMineReset;

		// Mineclear
		public const string CmdKeyMineClear = "mineclear";
        public const string CmdDesMineClear = "Removes all rocks and ores from the mine.";
        public const string CmdUseMineClear = CmdPrefix + CmdKeyMineClear;

		// Mineoverfill
		public const string CmdKeyMineOverfill = "mineoverfill";
        public const string CmdDesMineOverfill = "Fills the mine completely.";
        public const string CmdUseMineOverfill = CmdPrefix + CmdKeyMineOverfill;

		//// Time commands
		// Pause
		public const string CmdKeyPause = "pause";
        public const string CmdDesPause = "Toggles the game’s time pause on or off.";
        public const string CmdUsePause = CmdPrefix + CmdKeyPause;

		// Timespeed
		public const string CmdKeyTimespeed = "timespeed";
        public const string CmdDesTimespeed = "Sets or toggles minutes as game hour.";
        public const string CmdUseTimespeed = CmdPrefix + CmdKeyTimespeed + " [multiplier|reset]";

		// Time
		public const string CmdKeyDate = "time";
        public const string CmdDesDate = "Sets the current hour or day in the day/night cycle.";
        public const string CmdUseDate = CmdPrefix + CmdKeyDate + " [h|d] [value]";

		// Weather
		public const string CmdKeyWeather = "weather";
        public const string CmdDesWeather = "Set raining, heatwave, or clear.";
        public const string CmdUseWeather = CmdPrefix + CmdKeyWeather + " [raining|heatwave|clear]";

		// Season
		public const string CmdKeySeason = "season";
        public const string CmdDesSeason = "Changes the current season.";
        public const string CmdUseSeason = CmdPrefix + CmdKeySeason + " [Spring|Summer|Fall|Winter]";

		// Years
		public const string CmdKeyYear = "years";
        public const string CmdDesYear = "Increase or Decrease the Year.";
        public const string CmdUseYear = CmdPrefix + CmdKeyYear + " (-)[years]";

		//// Currency commands
		// Money
		public const string CmdKeyMoney = "money";
        public const string CmdDesMoney = "Alias for !coins.";
        public const string CmdUseMoney = CmdPrefix + CmdKeyMoney + " (-)[amount]";

		// Coins
		public const string CmdKeyCoins = "coins";
        public const string CmdDesCoins = "Adds or removes Coins.";
        public const string CmdUseCoins = CmdPrefix + CmdKeyCoins + " (-)[amount]";

		// Orbs
		public const string CmdKeyOrbs = "orbs";
        public const string CmdDesOrbs = "Adds or removes Orbs.";
        public const string CmdUseOrbs = CmdPrefix + CmdKeyOrbs + " (-)[amount]";

		// Tickets
		public const string CmdKeyTickets = "tickets";
        public const string CmdDesTickets = "Adds or removes Tickets.";
        public const string CmdUseTickets = CmdPrefix + CmdKeyTickets + " (-)[amount]";

		//// Player toggles
		// Jumper
		public const string CmdKeyJumper = "jumper";
		public const string CmdDesJumper = $"Alias for '{CmdPrefix + CmdKeyJumpOver}'.";
		public const string CmdUseJumper = CmdPrefix + CmdKeyJumper;

		// Jumpover
		public const string CmdKeyJumpOver = "jumpover";
		public const string CmdDesJumpOver = "Toggles ability to jump through objects.";
		public const string CmdUseJumpOver = CmdPrefix + CmdKeyJumpOver;

		// Dasher
		public const string CmdKeyDasher = "dasher";
		public const string CmdDesDasher = $"Alias for '!{CmdPrefix + CmdKeyDashInfinite}'.";
		public const string CmdUseDasher = CmdPrefix + CmdKeyDasher;

		// DashInf
		public const string CmdKeyDashInfinite = "dashinf";
		public const string CmdDesDashInfinite = "Toggles infinite dash charges.";
		public const string CmdUseDashInfinite = CmdPrefix + CmdKeyDashInfinite;

		// Manafill
		public const string CmdKeyManaFill = "manafill";
        public const string CmdDesManaFill = "Refills the player’s mana.";
        public const string CmdUseManaFill = CmdPrefix + CmdKeyManaFill;

		// ManaInf
		public const string CmdKeyManaInfinite = "manainf";
        public const string CmdDesManaInfinite = "Toggles infinite mana.";
        public const string CmdUseManaInfinite = CmdPrefix + CmdKeyManaInfinite;

		// Healthfill
		public const string CmdKeyHealthFill = "healthfill";
        public const string CmdDesHealthFill = "Refills the player’s health.";
        public const string CmdUseHealthFill = CmdPrefix + CmdKeyHealthFill;

		// Nohit
		public const string CmdKeyNoHit = "nohit";
        public const string CmdDesNoHit = "Toggles invincibility (no damage taken).";
        public const string CmdUseNoHit = CmdPrefix + CmdKeyNoHit;

		// Noclip
		public const string CmdKeyNoclip = "noclip";
        public const string CmdDesNoClip = "Toggles noclip (walk through walls).";
        public const string CmdUseNoClip = CmdPrefix + CmdKeyNoclip;

		// Sleep
		public const string CmdKeySleep = "sleep";
		public const string CmdDesSleep = "Sleeps through the night.";
		public const string CmdUseSleep = CmdPrefix + CmdKeySleep;

		//// Misc toggles
		// UI
		public const string CmdKeyUI = "ui";
        public const string CmdDesUI = "Toggles the game’s HUD.";
        public const string CmdUseUI = CmdPrefix + CmdKeyUI + " [on|off]*";

		// Autofillmuseum
		public const string CmdKeyAutoFillMuseum = "autofillmuseum";
        public const string CmdDesAutoFillMuseum = "Toggles auto-fill museum.";
        public const string CmdUseAutoFillMuseum = CmdPrefix + CmdKeyAutoFillMuseum;

		// Cheatfillmuseum
		public const string CmdKeyCheatFillMuseum = "cheatfillmuseum";
        public const string CmdDesCheatFillMuseum = "Toggles cheat-fill museum.";
        public const string CmdUseCheatFillMuseum = CmdPrefix + CmdKeyCheatFillMuseum;

		// Yearfix
		public const string CmdKeyYearFix = "yearfix";
        public const string CmdDesYearFix = "Toggles corrected year calculation.";
        public const string CmdUseYearFix = CmdPrefix + CmdKeyYearFix;

		// Cheats
		public const string CmdKeyCheats = "cheats";
        public const string CmdDesCheats = "Toggles the game’s built-in cheats and hotkeys.";
        public const string CmdUseCheats = CmdPrefix + CmdKeyCheats;

		//// NPC relationship
		// Relationship
		public const string CmdKeyRelationship = "relationship";
		public const string CmdDesRelationship = "Sets or adds to NPC relationship.";
		public const string CmdUseRelationship = CmdPrefix + CmdKeyRelationship + " [name|all] [value] [add]*";

		// Marry
		public const string CmdKeyMarry = "marry";
		public const string CmdDesMarry = "Marries a single or all NPCs.";
		public const string CmdUseMarry = CmdPrefix + CmdKeyMarry + " [name|all]";

		// Unmarry
		public const string CmdKeyUnmarry = "unmarry";
		public const string CmdDesUnmarry = "Alias for !divorce.";
		public const string CmdUseUnmarry = CmdPrefix + CmdKeyUnmarry + " [name|all]";

		// Divorce
		public const string CmdKeyDivorce = "divorce";
		public const string CmdDesDivorce = "Divorces a single or all NPCs.";
		public const string CmdUseDivorce = CmdPrefix + CmdKeyDivorce + " [name|all]";

		//// Items
		// Give
		public const string CmdKeyGiveItem = "give";
        public const string CmdDesGiveItem = "Gives item/s by ID or name.";
        public const string CmdUseGiveItem = CmdPrefix + CmdKeyGiveItem + " [ID|name] [amount]";

		// Items
		public const string CmdKeyShowItem = "items";
        public const string CmdDesShowItem = "Lists items matching the given name.";
        public const string CmdUseShowItem = CmdPrefix + CmdKeyShowItem + " [name]";

		// List
        public const string CmdKeyShowCategorizedItems = "list";
        public const string CmdDesShowCategorizedItems = "Show or Give Items filtered by category.";
        public const string CmdUseShowCategorizedItems = CmdPrefix + CmdKeyShowCategorizedItems + " [xp|currency|bonus|pet|decoration|armor|tool|food|crop|fish|normal]";

		// Devkit
		public const string CmdKeyDevKit = "devkit";
		public const string CmdDesDevKit = "Grants the developer kit items.";
		public const string CmdUseDevKit = CmdPrefix + CmdKeyDevKit;

		// ShowId
		public const string CmdKeyShowItemInfoOnTooltip = "showid";
        public const string CmdDesShowItemInfoOnTooltip = "Toggles showing item IDs in tooltips.";
        public const string CmdUseShowItemInfoOnTooltip = CmdPrefix + CmdKeyShowItemInfoOnTooltip;

		// ShowIdOnHover
		public const string CmdKeyShowItemInfoOnHover = "showidonhover";
		public const string CmdDesShowItemInfoOnHover = "Show item ID in Chat on hover.";
		public const string CmdUseShowItemInfoOnHover = CmdPrefix + CmdKeyShowItemInfoOnHover;

		//// Teleport
		// Tp
		public const string CmdKeyTeleport = "tp";
        public const string CmdDesTeleport = "Teleports the player to a location.";
        public const string CmdUseTeleport = CmdPrefix + CmdKeyTeleport + " [location]";

		// Tps
		public const string CmdKeyTeleportLocations = "tps";
        public const string CmdDesTeleportLocations = "Lists all available teleport locations.";
        public const string CmdUseTeleportLocations = CmdPrefix + CmdKeyTeleportLocations;

        public static Dictionary<string, Command> GeneratedCommands { get; private set; } = new() {
            // Debug
            { CmdKeyDebug,					new Command(CmdPrefix + CmdKeyDebug,					CmdDesDebug,					CmdUseDebug,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_Debug(commandInput)) },

            // Help
            { CmdKeyHelp,					new Command(CmdPrefix + CmdKeyHelp,						CmdDesHelp,						CmdUseHelp,						CommandState.None,        commandInput => CommandMethodes.CommandFunction_Help(commandInput)) },

            // State
            { CmdKeyState,					new Command(CmdPrefix + CmdKeyState,					CmdDesState,					CmdUseState,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_State(commandInput)) },
			
            // Clear
            { CmdKeyClearChat,				new Command(CmdPrefix + CmdKeyClearChat,			CmdDesClearChat,				CmdUseClearChat,				CommandState.None,        commandInput => CommandMethodes.CommandFunction_ClearChat(commandInput)) },

            // Feedback toggle
          //{ CmdKeyFeedbackDisabled,		new Command(CmdPrefix + CmdKeyFeedbackDisabled,			CmdDesFeedbackDisabled,			CmdUseFeedbackDisabled,			CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_FeedbackDisabled(commandInput)) },

            // Command-Target name 
          //{ CmdKeyName,					new Command(CmdPrefix + CmdKeyName,						CmdDesName,						CmdUseName,						CommandState.None,        commandInput => CommandMethodes.CommandFunction_SetName(commandInput)) },

            // Mine Commands
            { CmdKeyMineReset,				new Command(CmdPrefix + CmdKeyMineReset,				CmdDesMineReset,				CmdUseMineReset,				CommandState.None,        commandInput => CommandMethodes.CommandFunction_MinesReset(commandInput)) },
            { CmdKeyMineOverfill,			new Command(CmdPrefix + CmdKeyMineOverfill,				CmdDesMineOverfill,				CmdUseMineOverfill,				CommandState.None,        commandInput => CommandMethodes.CommandFunction_MinesOverfill(commandInput)) },
            { CmdKeyMineClear,				new Command(CmdPrefix + CmdKeyMineClear,				CmdDesMineClear,				CmdUseMineClear,				CommandState.None,        commandInput => CommandMethodes.CommandFunction_MinesClear(commandInput)) },

            // Time Commands
            { CmdKeyPause,					new Command(CmdPrefix + CmdKeyPause,					CmdDesPause,					CmdUsePause,					CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_Pause(commandInput)) },
            { CmdKeyTimespeed,				new Command(CmdPrefix + CmdKeyTimespeed,				CmdDesTimespeed,				CmdUseTimespeed,				CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_Timespeed(commandInput)) },
            { CmdKeyDate,					new Command(CmdPrefix + CmdKeyDate,						CmdDesDate,						CmdUseDate,						CommandState.None,        commandInput => CommandMethodes.CommandFunction_Date(commandInput)) },
            { CmdKeyWeather,				new Command(CmdPrefix + CmdKeyWeather,					CmdDesWeather,					CmdUseWeather,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_Weather(commandInput)) },
            { CmdKeySeason,					new Command(CmdPrefix + CmdKeySeason,					CmdDesSeason,					CmdUseSeason,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_Season(commandInput)) },
            { CmdKeyYear,					new Command(CmdPrefix + CmdKeyYear,						CmdDesYear,						CmdUseYear,						CommandState.None,        commandInput => CommandMethodes.CommandFunction_Year(commandInput)) },
          //{ CmdKeyYearFix,				new Command(CmdPrefix + CmdKeyYearFix,                  CmdDesYearFix,                  CmdUseYearFix,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_ToggleYearFix(commandInput)) },

            // Currency Commands
            { CmdKeyMoney,					new Command(CmdPrefix + CmdKeyMoney,					CmdDesMoney,					CmdUseMoney,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_Coins(commandInput)) },
            { CmdKeyCoins,					new Command(CmdPrefix + CmdKeyCoins,					CmdDesCoins,					CmdUseCoins,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_Coins(commandInput)) },
            { CmdKeyOrbs,					new Command(CmdPrefix + CmdKeyOrbs,						CmdDesOrbs,						CmdUseOrbs,						CommandState.None,        commandInput => CommandMethodes.CommandFunction_Orbs(commandInput)) },
            { CmdKeyTickets,				new Command(CmdPrefix + CmdKeyTickets,					CmdDesTickets,					CmdUseTickets,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_Tickets(commandInput)) },

            // Player Commands
            { CmdKeySleep,					new Command(CmdPrefix + CmdKeySleep,					CmdDesSleep,					CmdUseSleep,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_Sleep(commandInput)) },
            { CmdKeyManaFill,				new Command(CmdPrefix + CmdKeyManaFill,					CmdDesManaFill,					CmdUseManaFill,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_ManaFill(commandInput)) },
            { CmdKeyManaInfinite,			new Command(CmdPrefix + CmdKeyManaInfinite,				CmdDesManaInfinite,				CmdUseManaInfinite,				CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_ManaInfinite(commandInput)) },
            { CmdKeyHealthFill,				new Command(CmdPrefix + CmdKeyHealthFill,				CmdDesHealthFill,				CmdUseHealthFill,				CommandState.None,        commandInput => CommandMethodes.CommandFunction_HealthFill(commandInput)) },
            { CmdKeyNoHit,					new Command(CmdPrefix + CmdKeyNoHit,					CmdDesNoHit,					CmdUseNoHit,					CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_NoHit(commandInput)) },
            { CmdKeyNoclip,					new Command(CmdPrefix + CmdKeyNoclip,					CmdDesNoClip,					CmdUseNoClip,					CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_Noclip(commandInput)) },
		  //{ CmdKeyJumper,                 new Command(CmdPrefix + CmdKeyJumper,                   CmdDesJumper,                   CmdUseJumper,                   CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_JumpOver(commandInput)) },
			{ CmdKeyJumpOver,				new Command(CmdPrefix + CmdKeyJumpOver,                 CmdDesJumpOver,                 CmdUseJumpOver,					CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_JumpOver(commandInput)) },
		  //{ CmdKeyDasher,					new Command(CmdPrefix + CmdKeyDasher,                   CmdDesDasher,                   CmdUseDasher,					CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_DashInfinite(commandInput)) },
			{ CmdKeyDashInfinite,           new Command(CmdPrefix + CmdKeyDashInfinite,             CmdDesDashInfinite,             CmdUseDashInfinite,             CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_DashInfinite(commandInput)) },

            // Misc Commands
            { CmdKeyAutoFillMuseum,			new Command(CmdPrefix + CmdKeyAutoFillMuseum,			CmdDesAutoFillMuseum,			CmdUseAutoFillMuseum,			CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_AutoFillMuseum(commandInput)) },
            { CmdKeyCheatFillMuseum,		new Command(CmdPrefix + CmdKeyCheatFillMuseum,			CmdDesCheatFillMuseum,			CmdUseCheatFillMuseum,			CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_CheatFillMuseum(commandInput)) },
            { CmdKeyUI,						new Command(CmdPrefix + CmdKeyUI,						CmdDesUI,						CmdUseUI,						CommandState.None,        commandInput => CommandMethodes.CommandFunction_UI(commandInput)) },
            { CmdKeyCheats,					new Command(CmdPrefix + CmdKeyCheats,					CmdDesCheats,					CmdUseCheats,					CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_Cheats(commandInput)) },

            // NPC relationship Commands
            { CmdKeyRelationship,			new Command(CmdPrefix + CmdKeyRelationship,				CmdDesRelationship,				CmdUseRelationship,				CommandState.None,        commandInput => CommandMethodes.CommandFunction_Relationship(commandInput)) },
            { CmdKeyDivorce,				new Command(CmdPrefix + CmdKeyDivorce,					CmdDesDivorce,					CmdUseDivorce,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_Divorce(commandInput)) },
            { CmdKeyMarry,					new Command(CmdPrefix + CmdKeyMarry,					CmdDesMarry,					CmdUseMarry,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_Marry(commandInput)) },

            // Item Commands
            { CmdKeyGiveItem,				new Command(CmdPrefix + CmdKeyGiveItem,					CmdDesGiveItem,					CmdUseGiveItem,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_GiveItem(commandInput)) },
            { CmdKeyShowItem,				new Command(CmdPrefix + CmdKeyShowItem,					CmdDesShowItem,					CmdUseShowItem,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_ShowItem(commandInput)) },
            { CmdKeyShowCategorizedItems,	new Command(CmdPrefix + CmdKeyShowCategorizedItems,		CmdDesShowCategorizedItems,		CmdUseShowCategorizedItems,		CommandState.None,        commandInput => CommandMethodes.CommandFunction_ShowCategorizedItems(commandInput)) },
            { CmdKeyShowItemInfoOnHover,	new Command(CmdPrefix + CmdKeyShowItemInfoOnHover,		CmdDesShowItemInfoOnHover,		CmdUseShowItemInfoOnHover,		CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_ShowItemInfoOnHover(commandInput)) },
            { CmdKeyShowItemInfoOnTooltip,	new Command(CmdPrefix + CmdKeyShowItemInfoOnTooltip,	CmdDesShowItemInfoOnTooltip,	CmdUseShowItemInfoOnTooltip,	CommandState.Deactivated, commandInput => CommandMethodes.CommandFunction_ShowItemInfoOnTooltip(commandInput)) },
			{ CmdKeyDevKit,					new Command(CmdPrefix + CmdKeyDevKit,					CmdDesDevKit,					CmdUseDevKit,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_DevKit(commandInput)) },

            // Teleport
            { CmdKeyTeleport,				new Command(CmdPrefix + CmdKeyTeleport,					CmdDesTeleport,					CmdUseTeleport,					CommandState.None,        commandInput => CommandMethodes.CommandFunction_TeleportToScene(commandInput)) },
            { CmdKeyTeleportLocations,		new Command(CmdPrefix + CmdKeyTeleportLocations,		CmdDesTeleportLocations,		CmdUseTeleportLocations,		CommandState.None,        commandInput => CommandMethodes.CommandFunction_TeleportLocations(commandInput)) }
        };
		
		private static readonly Dictionary<string, string> _commandAliases = new() {
			{ CmdKeyDasher, CmdKeyDashInfinite },
			{ CmdKeyJumper, CmdKeyJumpOver },
			{ CmdKeyUnmarry, CmdKeyDivorce } };

		/// <summary>
		/// Determines if the entered chat text has a command prefix.
		/// </summary>
		/// <param name="inputText">The raw chat input text.</param>
		/// <returns>
		/// True when the text has a command prefix, else False.
		/// </returns>
		public static bool HasCommandPrefix(string prefixedCommandInput)
		{
			// Command syntax: must start with '!' but not '!!' (latter is an escape for literal '!')
			return prefixedCommandInput.Length >= 2 && prefixedCommandInput[0] == '!' && prefixedCommandInput[1] != '!';
		}

		/// <summary>
		/// Parses the input string to determine the command key (first token) and,
		/// if a matching command exists, invokes that command.
		/// </summary>
		/// <param name="commandInput">The full input string.</param>
		public static void ProcessCommand(string commandInput)
        {
			// remove prefix from command input
			commandInput = commandInput.Remove(0, 1);

			// get command key from input, resolve the alias and replace the command key commandInput with the resolved command key
			string commandKey = commandInput.Split([' '], System.StringSplitOptions.RemoveEmptyEntries)[0];
			string resolvedCommandKey = ResolveCommandAlias(commandKey);
			commandInput = commandInput.Replace(commandKey, resolvedCommandKey);
			
			// if the command is registered, invoke the command
			if (GeneratedCommands.TryGetValue(resolvedCommandKey, out Command command))
            {
                command.Invoke(commandInput);
            }
			else
			{
				CommandMethodes.MessageToChat($"Unknown Command: ".ColorText(CommandExtension.RedColor)
					+ commandKey.ColorText(Color.white)
					+ "\n" + commandInput.ColorText(CommandExtension.DarkGrayColor));
			}
        }

		/// <summary>
		/// Resolves known command aliases to their command keys.
		/// If the provided key matches a known alias, the corresponding key is returned;
		/// otherwise the original key is returned unchanged.
		/// </summary>
		/// <param name="commandKey">The input command key or alias to resolve.</param>
		/// <returns>The command key for the alias, or the original key if no alias matches.</returns>
		public static string ResolveCommandAlias(string commandKey)
		{
			return _commandAliases.TryGetValue(commandKey, out string newCommandKey) ? newCommandKey : commandKey;
		}

		/// <summary>
		/// Attempts to find a Command by its key and returns it if found, otherwise returns null.
		/// </summary>
		/// <param name="commandKey">The key/name of the command to look up.</param>
		/// <returns>The matching Command instance or null when not present.</returns>
		public static Command GetCommandByKey(string commandKey)
        {
			return GeneratedCommands.TryGetValue(commandKey, out Command command) ? command : null;
		}

		/// <summary>
		/// Checks whether the command identified by <paramref name="commandKey"/> exists and is currently in the Activated state.
		/// </summary>
		/// <param name="commandKey">The key/name of the command to check.</param>
		/// <returns>True if the command exists and is Activated, otherwise false.</returns>
		public static bool IsCommandActive(string commandKey)
        {
			return GeneratedCommands.TryGetValue(commandKey, out Command command) && command.State == CommandState.Activated;
		}

		/// <summary>
		/// Toggles the state of the command identified by <paramref name="commandKey"/> between
		/// Activated and Deactivated. If the command is found, its state is flipped and the
		/// method returns true if the new state is Activated; otherwise returns false.
		/// </summary>
		/// <param name="commandKey">The key/name of the command to toggle.</param>
		/// <returns>True when the command was found and is now Activated, false when not found or Deactivated.</returns>
		public static bool ToggleCommandState(string commandKey)
        {
			return GeneratedCommands.TryGetValue(commandKey, out Command command) && (command.State = (command.State == CommandState.Activated) ? CommandState.Deactivated : CommandState.Activated) == CommandState.Activated;
		}

		/// <summary>
		/// Sets the state of the command identified by <paramref name="commandKey"/> to the
		/// specified <paramref name="commandState"/> if the command exists.
		/// </summary>
		/// <param name="commandKey">The key/name of the command to modify.</param>
		/// <param name="commandState">The desired new state for the command.</param>
		public static void SetCommandState(string commandKey, CommandState commandState)
        {
            if (GeneratedCommands.TryGetValue(commandKey, out Command command))
            {
                command.State = commandState;
            }
        }

		/// <summary>
		/// Sets the state of the command identified by <paramref name="commandKey"/> based on
		/// the boolean <paramref name="activate"/> flag: Activated when true, Deactivated when false.
		/// </summary>
		/// <param name="commandKey">The key/name of the command to modify.</param>
		/// <param name="activate">True to activate the command, false to deactivate it.</param>
		public static void SetCommandState(string commandKey, bool activate)
        {
            if (GeneratedCommands.TryGetValue(commandKey, out Command command))
            {
                command.State = activate ? CommandState.Activated : CommandState.Deactivated;
            }
        }
    }
}
