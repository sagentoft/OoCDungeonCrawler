using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private PlayerAnimator playerAnimator;

    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float moveSpeed = 10f;

    private Vector3Int currentCell;
    private Rigidbody2D rb;
    private Vector2 freeMoveDirection;
    private bool freeMovementActive;
    private bool isInterpolating;

    public event Action OnWallCollision;
    
    public PlayerPerception Perception { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Perception = GetComponent<PlayerPerception>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentCell = new Vector3Int(-5, -2, 0);
        
        Vector3 worldPosition = grid.GetCellCenterWorld(currentCell);
        
        transform.position = worldPosition;
    }

    private void FixedUpdate()
    {
        if (!freeMovementActive || isInterpolating)
            return;

        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            freeMoveDirection * moveSpeed,
            acceleration * Time.fixedDeltaTime
        );
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Walls")) ;
        {
            OnWallCollision?.Invoke();
        }
    }

    private IEnumerator MoveToPosition(Vector2 targetPosition, float duration)
    {
        isInterpolating = true;

        Vector2 startPosition = rb.position;
        float elapsed = 0f;

        rb.linearVelocity = Vector2.zero;

        while (elapsed < duration)
        {
            elapsed += Time.fixedDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = 1f - Mathf.Pow(1f - t, 3f);

            Vector2 position = Vector2.Lerp(startPosition, targetPosition, t);

            rb.MovePosition(position);

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPosition);
        isInterpolating = false;
    }

    public void MoveOnGrid(Vector2Int direction)
    {
        if (isInterpolating)
            return;

        Vector3Int destination = currentCell + (Vector3Int)direction;

        if (!floorTilemap.HasTile(destination))
            return;

        if (wallsTilemap.HasTile(destination))
            return;

        currentCell = destination;

        Vector2 targetPosition = grid.GetCellCenterWorld(currentCell);
        StartCoroutine(MoveToPosition(targetPosition, 0.08f));

        playerAnimator.SetMovement(direction, false);
    }

    public void MoveFreely(Vector2 direction)
    {
        if (isInterpolating)
            return;
        
        freeMoveDirection = direction.normalized;
        freeMovementActive = true;
        playerAnimator.SetMovement(direction, direction.sqrMagnitude > 0.001f);
    }
    
    public void AlignToGrid()
    {
        freeMovementActive = false;
        freeMoveDirection = Vector2.zero;
        currentCell = grid.WorldToCell(transform.position);
        rb.position = grid.GetCellCenterWorld(currentCell);
        rb.linearVelocity = Vector2.zero;
        playerAnimator.SetMovement(Vector2.zero, false);
    }
}
