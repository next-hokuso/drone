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
public class MG_NetworkCheckCanvasCtrl : MonoBehaviour
{
    //================================================
    // [///]  各シーンごとに保持する変数
    //================================================
    // 表示制御としてリスト追加するものの設定
    private enum UINo
    {
    }
    protected ShUIList m_UIList = new ShUIList();

    // 他キャンバス
    private Canvas m_NetworkCheckCanvas = null;

    // ネットワークチェック
    private bool m_IsNetworkCheck = false;

    //================================================
    // [///] シーンの初期化
    //================================================
    public void SetUp()
    {
        // 自身から子供の表示制御の付与取得
        {
            GameObject thisGo = ShSceneUtils.GetSceneRootObj("MainGame", "NetworkCheckCanvas");

            m_NetworkCheckCanvas = thisGo.GetComponent<Canvas>();
            m_NetworkCheckCanvas.gameObject.SetActive(false);

            // リストへの登録
            m_UIList.Clear();
            {
            }

            // 初期化
            m_UIList.ProcInitialize();

            // 個別設定(ボタンなど) @TODO 処理を移動/改善する
            {
                // backボタン
                m_NetworkCheckCanvas.transform.Find("Btn_Back").GetComponent<Button>().onClick.AddListener(OnClick_NetworkCheckBack);

                // 設定
            }

            // 非表示
            m_UIList.Hide();
        }

        // Resorce Load
        //StartCoroutine("ProcResourcesLoad", true);
    }

    //================================================
    // [///] TODO:実装部分のため他に移したい2
    //================================================
    //================================================
    // [///] public method
    //================================================
    // オンライン警告画面 表示
    public void SetNetworkCheckEnable(bool enable)
    {
        m_NetworkCheckCanvas.gameObject.SetActive(enable);
    }

    // オンライン警告画面チェック
    public bool CheckNetwork()
    {
        if (!AppCommon.IsInternetReachable())
        {
            m_IsNetworkCheck = true;
            SetNetworkCheckEnable(true);
            return true;
        }
        else
        {
            m_IsNetworkCheck = false;
            return false;
        }
    }
    public bool IsCheckNetworkWidow()
    {
        return m_NetworkCheckCanvas.gameObject.activeSelf;
    }

    //================================================
    // [///] ボタン機能
    //================================================
    // オンライン警告画面 バック
    public void OnClick_NetworkCheckBack()
    {
        SetNetworkCheckEnable(false);
    }
}
