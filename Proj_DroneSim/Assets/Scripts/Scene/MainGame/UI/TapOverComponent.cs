using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TapOverComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image { get { return GetComponentInChildren<Image>(); } }

    public Login login = null;
    public Login.Select select = Login.Select.None;

    public void SetLogin(Login _login, Login.Select _select)
    {
        login = _login;
        select = _select;
    }

    // オブジェクトの範囲内にマウスポインタが入った際に呼び出されます。
    // this method called by mouse-pointer enter the object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        CustomInputButton cBtn = this.gameObject.GetComponent<CustomInputButton>();
        if (cBtn != null)
        {
            if (!cBtn.interactable) return;
        }
        else
        {
            TMP_InputField tInput = this.gameObject.GetComponent<TMP_InputField>();
            if (tInput != null)
            {
                if (!tInput.interactable) return;
            }
            else
            {
                Slider slider = this.gameObject.GetComponent<Slider>();
                if (slider != null)
                {
                    if (!slider.interactable) return;
                }
                else
                {
                    Button btn = this.gameObject.GetComponent<Button>();
                    if (btn != null)
                    {
                        if (!btn.interactable) return;
                    }
                }
            }
        }
        if (login)
        {
            switch (select)
            {
                case Login.Select.Start:
                    login.m_goStart_Select = this.gameObject;
                    break;
                case Login.Select.Create:
                    login.m_goCreate_Select = this.gameObject;
                    break;
                case Login.Select.CreateGender:
                    login.m_goCreate_Gender = this.gameObject;
                    break;
                case Login.Select.CreateAge:
                    login.m_goCreate_Age = this.gameObject;
                    break;
                case Login.Select.CreateWork:
                    login.m_goCreate_Work = this.gameObject;
                    break;
                case Login.Select.Login:
                    login.m_goLogin_Select = this.gameObject;
                    break;
                case Login.Select.Loggedin:
                    login.m_goLoggedin_Select = this.gameObject;
                    break;
                case Login.Select.Play:
                    login.m_goPlay_Select = this.gameObject;
                    break;
                case Login.Select.PlayReplay:
                    login.m_goPlay_Replay = this.gameObject;
                    break;
                case Login.Select.Option:
                    login.m_goOption_Select = this.gameObject;
                    break;
                case Login.Select.OptionKeyboard:
                    login.m_goOption_Keyboard = this.gameObject;
                    break;
                case Login.Select.OptionGamepad:
                    login.m_goOption_Gamepad = this.gameObject;
                    break;
                case Login.Select.Account:
                    login.m_goAccount_Select = this.gameObject;
                    break;
                case Login.Select.Dialog:
                    login.m_goDialog_Select = this.gameObject;
                    break;
                case Login.Select.Gamemode:
                    login.m_goGamemode_Select = this.gameObject;
                    break;
            }
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
    }

    // オブジェクトの範囲内からマウスポインタが出た際に呼び出されます。
    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
