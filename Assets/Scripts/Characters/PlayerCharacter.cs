using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private Grid grid;

    private Vector3Int currentCell;
    
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

    public void Move(Vector2Int direction)
    {
        currentCell += (Vector3Int)direction;
        transform.position = grid.GetCellCenterWorld(currentCell);
        
    }
}
