using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MainGame;
using System.Collections.Generic;

/// <summary>
/// タイマー表示クラス
/// </summary>
public class UIMissionFailedCtrl : ShUIBaseCtrl
{
    //================================================
    // [///] 定義
    //================================================
    private Vector3 m_SavePos = Vector3.zero;
    private RectTransform m_RectT = null;

    // 演出フラグ
    private bool m_IsProcEnd = false;
    private bool m_IsInOutAnimPlay = false;
    private bool m_IsWindowOpen = false;

    private TMPro.TMP_Text m_Text = null;
    private TMPro.TMP_Text m_TextVal = null;

    //================================================
    // [///]
    //================================================
    public override void ProcInitialize()
    {
        m_RectT = GetComponent<RectTransform>();

        m_Text = transform.Find("Text").GetComponent<TMPro.TMP_Text>();
        m_TextVal = transform.Find("Val").GetComponent<TMPro.TMP_Text>();

        gameObject.SetActive(false);
    }
    private void Start()
    {
    }
    private void Update()
    {
    }

    // リセット
    public void ProcReset()
    {
        m_IsProcEnd = false;
        m_IsWindowOpen = false;
        m_IsInOutAnimPlay = false;
    }

    public void ProcUISetting(int point, MissionSubtractPointId id)
    {
        switch (id)
        {
            case MissionSubtractPointId.OutArea:         m_Text.text = "飛行経路逸脱";  break;
            case MissionSubtractPointId.DiffInstruction: m_Text.text = "指示と異なる飛行";  break;
            case MissionSubtractPointId.DefectiveDir:    m_Text.text = "機首方向不良";  break;
            case MissionSubtractPointId.Huratuki:        m_Text.text = "ふらつき";  break;
            case MissionSubtractPointId.HuEnkatu:        m_Text.text = "不円滑";  break;
        }
        m_TextVal.text = point.ToString();

        // In/Out切り替え
        if (m_IsWindowOpen)
        {
            SetFrameOut();
            m_IsWindowOpen = false;
        }
        else
        {
            SetWindow();
            m_IsWindowOpen = true;
        }
    }

    // 使用中か
    public bool IsDisp()
    {
        return m_IsWindowOpen;
    }

    //================================================
    // [///] public method
    //================================================        
    // ウィンドウIn設定
    public void SetWindow()
    {
        if (m_IsWindowOpen) return;
        m_IsWindowOpen = true;

        m_IsProcEnd = false;
        m_IsInOutAnimPlay = true;

        // 位置調整
        if (m_SavePos == Vector3.zero)
        {
            m_SavePos = m_RectT.localPosition;
        }
        gameObject.transform.localPosition = m_SavePos + Vector3.right * 500.0f;
        gameObject.SetActive(true);

        StartCoroutine(ProcInWindow());
    }
    private IEnumerator ProcInWindow()
    {
        float timer = 0.0f;
        float endTime = 0.5f;
        while (true)
        {
            timer += Time.fixedDeltaTime;
            gameObject.transform.localPosition = Vector3.Lerp(m_SavePos + Vector3.right * 500.0f, m_SavePos, timer / endTime);
            if(timer > endTime)
            {
                break;
            }
            yield return null;
        }

        gameObject.transform.localPosition = m_SavePos;

        m_IsProcEnd = true;
        m_IsInOutAnimPlay = false;

        StartCoroutine(ProcAutoFrameOut());
    }
    private IEnumerator ProcAutoFrameOut()
    {
        float timer = 0.0f;
        float endTime = 1.5f;
        while (true)
        {
            timer += Time.fixedDeltaTime;
            if (timer > endTime)
            {
                break;
            }
            yield return null;
        }

        SetFrameOut();
    }
    private void OnUpdatePosition()
    {
    }
    // フレームアウト
    public void SetFrameOut()
    {
        if (!m_IsWindowOpen) return;
        m_IsWindowOpen = false;

        m_IsInOutAnimPlay = true;

        StartCoroutine(ProcOutWindow());
    }
    private IEnumerator ProcOutWindow()
    {
        float timer = 0.0f;
        float endTime = 0.5f;
        while (true)
        {
            timer += Time.fixedDeltaTime;
            gameObject.transform.localPosition = Vector3.Lerp(m_SavePos, m_SavePos + Vector3.right * 500.0f, timer / endTime);
            if (timer > endTime)
            {
                break;
            }
            yield return null;
        }

        gameObject.transform.localPosition = m_SavePos + Vector3.right * 500.0f;

        m_IsInOutAnimPlay = false;
    }

    //================================================
    // [///] public method
    //================================================
    public bool IsProcEnd()
    {
        return m_IsProcEnd;
    }
}
