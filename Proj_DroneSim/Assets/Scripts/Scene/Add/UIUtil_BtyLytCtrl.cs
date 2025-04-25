using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// ボタンレイアウト設定クラス
/// </summary>
public class UIUtil_BtyLytCtrl : ShUIBaseCtrl
{
    //================================================
    // [///] 定義
    //================================================
    // ボタン関係
    private Transform m_BtnContentT = null;
    private GameObject m_BtnPreObj = null;
    private List<Button> m_BtnList = new List<Button>();
    private List<DialogBtnSetting> m_BtnSettingList = new List<DialogBtnSetting>();

    //================================================
    // [///]
    //================================================
    // 初期化
    public override void ProcInitialize()
    {
        m_BtnContentT = transform.Find("Grp_BtnLyt");
        m_BtnContentT.gameObject.SetActive(true);

        // ボタン処理
        {
            m_BtnPreObj = transform.Find("Grp_BtnLyt/Btn").gameObject;
            m_BtnPreObj.transform.SetParent(transform);
            m_BtnPreObj.SetActive(false);
        }
    }

    /// <summary>
    /// DialogBtnSettingのリストに合わせてボタンを配置する
    /// </summary>
    public void SetBtnContent(DialogBtnSetting[] btnSettings)
    {
        // リストの削除
        {
            foreach (Button btn in m_BtnList)
            {
                if (btn)
                {
                    Destroy(btn.gameObject);
                }
            }
            m_BtnList.Clear();
        }

        // ボタンUIの設定
        {
            // リストの追加
            m_BtnSettingList.Clear();
            foreach (DialogBtnSetting info in btnSettings)
            {
                m_BtnSettingList.Add(info);
            }


            // Btn分のリスト作成
            int idx = 0;
            foreach (DialogBtnSetting info in m_BtnSettingList)
            {
                // 設定ボタン番号より多い場合はスキップ
                if(idx > (int)btnSettings.Length) { continue; }

                GameObject go = Instantiate(m_BtnPreObj);
                if (go)
                {
                    Transform t = go.transform;
                    go.name = "Btn";
                    t.SetParent(m_BtnContentT);
                    t.localScale = Vector3.one;
                    // テキスト設定
                    t.GetComponentInChildren<Text>().text = info.m_Text;

                    m_BtnList.Add(t.GetComponent<Button>());
                    go.SetActive(true);
                    idx++;
                }
            }
        }
    }

    //=========================================================================
    //
    //
    // [///] ボタン設定
    //
    //
    //=========================================================================
    public void SetBtnListner(int idx, UnityEngine.Events.UnityAction call)
    {
        if (idx < 0 && m_BtnList.Count <= idx) return;

        // 設定
        // m_BtnList[idx].onClick.AddListener(call);
        Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(
            m_BtnList[idx], call);
    }
}
