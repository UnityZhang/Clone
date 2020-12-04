using UnityEngine;

public class NormalBoard : ABoard
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