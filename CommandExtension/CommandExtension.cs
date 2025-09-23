using System.Reflection;
using BepInEx;
using HarmonyLib;
using QFSW.QC.Utilities;
using UnityEngine;

namespace CommandExtension
{
    public class PluginInfo
    {
        public const string PLUGIN_NAME = "Command Extension";
        public const string PLUGIN_GUID = "com.Rx4Byte.CommandExtension";
        public const string PLUGIN_VERSION = "1.2.3";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public partial class CommandExtension : BaseUnityPlugin
	{
		public const string GREET_MESSAGE = "> Command Extension Active!";
        public const string GREET_INFO_MESSAGE = "     type '!help' for a list of commands.";
		public const int PLAYER_INITIALIZATION_COUNT_WANTED = 2;  // the actual player spawned - used to greet player and setup commands
		public const bool DEBUG = false;
		public const bool DEBUG_LOG = DEBUG && false;
		public const bool DEBUG_HELPER = DEBUG && true;

		public static string MessageSeparator { get; private set; } = "  -  ".ColorText(BlackColor);
		public static Color NormalColor { get; private set; } = new(1F, 0.66F, 0.0F);
		public static Color GreetColor { get; private set; } = new(1F, 0.66F, 0.0F);
		public static Color GreetInfoColor { get; private set; } = new(0.7F, 0.44F, 0.0F);
		public static Color RedColor { get; private set; } = new(255, 0, 0);
		public static Color GreenColor { get; private set; } = new(0, 255, 0);
		public static Color YellowColor { get; private set; } = new(240, 240, 0);
		public static Color BlackColor { get; private set; } = Color.black;
		public static Color DarkGrayColor { get; private set; } = new(0.2f, 0.2f, 0.2f); //new(0.1f, 0.1f, 0.1f)


		private void Awake()
        {
			_ = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

		public static void GreetDebug()
		{
			CommandMethodes.MessageToChat(GREET_MESSAGE.ColorText(GreetColor));
			CommandMethodes.MessageToChat("DEBUG".ColorText(Color.magenta)
					+ (DEBUG_LOG ? " -DEBUG_LOG".ColorText(Color.magenta) : "")
					+ (DEBUG_HELPER ? " -DEBUG_HELPER".ColorText(Color.magenta) : ""));

			if (DEBUG_HELPER)
			{
				_ = CommandMethodes.CommandFunction_ManaInfinite(Commands.CmdKeyManaInfinite);
				_ = CommandMethodes.CommandFunction_DashInfinite(Commands.CmdKeyDashInfinite);
				_ = CommandMethodes.CommandFunction_JumpOver(Commands.CmdKeyJumpOver);
				_ = CommandMethodes.CommandFunction_NoHit(Commands.CmdKeyNoHit);
				_ = CommandMethodes.CommandFunction_Pause(Commands.CmdKeyPause);
			}
		}

		public static void GreetNormal()
		{
			CommandMethodes.MessageToChat(GREET_MESSAGE.ColorText(GreetColor));
			CommandMethodes.MessageToChat(GREET_INFO_MESSAGE.ColorText(GreetInfoColor));
		}
	}
}
