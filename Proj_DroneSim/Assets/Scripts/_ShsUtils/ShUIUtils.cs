using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shs
{
    //================================================
    // [///]  リストクラス
    //================================================
    public class ShUIList
    {
        public List<ShUIBaseCtrl> m_List = null;
        public ShUIList()
        {
            m_List = new List<ShUIBaseCtrl>();
        }
        public void Add(ShUIBaseCtrl _temp)
        {
            m_List.Add(_temp);
        }
        public void Clear()
        {
            m_List.Clear();
        }
        public void SetVisible(int _idx, bool _isVisible)
        {
            m_List[_idx].SetVisible(_isVisible);
        }
        // 初期化
        public void ProcInitialize()
        {
            foreach (ShUIBaseCtrl ctrl in m_List)
            {
                ctrl.ProcInitialize();
            }
        }
        // 表示切り替え
        public void Show()
        {
            foreach (IShVisible ctrl in m_List)
            {
                ctrl.Show();
            }
        }
        // 表示切り替え
        public void Hide()
        {
            foreach (IShVisible ctrl in m_List)
            {
                ctrl.Hide();
            }
        }
        // テキスト設定
        public void SetText(int _idx, string _text)
        {
            ((ShUITextCtrl)m_List[_idx]).SetText(_text);
        }
        // イメージ取得
        public Image GetImage(int _idx)
        {
            return ((ShUIImageCtrl)m_List[_idx]).GetImage();
        }

    }

    //================================================
    // [///]  UIUtils
    //================================================
    public class ShUIUtils
    {
        // ボタンの拡縮演出
        public static IEnumerator ProcBtnEffect(GameObject go, float effectTime = 0.5f)
        {
            // 演出
            float time = effectTime;
            float init = go.transform.localScale.x;

            float time1 = effectTime * 0.8f;
            float time2 = effectTime * 0.5f;
            float time3 = 0.0f;

            while (true)
            {
                time -= Time.deltaTime;
                float fps = Application.targetFrameRate;
                fps = 60.0f / fps;
                if (time > time1)
                {
                    // スケール
                    float add = 0.03f * fps;
                    go.transform.localScale += new Vector3(add, add, add);
                }
                else if (time > time2)
                {
                    // スケール
                    float add = -0.02f * fps;
                    go.transform.localScale += new Vector3(add, add, add);

                }
                else if (time > time3)
                {
                    // 演出後の値
                    go.transform.localScale = new Vector3(init, init, init);
                }
                else
                {
                    break;
                }
                yield return null;
            }
        }
        // ボタンの拡縮演出
        public static IEnumerator ProcBtnEffect2(GameObject go, float effectTime = 0.5f, float initSizeX = 1.0f)
        {
            // 演出
            float time = effectTime;
            float init = initSizeX;

            float time1 = effectTime * 0.8f;
            float time2 = effectTime * 0.5f;
            float time3 = 0.0f;

            while (true)
            {
                time -= Time.deltaTime;
                float fps = Application.targetFrameRate;
                fps = 60.0f / fps;
                if (time > time1)
                {
                    // スケール
                    float add = 0.03f * fps;
                    go.transform.localScale += new Vector3(add, add, add);
                }
                else if (time > time2)
                {
                    // スケール
                    float add = -0.02f * fps;
                    go.transform.localScale += new Vector3(add, add, add);

                }
                else if (time > time3)
                {
                    // 演出後の値
                    go.transform.localScale = new Vector3(init, init, init);
                }
                else
                {
                    break;
                }
                yield return null;
            }
        }


        /// <summary>
        /// ボタン設定
        /// </summary>
        /// <param name="btn">機能対象ボタン</param>
        /// <param name="call">アクションメソッド</param>
        public static void ProcAddListner_And_BtnDefaultSetting(Button btn, UnityEngine.Events.UnityAction call)
        {
            btn.onClick.AddListener(() => Yns.YnSys.GetAppCommon().SetDelayCall(call));

            // デフォルトParam設定
            // SetButtonDefaultParam(btn);
        }

        // ボタンのデフォルト設定(初回のパラメーター設定)
        public static void SetButtonDefaultParam(Button btn)
        {
            ColorBlock colorBlock = btn.colors;

            // ハイライトカラー
            colorBlock.highlightedColor = Color.white;

            // 押下時カラー
            colorBlock.pressedColor = new Color(245.0f / 255.0f, 234.0f / 255.0f, 68.0f / 255.0f, 1.0f);

            btn.colors = colorBlock;
        }
    }
}