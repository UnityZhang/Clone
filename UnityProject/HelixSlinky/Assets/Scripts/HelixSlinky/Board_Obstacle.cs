using UnityEngine;

public class Board_Obstacle : AProp
{
    public override void Trigger()
    {
        PlayerSpring.S.AddSlinky(-1);
    }
}