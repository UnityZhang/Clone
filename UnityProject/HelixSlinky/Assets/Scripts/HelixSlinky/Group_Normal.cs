using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group_Normal : ABoardGroup
{
    public Transform BoardsTran;
    ABoard[] m_boards;

    Renderer[] m_renderers;

    public MeshFilter[] MeshFilters;

    AProp[] m_Props;

    bool m_IsBroken = false;
    private void Start()
    {
        m_boards = BoardsTran.GetComponentsInChildren<ABoard>();
        m_renderers = BoardsTran.GetComponentsInChildren<Renderer>();
        MeshFilters = BoardsTran.GetComponentsInChildren<MeshFilter>();
        m_Props = GetComponentsInChildren<AProp>();
    }

    public override bool HaveBoard(Transform tran)
    {
        foreach (var item in m_boards)
        {
            if (item.transform == tran)
                return true;
        }
        return false;
    }

    public override void PUpdate(float y)
    {
        if (m_IsBroken)
            return;
        if (y <= transform.position.y + 0.2f)
        {
            Broken();
        }
    }
    public override void Broken()
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

    public override void ChangeMat(Material mat)
    {
        foreach (var item in m_renderers)
        {
            item.sharedMaterial = mat;
        }
    }
}

public abstract class ABoardGroup : MonoBehaviour
{
    public abstract void ChangeMat(Material mat);

    public abstract void Broken();

    public abstract void PUpdate(float y);

    public abstract bool HaveBoard(Transform tran);
}
