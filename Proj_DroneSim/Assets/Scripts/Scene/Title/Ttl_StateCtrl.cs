using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Game;

namespace Title
{
    public class Ttl_StateCtrl : MonoBehaviour
    {
        //================================================
        // [///]
        //================================================
        private enum FlowState
        {
            None = -1,
            InitState,      // 処理開始
            MainUpdate,     // メイン中

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
                case FlowState.InitState:
                    break;
                case FlowState.MainUpdate:
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
        //
        // [///] InitState
        //
        //================================================
        // 呼び出し:処理開始
        public void SetInitState()
        {
            m_CurrentFlowState = FlowState.InitState;

            // 開始時処理の呼び出し
            StartCoroutine(OnProc_InitState());
        }
        public IEnumerator OnProc_InitState()
        {
            yield break;
        }

        //================================================
        //
        // [///] SceneEnd
        //
        //================================================
        // 呼び出し:シーン破棄時
        public void SetSceneEndState()
        {
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
