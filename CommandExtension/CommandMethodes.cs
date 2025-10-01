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
		public static string CharacterName { get; set; }
		public static int PlayerInitializationCount { get; set; } = 0;
        public static bool UiVisible { get; set; } = true;
		public static float TimeMultiplier { get; set; } = CommandDefaultValues.timeMultiplier;  // timespeed

		private static readonly Dictionary<string, int> _itemIds = (Dictionary<string, int>)typeof(Database).GetField("ids", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Database.Instance);  // timespeed
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
		};  // timespeed
		private static bool _categorizedItemsAdded = false;  // timespeed

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
		public static void CommandFunction_Debug(string commandInput)
        {
			if (!CommandExtension.DEBUG)
			{
				return;
			}
			//string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);
			//
			//if (!(commandTokens.Length >= 2))
			//{
			//
			//}
			//else
			//{
			//	string param = commandTokens[1];
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
        }

        // Help
        public static void CommandFunction_Help(string commandInput)
        {
            string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			// When tokens >= 2, treat second token as the target command and print its help, if not, print general help
			if (commandTokens.Length >= 2)
            {
				// If the token is an alias, resolve it to the real command key, otherwise return the token unchanged
				string param1 = Commands.ResolveCommandAlias(commandTokens[1]);

				// If the command key is found, show detailed help for the command: <prefix+command>, usage, and description.
				// Else notify the user that the command was not found
				if (Commands.GeneratedCommands.TryGetValue(param1, out Command command))
				{
					Color commandKeyColor = command.State == CommandState.None ? CommandExtension.YellowColor
						: command.State == CommandState.Activated ? CommandExtension.GreenColor
						: CommandExtension.RedColor;

					MessageToChat($"[{param1.ColorText(commandKeyColor)} - Command]".ColorText(CommandExtension.BlackColor)
							+ "\n" + command.Usage.ColorText(CommandExtension.WhiteColor)
							+ "  -  " + command.Description.ColorText(CommandExtension.NormalColor));
				}
				else
				{
					MessageToChat($"Unknown Command: {param1.ColorText(CommandExtension.WhiteColor)}".ColorText(CommandExtension.RedColor));
					MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
				}
            }
            else
            {
                MessageToChat($"[{"List of Commands".ColorText(CommandExtension.DarkGrayColor)}]".ColorText(CommandExtension.BlackColor));
				foreach (KeyValuePair<string, Command> command in Commands.GeneratedCommands)
				{
					if (command.Value.State == CommandState.None)
					{
						MessageToChat($"{command.Key.ColorText(CommandExtension.YellowColor)} {command.Value.Description.ColorText(CommandExtension.NormalColor)}");
					}
					else if (command.Value.State == CommandState.Activated)
					{
						MessageToChat($"{command.Key.ColorText(CommandExtension.GreenColor)} {command.Value.Description.ColorText(CommandExtension.NormalColor)}");
					}
					else if (command.Value.State == CommandState.Deactivated)
					{
						MessageToChat($"{command.Key.ColorText(CommandExtension.RedColor)} {command.Value.Description.ColorText(CommandExtension.NormalColor)}");
					}
				}
			}
        }

        // State
        public static void CommandFunction_State(string commandInput)
        {
            //List<string> activeToggleMessages = [];
            //foreach (KeyValuePair<string, Command> command in Commands.GeneratedCommands)
            //{
            //    if (command.Value.State == CommandState.Activated)
            //    {
            //        activeToggleMessages.Add($"{command.Key.ColorText(CommandExtension.YellowColor)} {command.Value.State.ToString().ColorText(CommandExtension.GreenColor)}");
            //    }
            //}

			if (Commands.GeneratedCommands.Values.Any(command => command.State == CommandState.Activated))
			{
				MessageToChat($"[{"States".ColorText(CommandExtension.DarkGrayColor)}]".ColorText(CommandExtension.BlackColor));

				foreach (KeyValuePair<string, Command> generatedCommand in Commands.GeneratedCommands)
				{
					if (generatedCommand.Value.State == CommandState.Activated)
					{
						MessageToChat($"{generatedCommand.Key.ColorText(CommandExtension.YellowColor)} {generatedCommand.Value.State.ToString().ColorText(CommandExtension.GreenColor)}");
					}
				}

				return;
			}

			MessageToChat("No active commands".ColorText(CommandExtension.NormalColor));

			//if (activeToggleMessages.Count >= 1)
			//{
			//    MessageToChat($"[{"States".ColorText(CommandExtension.DarkGrayColor)}]".ColorText(CommandExtension.BlackColor));
			//
			//    foreach (string message in activeToggleMessages)
			//    {
			//        MessageToChat(message);
			//    }
			//}
			//else
			//{
			//    MessageToChat("No active commands".ColorText(CommandExtension.NormalColor));
			//}
		}

		// Clear Chat
		public static void CommandFunction_ClearChat(string commandInput)
		{
			QuantumConsole.Instance.ClearConsole();
		}

		// Toggle disable command feedback (output)
		public static void CommandFunction_FeedbackDisabled(string commandInput)
		{
			_ = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Set Command target name
		//public static void CommandFunction_SetName(string commandInput)
		//{
		//	string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);
		//
		//	if (commandTokens.Length >= 2)
		//	{
		//		PlayerNameForCommands = commandTokens[1];
		//		MessageToChat($"Command target name set to {commandTokens[1].ColorText(Color.magenta)}!".ColorText(CommandExtension.YellowColor));
		//	}
		//	else
		//	{
		//		PlayerNameForCommands = PlayerNameForCommandsFirst;
		//		MessageToChat($"Command target name {"restoCommandExtension.RedColor".ColorText(CommandExtension.GreenColor)} to {PlayerNameForCommandsFirst.ColorText(Color.magenta)}!".ColorText(CommandExtension.YellowColor));
		//	}
		//}

		//// Time Commands
		// Pause
		public static void CommandFunction_Pause(string commandInput)
		{
			_ = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Custom time speed
		public static void CommandFunction_Timespeed(string commandInput)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);
			string command = commandTokens[0];

			// Detect whether a parameter was provided
			bool hasParams = commandTokens.Length >= 2;

			if (hasParams)
			{
				bool setValue = int.TryParse(commandTokens[1], out int value);
				bool setDefault = commandTokens[1].Contains("r") || commandTokens[1].Contains("d");

				// If a numeric value was provided, compute and set TimeMultiplier
				if (setValue)
				{
					TimeMultiplier = (float)Math.Round(20f / value, 4);
				}
				// If the parameter contains 'r' or 'd', treat as reset/default request
				else if (setDefault) // r = reset | d = default
				{
					TimeMultiplier = CommandDefaultValues.timeMultiplier;
				}
				// Unvalid parameter, show usage and return
				else
				{
					MessageToChat(Commands.GetCommandByKey(command).Usage.ColorText(CommandExtension.RedColor));
					return;
				}

				// Ensure the timespeed command is activated and dont notify player
				SetCommandState(commandKey: command, activate: true, notifyPlayer: false);

				string setToValue = setValue ? TimeMultiplier.ToString() : "Default";
				// Inform the player about the activation and the resulting multiplier
				MessageToChat($"Custom Dayspeed {"Activated".ColorText(CommandExtension.GreenColor)} and set to: {setToValue.ColorText(CommandExtension.WhiteColor)}".ColorText(CommandExtension.NormalColor));
			}
			else
			{
				_ = ToggleCommand(command, true);
			}
		}

		// Set Date (only hour and day!)
		// TODO: split into day command and hour command, rename years command to year
		public static void CommandFunction_Date(string commandInput)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (commandTokens.Length == 3)
			{
				DayCycle Date = DayCycle.Instance;
				if (int.TryParse(commandTokens[2], out int dateValue))
				{
					switch (commandTokens[1][0])
					{
						// day
						case 'd':
							if (dateValue is <= 0 or > 28)
							{
								MessageToChat($"day must be between {"1-28".ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.RedColor));
								return;
							}

							Date.Time = new DateTime(Date.Time.Year, Date.Time.Month, dateValue, Date.Time.Hour + 1, Date.Time.Minute, Date.Time.Second, Date.Time.Millisecond);
							_ = typeof(DayCycle).GetMethod("SetInitialTime", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DayCycle.Instance, null);
							MessageToChat($"Day set to {dateValue.ToString().ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
							break;

						// hour
						case 'h':
							if (dateValue is < 6 or > 22) // 6-23 
							{
								MessageToChat($"hour must be between {"6-23".ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.RedColor));
								return;
							}

							Date.Time = new DateTime(Date.Time.Year, Date.Time.Month, Date.Time.Day, dateValue + 1, Date.Time.Minute, Date.Time.Second, Date.Time.Millisecond);
							MessageToChat($"Hour set to {dateValue.ToString().ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
							break;
					}

					return;
				}
			}

			MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
		}

		// Set Season
		public static void CommandFunction_Season(string commandInput)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (commandTokens.Length < 2 || !Enum.TryParse(commandTokens[1], true, out Season season2))
			{
				MessageToChat("invalid season!".ColorText(CommandExtension.RedColor));
				MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
				return;
			}

			DayCycle Date = DayCycle.Instance;
			int targetYear = Date.Time.Year + (((int)season2 - (int)Date.Season + 4) % 4);

			DayCycle.Instance.Time = new DateTime(targetYear, Date.Time.Month, 1, Date.Time.Hour, Date.Time.Minute, 0, DateTimeKind.Utc).ToUniversalTime();
			_ = typeof(DayCycle).GetMethod("SetInitialTime", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DayCycle.Instance, null);

			MessageToChat("Season set to ".ColorText(CommandExtension.NormalColor) + season2.ToString().ColorText(CommandExtension.WhiteColor));
		}

		// Set Year
		public static void CommandFunction_Year(string commandInput)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (!int.TryParse(Regex.Match(commandInput, @"\d+").Value, out int value))
			{
				MessageToChat("Invalid value".ColorText(CommandExtension.RedColor));
				MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
				return;
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
					MessageToChat("Value must be greater than 0");
					return;
				}
			}
			else
			{
				newYear = Date.Time.Year + (value * 4);
			}

			DayCycle.Instance.Time = new DateTime(newYear, Date.Time.Month, Date.Time.Day, Date.Time.Hour, Date.Time.Minute, 0, DateTimeKind.Utc).ToUniversalTime();
			_ = typeof(DayCycle).GetMethod("SetInitialTime", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DayCycle.Instance, null);

			MessageToChat($"Year set to {(Date.Time.Year / 4).ToString().ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
		}

		// Set Weather
		public static void CommandFunction_Weather(string commandInput)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (commandTokens.Length >= 2)
			{
				DayCycle Date = DayCycle.Instance;
				switch (commandTokens[1][0])
				{
					case 'r': // rain toggle
						Date.SetToRaining(!Date.Raining);
						MessageToChat($"{"Raining".ColorText(CommandExtension.WhiteColor)} turned {(!Date.Raining ? "Off".ColorText(CommandExtension.RedColor) : "On".ColorText(CommandExtension.GreenColor))}!".ColorText(CommandExtension.NormalColor));
						break;

					case 'h': // heatwave toggle
						Date.SetToHeatWave(!Date.Heatwave);
						MessageToChat($"{"Heatwave".ColorText(CommandExtension.WhiteColor)} turned {(!Date.Heatwave ? "Off".ColorText(CommandExtension.RedColor) : "On".ColorText(CommandExtension.GreenColor))}!".ColorText(CommandExtension.NormalColor));
						break;

					case 'c': // clear both
						Date.SetToHeatWave(false);
						Date.SetToRaining(false);
						MessageToChat($"{"Sunny".ColorText(CommandExtension.WhiteColor)} weather turned {"On".ColorText(CommandExtension.GreenColor)}!".ColorText(CommandExtension.NormalColor));
						break;
				}

				return;
			}

			MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
		}

		// Year Fix
		public static void CommandFunction_ToggleYearFix(string commandInput)
		{
			_ = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		//// Player Commands
		// Jumper
		public static void CommandFunction_JumpOver(string commandInput)
		{
			_ = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Dasher (Infinite Air-skips)
		public static void CommandFunction_DashInfinite(string commandInput)
		{
			_ = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Refill Mana
		public static void CommandFunction_ManaFill(string commandInput)
		{
			Player.Instance.AddMana(Player.Instance.MaxMana, 1f);
			MessageToChat($"{CharacterName}'s ".ColorText(CommandExtension.MagentaColor)
				+ "Mana filled".ColorText(CommandExtension.NormalColor));
		}

		// Infinite Mana
		public static void CommandFunction_ManaInfinite(string commandInput)
		{
			_ = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Refill health
		public static void CommandFunction_HealthFill(string commandInput)
		{
			Player.Instance.Heal(Player.Instance.MaxMana, true, 1f);
			MessageToChat($"{CharacterName}'s ".ColorText(CommandExtension.MagentaColor)
				+ "Health filled".ColorText(CommandExtension.NormalColor));
		}

		// Nohit
		public static void CommandFunction_NoHit(string commandInput)
		{
			_ = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// noclip
		public static void CommandFunction_Noclip(string commandInput)
		{
			Player.Instance.rigidbody.bodyType = Commands.ToggleCommandState(Commands.CmdKeyNoclip) ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
			_ = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Sleep
		public static void CommandFunction_Sleep(string commandInput)
		{
			Player.Instance.SkipSleep();
			MessageToChat($"{"Slept".ColorText(CommandExtension.WhiteColor)} once!".ColorText(CommandExtension.NormalColor));
		}

		//// Mine commands
		// Mines Reset
		public static void CommandFunction_MinesReset(string commandInput)
		{
			if (UnityEngine.Object.FindObjectOfType<MineGenerator2>() != null)
			{
				_ = typeof(MineGenerator2).GetMethod("ResetMines", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(MineGenerator2.Instance, [(ushort)0]);
				MessageToChat($"Mine {"reseted".ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
			}
			else
			{
				MessageToChat("Must be inside a Mine!".ColorText(CommandExtension.RedColor));
			}
		}

		// Mines Clear
		public static void CommandFunction_MinesClear(string commandInput)
		{
			if (UnityEngine.Object.FindObjectOfType<MineGenerator2>() != null)
			{
				_ = typeof(MineGenerator2).GetMethod("ClearMine", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(MineGenerator2.Instance, null);
				MessageToChat($"Mine {"cleared".ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
			}
			else
			{
				MessageToChat("Must be inside a Mine!".ColorText(CommandExtension.RedColor));
			}
		}

		// Mines Overfill
		public static void CommandFunction_MinesOverfill(string commandInput)
		{
			if (UnityEngine.Object.FindObjectOfType<MineGenerator2>() != null)
			{
				for (int i = 0; i < 30; i++)
				{
					_ = typeof(MineGenerator2).GetMethod("GenerateRocks", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(MineGenerator2.Instance, null);
				}

				MessageToChat($"Mine {"Overfilled".ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
			}
			else
			{
				MessageToChat("Must be inside a Mine!".ColorText(CommandExtension.RedColor));
			}
		}

		//// NPC relationship Commands
		// Set relationship
		public static void CommandFunction_Relationship(string commandInput)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (commandTokens.Length >= 3)
			{
				string name = commandTokens[1];
				bool all = commandTokens[1] == "all";
				bool add = commandTokens.Length >= 4 && commandTokens[3] == "add";

				if (float.TryParse(commandTokens[2], out float value))
				{
					value = Math.Max(0, Math.Min(100, value));
					NPCAI[] npcs = UnityEngine.Object.FindObjectsOfType<NPCAI>();

					if (npcs == null || npcs.Length == 0)
					{
						MessageToChat($"No NPC's found.".ColorText(CommandExtension.RedColor));
						return;
					}
					
					if (all)
					{
						foreach (NPCAI npc in npcs)
						{
							if (add)
							{
								npc.AddRelationship(value);
							}
							else
							{
								SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.relationships[npc.OriginalName] = value;
							}

							MessageToChat($"Relationship with {npc.OriginalName.ColorText(CommandExtension.WhiteColor)} is now {SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.relationships[npc.OriginalName].ToString().ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
						}
					}
					else
					{
						NPCAI npc = npcs.FirstOrDefault(npc => GetNpcName(npc.OriginalName).ToLower() == name);
						if (npc != null)
						{
							if (add)
							{
								npc.AddRelationship(value);
								MessageToChat($"Relationship with {npc.OriginalName.ColorText(CommandExtension.WhiteColor)} is now {SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.relationships[npc.OriginalName].ToString().ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
							}
							else
							{
								SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.relationships[npc.OriginalName] = value;
								MessageToChat($"Relationship with {npc.OriginalName.ColorText(CommandExtension.WhiteColor)} is now {SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.relationships[npc.OriginalName].ToString().ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
							}

							
							return;
						}
						else
						{
							MessageToChat($"No NPC with the name {name.ColorText(CommandExtension.WhiteColor)} found!".ColorText(CommandExtension.RedColor));
						}
					}
				}
				else
				{
					MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
				}
			}
		}

		// Marry
		public static void CommandFunction_Marry(string commandInput)
		{
			Marry(commandInput: commandInput, unmarry: false);
		}

		// Umarry
		public static void CommandFunction_Divorce(string commandInput)
		{
			Marry(commandInput: commandInput, unmarry: true);
		}

		//// Item Commands
		// Give Item by Id or Name
		public static void CommandFunction_GiveItem(string commandInput)
        {
            string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            if (commandTokens.Length >= 2)
            {
                if (int.TryParse(commandTokens[1], out int itemId))
                {
                    if (_itemIds.Values.Contains(itemId))
                    {
                        int itemAmount = (commandTokens.Length >= 3 && int.TryParse(commandTokens[2], out itemAmount)) ? itemAmount : 1;
                        Player.Instance.Inventory.AddItem(itemId, itemAmount, 0, true, true);
                        string itemName = "unknown";
                        Database.GetData<ItemData>(itemId, delegate (ItemData data)
                        {
                            itemName = data.Name;
                        });
                        MessageToChat($"{CharacterName.ColorText(CommandExtension.MagentaColor)} got {itemAmount.ToString().ColorText(CommandExtension.WhiteColor)} x {itemName.ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
                    }
                    else
                    {
                        MessageToChat($"no item with id: {itemId.ToString().ColorText(CommandExtension.WhiteColor)} found!".ColorText(CommandExtension.RedColor));
                    }
                }
                else
                {
                    int itemsFound = 0;
                    int lastItemId = 0;
                    if (commandTokens.Length >= 2)
                    {
                        foreach (KeyValuePair<string, int> id in _itemIds)
                        {
                            if (id.Key.Contains(commandTokens[1].ToLower()))
                            {
                                lastItemId = id.Value;
                                itemsFound++;
                                if (commandTokens[1] == id.Key)
                                {
                                    int itemAmount = (commandTokens.Length >= 3 && int.TryParse(commandTokens[2], out itemAmount)) ? itemAmount : 1;
                                    Player.Instance.Inventory.AddItem(lastItemId, itemAmount, 0, true, true);
									string itemName = "unknown";
									Database.GetData(lastItemId, delegate (ItemData data)
									{
										itemName = data.Name;
									});
									MessageToChat($"{CharacterName.ColorText(CommandExtension.MagentaColor)} got {itemAmount.ToString().ColorText(CommandExtension.WhiteColor)} x {itemName.ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
									return;
                                }
                            }
                        }

                        if (itemsFound == 1)
                        {
                            int itemAmount = (commandTokens.Length >= 3 && int.TryParse(commandTokens[2], out itemAmount)) ? itemAmount : 1;
							string itemName = "unknown";
							Database.GetData(lastItemId, delegate (ItemData data)
							{
								itemName = data.Name;
							});
							Player.Instance.Inventory.AddItem(lastItemId, itemAmount, 0, true, true);
                        }
                        else if (itemsFound > 1)
						{
                            MessageToChat($"[{commandTokens[1].ColorText(CommandExtension.WhiteColor)}{"-Items".ColorText(CommandExtension.DarkGrayColor)}]".ColorText(CommandExtension.BlackColor));
                            MessageToChat("use a unique item name or id".ColorText(CommandExtension.RedColor));
                            foreach (KeyValuePair<string, int> id in _itemIds)
                            {
                                if (id.Key.Contains(commandTokens[1]))
                                {
                                    MessageToChat(id.Key + " : " + id.Value.ToString());
                                }
                            }
                        }
                        else
                        {
                            MessageToChat($"No item name contains {commandTokens[1].ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.RedColor));
                        }
                    }
                    else
                    {
                        MessageToChat($"Invalid Item ID!".ColorText(CommandExtension.RedColor));
                        MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
                    }

                    return;
                }
            }
            else
            {
                MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
            }
        }

        // Show Item/s by Name
        public static void CommandFunction_ShowItem(string commandInput)
        {
            string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            if (commandTokens.Length >= 2)
            {
                List<string> items = [];
                foreach (KeyValuePair<string, int> id in _itemIds)
                {
                    if (id.Key.ToLower().Contains(commandTokens[1]))
                    {
                        items.Add(id.Key + " : " + id.Value.ToString());
                    }
                }

                if (items.Count >= 1)
                {
					MessageToChat($"[{commandTokens[1].ColorText(CommandExtension.WhiteColor)}{"-Items".ColorText(CommandExtension.DarkGrayColor)}]".ColorText(CommandExtension.BlackColor));
                    foreach (string ítem in items)
                    {
                        MessageToChat(ítem);
                    }
                }
            }
		}

		// Show Categorized items
		public static void CommandFunction_ShowCategorizedItems(string commandInput)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

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
				return;
			}

			if (!(commandTokens.Length >= 2))
			{
				return;
			}

			bool getItems = commandTokens.Length >= 3 && commandTokens[2] == "get";
			int amount = (commandTokens.Length >= 4 && int.TryParse(commandTokens[2], out amount)) ? amount : 1;
			string category = commandTokens[1];

			if (_categorizedItems.TryGetValue(category, out Dictionary<string, int> items))
			{
				MessageToChat($"[{category.ColorText(CommandExtension.WhiteColor)}{"-Ids".ColorText(CommandExtension.DarkGrayColor)}]".ColorText(CommandExtension.BlackColor));

				foreach (KeyValuePair<string, int> item in _categorizedItems[category])
				{
					MuseumGiveOrPrintItem(id: item.Value, name: item.Key, amount: amount, give: getItems);
				}
			}
			else
			{
				MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
			}
		}

		// Give Dev-Kit
		public static void CommandFunction_DevKit(string commandInput)
		{
			foreach (int item in new int[] { 30003, 30004, 30005, 30006, 30007, 30008 })
			{
				Player.Instance.Inventory.AddItem(item, 1, 0, true);
			}

			MessageToChat($"{CharacterName.ColorText(CommandExtension.MagentaColor)} got a {"DevKit".ColorText(CommandExtension.WhiteColor)}".ColorText(CommandExtension.NormalColor));
		}

		// Show id on Item Tooltip
		public static void CommandFunction_ShowItemInfoOnTooltip(string commandInput)
        {
            _ = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
        }

        // Show id in chat on hover
        public static void CommandFunction_ShowItemInfoOnHover(string commandInput)
        {
            _ = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		//// Currency Commands
		// Set Coins (Money)
		public static void CommandFunction_Coins(string commandInput)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (!int.TryParse(Regex.Match(commandInput, @"\d+").Value, out int moneyAmount))
			{
				MessageToChat("Invalid amount".ColorText(CommandExtension.RedColor));
				MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
				return;
			}

			if (commandInput.Contains("-"))
			{
				Player.Instance.AddMoney(-moneyAmount, true, true, true);
				MessageToChat($"{CharacterName.ColorText(CommandExtension.MagentaColor)} paid {moneyAmount.ToString().ColorText(CommandExtension.WhiteColor)} Coins!".ColorText(CommandExtension.NormalColor));
			}
			else
			{
				Player.Instance.AddMoney(moneyAmount, true, true, true);
				MessageToChat($"{CharacterName.ColorText(CommandExtension.MagentaColor)} got {moneyAmount.ToString().ColorText(CommandExtension.WhiteColor)} Coins!".ColorText(CommandExtension.NormalColor));
			}
		}

		// Set Orbs
		public static void CommandFunction_Orbs(string commandInput)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (!int.TryParse(Regex.Match(commandInput, @"\d+").Value, out int moneyAmount))
			{
				MessageToChat("Something wen't wrong..".ColorText(CommandExtension.RedColor));
				MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
				return;
			}

			if (commandInput.Contains("-"))
			{
				Player.Instance.AddOrbs(-moneyAmount);
				MessageToChat($"{CharacterName.ColorText(CommandExtension.MagentaColor)} paid {moneyAmount.ToString().ColorText(CommandExtension.WhiteColor)} Orbs!".ColorText(CommandExtension.NormalColor));
			}
			else
			{
				Player.Instance.AddOrbs(moneyAmount);
				MessageToChat($"{CharacterName.ColorText(CommandExtension.MagentaColor)} got {moneyAmount.ToString().ColorText(CommandExtension.WhiteColor)} Orbs!".ColorText(CommandExtension.NormalColor));
			}
		}

		// Set Tickets
		public static void CommandFunction_Tickets(string commandInput)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (!int.TryParse(Regex.Match(commandInput, @"\d+").Value, out int moneyAmount))
			{
				MessageToChat("Something wen't wrong..".ColorText(CommandExtension.RedColor));
				MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
				return;
			}

			if (commandInput.Contains("-"))
			{
				Player.Instance.AddTickets(-moneyAmount);
				MessageToChat($"{CharacterName.ColorText(CommandExtension.MagentaColor)} paid {moneyAmount.ToString().ColorText(CommandExtension.WhiteColor)} Tickets!".ColorText(CommandExtension.NormalColor));
			}
			else
			{
				Player.Instance.AddTickets(moneyAmount);
				MessageToChat($"{CharacterName.ColorText(CommandExtension.MagentaColor)} got {moneyAmount.ToString().ColorText(CommandExtension.WhiteColor)} Tickets!".ColorText(CommandExtension.NormalColor));
			}
		}

		//// Teleport
		// Teleport To // TODO: 
		public static void CommandFunction_TeleportToScene(string commandInput)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			if (!(commandTokens.Length >= 2))
			{
				return;
			}
			
			string sceneName = commandTokens[1];

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
				MessageToChat($"Location {sceneName.ColorText(CommandExtension.WhiteColor)} not found".ColorText(CommandExtension.RedColor));
			}
		}

		// Show teleport locations
		public static void CommandFunction_TeleportLocations(string commandInput)
		{
			MessageToChat($"[{"Locations".ColorText(CommandExtension.WhiteColor)}]".ColorText(CommandExtension.BlackColor));
			foreach (string tpLocation in _teleportLocations.Keys)
			{
				MessageToChat(tpLocation);
			}
		}

		//// Misc Commands
		// Auto-fill museum
		public static void CommandFunction_AutoFillMuseum(string commandInput)
		{
			_ = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// Cheat-fill museum
		public static void CommandFunction_CheatFillMuseum(string commandInput)
		{
			_ = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
		}

		// UI
		public static void CommandFunction_UI(string commandInput)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			static void GameObjectSetActiveState(GameObject obj, bool state)
			{
				obj?.SetActive(state);
			}

			if (commandTokens.Length >= 1)
			{
				bool flag = true;

				if (commandTokens.Length >= 2)
				{
					if (commandTokens[1].Contains("on"))
					{
						flag = true;
					}
					else if (commandTokens[1].Contains("of"))
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

				MessageToChat("UI now ".ColorText(CommandExtension.NormalColor) + (flag ? "Visible".ColorText(CommandExtension.GreenColor) : "Hidden".ColorText(CommandExtension.RedColor)));
			}
		}

		// Toggle Cheats
		public static void CommandFunction_Cheats(string commandInput)
		{
			Settings.EnableCheats = ToggleCommand(commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], true);
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
			QuantumConsole.Instance.LogPlayerText(message);
		}

		private static bool ToggleCommand(string commandKey, bool notifyPlayer = true)
		{
			bool isActive = Commands.ToggleCommandState(commandKey);
			string state = isActive ? "On".ColorText(CommandExtension.GreenColor) : "Off".ColorText(CommandExtension.RedColor);

			if (notifyPlayer)
			{
				MessageToChat($"{commandKey.ColorText(CommandExtension.WhiteColor)} now {state}".ColorText(CommandExtension.NormalColor));
			}

			return isActive;
		}

		private static void SetCommandState(string commandKey, bool activate = true, bool notifyPlayer = true)
        {
            Commands.SetCommandState(commandKey, activate);
            string state = activate
                ? "On".ColorText(CommandExtension.GreenColor) : "Off".ColorText(CommandExtension.RedColor);

            if (notifyPlayer)
            {
                MessageToChat($"{commandKey.ColorText(CommandExtension.WhiteColor)} {state}".ColorText(CommandExtension.NormalColor));
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
		private static void Marry(string commandInput, bool unmarry = false)
		{
			string[] commandTokens = commandInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

			static void MarryNpcai(NPCAI npcai, bool unmarry = false, bool notify = false)
			{
				if (!unmarry)
				{
					if (SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.relationships.ContainsKey(npcai.OriginalName))
					{
						SingletonBehaviour<GameSave>.Instance.CurrentSave.characterData.relationships[npcai.OriginalName] = 100f;
					}

					npcai.MarryPlayer();
					MessageToChat($"Married with {npcai.OriginalName.ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
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

					MessageToChat($"Divorced from {npcai.OriginalName.ColorText(CommandExtension.WhiteColor)}!".ColorText(CommandExtension.NormalColor));
				}
			}

            if (commandTokens.Length >= 2)
            {
				string name = commandTokens[1];
                bool all = commandTokens[1] == "all";
                NPCAI[] npcs = UnityEngine.Object.FindObjectsOfType<NPCAI>();

                if (npcs == null || npcs.Length == 0)
                {
					MessageToChat($"No NPC's found.".ColorText(CommandExtension.RedColor));
					return;
                }

                if (all)
                {
                    foreach (NPCAI npc in npcs)
                    {
						if (npc != null)
						{
							MarryNpcai(npcai: npc, unmarry: unmarry, notify: true);
						}
                    }
                }
                else
                {
					NPCAI npc = npcs.FirstOrDefault(npc => GetNpcName(npc.OriginalName).ToLower() == name);
                    if (npc != null)
                    {
                        MarryNpcai(npcai: npc, unmarry: unmarry, notify: true);
                    }
                    else
                    {
                        MessageToChat($"No NPC with the name {name.ColorText(CommandExtension.WhiteColor)} found!".ColorText(CommandExtension.RedColor));
                    }
                }
            }
            else
            {
                MessageToChat(Commands.GetCommandByKey(commandTokens[0]).Usage.ColorText(CommandExtension.RedColor));
            }

            return;
		}

		private static string GetNpcName(string name)
		{
			Match match = _npcNameRegex.Match(name);
			return match.Success ? match.Value : string.Empty;
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
}
