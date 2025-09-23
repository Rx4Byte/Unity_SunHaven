using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using QFSW.QC;
using QFSW.QC.Utilities;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using System.IO;
using Wish;
using System.Runtime.Remoting.Messaging;
using PSS;

namespace CommandExtension
{
    public class ItemDatabase : MonoBehaviour
    {
        public static Dictionary<string, int> itemIds;
        public static Dictionary<int, ItemData> itemDatas;
        public static Dictionary<int, ItemSellInfo> allItemSellInfos = ItemInfoDatabase.Instance.allItemSellInfos;
        public static Dictionary<int, ArmorInfo> allArmorInfos = ItemInfoDatabase.Instance.allArmorInfos;
        public static List<int> allDecorations = ItemInfoDatabase.Instance.allDecorations;
        public static Dictionary<int, CropInfo> cropInfos = ItemInfoDatabase.Instance.cropInfos;
        public static Dictionary<int, ForageableInfo> forageableInfos = ItemInfoDatabase.Instance.forageableInfos;

        public static bool Initialize()
        {
            ConstructDatabases();
            System.Random random = new System.Random();
            random.Next();
            for (int i = 0; i < 10; i++)
            {
                int ran = random.Next(allItemSellInfos.Count - 1);
                if (allItemSellInfos.TryGetValue(ran, out ItemSellInfo itemInfo))
                {
                    CommandMethodes.MessageToChat($"{itemInfo.keyName} - {itemInfo.name}");
                }
            }

            //CommandMethodes.GetPlayerForCommand().Inventory.AddItem(, 1, 0, true);
            //CommandMethodes.MessageToChat($"");
            return true;
        }

        public static ItemData GetItemData(string name)
        {
            return null;
        }

        public static ItemData GetItemData(int id)
        {
            return null;
        }

        private static bool ValidID(int id)
        {
            return (UnityEngine.Object)ItemDatabase.GetItemData(id) != (UnityEngine.Object)null;
        }

        private static void ConstructDatabases()
        {
            
        }
    }
}