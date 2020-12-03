using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qarth;

public class H_GameCtrl : TMonoSingleton<H_GameCtrl>
{

    [SerializeField]
    public Transform Root;
    [SerializeField]
    public float speed = 6f;//旋转速度
    public bool StartGame = true;

   

    void Start()
    {
        
    }

    private float OffsetX = 0;
    //private float OffsetY = 0;

    void Update()
    {
        if (!StartGame)
            return;
        if (Input.GetMouseButton(0))
        {
            OffsetX = Input.GetAxis("Mouse X");//获取鼠标x轴的偏移量
            //OffsetY = Input.GetAxis("Mouse Y");//获取鼠标y轴的偏移量

            Root.Rotate(new Vector3(0, -OffsetX, 0) * speed, Space.World);
        }
    }
    
   
}
