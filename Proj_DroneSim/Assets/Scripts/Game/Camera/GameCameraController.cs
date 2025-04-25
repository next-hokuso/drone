using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ShsInputController;

public class GameCameraController : MonoBehaviour
{
    //================================================
    // [///]
    //================================================

    //================================================
    // [///]
    //================================================
    public enum CameraMode
    {
        Free,   // 自由
        Chase,  // 追跡
        Lock,   // 固定
        LookAt, // 定点からの視点
        Chase_RotFollw, // 追跡+回転追従
    }

    //================================================
    // [///]
    //================================================
    [SerializeField] private float m_Dist = 8.0f;
    [SerializeField] private float m_AddHeightDist = 0.0f;
    [SerializeField] private float m_AddHorizontalDist = 0.0f;
    [SerializeField] private float m_AddPosZDist = 0.0f;
    [SerializeField] private float m_RotX = 20.0f;
    [SerializeField] private float m_RotY = 0.0f;
    [SerializeField] private float m_FoV = 0.0f;
    [SerializeField] private CameraMode m_CameraMode = CameraMode.Chase;
    [SerializeField] private Transform m_Target = null;
    private float m_OrthorSize = 0.0f;
    [SerializeField] private float m_AddOrthorValue = 0.0f;
    [SerializeField] private float m_ZoomValue = 0.0f;

    // this
    private Camera m_MyCamera = null;

    // リセット保持
    private float m_StDist = 0.0f;
    private float m_StRotX = 0.0f;
    private float m_StRotY = 0.0f;
    private float m_StAddHorizontalDist = 0.0f;
    private float m_StAddHeightDist = 0.0f;
    private float m_StAddPosZDist = 0.0f;
    private float m_StOrthorSize = 0.0f;
    private float m_StFoV = 0.0f;
    private Transform m_StTarget = null;

    private Vector3 DefaultTargetPos = new Vector3(0.0f, 0.5f, 0.0f);
    private Vector3 m_TargetPos = Vector3.zero;
    private Vector3 m_EyePos = Vector3.zero;
    private Vector3 m_OffsetHeight = Vector3.zero;

    // change save
    private bool m_IsChangeDist = false;
    private float m_ChangeEndDist = 0.0f;

    // ピンチイン/アウト
    //private float m_PinchInOutCheckDist = 0.0f;
    //private float m_PinchInOutStDist = 0.0f;

    // Tutorial
    private bool m_IsTutorial = false;
    private Vector3 m_TutorialStPos = Vector3.zero;

    // target
    private GameCameraMoveCtrl m_MoveCtrl = null;

    // 
    private Vector3 m_OffsetPos = Vector3.zero;

    // lock
    private Vector3 m_LockPos = Vector3.zero;
    private bool m_IsLockUpdate = false;

    // iTween用 hash
    private Hashtable m_iTweenHash;

    // 定点カメラでのズーム用
    private bool m_isFixedZoomIn = false;
    private bool m_isFixedZoomOut = false;
    private float m_FixedZoomTimer = 0.0f;
    private float m_FixedDurationTime = 0.0f;

    private bool m_IsModeHuman = false;

    // 
    private GameObject m_LookAtTarget = null;

    //================================================
    // [///]
    //================================================
    // Use this for initialization
    void Start()
    {
        m_StDist = m_Dist;
        m_StRotX = m_RotX;
        m_StRotY = m_RotY;
        m_StAddHorizontalDist = m_AddHorizontalDist;
        m_StAddPosZDist = m_AddPosZDist;
        m_StAddHeightDist = m_AddHeightDist;
        m_StFoV = m_FoV;
        m_MyCamera = GetComponent<Camera>();
        m_StOrthorSize = m_MyCamera.orthographicSize;
        m_StTarget = m_Target;
        ResetCameraParam();
        // 視点・参照点設定
        CalcEyePos(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // @カメラの移動はLateUpdateにて！！！！！！
    void LateUpdate()
    {
        if (m_IsTutorial)
        {
            return;
        }

        if (m_CameraMode == CameraMode.Lock)
        {
            // 固定
            if (m_IsLockUpdate)
            {
                transform.position = m_LockPos + m_OffsetPos;
            }
        }
        else if(m_CameraMode == CameraMode.LookAt)
        {
            // 視点
            {
                // 参照点から縦横移動
                float dist = m_Dist - m_ZoomValue;
                m_EyePos.y = dist * Mathf.Sin(Mathf.Deg2Rad * m_RotX);
                m_EyePos.z = -dist * Mathf.Cos(Mathf.Deg2Rad * m_RotX) * Mathf.Cos(Mathf.Deg2Rad * m_RotY);
                m_EyePos.x = -dist * Mathf.Sin(Mathf.Deg2Rad * m_RotY) * Mathf.Cos(Mathf.Deg2Rad * m_RotX);
            }
            transform.position = m_EyePos + m_TargetPos;
            // 参照点の方向へ向ける
            transform.LookAt(m_TargetPos);

            // DroneSim用処理
            {
                // 手前に来た場合
                float temp = m_LookAtTarget.transform.position.z - 0.4f;
                if (temp < transform.position.z)
                {
                    // 奥行加算
                    transform.position += (temp - m_EyePos.z) * Vector3.forward;
                }
            }
            // 高さ加算
            transform.position += m_AddHeightDist * Vector3.up;

            // 横加算
            transform.position += m_AddHorizontalDist * Vector3.right + Vector3.right * (m_AddPosZDist * transform.forward.x);

            // 奥行加算
            transform.position += m_AddPosZDist * Vector3.forward;
            //transform.position += Vector3.forward * (m_AddPosZDist * transform.forward.z);

            // @debug orthorgraphicsize
            {
                m_MyCamera.orthographicSize = m_StOrthorSize + m_AddOrthorValue;
            }

            // ターゲットに向ける
            transform.LookAt(m_LookAtTarget.transform.position);
        }
        else
        {
            if (m_CameraMode == CameraMode.Free || m_Target == null)
            {
                // 自由移動
            }
            else
            {
                // 目標追跡

                // 定点カメラでのズームイン
                if (m_isFixedZoomIn)
                {
                    m_FixedZoomTimer += (Time.deltaTime / m_FixedDurationTime);
                    m_FixedZoomTimer = Mathf.Min(m_FixedZoomTimer, 1.0f);
                    m_TargetPos = Vector3.Lerp(m_TargetPos, m_Target.position, m_FixedZoomTimer);
                }
                else
                // 定点カメラでのズームアウト
                if (m_isFixedZoomOut)
                {
                    m_FixedZoomTimer += (Time.deltaTime / m_FixedDurationTime);
                    m_FixedZoomTimer = Mathf.Min(m_FixedZoomTimer, 1.0f);
                    m_TargetPos = Vector3.Lerp(m_Target.position, m_StTarget.position, m_FixedZoomTimer);
                }
                else
                {
                    if (m_CameraMode == CameraMode.Chase_RotFollw)
                    {
                        Vector3 rot = m_Target.transform.eulerAngles;
                        m_RotY = rot.y;
                    }

                    // 参照点オブジェクトの位置取得
                    m_TargetPos = m_Target.position;
                }
                // Z移動をここで対応
                //m_TargetPos.z = m_AddPosZDist;
                //m_Target.position = m_TargetPos;

                //m_TargetPos.y = 0.0f;

                //// ピンチインアウトのズームインアウト
                //{
                //    if (GetTouch() == TouchInfo.PinchInOutBegan)
                //    {
                //        m_PinchInOutCheckDist = Vector2.Distance(GetTouchPosition(), GetTouchPosition(1));
                //
                //        m_PinchInOutStDist = m_Dist;
                //    }
                //    else if (GetTouch() == TouchInfo.PinchInOut)
                //    {
                //        // タッチ位置の移動後、長さを再測し、前回の距離からの相対値を取る。
                //        float newDist = Vector2.Distance(GetTouchPosition(), GetTouchPosition(1));
                //
                //        m_Dist = m_PinchInOutStDist + (m_PinchInOutCheckDist - newDist) / 100.0f;
                //
                //        m_Dist = Mathf.Clamp(m_Dist, DistLimitMin, 1000.0f);// 20.0f);
                //    }
                //}
                //// ドリーイン・ドリーアウト
                //{
                //    float scroll = Input.GetAxis("Mouse ScrollWheel");
                //    m_Dist += scroll;
                //    if (m_Dist < DistLimitMin)
                //    {
                //        m_Dist = DistLimitMin;
                //    }
                //    else if (m_Dist > 1020.0f)
                //    {
                //        m_Dist = 1020.0f;
                //    }
                //}
            }

            // 更新を他で呼び出す カクツキ対処テスト
            // 視点・参照点設定
            CalcEyePos(Vector3.zero);
        }
    }

    // 更新を他で呼び出す カクツキ対処テスト
    public void SetCalcPos(Vector3 nowPos)
    {
        CalcEyePos(nowPos);
    }

    //================================================
    // [///]
    //================================================
    // 視点・参照点設定
    private void CalcEyePos(Vector3 nowPos)
    {
        // 視点
        {
            // 参照点から縦横移動
            float dist = m_Dist - m_ZoomValue;
            m_EyePos.y = dist * Mathf.Sin(Mathf.Deg2Rad * m_RotX);
            m_EyePos.z = -dist * Mathf.Cos(Mathf.Deg2Rad * m_RotX) * Mathf.Cos(Mathf.Deg2Rad * m_RotY);
            m_EyePos.x = -dist * Mathf.Sin(Mathf.Deg2Rad * m_RotY) * Mathf.Cos(Mathf.Deg2Rad * m_RotX);
        }
        transform.position = m_EyePos + m_TargetPos;
        // 参照点の方向へ向ける
        transform.LookAt(m_TargetPos);


        // 高さ加算
        transform.position += m_AddHeightDist * Vector3.up;

        // 横加算
        transform.position += m_AddHorizontalDist * Vector3.right + Vector3.right * (m_AddPosZDist * transform.forward.x);

        // 奥行加算
        transform.position += m_AddPosZDist * Vector3.forward;
        //transform.position += Vector3.forward * (m_AddPosZDist * transform.forward.z);

        if(m_CameraMode == CameraMode.Chase_RotFollw)
        {
            // 回転設定
            transform.localEulerAngles = Vector3.right * m_RotX + Vector3.up * m_RotY;
        }

        // @debug orthorgraphicsize
        {
            m_MyCamera.orthographicSize = m_StOrthorSize + m_AddOrthorValue;
        }
    }

    //================================================
    // [///] カメラ距離/角度変更 UpdateMethod
    //================================================
    private void OnUpdateDistOnly(float dist)
    {
        m_Dist = dist;
    }
    private void OnUpdateRotYOnly(float rot)
    {
        m_RotY = rot;
    }
    private void OnUpdateRotXOnly(float rot)
    {
        m_RotX = rot;
    }
    private void OnUpdateAddHeightOnly(float height)
    {
        m_AddHeightDist = height;
    }
    private void OnUpdateAddHorizontalOnly(float height)
    {
        m_AddHorizontalDist = height;
    }
    private void OnUpdateAddPosZOnly(float height)
    {
        m_AddPosZDist = height;
    }
    private void OnUpdateOrthoGraphics(float val)
    {
        m_AddOrthorValue = val;
    }
    private void OnUpdateZoomValue(float val)
    {
        m_ZoomValue = val;
    }


    //================================================
    // [///] カメラ距離/角度変更
    //================================================
    // ---------------------------------
    // 完成演出カメラ
    public void SetHumanCamera()
    {
        m_Dist = 0.1f;
        m_AddHeightDist = 0.0f;
        m_AddHorizontalDist = 0.0f;
        m_AddPosZDist = 0.0f;
        m_RotX = 0.0f;
        m_RotY = 0.0f;
        m_FoV = 60.0f;

        transform.localEulerAngles = Vector3.zero;
        m_IsModeHuman = true;

        if (!m_MyCamera) m_MyCamera = GetComponent<Camera>();
        m_MyCamera.fieldOfView = m_FoV;
    }
    // ---------------------------------
    // パラメータリセット
    public void SetParamReset()
    {
        m_FoV = m_StFoV;
        if (!m_MyCamera) m_MyCamera = GetComponent<Camera>();
        m_MyCamera.fieldOfView = m_FoV;

        m_IsModeHuman = false;

        ResetCameraParam();
    }
    // ---------------------------------
    // 完成演出カメラ(調整前)
    public void SetPrevCamra()
    {
        m_Dist = 8.0f;
        m_StAddHeightDist = 0.5f;
        m_AddHeightDist = 0.5f;
        m_RotX = 20.0f;
        m_FoV = 60.0f;

        if (!m_MyCamera) m_MyCamera = GetComponent<Camera>();
        m_MyCamera.fieldOfView = m_FoV;
    }
    // ---------------------------------
    // クリアカメラ
    public void SetCameraAngle(float time)
    {
        float afterRotY = -360.0f;

        // RotY
        m_iTweenHash = iTween.Hash(
            "from", 0.0f,
            "to", afterRotY,
            "time", time,
            "easeType", iTween.EaseType.easeInOutSine,
            "onupdate", "OnUpdateRotYOnly",
            "onupdatetarget", gameObject
        );
        iTween.ValueTo(gameObject, m_iTweenHash);
    }
    // ---------------------------------
    // クリアカメラ
    public void SetPrevCameraAngle(float time)
    {
        float afterRotY = 720.0f;

        // RotY
        m_iTweenHash = iTween.Hash(
            "from", 0.0f,
            "to", afterRotY,
            "time", time,
            "easeType", iTween.EaseType.easeOutQuad,
            "onupdate", "OnUpdateRotYOnly",
            "onupdatetarget", gameObject
        );
        iTween.ValueTo(gameObject, m_iTweenHash);
    }
    // ---------------------------------
    // ブースト開始時のズームイン
    public void SetZoomInCamera(float time)
    {
        float afterZoomValue = 70.0f;

        // ZoomValue
        m_iTweenHash = iTween.Hash(
            "from", m_ZoomValue,
            "to", afterZoomValue,
            "time", time,
            "easeType", iTween.EaseType.easeOutQuad,
            "onupdate", "OnUpdateZoomValue",
            "onupdatetarget", gameObject
        );
        iTween.ValueTo(gameObject, m_iTweenHash);
    }
    // ---------------------------------
    // ブースト終了時のズームアウト
    public void SetZoomOutCamera(float time)
    {
        // ZoomValue
        m_iTweenHash = iTween.Hash(
            "from", m_ZoomValue,
            "to", 0.0f,
            "time", time,
            "easeType", iTween.EaseType.easeInQuad,
            "onupdate", "OnUpdateZoomValue",
            "onupdatetarget", gameObject
        );
        iTween.ValueTo(gameObject, m_iTweenHash);
    }
    // ---------------------------------
    // 定点カメラでのズームイン設定
    public void SetFixedZoomIn(bool flag, float duration = 0.0f)
    {
        m_isFixedZoomIn = flag;
        m_FixedZoomTimer = 0.0f;
        m_FixedDurationTime = duration;
    }
    // ---------------------------------
    // 定点カメラでのズームアウト設定
    public void SetFixedZoomOut(bool flag, float duration = 0.0f)
    {
        m_isFixedZoomOut = flag;
        m_FixedZoomTimer = 0.0f;
        m_FixedDurationTime = duration;
    }

    // ---------------------------------
    // 完成演出カメラ
    public void SetLookAtCamera(GameObject target)
    {
        m_LookAtTarget = target;

        m_Dist = 12.5f;
        // m_AddHeightDist = 0.0f;
        // m_AddHorizontalDist = 0.0f;
        // m_AddPosZDist = 0.0f;
         m_RotX = 45.0f;
        // m_RotY = 0.0f;
        // m_FoV = 60.0f;
        // 
        // transform.localEulerAngles = Vector3.zero;
        // 
        // if (!m_MyCamera) m_MyCamera = GetComponent<Camera>();
        // m_MyCamera.fieldOfView = m_FoV;

        m_CameraMode = CameraMode.LookAt;
    }

    // ---------------------------------
    // Endrress
    public void SetLookAtCamera_EndressMode(GameObject target)
    {
        m_LookAtTarget = target;

        m_Dist = 2.5f;
        m_AddHeightDist = 0.65f;
        // m_AddHorizontalDist = 0.0f;
        // m_AddPosZDist = 0.0f;
        m_RotX = 20.0f;
        // m_RotY = 0.0f;
        // m_FoV = 60.0f;
        // 
        // transform.localEulerAngles = Vector3.zero;
        // 
        // if (!m_MyCamera) m_MyCamera = GetComponent<Camera>();
        // m_MyCamera.fieldOfView = m_FoV;

        m_CameraMode = CameraMode.Chase;
    }

    // ---------------------------------
    // Endrress
    public void SetLookAtCamera_TreasureMode(GameObject target)
    {
        m_LookAtTarget = target;

        m_Dist = 2.5f;
        m_AddHeightDist = 0.65f;
        // m_AddHorizontalDist = 0.0f;
        // m_AddPosZDist = 0.0f;
        m_RotX = 20.0f;
        // m_RotY = 0.0f;
        // m_FoV = 60.0f;
        // 
        // transform.localEulerAngles = Vector3.zero;
        // 
        // if (!m_MyCamera) m_MyCamera = GetComponent<Camera>();
        // m_MyCamera.fieldOfView = m_FoV;

        m_CameraMode = CameraMode.Chase;
    }


    //================================================
    // [///]
    //================================================
    // カメラモード設定
    public void SetCametaMode(CameraMode _mode)
    {
        m_CameraMode = _mode;
        if(_mode == CameraMode.Lock)
        {
            m_LockPos = transform.position;
        }
    }

    // カメラパラメータリセット
    public void ResetCameraParam(bool isOrthographicReset = true)
    {
        // カメラモードも追従に戻す ヒエラルキー設定が外れるためコメントアウト
        // SetCametaMode(CameraMode.Chase);
        m_IsLockUpdate = false;

        m_Dist = m_StDist;
        m_RotX = m_StRotX;
        m_RotY = m_StRotY;
        m_OffsetHeight = Vector3.zero;
        m_AddHorizontalDist = m_StAddHorizontalDist;
        m_AddHeightDist = m_StAddHeightDist;
        m_AddPosZDist = m_StAddPosZDist;
        m_FoV = m_StFoV;
        m_OrthorSize = m_StOrthorSize;
        if (m_CameraMode == CameraMode.Free)
        {
            m_TargetPos = DefaultTargetPos;
        }

        ResetCameraMoveTarget();
    }

    // 追加高さ
    public float SetAddHeightValue(float value)
    {
        if (m_IsModeHuman)
        {
            // -5.0f ～5.0f
            float minVal = -1.5f;
            float maxVale = 2.0f;
            m_AddHeightDist = minVal + ((maxVale - (minVal)) * value);
            return m_AddHeightDist;
        }
        else
        {
            // -5.0f ～5.0f
            float minVal = 1.75f;
            float maxVale = 7.75f;
            m_AddHeightDist = minVal + ((maxVale - (minVal)) * value);
            return m_AddHeightDist;
        }
    }
    public float SetAddHeight(float value)
    {
        m_AddHeightDist = value;
        return m_AddHeightDist;
    }
    public float GetAddHeightValue()
    {
        if (m_IsModeHuman)
        {
            // 0にして最大値までの幅
            // -5.0f ～ 5.0f
            float minVal = -1.5f;
            float maxVale = 2.0f;
            return (m_AddHeightDist - minVal) / (maxVale - minVal);
        }
        else
        {
            // 0にして最大値までの幅
            // -5.0f ～ 5.0f center:4.75
            float minVal = 1.75f;
            float maxVale = 7.75f;
            return (m_AddHeightDist - minVal) / (maxVale - minVal);
        }
    }
    public float GetAddHeight()
    {
        return m_AddHeightDist;
    }
    // 追加高さ
    public float SetAddPosZValue(float value)
    {
        // -5.0f ～5.0f
        float minVal = -999.0f;
        float maxVale = 999.0f;
        m_AddPosZDist = minVal + ((maxVale - (minVal)) * value);
        return m_AddPosZDist;
    }
    public float SetAddPosZ(float value)
    {
        m_AddPosZDist = value;
        return m_AddPosZDist;
    }
    public float GetAddPosZValue()
    {
        // 0にして最大値までの幅
        // -5.0f ～ 5.0f
        float minVal = -999.0f;
        float maxVale = 999.0f;
        return (m_AddPosZDist - minVal) / (maxVale - minVal);
    }
    public float GetAddPosZ()
    {
        return m_AddPosZDist;
    }
    // 回転X
    public float SetRotXValue(float value)
    {
        if (m_IsModeHuman)
        {
            float minVal = -45.0f;
            float maxVale = 45.0f;
            m_RotX = minVal + ((maxVale - minVal) * value);
            return m_RotX;
        }
        else
        {
            float minVal = 0.0f;
            float maxVale = 60.0f;
            m_RotX = minVal + ((maxVale - minVal) * value);
            return m_RotX;
        }

    }
    public float SetRotX(float value)
    {
        if (m_IsModeHuman)
        {
            float minVal = -60.0f;
            float maxVale = 60.0f;
            m_RotX = Mathf.Clamp(value, minVal, maxVale);
        }
        else
        {
            float minVal = 0.0f;
            float maxVale = 60.0f;
            m_RotX = Mathf.Clamp(value, minVal, maxVale);
        }
        //m_RotX = value;
        return m_RotX;
    }
    public float GetRotXValue()
    {
        // // 0にして最大値までの幅
        // float minVal = 0.0f;
        // float maxVale = 90.0f;
        // return (m_RotX - minVal) / (maxVale - minVal);
        // 0にして最大値までの幅
        if (m_IsModeHuman)
        {
            float minVal = -45.0f;
            float maxVale = 45.0f;
            return (m_RotX - minVal) / (maxVale - minVal);
        }
        else
        {
            float minVal = 0.0f;
            float maxVale = 60.0f;
            return (m_RotX - minVal) / (maxVale - minVal);

        }
    }
    public float GetRotX()
    {
        return m_RotX;
    }
    // 回転Y
    public float SetRotYValue(float value)
    {
        float minVal = 0.0f;
        float maxVale = 360.0f;
        m_RotY = minVal + ((-maxVale - minVal) * value);
        return m_RotY;
    }
    public float SetRotY(float value)
    {
        m_RotY = value;
        return m_RotY;
    }
    public float GetRotYValue()
    {
        // 0にして最大値までの幅
        float minVal = 0.0f;
        float maxVale = 360.0f;
        return (m_RotY - minVal) / (-maxVale - minVal);
    }
    public float GetRotY()
    {
        return m_RotY;
    }

    // 距離
    public float SetCameraDistValue(float value)
    {
        // value=スライダーvalue
        m_Dist = value * 500.0f;
        return m_Dist;
    }
    public float SetCameraDist(float value)
    {
        // value=スライダーvalue
        m_Dist = value;
        return m_Dist;
    }
    public float GetDistValue()
    {
        return m_Dist / 500.0f;
    }
    public float GetCameraDist()
    {
        return m_Dist;
    }
    public float GetDist()
    {
        if (m_IsChangeDist)
        {
            return m_ChangeEndDist;
        }
        return m_Dist;
    }
    public float GetStDist()
    {
        return m_StDist;
    }

    // fov
    public float SetFoVValue(float value)
    {
        if (!m_MyCamera)
        {
            m_MyCamera = GetComponent<Camera>();
        }
        // 0 ～ 100.0f
        m_FoV = 0.0f + ((150.0f - 0.0f) * value);
        m_MyCamera.fieldOfView = m_FoV;
        return m_FoV;
    }
    public float SetFoV(float value)
    {
        m_FoV = value;
        m_MyCamera.fieldOfView = m_FoV;
        return m_FoV;
    }
    public float GetFovValue()
    {
        // 0にして最大値までの幅
        // 0 ～ 360
        return (m_FoV - 0.0f) / (150.0f - 0.0f);
    }
    public float GetFoV()
    {
        return m_FoV;
    }

    // orthorgraphicsサイズ
    public float SetOrthoGraphicsSize(float value)
    {
        m_MyCamera.orthographicSize = m_StOrthorSize + m_StOrthorSize * value;
        return m_MyCamera.orthographicSize;
    }
    public float GetOrthoGraphicsSizeValue()
    {
        return (m_MyCamera.orthographicSize - m_StOrthorSize) / (m_StOrthorSize);
    }
    public float GetOrthoGraphicsSize()
    {
        return m_MyCamera.orthographicSize;
    }

    //
    public float GetStAddHeightDist()
    {
        return m_StAddHeightDist;
    }

    // ---------------------------------------------------------------
    // 追従対処関連
    public void SetTarget(GameObject _go)
    {
        m_Target = _go != null ? _go.transform : null;
    }
    public void ResetTarget()
    {
        m_Target = m_StTarget;
    }
    public Transform GetTarget()
    {
        return m_Target;
    }

    public float GetStRotX()
    {
        return m_StRotX;
    }

    // カメラ移動スクリプト関連
    /// <summary>
    /// スクリプトをつけなおしているためロック関係も初期化される
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isReset"></param>
    public void SetCameraMove(GameObject player,bool isReset=false)
    {
        if (m_Target)
        {
            if (m_Target.GetComponent<GameCameraMoveCtrl>())
            {
                Destroy(m_Target.GetComponent<GameCameraMoveCtrl>());
            }
            m_MoveCtrl = m_Target.gameObject.AddComponent<GameCameraMoveCtrl>();
            m_MoveCtrl.SetInfo(player, isReset);
        }
    }
    // ゲーム開始時
    public void SetCameraCtrlSetStart()
    {
        if (m_MoveCtrl)
        {
            m_MoveCtrl.SetStartInfo();
        }
    }
    // X移動設定
    public void SetIsLockCameraMoveX(bool isLock)
    {
        if (m_MoveCtrl)
        {
            m_MoveCtrl.SetIsLockMoveX(isLock);
        }
    }
    // Y移動設定
    public void SetIsLockCameraMoveY(bool isLock)
    {
        if (m_MoveCtrl)
        {
            m_MoveCtrl.SetIsLockMoveY(isLock);
        }
    }
    // リセット
    public void ResetCameraMoveTarget()
    {
        if (m_MoveCtrl)
        {
            m_MoveCtrl.SetReset();
        }
    }
    // 停止
    public void StopCameraMoveTarget()
    {
        if (m_MoveCtrl)
        {
            m_MoveCtrl.SetProcEnd();
        }
    }
    // ゲームオーバー時
    public void SetGameOverCam()
    {
        if (m_MoveCtrl)
        {
            m_MoveCtrl.SetGameOverInfo();
        }
    }
    // ゴール ゲームオーバー時
    public void SetGoalGameOverCam()
    {
        if (m_MoveCtrl)
        {
            m_MoveCtrl.SetGoalEnd();
        }
    }
    // 一時停止
    public void SetCameraMoveStop()
    {
        if (m_MoveCtrl)
        {
            m_MoveCtrl.SetUpdate(false);
        }
    }
    // 停止
    public void SetCameraMoveUpdate(bool isUpdate)
    {
        if (m_MoveCtrl)
        {
            m_MoveCtrl.SetUpdate(isUpdate);
        }
    }

    // ターゲット位置更新
    public void UpdateCameraMove()
    {
        if (m_MoveCtrl)
        {
            m_MoveCtrl.ProcCameraTargetUpdate();
        }
    }

    // 強制視点設定
    public void ForceSetPos(Vector3 pos)
    {
        if (m_MoveCtrl)
        {
            m_MoveCtrl.ForcePosSet(pos);
        }
    }

    // 視点主取得
    public Vector3 GetMoveTargetPos()
    {
        if (m_MoveCtrl)
        {
            return m_MoveCtrl.transform.position;
        }
        return Vector3.zero;
    }
}
