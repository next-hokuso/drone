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
public class MG_SettingCtrl : MonoBehaviour
{
    private readonly string URL_PricacyPolicy = "https://hypercasual.orso.jp/privacypolicy.html";

    //================================================
    // [///]  各シーンごとに保持する変数
    //================================================
    // 表示制御としてリスト追加するものの設定
    private enum UINo
    {
    }
    protected ShUIList m_UIList = new ShUIList();

    // back状態
    private bool m_IsMenuSetting = false;

    // 他キャンバス
    private Canvas m_SettingCanvas = null;
    private Canvas m_LicenseCanvas = null;

    //================================================
    // [///] シーンの初期化
    //================================================
    public void SetUp()
    {
        // 自身から子供の表示制御の付与取得
        {
            GameObject settingGrp = ShSceneUtils.GetSceneRootObj("MainGame", "Grp_Settings");

            m_SettingCanvas = settingGrp.transform.Find("SettingCanvas").GetComponent<Canvas>();
            m_LicenseCanvas = settingGrp.transform.Find("LicenseCanvas").GetComponent<Canvas>();
            m_SettingCanvas.gameObject.SetActive(false);
            m_LicenseCanvas.gameObject.SetActive(false);

            // リストへの登録
            m_UIList.Clear();
            {
            }

            // 初期化
            m_UIList.ProcInitialize();

            // 個別設定(ボタンなど) @TODO 処理を移動/改善する
            {
                // m_SettingCanvas.transform.Find("Btn_").GetComponent<Button>()
                //     .onClick.AddListener(OnClick_Setting);
                // backボタン
                m_SettingCanvas.transform.Find("Btn_Back").GetComponent<Button>().onClick.AddListener(OnClick_Back);
                m_LicenseCanvas.transform.Find("Btn_Back").GetComponent<Button>().onClick.AddListener(OnClick_Back);
                // セッティング画面
                m_SettingCanvas.transform.Find("Btn_License").GetComponent<Button>().onClick.AddListener(OnClick_License);
                m_SettingCanvas.transform.Find("Btn_Privacy").GetComponent<Button>().onClick.AddListener(OnClick_PrivacyPolicy);

                // license設定
                gameObject.AddComponent<LicenseCtrl>().SetScrollViewInfo(m_LicenseCanvas.transform.Find("ScrollView"));

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
    // [///] ボタン機能
    //================================================
    // setting
    public void OnClick_Setting()
    {
        // setting表示
        m_SettingCanvas.gameObject.SetActive(true);
        m_IsMenuSetting = true;

        // SE:Play
        //GameStaticInfo.SetPlaySe(AudioId.Play);

        // ライセンス表示
        m_LicenseCanvas.gameObject.SetActive(false);

        // 覆っているため不要
        //m_AppCanvas.gameObject.SetActive(false);
    }
    // back
    public void OnClick_Back()
    {
        if (m_IsMenuSetting)
        {
            // タイトルへ戻る
            m_IsMenuSetting = false;
            // setting表示
            m_SettingCanvas.gameObject.SetActive(false);

            // SE:Play
            //GameStaticInfo.SetPlaySe(AudioId.Play);

            // mainに戻す
            //m_AppCanvas.gameObject.SetActive(true);
            MG_Mediator.MainCanvas.OnClick_Return_Settings();
        }
        else
        {
            OnClick_Setting();
        }
    }
    // ライセンス
    public void OnClick_License()
    {
        m_IsMenuSetting = false;

        // SE:Play
        //GameStaticInfo.SetPlaySe(AudioId.Play);

        // ライセンス表示
        m_LicenseCanvas.gameObject.SetActive(true);
        // setting表示 タップ無効でそのまま表示
        // m_SettingCanvas.gameObject.SetActive(false);
    }
    // プライバシーポリシー
    public void OnClick_PrivacyPolicy()
    {
        // SE:Play
        //GameStaticInfo.SetPlaySe(AudioId.Play);

        Application.OpenURL(URL_PricacyPolicy);
    }
}
