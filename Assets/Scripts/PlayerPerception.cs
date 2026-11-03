using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerception : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask detectionLayer;
    
    [CanBeNull]
    public TreasureChest FindNearbyTreasure(HashSet<TreasureChest> ignoredChests = null)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius,
            detectionLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out TreasureChest chest) && chest.isActiveAndEnabled &&
                (ignoredChests == null || !ignoredChests.Contains(chest)))
            {
                return chest;
            }
        }

        return null;
    }
}
