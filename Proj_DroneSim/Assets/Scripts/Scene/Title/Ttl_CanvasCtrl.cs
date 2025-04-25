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
public class Ttl_CanvasCtrl : ShCanvasBaseCtrl, IShVisible
{
    private readonly string SceneName = "Title";

    //================================================
    // [///]  各シーンごとに保持する変数
    //================================================
    // IShVisibleで使用する表示変数
    public bool IsVisible { get; private set; } = false;

    // 表示制御としてリスト追加するものの設定
    private enum UINo
    {
        Canvas,

        Grp_SceneOutsideBG,     // キャンバス外黒幕表示
    }
    protected ShUIList m_UIList = new ShUIList();

    // 自身
    protected Canvas m_ThisCanvas = null;

    //public ShXlsxTest m_TestCtrl = new ShXlsxTest();

    //================================================
    // [///] シーンの初期化
    //================================================
    public override void ProcInitialize()
    {
        // 自身
        m_ThisCanvas = Shs.ShSceneUtils.GetSceneRootObj(SceneName, "AppCanvas").GetComponent<Canvas>();

        // 自身から子供の表示制御の付与取得
        {
            Transform canvasT = m_ThisCanvas.transform;

            // リストへの登録
            m_UIList.Clear();
            {
                m_UIList.Add(canvasT.gameObject.AddComponent<ShUIBaseCtrl>());
                m_UIList.Add(canvasT.Find("Grp_SceneOutsideBG").gameObject.AddComponent<ShUIBaseCtrl>());
            }

            // 初期化
            m_UIList.ProcInitialize();

            // 個別設定(ボタンなど) @TODO 処理を移動/改善する
            {
                Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(
                    m_ThisCanvas.transform.Find("Btn/Btn_Start1").GetComponent<Button>(), OnClick_Start_MainGame);
                Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(
                    m_ThisCanvas.transform.Find("Btn/Btn_Start2").GetComponent<Button>(), OnClick_Start_MainGame);
            }

            // 非表示
            m_UIList.Hide();
        }

        // Resorce Load
        // StartCoroutine("ProcResourcesLoad", true);
    }
    public void TestButtonExec()
    {
        //m_TestCtrl.ExecExcelCreate();
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
    // [///] TODO:実装部分のため他に移したい
    //================================================
    public void OnClick_Start_MainGame()
    {
        // @取り急ぎ
        ShSceneUtils.GetAppCtrlObj(SceneName).GetComponent<Title.Ttl_TitleBaseCtrl>().ProcChangeScene();
    }
}
