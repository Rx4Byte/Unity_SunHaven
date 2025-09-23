using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using BepInEx;
using CommandExtension.Models;
using HarmonyLib;
using PSS;
using QFSW.QC;
using QFSW.QC.Utilities;
using UnityEngine;
using Wish;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using static UnityEngine.Random;

namespace CommandExtension
{
    public static class CommandMethodes
    {
        private const string gap = "  -  ";


        public static string playerNameForCommandsFirst;
        public static string playerNameForCommands;
        private static string lastScene;
        private static Vector2 lastLocation;

        private static Color Red = new Color(255, 0, 0);
        private static Color Green = new Color(0, 255, 0);
        private static Color Yellow = new Color(240, 240, 0);

        private static Dictionary<string, int> itemIds = (Dictionary<string, int>)typeof(Database).GetField("ids", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Database.Instance);
        private static Dictionary<string, int> currencyIds = new Dictionary<string, int>() { { "coins", 60000 }, { "orbs", 18010 }, { "tickets", 18011 } };
        private static Dictionary<string, int> expIds = new Dictionary<string, int>() { { "combatexp", 60003 }, { "farmingexp", 60004 }, { "miningexp", 60006 }, { "explorationexp", 60005 }, { "fishingexp", 60008 } };
        private static Dictionary<string, int> bonusIds = new Dictionary<string, int>() { { "health", 60009 }, { "mana", 60007 } };
        private static Dictionary<string, Dictionary<string, int>> categorizedItems = null;
        private static List<string> tpLocations = new List<string>() {
            "throneroom", "nelvari", "wishingwell", "altar", "hospital", "sunhaven", "sunhavenfarm/farm/home", "nelvarifarm", "nelvarimine", "nelvarihome",
            "withergatefarm", "castle", "withergatehome", "grandtree", "taxi", "dynus", "sewer", "nivara", "barracks", "elios", "dungeon", "store", "beach" };

        // Command states
        public static float timeMultiplier = CommandDefaultValues.timeMultiplier;
        public static bool jumpOver = Commands.GeneratedCommands[Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdJumper)].State == CommandState.Activated;
        public static bool cheatsOff = Commands.GeneratedCommands[Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdCheats)].State == CommandState.Activated;
        public static bool noclip = Commands.GeneratedCommands[Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdNoClip)].State == CommandState.Activated;
        public static bool infMana = Commands.GeneratedCommands[Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdManaInf)].State == CommandState.Activated;
        public static bool infAirSkips = Commands.GeneratedCommands[Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdDasher)].State == CommandState.Activated;
        public static bool printOnHover = Commands.GeneratedCommands[Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdPrintHoverItem)].State == CommandState.Activated;
        public static bool appendItemDescWithId = false;// Commands.GeneratedCommands[Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdAppendItemDescWithId)].State == CommandState.Activated;
        //public static bool yearFix = Commands.GeneratedCommands[Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdFixYear)].State == CommandState.Activated;
        public static bool uiVisible = true;

        struct CommandDefaultValues
        {
            public const float timeMultiplier = 0.2F;
        }

        public static Dictionary<int, ItemSellInfo> allItemSellInfos = ItemInfoDatabase.Instance.allItemSellInfos;
        public static bool CommandFunction_Debug(string[] mayCommandParam)
        {
            return true;
        }

        // HELP *
        public static bool CommandFunction_Help(bool status = false)
        {
            if (!status)
            {
                MessageToChat("[HELP]".ColorText(Color.black));
                foreach (Command command in Commands.GeneratedCommands)
                {
                    if (command.State == CommandState.Activated)
                        MessageToChat($"{command.Name.ColorText(Green)}{gap.ColorText(Color.black)}{command.Description.ColorText(new Color(1F, 0.66F, 0.0F))}");
                    if (command.State == CommandState.Deactivated)
                        MessageToChat($"{command.Name.ColorText(Red)}{gap.ColorText(Color.black)}{command.Description.ColorText(new Color(1F, 0.66F, 0.0F))}");
                    if (command.State == CommandState.None)
                        MessageToChat($"{command.Name.ColorText(Yellow)}{gap.ColorText(Color.black)}{command.Description.ColorText(new Color(1F, 0.66F, 0.0F))}");
                }
            }
            else
            {
                MessageToChat("[STATE]".ColorText(Color.black));
                foreach (Command command in Commands.GeneratedCommands)
                {
                    if (command.State == CommandState.Activated)
                        MessageToChat($"{command.Name.ColorText(Yellow)}{gap.ColorText(Color.black)}{(command.State.ToString().ColorText(Green))}");
                }
            }
            return true;
        }

        // PRINT SPECIAL ITEMS
        public static bool CommandFunction_PrintItemIds(string[] mayCommandParam)
        {
            if (categorizedItems == null)
            {
                if (!CategorizeItemList())
                {
                    MessageToChat($"Database Missing!".ColorText(Red));
                    return true;
                }
            }

            bool getItems = mayCommandParam.Length >= 3 && mayCommandParam[2] == "get";
            int amount = (mayCommandParam.Length >= 4 && int.TryParse(mayCommandParam[2], out amount)) ? amount : 1;

            switch ((mayCommandParam.Length >= 2) ? mayCommandParam[1] : "-")
            {
                case "xp":
                    MessageToChat("[Exp Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in expIds)
                    {
                        GiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "money":
                    MessageToChat("[Currency Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in currencyIds)
                    {
                        GiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                //case 'a':
                //    if (CategorizeItemList())
                //    {
                //        string file = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "\\itemIDs(CommandExtension).txt";
                //        if (!File.Exists(file))
                //        {
                //            using (File.Create(file))
                //            { }
                //        }
                //        bool isEmpty = true;
                //        File.WriteAllText(file, "");
                //        string overviewLine = new string('#', 80);
                //        foreach (var category in categorizedItems)
                //        {
                //            if (category.Value.Count >= 1)
                //            {
                //                if (isEmpty)
                //                {
                //                    File.AppendAllText(file, $"[{category.Key}]\n");
                //                    isEmpty = false;
                //                }
                //                else
                //                    File.AppendAllText(file, $"\n\n\n{overviewLine}\n[{category.Key}]\n");
                //                foreach (var item in category.Value.OrderBy(i => i.Key))
                //                {
                //                    File.AppendAllText(file, $"{item.Key} : {item.Value}\n");
                //                }
                //                File.AppendAllText(file, "");
                //            }
                //        }
                //        MessageToChat("ID list created inside your Sun Haven folder:".ColorText(Color.green));
                //        MessageToChat(file.ColorText(Color.white));
                //    }
                //    else
                //        CommandFunction_PrintToChat("ERROR: ".ColorText(Red) + "ItemDatabaseWrapper.ItemDatabase.ids".ColorText(Color.white) + " is empty!".ColorText(Red));
                //    break;

                case "bonus":
                    MessageToChat("[Bonus Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in bonusIds)
                    {
                        GiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "decoration":
                    MessageToChat("[Decoration Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["Decoration"])
                    {
                        GiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "armor":
                    MessageToChat("[Armor Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["Armor"])
                    {
                        GiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "tool":
                    MessageToChat("[Tool Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["Tool"])
                    {
                        GiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "food":
                    MessageToChat("[Food Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["Food"])
                    {
                        GiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "crop":
                    MessageToChat("[Crop Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["Crop"])
                    {
                        GiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "fish":
                    MessageToChat("[Fish Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["Fish"])
                    {
                        GiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "pet":
                    MessageToChat("[Pet Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["Pet"])
                    {
                        GiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "normal":
                    MessageToChat("[Normal Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["Normal"])
                    {
                        GiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                default:
                    MessageToChat(Commands.CmdPrintItemIds + " [xp|currency|bonus|pet|decoration|armor|tool|food|crop|fish|normal]".ColorText(Red));
                    return true;
            }

            return true;
        }

        // RESET MINES
        public static bool CommandFunction_ResetMines()
        {
            if (UnityEngine.Object.FindObjectOfType<MineGenerator2>() != null)
            {
                typeof(MineGenerator2).GetMethod("ResetMines", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(MineGenerator2.Instance, new object[] { (ushort)0 });
                MessageToChat($"Mine {"Reseted".ColorText(Green)}!".ColorText(Yellow));
            }
            else
                MessageToChat(("Must be inside a Mine!").ColorText(Red));
            return true;
        }

        // CLEAR MINES
        public static bool CommandFunction_ClearMines()
        {
            if (UnityEngine.Object.FindObjectOfType<MineGenerator2>() != null)
            {
                typeof(MineGenerator2).GetMethod("ClearMine", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(MineGenerator2.Instance, null);
                MessageToChat($"Mine {"Cleared".ColorText(Green)}!".ColorText(Yellow));
            }
            else
                MessageToChat(("Must be inside a Mine!").ColorText(Red));
            return true;
        }

        // OVERFILL MINES
        public static bool CommandFunction_OverfillMines()
        {
            if (UnityEngine.Object.FindObjectOfType<MineGenerator2>() != null)
            {
                for (int i = 0; i < 30; i++)
                {
                    typeof(MineGenerator2).GetMethod("GenerateRocks", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(MineGenerator2.Instance, null);
                }
                MessageToChat($"Mine {"Overfilled".ColorText(Green)}!".ColorText(Yellow));
            }
            else
                MessageToChat(("Must be inside a Mine!").ColorText(Red));
            return true;
        }

        // ADD MONEY/COINS
        public static bool CommandFunction_AddMoney(string mayCommand)
        {
            if (!int.TryParse(Regex.Match(mayCommand, @"\d+").Value, out int moneyAmount))
            {
                MessageToChat("Something wen't wrong..".ColorText(Red));
                MessageToChat("Try '!money 500' or '!coins 500'".ColorText(Red));
                return true;
            }
            if (mayCommand.Contains("-"))
            {
                GetPlayerForCommand().AddMoney(-moneyAmount, true, true, true);
                MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} paid {moneyAmount.ToString().ColorText(Color.white)} Coins!".ColorText(Yellow));
            }
            else
            {
                GetPlayerForCommand().AddMoney(moneyAmount, true, true, true);
                MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} got {moneyAmount.ToString().ColorText(Color.white)} Coins!".ColorText(Yellow));
            }
            return true;
        }

        // ADD MONEY
        public static bool CommandFunction_AddOrbs(string mayCommand)
        {
            if (!int.TryParse(Regex.Match(mayCommand, @"\d+").Value, out int moneyAmount))
            {
                MessageToChat("Something wen't wrong..".ColorText(Red));
                MessageToChat("Try '!orbs 500'".ColorText(Red));
                return true;
            }
            if (mayCommand.Contains("-"))
            {
                GetPlayerForCommand().AddOrbs(-moneyAmount);
                MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} paid {moneyAmount.ToString().ColorText(Color.white)} Orbs!".ColorText(Yellow));
            }
            else
            {
                GetPlayerForCommand().AddOrbs(moneyAmount);
                MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} got {moneyAmount.ToString().ColorText(Color.white)} Orbs!".ColorText(Yellow));
            }
            return true;
        }

        // ADD MONEY
        public static bool CommandFunction_AddTickets(string mayCommand)
        {
            if (!int.TryParse(Regex.Match(mayCommand, @"\d+").Value, out int moneyAmount))
            {
                MessageToChat("Something wen't wrong..".ColorText(Red));
                MessageToChat("Try '!tickets 500'".ColorText(Red));
                return true;
            }
            if (mayCommand.Contains("-"))
            {
                GetPlayerForCommand().AddTickets(-moneyAmount);
                MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} paid {moneyAmount.ToString().ColorText(Color.white)} Tickets!".ColorText(Yellow));
            }
            else
            {
                GetPlayerForCommand().AddTickets(moneyAmount);
                MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} got {moneyAmount.ToString().ColorText(Color.white)} Tickets!".ColorText(Yellow));
            }
            return true;
        }

        // SET NAME
        public static bool CommandFunction_SetName(string mayCommand)
        {
            string[] mayCommandParam = mayCommand.Split(' ');
            if (mayCommand.Length <= Commands.CmdName.Length + 1)
            {
                playerNameForCommands = playerNameForCommandsFirst;
                MessageToChat($"Command target name {"reseted".ColorText(Green)} to {playerNameForCommandsFirst.ColorText(Color.magenta)}!".ColorText(Yellow));
            }
            else if (mayCommandParam.Length >= 2)
            {
                playerNameForCommands = mayCommandParam[1];
                MessageToChat($"Command target name changed to {mayCommandParam[1].ColorText(Color.magenta)}!".ColorText(Color.yellow));
            }
            return true;
        }

        // CHANGE DATE (only hour and day!)
        // TODO: split into day command and hour command, rename years command to year
        public static bool CommandFunction_ChangeDate(string mayCommand)
        {
            string[] mayCommandParam = mayCommand.ToLower().Split(' ');
            if (mayCommandParam.Length == 3)
            {
                var Date = DayCycle.Instance;
                if (int.TryParse(mayCommandParam[2], out int dateValue))
                {
                    switch (mayCommandParam[1][0])
                    {
                        case 'd':
                            if (dateValue <= 0 || dateValue > 28)
                            { MessageToChat($"day {"1-28".ColorText(Color.white)} are allowed".ColorText(Red)); return true; }
                            Date.Time = new DateTime(Date.Time.Year, Date.Time.Month, dateValue, Date.Time.Hour + 1, Date.Time.Minute, Date.Time.Second, Date.Time.Millisecond);
                            typeof(DayCycle).GetMethod("SetInitialTime", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DayCycle.Instance, null);
                            MessageToChat($"{"Day".ColorText(Green)} set to {dateValue.ToString().ColorText(Color.white)}!".ColorText(Yellow));
                            break;
                        case 'h':
                            if (dateValue < 6 || dateValue > 23) // 6-23 
                            { MessageToChat($"hour {"6-23".ColorText(Color.white)} are allowed".ColorText(Red)); return true; }
                            Date.Time = new DateTime(Date.Time.Year, Date.Time.Month, Date.Time.Day, dateValue + 1, Date.Time.Minute, Date.Time.Second, Date.Time.Millisecond);
                            MessageToChat($"{"Hour".ColorText(Green)} set to {dateValue.ToString().ColorText(Color.white)}!".ColorText(Yellow));
                            break;
                    }
                }
            }
            return true;
        }

        // CHANGE WEATHER
        public static bool CommandFunction_ChangeWeather(string mayCommand)
        {
            string[] mayCommandParam = mayCommand.ToLower().Split(' ');
            if (mayCommandParam.Length >= 2)
            {
                var Date = DayCycle.Instance;
                switch (mayCommandParam[1][0])
                {
                    case 'r': // rain toggle
                        Date.SetToRaining(Date.Raining ? false : true);
                        MessageToChat($"{"Raining".ColorText(Green)} turned {(!Date.Raining ? "Off".ColorText(Red) : "On".ColorText(Green))}!".ColorText(Yellow));
                        break;
                    case 'h': // heatwave toggle
                        Date.SetToHeatWave(Date.Heatwave ? false : true);
                        MessageToChat($"{"Heatwave".ColorText(Green)} turned {(!Date.Heatwave ? "Off".ColorText(Red) : "On".ColorText(Green))}!".ColorText(Yellow));
                        break;
                    case 'c': // clear both
                        Date.SetToHeatWave(false);
                        Date.SetToRaining(false);
                        MessageToChat($"{"Sunny".ColorText(Green)} weather turned {"On".ColorText(Green)}!".ColorText(Yellow));
                        break;
                }
            }
            return true;
        }

        // GIVE ITEM BY ID/NAME 
        public static bool CommandFunction_GiveItemByIdOrName(string[] mayCommandParam)
        {
            if (mayCommandParam.Length >= 2)
            {
                if (int.TryParse(mayCommandParam[1], out int itemId))
                {
                    if (itemIds.Values.Contains(itemId))
                    {
                        int itemAmount = (mayCommandParam.Length >= 3 && int.TryParse(mayCommandParam[2], out itemAmount)) ? itemAmount : 1;
                        GetPlayerForCommand().Inventory.AddItem(itemId, itemAmount, 0, true, true);
                        string itemName = "unknown";
                        Database.GetData(itemId, delegate (ItemData data) 
                        {
                            itemName = data.Name;
                        });
                        MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} got {itemAmount.ToString().ColorText(Color.white)} x {itemName.ColorText(Color.white)}!".ColorText(Yellow));
                    }
                    else
                    {
                        MessageToChat($"no item with id: {itemId.ToString().ColorText(Color.white)} found!".ColorText(Red));
                    }
                }
                else
                {
                    int itemsFound = 0;
                    int lastItemId = 0;
                    if (mayCommandParam.Length >= 2)
                    {
                        foreach (KeyValuePair<string, int> id in itemIds)
                        {
                            if (id.Key.Contains(mayCommandParam[1].ToLower()))
                            {
                                lastItemId = id.Value;
                                itemsFound++;
                                if (mayCommandParam[1] == id.Key)
                                {
                                    int itemAmount = ((mayCommandParam.Length >= 3 && int.TryParse(mayCommandParam[2], out itemAmount)) ? itemAmount : 1);
                                    GetPlayerForCommand().Inventory.AddItem(lastItemId, itemAmount, 0, true, true);
                                    return true;
                                }
                            }
                        }

                        if (itemsFound == 1)
                        {
                            int itemAmount = ((mayCommandParam.Length >= 3 && int.TryParse(mayCommandParam[2], out itemAmount)) ? itemAmount : 1);
                            GetPlayerForCommand().Inventory.AddItem(lastItemId, itemAmount, 0, true, true);
                        }
                        else if (itemsFound > 1)
                        {
                            MessageToChat("[Items Containing: ]".ColorText(new Color(0.1f, 0.1f, 0.1f)) + mayCommandParam[1].ColorText(new Color(0.26f, 0.26f, 0.26f)));
                            MessageToChat("use a unique item-name or id".ColorText(Color.red));
                            foreach (KeyValuePair<string, int> id in itemIds)
                            {
                                if (id.Key.Contains(mayCommandParam[1]))
                                {
                                    MessageToChat(id.Key + " : ".ColorText(Color.black) + id.Value.ToString());
                                }
                            }
                        }
                        else
                        {
                            MessageToChat($"no item name contains {mayCommandParam[1].ColorText(Color.white)}!".ColorText(Red));
                        }
                    }
                    else
                    {
                        MessageToChat($"invalid Item ID!".ColorText(Red));
                    }
                    return true;
                }
            }
            else
                MessageToChat($"wrong use of !give".ColorText(Red));
            return true;
        }

        // GIVE ITEM BY ID/NAME 
        public static bool CommandFunction_ShowItemByName(string[] mayCommandParam)
        {
            if (mayCommandParam.Length >= 2)
            {
                List<string> items = new List<string>();
                foreach (KeyValuePair<string, int> id in itemIds)
                {
                    if (id.Key.ToLower().Contains(mayCommandParam[1]))
                    {
                        items.Add(id.Key.ColorText(Color.white) + " : ".ColorText(Color.black) + id.Value.ToString().ColorText(Color.white));
                    }
                }
                if (items.Count >= 1)
                {
                    MessageToChat($"[Items with: {mayCommandParam[1].ColorText(new Color(0.26f, 0.26f, 0.26f))}]".ColorText(new Color(0.1f, 0.1f, 0.1f)));
                    foreach (string ítem in items)
                    {
                        MessageToChat(ítem);
                    }
                }
            }
            return true;
        }

        // GIVE DEV ITEMS
        public static bool CommandFunction_GiveDevItems()
        {
            foreach (int item in new int[] { 30003, 30004, 30005, 30006, 30007, 30008 })
                GetPlayerForCommand().Inventory.AddItem(item, 1, 0, true);
            MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} got a {"DevKit".ColorText(Color.white)}".ColorText(Yellow));
            return true;
        }

        // INFINITE AIR-SKIPS
        public static bool CommandFunction_InfiniteAirSkips()
        {
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdDasher);
            Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            bool flag = infAirSkips = Commands.GeneratedCommands[i].State == CommandState.Activated;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // SHOW ID
        public static bool CommandFunction_ShowID()
        {
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdAppendItemDescWithId);
            Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            bool flag = appendItemDescWithId = Commands.GeneratedCommands[i].State == CommandState.Activated;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // FILL MANA
        public static bool CommandFunction_ManaFill()
        {
            var player = GetPlayerForCommand();
            player.AddMana(player.MaxMana, 1f);
            MessageToChat(playerNameForCommands.ColorText(Color.magenta) + "'s Mana Refilled".ColorText(Yellow));
            return true;
        }

        // INFINITE MANA
        public static bool CommandFunction_InfiniteMana()
        {
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdManaInf);
            Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            bool flag = infMana = Commands.GeneratedCommands[i].State == CommandState.Activated;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // FILL HEALTH
        public static bool CommandFunction_HealthFill()
        {
            var player = GetPlayerForCommand();
            player.Heal(player.MaxMana, true, 1f);
            MessageToChat(playerNameForCommands.ColorText(Color.magenta) + "'s Health Refilled".ColorText(Yellow));
            return true;
        }

        // SLEEP
        public static bool CommandFunction_Sleep()
        {
            GetPlayerForCommand().SkipSleep();
            MessageToChat($"{"Slept".ColorText(Green)} once! Another Day is a Good Day!".ColorText(Yellow));
            return true;
        }

        // PRINT ID ON HOVER
        public static bool CommandFunction_PrintItemIdOnHover()
        {
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdPrintHoverItem);
            Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            bool flag = Commands.GeneratedCommands[i].State == CommandState.Activated;
            printOnHover = flag;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // TOGGLE COMMAND FEEDBACK
        public static bool CommandFunction_ToggleFeedback()
        {
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdFeedbackDisabled);
            Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            bool flag = Commands.GeneratedCommands[i].State == CommandState.Activated;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // PAUSE
        public static bool CommandFunction_Pause()
        {
            // get Command values
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdPause);
            Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            bool flag = Commands.GeneratedCommands[i].State == CommandState.Activated;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // SLOW TIME
        public static bool CommandFunction_CustomDaySpeed(string mayCommand)
        {
            string[] mayCommandParam = mayCommand.Split(' ');
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdCustomDaySpeed);
            if (mayCommandParam.Length >= 2)
            {
                if (int.TryParse(mayCommandParam[1], out int value))
                {
                    Commands.GeneratedCommands[i].State = CommandState.Activated;
                    timeMultiplier = (float)System.Math.Round(20f / value, 4);
                    MessageToChat($"Custom Dayspeed {"Activated".ColorText(Green)} and {"changed".ColorText(Green)}! multiplier: {timeMultiplier.ToString().ColorText(Color.white)}".ColorText(Yellow));
                    return true;
                }
                else if (mayCommandParam[1].Contains("r") || mayCommandParam[1].Contains("d")) // r = reset | d = default
                {
                    timeMultiplier = CommandDefaultValues.timeMultiplier;
                    Commands.GeneratedCommands[i].State = CommandState.Activated;
                    MessageToChat($"Custom Dayspeed {"Activated".ColorText(Green)} and {"reseted".ColorText(Green)}! multiplier: {timeMultiplier.ToString().ColorText(Color.white)}".ColorText(Yellow));
                    return true;
                }
            }
            else
                Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            bool flag = Commands.GeneratedCommands[i].State == CommandState.Activated;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // UI ON/OFF
        public static bool CommandFunction_ToggleUI(string mayCommand)
        {
            string[] mayCommandParam = mayCommand.Split(' ');
            if (mayCommandParam.Length >= 1)
            {
                bool flag = true;
                if (mayCommandParam.Length >= 2)
                {
                    if (mayCommandParam[1].Contains("on"))
                    {
                        flag = true;
                    }
                    else if (mayCommandParam[1].Contains("of"))
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = uiVisible = !uiVisible;
                }

                var values = new (string rootName, string childName)[]
                {
                    ("Player",        "ActionBar"),
                    ("Player(Clone)", "ActionBar"),
                    ("Player",        "ExpBars"),
                    ("Player(Clone)", "ExpBars"),
                    ("Player",        "QuestTracking"),
                    ("Player(Clone)", "QuestTracking"),
                    ("Player",        "QuestTracker"),
                    ("Player(Clone)", "QuestTracker"),
                    ("Player",        "HelpNotifications"),
                    ("Player(Clone)", "HelpNotifications"),
                    ("Player",        "NotificationStack"),
                    ("Player(Clone)", "NotificationStack"),
                    ("Manager",       "UI")
                };

                for (int i = 0; i < values.Length; i++)
                {
                    MessageToChat("5".ColorText(Color.white) + ".".ColorText(Color.white) + i.ToString().ColorText(Color.white));
                    GameObject obj = Utilities.FindObject(GameObject.Find(values[i].rootName), values[i].childName);
                    GameObjectSetActiveState(obj, flag);
                }

                GameObject obj2 = GameObject.Find("QuestTrackerVisibilityToggle");
                GameObjectSetActiveState(obj2, flag);
                //GameObject gameObject = Utilities.FindObject(GameObject.Find("Player"), "ActionBar");
                //if (gameObject != null)
                //{
                //    gameObject.SetActive(flag);
                //}
                //GameObject gameObject2 = Utilities.FindObject(GameObject.Find("Player(Clone)"), "ActionBar");
                //if (gameObject2 != null)
                //{
                //    gameObject2.SetActive(flag);
                //}
                //GameObject gameObject3 = Utilities.FindObject(GameObject.Find("Player"), "ExpBars");
                //if (gameObject3 != null)
                //{
                //    gameObject3.SetActive(flag);
                //}
                //GameObject gameObject4 = Utilities.FindObject(GameObject.Find("Player(Clone)"), "ExpBars");
                //if (gameObject4 != null)
                //{
                //    gameObject4.SetActive(flag);
                //}
                //GameObject gameObject5 = Utilities.FindObject(GameObject.Find("Player"), "QuestTracking");
                //if (gameObject5 != null)
                //{
                //    gameObject5.SetActive(flag);
                //}
                //GameObject gameObject6 = Utilities.FindObject(GameObject.Find("Player(Clone)"), "QuestTracking");
                //if (gameObject6 != null)
                //{
                //    gameObject6.SetActive(flag);
                //}
                //GameObject gameObject7 = Utilities.FindObject(GameObject.Find("Player"), "QuestTracker");
                //if (gameObject7 != null)
                //{
                //    gameObject7.SetActive(flag);
                //}
                //GameObject gameObject8 = Utilities.FindObject(GameObject.Find("Player(Clone)"), "QuestTracker");
                //if (gameObject8 != null)
                //{
                //    gameObject8.SetActive(flag);
                //}
                //GameObject gameObject9 = Utilities.FindObject(GameObject.Find("Player"), "HelpNotifications");
                //if (gameObject9 != null)
                //{
                //    gameObject9.SetActive(flag);
                //}
                //GameObject gameObject10 = Utilities.FindObject(GameObject.Find("Player(Clone)"), "HelpNotifications");
                //if (gameObject10 != null)
                //{
                //    gameObject10.SetActive(flag);
                //}
                //GameObject gameObject11 = Utilities.FindObject(GameObject.Find("Player"), "NotificationStack");
                //if (gameObject11 != null)
                //{
                //    gameObject11.SetActive(flag);
                //}
                //GameObject gameObject12 = Utilities.FindObject(GameObject.Find("Player(Clone)"), "NotificationStack");
                //if (gameObject12 != null)
                //{
                //    gameObject12.SetActive(flag);
                //}
                //GameObject gameObject13 = Utilities.FindObject(GameObject.Find("Manager"), "UI");
                //if (gameObject13 != null)
                //{
                //    gameObject13.SetActive(flag);
                //}
                //GameObject gameObject14 = GameObject.Find("QuestTrackerVisibilityToggle");
                //if (gameObject14 != null)
                //{
                //    gameObject14.SetActive(flag);
                //}

                MessageToChat("UI now ".ColorText(Yellow) + (flag ? "Visible".ColorText(Green) : "Hidden".ColorText(Green)));
            }
            return true;
        }

        // JUMPER
        public static bool CommandFunction_Jumper()
        {
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdJumper);
            Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            bool flag = Commands.GeneratedCommands[i].State == CommandState.Activated;
            jumpOver = flag;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // NOCLIP
        public static bool CommandFunction_NoClip()
        {
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdNoClip);
            Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            GetPlayerForCommand().rigidbody.bodyType = Commands.GeneratedCommands[i].State == CommandState.Activated ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
            bool flag = Commands.GeneratedCommands[i].State == CommandState.Activated;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // NO HIT
        public static bool CommandFunction_NoHit()
        {
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdNoHit);
            Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            bool flag = Commands.GeneratedCommands[i].State == CommandState.Activated;
            GetPlayerForCommand().Invincible = flag;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // AUTO-FILL MUSEUM
        public static bool CommandFunction_AutoFillMuseum()
        {
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdAutoFillMuseum);
            Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            bool flag = Commands.GeneratedCommands[i].State == CommandState.Activated;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // CHEAT-FILL MUSEUM
        public static bool CommandFunction_CheatFillMuseum()
        {
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdCheatFillMuseum);
            Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            bool flag = Commands.GeneratedCommands[i].State == CommandState.Activated;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // TELEPORT
        public static bool CommandFunction_TeleportToScene(string[] mayCmdParam)
        {
            if (mayCmdParam.Length <= 1)
                return true;
            string scene = mayCmdParam[1];
            if (scene == "withergatefarm")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(138f, 89.16582f), "WithergateRooftopFarm");
            }
            else if (scene == "throneroom")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(21.5f, 8.681581f), "Throneroom");
            }
            else if (scene == "nelvari")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(320.3333f, 98.76098f), "Nelvari6");
            }
            else if (scene == "wishingwell")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(55.83683f, 61.48384f), "WishingWell");
            }
            else if (scene.Contains("altar"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(199.3957f, 122.6284f), "DynusAltar");
            }
            else if (scene.Contains("hospital"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(80.83334f, 65.58415f), "Hospital");
            }
            else if (scene.Contains("sunhaven"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(268.125f, 299.9311f), "Town10");
            }
            else if (scene.Contains("nelvarifarm"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(139.6753f, 100.4739f), "NelvariFarm");
            }
            else if (scene.Contains("nelvarimine"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(144.7133f, 152.1591f), "NelvariMinesEntrance");
            }
            else if (scene.Contains("nelvarihome"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(51.5f, 54.97755f), "NelvariPlayerHouse");
            }
            else if (scene.Contains("castle"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(133.7634f, 229.2485f), "Withergatecastleentrance");
            }
            else if (scene.Contains("withergatehome"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(63.5f, 54.624f), "WithergatePlayerApartment");
            }
            else if (scene.Contains("grandtree"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(314.4297f, 235.2298f), "GrandTreeEntrance1");
            }
            else if (scene.Contains("taxi"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(101.707f, 123.4622f), "WildernessTaxi");
            }
            else if (scene == "dynus")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(94.5f, 121.09f), "Dynus");
            }
            else if (scene == "sewer")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(134.5833f, 129.813f), "Sewer");
            }
            else if (scene == "nivara")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(99f, 266.6305f), "Nivara");
            }
            else if (scene.Contains("barrack"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(71.58334f, 54.56507f), "Barracks");
            }
            else if (scene.Contains("elios"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(113.9856f, 104.2902f), "DragonsMeet");
            }
            else if (scene.Contains("dungeon"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(136.48f, 193.92f), "CombatDungeonEntrance");
            }
            else if (scene.Contains("store"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(77.5f, 58.55f), "GeneralStore");
            }
            else if (scene.Contains("beach"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(96.491529f, 87.46871f), "BeachRevamp");
            }
            else if (scene == "home" || scene.Contains("sunhavenhome") || scene == "farm")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(316.4159f, 152.5824f), "2Playerfarm");
            }
            else if (scene == "back")
                ScenePortalManager.Instance.ChangeScene(lastLocation, lastScene);
            else
                MessageToChat("invalid scene name".ColorText(Color.red));

            return true;
        }

        // GET TELEPORT LOCATIONS
        public static bool CommandFunction_TeleportLocations()
        {
            MessageToChat("[Locations]".ColorText(Color.black));
            foreach (string tpLocation in tpLocations)
            {
                MessageToChat(tpLocation.ColorText(Color.white));
            }

            return true;
        }

        // SET RELATIONSHIP
        public static bool CommandFunction_Relationship(string[] mayCmdParam)
        {
            if (mayCmdParam.Length >= 3)
            {
                string name = mayCmdParam[1];
                bool all = mayCmdParam[1] == "all";
                bool add = mayCmdParam.Length >= 4 && mayCmdParam[3] == "add";

                float value;
                if (float.TryParse(mayCmdParam[2], out value))
                {
                    value = Math.Max(0, Math.Min(100, value));
                    NPCAI[] npcs = UnityEngine.Object.FindObjectsOfType<NPCAI>();
                    foreach (NPCAI npc in npcs)
                    {
                        if (all || npc.NPCName.ToLower() == name)
                        {
                            if (add)
                            {
                                npc.AddRelationship(value);
                                if (!all)
                                {
                                    MessageToChat($"Relationship with {npc.NPCName.ColorText(Color.white)} is now {SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships[npc.NPCName].ToString().ColorText(Color.white)}!".ColorText(Green));
                                    return true;
                                }
                            }
                            else
                            {
                                SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships[npc.NPCName] = value;
                                if (!all)
                                {
                                    MessageToChat($"Relationship with {npc.NPCName.ColorText(Color.white)} set to {value.ToString().ColorText(Color.white)}!".ColorText(Green));
                                    return true;
                                }
                            }
                        }
                    }
                    if (all)
                        MessageToChat(add ? "Relationships increased!".ColorText(Green) : "Relationships set!".ColorText(Green));
                    else
                        MessageToChat($"No NPC with the name {name.ColorText(Color.white)} found!".ColorText(Red));
                }
                else
                    MessageToChat($"NO VALID VALUE, try '{$"!relationship {name.ColorText(Color.white)} 10".ColorText(Color.white)}'".ColorText(Red));
            }
            return true;
        }

        // UN-MARRY NPC
        public static bool CommandFunction_UnMarry(string[] mayCmdParam)
        {
            if (mayCmdParam.Length >= 2)
            {
                bool all = mayCmdParam[1] == "all";
                string name = FirstCharToUpper(mayCmdParam[1]);
                NPCAI[] npcs = UnityEngine.Object.FindObjectsOfType<NPCAI>();

                if (npcs == null || npcs.Length == 0)
                {
                    return true;
                }

                if (all)
                {
                    foreach (NPCAI npc in npcs)
                    {
                        Marry(npcai: npc, unmarry: true, notify: true);
                    }
                }
                else
                {
                    NPCAI npc = npcs.FirstOrDefault(npc => GetNpcName(npc.NPCName) == name);
                    if (npc != null)
                    {
                        Marry(npcai: npc, unmarry: true, notify: true);
                    }
                    else
                    {
                        MessageToChat("no npc with the name: ".ColorText(Red) + name.ColorText(Yellow));
                    }
                }
            }
            else
            {
                MessageToChat("a name or parameter 'all' needed");
            }

            return true;
        }

        // MARRY NPC
        public static bool CommandFunction_MarryNPC(string[] mayCmdParam)
        {
            if (mayCmdParam.Length >= 2)
            {
                bool all = mayCmdParam[1] == "all";
                string name = FirstCharToUpper(mayCmdParam[1]);
                NPCAI[] npcs = UnityEngine.Object.FindObjectsOfType<NPCAI>();

                if (npcs == null || npcs.Length == 0)
                {
                    return true;
                }

                if (all)
                {
                    foreach (NPCAI npc in npcs)
                    {
                        Marry(npcai: npc, unmarry: false, notify: true);
                    }
                }
                else
                {
                    NPCAI npc = npcs.FirstOrDefault(npc => GetNpcName(npc.NPCName) == name);
                    if (npc != null)
                    {
                        Marry(npcai: npc, unmarry: false, notify: true);
                    }
                    else
                    {
                        MessageToChat("no npc with the name: ".ColorText(Red) + name.ColorText(Yellow));
                    }
                }
            }
            else
            {
                MessageToChat("a name or parameter 'all' needed");
            }
            return true;
        }

        // SET SEASON
        public static bool CommandFunction_SetSeason(string[] mayCmdParam)
        {
            if (mayCmdParam.Length < 2)
            {
                MessageToChat("specify the season!".ColorText(Red)); return true;
            }
            if (!Enum.TryParse(mayCmdParam[1], true, out Season season2))
            {
                MessageToChat("no valid season!".ColorText(Red)); return true;
            }
            var Date = DayCycle.Instance;
            int targetYear = Date.Time.Year + ((int)season2 - (int)Date.Season + 4) % 4;

            DayCycle.Instance.Time = new DateTime(targetYear, Date.Time.Month, 1, Date.Time.Hour, Date.Time.Minute, 0, DateTimeKind.Utc).ToUniversalTime();
            typeof(DayCycle).GetMethod("SetInitialTime", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DayCycle.Instance, null);

            MessageToChat("It's now ".ColorText(Yellow) + season2.ToString().ColorText(Green));
            //DayCycle.Instance.SetSeason(season2);
            return true;
        }

        // YEAR FIX
        public static bool CommandFunction_ToggleYearFix()
        {
            //int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdFixYear);
            //Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            //bool flag = Commands.GeneratedCommands[i].State == CommandState.Activated;
            //yearFix = flag;
            //CommandFunction_PrintToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }

        // CHANGE YEAR
        public static bool CommandFunction_Year(string mayCommand)
        {
            if (!int.TryParse(Regex.Match(mayCommand, @"\d+").Value, out int value))
            {
                MessageToChat("Something wen't wrong..".ColorText(Red));
                MessageToChat("Try '!years 1' or '!years -1'".ColorText(Red));
                return true;
            }
            var Date = DayCycle.Instance;
            int newYear;
            if (mayCommand.Contains("-"))
            {
                if (Date.Time.Year - (value * 4) >= 1)
                    newYear = Date.Time.Year - (value * 4);
                else
                {
                    MessageToChat("must be in year 1 or above");
                    return true;
                }
            }
            else
                newYear = Date.Time.Year + (value * 4);

            DayCycle.Instance.Time = new DateTime(newYear, Date.Time.Month, Date.Time.Day, Date.Time.Hour, Date.Time.Minute, 0, DateTimeKind.Utc).ToUniversalTime();
            typeof(DayCycle).GetMethod("SetInitialTime", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DayCycle.Instance, null);
            MessageToChat($"It's now Year {(Date.Time.Year / 4).ToString().ColorText(Green)}!".ColorText(Yellow));
            return true;
        }

        // TOGGLE CHEATS
        public static bool CommandFunction_ToggleCheats()
        {
            int i = Array.FindIndex(Commands.GeneratedCommands, command => command.Name == Commands.CmdPrefix + Commands.CmdCheats);
            Commands.GeneratedCommands[i].State = Commands.GeneratedCommands[i].State == CommandState.Activated ? CommandState.Deactivated : CommandState.Activated;
            bool flag = Commands.GeneratedCommands[i].State == CommandState.Activated;
            Settings.EnableCheats = flag;
            MessageToChat($"{Commands.GeneratedCommands[i].Name} {Commands.GeneratedCommands[i].State.ToString().ColorText(flag ? Green : Red)}".ColorText(Yellow));
            return true;
        }


        // ============================================================
        // Utility
        // ============================================================

        private static void GiveOrPrintItem(int id, int amount, bool give = false)
        {
            //Commands.CmdPrintItemIds + " [xp|currency|bonus|pet|decoration|armor|tool|food|crop|fish|normal]"
            if (give)
            {
                GetPlayerForCommand().Inventory.AddItem(id, amount, 0, true, true);
            }
            else
            {
                MessageToChat($"{amount} : {id}");
            }
        }
        public static void MessageToChat(string text)
        {
            if (Commands.GeneratedCommands.First(command => command.Name == Commands.CmdPrefix + Commands.CmdFeedbackDisabled).State == CommandState.Deactivated)
            {
                QuantumConsole.Instance.LogPlayerText(text);
            }
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        private static void GameObjectSetActiveState(GameObject obj, bool state)
        {
            obj?.SetActive(state);
        }

        private static void Marry(NPCAI npcai, bool unmarry = false, bool notify = false)
        {
            string npcName = npcai.NPCName;
            if (string.IsNullOrWhiteSpace(npcName) || !SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships.ContainsKey(npcai.NPCName))
            {
                return;
            }
            npcai.MarryPlayer();

            if (!unmarry)
            {
                if (SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships.ContainsKey(npcName))
                {
                    SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships[npcName] = 100f;
                }

                npcai.MarryPlayer();
                MessageToChat($"Married with {npcName.ColorText(Color.white)}!".ColorText(Green));
            }
            else
            {
                string progressStringCharacter = SingletonBehaviour<GameSave>.Instance.GetProgressStringCharacter("MarriedWith");
                if (!progressStringCharacter.IsNullOrWhiteSpace())
                {
                    SingletonBehaviour<GameSave>.Instance.SetProgressStringCharacter("MarriedWith", "");
                    SingletonBehaviour<GameSave>.Instance.SetProgressBoolCharacter("Married", false);
                    SingletonBehaviour<GameSave>.Instance.SetProgressBoolCharacter("MarriedTo" + progressStringCharacter, false);
                    GameSave.CurrentCharacter.Relationships[progressStringCharacter] = 40f;
                    SingletonBehaviour<NPCManager>.Instance.GetRealNPC(progressStringCharacter).GenerateCycle(false);
                }

                MessageToChat($"Divorced from {npcName.ColorText(Color.white)}!".ColorText(Green));
            }
        }
        private static readonly Regex npcNameRegex = new Regex(@"[a-zA-Z\s\.]+", RegexOptions.Compiled);
        private static string GetNpcName(string name)
        {
            var match = npcNameRegex.Match(name);
            return match.Success ? match.Value : string.Empty;
        }

        // get the player for singleplayer/multiplayer
        public static Player GetPlayerForCommand()
        {
            Player[] players = UnityEngine.Object.FindObjectsOfType<Player>();
            foreach (Player player in players)
            {
                if (players.Length == 1 || player.GetComponent<NetworkGamePlayer>().playerName == playerNameForCommands)
                    return player;
            }
            return null;
        }

        // Categorize all items
        private static bool CategorizeItemList()
        {
            if (itemIds == null || itemIds.Count < 1)
            {
                return false;
            }

            categorizedItems = new Dictionary<string, Dictionary<string, int>>() {
                { "Decoration", new Dictionary<string, int>() },
                { "Normal", new Dictionary<string, int>() },
                { "Armor", new Dictionary<string, int>() },
                { "Tool", new Dictionary<string, int>() },
                { "Food", new Dictionary<string, int>() },
                { "Crop", new Dictionary<string, int>() },
                { "Fish", new Dictionary<string, int>() },
                { "Pet", new Dictionary<string, int>() }};

            foreach (var item in itemIds)
            {
                if (ItemInfoDatabase.Instance.allItemSellInfos.TryGetValue(item.Value, out ItemSellInfo itemInfo))
                {
                    if (itemInfo.decorationType != DecorationType.None)
                    {
                        categorizedItems["Decoration"].Add(item.Key, item.Value);
                    }
                    else if (itemInfo.itemType == ItemType.Armor)
                    {
                        categorizedItems["Armor"].Add(item.Key, item.Value);
                    }
                    else if (itemInfo.itemType == ItemType.Tool || itemInfo.itemType == ItemType.WateringCan)
                    {
                        categorizedItems["Tool"].Add(item.Key, item.Value);
                    }
                    else if (itemInfo.itemType == ItemType.Food)
                    {
                        categorizedItems["Food"].Add(item.Key, item.Value);
                    }
                    else if (itemInfo.itemType == ItemType.Crop)
                    {
                        categorizedItems["Crop"].Add(item.Key, item.Value);
                    }
                    else if (itemInfo.itemType == ItemType.Fish)
                    {
                        categorizedItems["Fish"].Add(item.Key, item.Value);
                    }
                    else if (itemInfo.itemType == ItemType.Pet)
                    {
                        categorizedItems["Pet"].Add(item.Key, item.Value);
                    }
                    else
                    {
                        categorizedItems["Normal"].Add(item.Key, item.Value);
                    }
                }
            }

            return true;
        }
    }

    [CommandPrefix("!")]
    public partial class CommandsToGenerate
    {
        [Command(Commands.CmdHelp, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm0() { }

        [Command(Commands.CmdState, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm1() { }

        [Command(Commands.CmdPrintItemIds, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm2(string xp_currency_bonus_Pet_decoration_armor_tool_food_crop_fish_normal) { }

        [Command(Commands.CmdMineReset, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm3() { }

        [Command(Commands.CmdMineClear, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm4() { }

        [Command(Commands.CmdMineOverfill, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm5() { }

        [Command(Commands.CmdCoins, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm6(string amount) { }

        [Command(Commands.CmdOrbs, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm7(string amount) { }

        [Command(Commands.CmdTickets, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm8(string amount) { }

        [Command(Commands.CmdName, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm9(string playerName) { }

        [Command(Commands.CmdSetDate, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        public static void fm10(string DayOrHoure_and_Value) { }

        [Command(Commands.CmdWeather, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        public static void fm11(string raining_or_heatwave_or_clear) { }

        [Command(Commands.CmdGive, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm12(string itemName) { }

        [Command(Commands.CmdShowItems, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm13(string itemName_ToShowItemsWithGivenName) { }

        [Command(Commands.CmdDevKit, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm14() { }

        [Command(Commands.CmdDasher, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm15() { }

        //[Command(Commands.CmdAppendItemDescWithId, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        //[CommandDescription("None")]
        //private static void fm16(string INFO_ShowsItemIdsInDescription) { }

        [Command(Commands.CmdManaFill, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm17() { }

        [Command(Commands.CmdManaInf, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm18() { }

        [Command(Commands.CmdHealthFill, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm19() { }

        [Command(Commands.CmdSleep, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm20() { }

        [Command(Commands.CmdPrintHoverItem, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm21(string INFO_sendItemIdAndNameToChat) { }

        [Command(Commands.CmdFeedbackDisabled, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm22() { }

        [Command(Commands.CmdPause, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("None")]
        private static void fm23() { }

        [Command(Commands.CmdCustomDaySpeed, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("Set custom time speed")]
        private static void fm24(string Value_or_Nothing_to_reset) { }

        [Command(Commands.CmdUI, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("Toggle UI on/off")]
        private static void fm25(string On_or_Off) { }

        [Command(Commands.CmdJumper, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("Jump over objects")]
        private static void fm26() { }

        [Command(Commands.CmdNoClip, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("Walk through walls")]
        private static void fm27() { }

        [Command(Commands.CmdNoHit, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("Not hitable mode")]
        private static void fm28() { }

        [Command(Commands.CmdAutoFillMuseum, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("Fill Museum apon entry with items from inventory")]
        private static void fm29(string INFO_autofillMusuemOnEnterMuseum) { }
       
        [Command(Commands.CmdCheatFillMuseum, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("Fill Museum apon entry without loosing items")]
        private static void fm30(string INFO_cheatfillMusuemOnEnterMuseum) { }

        [Command(Commands.CmdTeleport, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("teleport to location")]
        private static void fm31(string location) { }

        [Command(Commands.CmdTeleportLocations, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("List teleport locations")]
        private static void fm32() { }

        //[Command(Commands.CmdDespawnPet, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        //[CommandDescription("despawn pet")]
        //private static void fm33(string Pet_Name) { }

        //[Command(Commands.CmdSpawnPet, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        //[CommandDescription("spawn pet")]
        //private static void fm34(string Pet_Name) { }

        //[Command(Commands.CmdPetList, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        //[CommandDescription("List all pets")]
        //private static void fm35(string Pet_Name) { }

        [Command(Commands.CmdUnMarry, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("Divorce/Unmarry")]
        private static void fm36(string NPC_Name) { }

        [Command(Commands.CmdRelationship, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("set NPC Relationship level (1-20?)")]
        private static void fm37(string NPC_Name_and_value) { }

        [Command(Commands.CmdMarryNpc, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("set Marry NPC's")]
        private static void fm38(string NPC_Name) { }

        [Command(Commands.CmdSetSeason, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("set the season (Spring, Summer, Fall, Winter)")]
        private static void fm39(string season) { }

        //[Command(Commands.CmdFixYear, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        //[CommandDescription("Fix the year calculation")]
        //private static void fm40(string value) { }

        [Command(Commands.CmdIncDecYear, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("Increase/Decrase the current year")]
        private static void fm41(string amount) { }

        [Command(Commands.CmdCheats, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        [CommandDescription("Toggle SunHaven cheats enabled")]
        private static void fm42() { }
    }
}
