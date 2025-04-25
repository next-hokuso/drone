using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// ダイアログ操作汎用クラス
/// </summary>
public class UIUtil_WindowCtrl : ShUIBaseCtrl
{
    //================================================
    // [///] 定義
    //================================================
    // 演出フラグ
    private bool m_IsProcEnd = false;
    private bool m_IsInOutAnimPlay = false;
    private bool m_IsWindowOpen = false;
    public bool IsWindowOpen() { return m_IsWindowOpen; }

    // フェードイン/アウト操作
    private UIUtil_FadeInOutCtrl m_FadeCtrl = null;

    //================================================
    // [///] 初期化
    //================================================
    public override void ProcInitialize()
    {
        // フェード操作設定
        m_FadeCtrl = gameObject.AddComponent<UIUtil_FadeInOutCtrl>();
    }

    //=========================================================================
    //
    //
    // [///] フレームイン設定
    //
    //
    //=========================================================================
    // ウィンドウIn設定
    public void SetOpenWindow()
    {
        if (IsInOutAnimPlay()) return;
        m_IsWindowOpen = true;

        m_IsProcEnd = false;
        m_IsInOutAnimPlay = true;

        gameObject.SetActive(true);
        StartCoroutine(ProcInWindow());
    }
    private IEnumerator ProcInWindow()
    {
        m_FadeCtrl.Set_FadeIn();
        yield return new WaitForSeconds(m_FadeCtrl.GetFadeInTime());

        SetWindowForceOpen();
    }
    // 即強制開く
    public void SetWindowForceOpen()
    {
        m_IsWindowOpen = true;
        m_IsProcEnd = true;
        m_IsInOutAnimPlay = false;
        gameObject.SetActive(true);
    }

    //=========================================================================
    //
    //
    // [///] フレームアウト設定
    //
    //
    //=========================================================================
    // フレームアウト
    public void SetCloseWindow()
    {
        if (IsInOutAnimPlay()) return;
        m_IsWindowOpen = false;

        m_IsInOutAnimPlay = true;

        StartCoroutine(ProcOutWindow());
    }
    private IEnumerator ProcOutWindow()
    {
        m_FadeCtrl.Set_FadeOut();
        yield return new WaitForSeconds(m_FadeCtrl.GetFadeOutTime());

        SetWindowForceClose();
        yield break;
    }
    // 即強制閉じる
    public void SetWindowForceClose()
    {
        m_IsWindowOpen = false;
        m_IsInOutAnimPlay = false;
        gameObject.SetActive(false);
    }


    //=========================================================================
    //
    //
    // [///] 状態チェック
    //
    //
    //=========================================================================
    public bool IsProcEnd()
    {
        return m_IsProcEnd;
    }
    public bool IsInOutAnimPlay()
    {
        return m_IsInOutAnimPlay;
    }
}
