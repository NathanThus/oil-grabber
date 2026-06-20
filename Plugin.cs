using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using Newtonsoft.Json;
using PerfectRandom.Sulfur.Core;
using PerfectRandom.Sulfur.Core.CharacterStats;
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
        var OilList = new List<EnhancementDTO>();
        var scrollList = new List<EnhancementDTO>();

        foreach (var enhancement in db.GetRawList())
        {
            List<ModifierDTO> itemModifiers = new();
            foreach (var mod in enhancement.modifiersApplied)
            {
                itemModifiers.Add(new ModifierDTO
                {
                    modifierName = mod.attribute.ToString(),
                    statModType = FromStatModTypeToString(mod.modType),
                    value = mod.value
                });
            }
            var enhancementDTO = new EnhancementDTO
            {
                name = enhancement.enchantmentName,
                modifiers = itemModifiers
            };

            if (enhancementDTO.name.Contains("Oil"))
            {
                OilList.Add(enhancementDTO);
            }
            else
            {
                scrollList.Add(enhancementDTO);
            }
        }

        string json = JsonConvert.SerializeObject(OilList, Formatting.Indented);

        string path = Path.Combine(Paths.GameRootPath, "oils.json");
        File.WriteAllText(path, json);

        json = JsonConvert.SerializeObject(scrollList, Formatting.Indented);
        
        path = Path.Combine(Paths.GameRootPath, "scrolls.json");
        File.WriteAllText(path, json);
    }

    
    public string FromStatModTypeToString(StatModType modtype) => modtype switch
    {
        StatModType.Flat        => "Flat",
        StatModType.PercentAdd  => "PercentAdd",
        StatModType.PercentMult => "PercentMult",
        _ => throw new ArgumentOutOfRangeException(nameof(modtype), $"Not expected direction value: {modtype}"),
    };
}
