//https://gist.github.com/am1tanaka/d73718a1865e663fc0ce24e44a9dcf2c
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class WebGLNativeInputFieldTMPro : TMPro.TMP_InputField
{
    public enum EDialogType
    {
        PromptPopup,
        OverlayHtml,
    }
    public string m_DialogTitle = "Input Text";
    public string m_DialogOkBtn = "OK";
    public string m_DialogCancelBtn = "Cancel";
    public EDialogType m_DialogType = EDialogType.OverlayHtml;

    private GameObject m_goSelected = null;
    private bool m_isActive = false;

#if UNITY_WEBGL && !UNITY_EDITOR
//#if UNITY_WEBGL

    string limitText(string argStr)
    {
        // nullチェック挿入
        if (!string.IsNullOrEmpty(argStr) && this.characterLimit > 0)
        {
            //Debug.Log("argStr length:" + argStr.Length);
            //Debug.Log("characterLimit:" + characterLimit);
            // 入力文字列が最大文字数以上の時のみ
            if (argStr.Length >= characterLimit)
            {
                argStr = argStr.Substring(0, characterLimit);
            }
        }
        return argStr;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        switch( m_DialogType ){
            case EDialogType.PromptPopup:
                // ---original source---
                //this.text = limitText(WebNativeDialog.OpenNativeStringDialog(m_DialogTitle, this.text));
                //StartCoroutine(this.DelayInputDeactive());
                if (!m_isActive)
                {
                    base.OnSelect(eventData);
                    m_isActive = true;
                    StartCoroutine(DelayInputActive());
                }
                break;
            case EDialogType.OverlayHtml:
                WebNativeDialog.SetUpOverlayDialog(m_DialogTitle, this.text , m_DialogOkBtn , m_DialogCancelBtn );
                StartCoroutine(this.OverlayHtmlCoroutine());
                break;
        }
    }

    private IEnumerator DelayInputActive()
    {
        yield return new WaitForSeconds(0.5f);
        this.text = limitText(WebNativeDialog.OpenNativeStringDialog(m_DialogTitle, this.text));
        StartCoroutine(this.DelayInputDeactive());
    }

    private IEnumerator DelayInputDeactive()
    {
        //yield return new WaitForEndOfFrame();
        //this.DeactivateInputField();
        //while (EventSystem.current.currentSelectedGameObject == m_goSelected)
        //{
        //    yield return new WaitForEndOfFrame();
        //}
        //m_goSelected = null;
        //m_isActive = false;
        //Debug.Log("isActive=false");

        yield return new WaitForEndOfFrame();
        this.DeactivateInputField();
        EventSystem.current.SetSelectedGameObject(null);
        m_isActive = false;
    }

    private IEnumerator OverlayHtmlCoroutine()
    {
        yield return new WaitForEndOfFrame();
        this.DeactivateInputField();
        EventSystem.current.SetSelectedGameObject(null);
        WebGLInput.captureAllKeyboardInput = false;
        while (WebNativeDialog.IsOverlayDialogActive())
        {
            yield return null;
        }
        WebGLInput.captureAllKeyboardInput = true;

        if (!WebNativeDialog.IsOverlayDialogCanceled())
        {
            this.text = limitText(WebNativeDialog.GetOverlayDialogValue());
        }
    }
    
#endif
}
