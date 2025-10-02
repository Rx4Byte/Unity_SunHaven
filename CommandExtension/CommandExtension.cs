using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace CommandExtension
{
    public class PluginInfo
    {
        public const string PLUGIN_NAME = "Command Extension";
        public const string PLUGIN_GUID = "com.Rx4Byte.CommandExtension";
        public const string PLUGIN_VERSION = "2.0.1";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public partial class CommandExtension : BaseUnityPlugin
	{
		public const int ACTUAL_PLAYER_INITIALIZATION = 2;  // the actual player spawned - used to greet player and setup commands
		public const string GREET_MESSAGE = "> Command Extension Active!";
        public const string GREET_INFO_MESSAGE = "     type '!help' for a list of commands.";
		public const bool DEBUG = false;
		public const bool DEBUG_LOG = DEBUG && false;
		public const bool DEBUG_HELPER = DEBUG && true;

		public static Color NormalColor { get; private set; } = new(1.0f, 0.66f, 0.0f);
		public static Color GreetColor { get; private set; } = new(1.0f, 0.66f, 0.0f);
		public static Color GreetInfoColor { get; private set; } = new(0.7f, 0.44f, 0.0f);
		public static Color RedColor { get; private set; } = new(0.776f, 0.157f, 0.157f);  //(1.0f, 0, 0.0f)
		public static Color GreenColor { get; private set; } = new(0.157f, 0.776f, 0.157f);  //(0.180f, 0.490f, 0.196f)
		public static Color BlueColor { get; private set; } = new(0.157f, 0.157f, 0.776f);  //
		public static Color MagentaColor { get; private set; } = new(0.533f, 0.000f, 0.533f, 1.000f);
		public static Color YellowColor { get; private set; } = new(0.984f, 0.882f, 0.176f);  //(0.94f, 0.94f, 0.0f)
		public static Color DarkGrayColor { get; private set; } = new(0.355f, 0.355f, 0.355f); //(0.1f, 0.1f, 0.1f)
		public static Color BlackColor { get; private set; } = new(0.08f, 0.08f, 0.08f);
		public static Color WhiteColor { get; private set; } = new(0.99f, 0.98f, 0.40f);

		/// <summary>
		/// Plugin entry point called by Unity when the component is initialized.
		/// Creates a Harmony instance and applies all patches found in the current assembly.
		/// The returned Harmony patch report is intentionally discarded.
		/// </summary>
		private void Awake()
        {
			_ = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.PLUGIN_GUID);
        }
	}
}
