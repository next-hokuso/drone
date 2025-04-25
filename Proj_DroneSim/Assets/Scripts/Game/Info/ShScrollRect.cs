using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShScrollRect : ScrollRect
{
    //================================================
    // [///]
    //================================================
    public bool m_IsDragUpdateStop = false;
    public bool m_IsBeginDrag = false;

    //================================================
    // [///]
    //================================================
    // Scroll View でマウスボタン押下時
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
    }

    // マウスボタン押下中に初めてマウスを押下した時
    public override void OnBeginDrag(PointerEventData eventData)
    {
        m_IsBeginDrag = true;
        base.OnBeginDrag(eventData);
    }

    // マウスボタン押下中にマウスを動かしている間
    public override void OnDrag(PointerEventData eventData)
    {
        if (m_IsDragUpdateStop) { return; }
        base.OnDrag(eventData);
    }

    // マウスボタンを離した時
    public override void OnEndDrag(PointerEventData eventData)
    {
        m_IsBeginDrag = false;
        base.OnEndDrag(eventData);
    }

    // 取得
    public bool IsBeginDrag()
    {
        return m_IsBeginDrag;
    }
}
