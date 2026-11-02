using UnityEngine;

public class AIController : MonoBehaviour
{
    private ControlManager controlManager;

    private float directionChangeTimer = 2;
    private Vector2 moveDirection;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0)
        {
            moveDirection = GenerateRandomDirection();
            directionChangeTimer = 2;
        }
        
        controlManager.SubmitMove(moveDirection, ControlSource.AI);
    }

    private Vector2 GenerateRandomDirection()
    {
        return Random.insideUnitCircle.normalized;
    }
    
    public void Initialize(ControlManager manager)
    {
        controlManager = manager;
    }
}
