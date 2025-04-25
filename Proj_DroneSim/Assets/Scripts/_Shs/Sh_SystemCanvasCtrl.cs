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
public class Sh_SystemCanvasCtrl : ShCanvasBaseCtrl, IShVisible
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

        Grp_SceneOutsideBG,     // キャンバス外黒幕表示
        SystemDialog,           // システムダイアログ
    }
    protected ShUIList m_UIList = new ShUIList();

    // 自身
    protected Canvas m_ThisCanvas = null;

    // 呼び出し
    public ShSysDialogCtrl SystemDialog { get; private set; } = null;

    //================================================
    // [///] シーンの初期化
    //================================================
    public override void ProcInitialize()
    {
        // 自身(DontDestroyOnLoadのYnSysUIから見つける)
        m_ThisCanvas = transform.GetComponent<Canvas>();

        // 自身から子供の表示制御の付与取得
        {
            Transform canvasT = m_ThisCanvas.transform;

            // リストへの登録
            m_UIList.Clear();
            {
                m_UIList.Add(canvasT.gameObject.AddComponent<ShUIBaseCtrl>());
                m_UIList.Add(canvasT.Find("Grp_SceneOutsideBG").gameObject.AddComponent<ShUIBaseCtrl>());
                SystemDialog = canvasT.Find("SystemDialog").gameObject.AddComponent<ShSysDialogCtrl>();
                m_UIList.Add(SystemDialog);
            }

            // 初期化
            m_UIList.ProcInitialize();

            // 個別設定(ボタンなど) @TODO 処理を移動/改善する
            {
            }

            // 非表示
            //m_UIList.Hide();
        }

        // Resorce Load
        // StartCoroutine("ProcResourcesLoad", true);
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
    // [///] TODO:実装部分のため他に移したい
    //================================================
}
