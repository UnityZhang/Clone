using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGroup : MonoBehaviour
{
    IBoard[] _boards;

    bool m_IsBroken = false;
    private void Start()
    {
        _boards = GetComponentsInChildren<IBoard>();
    }
    public void PUpdate(float y)
    {
        if (m_IsBroken)
            return;
        if (y <= transform.position.y)
        {
            foreach (var item in _boards)
            {
                item.Broken();
            }
        }
    }
}
