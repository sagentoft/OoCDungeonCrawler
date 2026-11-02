using UnityEngine;
using UnityEngine.InputSystem;


public class HumanController : MonoBehaviour
{
    private ControlManager controlManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            controlManager.SubmitMove(Vector2Int.up, ControlSource.Human);
        }
        
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            controlManager.SubmitMove(Vector2Int.down, ControlSource.Human);
        }
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            controlManager.SubmitMove(Vector2Int.left, ControlSource.Human);
        }
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            controlManager.SubmitMove(Vector2Int.right, ControlSource.Human);
        }
    }

    public void Initialize(ControlManager manager)
    {
        controlManager = manager;
    }
}
