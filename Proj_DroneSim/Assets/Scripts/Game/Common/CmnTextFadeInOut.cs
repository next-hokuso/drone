using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 指定時間でフェードイン/アウトする
public class CmnTextFadeInOut : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    private enum Action
    {
        None,
        FadeInAndMove,
        Wait,
        FadeOut,
    }
    private Action m_Action = Action.None;
    //
    private bool m_IsUpdate = false;
    //
    private float m_Time = 0.0f;
    //
    private float m_EndTime = 0.0f;
    //
    private TextMesh m_Text = null;
    //
    private float m_EndNum = 0.0f;
    //
    private float m_StartNum = 0.0f;
    //
    private bool m_IsGODestroy = false;

    // 
    private Vector3 m_StPos = Vector3.zero;
    private Vector3 m_EndPos = Vector3.zero;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnDestroy()
    {
        if (m_IsUpdate)
        {
            Color col = m_Text.color;
            col.a = m_EndNum;
            m_Text.color = col;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsUpdate)
        {
            switch (m_Action)
            {
                case Action.FadeInAndMove:
                    m_Time += Time.deltaTime;
                    if (m_Time > m_EndTime)
                    {
                        m_Time = 0.0f;
                        m_Action = Action.Wait;
                    }
                    {
                        float add = m_StartNum + (m_Time / m_EndTime);
                        Color col = m_Text.color;
                        col.a = add;
                        m_Text.color = col;

                        transform.position = Vector3.Lerp(m_StPos, m_EndPos, m_Time / m_EndTime);
                    }
                    break;

                case Action.Wait:
                    m_Time += Time.deltaTime;
                    if (m_Time > 0.5f)
                    {
                        m_Action = Action.FadeOut;
                    }
                    break;

                case Action.FadeOut:
                    {
                        Color col = m_Text.color;
                        col.a = m_EndNum;
                        m_Text.color = col;
                    }
                    m_IsUpdate = false;
                    if (m_IsGODestroy)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        Destroy(this);
                    }
                    break;
            }
        }
    }

    //================================================
    // [///]
    //================================================
    /// <summary>
    /// _time 経過時間,
    /// _startAlp=開始数値,
    /// _endNum=最終数値,
    /// _isGODestroy=GameObjectごと削除するか
    /// </summary>
    public void SetTextFadeOut(float _time, float _startNum, float _endNum, bool _isGODestroy = false)
    {
        //
        m_Text = gameObject.GetComponent<TextMesh>();
        Color col = m_Text.color;
        col.a = _startNum;
        m_Text.color = col;

        //
        m_StartNum = _startNum;
        m_EndTime = _time;
        m_EndNum = _endNum;
        m_IsGODestroy = _isGODestroy;

        m_IsUpdate = true;
        m_Time = 0.0f;
        m_Action = Action.FadeInAndMove;
    }
    public void SetTextFadeOut(float _time, Vector3 _stPos, Vector3 _endPos, bool _isFadeIn = true, bool _isGODestroy = false)
    {
        //
        m_Text = gameObject.GetComponent<TextMesh>();
        Color col = m_Text.color;
        col.a = _isFadeIn ? 0.0f : 1.0f;
        m_Text.color = col;

        //
        m_StartNum = _isFadeIn ? 0.0f : 1.0f;
        m_EndTime = _time;
        m_EndNum = _isFadeIn ? 1.0f : 0.0f ;
        m_IsGODestroy = _isGODestroy;

        // pos
        m_StPos = _stPos;
        m_EndPos = _endPos;
        transform.position = _stPos;

        m_IsUpdate = true;
        m_Time = 0.0f;
        m_Action = Action.FadeInAndMove;
    }

}