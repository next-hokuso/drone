using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// システムダイアログ表示クラス
public class DialogBtnSetting
{
    public string m_Text = "";
    public UnityEngine.Events.UnityAction m_Action = null;
    public bool m_IsCancel = false;
    public bool m_IsAction = false;
    public bool m_IsForceClose = false; // ダイアログ処理後UIを開く等即閉じてほしい
    public DialogBtnSetting(string text)
    {
        m_Text = text;
    }
    public DialogBtnSetting(string text, UnityEngine.Events.UnityAction call, bool isForceClose = false)
    {
        m_Text = text;
        m_Action = call;
        m_IsAction = true;
        m_IsForceClose = isForceClose;
    }
    public DialogBtnSetting(string text, bool isCancelCall)
    {
        m_Text = text;
        m_IsCancel = isCancelCall;
    }
}
public class SysDialogSetting
{
    public string m_InfoText = "";
    public DialogBtnSetting[] m_BtnSettingList = null;
    public SysDialogSetting(string text, DialogBtnSetting[] list)
    {
        m_InfoText = text;
        m_BtnSettingList = list;
    }
}

public class ShSysDialogCtrl : ShUIBaseCtrl
{
    //================================================
    // [///] 定義
    //================================================
    // 演出フラグ
    private bool m_IsProcEnd = false;
    private bool m_IsInOutAnimPlay = false;
    private bool m_IsWindowOpen = false;

    // テキスト関係
    private Text m_Txt_MainInfo = null;

    // ボタン関係
    private Transform m_BtnContentT = null;
    private GameObject m_BtnPreObj = null;
    private List<Button> m_ButtonList = new List<Button>();
    private List<DialogBtnSetting> m_BtnSettingList = new List<DialogBtnSetting>();

    // フェードイン/アウト操作
    private UIUtil_FadeInOutCtrl m_FadeCtrl = null;

    //================================================
    // [///]
    //================================================
    public override void ProcInitialize()
    {
        m_FadeCtrl = gameObject.AddComponent<UIUtil_FadeInOutCtrl>();
        m_Txt_MainInfo = transform.Find("Txt_Main").GetComponent<Text>();
        m_BtnContentT = transform.Find("Grp_BtnLyt");
        m_BtnPreObj = transform.Find("Grp_BtnLyt/Btn").gameObject;
        m_BtnPreObj.transform.SetParent(transform);
        m_BtnPreObj.SetActive(false);

        // 非表示
        gameObject.SetActive(false);
    }

    //================================================
    // [///] public method
    //================================================        
    // Window設定
    public void SetDialogSetting(SysDialogSetting setting)
    {
        m_Txt_MainInfo.text = setting.m_InfoText;
        // ボタンUIの設定
        {
            // リストの追加
            foreach(Button btn in m_ButtonList)
            {
                Destroy(btn.gameObject);
            }
            m_ButtonList.Clear();
            m_BtnSettingList.Clear();
            foreach (DialogBtnSetting info in setting.m_BtnSettingList)
            {
                m_BtnSettingList.Add(info);
            }

            // Btn分のリスト作成
            int idx = 0;
            foreach (DialogBtnSetting info in m_BtnSettingList)
            {
                GameObject go = Instantiate(m_BtnPreObj);
                if (go)
                {
                    Transform t = go.transform;
                    go.name = "Btn";
                    t.SetParent(m_BtnContentT);
                    t.localScale = Vector3.one;
                    // テキスト設定
                    t.GetComponentInChildren<Text>().text = info.m_Text;

                    // ボタン
                    m_ButtonList.Add(t.GetComponent<Button>());
                    if(info.m_IsAction)
                    {
                        // 処理
                        SetButtonListner(idx, info.m_Action);
                        if (info.m_IsForceClose)
                        {
                            SetButtonListner(idx, SetWindowForceClose);
                        }
                        else
                        {
                            SetButtonListner(idx, SetCloseWindow);
                        }
                    }
                    else if(info.m_IsCancel)
                    {
                        // フェードありWindowClose
                        SetButtonListner(idx, SetCloseWindow);
                    }
                    else
                    {
                        // 即WindowCloseではない
                        SetButtonListner(idx, SetCloseWindow);
                    }
                    go.SetActive(true);
                    idx++;
                }
            }
        }
    }
    // ボタンListner設定
    private void SetButtonListner(int idx, UnityEngine.Events.UnityAction call)
    {
        //m_ButtonList[idx].onClick.AddListener(call);
        Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(m_ButtonList[idx], call);
    }
    // ボタンListner設定
    public void AddButtonListner(int idx, UnityEngine.Events.UnityAction call)
    {
        //m_ButtonList[idx].onClick.AddListener(call);
        Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(m_ButtonList[idx], call);
    }
    // ウィンドウクローズ
    private void OnClikc_WindowClose()
    {
        SetCloseWindow();
    }

    //================================================
    // [///] public method
    //================================================        
    // ウィンドウIn設定
    public void SetOpenWindow()
    {
        if (m_IsWindowOpen) return;
        m_IsWindowOpen = true;

        m_IsProcEnd = false;
        m_IsInOutAnimPlay = true;

        gameObject.SetActive(true);
        StartCoroutine(ProcInWindow());
    }
    private IEnumerator ProcInWindow()
    {
        m_FadeCtrl.Set_FadeIn();
        yield return new WaitForSeconds(m_FadeCtrl.GetFadeInTime());

        m_IsProcEnd = true;
        m_IsInOutAnimPlay = false;
        yield break;
    }
    // フレームアウト
    public void SetCloseWindow()
    {
        if (!m_IsWindowOpen) return;
        m_IsWindowOpen = false;

        m_IsInOutAnimPlay = true;

        StartCoroutine(ProcOutWindow());
    }
    private IEnumerator ProcOutWindow()
    {
        m_FadeCtrl.Set_FadeOut();
        yield return new WaitForSeconds(m_FadeCtrl.GetFadeOutTime());

        SetWindowForceClose();
        yield break;
    }
    // Window
    public void SetWindowForceClose()
    {
        m_IsWindowOpen = false;
        m_IsInOutAnimPlay = false;
        gameObject.SetActive(false);
    }

    //================================================
    // [///] public method
    //================================================
    public bool IsProcEnd()
    {
        return m_IsProcEnd;
    }
    public bool IsInOutAnimPlaying()
    {
        return m_IsInOutAnimPlay;
    }
}
