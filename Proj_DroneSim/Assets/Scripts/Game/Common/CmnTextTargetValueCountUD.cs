using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CmnTextTargetValueCountUD : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    //
    private bool m_IsUpdate = false;
    //
    private Text m_Text = null;
    //
    private float m_Time = 0.0f;
    //
    private long m_StartNum = 0;
    //
    private long m_OneAddNum = 0;
    //
    private long m_EndNum = 0;
    //
    private string m_FrontAddText = "";
    //
    private string m_BackAddText = "";

    private long m_AddNum = 0;

    private float m_EndTime = 1.5f;

    //================================================
    // [///]
    //================================================
    void Start()
    {
    }

    void Update()
    {
        if (m_IsUpdate)
        {
            m_Time += Time.deltaTime;
            m_AddNum = (int)(m_OneAddNum * (m_Time / m_EndTime));
            bool isEnd = true;
            if (m_StartNum < m_EndNum)
            {
                isEnd = m_StartNum + m_AddNum >= m_EndNum;
            }
            else if (m_StartNum > m_EndNum)
            {
                isEnd = m_StartNum + m_AddNum <= m_EndNum;
            }
            if (isEnd)
            {
                m_AddNum = m_EndNum - m_StartNum;
                m_IsUpdate = false;
            }
            // m_Text.text = m_FrontAddText + (m_StartNum + m_AddNum).ToString() + m_BackAddText;
        }
        else
        {
            //m_Text.text = m_FrontAddText + m_EndNum.ToString() + m_BackAddText;
            Destroy(this);
        }
    }

    //================================================
    // [///]
    //================================================
    /// <summary>
    /// _time 経過時間,
    /// _startNum=開始数値,
    /// _addNum=加算数値,
    /// _endNum=最終数値,
    /// _endTime=終了までの時間
    /// _frontAddText=数値より前に追加するテキスト,
    /// _backAddText=数値より後に追加するテキスト,
    /// </summary>
    public void SetTextCountUpDown(long _startNum, long _addNum, long _endNum, float _endTime, string _frontAddText = "", string _backAddText = "")
    {
        //
        m_StartNum = _startNum;
        m_OneAddNum = _addNum;
        m_EndNum = _endNum;
        m_FrontAddText = _frontAddText;
        m_BackAddText = _backAddText;

        // テキスト取得
        m_Text = GetComponent<Text>();
        // m_Text.text = m_FrontAddText + _startNum.ToString() + m_BackAddText;

        m_Time = 0.0f;
        m_AddNum = 0;
        m_EndTime = _endTime;
        m_IsUpdate = true;
    }

}