using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MainGame;

public class UIInfoCtrl : ShUIBaseCtrl
{
    private enum InfoId
    {
        Vertical_Spd,
        Holizontal_Spd,
        Point,
        Time,
    }
    public TMPro.TMP_Text m_Text = null;

    public override void ProcInitialize()
    {
        m_Text = transform.Find("Val").GetComponent<TMPro.TMP_Text>();
        m_Text.text = "";
    }

    //
    public void Update()
    {
    }

    // リセット
    public void ProcReset()
    {
        m_Text.text = "";
    }

    // 設定
    public void SetHorizontalSpd(float spd)
    {
        if(spd < 0.0f) { spd *= -1.0f; }
        m_Text.text = spd.ToString("f1");
    }
    public void SetVerticalSpd(float spd)
    {
        if (spd < 0.0f) { spd *= -1.0f; }
        m_Text.text = spd.ToString("f1");
    }
    public void SetPoint(int point)
    {
        m_Text.text = point.ToString();
    }
    public void SetTimeText(float time)
    {
        float timeVal = (60.0f * 5.0f) - time;
        m_Text.text = Mathf.RoundToInt(timeVal).ToString();//AppCommon.GetTimeText_Sec(timeVal);
    }
}
