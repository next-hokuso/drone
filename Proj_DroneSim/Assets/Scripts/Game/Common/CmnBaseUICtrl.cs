using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;

public class CmnBaseUICtrl : MonoBehaviour
{
    //================================================
    // [///] 定義
    //================================================
    //================================================
    // [///] 
    //================================================
    protected Button m_Button = null;
    protected Text m_Text = null;
    protected Image m_Image = null;

    //================================================
    // [///]
    //================================================
    protected virtual void Start()
    {
        SetInfo();
    }
    protected virtual void Update()
    {

    }
    public virtual void SetInfo()
    {
        //m_Button = ;
        //m_Text = ;
        //m_Image = ;

        {
            // 機能付与
            if (m_Button)
            {
                m_Button.onClick.AddListener(OnClick_MyButton);
            }
        }
    }
    //================================================
    // [///] public method
    //================================================

    //================================================
    // [///] OnClick
    //================================================
    public void OnClick_MyButton()
    {
    }
}
