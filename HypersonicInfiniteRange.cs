using MelonLoader;
using BTD_Mod_Helper;
using System.Linq;
using Hypersonic;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using HarmonyLib;
using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;

[assembly: MelonInfo(typeof(HypersonicSimple), "SimpleHypersonic", ModHelperData.Version, "nolife99")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace Hypersonic;

public class HypersonicSimple : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        LoggerInstance.Msg("Loaded hypersonic!");
        HarmonyInstance.Patch(AccessTools.Method(typeof(GameModelLoader), nameof(GameModelLoader.Load)), 
            postfix : new(AccessTools.Method(typeof(HypersonicSimple), nameof(HypersonicSimple.OnGameLoad))));
    }
    public static void OnGameLoad(ref GameModel __result) => __result.towers.AsParallel().ForAll(t =>
    {
        if (Settings.InfiniteRange) t.range = 999;
        t.behaviors.Do(m =>
        {
            var potentialAttack = m.TryCast<AttackModel>();
            if (potentialAttack != null)
            {
                if (Settings.InfiniteRange) potentialAttack.range = 999;
                var wep = potentialAttack.weapons;
                if (Settings.Hypersonic) wep.Do(w => w.rate = 0);

                var length = wep.Length;
                for (var j = 0; j < length; j++) wep = wep.AddItem(wep[j]).ToArray();
            }
        });
    });
}
public class Settings : ModSettings
{
    public static readonly ModSettingBool Hypersonic = new(true)
    {
        displayName = "Enable hypersonic",
        icon = VanillaSprites.FasterBarrelSpinUpgradeIcon
    };
    public static readonly ModSettingBool InfiniteRange = new(true)
    {
        displayName = "Enable infinite range",
        icon = VanillaSprites.EpicRangeUpgradeIcon
    };
}