using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using System.Linq;

namespace MainGame
{
    public class MG_ObjMediator
    {
        //================================================
        // [///] 定義 
        //================================================
        // データ保管クラス
        private MG_ObjRepository m_ObjRepository = null;

        private Transform m_SetRoot = null;

        // ステージ
        private GameObject m_StageGo = null;

        // light
        private Light m_DirectionalLight = null;

        // ドローン操作
        public DroneCtrl DroneCtrl { private set; get; } = null;

        //================================================
        // [///] 初期設定
        //================================================
        public void SetInit()
        {
            m_ObjRepository = new MG_ObjRepository();
            m_ObjRepository.SetInit();

            m_SetRoot = MG_Mediator.RootT.Find("SetRoot");

            // --- Light ---- 
            m_DirectionalLight = Shs.ShSceneUtils.GetSceneRootObj("MainGame", "Directional Light").GetComponent<Light>();

            // カメラ
            GameObject goCamera = Shs.ShSceneUtils.GetSceneRootObj("MainGame", "CamRoot");
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
                // モードごとのミッションチェックトリガーオブジェクトの設定
                if (AppData.m_MissionId == AppData.MissionID.SquareFly)
                {
                    m_SetRoot.transform.Find("MissionCheck/SquareFlyMode").gameObject.SetActive(true);
                    m_SetRoot.transform.Find("MissionCheck/EightFlyMode").gameObject.SetActive(false);
                    m_SetRoot.transform.Find("MissionCheck/HappningFlyMode").gameObject.SetActive(false);
                }
                else if (AppData.m_MissionId == AppData.MissionID.EightFly)
                {
                    m_SetRoot.transform.Find("MissionCheck/SquareFlyMode").gameObject.SetActive(false);
                    m_SetRoot.transform.Find("MissionCheck/EightFlyMode").gameObject.SetActive(true);
                    m_SetRoot.transform.Find("MissionCheck/HappningFlyMode").gameObject.SetActive(false);
                }
                else if (AppData.m_MissionId == AppData.MissionID.HappningFly)
                {
                    m_SetRoot.transform.Find("MissionCheck/SquareFlyMode").gameObject.SetActive(false);
                    m_SetRoot.transform.Find("MissionCheck/EightFlyMode").gameObject.SetActive(false);
                    m_SetRoot.transform.Find("MissionCheck/HappningFlyMode").gameObject.SetActive(true);
                }
            }
            // フリーモード時の設定
            else if(AppData.m_PlayMode == AppData.PlayMode.Free)
            {
                // 床の白無地化
                m_SetRoot.transform.Find("FloorTextCanvas").gameObject.SetActive(false);
                m_SetRoot.transform.Find("Floor_Area").gameObject.SetActive(false);
                m_SetRoot.transform.Find("Cone").gameObject.SetActive(false);

                m_SetRoot.transform.Find("MissionCheck").gameObject.SetActive(false);
            }
            // リプレイ時
            else if(AppData.m_PlayMode == AppData.PlayMode.Replay)
            {
                m_SetRoot.transform.Find("MissionCheck").gameObject.SetActive(false);

                bool isReplayMission = true;
                if (AppData.GetReplayData(AppData.m_SelectReplayIndex).m_ReplayInfoList != null)
                {
                    isReplayMission = (AppData.GetReplayData(AppData.m_SelectReplayIndex).m_PlayMode == AppData.PlayMode.Mission);
                }
                // 床の白無地化
                m_SetRoot.transform.Find("FloorTextCanvas").gameObject.SetActive(isReplayMission);
                m_SetRoot.transform.Find("Floor_Area").gameObject.SetActive(isReplayMission);
                m_SetRoot.transform.Find("Cone").gameObject.SetActive(isReplayMission);
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

            // 要素の開始
            m_ObjRepository.SetGameStart();
        }
        // スタート演出後のステージ処理開始
        public void SetStart_ProcOn()
        {
        }
        // ゲームオーバー処理
        public void SetGameOver()
        {
        }
        // リセット
        public void SetReset()
        {
            m_ObjRepository.SetReset();

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
            // カメラターゲット設定
            MainCameraManager.I.CamCtrl.SetTarget(Shs.ShSceneUtils.GetSceneRootObj("MainGame", "Root").transform.Find("Human/Head").gameObject);

            // ドローン対応
            DroneCtrl = MG_Mediator.RootT.Find("Drone").gameObject.AddComponent<DroneCtrl>();
            DroneCtrl.SetStartInfo();

            MainCameraManager.I.CamCtrl.SetLookAtCamera(DroneCtrl.gameObject);
        }

        //================================================
        // [///] 
        //================================================
        // ドローンログ設定
        public void SaveDroneInputLog()
        {
            DroneCtrl.SaveLocalReplayData();
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
            if (AppData.m_PlayMode == AppData.PlayMode.Replay)
            {
                // ドローン
                DroneCtrl.ProcReplay_Update();
            }
            else
            {
                // ドローン
                DroneCtrl.ProcInput();
            }
        }
    }
}