using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// アプリ内の広告関連呼び出し処理
/// </summary>
public class AppAdUtils : YnsSingletonMonoBehaviour<AppAdUtils>
{
    // インタースティシャル広告を規定タイミングで表示するまでのカウンタ
    static public readonly int AdInstViewCount = 1;
    //  ''　カレントカウンタ
    static public float m_AdInstViewCount = 0;


    //======================================================================
    // [///] インタースティシャル広告
    //======================================================================
    public static IEnumerator AdUtil_CallInterstitial()
    {
        //// 表示するか
        //if (!AppData.IsInstAdDisp) yield break;

        // 表示カウンタチェック
        if (++m_AdInstViewCount < AdInstViewCount)
        {
            yield break;
        }

        // -------------------------------------------
        // ↓インタースティシャル広告表示
        m_AdInstViewCount = 0;

        // デバッグ表示の呼び出し
        MainGame.MG_Mediator.Dbg_CallAdInterstitialDisp();

        // アナリティクス
        AppAnalyticsUtils.SetAnalitics_Interstitial();

        // インタースティシャル広告の呼び出し
        //yield return AdManager.Instance.PlayMovieInterstitial();
    }
}