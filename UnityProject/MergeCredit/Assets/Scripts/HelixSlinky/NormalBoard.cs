using UnityEngine;

public class NormalBoard : MonoBehaviour, IBoard
{

    public void Broken()
    {
        gameObject.SetActive(false);
    }
}

public interface IBoard
{
    void Broken();
}