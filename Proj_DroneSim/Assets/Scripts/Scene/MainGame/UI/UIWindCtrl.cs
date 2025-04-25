using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MainGame;

public class UIWindCtrl : ShUIBaseCtrl
{
    private float[] RotList = { 0.001f, -45.0f, -90.0f, -135.0f, 179.99f, 135.0f, 90.0f, 45.0f };
    private WindDirNo m_WindDirNo = WindDirNo.Up;
    public TMPro.TMP_Text m_Text = null;

    // update
    private bool m_IsUpdate = false;
    private float m_Time = 0.0f;
    private float m_EndTime = 1.0f;
    private Vector3 m_StRot = Vector3.zero;
    private RectTransform m_WindUI = null;

    public override void ProcInitialize()
    {
        m_Text = transform.Find("Txt_Wind").GetComponent<TMPro.TMP_Text>();
        m_Text.text = "0.0m";

        m_WindUI = transform.Find("Img_Wind").GetComponent<RectTransform>();
    }

    //
    public void Update()
    {
        if (m_IsUpdate)
        {
            m_Time += Time.fixedDeltaTime;
            m_WindUI.localEulerAngles = Vector3.Lerp(m_StRot, Vector3.forward * RotList[(int)m_WindDirNo], m_Time / m_EndTime);
            if(m_Time > m_EndTime)
            {
                m_IsUpdate = false;
                m_WindUI.localEulerAngles = Vector3.forward * RotList[(int)m_WindDirNo];
            }
        }
    }

    // リセット
    public void ProcReset()
    {
        m_Text.text = "";
    }

    // 設定
    private void SetWindUIRotSetting()
    {
        m_StRot = m_WindUI.transform.localEulerAngles;
        m_Time = 0.0f;
        m_IsUpdate = true;
    }

    // 設定
    public void SetWind(float wind, WindDirNo no)
    {
        m_WindDirNo = no;
        SetWindUIRotSetting();

        m_Text.text = wind.ToString("f1") + "m";
    }
}
