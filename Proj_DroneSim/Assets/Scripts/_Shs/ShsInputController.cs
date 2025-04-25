using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yns;

static public class ShsInputController
{
    //================================================
    // [///]
    //================================================
    //
    public enum TouchMode
    {
        Move,
        PinchInOut,
    }

    //
    public enum TouchInfo
    {
        // タッチなし
        None = 99,

        // 以下は UnityEngine.TouchPhase の値に対応
        // タッチ開始
        Began = 0,
        // タッチ移動
        Moved = 1,
        // タッチ静止
        Stationary = 2,
        // タッチ終了
        Ended = 3,
        // タッチキャンセル
        Canceled = 4,

        // タッチが2, ピンチインアウト開始
        PinchInOutBegan = 5,
        // タッチが2, ピンチインアウト
        PinchInOut = 6,
    }

    //================================================
    // [///]
    //================================================
    // 入力取得
    static public TouchInfo GetTouch(bool maltiTapOn = true)
    {
        bool isEditor = true;
#if UNITY_EDITOR
#elif UNITY_ANDROID || UNITY_IOS
            isEditor = false;
#endif

        if (isEditor)
        {
            if (Input.GetMouseButtonDown(0)) { return TouchInfo.Began; }
            if (Input.GetMouseButton(0)) { return TouchInfo.Moved; }
            if (Input.GetMouseButtonUp(0)) { return TouchInfo.Ended; }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                // マルチタップに対応するか
                if (maltiTapOn)
                {
                    if (Input.touchCount == 1)
                    {
                        return (TouchInfo)((int)Input.GetTouch(0).phase);
                    }
                    else if (Input.touchCount == 2)
                    {
                        if (Input.GetTouch(0).phase == TouchPhase.Began ||
                            Input.GetTouch(1).phase == TouchPhase.Began)
                        {
                            return TouchInfo.PinchInOutBegan;
                        }
                        else
                        {
                            return TouchInfo.PinchInOut;
                        }
                    }
                    else
                    {
                        return TouchInfo.None;
                    }
                }
                else
                {
                    return (TouchInfo)((int)Input.GetTouch(0).phase);
                }
            }
        }

        return TouchInfo.None;
    }

    // 入力取得
    static public TouchInfo GetTouch_malti2(int touchNo)
    {
        bool isEditor = true;
#if UNITY_EDITOR
#elif UNITY_ANDROID || UNITY_IOS
            isEditor = false;
#endif

        if (isEditor)
        {
            if (Input.GetMouseButtonDown(0)) { return TouchInfo.Began; }
            if (Input.GetMouseButton(0)) { return TouchInfo.Moved; }
            if (Input.GetMouseButtonUp(0)) { return TouchInfo.Ended; }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                return (TouchInfo)((int)Input.GetTouch(touchNo).phase);
            }
        }

        return TouchInfo.None;
    }

    // タッチポジションを取得
    static public Vector3 GetTouchPosition(int _touchCount = 0)
    {
        bool isEditor = true;
#if UNITY_EDITOR
#elif UNITY_ANDROID || UNITY_IOS
            isEditor = false;
#endif

        if (isEditor)
        {
            TouchInfo touch = GetTouch();
            if (touch != TouchInfo.None) { return Input.mousePosition; }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(_touchCount);
                Vector3 pos = Vector3.zero;
                pos.x = touch.position.x;
                pos.y = touch.position.y;
                return pos;
            }
        }
        return Vector3.zero;
    }
}