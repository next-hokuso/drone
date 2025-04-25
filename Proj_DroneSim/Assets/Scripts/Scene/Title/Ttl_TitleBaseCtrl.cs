using System.Collections;
using UnityEngine;
using Shs;

namespace Title
{
    /// <summary>
    /// 制御スクリプト：Title
    /// </summary>
    public class Ttl_TitleBaseCtrl : ShSceneBaseCtrl
    {
        //================================================
        // [///] 定義
        //================================================
        // シーン名
        public const string SceneName = "Title";
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
        // ゲーム状態管理
        private Ttl_StateCtrl m_StateCtrl = null;
        // // オブジェクト管理
        // private MG_ObjMediator m_ObjMediator = null;
        // メインゲームキャンバス
        private Ttl_CanvasCtrl m_CanvasCtrl = null;

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
            // このタイミングでゲームデータロード
            Yns.YnSys.GetAppCommon().GameDataInitialize();

            // このタイミングで追加シーンの読み込みを行う
            yield return StartCoroutine(AddtiveScene());

            // --- Canvas ---- TODO:キャンバス設定タイミング変更
            GameObject canvas = ShSceneUtils.GetSceneRootObj(SceneName, CanvasName);
            m_CanvasCtrl = gameObject.AddComponent<Ttl_CanvasCtrl>();
            m_CanvasCtrl.ProcInitialize();

            // state管理付与
            m_StateCtrl = gameObject.AddComponent<Ttl_StateCtrl>();

            // 〇〇_Mediator セットアップ
            {
            }

            // オブジェクト生成の読み込み
            {
                // 読み込み
                // 生成
            }

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
            m_StateCtrl.SetInitState();

            // Canvasを開く
            m_CanvasCtrl.GetComponent<IShVisible>().Show();
        }

        //================================================
        // [///] シーンの変更
        //================================================
        // MainGameへの変更
        public override void ProcChangeScene()
        {
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
            ChangeScene_MainGame.SetChangeScene();
            yield break;
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