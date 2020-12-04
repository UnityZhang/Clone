using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGroup : MonoBehaviour
{
    public Transform BoardsTran;
    ABoard[] m_boards;

    Renderer[] m_renderers;

    AProp[] m_Props;

    bool m_IsBroken = false;
    private void Start()
    {
        m_boards = BoardsTran.GetComponentsInChildren<ABoard>();
        m_renderers = BoardsTran.GetComponentsInChildren<Renderer>();
        m_Props = GetComponentsInChildren<AProp>();
    }

    public bool HaveBoard(Transform tran)
    {
        foreach (var item in m_boards)
        {
            if (item.transform == tran)
                return true;
        }
        return false;
    }

    public void PUpdate(float y)
    {
        if (m_IsBroken)
            return;
        if (y <= transform.position.y + 0.2f)
        {
            Broken();
        }
    }
    public void Broken()
    {
        foreach (var item in m_boards)
        {
            item.Broken();
        }
        m_IsBroken = true;

        foreach (var item in m_Props)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void ChangeMat(Material mat)
    {
        foreach (var item in m_renderers)
        {
            item.sharedMaterial = mat;
        }
    }
}
