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
        //public const string CmdAutoFillMuseum = "autofillmuseum";
        //public const string CmdCheatFillMuseum = "cheatfillmuseum";
        //public const string CmdFixYear = "yearfix";
        public const string CmdCheats = "cheats";

        // npc relationship
        public const string CmdUnMarry = "divorce";
        public const string CmdMarryNpc = "marry";
        public const string CmdRelationship = "relationship";

        // items
        //public const string CmdGive = "give";
        //public const string CmdShowItems = "items";
        //public const string CmdPrintItemIds = "getitemids";
        //public const string CmdPrintHoverItem = "printhoveritem";
        //public const string CmdAppendItemDescWithId = "showid";

        // teleport
        public const string CmdTeleport = "tp";
        public const string CmdTeleportLocations = "tps";

        // pets
        //public const string CmdSpawnPet = "pet";
        //public const string CmdPetList = "pets";
        //public const string CmdDespawnPet = "despawnpet";

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
                new Command(CmdPrefix + CmdSleep,       "sleep to next the day", CommandState.None),
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
                //new Command(CmdPrefix + CmdGive,                    "give [ID] [AMOUNT]*",                                                      CommandState.None),
                //new Command(CmdPrefix + CmdShowItems,               "print items with the given name",                                          CommandState.None),
                //new Command(CmdPrefix + CmdPrintItemIds,            "print item ids [xp|money|all|bonus]",                                      CommandState.None),
                //new Command(CmdPrefix + CmdPrintHoverItem,          "print item id to chat",                                                    CommandState.Deactivated),
                //new Command(CmdPrefix + CmdAppendItemDescWithId,    "toggle id shown to item description",                                      CommandState.Deactivated),
                //new Command(CmdPrefix + CmdAutoFillMuseum,          "toggle museum's auto fill upon entry",                                     CommandState.Deactivated),
                //new Command(CmdPrefix + CmdCheatFillMuseum,         "toggle fill museum completely upon entry",                                 CommandState.Deactivated),
                new Command(CmdPrefix + CmdUI,                      "turn ui on/off", CommandState.None),
                new Command(CmdPrefix + CmdTeleport,                "teleport to some locations", CommandState.None),
                new Command(CmdPrefix + CmdTeleportLocations,       "get teleport locations", CommandState.None),
                //new Command(CmdPrefix + CmdDespawnPet,              "despawn current pet'",                                                     CommandState.None),
                //new Command(CmdPrefix + CmdSpawnPet,                "spawn a specific pet 'pet [name]'",                                        CommandState.None),
                //new Command(CmdPrefix + CmdPetList,                 "get the full list of pets '!pets'",                                        CommandState.None),
                new Command(CmdPrefix + CmdRelationship,            "'!relationship [name/all] [value] [add]*'", CommandState.None),
                new Command(CmdPrefix + CmdUnMarry,                 "unmarry an NPC '!divorce [name/all]'", CommandState.None),
                new Command(CmdPrefix + CmdMarryNpc,                "marry an NPC '!marry [name/all]'", CommandState.None),
                new Command(CmdPrefix + CmdSetSeason,               "change season", CommandState.None),
                //new Command(CmdPrefix + CmdFixYear,                 "fix year (if needed)",                                                     CommandState.Activated),
                new Command(CmdPrefix + CmdIncDecYear,              "add or sub years '!years [value]' '-' to sub", CommandState.None),
                new Command(CmdPrefix + CmdCheats,                  "Toggle Cheats and hotkeys like F7,F8", CommandState.Deactivated)
            };

            Array.Sort(value, (x, y) => x.Name.CompareTo(y.Name));
            return value;
        }

        public static void ProcessCommands(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            var action = mayCommandParam[0] switch
            {
                CmdHelp => (Action)(() => CommandMethodes.CommandFunction_Help()),
                _ => () => { } // default: do nothing
            };

            switch (mayCommandParam[0])
            {
                case CmdPrefix + CmdHelp:
                    CommandMethodes.CommandFunction_Help();
                    break;

                case CmdPrefix + CmdState:
                    CommandMethodes.CommandFunction_Help(true);
                    break;

                case CmdPrefix + CmdFeedbackDisabled:
                    CommandMethodes.CommandFunction_ToggleFeedback();
                    break;

                case CmdPrefix + CmdMineReset:
                    CommandMethodes.CommandFunction_ResetMines();
                    break;

                case CmdPrefix + CmdPause:
                    CommandMethodes.CommandFunction_Pause();
                    break;

                case CmdPrefix + CmdCustomDaySpeed:
                    CommandMethodes.CommandFunction_CustomDaySpeed(commandInput);
                    break;

                case CmdPrefix + CmdMoney:
                    CommandMethodes.CommandFunction_AddMoney(commandInput);
                    break;

                case CmdPrefix + CmdCoins:
                    CommandMethodes.CommandFunction_AddMoney(commandInput);
                    break;

                case CmdPrefix + CmdOrbs:
                    CommandMethodes.CommandFunction_AddOrbs(commandInput);
                    break;

                case CmdPrefix + CmdTickets:
                    CommandMethodes.CommandFunction_AddTickets(commandInput);
                    break;

                case CmdPrefix + CmdSetDate:
                    CommandMethodes.CommandFunction_ChangeDate(commandInput);
                    break;

                case CmdPrefix + CmdWeather:
                    CommandMethodes.CommandFunction_ChangeWeather(commandInput);
                    break;

                case CmdPrefix + CmdDevKit:
                    CommandMethodes.CommandFunction_GiveDevItems();
                    break;

                case CmdPrefix + CmdJumper:
                    CommandMethodes.CommandFunction_Jumper();
                    break;

                case CmdPrefix + CmdCheats:
                    CommandMethodes.CommandFunction_ToggleCheats();
                    break;

                case CmdPrefix + CmdSleep:
                    CommandMethodes.CommandFunction_Sleep();
                    break;

                case CmdPrefix + CmdDasher:
                    CommandMethodes.CommandFunction_InfiniteAirSkips();
                    break;

                case CmdPrefix + CmdManaFill:
                    CommandMethodes.CommandFunction_ManaFill();
                    break;

                case CmdPrefix + CmdManaInf:
                    CommandMethodes.CommandFunction_InfiniteMana();
                    break;

                case CmdPrefix + CmdHealthFill:
                    CommandMethodes.CommandFunction_HealthFill();
                    break;

                case CmdPrefix + CmdNoHit:
                    CommandMethodes.CommandFunction_NoHit();
                    break;

                case CmdPrefix + CmdMineOverfill:
                    CommandMethodes.CommandFunction_OverfillMines();
                    break;

                case CmdPrefix + CmdMineClear:
                    CommandMethodes.CommandFunction_ClearMines();
                    break;

                case CmdPrefix + CmdNoClip:
                    CommandMethodes.CommandFunction_NoClip();
                    break;

                //case CmdPrefix + CmdPrintHoverItem:
                //    CommandMethodes.CommandFunction_PrintItemIdOnHover();
                //    break;

                case CmdPrefix + CmdName:
                    CommandMethodes.CommandFunction_SetName(commandInput);
                    break;

                //case CmdPrefix + CmdGive:
                //    CommandMethodes.CommandFunction_GiveItemByIdOrName(mayCommandParam);
                //    break;

                //case CmdPrefix + CmdShowItems:
                //    CommandMethodes.CommandFunction_ShowItemByName(mayCommandParam);
                //    break;

                //case CmdPrefix + CmdPrintItemIds:
                //    CommandMethodes.CommandFunction_PrintItemIds(mayCommandParam);
                //    break;

                //case CmdPrefix + CmdAutoFillMuseum:
                //    CommandMethodes.CommandFunction_AutoFillMuseum();
                //    break;

                //case CmdPrefix + CmdCheatFillMuseum:
                //    CommandMethodes.CommandFunction_CheatFillMuseum();
                //    break;

                case CmdPrefix + CmdUI:
                    CommandMethodes.CommandFunction_ToggleUI(commandInput);
                    break;

                case CmdPrefix + CmdTeleport:
                    CommandMethodes.CommandFunction_TeleportToScene(mayCommandParam);
                    break;

                case CmdPrefix + CmdPrefix + CmdTeleportLocations:
                    CommandMethodes.CommandFunction_TeleportLocations();
                    break;

                //case CmdPrefix + CmdDespawnPet:
                //    CommandMethodes.CommandFunction_DespawnPet();
                //    break;

                //case CmdPrefix + CmdSpawnPet:
                //    CommandMethodes.CommandFunction_SpawnPet(mayCommandParam);
                //    break;

                //case CmdPrefix + CmdPetList:
                //    CommandMethodes.CommandFunction_GetPetList();
                //    break;

                //case CmdPrefix + CmdAppendItemDescWithId:
                //    CommandMethodes.CommandFunction_ShowID();
                //    break;

                case CmdPrefix + CmdRelationship:
                    CommandMethodes.CommandFunction_Relationship(mayCommandParam);
                    break;

                case CmdPrefix + CmdUnMarry:
                    CommandMethodes.CommandFunction_UnMarry(mayCommandParam);
                    break;

                case CmdPrefix + CmdMarryNpc:
                    CommandMethodes.CommandFunction_MarryNPC(mayCommandParam);
                    break;

                case CmdPrefix + CmdSetSeason:
                    CommandMethodes.CommandFunction_SetSeason(mayCommandParam);
                    break;

                //case CmdPrefix + CmdFixYear:
                //    CommandMethodes.CommandFunction_ToggleYearFix();
                //    break;

                case CmdPrefix + CmdIncDecYear:
                    CommandMethodes.CommandFunction_IncDecYear(commandInput);
                    break;

                default:
                    // TODO: notify the player about unknown command.
                    break;
            }
        }
    }
}
