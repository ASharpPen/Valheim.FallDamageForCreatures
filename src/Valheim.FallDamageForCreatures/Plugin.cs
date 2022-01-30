using BepInEx;
using HarmonyLib;

namespace Valheim.FallDamageForCreatures;

[BepInPlugin("fall_damage_for_creatures", "Fall Damage For Creatures", "1.1.0")]
public class Plugin : BaseUnityPlugin
{
    void Awake()
    {
        Log.Logger = Logger;

        new Harmony("mod.fall_damage_for_creatures").PatchAll();
    }
}

