using System.Collections;
using UnityEngine;

namespace SampleScene
{
    public class Samp_StateCtrl : MonoBehaviour
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
            yield break;
        }
    }
}
