using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonNavigation : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private BoxCollider2D playerCollider;

    private List<Vector3Int> testPath;
    private Grid grid;
    
    private static readonly Vector3Int[] Directions =
    {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public List<Vector3Int> FindWorldPath(Vector3 start, Vector3 goal)
    {
        Vector3Int startCell = grid.WorldToCell(start);
        Vector3Int goalCell = grid.WorldToCell(goal);
        
        return FindPath(startCell, goalCell);
    }
    
    public bool HasLineOfSight(Vector2 from, Vector2 to)
    {
        return Physics2D.Linecast(from, to, wallLayer).collider == null;
    }

    public bool HasClearPath(Vector2 from, Vector2 to)
    {
        Vector2 displacement = to - from;
        float distance = displacement.magnitude;

        if (distance <= 0.001f)
            return true;

        Vector2 castSize = playerCollider.bounds.size * 1.05f;

        RaycastHit2D hit = Physics2D.BoxCast(
            from,
            castSize,
            0f,
            displacement.normalized,
            distance,
            wallLayer
        );

        return hit.collider == null;
    }
    
    private List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        List<Vector3Int> openSet = new List<Vector3Int>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        openSet.Add(start);
        
        Dictionary<Vector3Int, int> gScore = new();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new();
        
        gScore[start] = 0;

        while (openSet.Count > 0)
        {
            Vector3Int current = openSet[0];
            int lowestFScore = gScore[current] + Heuristic(current, goal);

            foreach (Vector3Int cell in openSet)
            {
                int fScore = gScore[cell] + Heuristic(cell, goal);
                
                 if (fScore < lowestFScore)
                 {
                     current = cell;
                     lowestFScore = gScore[cell] + Heuristic(cell, goal);
                 }
            }
            
            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }
            
            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                int tentativeGScore = gScore[current] + 1;
                
                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    gScore[neighbor] = tentativeGScore;
                    cameFrom[neighbor] = current;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        
        return new List<Vector3Int>();
    }
    
    private bool IsWalkable(Vector3Int cell)
    {
        return floorTilemap.HasTile(cell) && !wallTilemap.HasTile(cell);
    }

    private List<Vector3Int> GetNeighbors(Vector3Int cell)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        
        foreach (Vector3Int direction in Directions)
        {
            Vector3Int neighbor = cell + direction;

            if (IsWalkable(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
        
        return neighbors;
    }

    private int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    
    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom,
        Vector3Int current)
    {
        List<Vector3Int> path = new();
        path.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    public Vector3 GetWaypointPosition(Vector3Int cell)
    {
        return grid.GetCellCenterWorld(cell);
    }
}

