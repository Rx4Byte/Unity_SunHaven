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
    public partial class ControllerBypass : BaseUnityPlugin
    {
        private void Awake() => Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

        [HarmonyPatch(typeof(Input), "GetButton")]
        class Patch_GetButton
        {
            static bool Prefix(string buttonName, ref bool __result)
            {
                return __result = false;
            }
        }

        [HarmonyPatch(typeof(Input), "GetButtonDown")]
        class Patch_GetButtonDown
        {
            static bool Prefix(string buttonName, ref bool __result)
            {
                return __result = false;
            }
        }

        [HarmonyPatch(typeof(Input), "GetButtonUp")]
        class Patch_GetButtonUp
        {
            static bool Prefix(string buttonName, ref bool __result)
            {
                return __result = false;
            }
        }

        [HarmonyPatch(typeof(Input), "GetAxis")]
        class Patch_GetAxis
        {
            static bool Prefix(string axisName, ref float __result)
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
        class Patch_GetAxisRaw
        {
            static bool Prefix(string axisName, ref float __result)
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
