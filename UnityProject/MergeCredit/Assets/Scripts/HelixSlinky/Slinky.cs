using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qarth;

public class Slinky : MonoBehaviour
{
    //[SerializeField]
    int Index;

    private Renderer rend;
    void Start()
    {
        
    }

    public void SetIndex(int i)
    {
        Index = i;
    }
    public void SetColor(float color1, float color2)
    {
        if (rend == null)
            rend = GetComponent<Renderer>();
        rend.material.SetColor("_Color", HSBColor.ToColor(new HSBColor(color1, color2, 1)));
    }

    public void Bounce(float y, float time, AnimationCurve curve)
    {
        LeanTween.moveLocalY(gameObject, y, time).setEase(curve);
    }
    
    public void HitBoard()
    {
        
    }
    
}