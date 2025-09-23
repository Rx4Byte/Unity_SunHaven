using BepInEx;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using Wish;

namespace AutoFillMuseum
{
    public static class PluginInfo
    {
        public const string PLUGIN_NAME = "AutoFill Museum";
        public const string PLUGIN_GUID = "com.Rx4Byte.AutoFillMuseum";
        public const string PLUGIN_VERSION = "1.1.1";
    }

    [Harmony]
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class AutoFillMuseum : BaseUnityPlugin
    {
        private static ConfigEntry<bool> ModEnabled { get; set; }
        //private static ConfigEntry<bool> ShowNotifications { get; set; }

        private void Awake()
        {
            ModEnabled = Config.Bind("General", "Enabled", true, $"Enable {PluginInfo.PLUGIN_NAME}");
            //ShowNotifications = Config.Bind("General", "Show Notifications", true, "Show notifications when items are added to the museum");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.PLUGIN_GUID);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HungryMonster), nameof(HungryMonster.SetMeta))]
        private static void HungryMonster_SetMeta(HungryMonster __instance)
        {
            if (!ModEnabled.Value || __instance.bundleType != BundleType.MuseumBundle || Player.Instance == null || Player.Instance.Inventory == null || __instance.sellingInventory == null)
            {
                return;
            }

            HungryMonster monster = __instance;
            Inventory playerInventory = Player.Instance.Inventory;

            foreach (var slotItemData in monster.sellingInventory.Items.Where(slotItemData => slotItemData.item != null && slotItemData.slot.numberOfItemToAccept != 0 && slotItemData.amount < slotItemData.slot.numberOfItemToAccept))
            {
                if (monster.name.ToLower().Contains("money"))
                {
                    continue;
                }

                foreach (var playerItem in playerInventory.Items)
                {
                    if (playerItem.id != slotItemData.slot.serializedItemToAccept.id)
                    {
                        continue;
                    }

                    int amount = Math.Min(playerItem.amount, slotItemData.slot.numberOfItemToAccept - slotItemData.amount);

                    monster.sellingInventory.AddItem(playerItem.id, amount, slotItemData.slotNumber, false);
                    playerInventory.RemoveItem(playerItem.item, amount);

                    monster.SaveMeta();
                    monster.SendNewMeta(monster.meta);
                    monster.UpdateFullness(true);

                    //if (ShowNotifications.Value)
                    //{
                    //    string itemName = "unkown";
                    //    if (ItemInfoDatabase.Instance.allItemSellInfos.TryGetValue(playerItem.id, out ItemSellInfo itemSellInfo))
                    //    {
                    //        itemName = itemSellInfo.name;
                    //    }
                    //    //SingletonBehaviour<NotificationStack>.Instance.SendNotification($"Added {itemName} to the museum!", playerItem.id, amount);
                    //}
                }
            }

            Array.ForEach(UnityEngine.Object.FindObjectsOfType<MuseumBundleVisual>(), vPodium => typeof(MuseumBundleVisual).GetMethod("OnSaveInventory", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(vPodium, null));
        }
    }
}