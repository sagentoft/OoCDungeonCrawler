using UnityEngine;

public enum ControlSource
{
    Human,
    AI
}

public class ControlManager
{
    private ControlSource activeSource = ControlSource.Human;
    
    private readonly PlayerCharacter player;

    private int remainingMoves = 4;

    public ControlManager(PlayerCharacter player)
    {
        this.player = player;
    }

    public void SubmitMove(Vector2Int input, ControlSource source)
    {
        if (source != activeSource)
            return;
        
        player.Move(input);
        
        if (--remainingMoves <= 0)
        {
            remainingMoves = 4;
            SwitchSource();
        }
    }

    private void SwitchSource()
    {
        if (activeSource == ControlSource.AI)
            activeSource = ControlSource.Human;
        else
            activeSource = ControlSource.AI;
    }
}
