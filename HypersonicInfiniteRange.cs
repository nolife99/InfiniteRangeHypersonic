using HarmonyLib;
using MelonLoader;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper;
using System.Linq;

using HypersonicInfiniteRange;
[assembly: MelonInfo(typeof(ModMain), ModHelperData.Name, ModHelperData.Version, ModHelperData.Version)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace HypersonicInfiniteRange;

public sealed class ModMain : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        LoggerInstance.Msg("Loaded hypersonic!");
        HarmonyInstance.Patch(AccessTools.Method(typeof(GameModelLoader), nameof(GameModelLoader.Load)), 
            postfix : new(AccessTools.Method(typeof(ModMain), nameof(ModMain.OnGameLoad))));
    }
    public static void OnGameLoad(ref GameModel __result) => __result.towers.AsParallel().ForAll(t =>
    {
        if (Configs.InfiniteRange) t.range = 999;
        t.behaviors.Do(m =>
        {
            var pAtt = m.TryCast<AttackModel>();
            if (pAtt != null)
            {
                if (Configs.InfiniteRange) pAtt.range = 999;
                
                var wep = pAtt.weapons;
                if (Configs.Hypersonic) wep.Do(w => w.rate = 0);
                for (var i = 0; i < wep.Length; i++) wep = wep.AddItem(wep[i]).ToArray();
            }
        });
    });
    sealed class Configs : ModSettings
    {
        internal static readonly ModSettingBool Hypersonic = new(true)
        {
            displayName = "Enable hypersonic (requires restart)",
            icon = VanillaSprites.FasterBarrelSpinUpgradeIcon
        };
        internal static readonly ModSettingBool InfiniteRange = new(true)
        {
            displayName = "Enable infinite range (requires restart)",
            icon = VanillaSprites.EpicRangeUpgradeIcon
        };
    }
}
