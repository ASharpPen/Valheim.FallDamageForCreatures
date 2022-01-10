using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx;
using HarmonyLib;

namespace Valheim.FallDamageForCreatures;

[BepInPlugin("fall_damage_for_creatures", "Fall Damage For Creatures", "0.0.1")]
public class Plugin : BaseUnityPlugin
{
    // Awake is called once when both the game and the plug-in are loaded
    void Awake()
    {
        Log.Logger = Logger;

        new Harmony("mod.zone_cleanup_fixer").PatchAll();
    }
}

