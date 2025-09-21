using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wish;

namespace YearCorrection
{
    public class PluginInfo
    {
        public const string PLUGIN_NAME = "Year Correction";
        public const string PLUGIN_GUID = "com.Rx4Byte.YearCorrection";
        public const string PLUGIN_VERSION = "1.0";
    }

        [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
        public partial class YearCorrection : BaseUnityPlugin
        {
            private void Awake() => Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

        #region Patches
        // Year fix
        #region Patch_DayCycle.Year
        [HarmonyPatch(typeof(DayCycle))]
        [HarmonyPatch("Year", MethodType.Getter)]
        public static class Patch_DayCycleYear
        {
            public static bool Prefix(ref int __result)
            {
                __result = (DayCycle.Day - 1) / 112 + 1;
                return false;
            }
        }
        #endregion
        #endregion
    }

}
