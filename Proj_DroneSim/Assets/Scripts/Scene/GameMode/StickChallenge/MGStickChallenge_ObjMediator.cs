using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using System.Linq;
using Shs;
using Yns;
using Unity.VisualScripting;
using System;

namespace MainGame
{
    public class MGStickChallenge_ObjMediator
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
        public Drone_MGStickChallengeCtrl DroneCtrl { private set; get; } = null;

        // ステージパーツPrefab
        private GameObject m_GroundPrefab = null;
        private List<GameObject> m_GOList = new List<GameObject>();
        private int m_GoDisableIdx = 0;


        //================================================
        // [///] 初期設定
        //================================================
        public void SetInit()
        {
            m_SetRoot = MGStickChallenge_Mediator.RootT.Find("SetRoot");

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
            DroneCtrl = MGStickChallenge_Mediator.RootT.Find("Drone").gameObject.AddComponent<Drone_MGStickChallengeCtrl>();
            DroneCtrl.SetStartInfo();


            Vector3 pos = DroneCtrl.transform.position;
            //pos.z += -50.0f;
            DroneCtrl.transform.position = pos;

            // カメラターゲット設定
            MainCameraManager.I.CamCtrl.SetTarget(DroneCtrl.gameObject);

            MainCameraManager.I.CamCtrl.SetLookAtCamera_TreasureMode(DroneCtrl.gameObject);


            {
               // m_SetRoot.transform.Find("Ground_Parts/StickChallenge").AddComponent<CmnEndressRotation>().SetRotation(Vector3.up * 90.0f);

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


            GameObject pre = Resources.Load("Prefab/Object/TestTree") as GameObject;
            {
                StageSet data = GD_StageSet.GetData(0);
                float posx = -data.m_MapSizeX * 4.0f / 2.0f;
                float posz = data.m_MapSizeY * 4.0f / 2.0f;

                for (int y = 0; y < data.m_MapSizeY; ++y) {
                    for (int x = 0; x < data.m_MapSizeX; ++x) {
                        GameObject go = GameObject.Instantiate(pre);
                        if (go)
                        {
                            go.transform.parent = m_SetRoot.transform;

                            Vector3 posB = Vector3.zero;
                            posB.x = posx + (x * 4.0f);
                            posB.z = posz + (y * -4.0f) ;
                            go.transform.localPosition = posB;

                            Vector3 posA = Vector3.zero;
                            float posxRand = UnityEngine.Random.Range(-10.0f, 11.0f);
                            posA.x =  posxRand * 0.1f;
                            float poszRand = UnityEngine.Random.Range(-10.0f, 11.0f);
                            posA.z =  poszRand * 0.1f;
                            go.transform.Find("tee_01").localPosition = posA;

                            float sizeRand = UnityEngine.Random.Range(15.0f, 31.0f) * 0.1f;
                            go.transform.Find("tee_01").localScale = Vector3.one * (100.0f * sizeRand); ;

                            Vector3 angle = Vector3.zero;
                            angle.x = UnityEngine.Random.Range(-80.0f, 81.0f) * 0.1f;
                            angle.z = UnityEngine.Random.Range(-80.0f, 81.0f) * 0.1f;
                            angle.y = UnityEngine.Random.Range(0.0f, 360.0f);
                            go.transform.Find("tee_01").localEulerAngles = angle;

                        }
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
                if (MGStickChallenge_Mediator.MainCanvas.GetTimerCtrl().GetTime() > 60.0f)
                {
                    // タイマーを停止
                    //MainGame.MGStickChallenge_Mediator.MainCanvas.GetTimerCtrl().ProcGameEnd();
                    ShInputManager.I.GetTimerCtrl().ProcGameEnd();

                    // リザルト呼び出し
                    MainGame.MGStickChallenge_Mediator.MainCanvas.SetResult(0, true);

                    // 
                    MGStickChallenge_Mediator.StateCtrl.Set6GameClear();

                }
            }

        }
    }
}