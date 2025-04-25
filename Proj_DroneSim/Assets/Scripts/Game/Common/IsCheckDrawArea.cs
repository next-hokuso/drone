using UnityEngine;
using UnityEngine.EventSystems;

public class IsCheckDrawArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //================================================
    // [///]
    //================================================
    // 描けるか
    private bool m_IsDrawArea = false;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
    }

    //================================================
    // [///] public
    //================================================
    public bool IsDrawArea()
    {
        return m_IsDrawArea;
    }

    //================================================
    // [///] CallBack
    //================================================
    // ボタン上にポインターが入ったら呼ばれる
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_IsDrawArea = true;
    }

    // ボタン範囲からポインターが離れたら呼ばれる
    public void OnPointerExit(PointerEventData eventData)
    {
        m_IsDrawArea = false;
    }
}
