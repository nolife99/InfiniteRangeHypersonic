using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;

using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

using System.Linq;
using System.Threading.Tasks;

using HarmonyLib;
using MelonLoader;
using HypersonicInfiniteRange;

[assembly: MelonInfo(typeof(ModMain), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace HypersonicInfiniteRange;

sealed class ModMain : BloonsTD6Mod
{
    public override void OnNewGameModel(GameModel gameModel) => gameModel.towers.AsParallel().ForAll(t =>
    {
        if (Configs.InfiniteRange) t.range = 1000;
        t.behaviors.Do(m =>
        {
            var attack = m.TryCast<AttackModel>();
            if (attack != null)
            {
                if (Configs.InfiniteRange)
                {
                    attack.range = 1000;
                    attack.attackThroughWalls = true;
                }

                var wep = attack.weapons;
                if (Configs.Hypersonic) wep.Do(w => w.rate = 0);

                Parallel.For(0, wep.Length, i => wep = wep.AddItem(wep[i]).ToArray());
            }
        });
    });
    sealed class Configs : ModSettings
    {
        internal static readonly ModSettingBool Hypersonic = new(true)
        {
            displayName = "Enable hypersonic",
            icon = VanillaSprites.FasterBarrelSpinUpgradeIcon
        };
        internal static readonly ModSettingBool InfiniteRange = new(true)
        {
            displayName = "Enable infinite range",
            icon = VanillaSprites.EpicRangeUpgradeIcon
        };
    }
}
