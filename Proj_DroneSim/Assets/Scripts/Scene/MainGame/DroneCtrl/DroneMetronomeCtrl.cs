using Game;
using UnityEngine;
public class DroneMetronomeCtrl : MonoBehaviour
{
    private bool m_IsUpdate = false;

    private float m_Timer = 0.0f;
    private float m_CheckTime = 0.0f;

    public void Start()
    {
        m_CheckTime = 60.0f / AppData.MetronomeBPM;
        // Debug.Log("メトロノーム更新時間 : " + m_CheckTime);
    }
    public void Update()
    {
        if (m_IsUpdate)
        {
            m_Timer += Time.fixedDeltaTime;
            if(m_Timer > m_CheckTime)
            {
                m_Timer -= m_CheckTime;
                // SE
                MainGame.MG_Mediator.GetAudio().PlaySe(AudioId.metronome.ToString(), false);
            }
        }
    }

    // リセット
    public void Reset()
    {
        m_Timer = 0.0f;
    }

    // 停止
    public void SetStop()
    {
        m_IsUpdate = false;
    }

    public void SetMetronome(bool isActive)
    {
        m_IsUpdate = isActive;

        if (isActive)
        {
            Reset();
        }

        // セーブデータ反映
        AppData.SetMetronomeFlightSound(isActive);
        AppCommon.Update_And_SaveGameData();
    }
}
