using UnityEngine;

public class Prop_AddSlinky : MonoBehaviour, IProp
{
    public int AddCount = 2;

    public void Trigger()
    {
        PlayerSpring.S.AddSlinky(AddCount);
    }

    void Start()
    {
        
    }

    
}

public interface IProp
{
    void Trigger();
}