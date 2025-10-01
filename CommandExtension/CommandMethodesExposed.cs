using QFSW.QC;

namespace CommandExtension
{
	/// <summary>
	/// Attribute-marked container that exposes in-game console commands to the game's
	/// command generator. Methods in this class are annotated with the [Command] attribute
	/// to get discovered by the game and registered as chat commands using the configured
	/// command prefix applied to the class (see <see cref="CommandPrefixAttribute"/>).
	/// 
	/// Behavior:
	/// Methods annotated with [Command] are registered as commands shown in the chat
	/// box and participate in the game's autocomplete (Tab key) and suggestion system.
	/// </summary>
#pragma warning disable IDE0060 // Remove unused parameter
	[CommandPrefix(Commands.CmdPrefix)]
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
		private static void GenerateCommand_Divorce(string NPC_Name) { }

		[Command(Commands.CmdKeyUnmarry, QFSW.QC.Platform.AllPlatforms, QFSW.QC.MonoTargetType.Single)]
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
