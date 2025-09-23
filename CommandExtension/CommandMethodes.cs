using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using UnityEngine.Device;
using Wish;

namespace CommandExtension
{
    public static class CommandMethodes
    {
        private static Dictionary<string, int> itemIds = (Dictionary<string, int>)typeof(Database).GetField("ids", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Database.Instance);
        private static Dictionary<string, Dictionary<string, int>> categorizedItems = new Dictionary<string, Dictionary<string, int>>() {
            { 
                "currency",
                new Dictionary<string, int>() {
                    { "coins", 60000 }, { "orbs", 18010 }, { "tickets", 18011 }}
            },
            { 
                "exp",
                new Dictionary<string, int>() {
                    { "combatexp", 60003 }, { "farmingexp", 60004 }, { "miningexp", 60006 }, { "explorationexp", 60005 }, { "fishingexp", 60008 }}
            },
            { 
                "bonus",
                new Dictionary<string, int>() {
                    { "health", 60009 }, { "mana", 60007 }}
            },
            { "decoration", new Dictionary<string, int>() },
            { "normal", new Dictionary<string, int>() },
            { "armor", new Dictionary<string, int>() },
            { "tool", new Dictionary<string, int>() },
            { "food", new Dictionary<string, int>() },
            { "crop", new Dictionary<string, int>() },
            { "fish", new Dictionary<string, int>() },
            { "pet", new Dictionary<string, int>() }};
        private static List<string> tpLocations = new List<string>() {
            "throneroom", "nelvari", "wishingwell", "altar", "hospital", "sunhaven", "sunhavenfarm/farm/home", "nelvarifarm", "nelvarimine", "nelvarihome",
            "withergatefarm", "castle", "withergatehome", "grandtree", "taxi", "dynus", "sewer", "nivara", "barracks", "elios", "dungeon", "store", "beach" };

        public static string playerNameForCommandsFirst;
        public static string playerNameForCommands;
        public static float timeMultiplier = CommandDefaultValues.timeMultiplier;
        public static bool uiVisible = true;
        private static string lastScene;
        private static Vector2 lastLocation;

        struct CommandDefaultValues
        {
            public const float timeMultiplier = 0.2F;
        }

        public static bool CommandFunction_Debug(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');
            return true;
        }

        // Help *
        public static bool CommandFunction_Help(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (mayCommandParam.Length >= 2)
            {
                MessageToChat($"[{mayCommandParam[1]} - Command]".ColorText(Color.black));
                foreach (Command command in Commands.GeneratedCommands.Values)
                {
                    if (command.State == CommandState.Activated)
                    {
                        MessageToChat($"{command.Key.ColorText(CommandExtension.GreenColor)}\n{command.Description.ColorText(new Color(1F, 0.66F, 0.0F))}");
                    }
                    else if (command.State == CommandState.Deactivated)
                    {
                        MessageToChat($"{command.Key.ColorText(CommandExtension.RedColor)}\n{command.Description.ColorText(new Color(1F, 0.66F, 0.0F))}");
                    }
                    if (command.State == CommandState.None)
                    {
                        MessageToChat($"{command.Key.ColorText(CommandExtension.YellowColor)}\n{command.Description.ColorText(new Color(1F, 0.66F, 0.0F))}");
                    }

                    MessageToChat($"{command.Usage.ColorText(CommandExtension.YellowColor)}");
                }
            }
            else
            {
                MessageToChat("[List of Commands]".ColorText(Color.black));
                foreach (Command command in Commands.GeneratedCommands.Values)
                {
                    if (command.State == CommandState.Activated)
                        MessageToChat($"{command.Key.ColorText(CommandExtension.GreenColor)}{CommandExtension.MESSAGE_GAP.ColorText(Color.black)}{command.Description.ColorText(new Color(1F, 0.66F, 0.0F))}");
                    if (command.State == CommandState.Deactivated)
                        MessageToChat($"{command.Key.ColorText(CommandExtension.RedColor)}{CommandExtension.MESSAGE_GAP.ColorText(Color.black)}{command.Description.ColorText(new Color(1F, 0.66F, 0.0F))}");
                    if (command.State == CommandState.None)
                        MessageToChat($"{command.Key.ColorText(CommandExtension.YellowColor)}{CommandExtension.MESSAGE_GAP.ColorText(Color.black)}{command.Description.ColorText(new Color(1F, 0.66F, 0.0F))}");
                }
            }

            return true;
        }

        // State *
        public static bool CommandFunction_State(string commandInput)
        {
            List<string> activeToggleMessages = new List<string>();
            foreach (Command command in Commands.GeneratedCommands.Values)
            {
                if (command.State == CommandState.Activated)
                {
                    activeToggleMessages.Add($"{command.Key.ColorText(CommandExtension.YellowColor)}{CommandExtension.MESSAGE_GAP.ColorText(Color.black)}{(command.State.ToString().ColorText(CommandExtension.GreenColor))}");
                }
            }

            if (activeToggleMessages.Count >= 1)
            {
                MessageToChat("[States]".ColorText(Color.black));

                foreach (string message in activeToggleMessages)
                {
                    MessageToChat(message);
                }
            }
            else
            {
                MessageToChat("No active commands".ColorText(CommandExtension.YellowColor));
            }
            return true;
        }

        // PRINT SPECIAL ITEMS
        public static bool CommandFunction_PrintItemIds(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (categorizedItems == null)
            {
                if (!CategorizeItemList())
                {
                    MessageToChat($"Database Missing!".ColorText(CommandExtension.RedColor));
                    return true;
                }
            }

            bool getItems = mayCommandParam.Length >= 3 && mayCommandParam[2] == "get";
            int amount = (mayCommandParam.Length >= 4 && int.TryParse(mayCommandParam[2], out amount)) ? amount : 1;

            switch ((mayCommandParam.Length >= 2) ? mayCommandParam[1] : "-")
            {
                case "xp":
                    MessageToChat("[Exp Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["exp"])
                    {
                        MuseumGiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "currency":
                    MessageToChat("[Currency Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["currency"])
                    {
                        MuseumGiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                /*
                case 'a':
                    if (CategorizeItemList())
                    {
                        string file = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "\\itemIDs(CommandExtension).txt";
                        if (!File.Exists(file))
                        {
                            using (File.Create(file))
                            { }
                        }
                        bool isEmpty = true;
                        File.WriteAllText(file, "");
                        string overviewLine = new string('#', 80);
                        foreach (var category in categorizedItems)
                        {
                            if (category.Value.Count >= 1)
                            {
                                if (isEmpty)
                                {
                                    File.AppendAllText(file, $"[{category.Key}]\n");
                                    isEmpty = false;
                                }
                                else
                                    File.AppendAllText(file, $"\n\n\n{overviewLine}\n[{category.Key}]\n");
                                foreach (var item in category.Value.OrderBy(i => i.Key))
                                {
                                    File.AppendAllText(file, $"{item.Key} : {item.Value}\n");
                                }
                                File.AppendAllText(file, "");
                            }
                        }
                        MessageToChat("ID list created inside your Sun Haven folder:".ColorText(Color.CommandExtension.GreenColor));
                        MessageToChat(file.ColorText(Color.white));
                    }
                    else
                        CommandFunction_PrintToChat("ERROR: ".ColorText(CommandExtension.RedColor) + "ItemDatabaseWrapper.ItemDatabase.ids".ColorText(Color.white) + " is empty!".ColorText(CommandExtension.RedColor));
                    break;
                */

                case "bonus":
                    MessageToChat("[Bonus Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["bonus"])
                    {
                        MuseumGiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "decoration":
                    MessageToChat("[Decoration Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["decoration"])
                    {
                        MuseumGiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "armor":
                    MessageToChat("[Armor Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["armor"])
                    {
                        MuseumGiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "tool":
                    MessageToChat("[Tool Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["tool"])
                    {
                        MuseumGiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "food":
                    MessageToChat("[Food Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["food"])
                    {
                        MuseumGiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "crop":
                    MessageToChat("[Crop Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["crop"])
                    {
                        MuseumGiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "fish":
                    MessageToChat("[Fish Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["fish"])
                    {
                        MuseumGiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "pet":
                    MessageToChat("[Pet Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["pet"])
                    {
                        MuseumGiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                case "normal":
                    MessageToChat("[Normal Ids]".ColorText(Color.black));
                    foreach (KeyValuePair<string, int> item in categorizedItems["normal"])
                    {
                        MuseumGiveOrPrintItem(item.Value, amount, getItems);
                    }
                    break;

                default:
                    MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
                    //MessageToChat(Commands.CmdKeyPrintItemIds + " [xp|currency|bonus|pet|decoration|armor|tool|food|crop|fish|normal]".ColorText(CommandExtension.RedColor));
                    return true;
            }

            return true;
        }

        // RESET MINES
        public static bool CommandFunction_ResetMines(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (UnityEngine.Object.FindObjectOfType<MineGenerator2>() != null)
            {
                typeof(MineGenerator2).GetMethod("ResetMines", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(MineGenerator2.Instance, new object[] { (ushort)0 });
                MessageToChat($"Mine {"Reseted".ColorText(CommandExtension.GreenColor)}!".ColorText(CommandExtension.YellowColor));
            }
            else
            {
                MessageToChat(("Must be inside a Mine!").ColorText(CommandExtension.RedColor));
            }
            return true;
        }

        // CLEAR MINES
        public static bool CommandFunction_ClearMines(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (UnityEngine.Object.FindObjectOfType<MineGenerator2>() != null)
            {
                typeof(MineGenerator2).GetMethod("ClearMine", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(MineGenerator2.Instance, null);
                MessageToChat($"Mine {"CleaCommandExtension.RedColor".ColorText(CommandExtension.GreenColor)}!".ColorText(CommandExtension.YellowColor));
            }
            else
            {
                MessageToChat(("Must be inside a Mine!").ColorText(CommandExtension.RedColor));
            }
            return true;
        }

        // OVERFILL MINES
        public static bool CommandFunction_OverfillMines(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (UnityEngine.Object.FindObjectOfType<MineGenerator2>() != null)
            {
                for (int i = 0; i < 30; i++)
                {
                    typeof(MineGenerator2).GetMethod("GenerateRocks", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(MineGenerator2.Instance, null);
                }

                MessageToChat($"Mine {"Overfilled".ColorText(CommandExtension.GreenColor)}!".ColorText(CommandExtension.YellowColor));
            }
            else
            {
                MessageToChat(("Must be inside a Mine!").ColorText(CommandExtension.RedColor));
            }
            return true;
        }

        // ADD MONEY/COINS
        public static bool CommandFunction_AddMoney(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (!int.TryParse(Regex.Match(commandInput, @"\d+").Value, out int moneyAmount))
            {
                MessageToChat("Something wen't wrong..".ColorText(CommandExtension.RedColor));
                MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
                return true;
            }
            if (commandInput.Contains("-"))
            {
                Player.Instance.AddMoney(-moneyAmount, true, true, true);
                MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} paid {moneyAmount.ToString().ColorText(Color.white)} Coins!".ColorText(CommandExtension.YellowColor));
            }
            else
            {
                Player.Instance.AddMoney(moneyAmount, true, true, true);
                MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} got {moneyAmount.ToString().ColorText(Color.white)} Coins!".ColorText(CommandExtension.YellowColor));
            }
            return true;
        }

        // ADD MONEY
        public static bool CommandFunction_AddOrbs(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (!int.TryParse(Regex.Match(commandInput, @"\d+").Value, out int moneyAmount))
            {
                MessageToChat("Something wen't wrong..".ColorText(CommandExtension.RedColor));
                MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
                return true;
            }
            if (commandInput.Contains("-"))
            {
                Player.Instance.AddOrbs(-moneyAmount);
                MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} paid {moneyAmount.ToString().ColorText(Color.white)} Orbs!".ColorText(CommandExtension.YellowColor));
            }
            else
            {
                Player.Instance.AddOrbs(moneyAmount);
                MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} got {moneyAmount.ToString().ColorText(Color.white)} Orbs!".ColorText(CommandExtension.YellowColor));
            }
            return true;
        }

        // ADD MONEY
        public static bool CommandFunction_AddTickets(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (!int.TryParse(Regex.Match(commandInput, @"\d+").Value, out int moneyAmount))
            {
                MessageToChat("Something wen't wrong..".ColorText(CommandExtension.RedColor));
                MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
                return true;
            }
            if (commandInput.Contains("-"))
            {
                Player.Instance.AddTickets(-moneyAmount);
                MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} paid {moneyAmount.ToString().ColorText(Color.white)} Tickets!".ColorText(CommandExtension.YellowColor));
            }
            else
            {
                Player.Instance.AddTickets(moneyAmount);
                MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} got {moneyAmount.ToString().ColorText(Color.white)} Tickets!".ColorText(CommandExtension.YellowColor));
            }
            return true;
        }

        // SET NAME
        public static bool CommandFunction_SetName(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (mayCommandParam.Length >= 2)
            {
                playerNameForCommands = mayCommandParam[1];
                MessageToChat($"Command target name set to {mayCommandParam[1].ColorText(Color.magenta)}!".ColorText(CommandExtension.YellowColor));
            }
            else
            {
                playerNameForCommands = playerNameForCommandsFirst;
                MessageToChat($"Command target name {"restoCommandExtension.RedColor".ColorText(CommandExtension.GreenColor)} to {playerNameForCommandsFirst.ColorText(Color.magenta)}!".ColorText(CommandExtension.YellowColor));
            }
            return true;
        }

        // CHANGE DATE (only hour and day!)
        // TODO: split into day command and hour command, rename years command to year
        public static bool CommandFunction_ChangeDate(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (mayCommandParam.Length == 3)
            {
                var Date = DayCycle.Instance;
                if (int.TryParse(mayCommandParam[2], out int dateValue))
                {
                    switch (mayCommandParam[1][0])
                    {
                        case 'd':
                            if (dateValue <= 0 || dateValue > 28)
                            { MessageToChat($"day {"1-28".ColorText(Color.white)} are allowed".ColorText(CommandExtension.RedColor)); return true; }
                            Date.Time = new DateTime(Date.Time.Year, Date.Time.Month, dateValue, Date.Time.Hour + 1, Date.Time.Minute, Date.Time.Second, Date.Time.Millisecond);
                            typeof(DayCycle).GetMethod("SetInitialTime", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DayCycle.Instance, null);
                            MessageToChat($"{"Day".ColorText(CommandExtension.GreenColor)} set to {dateValue.ToString().ColorText(Color.white)}!".ColorText(CommandExtension.YellowColor));
                            break;

                        case 'h':
                            if (dateValue < 6 || dateValue > 22) // 6-23 
                            { MessageToChat($"hour {"6-23".ColorText(Color.white)} are allowed".ColorText(CommandExtension.RedColor)); return true; }
                            Date.Time = new DateTime(Date.Time.Year, Date.Time.Month, Date.Time.Day, dateValue + 1, Date.Time.Minute, Date.Time.Second, Date.Time.Millisecond);
                            MessageToChat($"{"Hour".ColorText(CommandExtension.GreenColor)} set to {dateValue.ToString().ColorText(Color.white)}!".ColorText(CommandExtension.YellowColor));
                            break;
                    }
                    return true;
                }
            }

            MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
            return true;
        }

        // CHANGE WEATHER
        public static bool CommandFunction_ChangeWeather(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (mayCommandParam.Length >= 2)
            {
                var Date = DayCycle.Instance;
                switch (mayCommandParam[1][0])
                {
                    case 'r': // rain toggle
                        Date.SetToRaining(Date.Raining ? false : true);
                        MessageToChat($"{"Raining".ColorText(CommandExtension.GreenColor)} turned {(!Date.Raining ? "Off".ColorText(CommandExtension.RedColor) : "On".ColorText(CommandExtension.GreenColor))}!".ColorText(CommandExtension.YellowColor));
                        break;

                    case 'h': // heatwave toggle
                        Date.SetToHeatWave(Date.Heatwave ? false : true);
                        MessageToChat($"{"Heatwave".ColorText(CommandExtension.GreenColor)} turned {(!Date.Heatwave ? "Off".ColorText(CommandExtension.RedColor) : "On".ColorText(CommandExtension.GreenColor))}!".ColorText(CommandExtension.YellowColor));
                        break;

                    case 'c': // clear both
                        Date.SetToHeatWave(false);
                        Date.SetToRaining(false);
                        MessageToChat($"{"Sunny".ColorText(CommandExtension.GreenColor)} weather turned {"On".ColorText(CommandExtension.GreenColor)}!".ColorText(CommandExtension.YellowColor));
                        break;
                }

                return true;
            }

            MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
            return true;
        }

        // GIVE ITEM BY ID/NAME 
        public static bool CommandFunction_GiveItemByIdOrName(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (mayCommandParam.Length >= 2)
            {
                if (int.TryParse(mayCommandParam[1], out int itemId))
                {
                    if (itemIds.Values.Contains(itemId))
                    {
                        int itemAmount = (mayCommandParam.Length >= 3 && int.TryParse(mayCommandParam[2], out itemAmount)) ? itemAmount : 1;
                        Player.Instance.Inventory.AddItem(itemId, itemAmount, 0, true, true);
                        string itemName = "unknown";
                        Database.GetData(itemId, delegate (ItemData data)
                        {
                            itemName = data.Name;
                        });
                        MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} got {itemAmount.ToString().ColorText(Color.white)} x {itemName.ColorText(Color.white)}!".ColorText(CommandExtension.YellowColor));
                    }
                    else
                    {
                        MessageToChat($"no item with id: {itemId.ToString().ColorText(Color.white)} found!".ColorText(CommandExtension.RedColor));
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
                                    Player.Instance.Inventory.AddItem(lastItemId, itemAmount, 0, true, true);
                                    return true;
                                }
                            }
                        }

                        if (itemsFound == 1)
                        {
                            int itemAmount = ((mayCommandParam.Length >= 3 && int.TryParse(mayCommandParam[2], out itemAmount)) ? itemAmount : 1);
                            Player.Instance.Inventory.AddItem(lastItemId, itemAmount, 0, true, true);
                        }
                        else if (itemsFound > 1)
                        {
                            MessageToChat("[Items Containing: ]".ColorText(new Color(0.1f, 0.1f, 0.1f)) + mayCommandParam[1].ColorText(new Color(0.26f, 0.26f, 0.26f)));
                            MessageToChat("use a unique item-name or id".ColorText(CommandExtension.RedColor));
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
                            MessageToChat($"no item name contains {mayCommandParam[1].ColorText(Color.white)}!".ColorText(CommandExtension.RedColor));
                        }
                    }
                    else
                    {
                        MessageToChat($"invalid Item ID!".ColorText(CommandExtension.RedColor));
                        MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
                    }
                    return true;
                }
            }
            else
            {
                MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
            }
            return true;
        }

        // SHOW ITEM BY NAME 
        public static bool CommandFunction_ShowItemByName(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

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
        public static bool CommandFunction_GiveDevItems(string commandInput)
        {
            foreach (int item in new int[] { 30003, 30004, 30005, 30006, 30007, 30008 })
            {
                Player.Instance.Inventory.AddItem(item, 1, 0, true);
            }
            MessageToChat($"{playerNameForCommands.ColorText(Color.magenta)} got a {"DevKit".ColorText(Color.white)}".ColorText(CommandExtension.YellowColor));
            return true;
        }

        // INFINITE AIR-SKIPS
        public static bool CommandFunction_InfiniteAirSkips(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // SHOW ID
        public static bool CommandFunction_ShowID(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // FILL MANA
        public static bool CommandFunction_ManaFill(string commandInput)
        {
            Player.Instance.AddMana(Player.Instance.MaxMana, 1f);
            MessageToChat(playerNameForCommands.ColorText(Color.magenta) + "'s Mana Refilled".ColorText(CommandExtension.YellowColor));
            return true;
        }

        // INFINITE MANA
        public static bool CommandFunction_InfiniteMana(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // FILL HEALTH
        public static bool CommandFunction_HealthFill(string commandInput)
        {
            Player.Instance.Heal(Player.Instance.MaxMana, true, 1f);
            MessageToChat(playerNameForCommands.ColorText(Color.magenta) + "'s Health Refilled".ColorText(CommandExtension.YellowColor));
            return true;
        }

        // SLEEP
        public static bool CommandFunction_Sleep(string commandInput)
        {
            Player.Instance.SkipSleep();
            MessageToChat($"{"Slept".ColorText(CommandExtension.GreenColor)} once! Another Day is a Good Day!".ColorText(CommandExtension.YellowColor));
            return true;
        }

        // PRINT ID ON HOVER
        public static bool CommandFunction_PrintItemIdOnHover(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // TOGGLE COMMAND FEEDBACK
        public static bool CommandFunction_ToggleFeedback(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // PAUSE
        public static bool CommandFunction_Pause(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // SLOW TIME  // TODO: turn to toggle command
        public static bool CommandFunction_CustomDaySpeed(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');
            string command = mayCommandParam[0];

            bool hasParams = mayCommandParam.Length >= 2;

            if (hasParams)
            {
                bool setValue = int.TryParse(mayCommandParam[1], out int value);
                string action = "set to default";

                if (setValue)
                {
                    action = "set to";
                    timeMultiplier = (float)System.Math.Round(20f / value, 4);
                }
                else if (mayCommandParam[1].Contains("r") || mayCommandParam[1].Contains("d")) // r = reset | d = default
                {
                    timeMultiplier = CommandDefaultValues.timeMultiplier;
                }
                else
                {

                    return false;
                }

                SetCommandState(commandKey: command, activate: true);
                MessageToChat($"Custom Dayspeed {"Activated".ColorText(CommandExtension.GreenColor)} and {action.ColorText(CommandExtension.GreenColor)}: {timeMultiplier.ToString().ColorText(Color.white)}".ColorText(CommandExtension.YellowColor));
            }

            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // UI ON/OFF
        public static bool CommandFunction_ToggleUI(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

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
                    GameObject obj = Utilities.FindObject(GameObject.Find(values[i].rootName), values[i].childName);
                    GameObjectSetActiveState(obj, flag);
                }

                GameObject obj2 = GameObject.Find("QuestTrackerVisibilityToggle");
                GameObjectSetActiveState(obj2, flag);

                MessageToChat("UI now ".ColorText(CommandExtension.YellowColor) + (flag ? "Visible".ColorText(CommandExtension.GreenColor) : "Hidden".ColorText(CommandExtension.GreenColor)));
            }
            return true;
        }

        // JUMPER
        public static bool CommandFunction_Jumper(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // NOCLIP
        public static bool CommandFunction_NoClip(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // NO HIT
        public static bool CommandFunction_NoHit(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // AUTO-FILL MUSEUM
        public static bool CommandFunction_AutoFillMuseum(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // CHEAT-FILL MUSEUM
        public static bool CommandFunction_CheatFillMuseum(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // TELEPORT  // TODO: 
        public static bool CommandFunction_TeleportToScene(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (!(mayCommandParam.Length >= 2))
            {
                return true;
            }

            string sceneName = mayCommandParam[1];
            if (sceneName == "withergatefarm")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(138f, 89.16582f), "WithergateRooftopFarm");
            }
            else if (sceneName == "throneroom")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(21.5f, 8.681581f), "Throneroom");
            }
            else if (sceneName == "nelvari")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(320.3333f, 98.76098f), "Nelvari6");
            }
            else if (sceneName == "wishingwell")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(55.83683f, 61.48384f), "WishingWell");
            }
            else if (sceneName.Contains("altar"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(199.3957f, 122.6284f), "DynusAltar");
            }
            else if (sceneName.Contains("hospital"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(80.83334f, 65.58415f), "Hospital");
            }
            else if (sceneName.Contains("sunhaven"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(268.125f, 299.9311f), "Town10");
            }
            else if (sceneName.Contains("nelvarifarm"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(139.6753f, 100.4739f), "NelvariFarm");
            }
            else if (sceneName.Contains("nelvarimine"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(144.7133f, 152.1591f), "NelvariMinesEntrance");
            }
            else if (sceneName.Contains("nelvarihome"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(51.5f, 54.97755f), "NelvariPlayerHouse");
            }
            else if (sceneName.Contains("castle"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(133.7634f, 229.2485f), "Withergatecastleentrance");
            }
            else if (sceneName.Contains("withergatehome"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(63.5f, 54.624f), "WithergatePlayerApartment");
            }
            else if (sceneName.Contains("grandtree"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(314.4297f, 235.2298f), "GrandTreeEntrance1");
            }
            else if (sceneName.Contains("taxi"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(101.707f, 123.4622f), "WildernessTaxi");
            }
            else if (sceneName == "dynus")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(94.5f, 121.09f), "Dynus");
            }
            else if (sceneName == "sewer")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(134.5833f, 129.813f), "Sewer");
            }
            else if (sceneName == "nivara")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(99f, 266.6305f), "Nivara");
            }
            else if (sceneName.Contains("barrack"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(71.58334f, 54.56507f), "Barracks");
            }
            else if (sceneName.Contains("elios"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(113.9856f, 104.2902f), "DragonsMeet");
            }
            else if (sceneName.Contains("dungeon"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(136.48f, 193.92f), "CombatDungeonEntrance");
            }
            else if (sceneName.Contains("store"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(77.5f, 58.55f), "GeneralStore");
            }
            else if (sceneName.Contains("beach"))
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(96.491529f, 87.46871f), "BeachRevamp");
            }
            else if (sceneName == "home" || sceneName.Contains("sunhavenhome") || sceneName == "farm")
            {
                lastScene = ScenePortalManager.ActiveSceneName;
                lastLocation = Player.Instance.transform.position;
                ScenePortalManager.Instance.ChangeScene(new Vector2(316.4159f, 152.5824f), "2Playerfarm");
            }
            else if (sceneName == "back")
            {
                ScenePortalManager.Instance.ChangeScene(lastLocation, lastScene);
            }
            else
            {
                MessageToChat($"Location {sceneName.ColorText(Color.white)} not found".ColorText(CommandExtension.RedColor));
            }

            return true;
        }

        // GET TELEPORT LOCATIONS
        public static bool CommandFunction_TeleportLocations(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            MessageToChat("[Locations]".ColorText(Color.black));
            foreach (string tpLocation in tpLocations)
            {
                MessageToChat(tpLocation.ColorText(Color.white));
            }

            return true;
        }

        // SET RELATIONSHIP
        public static bool CommandFunction_Relationship(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (mayCommandParam.Length >= 3)
            {
                string name = mayCommandParam[1];
                bool all = mayCommandParam[1] == "all";
                bool add = mayCommandParam.Length >= 4 && mayCommandParam[3] == "add";

                float value;
                if (float.TryParse(mayCommandParam[2], out value))
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
                            }
                            else
                            {
                                SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships[npc.NPCName] = value;
                            }

                            MessageToChat($"Relationship with {npc.NPCName.ColorText(Color.white)} set to {SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships[npc.NPCName].ToString().ColorText(Color.white)}!".ColorText(CommandExtension.GreenColor));
                            return true;
                        }
                    }

                    MessageToChat($"No NPC with the name {name.ColorText(Color.white)} found!".ColorText(CommandExtension.RedColor));
                }
                else
                {
                    MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
                }
            }

            return true;
        }

        // UN-MARRY NPC
        public static bool CommandFunction_UnMarry(string commandInput)
        {
            return ProcessMarry(commandInput, true);
        }

        // MARRY NPC
        public static bool CommandFunction_Marry(string commandInput)
        {
            return ProcessMarry(commandInput, false);
        }

        // SET SEASON
        public static bool CommandFunction_SetSeason(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (mayCommandParam.Length < 2 || !Enum.TryParse(mayCommandParam[1], true, out Season season2))
            {
                MessageToChat("invalid season!".ColorText(CommandExtension.RedColor));
                MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
                return true;
            }

            var Date = DayCycle.Instance;
            int targetYear = Date.Time.Year + ((int)season2 - (int)Date.Season + 4) % 4;

            DayCycle.Instance.Time = new DateTime(targetYear, Date.Time.Month, 1, Date.Time.Hour, Date.Time.Minute, 0, DateTimeKind.Utc).ToUniversalTime();
            typeof(DayCycle).GetMethod("SetInitialTime", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DayCycle.Instance, null);

            MessageToChat("It's now ".ColorText(CommandExtension.YellowColor) + season2.ToString().ColorText(CommandExtension.GreenColor));
            return true;
        }

        // YEAR FIX
        public static bool CommandFunction_ToggleYearFix(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }

        // CHANGE YEAR
        public static bool CommandFunction_Year(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split(' ');

            if (!int.TryParse(Regex.Match(commandInput, @"\d+").Value, out int value))
            {
                MessageToChat("Something wen't wrong..".ColorText(CommandExtension.RedColor));
                MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
                return true;
            }

            DayCycle Date = DayCycle.Instance;
            int newYear;

            if (commandInput.Contains("-"))
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
            {
                newYear = Date.Time.Year + (value * 4);
            }

            DayCycle.Instance.Time = new DateTime(newYear, Date.Time.Month, Date.Time.Day, Date.Time.Hour, Date.Time.Minute, 0, DateTimeKind.Utc).ToUniversalTime();
            typeof(DayCycle).GetMethod("SetInitialTime", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DayCycle.Instance, null);

            MessageToChat($"It's now Year {(Date.Time.Year / 4).ToString().ColorText(CommandExtension.GreenColor)}!".ColorText(CommandExtension.YellowColor));
            return true;
        }

        // TOGGLE CHEATS
        public static bool CommandFunction_ToggleCheats(string commandInput)
        {
            return ToggleCommandState(commandInput.Split(' ')[0], true);
        }


        // ============================================================
        // Utility
        // ============================================================

        private static bool ToggleCommandState(string commandKey, bool notifyPlayer = true)
        {
            string state = Commands.ToggleCommandState(commandKey)
                ? "On".ColorText(CommandExtension.GreenColor) : "Off".ColorText(CommandExtension.RedColor);

            if (notifyPlayer)
            {
                MessageToChat($"{commandKey} {state}".ColorText(CommandExtension.YellowColor));
            }

            return true;
        }

        private static bool SetCommandState(string commandKey, bool activate = true, bool notifyPlayer = true)
        {
            Commands.SetCommandState(commandKey, activate);
            string state = activate
                ? "On".ColorText(CommandExtension.GreenColor) : "Off".ColorText(CommandExtension.RedColor);

            if (notifyPlayer)
            {
                MessageToChat($"{commandKey} {state}".ColorText(CommandExtension.YellowColor));
            }

            return true;
        }

        public static void MessageToChat(string text)
        {
            if (Commands.IsCommandActive(Commands.CmdPrefix + Commands.CmdKeyFeedbackDisabled))
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

        private static void MuseumGiveOrPrintItem(int id, int amount, bool give = false)
        {
            //Commands.CmdPrintItemIds + " [xp|currency|bonus|pet|decoration|armor|tool|food|crop|fish|normal]"
            if (give)
            {
                Player.Instance.Inventory.AddItem(id, amount, 0, true, true);
            }
            else
            {
                MessageToChat($"{amount} : {id}");
            }
        }

        private static void GameObjectSetActiveState(GameObject obj, bool state)
        {
            obj?.SetActive(state);
        }

		private static readonly Regex npcNameRegex = new(@"[a-zA-Z\s\.]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private static bool ProcessMarry(string commandInput, bool unmarry = false)
        {
			static void Marry(NPCAI npcai, bool unmarry = false, bool notify = false)
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
					MessageToChat($"Married with {npcName.ColorText(Color.white)}!".ColorText(CommandExtension.GreenColor));
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

					MessageToChat($"Divorced from {npcName.ColorText(Color.white)}!".ColorText(CommandExtension.GreenColor));
				}
			}

			string[] mayCommandParam = commandInput.Split(' ');

            if (mayCommandParam.Length >= 2)
            {
                string name = FirstCharToUpper(mayCommandParam[1]);
                bool all = mayCommandParam[1] == "all";
                NPCAI[] npcs = UnityEngine.Object.FindObjectsOfType<NPCAI>();

                if (npcs == null || npcs.Length == 0)
                {
                    return true;
                }


                if (all)
                {
                    foreach (NPCAI npc in npcs)
                    {
                        Marry(npcai: npc, unmarry: unmarry, notify: true);
                    }
                }
                else
                {
					static string GetNpcName(string name)
					{
						Match match = npcNameRegex.Match(name);
						return match.Success ? match.Value : string.Empty;
					}

					NPCAI npc = npcs.FirstOrDefault(npc => GetNpcName(npc.NPCName) == name);
                    if (npc != null)
                    {
                        Marry(npcai: npc, unmarry: unmarry, notify: true);
                    }
                    else
                    {
                        MessageToChat($"No NPC with the name {name.ColorText(Color.white)} found!".ColorText(CommandExtension.RedColor));
                    }
                }
            }
            else
            {
                MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
            }

            return true;
        }

        //private static void Marry(NPCAI npcai, bool unmarry = false, bool notify = false)
        //{
        //    string npcName = npcai.NPCName;
        //    if (string.IsNullOrWhiteSpace(npcName) || !SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships.ContainsKey(npcai.NPCName))
        //    {
        //        return;
        //    }
		//
        //    npcai.MarryPlayer();
		//
        //    if (!unmarry)
        //    {
        //        if (SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships.ContainsKey(npcName))
        //        {
        //            SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships[npcName] = 100f;
        //        }
		//
        //        npcai.MarryPlayer();
        //        MessageToChat($"Married with {npcName.ColorText(Color.white)}!".ColorText(CommandExtension.GreenColor));
        //    }
        //    else
        //    {
        //        string progressStringCharacter = SingletonBehaviour<GameSave>.Instance.GetProgressStringCharacter("MarriedWith");
        //        if (!progressStringCharacter.IsNullOrWhiteSpace())
        //        {
        //            SingletonBehaviour<GameSave>.Instance.SetProgressStringCharacter("MarriedWith", "");
        //            SingletonBehaviour<GameSave>.Instance.SetProgressBoolCharacter("Married", false);
        //            SingletonBehaviour<GameSave>.Instance.SetProgressBoolCharacter("MarriedTo" + progressStringCharacter, false);
        //            GameSave.CurrentCharacter.Relationships[progressStringCharacter] = 40f;
        //            SingletonBehaviour<NPCManager>.Instance.GetRealNPC(progressStringCharacter).GenerateCycle(false);
        //        }
		//
        //        MessageToChat($"Divorced from {npcName.ColorText(Color.white)}!".ColorText(CommandExtension.GreenColor));
        //    }
        //}

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
        [Command(Commands.CmdKeyHelp, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm0() { }

        [Command(Commands.CmdKeyState, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm1() { }

        [Command(Commands.CmdKeyPrintCategorizedItems, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm2(string xp_currency_bonus_Pet_decoration_armor_tool_food_crop_fish_normal) { }

        [Command(Commands.CmdKeyMineReset, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm3() { }

        [Command(Commands.CmdKeyMineClear, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm4() { }

        [Command(Commands.CmdKeyMineOverfill, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm5() { }

        [Command(Commands.CmdKeyCoins, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm6(string amount) { }

        [Command(Commands.CmdKeyOrbs, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm7(string amount) { }

        [Command(Commands.CmdKeyTickets, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm8(string amount) { }

        [Command(Commands.CmdKeyName, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm9(string playerName) { }

        [Command(Commands.CmdKeySetDate, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        public static void fm10(string DayOrHoure_and_Value) { }

        [Command(Commands.CmdKeyWeather, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        public static void fm11(string raining_or_heatwave_or_clear) { }

        [Command(Commands.CmdKeyGive, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm12(string itemName) { }

        [Command(Commands.CmdKeyShowItems, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm13(string itemName_ToShowItemsWithGivenName) { }

        [Command(Commands.CmdKeyDevKit, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm14() { }

        [Command(Commands.CmdKeyDasher, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm15() { }

        //[Command(Commands.CmdAppendItemDescWithId, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        //private static void fm16(string INFO_ShowsItemIdsInDescription) { }

        [Command(Commands.CmdKeyManaFill, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm17() { }

        [Command(Commands.CmdKeyManaInf, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm18() { }

        [Command(Commands.CmdKeyHealthFill, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm19() { }

        [Command(Commands.CmdKeySleep, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm20() { }

        [Command(Commands.CmdKeyShowItemIdOnHover, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm21(string INFO_sendItemIdAndNameToChat) { }

        [Command(Commands.CmdKeyFeedbackDisabled, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm22() { }

        [Command(Commands.CmdKeyPause, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm23() { }

        [Command(Commands.CmdKeyCustomDaySpeed, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm24(string Value_or_Nothing_to_reset) { }

        [Command(Commands.CmdKeyUI, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm25(string On_or_Off) { }

        [Command(Commands.CmdKeyJumper, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm26() { }

        [Command(Commands.CmdKeyNoClip, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm27() { }

        [Command(Commands.CmdKeyNoHit, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm28() { }

        [Command(Commands.CmdKeyAutoFillMuseum, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm29(string INFO_autofillMusuemOnEnterMuseum) { }
       
        [Command(Commands.CmdKeyCheatFillMuseum, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm30(string INFO_cheatfillMusuemOnEnterMuseum) { }

        [Command(Commands.CmdKeyTeleport, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm31(string location) { }

        [Command(Commands.CmdKeyTeleportLocations, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm32() { }

        //[Command(Commands.CmdDespawnPet, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        //private static void fm33(string Pet_Name) { }

        //[Command(Commands.CmdSpawnPet, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        //private static void fm34(string Pet_Name) { }

        //[Command(Commands.CmdPetList, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        //private static void fm35(string Pet_Name) { }

        [Command(Commands.CmdKeyUnMarry, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm36(string NPC_Name) { }

        [Command(Commands.CmdKeyRelationship, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm37(string NPC_Name_and_value) { }

        [Command(Commands.CmdKeyMarry, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm38(string NPC_Name) { }

        [Command(Commands.CmdKeySetSeason, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm39(string season) { }

        //[Command(Commands.CmdFixYear, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        //private static void fm40(string value) { }

        [Command(Commands.CmdKeyIncDecYear, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm41(string amount) { }

        [Command(Commands.CmdKeyCheats, QFSW.QC.Platform.AllPlatforms, MonoTargetType.Single)]
        private static void fm42() { }
    }
}
