using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CommandExtension.Models;
using PSS;
using QFSW.QC;
using QFSW.QC.Utilities;
using UnityEngine;
using Wish;

namespace CommandExtension
{
    public static class CommandMethodes
    {
		public static string PlayerNameForCommandsFirst { get; set; }
		public static string PlayerNameForCommands { get; set; }
        public static bool UiVisible { get; set; } = true;
		public static float TimeMultiplier { get; set; } = CommandDefaultValues.timeMultiplier;
		public static int PlayerInitializationCount { get; set; } = 0;

		private static readonly Dictionary<string, int> _itemIds = (Dictionary<string, int>)typeof(Database).GetField("ids", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Database.Instance);
        private static readonly Dictionary<string, Dictionary<string, int>> _categorizedItems = new() {
            { "currency", new Dictionary<string, int>() { { "coins", 60000 }, { "orbs", 18010 }, { "tickets", 18011 }} },
			{ "xp", new Dictionary<string, int>() { { "combatexp", 60003 }, { "farmingexp", 60004 }, { "miningexp", 60006 }, { "explorationexp", 60005 }, { "fishingexp", 60008 }} },
			{ "bonus", new Dictionary<string, int>() { { "health", 60009 }, { "mana", 60007 }} },
			{ "decoration", new Dictionary<string, int>() { } },
			{ "normal", new Dictionary<string, int>() { } },
			{ "armor", new Dictionary<string, int>() { } },
			{ "tool", new Dictionary<string, int>() { } },
			{ "food", new Dictionary<string, int>() { } },
			{ "crop", new Dictionary<string, int>() { } },
			{ "fish", new Dictionary<string, int>() { } },
			{ "pet", new Dictionary<string, int>() { } }
		};
		private static bool _categorizedItemsAdded = false;

        private static string _lastScene;
        private static Vector2 _lastLocation;
		private static readonly Dictionary<string, TeleportLocation> _teleportLocations = new() {
			{ "withergatefarm", new TeleportLocation(new Vector2(138f,			89.16582f),		"WithergateRooftopFarm") },
			{ "throneroom",     new TeleportLocation(new Vector2(21.5f,			8.681581f),		"Throneroom") },
			{ "nelvari",        new TeleportLocation(new Vector2(320.3333f,		98.76098f),     "Nelvari6") },
			{ "wishingwell",    new TeleportLocation(new Vector2(55.83683f,		61.48384f),     "WishingWell") },
			{ "altar",          new TeleportLocation(new Vector2(199.3957f,		122.6284f),     "DynusAltar") },
			{ "hospital",       new TeleportLocation(new Vector2(80.83334f,		65.58415f),     "Hospital") },
			{ "sunhaven",       new TeleportLocation(new Vector2(268.125f,		299.9311f),		"Town10") },
			{ "nelvarifarm",    new TeleportLocation(new Vector2(139.6753f,		100.4739f),     "NelvariFarm") },
			{ "nelvarimine",    new TeleportLocation(new Vector2(144.7133f,		152.1591f),     "NelvariMinesEntrance") },
			{ "nelvarihome",    new TeleportLocation(new Vector2(51.5f,			54.97755f),		"NelvariPlayerHouse") },
			{ "castle",         new TeleportLocation(new Vector2(133.7634f,		229.2485f),     "Withergatecastleentrance") },
			{ "withergatehome", new TeleportLocation(new Vector2(63.5f,			54.624f),		"WithergatePlayerApartment") },
			{ "grandtree",      new TeleportLocation(new Vector2(314.4297f,		235.2298f),     "GrandTreeEntrance1") },
			{ "taxi",           new TeleportLocation(new Vector2(101.707f,		123.4622f),		"WildernessTaxi") },
			{ "dynus",          new TeleportLocation(new Vector2(94.5f,			121.09f),		"Dynus") },
			{ "sewer",          new TeleportLocation(new Vector2(134.5833f,		129.813f),      "Sewer") },
			{ "nivara",         new TeleportLocation(new Vector2(99f,			266.6305f),		"Nivara") },
			{ "barrack",        new TeleportLocation(new Vector2(71.58334f,		54.56507f),     "Barracks") },
			{ "elios",          new TeleportLocation(new Vector2(113.9856f,		104.2902f),     "DragonsMeet") },
			{ "dungeon",        new TeleportLocation(new Vector2(136.48f,		193.92f),		"CombatDungeonEntrance") },
			{ "store",          new TeleportLocation(new Vector2(77.5f,			58.55f),		"GeneralStore") },
			{ "beach",          new TeleportLocation(new Vector2(96.491529f,	87.46871f),		"BeachRevamp") },
			{ "home",           new TeleportLocation(new Vector2(316.4159f,		152.5824f),     "2Playerfarm") },
			{ "farm",           new TeleportLocation(new Vector2(316.4159f,		152.5824f),     "2Playerfarm") }
			//{ "back",			new TeleportLocation(_lastLocation,								_lastScene) }
		};


		//// Core
		// Debug
		public static bool CommandFunction_Debug(string commandInput)
        {
			//string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);
			//
			//if (!(mayCommandParam.Length >= 2))
			//{
			//
			//}
			//else
			//{
			//	string param = mayCommandParam[1];
			//
			//	if (param == "1")
			//	{
			//
			//	}
			//	else if (param == "2")
			//	{
			//
			//	}
			//}
			QuantumConsole.Instance.ClearConsole();
			return true;
        }

        // Help
        public static bool CommandFunction_Help(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            if (mayCommandParam.Length >= 2)
            {
				string param1 = Commands.ResolveCommandAlias(mayCommandParam[1]);
				if (Commands.GeneratedCommands.TryGetValue(param1, out Command command))
				{
					Color commandKeyColor = command.State == CommandState.None ? CommandExtension.DarkGrayColor
						: command.State == CommandState.Activated ? CommandExtension.GreenColor
						: CommandExtension.RedColor;

					MessageToChat($"\n[{command.PrefixedKey.ColorText(commandKeyColor)} - Command]".ColorText(Color.black)
							+ "\n" + command.Usage.ColorText(CommandExtension.YellowColor)
							+ "  -  " + command.Description.ColorText(CommandExtension.NormalColor));
				}
				else
				{
					MessageToChat($"Unknown Command: {param1}".ColorText(CommandExtension.RedColor));
					MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
				}
            }
            else
            {
                MessageToChat("[List of Commands]".ColorText(Color.black));
                foreach (Command command in Commands.GeneratedCommands.Values)
                {
                    if (command.State == CommandState.Activated)
					{
						MessageToChat($"{command.PrefixedKey.ColorText(CommandExtension.GreenColor)} {command.Description.ColorText(new Color(1F, 0.66F, 0.0F))}");
					}

					if (command.State == CommandState.Deactivated)
					{
						MessageToChat($"{command.PrefixedKey.ColorText(CommandExtension.RedColor)} {command.Description.ColorText(new Color(1F, 0.66F, 0.0F))}");
					}

					if (command.State == CommandState.None)
					{
						MessageToChat($"{command.PrefixedKey.ColorText(CommandExtension.YellowColor)} {command.Description.ColorText(new Color(1F, 0.66F, 0.0F))}");
					}
				}
            }

            return true;
        }

        // State
        public static bool CommandFunction_State(string commandInput)
        {
            List<string> activeToggleMessages = [];
            foreach (Command command in Commands.GeneratedCommands.Values)
            {
                if (command.State == CommandState.Activated)
                {
                    activeToggleMessages.Add($"{command.PrefixedKey.ColorText(CommandExtension.YellowColor)} {command.State.ToString().ColorText(CommandExtension.GreenColor)}");
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

		// Clear Chat
		public static bool CommandFunction_ClearChat(string commandInput)
		{
			QuantumConsole.Instance.ClearConsole();
			return true;
		}

		// Toggle disable command feedback (output)
		public static bool CommandFunction_FeedbackDisabled(string commandInput)
		{
			return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Set Command target name
		public static bool CommandFunction_SetName(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (mayCommandParam.Length >= 2)
			{
				PlayerNameForCommands = mayCommandParam[1];
				MessageToChat($"Command target name set to {mayCommandParam[1].ColorText(Color.magenta)}!".ColorText(CommandExtension.YellowColor));
			}
			else
			{
				PlayerNameForCommands = PlayerNameForCommandsFirst;
				MessageToChat($"Command target name {"restoCommandExtension.RedColor".ColorText(CommandExtension.GreenColor)} to {PlayerNameForCommandsFirst.ColorText(Color.magenta)}!".ColorText(CommandExtension.YellowColor));
			}

			return true;
		}

		//// Time Commands
		// Pause
		public static bool CommandFunction_Pause(string commandInput)
		{
			return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Custom time speed
		public static bool CommandFunction_Timespeed(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);
			string command = mayCommandParam[0];

			// Detect whether a parameter was provided
			bool hasParams = mayCommandParam.Length >= 2;

			if (hasParams)
			{
				// Try to parse the first parameter as an integer multiplier value
				bool setValue = int.TryParse(mayCommandParam[1], out int value);
				string action = "set to default";

				// If a numeric value was provided, compute and set TimeMultiplier
				if (setValue)
				{
					action = "set to";
					TimeMultiplier = (float)System.Math.Round(20f / value, 4);
				}
				// If the parameter contains 'r' or 'd', treat as reset/default request
				else if (mayCommandParam[1].Contains("r") || mayCommandParam[1].Contains("d")) // r = reset | d = default
				{
					TimeMultiplier = CommandDefaultValues.timeMultiplier;
				}
				// Parameter provided but not valid -> show usage and fail
				else
				{
					MessageToChat(Commands.GetCommandByKey(command).Usage.ColorText(CommandExtension.RedColor));
					return true;
				}

				// Ensure the timespeed command is activated and dont notify player
				SetCommandState(commandKey: command, activate: true, notifyPlayer: false);

				// Inform the player about the activation and the resulting multiplier
				MessageToChat($"Custom Dayspeed {"Activated".ColorText(CommandExtension.GreenColor)} and {action.ColorText(CommandExtension.GreenColor)}: {TimeMultiplier.ToString().ColorText(Color.white)}".ColorText(CommandExtension.YellowColor));
				return true;
			}

			_ = ToggleCommandState(command, true);
			return true;
		}

		// Set Date (only hour and day!)
		// TODO: split into day command and hour command, rename years command to year
		public static bool CommandFunction_Date(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (mayCommandParam.Length == 3)
			{
				DayCycle Date = DayCycle.Instance;
				if (int.TryParse(mayCommandParam[2], out int dateValue))
				{
					switch (mayCommandParam[1][0])
					{
						case 'd':
							if (dateValue is <= 0 or > 28)
							{
								MessageToChat($"day {"1-28".ColorText(Color.white)} are allowed".ColorText(CommandExtension.RedColor)); return true;
							}

							Date.Time = new DateTime(Date.Time.Year, Date.Time.Month, dateValue, Date.Time.Hour + 1, Date.Time.Minute, Date.Time.Second, Date.Time.Millisecond);
							_ = typeof(DayCycle).GetMethod("SetInitialTime", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DayCycle.Instance, null);
							MessageToChat($"{"Day".ColorText(CommandExtension.GreenColor)} set to {dateValue.ToString().ColorText(Color.white)}!".ColorText(CommandExtension.YellowColor));
							break;

						case 'h':
							if (dateValue is < 6 or > 22) // 6-23 
							{
								MessageToChat($"hour {"6-23".ColorText(Color.white)} are allowed".ColorText(CommandExtension.RedColor)); return true;
							}

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

		// Set Season
		public static bool CommandFunction_Season(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (mayCommandParam.Length < 2 || !Enum.TryParse(mayCommandParam[1], true, out Season season2))
			{
				MessageToChat("invalid season!".ColorText(CommandExtension.RedColor));
				MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
				return true;
			}

			DayCycle Date = DayCycle.Instance;
			int targetYear = Date.Time.Year + (((int)season2 - (int)Date.Season + 4) % 4);

			DayCycle.Instance.Time = new DateTime(targetYear, Date.Time.Month, 1, Date.Time.Hour, Date.Time.Minute, 0, DateTimeKind.Utc).ToUniversalTime();
			_ = typeof(DayCycle).GetMethod("SetInitialTime", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DayCycle.Instance, null);

			MessageToChat("It's now ".ColorText(CommandExtension.YellowColor) + season2.ToString().ColorText(CommandExtension.GreenColor));
			return true;
		}

		// Set Year
		public static bool CommandFunction_Year(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

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
				{
					newYear = Date.Time.Year - (value * 4);
				}
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
			_ = typeof(DayCycle).GetMethod("SetInitialTime", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DayCycle.Instance, null);

			MessageToChat($"It's now Year {(Date.Time.Year / 4).ToString().ColorText(CommandExtension.GreenColor)}!".ColorText(CommandExtension.YellowColor));
			return true;
		}

		// Set Weather
		public static bool CommandFunction_Weather(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (mayCommandParam.Length >= 2)
			{
				DayCycle Date = DayCycle.Instance;
				switch (mayCommandParam[1][0])
				{
					case 'r': // rain toggle
						Date.SetToRaining(!Date.Raining);
						MessageToChat($"{"Raining".ColorText(CommandExtension.GreenColor)} turned {(!Date.Raining ? "Off".ColorText(CommandExtension.RedColor) : "On".ColorText(CommandExtension.GreenColor))}!".ColorText(CommandExtension.YellowColor));
						break;

					case 'h': // heatwave toggle
						Date.SetToHeatWave(!Date.Heatwave);
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

		// Year Fix
		public static bool CommandFunction_ToggleYearFix(string commandInput)
		{
			return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		//// Player Commands
		// Jumper
		public static bool CommandFunction_JumpOver(string commandInput)
		{
			return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Dasher (Infinite Air-skips)
		public static bool CommandFunction_DashInfinite(string commandInput)
		{
			return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Refill Mana
		public static bool CommandFunction_ManaFill(string commandInput)
		{
			Player.Instance.AddMana(Player.Instance.MaxMana, 1f);
			MessageToChat(PlayerNameForCommands.ColorText(Color.magenta) + "'s Mana Refilled".ColorText(CommandExtension.YellowColor));
			return true;
		}

		// Infinite Mana
		public static bool CommandFunction_ManaInfinite(string commandInput)
		{
			return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Refill health
		public static bool CommandFunction_HealthFill(string commandInput)
		{
			Player.Instance.Heal(Player.Instance.MaxMana, true, 1f);
			MessageToChat(PlayerNameForCommands.ColorText(Color.magenta) + "'s Health Refilled".ColorText(CommandExtension.YellowColor));
			return true;
		}

		// Nohit
		public static bool CommandFunction_NoHit(string commandInput)
		{
			return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// noclip
		public static bool CommandFunction_Noclip(string commandInput)
		{
			return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Sleep
		public static bool CommandFunction_Sleep(string commandInput)
		{
			Player.Instance.SkipSleep();
			MessageToChat($"{"Slept".ColorText(CommandExtension.GreenColor)} once! Another Day is a Good Day!".ColorText(CommandExtension.YellowColor));
			return true;
		}

		//// Mine commands
		// Mines Reset
		public static bool CommandFunction_MinesReset(string commandInput)
		{
			if (UnityEngine.Object.FindObjectOfType<MineGenerator2>() != null)
			{
				_ = typeof(MineGenerator2).GetMethod("ResetMines", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(MineGenerator2.Instance, [(ushort)0]);
				MessageToChat($"Mine {"Reseted".ColorText(CommandExtension.GreenColor)}!".ColorText(CommandExtension.YellowColor));
			}
			else
			{
				MessageToChat("Must be inside a Mine!".ColorText(CommandExtension.RedColor));
			}

			return true;
		}

		// Mines Clear
		public static bool CommandFunction_MinesClear(string commandInput)
		{
			if (UnityEngine.Object.FindObjectOfType<MineGenerator2>() != null)
			{
				_ = typeof(MineGenerator2).GetMethod("ClearMine", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(MineGenerator2.Instance, null);
				MessageToChat($"Mine {"CleaCommandExtension.RedColor".ColorText(CommandExtension.GreenColor)}!".ColorText(CommandExtension.YellowColor));
			}
			else
			{
				MessageToChat("Must be inside a Mine!".ColorText(CommandExtension.RedColor));
			}

			return true;
		}

		// Mines Overfill
		public static bool CommandFunction_MinesOverfill(string commandInput)
		{
			if (UnityEngine.Object.FindObjectOfType<MineGenerator2>() != null)
			{
				for (int i = 0; i < 30; i++)
				{
					_ = typeof(MineGenerator2).GetMethod("GenerateRocks", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(MineGenerator2.Instance, null);
				}

				MessageToChat($"Mine {"Overfilled".ColorText(CommandExtension.GreenColor)}!".ColorText(CommandExtension.YellowColor));
			}
			else
			{
				MessageToChat("Must be inside a Mine!".ColorText(CommandExtension.RedColor));
			}

			return true;
		}

		//// NPC relationship Commands
		// Set relationship
		public static bool CommandFunction_Relationship(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (mayCommandParam.Length >= 3)
			{
				string name = mayCommandParam[1];
				bool all = mayCommandParam[1] == "all";
				bool add = mayCommandParam.Length >= 4 && mayCommandParam[3] == "add";

				if (float.TryParse(mayCommandParam[2], out float value))
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

		// Marry
		public static bool CommandFunction_Marry(string commandInput)
		{
			return ProcessMarry(commandInput, false);
		}

		// Umarry
		public static bool CommandFunction_Divorce(string commandInput)
		{
			return ProcessMarry(commandInput, true);
		}

		//// Item Commands
		// Give Dev-Kit
		public static bool CommandFunction_DevKit(string commandInput)
		{
			foreach (int item in new int[] { 30003, 30004, 30005, 30006, 30007, 30008 })
			{
				Player.Instance.Inventory.AddItem(item, 1, 0, true);
			}

			MessageToChat($"{PlayerNameForCommands.ColorText(Color.magenta)} got a {"DevKit".ColorText(Color.white)}".ColorText(CommandExtension.YellowColor));
			return true;
		}

		// Give Item by Id or Name
		public static bool CommandFunction_GiveItem(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            if (mayCommandParam.Length >= 2)
            {
                if (int.TryParse(mayCommandParam[1], out int itemId))
                {
                    if (_itemIds.Values.Contains(itemId))
                    {
                        int itemAmount = (mayCommandParam.Length >= 3 && int.TryParse(mayCommandParam[2], out itemAmount)) ? itemAmount : 1;
                        Player.Instance.Inventory.AddItem(itemId, itemAmount, 0, true, true);
                        string itemName = "unknown";
                        Database.GetData<ItemData>(itemId, delegate (ItemData data)
                        {
                            itemName = data.Name;
                        });
                        MessageToChat($"{PlayerNameForCommands.ColorText(Color.magenta)} got {itemAmount.ToString().ColorText(Color.white)} x {itemName.ColorText(Color.white)}!".ColorText(CommandExtension.YellowColor));
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
                        foreach (KeyValuePair<string, int> id in _itemIds)
                        {
                            if (id.Key.Contains(mayCommandParam[1].ToLower()))
                            {
                                lastItemId = id.Value;
                                itemsFound++;
                                if (mayCommandParam[1] == id.Key)
                                {
                                    int itemAmount = (mayCommandParam.Length >= 3 && int.TryParse(mayCommandParam[2], out itemAmount)) ? itemAmount : 1;
                                    Player.Instance.Inventory.AddItem(lastItemId, itemAmount, 0, true, true);
									string itemName = "unknown";
									Database.GetData(lastItemId, delegate (ItemData data)
									{
										itemName = data.Name;
									});
									MessageToChat($"{PlayerNameForCommands.ColorText(Color.magenta)} got {itemAmount.ToString().ColorText(Color.white)} x {itemName.ColorText(Color.white)}!".ColorText(CommandExtension.YellowColor));
									return true;
                                }
                            }
                        }

                        if (itemsFound == 1)
                        {
                            int itemAmount = (mayCommandParam.Length >= 3 && int.TryParse(mayCommandParam[2], out itemAmount)) ? itemAmount : 1;
							string itemName = "unknown";
							Database.GetData(lastItemId, delegate (ItemData data)
							{
								itemName = data.Name;
							});
							Player.Instance.Inventory.AddItem(lastItemId, itemAmount, 0, true, true);
                        }
                        else if (itemsFound > 1)
						{
                            MessageToChat($"[Items Containing: {mayCommandParam[1].ColorText(CommandExtension.DarkGrayColor)}]".ColorText(CommandExtension.BlackColor));
                            MessageToChat("use a unique item-name or id".ColorText(CommandExtension.RedColor));
                            foreach (KeyValuePair<string, int> id in _itemIds)
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

        // Show Item/s by Name
        public static bool CommandFunction_ShowItem(string commandInput)
        {
            string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            if (mayCommandParam.Length >= 2)
            {
                List<string> items = [];
                foreach (KeyValuePair<string, int> id in _itemIds)
                {
                    if (id.Key.ToLower().Contains(mayCommandParam[1]))
                    {
                        items.Add(id.Key.ColorText(Color.white) + " : ".ColorText(Color.black) + id.Value.ToString().ColorText(Color.white));
                    }
                }

                if (items.Count >= 1)
                {
                    MessageToChat($"[Items with: {mayCommandParam[1].ColorText(CommandExtension.DarkGrayColor)}]".ColorText(CommandExtension.BlackColor));
                    foreach (string ítem in items)
                    {
                        MessageToChat(ítem);
                    }
                }
            }

            return true;
		}

		// Show Categorized items
		public static bool CommandFunction_ShowCategorizedItems(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			static void MuseumGiveOrPrintItem(int id, string name, int amount, bool give = false)
			{
				//Commands.CmdPrintItemIds + " [xp|currency|bonus|pet|decoration|armor|tool|food|crop|fish|normal]"
				if (give)
				{
					Player.Instance.Inventory.AddItem(id, amount, 0, true, true);
				}
				else
				{
					MessageToChat($"{name} : {id}");
				}
			}

			if (!_categorizedItemsAdded && !CategorizeItemList())
			{
				MessageToChat($"Database Missing!".ColorText(CommandExtension.RedColor));
				return true;
			}

			if (!(mayCommandParam.Length >= 2))
			{
				return true;
			}

			bool getItems = mayCommandParam.Length >= 3 && mayCommandParam[2] == "get";
			int amount = (mayCommandParam.Length >= 4 && int.TryParse(mayCommandParam[2], out amount)) ? amount : 1;
			string category = mayCommandParam[1];

			if (_categorizedItems.TryGetValue(category, out Dictionary<string, int> items))
			{
				MessageToChat($"[{category} Ids]".ColorText(Color.black));

				foreach (KeyValuePair<string, int> item in _categorizedItems[category])
				{
					MuseumGiveOrPrintItem(id: item.Value, name: item.Key, amount: amount, give: getItems);
				}
			}
			else
			{
				MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
			}

			return true;
		}

        // Show id on Item Tooltip
        public static bool CommandFunction_ShowItemInfoOnTooltip(string commandInput)
        {
            return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
        }

        // Show id in chat on hover
        public static bool CommandFunction_ShowItemInfoOnHover(string commandInput)
        {
            return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		//// Currency Commands
		// Set Coins (Money)
		public static bool CommandFunction_Coins(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (!int.TryParse(Regex.Match(commandInput, @"\d+").Value, out int moneyAmount))
			{
				MessageToChat("Something wen't wrong..".ColorText(CommandExtension.RedColor));
				MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
				return true;
			}

			if (commandInput.Contains("-"))
			{
				Player.Instance.AddMoney(-moneyAmount, true, true, true);
				MessageToChat($"{PlayerNameForCommands.ColorText(Color.magenta)} paid {moneyAmount.ToString().ColorText(Color.white)} Coins!".ColorText(CommandExtension.YellowColor));
			}
			else
			{
				Player.Instance.AddMoney(moneyAmount, true, true, true);
				MessageToChat($"{PlayerNameForCommands.ColorText(Color.magenta)} got {moneyAmount.ToString().ColorText(Color.white)} Coins!".ColorText(CommandExtension.YellowColor));
			}

			return true;
		}

		// Set Orbs
		public static bool CommandFunction_Orbs(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (!int.TryParse(Regex.Match(commandInput, @"\d+").Value, out int moneyAmount))
			{
				MessageToChat("Something wen't wrong..".ColorText(CommandExtension.RedColor));
				MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
				return true;
			}

			if (commandInput.Contains("-"))
			{
				Player.Instance.AddOrbs(-moneyAmount);
				MessageToChat($"{PlayerNameForCommands.ColorText(Color.magenta)} paid {moneyAmount.ToString().ColorText(Color.white)} Orbs!".ColorText(CommandExtension.YellowColor));
			}
			else
			{
				Player.Instance.AddOrbs(moneyAmount);
				MessageToChat($"{PlayerNameForCommands.ColorText(Color.magenta)} got {moneyAmount.ToString().ColorText(Color.white)} Orbs!".ColorText(CommandExtension.YellowColor));
			}

			return true;
		}

		// Set Tickets
		public static bool CommandFunction_Tickets(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (!int.TryParse(Regex.Match(commandInput, @"\d+").Value, out int moneyAmount))
			{
				MessageToChat("Something wen't wrong..".ColorText(CommandExtension.RedColor));
				MessageToChat(Commands.GetCommandByKey(mayCommandParam[0]).Usage.ColorText(CommandExtension.RedColor));
				return true;
			}

			if (commandInput.Contains("-"))
			{
				Player.Instance.AddTickets(-moneyAmount);
				MessageToChat($"{PlayerNameForCommands.ColorText(Color.magenta)} paid {moneyAmount.ToString().ColorText(Color.white)} Tickets!".ColorText(CommandExtension.YellowColor));
			}
			else
			{
				Player.Instance.AddTickets(moneyAmount);
				MessageToChat($"{PlayerNameForCommands.ColorText(Color.magenta)} got {moneyAmount.ToString().ColorText(Color.white)} Tickets!".ColorText(CommandExtension.YellowColor));
			}

			return true;
		}

		//// Teleport
		// Teleport To // TODO: 
		public static bool CommandFunction_TeleportToScene(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (!(mayCommandParam.Length >= 2))
			{
				return true;
			}
			
			string sceneName = mayCommandParam[1];

			if (_teleportLocations.TryGetValue(sceneName, out TeleportLocation location))
			{
				_lastScene = ScenePortalManager.ActiveSceneName;
				_lastLocation = Player.Instance.transform.position;
				ScenePortalManager.Instance.ChangeScene(location.Position, location.Destination);
			}
			else if (sceneName == "back")
			{
				ScenePortalManager.Instance.ChangeScene(_lastLocation, _lastScene);
			}
			else
			{
				MessageToChat($"Location {sceneName.ColorText(Color.white)} not found".ColorText(CommandExtension.RedColor));
			}

			return true;
		}

		// Show teleport locations
		public static bool CommandFunction_TeleportLocations(string commandInput)
		{
			MessageToChat("[Locations]".ColorText(Color.black));
			foreach (string tpLocation in _teleportLocations.Keys)
			{
				MessageToChat(tpLocation.ColorText(Color.white));
			}

			return true;
		}

		//// Misc Commands
		// Auto-fill museum
		public static bool CommandFunction_AutoFillMuseum(string commandInput)
		{
			return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Cheat-fill museum
		public static bool CommandFunction_CheatFillMuseum(string commandInput)
		{
			return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// UI
		public static bool CommandFunction_UI(string commandInput)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			static void GameObjectSetActiveState(GameObject obj, bool state)
			{
				obj?.SetActive(state);
			}

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
					flag = UiVisible = !UiVisible;
				}

				(string rootName, string childName)[] values =
				[
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
				];

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

		// Toggle Cheats
		public static bool CommandFunction_Cheats(string commandInput)
		{
			return ToggleCommandState(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}


		// ============================================================
		// Utility
		// ============================================================

		/// <summary>
		/// Sends a text message to the player's chat when command feedback is enabled.
		/// Checks the feedback-disabled command flag and logs the message to the QuantumConsole
		/// instance if chat feedback is currently allowed.
		/// </summary>
		/// <param name="message">The text to send to the player's chat.</param>
		public static void MessageToChat(string message)
        {
            if (!Commands.IsCommandActive(Commands.CmdPrefix + Commands.CmdKeyFeedbackDisabled))
            {
                QuantumConsole.Instance.LogPlayerText(message);
            }
        }

		private static bool ToggleCommandState(string commandKey, bool notifyPlayer = true)
        {
            string state = Commands.ToggleCommandState(commandKey) ? "On".ColorText(CommandExtension.GreenColor) : "Off".ColorText(CommandExtension.RedColor);

            if (notifyPlayer)
            {
                MessageToChat($"{commandKey} {state}".ColorText(CommandExtension.YellowColor));
            }

            return true;
        }

        private static void SetCommandState(string commandKey, bool activate = true, bool notifyPlayer = true)
        {
            Commands.SetCommandState(commandKey, activate);
            string state = activate
                ? "On".ColorText(CommandExtension.GreenColor) : "Off".ColorText(CommandExtension.RedColor);

            if (notifyPlayer)
            {
                MessageToChat($"{commandKey} {state}".ColorText(CommandExtension.YellowColor));
            }
        }

        private static bool CategorizeItemList()
        {
            if (_itemIds == null || _itemIds.Count < 1)
            {
                return false;
            }

			foreach (KeyValuePair<string, int> item in _itemIds)
            {
                if (ItemInfoDatabase.Instance.allItemSellInfos.TryGetValue(item.Value, out ItemSellInfo itemInfo))
                {
                    if (itemInfo.decorationType is not DecorationType.None)
                    {
                        _categorizedItems["decoration"].Add(item.Key, item.Value);
                    }
                    else if (itemInfo.itemType is ItemType.Armor)
                    {
                        _categorizedItems["armor"].Add(item.Key, item.Value);
                    }
                    else if (itemInfo.itemType is ItemType.Tool or ItemType.WateringCan)
                    {
                        _categorizedItems["tool"].Add(item.Key, item.Value);
                    }
                    else if (itemInfo.itemType is ItemType.Food)
                    {
                        _categorizedItems["food"].Add(item.Key, item.Value);
                    }
                    else if (itemInfo.itemType is ItemType.Crop)
                    {
                        _categorizedItems["crop"].Add(item.Key, item.Value);
                    }
                    else if (itemInfo.itemType is ItemType.Fish)
                    {
                        _categorizedItems["fish"].Add(item.Key, item.Value);
                    }
                    else if (itemInfo.itemType is ItemType.Pet)
                    {
                        _categorizedItems["pet"].Add(item.Key, item.Value);
                    }
                    else
                    {
                        _categorizedItems["normal"].Add(item.Key, item.Value);
                    }
                }
            }

			return _categorizedItemsAdded = true;
        }

		private static readonly Regex _npcNameRegex = new(@"[a-zA-Z\s\.]+", RegexOptions.Compiled);
		private static bool ProcessMarry(string commandInput, bool unmarry = false)
		{
			string[] mayCommandParam = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			static void Marry(NPCAI npcai, bool unmarry = false, bool notify = false)
			{
				//if (!SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships.ContainsKey(npcai.NPCName))
				//{
				//	return;
				//}

				if (!unmarry)
				{
					if (SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships.ContainsKey(npcai.OriginalName))
					{
						SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.Relationships[npcai.OriginalName] = 100f;
					}

					npcai.MarryPlayer();
					MessageToChat($"Married with {npcai.OriginalName.ColorText(Color.white)}!".ColorText(CommandExtension.GreenColor));
				}
				else
				{
					npcai.MarryPlayer();
					string progressStringCharacter = SingletonBehaviour<GameSave>.Instance.GetProgressStringCharacter("MarriedWith");

					if (!string.IsNullOrWhiteSpace(progressStringCharacter))
					{
						SingletonBehaviour<GameSave>.Instance.SetProgressStringCharacter("MarriedWith", "");
						SingletonBehaviour<GameSave>.Instance.SetProgressBoolCharacter("Married", false);
						SingletonBehaviour<GameSave>.Instance.SetProgressBoolCharacter("MarriedTo" + progressStringCharacter, false);
						GameSave.CurrentCharacter.Relationships[progressStringCharacter] = 40f;
						SingletonBehaviour<NPCManager>.Instance.GetRealNPC(progressStringCharacter).GenerateCycle(false);
					}

					MessageToChat($"Divorced from {npcai.OriginalName.ColorText(Color.white)}!".ColorText(CommandExtension.GreenColor));
				}
			}

			static string GetNpcName(string name)
			{
				Match match = _npcNameRegex.Match(name);
				return match.Success ? match.Value : string.Empty;
			}

			static string FirstCharToUpper(string input)
			{
				return string.IsNullOrEmpty(input) ? input : input.First().ToString().ToUpper() + input.Substring(1);
			}

            if (mayCommandParam.Length >= 2)
            {
				string name = FirstCharToUpper(mayCommandParam[1]);
                bool all = mayCommandParam[1] == "all";
                NPCAI[] npcs = UnityEngine.Object.FindObjectsOfType<NPCAI>();

                if (npcs == null || npcs.Length == 0)
                {
					MessageToChat($"No NPC's found.".ColorText(CommandExtension.RedColor));
					return true;
                }

                if (all)
                {
                    foreach (NPCAI npc in npcs)
                    {
						if (npc != null)
						{
							Marry(npcai: npc, unmarry: unmarry, notify: true);
						}
                    }
                }
                else
                {
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


		// ============================================================
		// Structs
		// ============================================================

		private readonly struct CommandDefaultValues
		{
			public const float timeMultiplier = 0.2F;
		}

		private readonly struct TeleportLocation(Vector2 position, string destination)
		{
			public readonly Vector2 Position { get; } = position;
			public readonly string Destination { get; } = destination;
		}
	}

	/// <summary>
	/// Attribute-marked container that exposes in-game console commands to the game's
	/// command generator. Methods in this class annotated with the [Command] attribute
	/// are discovered by the game and registered as chat commands using the configured
	/// command prefix applied to the class (see <see cref="CommandPrefixAttribute"/>).
	/// 
	/// Behavior:
	/// Methods annotated with [Command] are registered as commands shown in the chat
	/// box and participate in the game's autocomplete (Tab key) and suggestion system.
	/// </summary>
#pragma warning disable IDE0060 // Remove unused parameter
	[CommandPrefix("!")]
    public partial class CommandsToGenerate
    {
		// Help
		[Command(Commands.CmdKeyHelp, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
        private static void GenerateCommand_Help() { }


		// State
		[Command(Commands.CmdKeyState, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
        private static void GenerateCommand_State() { }

		// Clear Chat
		[Command(Commands.CmdKeyClearChat, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_ClearChat() { }

		// Feedback toggle
		//[Command(Commands.CmdKeyFeedbackDisabled, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		//private static void GenerateCommand_FeedbackDisabled() { }


		// Command target name
		//[Command(Commands.CmdKeyName, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		//private static void GenerateCommand_Name(string playerName) { }


		// Mine commands
		[Command(Commands.CmdKeyMineReset, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_MineReset() { }

		[Command(Commands.CmdKeyMineClear, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_MineClear() { }

		[Command(Commands.CmdKeyMineOverfill, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_MineOverfill() { }


		// Time commands
		[Command(Commands.CmdKeyPause, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_Pause() { }

		[Command(Commands.CmdKeyTimespeed, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_CustomDaySpeed(string Value_or_Nothing_to_reset) { }

		[Command(Commands.CmdKeyDate, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		public static void GenerateCommand_SetDate(string DayOrHoure_and_Value) { }

		[Command(Commands.CmdKeySeason, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_SetSeason(string season) { }

		[Command(Commands.CmdKeyYear, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_Years(string amount) { }

		[Command(Commands.CmdKeyWeather, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		public static void GenerateCommand_Weather(string raining_heatwave_clear) { }

		//[Command(Commands.CmdFixYear, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		//private static void fm40(string value) { }


		// Currency commands
		[Command(Commands.CmdKeyMoney, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_Money(string amount) { }

		[Command(Commands.CmdKeyCoins, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_Coins(string amount) { }

		[Command(Commands.CmdKeyOrbs, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
        private static void GenerateCommand_Orbs(string amount) { }

        [Command(Commands.CmdKeyTickets, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
        private static void GenerateCommand_Tickets(string amount) { }


		// Player
		[Command(Commands.CmdKeySleep, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_Sleep() { }

		[Command(Commands.CmdKeyDashInfinite, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_DashInfinite() { }

		[Command(Commands.CmdKeyDasher, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_Dasher() { }

		[Command(Commands.CmdKeyJumper, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_Jumper() { }

		[Command(Commands.CmdKeyJumpOver, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_JumpOver() { }

		[Command(Commands.CmdKeyNoclip, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_NoClip() { }

		[Command(Commands.CmdKeyManaFill, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_ManaFill() { }

		[Command(Commands.CmdKeyManaInfinite, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_ManaInf() { }

		[Command(Commands.CmdKeyHealthFill, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_HealthFill() { }

		[Command(Commands.CmdKeyNoHit, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_NoHit() { }


		// Misc
		[Command(Commands.CmdKeyAutoFillMuseum, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_AutoFillMuseum() { }

		[Command(Commands.CmdKeyCheatFillMuseum, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_CheatFillMuseum() { }

		[Command(Commands.CmdKeyUI, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCompmand_UI(string On_or_Off) { }

		[Command(Commands.CmdKeyCheats, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_Cheats() { }


		// NPC relationship
		[Command(Commands.CmdKeyMarry, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_Marry(string NPC_Name) { }

		[Command(Commands.CmdKeyDivorce, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_Unmarry(string NPC_Name) { }

		[Command(Commands.CmdKeyRelationship, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_Relationship(string NPC_Name_and_value) { }


		// Items
		[Command(Commands.CmdKeyShowCategorizedItems, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_CategorizedItems(string xp_currency_bonus_Pet_decoration_armor_tool_food_crop_fish_normal) { }

		[Command(Commands.CmdKeyGiveItem, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
        private static void GenerateCommand_Give(string itemName) { }

        [Command(Commands.CmdKeyShowItem, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
        private static void GenerateCommand_ShowItems(string itemName) { }

		[Command(Commands.CmdKeyShowItemInfoOnHover, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_ShowItemIdOnHover() { }

		[Command(Commands.CmdKeyDevKit, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
        private static void GenerateCommand_DevKit() { }

		[Command(Commands.CmdKeyShowItemInfoOnTooltip, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_ShowItemIdOnTooltip(string INFO_ShowsItemIdsInDescription) { }

		// Teleport
		[Command(Commands.CmdKeyTeleport, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_Teleport(string location) { }

		[Command(Commands.CmdKeyTeleportLocations, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
		private static void GenerateCommand_TeleportLocations() { }
	}
#pragma warning restore IDE0060 // Remove unused parameter
}
