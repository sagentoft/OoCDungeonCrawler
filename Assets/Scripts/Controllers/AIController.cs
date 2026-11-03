using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIController : MonoBehaviour
{
    private enum AIState
    {
        Wander,
        SeekTreasure
    }
    
    [SerializeField] private DungeonNavigation navigation;
    
    private ControlManager controlManager;

    private float directionChangeTimer = 2;
    private float interactionRange = 1.5f;
    private Vector2 moveDirection;
    private AIState aiState;
    
    private List<Vector3Int> currentPath;
    private int waypointIndex;
    private TreasureChest targetChest;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aiState = AIState.Wander;
    }

    // Update is called once per frame
    void Update()
    {

        if (targetChest == null)
        {
            targetChest = controlManager.Player.Perception.FindNearbyTreasure();
            
            if (targetChest != null)
            {
                if (aiState != AIState.SeekTreasure)
                {
                    currentPath = navigation.FindWorldPath(controlManager.Player.transform.position,
                        targetChest.transform.position);
                
                    Debug.Log($"Found path with {currentPath.Count} cells");
                }
            
                waypointIndex = 0;
                aiState = AIState.SeekTreasure;
            }
        }

        if (targetChest != null)
        {
            FollowPath();
            return;
        }
        
        aiState = AIState.Wander;
        
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
        if (source != ControlSource.AI || targetChest == null)
            return;

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
        if (currentPath == null || waypointIndex >= currentPath.Count)
        {
            controlManager.SubmitMove(Vector2.zero, ControlSource.AI);

            if (targetChest != null)
            {
                Vector2 toChest = targetChest.transform.position -
                                    controlManager.Player.transform.position;

                if (toChest.sqrMagnitude < interactionRange * interactionRange)
                {
                    targetChest.Open();
                    targetChest = null;
                    aiState = AIState.Wander;
                }
            }

            return;
        }
        
        Vector3 waypoint = navigation.GetWaypointPosition(currentPath[waypointIndex]);
        Vector2 toWaypoint = waypoint - controlManager.Player.transform.position;

        if (toWaypoint.sqrMagnitude < 0.1f * 0.1f)
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
