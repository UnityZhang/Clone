using UnityEngine;
using UnityEngine.UI;
using Qarth;

public class Board_RemoveSlinky : AProp
{
    public int Count = 2;
    public Text Text;
    int m_Timer;

    public override void Trigger()
    {
        m_Timer = Timer.S.Post2Scale((count) =>
        {
            if (!H_GameCtrl.S.StartGame)
                Timer.S.Cancel(m_Timer);
            else
            {
                int value = Count - 1;
                PlayerSpring.S.AddSlinky(-1);
                Text.text = value.ToString();
                if (count == Count)
                {
                    gameObject.SetActive(false);
                    PlayerSpring.S.StartFall();
                }
            }
        }, 0.1f, Count);
    }

    private void Start()
    {
        Text.text = Count.ToString();
    }

    
}