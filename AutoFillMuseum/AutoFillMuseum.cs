using BepInEx;
using HarmonyLib;
using System;
using System.Reflection;
using BepInEx.Configuration;
using Wish;
using UnityEngine;
using QFSW.QC;
using QFSW.QC.Utilities;

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
			_ = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.PLUGIN_GUID);
        }

		[HarmonyPatch(typeof(HungryMonster), nameof(HungryMonster.SetMeta))]
		public static void Postfix(HungryMonster __instance, DecorationPositionData decorationData)
		{
			// Only proceed for actual museum bundles
			if (!ModEnabled.Value || __instance.bundleType != BundleType.MuseumBundle || __instance.name.ToLower().Contains("money"))
			{
				return;
			}

			// Ensure a valid player context for auto-fill
			if (Player.Instance == null || Player.Instance.Inventory == null || Player.Instance.Inventory.Items == null)
			{
				return;
			}

			Player player = Player.Instance;

			HungryMonster monster = __instance;
			if (monster.sellingInventory == null || monster.sellingInventory.Items == null)
			{
				return;
			}

			foreach (SlotItemData monsterSlotItemData in monster.sellingInventory.Items)
			{
				if (monsterSlotItemData == null || monsterSlotItemData.item == null
					|| monsterSlotItemData.slot.numberOfItemToAccept == 0 || monsterSlotItemData.amount >= monsterSlotItemData.slot.numberOfItemToAccept)
				{
					continue;
				}

				foreach (SlotItemData playerSlotItemData in player.Inventory.Items)
				{
					if (playerSlotItemData == null || playerSlotItemData.id != monsterSlotItemData.slot.serializedItemToAccept.id || playerSlotItemData.amount <= 0)
					{
						continue;
					}
					
					int transferAmount = Math.Min(playerSlotItemData.amount, monsterSlotItemData.slot.numberOfItemToAccept - monsterSlotItemData.amount);

					monster.sellingInventory.AddItem(item: playerSlotItemData.id, amount: transferAmount, slot: monsterSlotItemData.slotNumber, sendNotification: false);

					ItemIcon itemIcon = monsterSlotItemData.slot.GetComponentInChildren<ItemIcon>();
					if (!itemIcon)
					{
						itemIcon = UnityEngine.Object.Instantiate<ItemIcon>(SingletonBehaviour<Prefabs>.Instance.ItemIcon, monsterSlotItemData.slot.transform);
						monsterSlotItemData.slot.ModifyItemQuality(monsterSlotItemData.item);
						itemIcon.Initialize(monsterSlotItemData);
					}

					itemIcon.UpdateAmount(monsterSlotItemData.slot.numberOfItemToAccept);

					string itemName = "unkown";
					if (ItemInfoDatabase.Instance.allItemSellInfos.TryGetValue(playerSlotItemData.id, out ItemSellInfo itemSellInfo))
					{
						itemName = itemSellInfo.name;
					}

					_ = player.Inventory.RemoveItem(id: playerSlotItemData.id, amount: transferAmount);
					QuantumConsole.Instance.LogPlayerText($"Removed: {transferAmount.ToString().ColorText(Color.white)} x " + $"{itemName.ColorText(Color.white)}");
				}
			}

			monster.SetupIconEvent();
			monster.UpdateFullness(true);

			MethodInfo OnSaveInventoryMethode = typeof(MuseumBundleVisual).GetMethod("OnSaveInventory", BindingFlags.Instance | BindingFlags.NonPublic);
			MuseumBundleVisual[] museumBundleVisuals = UnityEngine.Object.FindObjectsOfType<MuseumBundleVisual>();
			foreach (MuseumBundleVisual museumBundleVisual in museumBundleVisuals)
			{
				_ = OnSaveInventoryMethode.Invoke(museumBundleVisual, null);
			}
		}
    }
}
