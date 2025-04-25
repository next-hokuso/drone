using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using Shs;

namespace MainGame
{
    /// <summary>
    /// メインゲームのデバッグ
    /// </summary>
    public class MGD_DebugCtrl : MonoBehaviour
    {
        //================================================
        // [///]
        //================================================
        private GameObject m_DebugBtn = null;

        // タップチェック
        private bool m_IsDoubleTapCheck = false;
        private float m_DoubleTapCheckTimer = 0.0f;
        private bool m_IsTripleTapCheck = false;
        private float m_TripleTapCheckTimer = 0.0f;

        // デバッグルート
        private GameObject m_DebugRoot = null;
        // デバッグキャンバス
        private GameObject m_DebugCanvas = null;
        // デバッグ操作
        private DebugMainGameSettings m_DbgSettings = null;


        // デバッグ表示
        private GameObject m_DbgAdBanner = null;
        private GameObject m_DbgInstCheck = null;

        //================================================
        // [///]
        //================================================
        void Start()
        {
        }
        void Update()
        {
            // ダブルタップチェック
            if (m_IsDoubleTapCheck)
            {
                m_DoubleTapCheckTimer += Time.deltaTime;
                if (m_DoubleTapCheckTimer > 0.3f)
                {
                    m_DoubleTapCheckTimer = 0.0f;
                    m_IsDoubleTapCheck = false;
                }
            }
            // トリプルタップチェック
            if (m_IsTripleTapCheck)
            {
                m_TripleTapCheckTimer += Time.deltaTime;
                if (m_TripleTapCheckTimer > 0.3f)
                {
                    m_TripleTapCheckTimer = 0.0f;
                    m_IsTripleTapCheck = false;
                }
            }
        }

        public void SetInitialize()
        {
            Canvas appCanvas = ShSceneUtils.GetSceneRootObj("MainGame", "AppCanvas").GetComponent<Canvas>();

            // デバッグオブジェクト
            appCanvas.transform.Find("Debug").gameObject.SetActive(true);

            // デバッグメニューボタン
            m_DebugBtn = appCanvas.transform.Find("Debug/Btn_DebugBtn").gameObject;
            m_DebugBtn.GetComponent<Button>().onClick.AddListener(OnClick_Debug);

            // 広告バナー
            m_DbgAdBanner = appCanvas.transform.Find("Debug/Panel_Banner").gameObject;
            m_DbgAdBanner.SetActive(true);//AppDebugData.m_IsDbgAdUIEnable);             // @debug 強制True中

            // 広告インタースティシャル広告表示チェック
            m_DbgInstCheck = appCanvas.transform.Find("Debug/Panel_AdView").gameObject;
            m_DbgInstCheck.SetActive(false);

            // デバッグメニュー一括
            m_DebugRoot = ShSceneUtils.GetSceneRootObj("MainGame", "DebugRoot");
            m_DebugCanvas = m_DebugRoot.transform.Find("DebugCanvas").gameObject;
            m_DbgSettings = m_DebugCanvas.AddComponent<DebugMainGameSettings>();
            m_DbgSettings.SetInfo(this);
            m_DbgSettings.gameObject.SetActive(false);

            m_DebugBtn.SetActive(true);

            // バナー表示設定
            Dbg_ChangeAdBannerDisp();

            // 自信を設定
            MG_Mediator.SetMainGameDebug(this);
        }

        //================================================
        // [///] ad debug
        //================================================
        // バナー表示変更
        public void Dbg_ChangeAdBannerDisp()
        {
            m_DbgAdBanner.SetActive(AppDebugData.m_IsDbgAdUIEnable);
        }
        // インタースティシャル広告表示チェック表示変更
        public void Dbg_ChangeAdIntersitialDisp(bool isDisp)
        {
            m_DbgInstCheck.SetActive(isDisp);
        }
        // インタースティシャル広告表示チェック呼び出し
        public void Dbg_SetIntersitialCheck()
        {
            StartCoroutine(SetDbgInterstitialCheck());
        }
        private IEnumerator SetDbgInterstitialCheck()
        {
            if (AppDebugData.m_IsDbgAdUIEnable)
            {
                // デバッグ表示設定
                Dbg_ChangeAdIntersitialDisp(true);
                yield return new WaitForSeconds(3.0f);

                Dbg_ChangeAdIntersitialDisp(false);
            }
        }

        //================================================
        // [///] click
        //================================================
        // 
        public void OnClick_Debug()
        {
            // トリプルタップ判定
            if (m_IsTripleTapCheck)
            {
                if (m_TripleTapCheckTimer <= 0.3f)
                {
                    m_IsTripleTapCheck = false;
                    m_TripleTapCheckTimer = 0.0f;

                    // デバッグを開く
                    OpenDebug();
                }
            }
            // ダブルタップ判定
            else if (m_IsDoubleTapCheck)
            {
                if (m_DoubleTapCheckTimer <= 0.3f)
                {
                    m_IsDoubleTapCheck = false;
                    m_DoubleTapCheckTimer = 0.0f;
                    m_IsTripleTapCheck = true;
                    return;
                }
            }
            else
            {
                m_IsDoubleTapCheck = true;
                return;
            }
        }
        // デバッグを開く
        private void OpenDebug()
        {
            m_DebugCanvas.SetActive(true);
            m_DebugRoot.SetActive(true);
        }

        //================================================
        // [///] 外部click
        //================================================
        // 戻す
        public void OnClick_Return()
        {
            // デバッグを閉じる
            m_DebugCanvas.SetActive(false);
        }
    }
}