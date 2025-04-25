using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using System.Linq;

namespace SampleScene
{
    public class Samp_ObjMediator
    {
        //================================================
        // [///] 定義 
        //================================================
        // データ保管クラス
        //

        private Transform m_SetRoot = null;

        //================================================
        // [///] 初期設定
        //================================================
        public void SetInit()
        {
            // m_ObjRepository = new MG_ObjRepository();
            // m_ObjRepository.SetInit();

            // m_SetRoot = MG_Mediator.RootT.Find("SetRoot");
        }

        //================================================
        // [///] ステージの読み込み
        //================================================
        public void ProcLoadStage()
        {
        }

        //================================================
        // [///] state変更時の処理関連
        //================================================
        // スタート時
        public void SetStart()
        {
            m_SetRoot.gameObject.SetActive(true);

            // 要素の開始
            //m_ObjRepository.SetGameStart();
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
            //m_ObjRepository.SetReset();
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
            // GameObject.Destroy(m_StageGo);
            // m_StageGo = null;

            SetReset();
        }

        //================================================
        // [///] create関連
        //================================================
        // 要素が多くなった場合別クラスに移植する
        public void CreateGameObject()
        {
        }

        //================================================
        // [///] 
        //================================================
    }
}