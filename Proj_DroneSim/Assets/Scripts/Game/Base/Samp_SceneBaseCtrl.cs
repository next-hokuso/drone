using System.Collections;
using UnityEngine;
using Shs;

namespace SampleScene
{
    /// <summary>
    /// 制御スクリプト：
    /// </summary>
    public class Samp_SceneBaseCtrl : ShSceneBaseCtrl
    {
        //================================================
        // [///] 定義
        //================================================
        // シーン名
        public const string SceneName = "";
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
        // シーンのメインカメラ
        private Camera m_Camera = null;
        // // ゲーム状態管理
        // private MG_GameStateCtrl m_GameStateCtrl = null;
        // // オブジェクト管理
        // private MG_ObjMediator m_ObjMediator = null;
        // メインゲームキャンバス
        // private MG_CanvasCtrl m_CanvasCtrl = null;

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
            // GameObject canvas = ShSceneUtils.GetSceneRootObj(SceneName, CanvasName);
            // m_CanvasCtrl = gameObject.AddComponent<MG_CanvasCtrl>();
            // m_CanvasCtrl.ProcInitialize();

            // state管理付与
            // m_GameStateCtrl = gameObject.AddComponent<MG_GameStateCtrl>();

            // 〇〇_Mediator セットアップ
            {
                // MG_Mediator.SetGameStateCtrl(m_GameStateCtrl);
                // // 配置ルート
                // MG_Mediator.SetRootT(m_SetRoot);
                // // オブジェクト管理
                // m_ObjMediator = new MG_ObjMediator();
                // m_ObjMediator.SetInit();
                // MG_Mediator.SetObjMediator(m_ObjMediator);
                // // キャンバス
                // MG_Mediator.SetCanvas(m_CanvasCtrl);
                // // カメラ
                // GameObject goCamera = ShSceneUtils.GetSceneRootObj(SceneName, "ObjCamera");
                // MG_Mediator.SetCamera(goCamera.GetComponent<Camera>());
                // GameObject uiCamera = ShSceneUtils.GetSceneRootObj(SceneName, "Main Camera");
                // MG_Mediator.SetUICamera(uiCamera.GetComponent<Camera>());
                // // Audio
                // MG_Mediator.SetAudio(Yns.YnSys.GetGoYnSys().GetComponent<YnsSimpleAudio>());
            }

            // 初期ステージの読み込み
            {
                // m_ObjMediator.ProcLoadStage();
            }

            // オブジェクトの生成
            // m_ObjMediator.CreateGameObject();

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
            // m_GameStateCtrl.SetStartGameState();
        }

        //================================================
        // [///] シーンの変更
        //================================================
        public override void ProcChangeScene()
        {
        }

        //================================================
        // [///] 個別処理
        //================================================
        // シーン追加ロードする場合
        private IEnumerator AddtiveScene()
        {
            yield break;
        }

        // デバッグ処理 ⇒ デバッグCanvasCtrlとして新規スクリプトかする
        private void AddDebug()
        {
        }
    }
}