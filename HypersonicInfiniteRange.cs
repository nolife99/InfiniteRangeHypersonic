using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2Cpp;

using HarmonyLib;
using MelonLoader;

using System.Linq;
using System.Threading.Tasks;

using HypersonicInfiniteRange;

[assembly: MelonInfo(typeof(ModMain), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace HypersonicInfiniteRange;

public sealed class ModMain : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        MelonLogger.Msg("Loaded hypersonic!");
        var instance = new HarmonyLib.Harmony("69");
        instance.Patch(AccessTools.Method(typeof(GameModelLoader), nameof(GameModelLoader.Load)), 
            postfix: new(AccessTools.Method(typeof(ModMain), nameof(ModMain.OnGameLoad))));
    }
    public static void OnGameLoad(ref GameModel __result) => __result.towers.AsParallel().ForAll(t =>
    {
        if (Configs.InfiniteRange) t.range = 999;
        t.behaviors.Do(m =>
        {
            var attack = m.TryCast<AttackModel>();
            if (attack != null)
            {
                if (Settings.InfiniteRange)
                {
                    attack.range = 999;
                    attack.attackThroughWalls = true;
                }

                var wep = attack.weapons;
                if (Settings.Hypersonic) wep.Do(w => w.rate = 0);

                Parallel.For(0, wep.Length, i => wep = wep.AddItem(wep[i]).ToArray());
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
