using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DbgContentInfoBase : MonoBehaviour
{
    //================================================
    // [///] 定義
    //================================================
    protected Color Color_DefBtn_Enable = new Color(255.0f / 255.0f, 146.0f / 255.0f, 47.0f / 255.0f, 1.0f);
    protected Color Color_DefBtn_Disable = new Color(116.0f / 255.0f, 116.0f / 255.0f, 116.0f / 255.0f, 1.0f);

    //================================================
    // [///]
    //================================================
    protected Text m_InfoText = null;
    protected InputField m_InputField = null;
    protected InputField m_InputField2 = null;
    protected Button m_Button = null;
    protected Text m_ButtonText = null;
    protected Button m_Button2 = null;
    protected Slider m_Slider = null;
    protected float m_SliderValue = 0.0f;

    protected Text m_ContentText = null;

    //================================================
    // [///]
    //================================================
    protected virtual void Start()
    {
        enabled = false;
    }

    // Componentの取得
    public virtual void SetInfoText(string text)
    {
        m_InfoText = transform.Find("Text").GetComponent<Text>();
        m_InfoText.text = text;
    }
    public virtual void SetInputFieldText(string text)
    {
        m_InputField = transform.Find("InputField").GetComponent<InputField>();
        m_InputField.text = text;
    }
    public virtual void SetInputField2Text(string text)
    {
        m_InputField2 = transform.Find("InputField2").GetComponent<InputField>();
        m_InputField2.text = text;
    }
    public virtual void SetButton(string buttonText = "")
    {
        m_Button = transform.Find("Button").GetComponent<Button>();
        m_ButtonText = m_Button.GetComponentInChildren<Text>();
        m_ButtonText.text = buttonText;
    }
    public virtual void SetButton2(string buttonText = "")
    {
        m_Button2 = transform.Find("Button2").GetComponent<Button>();
        m_Button2.GetComponentInChildren<Text>().text = buttonText;
    }
    public virtual void SetSlider()
    {
        m_Slider = transform.Find("Slider").GetComponent<Slider>();
        m_Slider.value = m_SliderValue;
    }
    public virtual void SetContentText(string text)
    {
        m_ContentText = transform.Find("ContentText").GetComponent<Text>();
        m_ContentText.text = text;
    }

    // Listenerの設定
    public virtual void SetInputField_OnEdit()
    {
        m_InputField.onEndEdit.AddListener(ProcOnEndEdit);
    }
    public virtual void SetInputField2_OnEdit()
    {
        m_InputField2.onEndEdit.AddListener(ProcOnEndEdit2);
    }
    public virtual void SetInputField1_OnEdit(UnityEngine.Events.UnityAction<string> call)
    {
        m_InputField.onEndEdit.AddListener(call);
    }
    public virtual void SetInputField2_OnEdit(UnityEngine.Events.UnityAction<string> call)
    {
        m_InputField2.onEndEdit.AddListener(call);
    }
    public virtual void SetSlider_OnEdit(UnityEngine.Events.UnityAction<float> call)
    {
        m_Slider.onValueChanged.AddListener(call);
    }

    public virtual void SetButton1_OnClick(UnityEngine.Events.UnityAction call)
    {
        m_Button.onClick.AddListener(call);
    }
    public virtual void SetButton2_OnClick(UnityEngine.Events.UnityAction call)
    {
        m_Button2.onClick.AddListener(call);
    }

    //================================================
    // [///]
    //================================================
    protected virtual void ProcOnEndEdit(string arg)
    {
    }
    protected virtual void ProcOnEndEdit2(string arg)
    {
    }
}
