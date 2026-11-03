using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallsTilemap;

    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float moveSpeed = 10f;

    private Vector3Int currentCell;
    private Rigidbody2D rb;
    private Vector2 freeMoveDirection;
    private bool freeMovementActive;
    
    public PlayerPerception Perception { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Perception = GetComponent<PlayerPerception>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentCell = new Vector3Int(2, 3, 0);
        
        Vector3 worldPosition = grid.GetCellCenterWorld(currentCell);
        
        transform.position = worldPosition;
    }

    private void FixedUpdate()
    {
        if (!freeMovementActive)
            return;

        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            freeMoveDirection * moveSpeed,
            acceleration * Time.fixedDeltaTime
        );
    }

    public void MoveOnGrid(Vector2Int direction)
    {
        Vector3Int destination = currentCell + (Vector3Int)direction;
        
        if (!floorTilemap.HasTile(destination))
            return;

        if (wallsTilemap.HasTile(destination))
            return;
        
        currentCell = destination;
        transform.position = grid.GetCellCenterWorld(currentCell);
        
    }

    public void MoveFreely(Vector2 direction)
    {
        freeMoveDirection = direction.normalized;
        freeMovementActive = true;
    }
    
    public void AlignToGrid()
    {
        freeMovementActive = false;
        freeMoveDirection = Vector2.zero;
        currentCell = grid.WorldToCell(transform.position);
        rb.position = grid.GetCellCenterWorld(currentCell);
        rb.linearVelocity = Vector2.zero;
    }
}
