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

namespace CommandExtension
{
    public class ItemDatabaseWrapper
    {
        public static ItemDatabaseWrapper Instance { get; private set; }
        private static Dictionary<int, ItemSellInfo> itemDatabase = ItemInfoDatabase.Instance.allItemSellInfos;
        private static List<int> keys = getItemDatabaseKeys();
        private static List<ItemSellInfo> values = getItemDatabaseValues();
        private static List<int> getItemDatabaseKeys()
        {
            List<int> keys__ = new List<int>();
            foreach (int item in itemDatabase.Keys)
            {
                keys__.Add(item);
            }
            return keys__;
        }

        private static List<ItemSellInfo> getItemDatabaseValues()
        {
            List<ItemSellInfo> values__ = new List<ItemSellInfo>();
            foreach (ItemSellInfo item_info in itemDatabase.Values)
            {
                values__.Add(item_info);
            }
            return values__;
        }

        public Dictionary<int, ItemSellInfo> GetItemDatabase() { return itemDatabase; }

        public List<int> GetKeys()
        {
            return keys;
        }

        public List<ItemSellInfo> GetValues()
        {
            return values;
        }

        public ItemDatabaseWrapper()
        {
        }

        public class ItemDatabase : MonoBehaviour
        {
            public static Dictionary<int, ItemData> itemDatas;
            public static Dictionary<string, int> ids = new Dictionary<string, int>();
            public static bool constructed;

            public static void ConstructDatabase(ItemData[] itemArray)
            {
                if (ItemDatabase.constructed)
                    return;
                ItemDatabase.itemDatas = new Dictionary<int, ItemData>();
                for (int index = 0; index < itemArray.Length; ++index)
                {
                    ItemDatabase.itemDatas[itemArray[index].id] = itemArray[index];
                    string lower = itemArray[index].name.RemoveWhitespace().ToLower();
                    ItemDatabase.ids.Add(lower, itemArray[index].id);
                    itemArray[index].Awake();
                }
                Debug.Log((object)ItemDatabase.ids.Count);
                ItemDatabase.constructed = true;
            }

            public static void ConstructDatabase()
            {
                int num = ItemDatabase.constructed ? 1 : 0;
            }

            public static void DebugItemList()
            {
                foreach (KeyValuePair<string, int> id in ItemDatabase.ids)
                    Debug.Log((object)(id.Value.ToString() + " - " + id.Key));
            }

            public static int GetID(string tileName)
            {
                int num;
                return ItemDatabase.ids.TryGetValue(tileName.RemoveWhitespace().ToLower(), out num) ? num : -1;
            }

            public static T GetItemData<T>(int id) where T : ItemData
            {
                if (!ItemDatabase.itemDatas.ContainsKey(id))
                    return default(T);
                return ItemDatabase.itemDatas[id] is T ? (T)ItemDatabase.itemDatas[id] : default(T);
            }

            public static T GetItemData<T>(Item item) where T : ItemData
            {
                return ItemDatabase.GetItemData<T>(item.ID());
            }

            public static ItemData GetItemData(int id)
            {
                ItemData itemData;
                return !ItemDatabase.itemDatas.TryGetValue(id, out itemData) ? (ItemData)null : itemData;
            }

            public static ItemData GetItemData(Item item)
            {
                ItemData itemData;
                return !ItemDatabase.itemDatas.TryGetValue(item.ID(), out itemData) ? (ItemData)null : itemData;
            }

            public static bool ValidID(int id) => (UnityEngine.Object)ItemDatabase.GetItemData(id) != (UnityEngine.Object)null;
        }
    }
}