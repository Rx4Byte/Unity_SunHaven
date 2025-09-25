using BepInEx;
using HarmonyLib;
using System;
using QFSW.QC.Utilities;
using System.Reflection;
using BepInEx.Configuration;
using Wish;
using QFSW.QC;
using UnityEngine;

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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HungryMonster), nameof(HungryMonster.SetMeta))]
        private static void HungryMonster_SetMeta(HungryMonster __instance)
        {
			// Only proceed for actual museum bundles
			if (!ModEnabled.Value || __instance.bundleType != BundleType.MuseumBundle)
			{
				return;
			}

			// Ensure a valid player context for auto-fill
			Player player = Player.Instance;
			if (player.Inventory == null || player.Inventory.Items == null)
			{
				return;
			}

			Inventory playerInventory = player.Inventory;


			HungryMonster monster = __instance;
			if (monster.sellingInventory == null || monster.sellingInventory.Items == null)
			{
				return;
			}

			Inventory monsterInventory = monster.sellingInventory;

			foreach (SlotItemData moonsterItem in monsterInventory.Items)
					{
						if (moonsterItem.slot.numberOfItemToAccept == 0 || moonsterItem.amount >= moonsterItem.slot.numberOfItemToAccept)
						{
							continue;
						}

						if (monster.name.ToLower().Contains("money"))
						{
							continue;
						}

						foreach (SlotItemData playerItem in playerInventory.Items)
						{
							if (playerItem.id != moonsterItem.slot.serializedItemToAccept.id)
							{
								continue;
							}

							int amount = Math.Min(playerItem.amount, moonsterItem.slot.numberOfItemToAccept - moonsterItem.amount);

							monsterInventory.AddItem(item: playerItem.id,
								amount: amount,
								slot: moonsterItem.slotNumber,
								sendNotification: false);

							string itemName = "unkown";
							if (ItemInfoDatabase.Instance.allItemSellInfos.TryGetValue(playerItem.id, out ItemSellInfo itemSellInfo))
							{
								itemName = itemSellInfo.name;
							}

							QuantumConsole.Instance.LogPlayerText($"added: {amount.ToString().ColorText(Color.white)} * "
								+ $"{itemName.ColorText(Color.green)}"
								+ " to the museum!");
							_ = playerInventory.RemoveItem(playerItem.item, amount);

							monster.SaveMeta();
							monster.SendNewMeta(monster.meta);
							monster.UpdateFullness(true);
						}
					}

            Array.ForEach(UnityEngine.Object.FindObjectsOfType<MuseumBundleVisual>(), vPodium => typeof(MuseumBundleVisual).GetMethod("OnSaveInventory", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(vPodium, null));
        }
    }
}
