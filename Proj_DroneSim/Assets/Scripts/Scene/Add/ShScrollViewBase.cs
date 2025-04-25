using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShScroll
{
    //================================================
    // [///]  リストクラス
    //================================================
    public class ShContentUIList
    {
        public List<ShViewContentBase> m_List = null;
        public ShContentUIList()
        {
            m_List = new List<ShViewContentBase>();
        }
        public void Add(ShViewContentBase _temp)
        {
            m_List.Add(_temp);
        }
        public void Clear()
        {
            m_List.Clear();
        }
        public void Destroy()
        {
            foreach(ShViewContentBase ctrl in m_List)
            {
                if (ctrl)
                {
                    ctrl.SetDestroy();
                }
            }
        }
        // リストにチェックがあるか
        public bool IsSelected()
        {
            bool isSelect = false;
            foreach (ShViewContentBase ctrl in m_List)
            {
                if (ctrl)
                {
                    if (ctrl.IsCheckedToggle())
                    {
                        isSelect = true;
                        break;
                    }
                }
            }
            return isSelect;
        }
    }

    /// <summary>
    /// スクロールビュー操作用
    /// </summary>
    public class ShScrollViewBase : ShUIBaseCtrl
    {
        //================================================
        // [///] 定義
        //================================================
        // 本体
        protected RectTransform RectT { private set; get; } = null;
        protected ScrollRect m_ScrollRect = null;

        // コンテンツ
        // Fitter
        protected ContentSizeFitter m_SizeFitter = null;
        // レイアウトグループ @TODO:GridLayoutGroup設定のみ想定
        protected GridLayoutGroup m_LayoutGrp = null;
        // リストデータ
        protected ShContentUIList m_ContentList = null;
        // コンテンツUIプレファブ(継承先で設定)
        protected GameObject m_ContentPrefab = null;

        // リストデータが無い表示
        protected GameObject m_NoneDataDisp = null;

        // リアクション用:最後に選択したボタン
        public ShViewContentBase SelectedUI { private set; get; } = null;


        //================================================
        // [///] 初期化
        //================================================
        public override void ProcInitialize()
        {
            // 自身
            RectT = GetComponent<RectTransform>();
            m_ScrollRect = GetComponent<ScrollRect>();

            // コンテンツ
            m_SizeFitter = gameObject.GetComponentInChildren<ContentSizeFitter>();
            if (!m_SizeFitter)
            {
                m_SizeFitter = m_ScrollRect.content.gameObject.AddComponent<ContentSizeFitter>();
            }
            m_LayoutGrp = gameObject.GetComponentInChildren<GridLayoutGroup>();
            if (!m_LayoutGrp)
            {
                m_LayoutGrp = m_ScrollRect.content.gameObject.AddComponent<GridLayoutGroup>();
            }
            m_ContentList = new ShContentUIList();

            // データが無い表示
            m_NoneDataDisp = transform.Find("Grp_NoneData").gameObject;
            m_NoneDataDisp.SetActive(false);

            AddProcInitialize();
        }
        protected virtual void AddProcInitialize() { }

        //=========================================================================
        //
        //
        // [///] ScrollRect - 全体設定
        //
        //
        //=========================================================================
        // ----------------------------------------------
        /// <summary>
        /// スクロールビューのサイズ設定
        /// </summary>
        protected void SetViewSize(float width, float height)
        {
            RectT.sizeDelta = new Vector2(width, height);
        }



        //=========================================================================
        //
        //
        // [///] ScrollRect - バー設定
        //
        //
        //=========================================================================
        // ----------------------------------------------
        // スクロールバーの表示設定
        public enum ScrollBarSettings
        {
            None,
            Horizontal,
            Vertical,
            Both,
        }
        // ----------------------------------------------
        /// <summary>
        /// スクロールバーの設定
        /// </summary>
        protected void SetScrollBarSetting(ScrollBarSettings barset)
        {
            bool isHorizontal = false;
            bool isVertical = false;
            switch (barset)
            {
                case ScrollBarSettings.None:
                    break;
                case ScrollBarSettings.Horizontal:
                    isHorizontal = true;
                    break;
                case ScrollBarSettings.Vertical:
                    isVertical = true;
                    break;
                case ScrollBarSettings.Both:
                    isHorizontal = true;
                    isVertical = true;
                    break;
            }

            // 反映
            m_ScrollRect.horizontal = isHorizontal;
            m_ScrollRect.vertical = isVertical;
        }


        //=========================================================================
        //
        //
        // [///] ScrollRect - レイアウト設定
        //
        //
        //=========================================================================
        // ----------------------------------------------
        /// <summary>
        /// Padding設定
        /// </summary>
        protected void SetLayoutPadding(RectOffset offset)
        {
            m_LayoutGrp.padding = offset;
        }
        /// <summary>
        /// セルサイズ設定
        /// </summary>
        /// <param name="size">セルサイズ</param>
        protected void SetLayoutCellSize(Vector2 size)
        {
            m_LayoutGrp.cellSize = size;
        }
        /// <summary>
        /// セル間の余白設定
        /// </summary>
        protected void SetLayoutCellMargin(Vector2 marginSize)
        {
            m_LayoutGrp.spacing = marginSize;
        }
        /// <summary>
        /// セルの向き設定
        /// </summary>
        protected void SetLayoutAxis_Horizontal()
        {
            m_LayoutGrp.startAxis = GridLayoutGroup.Axis.Horizontal;
        }
        protected void SetLayoutAxis_Vertical()
        {
            m_LayoutGrp.startAxis = GridLayoutGroup.Axis.Vertical;
        }
        /// <summary>
        /// セルのアンカー設定
        /// </summary>
        protected void SetLayoutCellAlighnment(TextAnchor anchor)
        {
            m_LayoutGrp.childAlignment = anchor;
        }
        /// <summary>
        /// セルの行列設定 行数指定
        /// </summary>
        protected void SetLayoutConstraint_Row(int cnt)
        {
            m_LayoutGrp.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            m_LayoutGrp.constraintCount = cnt;
        }
        /// <summary>
        /// セルの行列設定 縦数指定
        /// </summary>
        protected void SetLayoutConstraint_Column(int cnt)
        {
            m_LayoutGrp.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            m_LayoutGrp.constraintCount = cnt;
        }
        /// <summary>
        /// セルの行列数設定
        /// </summary>
        protected void SetLayoutConstraintCount(int cnt)
        {
            m_LayoutGrp.constraintCount = cnt;
        }


        //=========================================================================
        //
        //
        // [///] Content - リスト設定
        //
        //
        //=========================================================================
        // ----------------------------------------------
        /// <summary>
        /// 選択したコンテンツUIの設定
        /// </summary>
        public void SetSelectedContent(ShViewContentBase ctrl)
        {
            SelectedUI = ctrl;
        }
        /// <summary>
        /// リストにチェックがあるか
        /// </summary>
        public bool IsSelectedExist()
        {
            return m_ContentList.IsSelected();
        }


        //=========================================================================
        //
        //
        // [///] ScrollRect - リストが無い表示
        //
        //
        //=========================================================================
        // ----------------------------------------------
        /// <summary>
        /// ありません表示
        /// </summary>
        protected void SetNoneDataActive()
        {
            m_NoneDataDisp.SetActive(true);
        }



    }// class
}// namespace