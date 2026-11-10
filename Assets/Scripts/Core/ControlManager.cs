using System;
using UnityEngine;

public enum ControlSource
{
    Human,
    AI
}

public class ControlManager
{
    private ControlSource activeSource = ControlSource.AI;
    
    private readonly PlayerCharacter player;

    private float controlTimer = 99999;
    
    public PlayerCharacter Player => player;
    public ControlSource ActiveSource => activeSource;
    
    public event Action<ControlSource> OnControlChanged;
    public event Action OnWallCollision;

    public ControlManager(PlayerCharacter player)
    {
        this.player = player;
        player.OnWallCollision += () => OnWallCollision?.Invoke();
    }
    
    public void Tick(float deltaTime)
    {
        controlTimer -= deltaTime;

        if (controlTimer <= 0f)
        {
            if (activeSource == ControlSource.AI)
                player.AlignToGrid();
            
            SwitchSource();
            controlTimer = 5f;
        }

    }

    public void SubmitMove(Vector2 input, ControlSource source)
    {
        if (source != activeSource)
            return;
        
        if (source == ControlSource.Human)
            player.MoveOnGrid(Vector2Int.RoundToInt(input));
        else
            player.MoveFreely(input);
        
    }

    private void SwitchSource()
    {
        Debug.Log("Switching source");
        activeSource = activeSource == ControlSource.AI ?
            ControlSource.Human : ControlSource.AI;
        
        OnControlChanged?.Invoke(activeSource);
    }
}
