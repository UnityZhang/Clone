using UnityEngine;

public class Prop_MaxSpeed: AProp
{

    public override void Trigger()
    {
        PlayerSpring.S.StartMaxSpeedState(PlayerSpring.S.MaxSpeedTime);

        gameObject.SetActive(false);
    }
}