using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class ShUIBtnCtrl : ShUIBaseCtrl
{
    public Button m_Btn = null;

    public override void ProcInitialize()
    {
        m_Btn = GetComponent<Button>();
    }
}
