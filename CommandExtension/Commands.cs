using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandExtension.Models;

namespace CommandExtension
{
    public static class Commands
    {
        // Core prefix
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

        // Feedback toggle
        public const string CmdKeyFeedbackDisabled = "feedback";
        public const string CmdDesFeedbackDisabled = "Toggles chat feedback for command execution.";
        public const string CmdUseFeedbackDisabled = CmdPrefix + CmdKeyFeedbackDisabled;

        // Command target name
        public const string CmdKeyName = "name";
        public const string CmdDesName = "Sets or resets the player name that commands will target.";
        public const string CmdUseName = CmdPrefix + CmdKeyName + " [playerName]*";

        // Mine commands
        public const string CmdKeyMineReset = "minereset";
        public const string CmdDesMineReset = "Reset the current mine.";
        public const string CmdUseMineReset = CmdPrefix + CmdKeyMineReset;

        public const string CmdKeyMineClear = "mineclear";
        public const string CmdDesMineClear = "Removes all rocks and ores from the mine.";
        public const string CmdUseMineClear = CmdPrefix + CmdKeyMineClear;

        public const string CmdKeyMineOverfill = "mineoverfill";
        public const string CmdDesMineOverfill = "Fills the mine completely with rocks and ores.";
        public const string CmdUseMineOverfill = CmdPrefix + CmdKeyMineOverfill;

        // Time commands
        public const string CmdKeyPause = "pause";
        public const string CmdDesPause = "Toggles the game’s time pause on or off.";
        public const string CmdUsePause = CmdPrefix + CmdKeyPause;

        public const string CmdKeyCustomDaySpeed = "timespeed";
        public const string CmdDesCustomDaySpeed = "Sets or toggles a custom day-length multiplier.";
        public const string CmdUseCustomDaySpeed = CmdPrefix + CmdKeyCustomDaySpeed + " [multiplier|reset]";

        public const string CmdKeySetDate = "time";
        public const string CmdDesSetDate = "Sets the current hour or day in the day/night cycle.";
        public const string CmdUseSetDate = CmdPrefix + CmdKeySetDate + " [h|d] [value]";

        public const string CmdKeyWeather = "weather";
        public const string CmdDesWeather = "Changes weather to raining, heatwave, or clear.";
        public const string CmdUseWeather = CmdPrefix + CmdKeyWeather + " [raining|heatwave|clear]";

        public const string CmdKeySetSeason = "season";
        public const string CmdDesSetSeason = "Changes the current season.";
        public const string CmdUseSetSeason = CmdPrefix + CmdKeySetSeason + " [Spring|Summer|Fall|Winter]";

        public const string CmdKeyIncDecYear = "years";
        public const string CmdDesIncDecYear = "Advances or rewinds the in-game year by increments of four months.";
        public const string CmdUseIncDecYear = CmdPrefix + CmdKeyIncDecYear + " (-)[years]";

        // Currency commands
        public const string CmdKeyMoney = "money";
        public const string CmdDesMoney = "Alias for !coins.";
        public const string CmdUseMoney = CmdPrefix + CmdKeyMoney + " (-)[amount]";

        public const string CmdKeyCoins = "coins";
        public const string CmdDesCoins = "Adds or subtracts Coins from the player.";
        public const string CmdUseCoins = CmdPrefix + CmdKeyCoins + " (-)[amount]";

        public const string CmdKeyOrbs = "orbs";
        public const string CmdDesOrbs = "Adds or subtracts Orbs from the player.";
        public const string CmdUseOrbs = CmdPrefix + CmdKeyOrbs + " (-)[amount]";

        public const string CmdKeyTickets = "tickets";
        public const string CmdDesTickets = "Adds or subtracts Tickets from the player.";
        public const string CmdUseTickets = CmdPrefix + CmdKeyTickets + " (-)[amount]";

        public const string CmdKeyDevKit = "devkit";
        public const string CmdDesDevKit = "Grants the developer kit items.";
        public const string CmdUseDevKit = CmdPrefix + CmdKeyDevKit;

        // Player toggles
        public const string CmdKeyJumper = "jumper";
        public const string CmdDesJumper = "Toggles ability to jump through objects.";
        public const string CmdUseJumper = CmdPrefix + CmdKeyJumper;

        public const string CmdKeyDasher = "dasher";
        public const string CmdDesDasher = "Toggles infinite dash charges.";
        public const string CmdUseDasher = CmdPrefix + CmdKeyDasher;

        public const string CmdKeyManaFill = "manafill";
        public const string CmdDesManaFill = "Refills the player’s mana to maximum.";
        public const string CmdUseManaFill = CmdPrefix + CmdKeyManaFill;

        public const string CmdKeyManaInf = "manainf";
        public const string CmdDesManaInf = "Toggles infinite mana.";
        public const string CmdUseManaInf = CmdPrefix + CmdKeyManaInf;

        public const string CmdKeyHealthFill = "healthfill";
        public const string CmdDesHealthFill = "Refills the player’s health to maximum.";
        public const string CmdUseHealthFill = CmdPrefix + CmdKeyHealthFill;

        public const string CmdKeyNoHit = "nohit";
        public const string CmdDesNoHit = "Toggles invincibility (no damage taken).";
        public const string CmdUseNoHit = CmdPrefix + CmdKeyNoHit;

        public const string CmdKeyNoClip = "noclip";
        public const string CmdDesNoClip = "Toggles noclip mode (walk through walls).";
        public const string CmdUseNoClip = CmdPrefix + CmdKeyNoClip;

        // Misc toggles
        public const string CmdKeyUI = "ui";
        public const string CmdDesUI = "Toggles the game’s HUD and UI elements.";
        public const string CmdUseUI = CmdPrefix + CmdKeyUI + " [on|off]*";

        public const string CmdKeyAutoFillMuseum = "autofillmuseum";
        public const string CmdDesAutoFillMuseum = "Toggles auto-fill of museum bundles on entry.";
        public const string CmdUseAutoFillMuseum = CmdPrefix + CmdKeyAutoFillMuseum;

        public const string CmdKeyCheatFillMuseum = "cheatfillmuseum";
        public const string CmdDesCheatFillMuseum = "Toggles cheat-fill of museum bundles on entry.";
        public const string CmdUseCheatFillMuseum = CmdPrefix + CmdKeyCheatFillMuseum;

        public const string CmdKeyFixYear = "yearfix";
        public const string CmdDesFixYear = "Toggles corrected year calculation.";
        public const string CmdUseFixYear = CmdPrefix + CmdKeyFixYear;

        public const string CmdKeyCheats = "cheats";
        public const string CmdDesCheats = "Toggles the game’s built-in cheats and hotkeys.";
        public const string CmdUseCheats = CmdPrefix + CmdKeyCheats;

        // NPC relationship
        public const string CmdKeyUnMarry = "divorce";
        public const string CmdDesUnMarry = "Divorces a single or all NPCs.";
        public const string CmdUseUnMarry = CmdPrefix + CmdKeyUnMarry + " [name|all]";

        public const string CmdKeyMarry = "marry";
        public const string CmdDesMarry = "Marries a single or all NPCs.";
        public const string CmdUseMarry = CmdPrefix + CmdKeyMarry + " [name|all]";

        public const string CmdKeyRelationship = "relationship";
        public const string CmdDesRelationship = "Sets or adds to NPC relationship values.";
        public const string CmdUseRelationship = CmdPrefix + CmdKeyRelationship + " [name|all] [value] [add]*";

        // Items
        public const string CmdKeyGive = "give";
        public const string CmdDesGive = "Gives item(s) to the player by ID or name.";
        public const string CmdUseGive = CmdPrefix + CmdKeyGive + " [ID|name] [amount]";

        public const string CmdKeyShowItems = "items";
        public const string CmdDesShowItems = "Lists items matching the given name.";
        public const string CmdUseShowItems = CmdPrefix + CmdKeyShowItems + " [name]";

        public const string CmdKeyPrintCategorizedItems = "getitemids";
        public const string CmdDesPrintCategorizedItems = "Prints item IDs filtered by category.";
        public const string CmdUsePrintCategorizedItems = CmdPrefix + CmdKeyPrintCategorizedItems + " [xp|currency|bonus|pet|decoration|armor|tool|food|crop|fish|normal]";

        public const string CmdKeyShowItemIdOnHover = "showidonhover";
        public const string CmdDesShowItemIdOnHover = "Displays the item ID when hovering.";
        public const string CmdUseShowItemIdOnHover = CmdPrefix + CmdKeyShowItemIdOnHover;

        public const string CmdKeyShowItemIdOnTooltip = "showid";
        public const string CmdDesShowItemIdOnTooltip = "Toggles showing item IDs in tooltips.";
        public const string CmdUseShowItemIdOnTooltip = CmdPrefix + CmdKeyShowItemIdOnTooltip;

        // Teleport
        public const string CmdKeyTeleport = "tp";
        public const string CmdDesTeleport = "Teleports the player to a location.";
        public const string CmdUseTeleport = CmdPrefix + CmdKeyTeleport + " [location]";

        public const string CmdKeyTeleportLocations = "tps";
        public const string CmdDesTeleportLocations = "Lists all available teleport destinations.";
        public const string CmdUseTeleportLocations = CmdPrefix + CmdKeyTeleportLocations;

        // Sleep
        public const string CmdKeySleep = "sleep";
        public const string CmdDesSleep = "Sleeps through the night.";
        public const string CmdUseSleep = CmdPrefix + CmdKeySleep;

        public static readonly Dictionary<string, Command> GeneratedCommands = new Dictionary<string, Command>() {
            // Debug
            { CmdPrefix + CmdKeyDebug,              new Command(CmdPrefix + CmdKeyDebug,                  CmdDesDebug,                CmdUseDebug,                CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_Debug(commandInput)) },

            // Help
            { CmdPrefix + CmdKeyHelp,               new Command(CmdPrefix + CmdKeyHelp,                   CmdDesHelp,                 CmdUseHelp,                 CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_Help(commandInput)) },

            // State
            { CmdPrefix + CmdKeyState,              new Command(CmdPrefix + CmdKeyState,                  CmdDesState,                CmdUseState,                CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_State(commandInput)) },

            // Feedback toggle
            { CmdPrefix + CmdKeyFeedbackDisabled,   new Command(CmdPrefix + CmdKeyFeedbackDisabled,       CmdDesFeedbackDisabled,     CmdUseFeedbackDisabled,     CommandState.Deactivated, (string commandInput) => CommandMethodes.CommandFunction_ToggleFeedback(commandInput)) },

            // Command target name
            { CmdPrefix + CmdKeyName,               new Command(CmdPrefix + CmdKeyName,                   CmdDesName,                 CmdUseName,                 CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_SetName(commandInput)) },

            // Mine commands
            { CmdPrefix + CmdKeyMineReset,          new Command(CmdPrefix + CmdKeyMineReset,              CmdDesMineReset,            CmdUseMineReset,            CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_ResetMines(commandInput)) },
            { CmdPrefix + CmdKeyMineOverfill,       new Command(CmdPrefix + CmdKeyMineOverfill,           CmdDesMineOverfill,         CmdUseMineOverfill,         CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_OverfillMines(commandInput)) },
            { CmdPrefix + CmdKeyMineClear,          new Command(CmdPrefix + CmdKeyMineClear,              CmdDesMineClear,            CmdUseMineClear,            CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_ClearMines(commandInput)) },

            // Time commands
            { CmdPrefix + CmdKeyPause,              new Command(CmdPrefix + CmdKeyPause,                  CmdDesPause,                CmdUsePause,                CommandState.Deactivated, (string commandInput) => CommandMethodes.CommandFunction_Pause(commandInput)) },
            { CmdPrefix + CmdKeyCustomDaySpeed,     new Command(CmdPrefix + CmdKeyCustomDaySpeed,         CmdDesCustomDaySpeed,       CmdUseCustomDaySpeed,       CommandState.Deactivated, (string commandInput) => CommandMethodes.CommandFunction_CustomDaySpeed(commandInput)) },
            { CmdPrefix + CmdKeySetDate,            new Command(CmdPrefix + CmdKeySetDate,                CmdDesSetDate,              CmdUseSetDate,              CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_ChangeDate(commandInput)) },
            { CmdPrefix + CmdKeyWeather,            new Command(CmdPrefix + CmdKeyWeather,                CmdDesWeather,              CmdUseWeather,              CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_ChangeWeather(commandInput)) },
            { CmdPrefix + CmdKeySetSeason,          new Command(CmdPrefix + CmdKeySetSeason,              CmdDesSetSeason,            CmdUseSetSeason,            CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_SetSeason(commandInput)) },
            { CmdPrefix + CmdKeyIncDecYear,         new Command(CmdPrefix + CmdKeyIncDecYear,             CmdDesIncDecYear,           CmdUseIncDecYear,           CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_Year(commandInput)) },
          //{ CmdPrefix + CmdFixYear,               new Command(CmdDesYear,                 CmdUseYear,                 CommandState.Activated),

            // Currency commands
            { CmdPrefix + CmdKeyMoney,              new Command(CmdPrefix + CmdKeyMoney,                  CmdDesMoney,                CmdUseMoney,                CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_AddMoney(commandInput)) },
            { CmdPrefix + CmdKeyCoins,              new Command(CmdPrefix + CmdKeyCoins,                  CmdDesCoins,                CmdUseCoins,                CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_AddMoney(commandInput)) },
            { CmdPrefix + CmdKeyOrbs,               new Command(CmdPrefix + CmdKeyOrbs,                   CmdDesOrbs,                 CmdUseOrbs,                 CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_AddOrbs(commandInput)) },
            { CmdPrefix + CmdKeyTickets,            new Command(CmdPrefix + CmdKeyTickets,                CmdDesTickets,              CmdUseTickets,              CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_AddTickets(commandInput)) },

            // Player
            { CmdPrefix + CmdKeySleep,              new Command(CmdPrefix + CmdKeySleep,                  CmdDesSleep,                CmdUseSleep,                CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_Sleep(commandInput)) },
            { CmdPrefix + CmdKeyManaFill,           new Command(CmdPrefix + CmdKeyManaFill,               CmdDesManaFill,             CmdUseManaFill,             CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_ManaFill(commandInput)) },
            { CmdPrefix + CmdKeyManaInf,            new Command(CmdPrefix + CmdKeyManaInf,                CmdDesManaInf,              CmdUseManaInf,              CommandState.Deactivated, (string commandInput) => CommandMethodes.CommandFunction_InfiniteMana(commandInput)) },
            { CmdPrefix + CmdKeyHealthFill,         new Command(CmdPrefix + CmdKeyHealthFill,             CmdDesHealthFill,           CmdUseHealthFill,           CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_HealthFill(commandInput)) },
            { CmdPrefix + CmdKeyNoHit,              new Command(CmdPrefix + CmdKeyNoHit,                  CmdDesNoHit,                CmdUseNoHit,                CommandState.Deactivated, (string commandInput) => CommandMethodes.CommandFunction_NoHit(commandInput)) },
            { CmdPrefix + CmdKeyNoClip,             new Command(CmdPrefix + CmdKeyNoClip,                 CmdDesNoClip,               CmdUseNoClip,               CommandState.Deactivated, (string commandInput) => CommandMethodes.CommandFunction_NoClip(commandInput)) },
            { CmdPrefix + CmdKeyJumper,             new Command(CmdPrefix + CmdKeyJumper,                 CmdDesJumper,               CmdUseJumper,               CommandState.Deactivated, (string commandInput) => CommandMethodes.CommandFunction_Jumper(commandInput)) },
            { CmdPrefix + CmdKeyDasher,             new Command(CmdPrefix + CmdKeyDasher,                 CmdDesDasher,               CmdUseDasher,               CommandState.Deactivated, (string commandInput) => CommandMethodes.CommandFunction_InfiniteAirSkips(commandInput)) },

            // Misc
            { CmdPrefix + CmdKeyAutoFillMuseum,     new Command(CmdPrefix + CmdKeyAutoFillMuseum,         CmdDesAutoFillMuseum,       CmdUseAutoFillMuseum,       CommandState.Deactivated, (string commandInput) => CommandMethodes.CommandFunction_AutoFillMuseum(commandInput)) },
            { CmdPrefix + CmdKeyCheatFillMuseum,    new Command(CmdPrefix + CmdKeyCheatFillMuseum,        CmdDesCheatFillMuseum,      CmdUseCheatFillMuseum,      CommandState.Deactivated, (string commandInput) => CommandMethodes.CommandFunction_CheatFillMuseum(commandInput)) },
            { CmdPrefix + CmdKeyUI,                 new Command(CmdPrefix + CmdKeyUI,                     CmdDesUI,                   CmdUseUI,                   CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_ToggleUI(commandInput)) },
            { CmdPrefix + CmdKeyCheats,             new Command(CmdPrefix + CmdKeyCheats,                 CmdDesCheats,               CmdUseCheats,               CommandState.Deactivated, (string commandInput) => CommandMethodes.CommandFunction_ToggleCheats(commandInput)) },

            // NPC relationship
            { CmdPrefix + CmdKeyRelationship,       new Command(CmdPrefix + CmdKeyRelationship,           CmdDesRelationship,         CmdUseRelationship,         CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_Relationship(commandInput)) },
            { CmdPrefix + CmdKeyUnMarry,            new Command(CmdPrefix + CmdKeyUnMarry,                CmdDesUnMarry,              CmdUseUnMarry,              CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_UnMarry(commandInput)) },
            { CmdPrefix + CmdKeyMarry,              new Command(CmdPrefix + CmdKeyMarry,                  CmdDesMarry,                CmdUseMarry,                CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_Marry(commandInput)) },

            // Items
            { CmdPrefix + CmdKeyGive,               new Command(CmdPrefix + CmdKeyGive,                   CmdDesGive,                 CmdUseGive,                 CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_GiveItemByIdOrName(commandInput)) },
            { CmdPrefix + CmdKeyShowItems,          new Command(CmdPrefix + CmdKeyShowItems,              CmdDesShowItems,            CmdUseShowItems,            CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_ShowID(commandInput)) },
            { CmdPrefix + CmdKeyPrintCategorizedItems,       new Command(CmdPrefix + CmdKeyPrintCategorizedItems,           CmdDesPrintCategorizedItems,         CmdUsePrintCategorizedItems,         CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_PrintItemIds(commandInput)) },
            { CmdPrefix + CmdKeyShowItemIdOnHover,  new Command(CmdPrefix + CmdKeyShowItemIdOnHover,      CmdDesShowItemIdOnHover,    CmdUseShowItemIdOnHover,    CommandState.Deactivated, (string commandInput) => CommandMethodes.CommandFunction_PrintItemIdOnHover(commandInput)) },
          //{ CmdPrefix + CmdAppendItemDescWithId,  new Command(CmdPrefix + CmdAppendItemDescWithId,      CmdDesendItemDescWithId,    CmdUseendItemDescWithId,    CommandState.Deactivated),
            { CmdPrefix + CmdKeyDevKit,             new Command(CmdPrefix + CmdKeyDevKit,                 CmdDesDevKit,               CmdUseDevKit,               CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_GiveDevItems(commandInput)) },

            // Teleport
            { CmdPrefix + CmdKeyTeleport,           new Command(CmdPrefix + CmdKeyTeleport,               CmdDesTeleport,             CmdUseTeleport,             CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_TeleportToScene(commandInput)) },
            { CmdPrefix + CmdKeyTeleportLocations,  new Command(CmdPrefix + CmdKeyTeleportLocations,      CmdDesTeleportLocations,    CmdUseTeleportLocations,    CommandState.None,        (string commandInput) => CommandMethodes.CommandFunction_TeleportLocations(commandInput)) }
        };

        public static void ProcessCommands(string commandInput)
        {
            string commandKey = commandInput.Split(' ')[0];
            
            if (GeneratedCommands.TryGetValue(commandKey, out Command command))
            {
                command.Invoke(commandInput);
            }

        }

        public static Command GetCommandByKey(string commandKey)
        {
            if (GeneratedCommands.TryGetValue(commandKey, out Command command))
            {
                return command;
            }

            return null;
        }

        public static bool IsCommandActive(string commandKey)
        {
            if (GeneratedCommands.TryGetValue(commandKey, out Command command))
            {
                return command.State == CommandState.Activated;
            }

            return false;
        }

        public static bool ToggleCommandState(string commandKey)
        {
            if (GeneratedCommands.TryGetValue(commandKey, out Command command))
            {
                return (command.State = (command.State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated)) == CommandState.Activated;
            }

            return false;
        }

        public static void SetCommandState(string commandKey, CommandState commandState)
        {
            if (GeneratedCommands.TryGetValue(commandKey, out Command command))
            {
                command.State = commandState;
            }
        }

        public static void SetCommandState(string commandKey, bool activate)
        {
            if (GeneratedCommands.TryGetValue(commandKey, out Command command))
            {
                command.State = activate ? CommandState.Activated : CommandState.Deactivated;
            }
        }
    }
}
