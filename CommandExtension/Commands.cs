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
        // Core
        public const string CmdPrefix = "!";
        public const string CmdDebug = "debug";
        public const string CmdHelp = "help";
        public const string CmdState = "state";
        public const string CmdFeedbackDisabled = "feedback";
        public const string CmdName = "name";

        // mine related
        public const string CmdMineReset = "minereset";
        public const string CmdMineClear = "mineclear";
        public const string CmdMineOverfill = "mineoverfill";

        // time
        public const string CmdPause = "pause";
        public const string CmdCustomDaySpeed = "timespeed";
        public const string CmdSetDate = "time";
        public const string CmdWeather = "weather";
        public const string CmdIncDecYear = "years";
        public const string CmdSetSeason = "season";

        // currency
        public const string CmdMoney = "money";
        public const string CmdCoins = "coins";
        public const string CmdOrbs = "orbs";
        public const string CmdTickets = "tickets";
        public const string CmdDevKit = "devkit";

        // toggles
        public const string CmdJumper = "jumper";
        public const string CmdDasher = "dasher";
        public const string CmdManaFill = "manafill";
        public const string CmdManaInf = "manainf";
        public const string CmdHealthFill = "healthfill";
        public const string CmdNoHit = "nohit";
        public const string CmdNoClip = "noclip";
        public const string CmdUI = "ui";
        public const string CmdAutoFillMuseum = "autofillmuseum";
        public const string CmdCheatFillMuseum = "cheatfillmuseum";
        //public const string CmdFixYear = "yearfix";
        public const string CmdCheats = "cheats";

        // npc relationship
        public const string CmdUnMarry = "divorce";
        public const string CmdMarryNpc = "marry";
        public const string CmdRelationship = "relationship";

        // items
        public const string CmdGive = "give";
        public const string CmdShowItems = "items";
        public const string CmdPrintItemIds = "getitemids";
        public const string CmdPrintHoverItem = "printhoveritem";
        public const string CmdAppendItemDescWithId = "showid";

        // teleport
        public const string CmdTeleport = "tp";
        public const string CmdTeleportLocations = "tps";

        // misc
        public const string CmdSleep = "sleep";

        public static Command[] GeneratedCommands = GenerateCommands();
        private static Command[] GenerateCommands()
        {
            Command[] value = new Command[] {
                new Command(CmdPrefix + CmdHelp,                    "print commands to chat", CommandState.None),
                new Command(CmdPrefix + CmdMineReset,               "refill all mine shafts!", CommandState.None),
                new Command(CmdPrefix + CmdPause,                   "toggle time pause!", CommandState.Deactivated),
                new Command(CmdPrefix + CmdCustomDaySpeed,          "toggle or change dayspeed, paused if '!pause' is activ!", CommandState.Deactivated),
                new Command(CmdPrefix + CmdMoney,                   "give or remove coins", CommandState.None),
                new Command(CmdPrefix + CmdOrbs,                    "give or remove Orbs", CommandState.None),
                new Command(CmdPrefix + CmdTickets,                 "give or remove Tickets", CommandState.None),
                new Command(CmdPrefix + CmdSetDate,                 "set hour '6-23' e.g. 'set h 12'\nset Day '1-28' e.g. 'set d 12'", CommandState.None),
                new Command(CmdPrefix + CmdWeather,                 "'!weather [raining|heatwave|clear]'", CommandState.None),
                new Command(CmdPrefix + CmdDevKit,                  "get dev items", CommandState.None),
                new Command(CmdPrefix + CmdJumper,                  "jump over object's (actually noclip while jump)", CommandState.Deactivated),
                new Command(CmdPrefix + CmdState,                   "print activ commands", CommandState.None),
                new Command(CmdPrefix + CmdSleep,                   "sleep to next the day", CommandState.None),
                new Command(CmdPrefix + CmdDasher,                  "infinite dashes", CommandState.Deactivated),
                new Command(CmdPrefix + CmdManaFill,                "mana refill", CommandState.None),
                new Command(CmdPrefix + CmdManaInf,                 "infinite mana", CommandState.Deactivated),
                new Command(CmdPrefix + CmdHealthFill,              "health refill", CommandState.None),
                new Command(CmdPrefix + CmdNoHit,                   "no hit (disable hitbox)", CommandState.Deactivated),
                new Command(CmdPrefix + CmdMineOverfill,            "fill mine completely with rocks & ores", CommandState.None),
                new Command(CmdPrefix + CmdMineClear,               "clear mine completely from rocks & ores", CommandState.None),
                new Command(CmdPrefix + CmdNoClip,                  "walk trough everything", CommandState.Deactivated),
                new Command(CmdPrefix + CmdName,                    "set name for command target ('!name Lynn') only '!name resets it' ", CommandState.None),
                new Command(CmdPrefix + CmdFeedbackDisabled,        "toggle command feedback on/off", CommandState.Deactivated),
                new Command(CmdPrefix + CmdGive,                    "give [ID] [AMOUNT]*",                                                      CommandState.None),
                new Command(CmdPrefix + CmdShowItems,               "print items with the given name",                                          CommandState.None),
                new Command(CmdPrefix + CmdPrintItemIds,            "print item ids [xp|money|all|bonus]",                                      CommandState.None),
                new Command(CmdPrefix + CmdPrintHoverItem,          "print item id to chat",                                                    CommandState.Deactivated),
                //new Command(CmdPrefix + CmdAppendItemDescWithId,    "toggle id shown to item description",                                      CommandState.Deactivated),
                new Command(CmdPrefix + CmdAutoFillMuseum,          "toggle museum's auto fill upon entry",                                     CommandState.Deactivated),
                new Command(CmdPrefix + CmdCheatFillMuseum,         "toggle fill museum completely upon entry",                                 CommandState.Deactivated),
                new Command(CmdPrefix + CmdUI,                      "turn ui on/off", CommandState.None),
                new Command(CmdPrefix + CmdTeleport,                "teleport to some locations", CommandState.None),
                new Command(CmdPrefix + CmdTeleportLocations,       "get teleport locations", CommandState.None),
                new Command(CmdPrefix + CmdRelationship,            "'!relationship [name/all] [value] [add]*'", CommandState.None),
                new Command(CmdPrefix + CmdUnMarry,                 "unmarry an NPC '!divorce [name/all]'", CommandState.None),
                new Command(CmdPrefix + CmdMarryNpc,                "marry an NPC '!marry [name/all]'", CommandState.None),
                new Command(CmdPrefix + CmdSetSeason,               "change season", CommandState.None),
                //new Command(CmdPrefix + CmdFixYear,               "fix year (if needed)",                                                     CommandState.Activated),
                new Command(CmdPrefix + CmdIncDecYear,              "add or sub years '!years [value]' '-' to sub", CommandState.None),
                new Command(CmdPrefix + CmdCheats,                  "Toggle Cheats and hotkeys like F7,F8", CommandState.Deactivated)
            };

            Array.Sort(value, (x, y) => x.Name.CompareTo(y.Name));
            return value;
        }

        public static void ProcessCommands(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');
            string cmd = mayCommandParam[0];
            
            Action action = cmd switch
            {
                CmdPrefix + CmdDebug => () => CommandMethodes.CommandFunction_Debug(mayCommandParam),
                CmdPrefix + CmdHelp => () => CommandMethodes.CommandFunction_Help(),
                CmdPrefix + CmdState => () => CommandMethodes.CommandFunction_Help(true),
                CmdPrefix + CmdFeedbackDisabled => () => CommandMethodes.CommandFunction_ToggleFeedback(),
                CmdPrefix + CmdMineReset => () => CommandMethodes.CommandFunction_ResetMines(),
                CmdPrefix + CmdPause => () => CommandMethodes.CommandFunction_Pause(),
                CmdPrefix + CmdCustomDaySpeed => () => CommandMethodes.CommandFunction_CustomDaySpeed(commandInput),
                CmdPrefix + CmdMoney => () => CommandMethodes.CommandFunction_AddMoney(commandInput),
                CmdPrefix + CmdCoins => () => CommandMethodes.CommandFunction_AddMoney(commandInput),
                CmdPrefix + CmdOrbs => () => CommandMethodes.CommandFunction_AddOrbs(commandInput),
                CmdPrefix + CmdTickets => () => CommandMethodes.CommandFunction_AddTickets(commandInput),
                CmdPrefix + CmdSetDate => () => CommandMethodes.CommandFunction_ChangeDate(commandInput),
                CmdPrefix + CmdWeather => () => CommandMethodes.CommandFunction_ChangeWeather(commandInput),
                CmdPrefix + CmdDevKit => () => CommandMethodes.CommandFunction_GiveDevItems(),
                CmdPrefix + CmdJumper => () => CommandMethodes.CommandFunction_Jumper(),
                CmdPrefix + CmdCheats => () => CommandMethodes.CommandFunction_ToggleCheats(),
                CmdPrefix + CmdSleep => () => CommandMethodes.CommandFunction_Sleep(),
                CmdPrefix + CmdDasher => () => CommandMethodes.CommandFunction_InfiniteAirSkips(),
                CmdPrefix + CmdManaFill => () => CommandMethodes.CommandFunction_ManaFill(),
                CmdPrefix + CmdManaInf => () => CommandMethodes.CommandFunction_InfiniteMana(),
                CmdPrefix + CmdHealthFill => () => CommandMethodes.CommandFunction_HealthFill(),
                CmdPrefix + CmdNoHit => () => CommandMethodes.CommandFunction_NoHit(),
                CmdPrefix + CmdMineOverfill => () => CommandMethodes.CommandFunction_OverfillMines(),
                CmdPrefix + CmdMineClear => () => CommandMethodes.CommandFunction_ClearMines(),
                CmdPrefix + CmdNoClip => () => CommandMethodes.CommandFunction_NoClip(),
                CmdPrefix + CmdPrintHoverItem => () => CommandMethodes.CommandFunction_PrintItemIdOnHover(),
                CmdPrefix + CmdName => () => CommandMethodes.CommandFunction_SetName(commandInput),
                CmdPrefix + CmdGive => () => CommandMethodes.CommandFunction_GiveItemByIdOrName(mayCommandParam),
                CmdPrefix + CmdShowItems => () => CommandMethodes.CommandFunction_ShowItemByName(mayCommandParam),
                CmdPrefix + CmdPrintItemIds => () => CommandMethodes.CommandFunction_PrintItemIds(mayCommandParam),
                CmdPrefix + CmdAutoFillMuseum => () => CommandMethodes.CommandFunction_AutoFillMuseum(),
                CmdPrefix + CmdCheatFillMuseum => () => CommandMethodes.CommandFunction_CheatFillMuseum(),
                CmdPrefix + CmdUI => () => CommandMethodes.CommandFunction_ToggleUI(commandInput),
                CmdPrefix + CmdTeleport => () => CommandMethodes.CommandFunction_TeleportToScene(mayCommandParam),
                CmdPrefix + CmdTeleportLocations => () => CommandMethodes.CommandFunction_TeleportLocations(),
                CmdPrefix + CmdAppendItemDescWithId => () => CommandMethodes.CommandFunction_ShowID(),
                CmdPrefix + CmdRelationship => () => CommandMethodes.CommandFunction_Relationship(mayCommandParam),
                CmdPrefix + CmdUnMarry => () => CommandMethodes.CommandFunction_UnMarry(mayCommandParam),
                CmdPrefix + CmdMarryNpc => () => CommandMethodes.CommandFunction_MarryNPC(mayCommandParam),
                CmdPrefix + CmdSetSeason => () => CommandMethodes.CommandFunction_SetSeason(mayCommandParam),
                CmdPrefix + CmdIncDecYear => () => CommandMethodes.CommandFunction_Year(commandInput),
                //CmdPrefix + CmdFixYear => () => CommandMethodes.CommandFunction_ToggleYearFix(),

                // default: unknown command, no-op
                _ => () => { }
            };

            action();
        }
    }
}
