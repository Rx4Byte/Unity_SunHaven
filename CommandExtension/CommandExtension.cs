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
		public const bool DEBUG = false;
		public const bool DEBUG_LOG = DEBUG && false;
		public const bool DEBUG_HELPER = DEBUG && true;
		public const int PLAYER_INITIALIZATION_COUNT_WANTED = 2;  // the actual player spawned - used to greet player and setup commands
		public const string GREET_MESSAGE = "> Command Extension Active!";
        public const string GREET_INFO_MESSAGE = "     type '!help' for a list of commands.";

		public static Color NormalColor { get; private set; } = new(1.0f, 0.66f, 0.0f);
		public static Color GreetColor { get; private set; } = new(1.0f, 0.66f, 0.0f);
		public static Color GreetInfoColor { get; private set; } = new(0.7f, 0.44f, 0.0f);
		public static Color RedColor { get; private set; } = new(1.0f, 0, 0.0f);
		public static Color GreenColor { get; private set; } = new(0.0f, 1.0f, 0.0f);
		public static Color YellowColor { get; private set; } = new(0.94f, 0.94f, 0.0f);
		public static Color BlackColor { get; private set; } = Color.black;
		public static Color DarkGrayColor { get; private set; } = new(0.235f, 0.235f, 0.235f); //new(0.1f, 0.1f, 0.1f)

		/// <summary>
		/// Plugin entry point called by Unity when the component is initialized.
		/// Creates a Harmony instance and applies all patches found in the current assembly.
		/// The returned Harmony patch report is intentionally discarded.
		/// </summary>
		private void Awake()
        {
			_ = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

		public static void GreetNormal()
		{
			CommandMethodes.MessageToChat(GREET_MESSAGE.ColorText(GreetColor));
			CommandMethodes.MessageToChat(GREET_INFO_MESSAGE.ColorText(GreetInfoColor));
		}

		public static void GreetDebug()
		{
			CommandMethodes.MessageToChat(GREET_MESSAGE.ColorText(GreetColor));
			CommandMethodes.MessageToChat("DEBUG".ColorText(Color.magenta)
					+ (DEBUG_LOG ? "    -DEBUG_LOG".ColorText(Color.magenta) : "")
					+ (DEBUG_HELPER ? "    -DEBUG_HELPER".ColorText(Color.magenta) : ""));

			if (DEBUG_HELPER)
			{
				_ = CommandMethodes.CommandFunction_Pause(Commands.CmdKeyPause);
				_ = CommandMethodes.CommandFunction_JumpOver(Commands.CmdKeyJumpOver);
				_ = CommandMethodes.CommandFunction_DashInfinite(Commands.CmdKeyDashInfinite);
				_ = CommandMethodes.CommandFunction_ManaInfinite(Commands.CmdKeyManaInfinite);
				_ = CommandMethodes.CommandFunction_NoHit(Commands.CmdKeyNoHit);
			}
		}
	}
}
