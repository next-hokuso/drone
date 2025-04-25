using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace MainGame
{
    public class MG_ObjRepository
    {
        //================================================
        // [///] 定義 
        //================================================
        private IShObjNodeList m_ObjNodeList = null;

        //================================================
        // [///] 初期設定
        //================================================
        public void SetInit()
        {
            m_ObjNodeList = new IShObjNodeList();
        }

        //================================================
        // [///] create関連
        //================================================
        public void AddObjNode(IShObjNode _node)
        {
            m_ObjNodeList.Add(_node);
        }

        //================================================
        // [///] state変更時の処理関連
        //================================================
        // スタート時
        public void SetGameStart()
        {
            // 要素の開始
            m_ObjNodeList.ProcStart();
        }

        // ゲームオーバー処理
        public void SetGameOver()
        {
        }

        // リセット
        public void SetReset()
        {
        }

        // リトライ処理
        public void SetRetry()
        {
            SetReset();
        }

        // 次へ(NEXT)処理
        public void SetNext()
        {
            SetReset();
        }

        // 削除対応(ステージ選択に戻る場合の削除処理
        public void SetDestroy()
        {
            SetReset();
        }
    }
}