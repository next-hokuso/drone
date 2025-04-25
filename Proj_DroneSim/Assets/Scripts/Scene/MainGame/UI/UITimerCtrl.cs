using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MainGame;
using System.Collections.Generic;

/// <summary>
/// タイマー表示クラス
/// </summary>
public class UITimerCtrl : ShUIBaseCtrl
{
    public TMPro.TMP_Text m_Text = null;
    public StopWatch m_Timer = null;
    public bool m_IsUpdate = false;

    public float m_AddTime = 0.0f;
    private bool m_IsCountUp = false;

    public override void ProcInitialize()
    {
        m_Text = GetComponent<TMPro.TMP_Text>();
        m_Text.text = "Time:00:00";
        m_Timer = new StopWatch();
    }

    //
    public void Update()
    {
        // if (m_IsUpdate)
        // {
        //     m_Timer.Update();
        //     TextUpdate();
        // }
    }
    private void TextUpdate()
    {
        float timeVal = m_Timer.TimeVal;
        m_Text.text = "Time:" + AppCommon.GetTimeText_Sec(timeVal);
    }

    // 外部アップデート
    public void ProcUpdate()
    {
        if (m_IsUpdate && !m_IsCountUp)
        {

            // タイム反映
            if (AppData.m_PlayMode <= AppData.PlayMode.Replay)
            {
                m_Timer.Update();
                TextUpdate();

                MG_Mediator.MainCanvas.GetTimeInfo().SetTimeText(ShInputManager.I.GetTimerCtrl().GetTime());
            }
            else if(AppData.m_PlayMode == AppData.PlayMode.Game_Endress)
            {
                m_Timer.Update();
                float timeVal = 30.0f - m_Timer.TimeVal;
                if (timeVal <= 0) timeVal = 0;
                m_Text.text = "Time:" + AppCommon.GetTimeText_Sec(timeVal);

                //MGEndress_Mediator.MainCanvas.GetTimeInfo().SetTimeText(ShInputManager.I.GetTimerCtrl().GetTime());
            }
            else if (AppData.m_PlayMode == AppData.PlayMode.Game_Treasure)
            {
                m_Timer.Update();
                float timeVal = 60.0f - m_Timer.TimeVal;
                if (timeVal <= 0) timeVal = 0;
                m_Text.text = "Time:" + AppCommon.GetTimeText_Sec(timeVal);

                //MGEndress_Mediator.MainCanvas.GetTimeInfo().SetTimeText(ShInputManager.I.GetTimerCtrl().GetTime());
            }
        }

    }

    public void AddTime_CountUp(float addTime)
    {
        m_IsCountUp = true;
        m_AddTime = addTime;
        StartCoroutine(ProcCountUp());
    }
    private IEnumerator ProcCountUp()
    {
        float time = 0.0f;
        float stVal = m_Timer.TimeVal;
        float tempVal = 0.0f;
        while (true)
        {
            time += Time.deltaTime;
            if(time > 0.3f)
            {
                break;
            }

            tempVal = stVal - m_AddTime * (time / 0.3f);
            tempVal = tempVal < 0 ? 0 : tempVal;
            m_Timer.TimeVal = tempVal;

            if (AppData.m_PlayMode == AppData.PlayMode.Game_Endress)
            {
                float timeVal = 30.0f - m_Timer.TimeVal;
                if (timeVal <= 0) timeVal = 0;
                m_Text.text = "Time:" + AppCommon.GetTimeText_Sec(timeVal);
            }
            else if (AppData.m_PlayMode == AppData.PlayMode.Game_Treasure)
            {
                float timeVal = 60.0f - m_Timer.TimeVal;
                if (timeVal <= 0) timeVal = 0;
                m_Text.text = "Time:" + AppCommon.GetTimeText_Sec(timeVal);
            }
            yield return null;
        }

        m_Timer.TimeVal = stVal - m_AddTime;

        if (AppData.m_PlayMode == AppData.PlayMode.Game_Endress)
        {
            float timeVal = 30.0f - m_Timer.TimeVal;
            if (timeVal <= 0) timeVal = 0;
            m_Text.text = "Time:" + AppCommon.GetTimeText_Sec(timeVal);
        }
        else if (AppData.m_PlayMode == AppData.PlayMode.Game_Treasure)
        {
            float timeVal = 60.0f - m_Timer.TimeVal;
            if (timeVal <= 0) timeVal = 0;
            m_Text.text = "Time:" + AppCommon.GetTimeText_Sec(timeVal);
        }
        m_IsCountUp = false;
    }

    // 開始
    public void ProcStart()
    {
        m_IsUpdate = true;
    }
    // 停止
    public void ProcGameEnd()
    {
        m_IsUpdate = false;
    }

    // リセット
    public void ProcReset()
    {
        m_IsUpdate = false;
        m_Text.text = "Time:00:00";
        m_Timer.Reset();
    }

    // 取得
    public float GetTime()
    {
        return m_Timer.TimeVal;
    }

    // セットタイム
    public void SetTime(float val)
    {
        m_Timer.TimeVal = val;
    }

    // 追加
    public void AddTime(float val)
    {
        m_Timer.TimeVal -= val;
    }
}
