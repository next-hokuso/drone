using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class ShUITextCtrl : ShUIBaseCtrl
{
    public Text m_Text = null;

    public override void ProcInitialize()
    {
        m_Text = GetComponent<Text>();
        if(m_Text == null)
        {
            m_Text = GetComponentInChildren<Text>();
        }
    }
    public void SetText(string _text)
    {
        m_Text.text = _text;
    }
}
public class ShUITMPTextCtrl : ShUIBaseCtrl
{
    public TMPro.TMP_Text m_Text = null;

    public override void ProcInitialize()
    {
        m_Text = GetComponent<TMPro.TMP_Text>();
        if (m_Text == null)
        {
            m_Text = GetComponentInChildren<TMPro.TMP_Text>();
        }
    }
    public void SetText(string _text)
    {
        m_Text.text = _text;
    }
}
