using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using System.Linq;
using Shs;
using Yns;
using System;

namespace MainGame
{
    public class MGEndress_ObjMediator
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
        public Drone_MGEnddressCtrl DroneCtrl { private set; get; } = null;

        // ステージパーツPrefab
        private GameObject m_GroundPrefab = null;
        private GameObject m_GroundPrefab2 = null;
        private GameObject m_GroundPrefab3 = null;
        private List<GameObject> m_GOList = new List<GameObject>();
        private int m_GoDisableIdx = 0;


        //================================================
        // [///] 初期設定
        //================================================
        public void SetInit()
        {
            m_SetRoot = MGEndress_Mediator.RootT.Find("SetRoot");

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
            DroneCtrl = MGEndress_Mediator.RootT.Find("Drone").gameObject.AddComponent<Drone_MGEnddressCtrl>();
            DroneCtrl.SetStartInfo();

            // カメラターゲット設定
            MainCameraManager.I.CamCtrl.SetTarget(DroneCtrl.gameObject);

            MainCameraManager.I.CamCtrl.SetLookAtCamera_EndressMode(DroneCtrl.gameObject);


            // ステージ
            {
                m_GroundPrefab = m_SetRoot.transform.Find("Ground_Parts").gameObject;
                m_GroundPrefab.SetActive(false);
                m_GroundPrefab2 = m_SetRoot.transform.Find("Ground_Parts2").gameObject;
                m_GroundPrefab2.SetActive(false);
                m_GroundPrefab3 = m_SetRoot.transform.Find("Ground_Parts3").gameObject;
                m_GroundPrefab3.SetActive(false);

                {
                    foreach(CmnDistanceChecker ctrl in m_GroundPrefab.GetComponentsInChildren<CmnDistanceChecker>())
                    {
                        // 表示対処
                        ctrl.SetTargetObj(MGEndress_Mediator.ObjMediator.DroneCtrl.gameObject);
                        ctrl.SetTargetDistance(300.0f);
                    }
                    foreach (CmnDistanceChecker ctrl in m_GroundPrefab2.GetComponentsInChildren<CmnDistanceChecker>())
                    {
                        // 表示対処
                        ctrl.SetTargetObj(MGEndress_Mediator.ObjMediator.DroneCtrl.gameObject);
                        ctrl.SetTargetDistance(300.0f);
                    }
                    foreach (CmnDistanceChecker ctrl in m_GroundPrefab3.GetComponentsInChildren<CmnDistanceChecker>())
                    {
                        // 表示対処
                        ctrl.SetTargetObj(MGEndress_Mediator.ObjMediator.DroneCtrl.gameObject);
                        ctrl.SetTargetDistance(300.0f);
                    }

                }

                for (int i = 0; i < 20; ++i) {
                    int rand = UnityEngine.Random.Range(0, 6);
                    GameObject prefab = null;
                    switch (rand)
                    {
                        case 0:
                        case 1:
                            prefab = m_GroundPrefab; break;
                        case 2:
                        case 3:
                            prefab = m_GroundPrefab2; break;
                        case 4:
                        case 5:
                            prefab = m_GroundPrefab3; break;
                    }

                    GameObject go = GameObject.Instantiate(prefab);
                    if (go)
                    {
                        go.transform.parent = m_SetRoot.transform;
                        go.transform.position = Vector3.forward * (i * 100.0f);
                        go.transform.localEulerAngles = rand %2 == 0 ? Vector3.up * 180.0f : Vector3.zero;
                        go.transform.localScale = Vector3.one;
                        go.SetActive(true);

                        m_GOList.Add(go);
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

        bool isEnd = false;
        //================================================
        // [///] アップデート処理
        //================================================
        public void ProcUpdate()
        {
            if (isEnd) return;

            {
                // ドローン
                DroneCtrl.ProcInput();

                // YnSys.SetDbgText("飛距離Z : " + (DroneCtrl.transform.position.z * 0.1f).ToString("f1") + "m");
                MGEndress_Mediator.MainCanvas.SetZDistText((DroneCtrl.transform.position.z * 0.1f).ToString("f1") + "m");
            }

            // ステージ
            if(m_GoDisableIdx < 20)
            {
                if (m_GOList[m_GoDisableIdx].transform.position.z < DroneCtrl.transform.position.z - 60.0f)
                {
                    GameObject.Destroy(m_GOList[m_GoDisableIdx]);
                    m_GoDisableIdx++;
                }
            }

            //
            {
                if(MGEndress_Mediator.MainCanvas.GetTimerCtrl().GetTime() > 30.0f)
                {
                    // タイマーを停止
                    MainGame.MGEndress_Mediator.MainCanvas.GetTimerCtrl().ProcGameEnd();

                    // リザルト呼び出し
                    MainGame.MGEndress_Mediator.MainCanvas.SetResult(0, true);

                    // 
                    MGEndress_Mediator.StateCtrl.SetGameClear();

                    DroneCtrl.SetGameOver();

                    isEnd = true;
                }
            }
        }
    }
}