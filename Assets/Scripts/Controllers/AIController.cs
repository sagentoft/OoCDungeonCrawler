using UnityEngine;

public class AIController : MonoBehaviour
{
    private ControlManager controlManager;

    private int moveCooldown = 1;
    private float moveTimer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveTimer += Time.deltaTime;
        
        if (moveTimer >= moveCooldown)
        {
            controlManager.SubmitMove(GenerateRandomMovementAction(), ControlSource.AI);
            moveTimer = 0;
        }
    }
    
    private Vector2Int GenerateRandomMovementAction()
    {
        return Random.Range(0,4) switch
        {
            0 => Vector2Int.up,
            1 => Vector2Int.down,
            2 => Vector2Int.left,
            3 => Vector2Int.right,
        };
    }
    
    public void Initialize(ControlManager manager)
    {
        controlManager = manager;
    }
}
