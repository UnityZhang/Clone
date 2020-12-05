using UnityEngine;

public class Board_Normal : ABoard
{
    public override void Broken()
    {
        gameObject.SetActive(false);
    }
}


public abstract class ABoard : MonoBehaviour
{
    public virtual void Broken()
    {
        gameObject.SetActive(false);
    }
}