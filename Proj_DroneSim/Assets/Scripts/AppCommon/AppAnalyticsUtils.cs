using System;
using UnityEngine;

/// <summary>
/// アプリ内のアナリティクス送信用のクラス
/// </summary>
public class AppAnalyticsUtils : YnsSingletonMonoBehaviour<AppAnalyticsUtils>
{
    //======================================================================
    //
    // [///] game_start
    //
    //======================================================================
    ///<summary> アナリティクス設定:ステージ画面表示時)</summary>
    public static void SetAnalitics_GameStart_DispStage()
    {
        //int stageNo = AppData.GetNowStageDispNo();
        //string log = "AnalyticsLog:game_start ステージ画面表示時　";
        //log += "[stageNo:" + stageNo.ToString() + "]";
        //Debug.Log(log);

        //InAppEventManager.Instance.SendGameStartEvent(stageNo);
    }
    ///<summary> アナリティクス設定:ステージ選択時(STARTボタン押下時)</summary>
    public static void SetAnalitics_GameStart_StageSelect()
    {
        //int stageNo = AppData.GetNowStageDispNo();
        //string log = "AnalyticsLog:game_start ステージボタン押下時　";
        //log += "[stageNo:" + stageNo.ToString() + "]";
        //Debug.Log(log);

        //InAppEventManager.Instance.SendGameStartEvent(stageNo);
    }
    ///<summary> アナリティクス設定:ゲームプレイ:リトライボタン押下時
    ///(結果画面)</summary>
    public static void SetAnalitics_GameStart_Retry()
    {
        //int stageNo = AppData.GetNowStageDispNo();
        //string log = "AnalyticsLog:game_start リトライボタン押下時　";
        //log += "[stageNo:" + stageNo.ToString() + "]";
        //Debug.Log(log);

        //InAppEventManager.Instance.SendGameStartEvent(stageNo);
    }
    ///<summary> ゲームプレイ(結果画面)NEXTボタン押下時
    public static void SetAnalitics_GameStart_Next()
    {
        //int stageNo = AppData.GetNowStageDispNo();
        //string log = "AnalyticsLog:game_start NEXTボタン押下時　";
        //log += "[stageNo:" + stageNo.ToString() + "]";
        //Debug.Log(log);

        //InAppEventManager.Instance.SendGameStartEvent(stageNo);
    }

    //======================================================================
    //
    // [///] get_score
    //
    //======================================================================
    ///<summary> アナリティクス設定:結果画面表示時</summary>
    public static void SetAnalitics_GetScore_Clear()
    {
        //int stageNo = AppData.GetNowStageDispNo();
        //int score = 1;
        //
        //string log = "AnalyticsLog:get_score 結果画面表示時　";
        //log += "[stageNo:" + stageNo.ToString() + "]";
        //log += " score:" + score + " ]";
        //Debug.Log(log);

        //InAppEventManager.Instance.SendGetScoreEvent(stageNo, score);
    }
    ///<summary> アナリティクス設定:結果画面表示時</summary>
    public static void SetAnalitics_GetScore_GameOver()
    {
        //int stageNo = AppData.GetNowStageDispNo();
        //int score = 0;
        //
        //string log = "AnalyticsLog:get_score 結果画面表示時　";
        //log += "[stageNo:" + stageNo.ToString();
        //log += " score:" + score + " ]";
        //Debug.Log(log);

        //InAppEventManager.Instance.SendGetScoreEvent(stageNo, 0);
    }

    //======================================================================
    //
    // [///] view_ad_inst
    //
    //======================================================================
    ///<summary> インタースティシャル広告(</summary>
    public static void SetAnalitics_Interstitial()
    {
        Debug.Log("AnalyticsLog:view_ad_inst");
        //InAppEventManager.Instance.SendViewAdInstEvent();
    }

}