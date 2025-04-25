using System.Collections;
using UnityEngine;
using System.IO;
using Game;

public class CmnCoinAddAnim : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    enum Proc
    {
        None,
        DelayTime,
        ScaleAdd,
        Move,
        End,
    }
    private Proc m_Proc = Proc.None;

    //
    private float m_Timer = 0.0f;
    //
    private float m_DelayTime = 0.0f;
    //
    private float m_ScaleTime = 0.0f;
    //
    private float m_MoveTime = 0.0f;
    //
    private GameObject m_CompleteMethodTarget = null;
    //
    private string m_CompleteMethodName = "";
    //
    private Vector3 m_TargetPos = Vector3.zero;

    // 今回用
    private bool m_IsFirst = false;
    public bool GetIsFirst() { return m_IsFirst; }
    private int m_SetMoney = 0;
    public int GetSetMoney() { return m_SetMoney; }
    private bool m_IsVibe = false;
    public bool GetIsVibe() { return m_IsVibe; }


    private Vector3 m_StScale = Vector3.zero;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_Proc)
        {
            case Proc.None:
                break;

            case Proc.DelayTime:
                if(m_Timer < m_DelayTime)
                {
                    m_Timer += Time.deltaTime;
                }
                else
                {
                    m_Timer = 0.0f;
                    m_Proc = Proc.ScaleAdd;
                }
                break;

            case Proc.ScaleAdd:
                if (m_Timer < m_ScaleTime)
                {
                    m_Timer += Time.deltaTime;
                    transform.localScale = m_StScale * (m_Timer / m_ScaleTime);
                }
                else
                {
                    transform.localScale = m_StScale;

                    // 移動
                    Hashtable hash = iTween.Hash(
                    "position",     m_TargetPos,
                    "time",         m_ScaleTime,
                    "easeType",     iTween.EaseType.linear,
                    "oncomplete",   m_CompleteMethodName,
                    "oncompletetarget", m_CompleteMethodTarget,
                    "oncompleteparams", gameObject
                    );
                    iTween.MoveTo(gameObject, hash);

                    m_Proc = Proc.End;
                }
                break;

            case Proc.End:
                break;
        }
    }

    //
    public void SetCoinAnim(float _delayTime, float _scaleTime, Vector3 _targetPos, GameObject _completeTarget, string _completeMethodName,
                            bool _isFirst, int _targeMoney, bool _isVibe)
    {
        m_DelayTime = _delayTime;
        m_ScaleTime = 0.4f;
        m_MoveTime = 1.0f - _delayTime - 0.4f;
        m_TargetPos = _targetPos;
        m_CompleteMethodTarget = _completeTarget;
        m_CompleteMethodName = _completeMethodName;

        m_IsFirst = _isFirst;
        m_SetMoney = _targeMoney;
        m_IsVibe = _isVibe;

        m_StScale = transform.localScale;
        transform.localScale = Vector3.zero;

        m_Proc = Proc.DelayTime;
    }
}
