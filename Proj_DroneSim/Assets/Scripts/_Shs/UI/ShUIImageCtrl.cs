using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class ShUIImageCtrl : ShUIBaseCtrl
{
    private Image m_Image = null;

    public override void ProcInitialize()
    {
        m_Image = GetComponent<Image>();
        if(m_Image == null)
        {
            m_Image = GetComponentInChildren<Image>();
        }
    }
    public Image GetImage()
    {
        return m_Image;
    }
}
