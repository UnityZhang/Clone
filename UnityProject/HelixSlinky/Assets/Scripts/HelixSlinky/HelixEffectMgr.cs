using UnityEngine;
using Qarth;
using GameWish.Game;

public class HelixEffectMgr : TMonoSingleton<HelixEffectMgr>
{
    EffectGameObject m_FallEffect;
    EffectGameObject m_FallLineEffect;

    public void StartCommonFall(Transform parent)
    {
        m_FallEffect = EffectManager.S.SpawnEffect(EffectName.NormalFalling, -1);
        m_FallEffect.transform.SetParent(parent);
        m_FallEffect.transform.localPosition = Vector3.zero + Vector3.forward;

        if (m_FallLineEffect == null)
        {
            m_FallLineEffect = EffectManager.S.SpawnEffect(EffectName.SpeedLineFalling_normal, -1);
            m_FallLineEffect.transform.SetParent(parent);
            m_FallLineEffect.transform.position = new Vector3(0, parent.position.y, 0);
        }
    }

    public void StartMaxSpeedFall(Transform parent)
    {
        if (m_FallEffect != null)
            m_FallEffect.DelayDeSpawn();

        m_FallEffect = EffectManager.S.SpawnEffect(EffectName.SuperFalling, -1);
        m_FallEffect.transform.SetParent(parent);
        m_FallEffect.transform.localPosition = Vector3.zero;

        if (m_FallLineEffect == null)
        {
            m_FallLineEffect = EffectManager.S.SpawnEffect(EffectName.SpeedLineFalling_Super, -1);
            m_FallLineEffect.transform.SetParent(parent);
            m_FallLineEffect.transform.position = new Vector3(0, parent.position.y, 0);
        }
    }
    public void StopMaxSpeedFall()
    {
        m_FallEffect.DelayDeSpawn();
        m_FallEffect = null;

        m_FallLineEffect.DelayDeSpawn();
        m_FallLineEffect = null;
    }

}
