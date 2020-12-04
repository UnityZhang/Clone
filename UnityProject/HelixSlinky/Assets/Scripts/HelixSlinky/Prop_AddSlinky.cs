using UnityEngine;

public class Prop_AddSlinky: AProp
{
    public int AddCount = 2;

    public override void Trigger()
    {
        PlayerSpring.S.AddSlinky(AddCount);
        gameObject.SetActive(false);
    }

    
}

public abstract class AProp : MonoBehaviour
{
    //public virtual void Start()
    //{

    //}

    public virtual void Trigger()
    {

    }
}