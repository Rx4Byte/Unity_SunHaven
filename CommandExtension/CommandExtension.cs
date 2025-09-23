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
        public const string GREET = "> Command Extension Active!";
        public const string GREET_INFO = "     type '!help' for a list of commands.";
        public const string MESSAGE_GAP = "  -  ";
        public const bool DEBUG = true;
        public const bool DEBUG_LOG = DEBUG && true;

        public static Color GreetColor = new Color(1F, 0.66F, 0.0F);
        public static Color GreetInfoColor = new Color(0.7F, 0.44F, 0.0F);
        public static Color RedColor = new Color(255, 0, 0);
        public static Color GreenColor = new Color(0, 255, 0);
        public static Color YellowColor = new Color(240, 240, 0);

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }
    }
}