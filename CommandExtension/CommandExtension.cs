using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using BepInEx;
using CommandExtension.Models;
using HarmonyLib;
using QFSW.QC;
using QFSW.QC.Utilities;
using UnityEngine;
using Wish;

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
        // debug var's
        public const bool debug = false;
        public const bool debugLog = debug && true;

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }
    }
}