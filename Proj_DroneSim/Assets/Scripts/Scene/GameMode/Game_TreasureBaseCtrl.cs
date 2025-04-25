using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Yns;
using Shs;
using Game;

namespace MainGame
{
    /// <summary>
    /// 制御スクリプト：MainGame
    /// </summary>
    public class Game_TreasureBaseCtrl : ShSceneBaseCtrl
    {
        //================================================
        // [///] 定義
        //================================================
        // シーン名
        public const string SceneName = "Game_Treasure";
        // メインカメラ名
        public const string MainCameraName = "Main Camera";
        // 環境ライト名
        public const string LightName = "Directional Light";
        // キャンバス名
        public const string CanvasName = "AppCanvas";
        // Root名
        public const string RootName = "Root";

        //================================================
        // [///]  各シーンごとに保持する変数
        //================================================
        // 配置ルート
        private Transform m_SetRoot = null;
        // シーンUICanvas制御


        // シーンのメインカメラ
        private Camera m_Camera = null;
        // ゲーム状態管理
        private MGTreasure_GameStateCtrl m_StateCtrl = null;
        // オブジェクト管理
        private MGTreasure_ObjMediator m_ObjMediator = null;

        // メインゲームキャンバス
        private MGTreasure_CanvasCtrl m_CanvasCtrl = null;

        // add canvas
        private InputCanvasCtrl m_InputCanvasCtrl = null;


        //================================================
        // [///] シーンの初期化
        //================================================
        public override void ProcInitialize()
        {
            // --- カメラ ---- 
            GameObject goCamera = ShSceneUtils.GetSceneRootObj(SceneName, MainCameraName);
            goCamera.SetActive(true);
            m_Camera = goCamera.GetComponent<Camera>();

            // --- Light ---- 
            GameObject goLight = ShSceneUtils.GetSceneRootObj(SceneName, LightName);
            goLight.SetActive(true);

            // --- Root ---- 
            m_SetRoot = ShSceneUtils.GetSceneRootObj(SceneName, RootName).transform;
            m_SetRoot.gameObject.SetActive(true);

            // m_Ope = gameObject.AddComponent<MainGameOpe>();
            // m_CanvasCtrl = gameObject.AddComponent<MainGameCanvasCtrl>();
            // m_CanvasCtrl.SetInfo(SceneName, CanvasName);

            // Resorce Load
            StartCoroutine("ProcResourcesLoad", true);
        }

        //================================================
        // [///] シーンに必要なリソース読み込み
        //================================================
        public override IEnumerator ProcResourcesLoad()
        {
            // このタイミングで追加シーンの読み込みを行う
            yield return StartCoroutine(AddtiveScene());

            // --- Canvas ---- TODO:キャンバス設定タイミング変更
            GameObject canvas = ShSceneUtils.GetSceneRootObj(SceneName, CanvasName);
            m_CanvasCtrl = gameObject.AddComponent<MGTreasure_CanvasCtrl>();

            // 追加キャンバス
            GameObject addCanvas = ShSceneUtils.GetSceneRootObj(SceneName, "AddCanvas");
            m_InputCanvasCtrl = addCanvas.transform.Find("InputCanvas").gameObject.AddComponent<InputCanvasCtrl>();

            // 追加キャンバス
            // WipeCanvasCtrl wipeCanvas = addCanvas.transform.Find("WipeCanvas").gameObject.AddComponent<WipeCanvasCtrl>();

            // MGTreasure_Mediator セットアップ
            {
                // state管理付与
                m_StateCtrl = gameObject.AddComponent<MGTreasure_GameStateCtrl>();
                MGTreasure_Mediator.SetGameStateCtrl(m_StateCtrl);
                // 配置ルート
                MGTreasure_Mediator.SetRootT(m_SetRoot);
                // オブジェクト管理
                m_ObjMediator = new MGTreasure_ObjMediator();
                m_ObjMediator.SetInit();
                MGTreasure_Mediator.SetObjMediator(m_ObjMediator);
                // キャンバス
                MGTreasure_Mediator.SetCanvas(m_CanvasCtrl);
                // カメラ
                GameObject goCamera = ShSceneUtils.GetSceneRootObj(SceneName, "CamRoot");
                MGTreasure_Mediator.SetCamera(goCamera.transform.Find("ObjCamera").GetComponent<Camera>());
                GameObject uiCamera = ShSceneUtils.GetSceneRootObj(SceneName, "Main Camera");
                MGTreasure_Mediator.SetUICamera(uiCamera.GetComponent<Camera>());

                // InputActionManagerTest
                ShInputManager.I.setup();
            }

            // 初期化
            {
                m_CanvasCtrl.ProcInitialize();
                m_InputCanvasCtrl.ProcInitialize();
                //wipeCanvas.ProcInitialize();

                ShInputManager.I.SetInputCanvasCtrl(m_InputCanvasCtrl);
                //ShInputManager.I.SetWipeCanvasCtrl(wipeCanvas);
                ShInputManager.I.SetTimerCtrl(m_CanvasCtrl.GetTimerCtrl());
            }

            // 初期ステージの読み込み
            {
                m_ObjMediator.ProcLoadStage();
            }

            // オブジェクトの生成
            m_ObjMediator.CreateGameObject();

            // ゲームオブジェクト/リソースの準備が整ってからの呼び出し
            {
                // デバッグ設定
                AddDebug();
            }

            // 読み込み完了
            m_IsCompleteSceneSetup = true;
        }

        //================================================
        // [///] シーンの処理開始
        //================================================
        public override void ProcSceneProcStart()
        {
            m_StateCtrl.SetStartGameState();

            ShInputManager.I.ProcSceneProcStart();
        }

        //================================================
        // [///] シーンの変更
        //================================================
        public override void ProcChangeScene()
        {
            // 遷移重複チェック
            if (!ShTransitionSys.IsTransitionEnd()) return;

            // ここでログインシーンへの遷移設定
            ShTransitionSys.SetYnsysLoadNextScene("Login", 0.25f);
            ShTransitionSys.SetLoadScene(true);
            ShTransitionSys.ChangeCoverColorBlack();

            // ↓SetSceneEndStateでフェード設定している

            StartCoroutine(OnProc_ChangeScene_MainGame());
        }
        private IEnumerator OnProc_ChangeScene_MainGame()
        {
            // シーン破棄処理呼び出し
            m_StateCtrl.SetSceneEndState();
            while (!m_StateCtrl.IsStage_End())
            {
                yield return null;
            }

            // シーン変更処理呼び出し
            //ChangeScene_Title.SetChangeScene();
            yield break;
        }

        public void ProcChangeScene_Replay()
        {
            // シーン破棄処理呼び出し
            m_StateCtrl.SetSceneEndState2();

            ShTransitionSys.SetYnsysLoadNextScene("MainGame", 0.25f);
            ShTransitionSys.SetLoadScene(false);
            ShTransitionSys.ChangeCoverColorBlack();

            ShTransitionSys.SetTransition(0.5f, this, "OnProc_ChangeScene_Replay", false, false, false);
        }
        private IEnumerator OnProc_ChangeScene_Replay()
        {
            // シーン変更処理呼び出し
            ChangeScene_Treasure.SetChangeScene();
            yield break;
        }

        //================================================
        // [///] 個別処理
        //================================================
        // シーン追加ロードする場合
        private IEnumerator AddtiveScene()
        {
#if DEBUG_ENABLE
            // {
            //     // デバッグシーンの追加
            //     string addSceneName = "DebugSet";
            //     yield return ShSceneUtils.LoadAddtiveScene(addSceneName);
            //     // MainCameraがある場合削除
            //     ShSceneUtils.GetSceneRootObj(addSceneName, "Main Camera");
            //     ShSceneUtils.GetSceneRootObj(addSceneName, "AppCanvas").SetActive(false);
            // }
#endif

            // // タイトルを追加
            // {
            //     string addSceneName = "Title_Overray";
            //     yield return ShSceneUtils.LoadAddtiveScene(addSceneName);
            //     // カメラの破棄
            //     ShSceneUtils.AddSceneCameraDestroy(addSceneName, "Main Camera");
            //     // キャンバスの付与
            //     GameObject canvasGo = ShSceneUtils.GetSceneRootObj(addSceneName, "AppCanvas");
            //     Ttl_CanvasCtrl canvasCtrl = canvasGo.AddComponent<Ttl_CanvasCtrl>();
            //     canvasCtrl.ProcInitialize();
            // 
            //     // MGTreasure_Mediatorの設定
            //     MGTreasure_Mediator.SetTitleCanvas(canvasCtrl);
            // }

            // // リザルトを追加
            // {
            //     string addSceneName = "Result_Overray";
            //     yield return ShSceneUtils.LoadAddtiveScene(addSceneName);
            //     m_Result = Yns.YnSys.m_AppCommon.GetRootCanvas("Result_Overray", "AppControl").
            //         GetComponent<Result>();
            //     m_Result.SetReturnScene(this);
            // }

            yield break;
        }

        // デバッグ処理 ⇒ デバッグCanvasCtrlとして新規スクリプトかする
        private void AddDebug()
        {
// 
// #if DEBUG_ENABLE
//             // デバッグ処理
//             // debugボタン用処理追加
//             MGD_DebugCtrl ctrl = gameObject.AddComponent<MGD_DebugCtrl>();
//             ctrl.SetInitialize();
// #else
//             // デバッグ処理の非表示
//             Canvas appCanvas =
//                 Yns.YnSys.m_AppCommon.GetRootCanvas("MainGame", "AppCanvas", false).GetComponent<Canvas>();
// 
//             // デバッグオブジェクト
//             appCanvas.transform.Find("Debug").gameObject.SetActive(false);
// 
//             // デバッグメニュー一括
//             GameObject debugRoot = Yns.YnSys.m_AppCommon.GetRootCanvas("MainGame", "DebugRoot", false);
//             debugRoot.SetActive(false);
// #endif
        }
    }
}