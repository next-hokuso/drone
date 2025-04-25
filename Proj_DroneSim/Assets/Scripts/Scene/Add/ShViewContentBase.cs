using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// リストコンテンツ表示用
/// </summary>
public class ShViewContentBase : MonoBehaviour
{
    //================================================
    // [///] 定義
    //================================================
    public int Index { private set; get; } = 0;
    public ShScroll.ShScrollViewBase m_ParentCtrl = null;   // ボタンクリック時などのリアクション用

    public Button m_Button = null;
    public Toggle m_Toggle = null;
    private bool m_IsToggleExist = false;
    public Text m_InfoText = null;
    private RawImage m_RawImg = null;

    //=========================================================================
    //
    // [///] 初期化 (インデックス設定)
    //
    //=========================================================================
    public void ProcInitialize(int index, ShScroll.ShScrollViewBase ctrl)
    {
        Index = index;
        m_ParentCtrl = ctrl;

        // 初期設定
        transform.name = "SetUI_" + index;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = Vector3.one;
    }
    public void SetParentCtrl(ShScroll.ShScrollViewBase ctrl)
    {
        m_ParentCtrl = ctrl;
    }

    //=========================================================================
    //
    // [///] ボタン設定
    //
    //=========================================================================
    public void SetButton()
    {
        m_Button = GetComponentInChildren<Button>();
    }
    /// <summary>
    /// ボタンクリック時に親(スクロールビュー)に自分を設定する
    /// </summary>
    /// 　→eventsystemを使って外から選択したGameObject取得→GetComponentの方が自由になる
    public void SetOnClick_Btn_ThisSendToParentCtrl()
    {
        //m_Button.onClick.AddListener(SendToParentCtrl);
        Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(m_Button, SendToParentCtrl);
    }
    private void SendToParentCtrl()
    {
        m_ParentCtrl.SetSelectedContent(this);
    }
    // ボタンListner設定
    public void AddButtonListner(UnityEngine.Events.UnityAction call)
    {
        if (!m_Button)
        {
            m_Button = GetComponentInChildren<Button>();
            SetOnClick_Btn_ThisSendToParentCtrl();
        }
        // m_Button.onClick.AddListener(call);
        Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(m_Button, call);
    }

    //=========================================================================
    //
    // [///] Toggle設定
    //
    //=========================================================================
    public void SetToggle()
    {
        m_Toggle = GetComponentInChildren<Toggle>();
        m_Toggle.isOn = false;
        m_IsToggleExist = true;
    }
    // トグルとボタンを紐づける
    public void SetToggle_AddButtonListner()
    {
        AddButtonListner(OnClick_Btn_ToggleChange);
    }
    // トグルの切り替え設定
    private void OnClick_Btn_ToggleChange()
    {
        if (m_IsToggleExist)
        {
            m_Toggle.isOn = !m_Toggle.isOn;
        }
    }
    // トグルの状態取得
    public bool IsCheckedToggle()
    {
        if (m_IsToggleExist)
        {
            return m_Toggle.isOn;
        }
        else { return false; }
    }

    //=========================================================================
    //
    // [///] Text設定
    //
    //=========================================================================
    public void SetInfoText(string text)
    {
        m_InfoText = transform.Find("Text").GetComponent<Text>();
        m_InfoText.text = text;
    }

    //=========================================================================
    //
    // [///] RawImage設定
    //
    //=========================================================================
    public void SetRawImage(Texture2D tex)
    {
        if (tex)
        {
            m_RawImg = transform.Find("RawImage").GetComponent<RawImage>();
            m_RawImg.texture = tex;
            m_RawImg.enabled = true;
        }
        else
        {
            m_RawImg = transform.Find("RawImage").GetComponent<RawImage>();
            m_RawImg.enabled = false;
        }
    }

    // 自身の削除
    public void SetDestroy()
    {
        // 自身の削除
        Destroy(gameObject);
    }
}
