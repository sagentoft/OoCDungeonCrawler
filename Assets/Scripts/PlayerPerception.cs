using JetBrains.Annotations;
using UnityEngine;

public class PlayerPerception : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask detectionLayer;
    
    [CanBeNull]
    public TreasureChest FindNearbyTreasure()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius,
            detectionLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out TreasureChest chest))
            {
                return chest;
            }
        }

        return null;
    }
}
