using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIController : MonoBehaviour
{
    [SerializeField] private DungeonNavigation navigation;
    
    private ControlManager controlManager;

    private float directionChangeTimer = 2;
    private float interactionRange = 1.5f;
    private Vector2 moveDirection;
    
    private List<Vector3Int> currentPath;
    private int waypointIndex;
    [SerializeField] private TreasureChest targetChest;
    // The dungeon is static: do not repeatedly select targets whose paths failed.
    private readonly HashSet<TreasureChest> unreachableChests = new();
    
    private void OnEnable()
    {
        controlManager.OnWallCollision += () => moveDirection = -moveDirection;
    }

    // Update is called once per frame
    void Update()
    {
        if (controlManager == null)
            return;

        if (targetChest != null && !targetChest.isActiveAndEnabled)
            ClearTarget();

        if (targetChest == null)
        {
            targetChest = controlManager.Player.Perception.FindNearbyTreasure(unreachableChests);
        }

        if (controlManager.ActiveSource != ControlSource.AI)
            return;
        
        if (targetChest != null && currentPath == null)
            RecalculatePath();
        
        if (targetChest != null)
        {
            FollowPath();
            return;
        }
        
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0)
        {
            moveDirection = GenerateRandomDirection();
            directionChangeTimer = 2;
        }
        
        controlManager.SubmitMove(moveDirection, ControlSource.AI);
    }
    
    private void HandleControlChanged(ControlSource source)
    {
        if (source != ControlSource.AI || targetChest == null || !targetChest.isActiveAndEnabled)
            return;

        RecalculatePath();
    }

    private void RecalculatePath()
    {
        currentPath = navigation.FindWorldPath(
            controlManager.Player.transform.position,
            targetChest.transform.position
        );

        waypointIndex = 0;
    }

    private Vector2 GenerateRandomDirection()
    {
        return Random.insideUnitCircle.normalized;
    }

    private void FollowPath()
    {
        Vector2 playerPosition = controlManager.Player.transform.position;
        Vector2 chestPosition = targetChest.transform.position;
        
        if ((chestPosition - playerPosition).sqrMagnitude <= interactionRange * interactionRange &&
            navigation.HasLineOfSight(playerPosition, chestPosition))
        {
            controlManager.SubmitMove(Vector2.zero, ControlSource.AI);
            targetChest.Open();
            ClearTarget();
            return;
        }

        if (currentPath == null || waypointIndex >= currentPath.Count)
        {
            controlManager.SubmitMove(Vector2.zero, ControlSource.AI);
            unreachableChests.Add(targetChest);
            ClearTarget();
            return;
        }

        for (int i = currentPath.Count - 1; i > waypointIndex; i--)
        {
            Vector2 candidate = navigation.GetWaypointPosition(currentPath[i]);

            if (navigation.HasClearPath(playerPosition, candidate))
            {
                waypointIndex = i;
                break;
            }
        }
        
        Vector3 waypoint = navigation.GetWaypointPosition(currentPath[waypointIndex]);
        Vector2 toWaypoint = waypoint - controlManager.Player.transform.position;

        if (toWaypoint.sqrMagnitude < 0.01f * 0.01f)
        {
            waypointIndex++;
            controlManager.SubmitMove(Vector2.zero, ControlSource.AI);
            return;
        }
        
        controlManager.SubmitMove(toWaypoint.normalized, ControlSource.AI);
        
        Debug.DrawLine(
            controlManager.Player.transform.position,
            waypoint,
            Color.red
        );
    }
    
    private void ClearTarget()
    {
        targetChest = null;
        currentPath = null;
        waypointIndex = 0;
    }

    public void Initialize(ControlManager manager)
    {
        controlManager = manager;
        controlManager.OnControlChanged += HandleControlChanged;
    }
    
    private void OnDestroy()
    {
        if (controlManager != null)
            controlManager.OnControlChanged -= HandleControlChanged;
    }
}
