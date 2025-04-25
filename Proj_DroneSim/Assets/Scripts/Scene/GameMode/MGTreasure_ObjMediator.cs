using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using System.Linq;
using Shs;
using Yns;
using Unity.VisualScripting;

namespace MainGame
{
    public class MGTreasure_ObjMediator
    {
        //================================================
        // [///] 定義 
        //================================================
        private Transform m_SetRoot = null;

        // ステージ
        private GameObject m_StageGo = null;

        // light
        private Light m_DirectionalLight = null;

        // ドローン操作
        public Drone_MGTreasureCtrl DroneCtrl { private set; get; } = null;

        // ステージパーツPrefab
        private GameObject m_GroundPrefab = null;
        private List<GameObject> m_GOList = new List<GameObject>();
        private int m_GoDisableIdx = 0;


        //================================================
        // [///] 初期設定
        //================================================
        public void SetInit()
        {
            m_SetRoot = MGTreasure_Mediator.RootT.Find("SetRoot");

            // --- Light ---- 
            m_DirectionalLight = Shs.ShSceneUtils.GetSceneRootObj(AppCommon.CurrentSceneName, "Directional Light").GetComponent<Light>();

            // カメラ
            GameObject goCamera = Shs.ShSceneUtils.GetSceneRootObj(AppCommon.CurrentSceneName, "CamRoot");
            MainCameraManager.I.SetCamera(goCamera.transform.Find("ObjCamera").GetComponent<Camera>());
        }

        //================================================
        // [///] ステージの読み込み
        //================================================
        public void ProcLoadStage()
        {
            // ステージ設定
            if (AppData.m_PlayMode == AppData.PlayMode.Mission)
            {
            }
            // フリーモード時の設定
            else if(AppData.m_PlayMode == AppData.PlayMode.Free)
            {
            }
            // リプレイ時
            else if(AppData.m_PlayMode == AppData.PlayMode.Replay)
            {
            }
        }

        //================================================
        // [///] state変更時の処理関連
        //================================================
        // スタート時
        public void SetStart()
        {
            m_SetRoot.gameObject.SetActive(true);

            // drone
            DroneCtrl.SetStart();
        }
        // スタート演出後のステージ処理開始
        public void SetStart_ProcOn()
        {
            DroneCtrl.SetStart_ProcOn();
        }
        // ゲームオーバー処理
        public void SetGameOver()
        {
        }
        // リセット
        public void SetReset()
        {

            // drone
            DroneCtrl.ProcReset();
        }
        // リトライ処理
        public void SetRetry()
        {
            SetReset();

            SetStart();
            SetStart_ProcOn();
        }
        // 次へ(NEXT)処理
        public void SetNext()
        {
            SetReset();
            SetStart();
        }
        // 削除対応(ステージ選択に戻る場合の削除処理
        public void SetDestroy()
        {
            SetReset();
        }
        // ステージ切り替え(ステージの削除)
        public void SetStageChage()
        {
            GameObject.Destroy(m_StageGo);
            m_StageGo = null;

            SetReset();
        }

        //================================================
        // [///] create関連
        //================================================
        // 要素が多くなった場合別クラスに移植する
        public void CreateGameObject()
        {
            // ドローン対応
            DroneCtrl = MGTreasure_Mediator.RootT.Find("Drone").gameObject.AddComponent<Drone_MGTreasureCtrl>();
            DroneCtrl.SetStartInfo();


            Vector3 pos = DroneCtrl.transform.position;
            //pos.z += -50.0f;
            DroneCtrl.transform.position = pos;

            // カメラターゲット設定
            MainCameraManager.I.CamCtrl.SetTarget(DroneCtrl.gameObject);

            MainCameraManager.I.CamCtrl.SetLookAtCamera_TreasureMode(DroneCtrl.gameObject);


            {
                m_SetRoot.transform.Find("Ground_Parts/Treasure").AddComponent<CmnEndressRotation>().SetRotation(Vector3.up * 90.0f);

                foreach (CmnDistanceChecker ctrl in m_SetRoot.transform.Find("Ground_Parts").GetComponentsInChildren<CmnDistanceChecker>())
                {
                    // // 表示対処
                    // ctrl.SetTargetObj(DroneCtrl.gameObject);
                    // ctrl.SetTargetDistance(300.0f);
                }
            }

            {
                foreach (CmnEndressRotation go in m_SetRoot.transform.Find("check").GetComponentsInChildren<CmnEndressRotation>())
                {
                    go.SetRotation(Vector3.up * 90.0f);
                }
            }

            foreach (Transform t in m_SetRoot.GetComponentsInChildren<Transform>())
            {
                if (t.name.Contains("Tree"))
                {
                    int rand = UnityEngine.Random.Range(15, 31);
                    t.localScale = Vector3.one * (float)rand * 0.1f;
                }
            }
        }

        //================================================
        // [///] 
        //================================================
        // ドローンログ設定
        public void SaveDroneInputLog()
        {
            //DroneCtrl.SaveLocalReplayData();
        }
        // ドローンバウンド設定
        public void DbgDroneBouncinessSetting()
        {
            DroneCtrl.ProcChange_PhysicsMaterialBounciness();
        }

        //================================================
        // [///] アップデート処理
        //================================================
        public void ProcUpdate()
        {
            {
                // ドローン
                DroneCtrl.ProcInput();

                //YnSys.SetDbgText("飛距離Z : " + (DroneCtrl.transform.position.z * 0.1f).ToString("f1") + "m");
            }

            //
            {
                if (MGTreasure_Mediator.MainCanvas.GetTimerCtrl().GetTime() > 60.0f)
                {
                    // タイマーを停止
                    //MainGame.MGTreasure_Mediator.MainCanvas.GetTimerCtrl().ProcGameEnd();
                    ShInputManager.I.GetTimerCtrl().ProcGameEnd();

                    // リザルト呼び出し
                    MainGame.MGTreasure_Mediator.MainCanvas.SetResult(0, true);

                    // 
                    MGTreasure_Mediator.StateCtrl.Set6GameClear();

                }
            }

        }
    }
}