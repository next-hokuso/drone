using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;

public class LicenseCtrl : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    private const int FontSize = 52;
    private const int OneRowHeightSize = 70;         // fontsize:52から設定62->70
    private const int OneRowTitleHeightSize = 70;    // fontsize:52から+30設定82->70

    //================================================
    // [///]
    //================================================
    private ScrollRect m_ScrollRect = null;
    // スライドビューの埋め込みテキストレイアウト設定
    private VerticalLayoutGroup m_ViewportLayoutSetting = null;
    // タイトル/テキスト
    private Text m_TitleText = null;
    private Text m_MainText = null;

    //================================================
    // [///]
    //================================================
    void Start()
    {
    }
    void Update()
    {
        
    }

    //================================================
    // [///]
    //================================================
    public void SetScrollViewInfo(Transform scrollRect)
    {
        //StartCoroutine(SetInfo(scrollRect));
    }
    //private IEnumerator SetInfo(Transform scrollRect)
    //{
    //    // 読み込み待ちしてから設定
    //    while (true)
    //    {
    //        if (GD_LicenseText.IsLoading()) { break; }
    //        yield return null;
    //    }
    //
    //    // 変数セッティング
    //    m_ScrollRect = scrollRect.GetComponent<ScrollRect>();
    //
    //    Transform content = m_ScrollRect.transform.Find("Viewport").Find("Content");
    //    m_ViewportLayoutSetting = content.GetComponent<VerticalLayoutGroup>();
    //
    //    m_TitleText = m_ScrollRect.transform.parent.transform.Find("Text_Title").GetComponent<Text>();
    //    m_MainText = content.Find("Text").GetComponent<Text>();
    //
    //    // スクロールビューテキスト設定
    //    TextData titleData = GD_LicenseText.GetTextData(TextId.Text_LicenseTitle);
    //    TextData txtData = GD_LicenseText.GetTextData(TextId.Text_LicenseText);
    //    int rowHeight = 0;
    //
    //    // テキスト設定
    //    m_TitleText.text = titleData.m_Text;
    //    m_MainText.text = txtData.m_Text;
    //
    //    // alignment
    //    m_TitleText.alignment = TextAnchor.UpperLeft + (int)(titleData.m_TextAlignment);
    //    m_MainText.alignment = TextAnchor.MiddleLeft + (int)(txtData.m_TextAlignment);
    //
    //    // rectT height変更
    //    RectTransform titleRectT = m_TitleText.GetComponent<RectTransform>();
    //    rowHeight = titleData.m_Text.Length <= 0 ? 0 : OneRowTitleHeightSize;
    //    titleRectT.sizeDelta = new Vector2(300, rowHeight);
    //
    //    RectTransform textRect = m_MainText.GetComponent<RectTransform>();
    //    int num = txtData.m_Text.IndexOfAny(("\n").ToCharArray());
    //    textRect.sizeDelta = new Vector2(0, OneRowHeightSize * num);
    //
    //    // rectview変更
    //    rowHeight += OneRowHeightSize * num + OneRowHeightSize;
    //    content.GetComponent<LayoutElement>().minHeight = rowHeight;
    //}
}
