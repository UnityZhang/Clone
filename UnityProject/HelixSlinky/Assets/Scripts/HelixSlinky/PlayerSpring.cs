using GameWish.Game;
using Qarth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpring : TMonoSingleton<PlayerSpring>
{
    public GameObject SlinkyPrefab;

    public Transform CameraTarget;
    public Transform ObstacleRoot;
    public Transform SlinkysTrans;

    public Material mat;
    public float m_WaveHeight = 1.2f;
    public float TargetDistance = 3;

    public int InitSlinkyCount;
    public List<Slinky> SlinkyList;
    [Header("圆环下落时间隔距离")]
    public float SlinkyDistance = 0.45f;
    [Header("弹起时圆环的最小间隔距离")]
    public float SlinkyBounceMinDistance = 0.16f;
    [Header("弹起的长度")]
    public float UpDistance = 5;
    [Header("初始下落速度")]
    public float GoDownSpeed = 80f;
    [Header("最大下落速度")]
    public float MaxSpeed = 100;
    [Header("加速下落速率")]
    [Range(0.3f, 2f)]
    public float AccelerateRate = 1.1f;
    [Header("上升总时间")]
    public float GoUpTime = 0.25f;
    [Header("加速道具持续时间")]
    public float MaxSpeedTime = 3f;
    [Header("圆环颜色饱和度")]
    [Range(0, 1)]
    public float SlinkyColor;
    [Header("圆环弹起的弹动曲线")]
    public AnimationCurve BounceCurve;


    ObstacleGroup[] m_ObstacleGroups;
    [HideInInspector]
    public bool m_CanDamageNextBoard;//撞掉下一个板
    public float MaxSpeedStateTime { get; private set; }//无敌时间
    bool m_MaxSpeedState = false;

    List<Slinky> m_WaitForRecycleSlinkyList = new List<Slinky>();

    float m_FallTime = 0;
    bool IsDown = false;
    float m_RayPointOffset = 1.5f;//需要判断左右两边
    
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
        MaxSpeedStateTime = 0;
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
        m_FallTime = 0;
    }

    /// <summary>
    /// 添加圆环 现只能在下落时添加
    /// </summary>
    /// <param name="count"></param>
    public void AddSlinky(int count = 1)
    {
        if (count > 0)
        {
            //if (!IsDown)
            //{
            //    return;
            //}
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
            m_FallTime += Time.deltaTime;
            if (MaxSpeedStateTime > 0)
                MaxSpeedStateTime -= Time.deltaTime;
            else if(m_MaxSpeedState)
                StopMaxSpeedState();
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

    public void StartMaxSpeedState(float time)
    {
        MaxSpeedStateTime = time;
        m_MaxSpeedState = true;
        HelixEffectMgr.S.StartMaxSpeedFall(SlinkyList[SlinkyList.Count - 1].transform);
    }
    void StopMaxSpeedState()
    {
        m_FallTime = 0;
        m_MaxSpeedState = false;
        HelixEffectMgr.S.StopMaxSpeedFall();
    }

    /// <summary>
    /// 碰到板子
    /// </summary>
    /// <param name="stopPos"></param>
    /// <param name="hit"></param>
    void HitBoard(Vector3 stopPos, Transform hit)
    {
        if (m_CanDamageNextBoard)
        {
            foreach (var item in m_ObstacleGroups)
            {
                if (item.HaveBoard(hit))
                {
                    item.Broken();
                    break;
                }
            }
            m_CanDamageNextBoard = false;
            m_FallTime = 0;
            HelixEffectMgr.S.StopMaxSpeedFall();
        }
        else
        {
            if (MaxSpeedStateTime > 0)//无敌状态 破坏掉
            {
                foreach (var item in m_ObstacleGroups)
                {
                    if (item.HaveBoard(hit))
                    {
                        item.Broken();
                        break;
                    }
                }
            }
            else
            {
                IsDown = false;
                SlinkysTrans.position = stopPos;
                StartUp();
            }
        }
    }

    void FallToMaxSpeed()
    {
        //Log.e(m_FallTime);
        m_CanDamageNextBoard = true;
        HelixEffectMgr.S.StartCommonFall(SlinkyList[0].transform);
    }

    public void CheckCollider()
    {
        Vector3 oriPos = SlinkysTrans.position;//记录原来的位置 
        float test;
        if (m_MaxSpeedState)//最大速度
            test = MaxSpeed;
        else
        {
            test = Mathf.Clamp(GoDownSpeed * m_FallTime * AccelerateRate, 0, MaxSpeed);
            if (test == MaxSpeed && !m_CanDamageNextBoard)//达到最大速度
                FallToMaxSpeed();
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
                RaycastHit t = hitinfo1.transform.gameObject.layer == 12 ? hitinfo1 : hitinfo2;
                HitBoard(new Vector3(SlinkysTrans.position.x, t.point.y, SlinkysTrans.position.z), t.transform);
            }
            else
            {
                //hitinfo1.transform.GetComponent<IProp>().Trigger();
            }
        }
        else if (isCollider1 || isCollider2)
        {
            if (isCollider1)
            {
                if (hitinfo1.transform.gameObject.layer == 12)
                {
                    HitBoard(new Vector3(SlinkysTrans.position.x, hitinfo1.point.y, SlinkysTrans.position.z), hitinfo1.transform);
                }
                else if (hitinfo1.transform.gameObject.layer == 11)
                {
                    //hitinfo1.transform.GetComponent<IProp>().Trigger();
                }
            }
            else
            {
                if (hitinfo2.transform.gameObject.layer == 12)
                {
                    HitBoard(new Vector3(SlinkysTrans.position.x, hitinfo2.point.y, SlinkysTrans.position.z), hitinfo2.transform);
                }
                else if (hitinfo2.transform.gameObject.layer == 11)
                {
                    //hitinfo2.transform.GetComponent<IProp>().Trigger();
                }
            }
        }
        if (!IsDown)
        {
            Material mat1 = ChangeBoardMat(isCollider1 ? hitinfo1.transform : hitinfo2.transform);
            Vector3 hitcenter = isCollider1?hitinfo1.point - new Vector3(m_RayPointOffset, 0, 0): hitinfo2.point + new Vector3(m_RayPointOffset, 0, 0);
            Vector4 point = new Vector4(hitcenter.x, hitcenter.y, hitcenter.z, 0);

            mat.SetFloat("_PointTime", Time.time);
            mat.SetVector("hitPoint", point);
            mat.SetFloat("_WaveScal", m_WaveHeight);
            StartCoroutine(BoardBounce(mat1));

            //EffectManager.S.PlayEffect(EffectName.OnTheGround, hitcenter + new Vector3(0, 0.05f, 0));
        }
        //同时移动相机跟随物体
        ChangeCameraTargetPos();
    }
    void ChangeCameraTargetPos()
    {
        float value = SlinkyList[SlinkyList.Count - 1].transform.position.y + TargetDistance;
        if (value < CameraTarget.position.y)
        {
            CameraTarget.position = new Vector3(CameraTarget.position.x, value, CameraTarget.position.z);
        }
    }

    Material ChangeBoardMat(Transform board)
    {
        Material mat1 = new Material(mat);
        //Material mat1 = mat;
        foreach (var item in m_ObstacleGroups)
        {
            if (item.HaveBoard(board))
            {
                item.ChangeMat(mat1);
                break;
            }
        }
        return mat1;
    }

    IEnumerator BoardBounce(Material mat1)
    {
        float wave = mat1.GetFloat("_WaveScal");
        while (wave > 0)
        {
            wave = mat1.GetFloat("_WaveScal");
            mat1.SetFloat("_WaveScal", wave * 0.8f);
            if (wave < 0.01f)
            {
                mat1.SetFloat("_WaveScal", 0);
            }
            yield return null;
        }
    }

    
}
