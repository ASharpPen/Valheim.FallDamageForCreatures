using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Valheim.FallDamageForCreatures;

[HarmonyPatch]
internal class FallDamagePatch
{
    private static HashSet<int> BlackListed { get; } = new();

    static FallDamagePatch()
    {
        try
        {
            BlackListed = new()
            {
                "Blob".GetStableHashCode(),
                "BlobElite".GetStableHashCode(),
                "BlobTar".GetStableHashCode()
            };
        }
        catch(Exception e)
        {
            Log.LogWarning("Failed to initialize list of creatures to not apply fall damage to.", e);
        }
    }

    private static Collider LastGroundContactCollider;
    private static Vector3 LastGroundContactNormal;
    private static Vector3 LastGroundContactPoint;
    private static float LastMaxAirAltitude;
    private static bool GroundContact;

    [HarmonyPatch(typeof(Character), nameof(Character.UpdateGroundContact))]
    [HarmonyPrefix]
    private static void RememberContact(Character __instance)
    {
        if (__instance.IsPlayer())
        {
            return;
        }

        LastGroundContactCollider = __instance.m_lowestContactCollider;
        LastGroundContactNormal = __instance.m_groundContactNormal;
        LastGroundContactPoint = __instance.m_groundContactPoint;
        LastMaxAirAltitude = __instance.m_maxAirAltitude;
        GroundContact = __instance.m_groundContact;
    }

    [HarmonyPatch(typeof(Character), nameof(Character.UpdateGroundContact))]
    [HarmonyPostfix]
    private static void AddFallDamageToNonPlayers(Character __instance)
    {
        if (__instance.IsPlayer())
        {
            return;
        }

        if (!GroundContact)
        {
            return;
        }

        int? prefabHash = __instance.m_nview?.GetZDO()?.m_prefab;
        if (prefabHash is not null &&
            BlackListed.Contains(prefabHash.Value))
        {
            return;
        }

        float num = Mathf.Max(0f, LastMaxAirAltitude - __instance.transform.position.y);

        if (num > 4)
        {
            HitData hitData = new HitData();
            hitData.m_damage.m_damage = Mathf.Clamp01((num - 4f) / 16f) * 100f;
            hitData.m_point = LastGroundContactPoint;
            hitData.m_dir = LastGroundContactNormal;
            __instance.Damage(hitData);

#if DEBUG
            Log.LogDebug("Ouch!");
#endif
        }
    }
}
