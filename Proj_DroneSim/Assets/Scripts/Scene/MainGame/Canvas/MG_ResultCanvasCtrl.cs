using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shs;
using UnityEngine.UI;
using MainGame;
using Game;

/// <summary>
/// キャンバス制御スクリプト：
/// 　シーンの読み込み時に呼び出すものを抽象化
/// 　各シーンごとに読み込まれるものは継承先で実装する
/// 　(シーンのから呼び出す)
/// </summary>
public class MG_ResultCanvasCtrl : ShCanvasBaseCtrl, IShVisible
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

        Btn_Retry,         // リトライ
        Btn_Next,          // Next
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

    private bool m_IsAddCoinFlag = false;

    private bool m_IsBeforeStageChange = false;

    //================================================
    // [///] シーンの初期化
    //================================================
    public override void ProcInitialize()
    {
        // 自身
        m_ThisCanvas = Shs.ShSceneUtils.GetSceneRootObj("MainGame", "ResultCanvas").GetComponent<Canvas>();

        // 自身から子供の表示制御の付与取得
        {
            Transform canvasT = m_ThisCanvas.transform;

            // リストへの登録
            m_UIList.Clear();
            {
                m_UIList.Add(canvasT.gameObject.AddComponent<ShUIBaseCtrl>());

                m_UIList.Add(canvasT.Find("Btn_Retry").gameObject.AddComponent<ShUIBaseCtrl>());
                m_UIList.Add(canvasT.Find("Btn_Next").gameObject.AddComponent<ShUIBaseCtrl>());
            }

            // 初期化
            m_UIList.ProcInitialize();

            // 個別設定(ボタンなど) @TODO 処理を移動/改善する
            {
                // ボタン
                m_UIList.m_List[(int)UINo.Btn_Retry].GetComponent<Button>().
                    onClick.AddListener(OnClick_GameRetry);
                m_UIList.m_List[(int)UINo.Btn_Next].GetComponent<Button>().
                    onClick.AddListener(OnClick_GameNext);

                // 設定
            }

            // 非表示
            m_UIList.Hide();
        }

        // Resorce Load
        //StartCoroutine("ProcResourcesLoad", true);
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

        m_UIList.m_List[(int)UINo.Btn_Retry].SetVisible(false);
        m_UIList.m_List[(int)UINo.Btn_Next].SetVisible(false);

        // Retryボタンの有効化
        m_UIList.m_List[(int)UINo.Btn_Retry].SetVisible(true);
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
        m_UIList.Hide();
    }
    // クリア時
    public void ProcGameClear()
    {
    }

    //================================================
    // [///] TODO:実装部分のため他に移したい
    //================================================

    //================================================
    // [///] TODO:実装部分のため他に移したい2
    //================================================
    public bool IsBeforeStageChange()
    {
        return m_IsBeforeStageChange;
    }

    public void OnClick_GameRetry()
    {
        // ネットワークチェック
        if (MG_Mediator.NetworkCheckCanvas.CheckNetwork())
        {
            return;
        }

        // Retry/Nextによる遷移前時は弾く
        if (IsBeforeStageChange()) return;
        // リセット中は弾く
        if (MG_Mediator.StateCtrl.IsMainGameReset()) return;
        m_IsBeforeStageChange = true;
        StartCoroutine(ProcGameRetry());
    }
    private IEnumerator ProcGameRetry()
    {
        // ボタン演出
        StartCoroutine(ShUIUtils.ProcBtnEffect(m_UIList.m_List[(int)UINo.Btn_Retry].gameObject));
        // ボタン演出待ち
        yield return new WaitForSeconds(0.4f);

        // アナリティクス設定:(結果画面)リトライボタン押下時
        AppAnalyticsUtils.SetAnalitics_GameStart_Retry();

        // インタースティシャル広告
        yield return StartCoroutine(AppAdUtils.AdUtil_CallInterstitial());

        m_IsBeforeStageChange = false;

        MainGame.MG_Mediator.StateCtrl.SetMainGameReset();
    }
    public void OnClick_GameNext()
    {
        // ネットワークチェック
        if (MG_Mediator.NetworkCheckCanvas.CheckNetwork())
        {
            return;
        }

        // Retry/Nextによる遷移前時は弾く
        if (IsBeforeStageChange()) return;
        // リセット中（ステージ選択からのステージ変更）は弾く
        if (MG_Mediator.StateCtrl.IsMainGameReset()) return;
        m_IsBeforeStageChange = true;
        StartCoroutine(ProcGameNext());
    }
    private IEnumerator ProcGameNext()
    {
        // ボタン演出
        StartCoroutine(ShUIUtils.ProcBtnEffect(m_UIList.m_List[(int)UINo.Btn_Next].gameObject));
        // ボタン演出待ち
        yield return new WaitForSeconds(0.4f);

        // アナリティクス設定:(結果画面)ネクストボタン押下時
        AppAnalyticsUtils.SetAnalitics_GameStart_Next();

        // インタースティシャル広告
        yield return StartCoroutine(AppAdUtils.AdUtil_CallInterstitial());

        m_IsBeforeStageChange = false;

        //// ステージの切り替え
        //AppData.m_SelectStageNo.SelectStageNo(AppData.m_SelectStageNo.No + 1);

        MainGame.MG_Mediator.StateCtrl.SetStageChange();
    }
}
