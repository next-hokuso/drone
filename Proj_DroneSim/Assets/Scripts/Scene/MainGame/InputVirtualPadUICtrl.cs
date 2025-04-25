using System.Collections;
using UnityEngine;
public class InputVirtualPadUICtrl : ShUIBaseCtrl
{
    private Vector3 m_TempVec = Vector3.zero;
    private RectTransform m_PadT = null;
    private float m_PadMoveLimitVal = 0.0f;
    private IsCheckDrawArea m_PadTapCheck = null;

    // 入力値表示
    private TMPro.TMP_Text m_ValXText = null;
    private TMPro.TMP_Text m_ValYText = null;

    // タッチ時用val
    private float m_ValXRate = 0.0f;
    private float m_ValYRate = 0.0f;

    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================
    public override void ProcInitialize() {
        m_PadT = transform.Find("TapPad/Pad").GetComponent<RectTransform>();
        RectTransform pad = transform.Find("TapPad").GetComponent<RectTransform>();
        m_PadMoveLimitVal = pad.sizeDelta.x * 0.6f - m_PadT.sizeDelta.x * 0.5f;

        m_ValXText = transform.Find("Grp_XVal/Text").GetComponent<TMPro.TMP_Text>();
        m_ValYText = transform.Find("Grp_YVal/Text").GetComponent<TMPro.TMP_Text>();

        m_ValXText.text = "0";
        m_ValYText.text = "0";
    }
    // LRスティック別設定
    public void ProcInitialize_LRSetting(bool isLeft)
    {
        bool isModeDefault = (AppData.GetCurrentConnectMode() == AppData.ConnectM.Mode2);
        bool isL_UpDownRotUI = isModeDefault;
        bool isR_Move = isModeDefault;

        if (isLeft)
        {
            // パッド画像設定
            transform.Find("TapPad/Img_Base_UpDown_Rot").gameObject.SetActive(isL_UpDownRotUI);
            transform.Find("TapPad/Img_Base_Move").gameObject.SetActive(!isL_UpDownRotUI);
        }
        else
        {
            // パッド画像設定
            transform.Find("TapPad/Img_Base_UpDown_Rot").gameObject.SetActive(!isR_Move);
            transform.Find("TapPad/Img_Base_Move").gameObject.SetActive(isR_Move);
        }
    }

    private void ProcReset()
    {

    }

    public IsCheckDrawArea GetIsCheckTapArea()
    {
        return transform.Find("TapPad/Pad").GetComponent<IsCheckDrawArea>();
    }

    public float GetTouchStickRateX()
    {
        return m_ValXRate;
    }
    public float GetTouchStickRateY()
    {
        return m_ValYRate;
    }

    public void SetPad_TouchL(Vector3 tapPos, float valXX, float valYY)
    {
        Vector3 dir = (tapPos - transform.position).normalized;

        // カメラキャンバスにアタッチメントされていて位置ズレていたためOverrayに変更
        float distX = (valXX - transform.position.x);
        float valX = distX / m_PadMoveLimitVal;

        float distY = (valYY - transform.position.y);
        float valY = distY / m_PadMoveLimitVal;

        m_ValXRate = Mathf.Clamp(valX, -1.0f, 1.0f);
        m_ValYRate = Mathf.Clamp(valY, -1.0f, 1.0f);

        m_TempVec.x = m_PadMoveLimitVal * m_ValXRate;
        m_TempVec.y = m_PadMoveLimitVal * m_ValYRate;
        // 超えていた場合
        if(Vector3.Distance(tapPos, transform.position) > m_PadMoveLimitVal)
        {
            // 変更
            m_TempVec.x = m_PadMoveLimitVal * dir.x;
            m_TempVec.y = m_PadMoveLimitVal * dir.y;

            // rate
            m_ValXRate = m_TempVec.x / m_PadMoveLimitVal;
            m_ValYRate = m_TempVec.y / m_PadMoveLimitVal;
        }
        SetPad(m_ValXRate, m_ValYRate);
    }

    public void SetPad(float valX, float valY)
    {
        m_TempVec.x = m_PadMoveLimitVal * valX;
        m_TempVec.y = m_PadMoveLimitVal * valY;
        m_PadT.localPosition = m_TempVec;

        // 入力値表示更新
        if(valX > 0.0f)
        {
            m_ValXText.text = "+" + Mathf.RoundToInt(valX * 100.0f).ToString();
        }
        else if(valX < 0.0f)
        {
            m_ValXText.text = Mathf.RoundToInt(valX * 100.0f).ToString();
        }
        else
        {
            m_ValXText.text = "0";
        }

        if (valY > 0.0f)
        {
            m_ValYText.text = "+" + Mathf.RoundToInt(valY * 100.0f).ToString();
        }
        else
        {
            m_ValYText.text = Mathf.RoundToInt(valY * 100.0f).ToString();
        }
    }
}
