using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using Newtonsoft.Json;
using PerfectRandom.Sulfur.Core;
using PerfectRandom.Sulfur.Core.Items;
using UnityEngine;

namespace OilGrabber;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private IEnumerator Start()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        while (!StaticInstance<AsyncAssetLoading>.Instance.loadingDone)
        {
            yield return null;
        }

        var database = StaticInstance<AsyncAssetLoading>.Instance.enchantmentDatabase;
        if (database == null)
        {
            Logger.LogError("Database not found");
            throw new ArgumentNullException(nameof(database));
        }

        ExportDB(database);
        Logger.LogMessage("Finished!");
    }

    public void ExportDB(EnchantmentDatabase db)
    {
        var list = new List<EnhancementDTO>();

        foreach (var enchantment in db.GetRawList())
        {
            list.Add(new EnhancementDTO
            {
                name = enchantment.enchantmentName,
                modifiers = enchantment.modifiersApplied,
            });
        }

        string json = JsonConvert.SerializeObject(list, Formatting.Indented);

        string path = Path.Combine(Paths.GameRootPath, "enchantments.json");
        File.WriteAllText(path, json);
    }
}
