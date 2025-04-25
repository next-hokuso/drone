using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MainGame;

public class UIActionBtnCtrl : ShUIBaseCtrl
{
    // 178 = 255 * 0.7
    private Color DisActiveCol = new Color(178.0f / 255.0f, 178.0f / 255.0f, 178.0f / 255.0f, 1.0f);
    private Image m_Img = null;
    private Button m_Btn = null;

    public override void ProcInitialize()
    {
        m_Img = GetComponent<Image>();
        m_Btn = GetComponent<Button>();
    }

    //
    public void Update()
    {
    }

    public void LateUpdate()
    {
    }

    // 設定
    public void SetBtnColor(bool isActive)
    {
        m_Img.color = isActive ? Color.white : DisActiveCol;
    }
    // interactive
    public void SetBtnInteractable(bool isTrue)
    {
        m_Btn.interactable = isTrue;
    }
}
