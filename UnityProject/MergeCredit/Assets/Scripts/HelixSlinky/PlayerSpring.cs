using System.Collections.Generic;
using UnityEngine;
using Qarth;

public class PlayerSpring : TMonoSingleton<PlayerSpring>
{
    public GameObject SlinkyPrefab;

    public Transform CameraTarget;
    public Transform ObstacleRoot;

    public Transform SlinkysTrans;
    public float TargetDistance = 3;

    public int InitSlinkyCount;
    public List<Slinky> SlinkyList;
    public float SlinkyDistance = 0.45f;
    public float SlinkyBounceMinDistance = 0.16f;
    public float UpDistance = 5;
    public float GoDownSpeed = 80f;
    public float MaxSpeed = 100;
    [Range(0.3f, 2f)]
    public float AccelerateRate = 1.1f;

    public float GoUpTime = 0.25f;

    [Range(0, 1)]
    public float SlinkyColor;

    public AnimationCurve BounceCurve;


    ObstacleGroup[] m_ObstacleGroups;
    [HideInInspector]
    public bool m_CanDamageNextBoard;
    float m_MaxSpeedTime = 0;
    List<Slinky> m_WaitForRecycleSlinkyList = new List<Slinky>();

    float DownTime = 0;
    bool IsDown = false;
    float m_RayPointOffset = 1.7f;//需要判断左右两边

    public int GetSlinkyCount()
    {
        return SlinkyList.Count;
    }
    public Slinky GetSlinky(int index)
    {
        if (index >= 0 && index < SlinkyList.Count)
        {
            return SlinkyList[index];
        }
        return null;
    }

    private void Awake()
    {
        for (int i = SlinkyList.Count - 2; i >= 0; i--)
        {
            SlinkyList[i].transform.localPosition = SlinkyList[i + 1].transform.localPosition + new Vector3(0, SlinkyDistance, 0);
        }
        SetAllSlinkyColor();
        StartFall();
        m_ObstacleGroups = ObstacleRoot.GetComponentsInChildren<ObstacleGroup>();
    }
    void Start()
    {

    }

    public void StartFall()
    {
        for (int i = 0; i < SlinkyList.Count; i++)
        {
            SlinkyList[i].SetIndex(i);
        }
        IsDown = true;
        DownTime = 0;
    }

    /// <summary>
    /// 添加圆环 现只能在下落时添加
    /// </summary>
    /// <param name="count"></param>
    public void AddSlinky(int count = 1)
    {
        if (count > 0)
        {
            if (!IsDown)
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                Slinky slinky = CreateSlinky();
                SlinkyList.Insert(0, slinky);
                SetAllSlinkyIndex();
                slinky.transform.localPosition = SlinkyList[1].transform.localPosition + new Vector3(0, SlinkyDistance, 0);
            }
            SetAllSlinkyColor();
        }
        else
        {
            RemoveSlinky(count * -1);
        }
    }

    /// <summary>
    /// 移除圆环
    /// </summary>
    /// <param name="count"></param>
    public void RemoveSlinky(int count = 1)
    {
        //检测游戏失败
        if (SlinkyList.Count - count < 5)
        {
            Log.e("游戏失败");
            return;
        }
        for (int i = 0; i < count; i++)
        {
            m_WaitForRecycleSlinkyList.Add(SlinkyList[i]);
        }
    }


    Slinky CreateSlinky()
    {
        GameObject obj = Instantiate(SlinkyPrefab, SlinkysTrans);
        obj.transform.SetAsFirstSibling();
        Slinky s = obj.AddComponent<Slinky>();
        return s;
    }
    void RecycleSlinky(Slinky s)
    {
        Destroy(s.gameObject);
    }

    void SetAllSlinkyColor()
    {
        float v = 1.0f / SlinkyList.Count;
        for (int i = 0; i < SlinkyList.Count; i++)
        {
            SlinkyList[i].SetColor(v * i, SlinkyColor);
        }
    }
    void SetAllSlinkyIndex()
    {
        for (int i = 0; i < SlinkyList.Count; i++)
        {
            SlinkyList[i].SetIndex(i);
        }
    }

    public void StartUp()
    {
        LeanTween.moveY(SlinkysTrans.gameObject, SlinkysTrans.position.y + UpDistance, GoUpTime).setEaseOutCubic().setOnComplete(StartFall);
        //弹动效果
        //计算圆环中心点
        float centerY = (SlinkyList[0].transform.localPosition.y + SlinkyList[SlinkyList.Count - 1].transform.localPosition.y) * 0.5f;
        //单双数情况
        float time = GoUpTime * 0.8f;
        if (SlinkyList.Count%2 == 1)
        {
            int index = SlinkyList.Count / 2;
            for (int i = index + 1; i < SlinkyList.Count; i++)
            {
                SlinkyList[i].Bounce(centerY - ((i - index) * SlinkyBounceMinDistance), time, BounceCurve);
            }
            for (int i = index - 1; i >= 0; i--)
            {
                SlinkyList[i].Bounce(centerY + ((index - i) * SlinkyBounceMinDistance), time, BounceCurve);
            }
        }
        else
        {
            int index = SlinkyList.Count / 2;
            for (int i = index; i < SlinkyList.Count; i++)
            {
                SlinkyList[i].Bounce(centerY - ((i - index) * SlinkyBounceMinDistance) - (SlinkyBounceMinDistance * 0.5f), time, BounceCurve);
            }
            for (int i = index - 1; i >= 0; i--)
            {
                SlinkyList[i].Bounce(centerY + ((index - 1 - i) * SlinkyBounceMinDistance) + (SlinkyBounceMinDistance * 0.5f), time, BounceCurve);
            }
        }
    }

    
    void Update()
    {
        //更新圆环位置
        if (IsDown)
        {
            DownTime += Time.deltaTime;
            CheckCollider();
        }
        //移除圆环（不能在update中移除）
        if (m_WaitForRecycleSlinkyList.Count > 0)
        {
            foreach (var s in m_WaitForRecycleSlinkyList)
            {
                SlinkyList.Remove(s);
                RecycleSlinky(s);
            }
            m_WaitForRecycleSlinkyList.Clear();
            SetAllSlinkyIndex();
            SetAllSlinkyColor();
        }
        //更新board
        foreach (var item in m_ObstacleGroups)
        {
            item.PUpdate(SlinkyList[SlinkyList.Count - 1].transform.position.y);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddSlinky();
        }
    }

    public void CheckCollider()
    {
        Vector3 oriPos = SlinkysTrans.position;//记录原来的位置 
        float test = Mathf.Clamp(GoDownSpeed * DownTime * AccelerateRate, 0, MaxSpeed);
        if (test == MaxSpeed || !m_CanDamageNextBoard)//达到最大速度
        {
            m_CanDamageNextBoard = true;
        }

        SlinkysTrans.Translate(SlinkysTrans.up * -test * Time.deltaTime, Space.World); //移动 
        float length = (SlinkysTrans.position - oriPos).magnitude;//射线的长度 
        Vector3 direction = SlinkysTrans.position - oriPos;//方向 
        //左右两个位置都检测
        RaycastHit hitinfo1;
        bool isCollider1 = Physics.Raycast(oriPos + new Vector3(m_RayPointOffset, 0, 0), direction, out hitinfo1, length);//在两个位置之间发起一条射线，然后通过这条射线去检测有没有发生碰撞 
        RaycastHit hitinfo2;
        bool isCollider2 = Physics.Raycast(oriPos + new Vector3(-m_RayPointOffset, 0, 0), direction, out hitinfo2, length);
        //优先处理board
        if (isCollider1 && isCollider2)
        {
            if ((hitinfo1.transform.gameObject.layer == 12 && hitinfo2.transform.gameObject.layer == 12) || (hitinfo1.transform.gameObject.layer == 12 && hitinfo2.transform.gameObject.layer != 12) || (hitinfo2.transform.gameObject.layer == 12 && hitinfo1.transform.gameObject.layer != 12))
            {
                IsDown = false;
                SlinkysTrans.position = new Vector3(SlinkysTrans.position.x, hitinfo1.transform.gameObject.layer == 12 ? hitinfo1.point.y : hitinfo2.point.y, SlinkysTrans.position.z);
                StartUp();
            }
            else
            {
                hitinfo1.transform.GetComponent<IProp>().Trigger();
                hitinfo1.transform.gameObject.SetActive(false);
            }
        }
        else if (isCollider1 || isCollider2)
        {
            if (isCollider1)
            {
                if (hitinfo1.transform.gameObject.layer == 12)
                {
                    IsDown = false;
                    SlinkysTrans.position = new Vector3(SlinkysTrans.position.x, hitinfo1.point.y, SlinkysTrans.position.z);
                    StartUp();
                }
                else if (hitinfo1.transform.gameObject.layer == 11)
                {
                    hitinfo1.transform.GetComponent<IProp>().Trigger();
                    hitinfo1.transform.gameObject.SetActive(false);
                }
            }
            else
            {
                if (hitinfo2.transform.gameObject.layer == 12)
                {
                    IsDown = false;
                    SlinkysTrans.position = new Vector3(SlinkysTrans.position.x, hitinfo2.point.y, SlinkysTrans.position.z);
                    StartUp();
                }
                else if (hitinfo2.transform.gameObject.layer == 11)
                {
                    hitinfo2.transform.GetComponent<IProp>().Trigger();
                    hitinfo2.transform.gameObject.SetActive(false);
                }
            }
        }
        //同时移动相机跟随物体
        ChangeCameraTargetPos();
    }
    void ChangeCameraTargetPos()
    {
        float value = SlinkysTrans.position.y + TargetDistance;
        if (value < CameraTarget.position.y)
        {
            CameraTarget.position = new Vector3(CameraTarget.position.x, value, CameraTarget.position.z);
        }
    }
}
