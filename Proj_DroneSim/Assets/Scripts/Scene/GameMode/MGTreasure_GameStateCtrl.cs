using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Game;

namespace MainGame
{
    public class MGTreasure_GameStateCtrl : MonoBehaviour
    {
        //================================================
        // [///]
        //================================================
        private enum FlowState
        {
            None = -1,
            StateStart,     // 処理開始
            MainUpdate,     // メイン中
            GameOver,       // GOver
            GameClear,      // Clear

            Result_Wait,    // リザルト後の選択待ち

            StageChange,    // ステージ切り替え

            MainGameReset,  // リセット
            DbgWait,        // デバッグ待機

            SceneEnd,   // シーン破棄時
            End,        // 終了
        }
        private FlowState m_CurrentFlowState = FlowState.None;
        private FlowState m_PrevFlowState = FlowState.None;

        // 複雑になる場合はIShStateとかで分ける

        //================================================
        // [///]
        //================================================
        public void Start()
        {
        }

        public void Update()
        {
            ProcUpdate();
        }
        public void LateUpdate()
        {
            ProcLateUpdate();
        }
        public void ProcUpdate()
        {
            switch (m_CurrentFlowState)
            {
                case FlowState.StateStart:
                    break;
                case FlowState.MainUpdate:
                    ShInputManager.I.GetTimerCtrl().ProcUpdate();

                    MGTreasure_Mediator.ObjMediator.ProcUpdate();
                    break;

                case FlowState.GameClear:
                    break;
                case FlowState.Result_Wait:
                    break;

                case FlowState.MainGameReset:
                    break;
                case FlowState.DbgWait:
                    break;
            }
        }
        public void ProcLateUpdate()
        {
            switch (m_CurrentFlowState)
            {
                case FlowState.MainUpdate:
                    break;
            }
        }
        //================================================
        // [///] StartGame
        //================================================
        // 呼び出し:ゲーム処理開始
        public void SetStartGameState()
        {
            m_CurrentFlowState = FlowState.StateStart;
            
            // 開始時処理の呼び出し
            StartCoroutine(IsGameStart());
        }

        /// <summary>
        /// ゲームスタート
        /// </summary>
        /// <param name="isStartEff">スタート演出の有無</param>
        /// <param name="isAppFirstStart">アプリ起動後初スタートか</param>
        /// <returns></returns>
        public IEnumerator IsGameStart(bool isStartEff=true, bool isAppFirstStart = false)
        {
            // TODO:Startと別にInitを用意する
            // UI表示
            MGTreasure_Mediator.MainCanvas.GetComponent<IShVisible>().Show();
            yield return null;

            // スタート待機
            while(!MGTreasure_Mediator.MainCanvas.IsStart)
            {
                // 入力チェック
                if (AppData.GetTrg(AppData.Action.Decide))
                {
                    MGTreasure_Mediator.MainCanvas.OnClick_GameStart();
                    // SE
                    CoreManager.I.AudioComp.PlaySe(AudioId.count.ToString(), false);
                    AppData.ResetPadTrgs();
                }
                yield return null;
            }
            // スタート演出
            {
                MGTreasure_Mediator.MainCanvas.SetAnimEff_GameStart();
                //yield return new WaitForSeconds(3.05f + 0.5f);
            }

            // BGM:
            CoreManager.I.AudioComp.PlayBgm(AudioId.BGM01.ToString());

            // 開始時呼び出し
            MGTreasure_Mediator.ObjMediator.SetStart();
            MGTreasure_Mediator.ObjMediator.SetStart_ProcOn();
            MGTreasure_Mediator.MainCanvas.ProcSceneProcStart();
            yield return null;

            // 開始
            //m_IsGame = true;
            m_CurrentFlowState = FlowState.MainUpdate;
            yield return null;
        }

        //================================================
        // [///] Reset
        //================================================
        // 呼び出し:メインゲームのリセット
        public void Set6GameClear()
        {
            m_CurrentFlowState = FlowState.GameClear;
        }

        //================================================
        // [///] Reset
        //================================================
        // 呼び出し:メインゲームのリセット
        public void SetMainGameReset()
        {
            m_CurrentFlowState = FlowState.MainGameReset;

            // 処理の呼び出し
            StartCoroutine(ProcMainGameReset());
        }
        public IEnumerator ProcMainGameReset()
        {
            // フェード
            ShTransitionSys.SetLoadScene(false);
            ShTransitionSys.SetTransition(0.2f, this, "TransitionSampleState", false, false, false);
            yield return new WaitForSeconds(0.2f);

            // UIリセット
            MGTreasure_Mediator.MainCanvas.ProcSceneReset();
            // オブジェクトのリセット
            MGTreasure_Mediator.ObjMediator.SetReset();
            yield return null;

            // オブジェクトの生成
            MGTreasure_Mediator.ObjMediator.CreateGameObject();
            yield return null;

            // 覆い外し
            ShTransitionSys.SetFillOutCover();

            // 開始
            SetStartGameState();
            // yield return null;
        }
        public IEnumerator TransitionSampleState()
        {
            yield return null;
        }

        //================================================
        // [///] StageChange
        //================================================
        // 呼び出し:ステージ切り替え
        public void SetStageChange()
        {
            m_CurrentFlowState = FlowState.MainGameReset;

            // 処理の呼び出し
            StartCoroutine(ProcStageChange());
        }
        private IEnumerator ProcStageChange()
        {
            // フェード
            ShTransitionSys.SetLoadScene(false);
            ShTransitionSys.SetTransition(0.2f, this, "TransitionSampleState2", false, false, false);
            yield return new WaitForSeconds(0.2f);

            // UIリセット
            MGTreasure_Mediator.MainCanvas.ProcSceneReset();
            // オブジェクトのリセット(ステージの削除)
            MGTreasure_Mediator.ObjMediator.SetStageChage();

            // 新ステージの設定
            MGTreasure_Mediator.ObjMediator.ProcLoadStage();

            yield return null;

            // オブジェクトの生成
            MGTreasure_Mediator.ObjMediator.CreateGameObject();
            yield return null;

            // 覆い外し
            ShTransitionSys.SetFillOutCover();

            // 開始
            SetStartGameState();
        }
        public IEnumerator TransitionSampleState2()
        {
            yield return null;
        }


        //================================================
        // [///] 
        //================================================
        // インゲーム中か
        public bool IsMainUpdate()
        {
            return m_CurrentFlowState == FlowState.MainUpdate;
        }
        // メイン～リザルト中か
        public bool IsMainUpdateIncludeResult()
        {
            return m_CurrentFlowState >= FlowState.MainUpdate && m_CurrentFlowState <= FlowState.Result_Wait;
        }
        // メニューを開けるか
        public bool IsUseMenu()
        {
            if (m_CurrentFlowState == FlowState.StateStart) return false;

            return true;
        }
        public bool IsMainGameReset()
        {
            return m_CurrentFlowState == FlowState.MainGameReset;
        }

        //================================================
        // [///] 
        //================================================
        // 呼び出し:ゲーム再開
        public void CallGameReStart()
        {
            if (m_CurrentFlowState != FlowState.DbgWait) return;

            m_CurrentFlowState = m_PrevFlowState;
        }
        // 呼び出し:デバッグ_ゲーム一時停止
        public void CallGameWait()
        {
            if (m_CurrentFlowState != FlowState.MainUpdate) return;

            m_PrevFlowState = m_CurrentFlowState;
            m_CurrentFlowState = FlowState.DbgWait;
        }

        //================================================
        //
        // [///] SceneEnd
        //
        //================================================
        // 呼び出し:シーン破棄時
        public void SetSceneEndState()
        {
            // 飛行中SEの停止
            CoreManager.I.AudioComp.StopSe(AudioId.flying.ToString(), true);

            m_CurrentFlowState = FlowState.SceneEnd;

            // 処理の呼び出し
            StartCoroutine(OnProc_SceneEnd());
        }
        public IEnumerator OnProc_SceneEnd()
        {
            // 遷移システム:画面カバー/ロード画面/自動終了設定
            float fadeTime = 0.2f;
            bool isLoad = false;
            bool isFillOutAuto = false;
            // 呼び出し
            ShTransitionSys.SetTransition(fadeTime, isLoad, isFillOutAuto);
            yield return new WaitForSeconds(fadeTime);  // 画面を覆うまで待つ

            // シーン後処理設定
            //   → ゲームオブジェクトの破棄等

            // 処理終了 → 状態変更
            m_CurrentFlowState = FlowState.End;
            yield break;
        }

        // 呼び出し:シーン破棄時
        public void SetSceneEndState2()
        {
            // 飛行中SEの停止
            CoreManager.I.AudioComp.StopSe(AudioId.flying.ToString(), true);

            // 処理終了 → 状態変更
            m_CurrentFlowState = FlowState.End;
        }

        //================================================
        //
        // [///] StateCheck : 状態を返す
        //
        //================================================
        // シーン処理終了状態か
        public bool IsStage_End()
        {
            return m_CurrentFlowState == FlowState.End;
        }

    }
}
