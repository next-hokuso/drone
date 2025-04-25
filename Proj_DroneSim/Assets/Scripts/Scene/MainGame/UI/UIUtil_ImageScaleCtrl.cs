using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ShsInputController;

public class UIUtil_ImageScaleCtrl : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    private Vector3 m_BaseScale = Vector3.zero;
    private float m_ScaleValue = 0.0f;

    // 起動フラグ
    private bool m_IsUpdate = false;
    private bool m_IsProc = true;

    // ピンチインアウト
    private float m_PinchInOutCheckDist = 0.0f;
    private float m_PinchInOutStDist = 0.0f;
    private bool m_IsProcPinchInOut = false;

    // ダブルタップ判定
    private bool m_IsSingleTapCheck = false;
    private float m_SingleTapCheckTimer = 0.0f;

    private bool m_IsDoubleTapCheck = false;
    private float m_DoubleTapCheckTimer = 0.0f;

    // マルチスワイプでの移動
    private bool m_IsPinchInOutCheck = false;
    private float m_TapRate = 0.0f;
    private Vector3 m_StTapPos = Vector3.zero;
    private Vector3 m_StTapNowPos = Vector3.zero;

    private RectTransform m_RectT = null;

    // 移動判定チェック
    private bool m_IsMove = false;
    private IsCheckDrawArea m_IsCheckTap = null;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
    }
    public void ProcInitialize()
    {
        m_RectT = GetComponent<RectTransform>();
        m_IsCheckTap = gameObject.AddComponent<IsCheckDrawArea>();
    }

    private void Update()
    {
        if (m_IsUpdate && m_IsProc)
        {
            // ピンチインアウトのズームインアウト
            {
                if (GetTouch() == TouchInfo.Began)
                {
                    m_TapRate = 0.0f;
                    m_StTapPos = GetTouchPosition();
                    m_StTapNowPos = transform.localPosition;
                    m_IsProcPinchInOut = true;

                    // タップチェック
                    TapCheck();
                    //m_IsDoubleTapCheck = false;

                    if (m_IsCheckTap.IsDrawArea())
                    {
                        m_IsMove = true;
                    }
                }
                else if(GetTouch() == TouchInfo.PinchInOutBegan)
                {
                    m_PinchInOutCheckDist = Vector2.Distance(GetTouchPosition(), GetTouchPosition(1));
                    m_PinchInOutStDist = m_ScaleValue;
                    Debug.Log("---PinchInOutBegan---");
                }
                else if (GetTouch() == TouchInfo.PinchInOut)
                {
                    // タッチ位置の移動後、長さを再測し、前回の距離からの相対値を取る。
                    float newDist = Vector2.Distance(GetTouchPosition(), GetTouchPosition(1));                
                    Debug.Log("---PinchInOut---");

                    // ピンチインアウト
                    if (m_IsPinchInOutCheck)
                    {
                        m_ScaleValue = m_PinchInOutStDist - (m_PinchInOutCheckDist - newDist) / 200.0f;
                        if (m_ScaleValue < m_BaseScale.x)
                        {
                            m_ScaleValue = m_BaseScale.x;
                        }
                        // 2倍サイズまで
                        if (m_ScaleValue > m_BaseScale.x * 2.0f)
                        {
                            m_ScaleValue = m_BaseScale.x * 2.0f;
                        }
                        transform.localScale = Vector3.one * m_ScaleValue;
                    }
                    else
                    {
                        // ピンチインアウト移行
                        if(newDist > Screen.width * 0.2f)
                        {
                            m_TapRate = 0.0f;
                            m_IsPinchInOutCheck = true;
                            m_PinchInOutCheckDist = Vector2.Distance(GetTouchPosition(), GetTouchPosition(1));
                            m_PinchInOutStDist = m_ScaleValue;
                        }
                        else
                        {
                            // マルチスワイプの移動
                            ProcMove();
                        }
                    }
                }
                if(GetTouch() == TouchInfo.Moved || GetTouch() == TouchInfo.Stationary)
                {
                    if (!m_IsPinchInOutCheck)
                    {
                        // マルチスワイプの移動
                        ProcMove();
                    }
                }
                if(GetTouch() == TouchInfo.Ended)
                {
                    m_IsProcPinchInOut = false;
                    m_IsPinchInOutCheck = false;
                    m_TapRate = 0.0f;
                    m_IsMove = false;

                    m_IsSingleTapCheck = false;
                }
            }
            // ドリーイン・ドリーアウト
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                // fovテスト
                m_ScaleValue += scroll;
                if (m_ScaleValue < m_BaseScale.x)
                {
                    m_ScaleValue = m_BaseScale.x;
                }
                // 2倍サイズまで
                if (m_ScaleValue > m_BaseScale.x * 2.0f)
                {
                    m_ScaleValue = m_BaseScale.x * 2.0f;
                }
                transform.localScale = Vector3.one * m_ScaleValue;

                // editorテスト
                //if(GetTouch()== TouchInfo.Began)
                //{
                //    m_TapRate = 0.0f;
                //    m_StTapPos = GetTouchPosition();
                //    m_StTapNowPos = transform.localPosition;
                //    m_IsPinchInOutCheck = true;
                //}
                //if (GetTouch() == TouchInfo.Moved)
                //{
                //    ProcMove();
                //}
            }

            // ダブルタップチェック
            if (m_IsDoubleTapCheck)
            {
                m_DoubleTapCheckTimer += Time.deltaTime;
                if (m_DoubleTapCheckTimer > 0.3f)
                {
                    m_DoubleTapCheckTimer = 0.0f;
                    m_IsDoubleTapCheck = false;
                }
            }
            else if (m_IsSingleTapCheck)
            {
                m_SingleTapCheckTimer += Time.deltaTime;
                if (m_SingleTapCheckTimer > 0.3f)
                {
                    m_SingleTapCheckTimer = 0.0f;
                    m_IsSingleTapCheck = false;
                }
            }
        }
    }
    private void ProcMove()
    {
        if (!m_IsMove) { return; }

        // マルチスワイプの移動処理
        Vector3 tapPos = GetTouchPosition();

        // 現在位置
        Vector3 nowPos = Vector3.zero;
        // タップした位置からスライドした方向と大きさ
        Vector3 direction = Vector3.zero;
        nowPos = transform.position;
        direction = ((nowPos + (tapPos - m_StTapPos)) - nowPos).normalized;
        direction.z = direction.y;

        // 最大半径確認 画面横範囲 約3/10
        float maxCheckDistance = (Screen.width * 0.99f);//0.15f);// 0.19f);//0.28f);

        // 移動距離設定
        float stTapToTapDist = Vector3.Distance(tapPos, m_StTapPos);
        // 移動量算
        m_TapRate = stTapToTapDist / maxCheckDistance;

        Vector3 dir = (tapPos - m_StTapPos);
        Vector3 pos = dir.normalized * (m_TapRate * maxCheckDistance);
        pos = m_StTapNowPos + pos;
        pos.z = 0.0f;

        // 移動制限
        Vector2 size = Vector2.zero;
        size.x = m_BaseScale.x * m_RectT.sizeDelta.x;
        size.y = m_BaseScale.y * m_RectT.sizeDelta.y;
        // 左
        if (pos.x < size.x * -1.0f)
        {
            pos.x = size.x * -1.0f;
        }
        // 右
        else if (pos.x > size.x)
        {
            pos.x = size.x;
        }
        // 上
        if (pos.y > size.y)
        {
            pos.y = size.y;
        }
        else if (pos.y < size.y * -1.0f)
        {
            pos.y = size.y * -1.0f;
        }
        transform.localPosition = pos;

        //m_TapRate *= dir.x > 0.0f ? 1.0f : -1.0f;
    }

    // スケール調整後設定
    public void SetBaseScale()
    {
        m_BaseScale = transform.localScale;
        m_ScaleValue = m_BaseScale.x;
        transform.localPosition = Vector3.zero;
        m_IsUpdate = true;
        m_IsProcPinchInOut = false;
    }
    public void SetEndUpdate()
    {
        m_IsUpdate = false;
        m_IsProcPinchInOut = false;
    }

    // ピンチインアウト中か
    public bool IsPinchInOut()
    {
        return m_IsProcPinchInOut;
    }

    // サイズを元に戻す
    private IEnumerator ProcScaling()
    {
        m_IsProc = false;

        float timer = 0.0f;
        float endTime = 0.2f;

        float nowVal = m_ScaleValue;
        float addVal = m_BaseScale.x - nowVal;

        Vector3 nowPos = transform.localPosition;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > endTime) { break; }

            // サイズ
            m_ScaleValue = nowVal + (timer / endTime * addVal);
            transform.localScale = Vector3.one * m_ScaleValue;

            // 位置
            transform.localPosition = Vector3.Lerp(nowPos, Vector3.zero, timer / endTime);
            yield return null;
        }
        m_ScaleValue = m_BaseScale.x;
        transform.localScale = Vector3.one * m_ScaleValue;
        transform.localPosition = Vector3.zero;
        m_IsProc = true;
    }

    //================================================
    // [///]
    //================================================
    public void OnClick_TapCheck()
    {
        //// ダブルタップ判定
        //if (m_IsDoubleTapCheck)
        //{
        //    if (m_DoubleTapCheckTimer <= 0.3f)
        //    {
        //        m_IsDoubleTapCheck = false;
        //        m_DoubleTapCheckTimer = 0.0f;
        //
        //        // サイズを元に戻す
        //        StartCoroutine(ProcScaling());
        //        return;
        //    }
        //}
        //else
        //{
        //    m_IsDoubleTapCheck = true;
        //    return;
        //}
    }
    private void TapCheck()
    {
        {
            // ダブルタップ判定
            if (m_IsDoubleTapCheck)
            {
                if (m_DoubleTapCheckTimer <= 0.3f)
                {
                    m_IsDoubleTapCheck = false;
                    m_DoubleTapCheckTimer = 0.0f;

                    // サイズを元に戻す
                    StartCoroutine(ProcScaling());
                    return;
                }
            }
            else
            {
                m_IsDoubleTapCheck = true;
                return;
            }
        }
    }
}
