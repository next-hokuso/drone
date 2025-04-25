using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shs;
using UnityEngine.UI;
using MainGame;
using Game;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// キャンバス制御スクリプト：
/// 　シーンの読み込み時に呼び出すものを抽象化
/// 　各シーンごとに読み込まれるものは継承先で実装する
/// 　(シーンのから呼び出す)
/// </summary>
public class MGStickChallenge_CanvasCtrl : ShCanvasBaseCtrl, IShVisible
{
    //================================================
    // [///]  各シーンごとに保持する変数
    //================================================
    // IShVisibleで使用する表示変数
    public bool IsVisible { get; private set; } = false;

    // 表示制御としてリスト追加するものの設定
    private enum UINo
    {
        Canvas,

        // 
        BlackBG,
        Btn_Start,

        // 演出
        Anim_Start,

        // リザルト
        Grp_Result,
        Result_Mission,
        Result_Score,
        Result_ReplayBtn,
        Result_ReturnBtn,

        Text_Time,
    }
    protected ShUIList m_UIList = new ShUIList();

    // 自身
    protected Canvas m_ThisCanvas = null;

    // TODO:他に移動
    public bool IsSelectCheck { get; private set; } = false;
    public void SetSelectCheck()
    {
        IsSelectCheck = true;
        StartCoroutine(SelectCheck());
    }
    private IEnumerator SelectCheck()
    {
        yield return new WaitForSeconds(0.4f);

        IsSelectCheck = false;
    }
    // DebugCameraからの戻り識別
    public bool IsDebugCameraReturn { get; set; } = false;
    // Tutorialの表示番号 ※外部からのアクセス専用
    public int TutorialNum { get; set; } = 0;

    // 開始演出
    public bool IsStart { get; private set; } = false;
    public void SetStart()
    {
        IsStart = true;
    }

    private bool m_IsResultSelected = false;
    private int m_ResultBtnIdx = 0;

    // グリッド用:ワイプカメラがゲームカメラか
    // private bool m_IsCurrentWipeGameCam = false;

    //================================================
    // [///] シーンの初期化
    //================================================
    public void Awake()
    {
    }

    public override void ProcInitialize()
    {
        // 自身
        m_ThisCanvas = Shs.ShSceneUtils.GetSceneRootObj(AppCommon.CurrentSceneName, "AppCanvas").GetComponent<Canvas>();

        // 自身から子供の表示制御の付与取得
        {
            Transform canvasT = m_ThisCanvas.transform;

            // リストへの登録
            m_UIList.Clear();
            {
                m_UIList.Add(canvasT.gameObject.AddComponent<ShUIBaseCtrl>());

                m_UIList.Add(canvasT.Find("Image_BG").gameObject.AddComponent<ShUIBaseCtrl>());
                m_UIList.Add(canvasT.Find("Btn_Start").gameObject.AddComponent<ShUIBaseCtrl>());

                // 演出関係
                m_UIList.Add(canvasT.Find("Anim_Start").gameObject.AddComponent<ShUIBaseCtrl>());

                // result
                m_UIList.Add(canvasT.Find("Grp_Result").gameObject.AddComponent<ShUIBaseCtrl>());
                m_UIList.Add(canvasT.Find("Grp_Result/Txt_Mission").gameObject.AddComponent<ShUITMPTextCtrl>());
                m_UIList.Add(canvasT.Find("Grp_Result/Grp_Point/Val").gameObject.AddComponent<ShUITMPTextCtrl>());
                m_UIList.Add(canvasT.Find("Grp_Result/Btn_Replay").gameObject.AddComponent<ShUIBaseCtrl>());
                m_UIList.Add(canvasT.Find("Grp_Result/Btn_Return").gameObject.AddComponent<ShUIBaseCtrl>());

                m_UIList.Add(canvasT.Find("Grp_Hader/Txt_Time").gameObject.AddComponent<UITimerCtrl>());
            }

            // 初期化
            m_UIList.ProcInitialize();

            // 個別設定(ボタンなど) @TODO 処理を移動/改善する
            {
                // 戻る
                Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(
                    canvasT.Find("Grp_Hader/Btn_Return").GetComponent<Button>(), OnClick_ReturnScene);

                //canvasT.Find("Btn_Start").GetComponent<Button>().onClick.AddListener(OnClick_GameStart);
                Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(
                 canvasT.Find("Btn_Start").GetComponent<Button>(), OnClick_GameStart);

                // リザルト
                //canvasT.Find("Grp_Result/Btn_Replay").GetComponent<Button>().onClick.AddListener(OnClick_Replay);
                //canvasT.Find("Grp_Result/Btn_Return").GetComponent<Button>().onClick.AddListener(OnClick_ReturnScene);
                Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(
                    canvasT.Find("Grp_Result/Btn_Replay").GetComponent<Button>(), OnClick_Replay);
                Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(
                    canvasT.Find("Grp_Result/Btn_Return").GetComponent<Button>(), OnClick_ReturnScene);

            }

            // 非表示
            m_UIList.Hide();
        }

        // Resorce Load
        StartCoroutine("ProcResourcesLoad", true);
    }

    //================================================
    // [///] シーンに必要なリソース読み込み
    //================================================
    public override IEnumerator ProcResourcesLoad()
    {
        // 読み込み完了
        m_IsCompleteSceneSetup = true;

        yield break;
    }

    //================================================
    // [///] 表示切り替え
    //================================================
    void IShVisible.Show()
    {
        m_UIList.Show();


        // TODO:キャンバスを開いた際に見えないものはどうするか => デフォルトTorF指定できるようにしたい
        m_UIList.m_List[(int)UINo.Anim_Start].SetVisible(false);
        m_UIList.m_List[(int)UINo.Grp_Result].SetVisible(false);
    }
    void IShVisible.Hide()
    {
        m_UIList.Hide();
    }

    //================================================
    // [///] シーンの処理開始
    //================================================
    public override void ProcSceneProcStart()
    {
    }
    // リセット
    public override void ProcSceneReset()
    {
    }
    // クリア時
    public void ProcGameClear()
    {
    }
    // リザルト待機
    public void ProcResultWait()
    {
    }

    //================================================
    // [///] 開始演出
    //================================================
    public void SetAnimEff_GameStart()
    {
        StartCoroutine(AnimEff_GameStart());
    }
    private IEnumerator AnimEff_GameStart()
    {
        // スタートボタンのOff
        m_UIList.m_List[(int)UINo.Btn_Start].SetVisible(false);

        // // スタート演出
        // m_UIList.m_List[(int)UINo.Anim_Start].SetVisible(true);
        // 
        // // スタート演出待機
        // yield return new WaitForSeconds(3.05f);

        // 暗幕
        float time = 0.25f;
        float tempTime = time;
        Image img = m_UIList.m_List[(int)UINo.BlackBG].GetComponent<Image>();
        Color stCol = img.color;
        Color endCol = img.color;
        endCol.a = 0.0f;
        while (time > 0.0f)
        {
            time += Time.deltaTime * -1.0f;
            img.color = Color.Lerp(stCol, endCol, 1.0f - time / tempTime);
            yield return null;
        }

        // 非表示
        m_UIList.m_List[(int)UINo.Anim_Start].SetVisible(false);
        m_UIList.m_List[(int)UINo.BlackBG].SetVisible(false);
        // 戻す
        img.color = stCol;

        // タイマー開始
        ((UITimerCtrl)m_UIList.m_List[(int)UINo.Text_Time]).ProcStart();
    }

    //================================================
    // [///] TODO:実装部分のため他に移したい
    //================================================
    public void OnClick_GameStart()
    {
        // フラグのON
        SetStart();
    }
    // タイマーの取得
    public UITimerCtrl GetTimerCtrl()
    {
        return ((UITimerCtrl)m_UIList.m_List[(int)UINo.Text_Time]);
    }
    // リザルト
    public void SetResult(int score, bool isFailed=false)
    {
        // 暗幕/リザルト
        m_UIList.m_List[(int)UINo.BlackBG].SetVisible(true);
        m_UIList.m_List[(int)UINo.Grp_Result].SetVisible(true);

        ((ShUITMPTextCtrl)m_UIList.m_List[(int)UINo.Result_Score]).SetText(score.ToString());
        if (isFailed)
        {
            ((ShUITMPTextCtrl)m_UIList.m_List[(int)UINo.Result_Mission]).SetText("時間切れ");
            ((ShUITMPTextCtrl)m_UIList.m_List[(int)UINo.Result_Mission]).m_Text.color = Color.red;
            m_ThisCanvas.transform.Find("Grp_Result/Grp_Point").gameObject.SetActive(false);

            TMPro.TMP_Text text = m_ThisCanvas.transform.Find("Grp_Result/Txt_Time").GetComponent<TMPro.TMP_Text>();
            text.text = "";
        }
        else
        {
            ((ShUITMPTextCtrl)m_UIList.m_List[(int)UINo.Result_Mission]).SetText("宝箱発見！");
            ((ShUITMPTextCtrl)m_UIList.m_List[(int)UINo.Result_Mission]).m_Text.color = Color.yellow;


            {
                TMPro.TMP_Text text = m_ThisCanvas.transform.Find("Grp_Result/Txt_Time").GetComponent<TMPro.TMP_Text>();
                float timeVal = 60.0f - ShInputManager.I.GetTimerCtrl().GetTime();
                if (timeVal <= 0) timeVal = 0;
                text.text = "Time:" + AppCommon.GetTimeText_Sec(timeVal);
            }
        }

        EventSystem.current.SetSelectedGameObject(m_UIList.m_List[(int)UINo.Result_ReplayBtn].gameObject);

        m_IsResultSelected = true;
    }
    public override void Update()
    {
        if (m_IsResultSelected)
        {
            if (AppData.GetPadTrg(AppData.Action.Decide) || AppData.GetTrg_KeyOnly(AppData.Action.Decide))
            {
                m_IsResultSelected = false;
                {
                    if (m_ResultBtnIdx == 0)
                    {
                        OnClick_Replay();
                        m_UIList.m_List[(int)UINo.Result_ReplayBtn].gameObject.GetComponent<Button>().enabled = false;
                        // SE
                        CoreManager.I.AudioComp.PlaySe(AudioId.count.ToString(), false);
                    }
                    else
                    {
                        OnClick_ReturnScene();
                        m_UIList.m_List[(int)UINo.Result_ReturnBtn].gameObject.GetComponent<Button>().enabled = false;
                        // SE
                        CoreManager.I.AudioComp.PlaySe(AudioId.count.ToString(), false);
                    }
                }
            }
            else if (AppData.GetPadTrg(AppData.Action.LDown) || 
                 AppData.GetTrg_KeyOnly(AppData.Action.LDown) ||
                 AppData.GetTrg_KeyOnly(AppData.Action.RDown))
            {
                m_ResultBtnIdx++;
                if (m_ResultBtnIdx > 1) m_ResultBtnIdx = 0;
                if (m_ResultBtnIdx == 0)
                {
                    EventSystem.current.SetSelectedGameObject(m_UIList.m_List[(int)UINo.Result_ReplayBtn].gameObject);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(m_UIList.m_List[(int)UINo.Result_ReturnBtn].gameObject);
                }
            }
            else if (AppData.GetPadTrg(AppData.Action.LUp) || 
                AppData.GetTrg_KeyOnly(AppData.Action.LUp) ||
                 AppData.GetTrg_KeyOnly(AppData.Action.RUp))
            {
                m_ResultBtnIdx--;
                if (m_ResultBtnIdx < 0) m_ResultBtnIdx = 1;
                if (m_ResultBtnIdx == 0)
                {
                    EventSystem.current.SetSelectedGameObject(m_UIList.m_List[(int)UINo.Result_ReplayBtn].gameObject);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(m_UIList.m_List[(int)UINo.Result_ReturnBtn].gameObject);
                }
            }
            AppData.ResetPadTrgs();
        }
    }

    //================================================
    // [///] TODO:実装部分のため他に移したい2
    //================================================
    public void OnClick_GameRetry()
    {
        MainGame.MGStickChallenge_Mediator.StateCtrl.SetMainGameReset();
    }
    public void OnClick_Settings()
    {
        // 開けるか
        if(!MGStickChallenge_Mediator.StateCtrl.IsUseMenu()) return;

        // このタイミングではゲームを停止する
        MGStickChallenge_Mediator.StateCtrl.CallGameWait();
    }
    public void OnClick_Return_Settings()
    {
        // このタイミングではゲームを再開する
        MGStickChallenge_Mediator.StateCtrl.CallGameReStart();
    }
    // 戻る
    public void OnClick_ReturnScene()
    {
        // ドローン操作ログの設定
        MGStickChallenge_Mediator.ObjMediator.SaveDroneInputLog();

        // @取り急ぎ
        ShSceneUtils.GetAppCtrlObj("Game_StickChallenge").GetComponent<Game_StickChallengeBaseCtrl>().ProcChangeScene();
    }
    // リプレイ
    public void OnClick_Replay()
    {
        // ドローン操作ログの設定
        MGStickChallenge_Mediator.ObjMediator.SaveDroneInputLog();

        // @取り急ぎ
        ShSceneUtils.GetAppCtrlObj("Game_StickChallenge").GetComponent<Game_StickChallengeBaseCtrl>().ProcChangeScene_Replay();
    }
}
