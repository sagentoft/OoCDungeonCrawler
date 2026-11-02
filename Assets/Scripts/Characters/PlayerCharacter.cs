using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallsTilemap;

    [SerializeField] private float moveSpeed = 10f;

    private Vector3Int currentCell;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentCell = new Vector3Int(2, 3, 0);
        
        Vector3 worldPosition = grid.GetCellCenterWorld(currentCell);
        
        transform.position = worldPosition;
    }

    // Update is called once per frame
    void Update()
    {

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
        rb.linearVelocity = direction * moveSpeed;
    }
    
    public void AlignToGrid()
    {
        currentCell = grid.WorldToCell(transform.position);
        rb.position = grid.GetCellCenterWorld(currentCell);
        rb.linearVelocity = Vector2.zero;
    }
}
