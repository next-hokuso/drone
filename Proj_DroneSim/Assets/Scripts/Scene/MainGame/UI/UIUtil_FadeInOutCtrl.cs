using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// フェード操作用クラス
/// 　付与したオブジェクトから子供の Text / Image のカラー操作
/// </summary>
public class UIUtil_FadeInOutCtrl : ShUIBaseCtrl
{
    //================================================
    // [///] 定義
    //================================================
    private const float CommonFadeTime = 0.25f;

    //================================================
    // [///] 定義
    //================================================
    // 演出フラグ
    private bool m_IsGetData = false;
    private bool m_IsProcEnd = false;
    private bool m_IsWindowOpen = false;

    // フェードイン/アウト時間変更用
    private bool m_IsChangeFadeInOutTime = false;
    private float m_ChangeFadeInTime = CommonFadeTime;
    private float m_ChangeFadeOutTime = CommonFadeTime;

    // テキスト関係
    private CanvasGroup m_CanvasGroup = null;

    //================================================
    // [///]
    //================================================
    public override void ProcInitialize()
    {
    }

    // データ設定
    private void GetComponentData()
    {
    }

    // フェードイン/アウト時間の変更
    public void SetForce_FadeInOutTime(float inTime, float outTime)
    {
        m_IsChangeFadeInOutTime = true;
        m_ChangeFadeInTime = inTime;
        m_ChangeFadeOutTime = outTime;
    }

    //================================================
    // [///] 処理
    //================================================
    // フェードイン処理
    public void Set_FadeIn()
    {
        //if (m_IsWindowOpen) return;
        m_IsWindowOpen = true;

        // データを取得していない
        if (!m_IsGetData)
        {
            m_IsGetData = true;
            GetComponentData();
            m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        m_IsProcEnd = false;
        StartCoroutine(ProcFadeIn_Normal());
    }
    private IEnumerator ProcFadeIn_Normal()
    {
        float time = 0.0f;
        float endTime = CommonFadeTime;
        if (m_IsChangeFadeInOutTime) { endTime = m_ChangeFadeInTime; }

        while (true)
        {
            time += Time.deltaTime;
            if(time > endTime)
            {
                break;
            }
            m_CanvasGroup.alpha = time / endTime;
            yield return null;
        }


        // フェード完了後処理
        m_CanvasGroup.alpha = 1.0f;
        m_IsProcEnd = true;
    }
    public float GetFadeInTime()
    {
        return m_ChangeFadeInTime;
    }


    //================================================
    //
    // [///] Fade - Out
    //
    //================================================
    public void Set_FadeOut()
    {
        if (!m_IsWindowOpen) return;
        m_IsWindowOpen = false;

        // データを取得していない
        if (!m_IsGetData)
        {
            m_IsGetData = true;
            GetComponentData();
        }

        m_IsProcEnd = false;
        StartCoroutine(ProcFadeOut_Normal());
    }
    private IEnumerator ProcFadeOut_Normal()
    {
        float time = 0.0f;
        float endTime = CommonFadeTime;
        if (m_IsChangeFadeInOutTime) { endTime = m_ChangeFadeOutTime; }

        while (true)
        {
            time += Time.deltaTime;
            if (time > endTime)
            {
                break;
            }
            m_CanvasGroup.alpha = 1.0f - time / endTime;
            yield return null;
        }

        // フェード完了後処理
        m_CanvasGroup.alpha = 0.0f;
        m_IsProcEnd = true;
    }
    public float GetFadeOutTime()
    {
        return m_ChangeFadeOutTime;
    }
    // フェードアウト状態に変更
    public void SetState_FadeOut()
    {
        // データを取得していない
        if (!m_IsGetData)
        {
            m_IsGetData = true;
            GetComponentData();
            m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // フェード完了後処理
        m_CanvasGroup.alpha = 0.0f;
        m_IsProcEnd = true;
    }

    //================================================
    // [///] public method
    //================================================
    public bool IsProcEnd()
    {
        return m_IsProcEnd;
    }
}
