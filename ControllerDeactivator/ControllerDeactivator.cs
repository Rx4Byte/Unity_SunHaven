using BepInEx;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace ControllerBypass
{
    public class PluginInfo
    {
        public const string PLUGIN_NAME = "Controller Deactivator";
        public const string PLUGIN_GUID = "com.Rx4Byte.ControllerDeactivator";
        public const string PLUGIN_VERSION = "1.0.1";
    }

#pragma warning disable IDE0060 // Remove unused parameter
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public partial class ControllerDeactivator : BaseUnityPlugin
    {
		private void Awake()
		{
			_ = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.PLUGIN_GUID);
		}

		[HarmonyPatch(typeof(Input), "GetButton")]
		public class Patch_GetButton
        {
			public static bool Prefix(string buttonName, ref bool __result)
            {
                return __result = false;
            }
        }

        [HarmonyPatch(typeof(Input), "GetButtonDown")]
		public class Patch_GetButtonDown
        {
			public static bool Prefix(string buttonName, ref bool __result)
            {
                return __result = false;
            }
        }

        [HarmonyPatch(typeof(Input), "GetButtonUp")]
		public class Patch_GetButtonUp
        {
			public static bool Prefix(string buttonName, ref bool __result)
            {
                return __result = false;
            }
        }

        [HarmonyPatch(typeof(Input), "GetAxis")]
		public class Patch_GetAxis
        {
			public static bool Prefix(string axisName, ref float __result)
            {
                if (axisName != "Mouse ScrollWheel")
                {
                    __result = 0f;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(Input), "GetAxisRaw")]
		public class Patch_GetAxisRaw
        {
			public static bool Prefix(string axisName, ref float __result)
            {
                if (axisName != "Mouse ScrollWheel")
                {
                    __result = 0f;
                    return false;
                }

                return true;
            }
        }
	}
#pragma warning restore IDE0060 // Remove unused parameter
}
