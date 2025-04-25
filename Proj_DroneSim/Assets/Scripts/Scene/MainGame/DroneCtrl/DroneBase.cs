using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static ShsInputController;
using UnityEngine.InputSystem.DualShock;

public class DroneBase : MonoBehaviour
{
    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================
    // 最大移動量
    protected const float MoveLimitX = 3.6f;
    // フリック判定値角度
    protected const float FlickCheckAngle = 45.0f;

    // 移動制限
    protected float MoveLimitZ_Plus = 8.2f;
    protected float MoveLimitZ_Minus = -13.2f;
    protected float MoveLimitX_PlusMinus = 13.8f;

    protected float MoveLimitY_Plus = 15.0f;
    protected float MoveLimitY_Minus = 0.0f;

    // ログ保存時間上限
    protected const float LogSaveLimitTime = 60.0f * 5.0f;

    public enum InputDirId
    {
        Up,
        Down,
        Right,
        Left,
    }

    public enum InputActionKey
    {
        None = 0,

        Null,           // null : コントローラー接続が無い等...
        Replay_End,     // リプレイの終了

        // 共通キー
        LStick_X = 50,
        LStick_Y,
        RStick_X,
        RStick_Y,

        Key_Pressed_LeftUp,
        Key_Pressed_LeftDown,
        Key_Pressed_LeftL,
        Key_Pressed_LeftR,
        Key_Pressed_RightUp,
        Key_Pressed_RightDown,
        Key_Pressed_RightL,
        Key_Pressed_RightR,

        Key_Release_LeftUp,
        Key_Release_LeftDown,
        Key_Release_LeftL,
        Key_Release_LeftR,
        Key_Release_RightUp,
        Key_Release_RightDown,
        Key_Release_RightL,
        Key_Release_RightR,


        // Gamepad
        Gamepad_LStick_X = 100,
        Gamepad_LStick_Y,
        Gamepad_RStick_X,
        Gamepad_RStick_Y,
        // Gamepad Pressed
        // Gamepad Release

        // Keyboard Pressed
        Keyboard_Pressed_A = 300,
        Keyboard_Pressed_D,
        Keyboard_Pressed_W,
        Keyboard_Pressed_S,
        Keyboard_Pressed_LeftArrow,
        Keyboard_Pressed_RightArrow,
        Keyboard_Pressed_UpArrow,
        Keyboard_Pressed_DownArrow,
        // Keyboard Release
        Keyboard_Release_A = 400,
        Keyboard_Release_D,
        Keyboard_Release_W,
        Keyboard_Release_S,
        Keyboard_Release_LeftArrow,
        Keyboard_Release_RightArrow,
        Keyboard_Release_UpArrow,
        Keyboard_Release_DownArrow,

        // UI
        UI_LStick_X = 600,
        UI_LStick_Y,
        UI_RStick_X,
        UI_RStick_Y,


        // ボタン機能
        Proc_VisionSensor_On = 1000,
        Proc_VisionSensor_Off,
        // フリップ
        Proc_FripWait_On,
        Proc_FripWait_Off,
        Proc_Frip_Forward,
        Proc_Frip_Back,
        Proc_Frip_Right,
        Proc_Frip_Left,
        // メトロノーム
        Proc_Metronome_On,
        Proc_Metronome_Off,
        // 速度
        Proc_Spd_Change1,
        Proc_Spd_Change2,
        Proc_Spd_Change3,
        // ヘッドレス
        Proc_Headless_On,
        Proc_Headless_Off,
        // 自動離着陸
        Proc_AutoFly_On,
        Proc_AutoFly_On_MonterOnWait,
        Proc_AutoFly_Off,
        Proc_AutoFly_On_MonterOnWait_Off,
        Proc_AutoLanding_On,
    }

    // 
    protected enum DroneActionId
    {
        None,
        Wait,
        MonterOn,
        Fly,
    }
    protected DroneActionId m_CurrentDroneActionId = DroneActionId.None;

    //================================================
    // [///] 速度関連
    //================================================
    #region SpdSetting
    // 上昇/下降移動速度
    protected float m_AddPosYSpd = 0.0f;
    // 限界値
    protected const float AddPosYLimitSpd = 12.0f;
    // 加速値 限界値を2で割り = 0.5秒で最高速
    protected const float AddPosYAccele = AddPosYLimitSpd * 2.0f;
    // 減衰量
    protected const float PosYDampRate = 0.1f;

    // 回転
    protected float m_AddRotYSpd = 0.0f;
    // 限界値
    protected const float AddRotYLimitSpd = 135.0f;
    // 加速値 限界値を2で割り = 0.5秒で最高速
    protected const float AddRotYAccele = AddRotYLimitSpd * 2.0f;
    protected const float RotYDampRate = 0.15f;

    // 移動X
    protected float m_AddPosXSpd = 0.0f;
    // 限界値
    protected const float AddPosXLimitSpd = 5.0f;
    // 加速値 限界値を2で割り = 0.5秒で最高速
    protected const float AddPosXAccele = AddPosXLimitSpd * 2.0f;
    // 減衰量
    protected const float PosXDampRate = 0.12f;

    // 移動Z
    protected float m_AddPosZSpd = 0.0f;
    // 限界値
    protected const float AddPosZLimitSpd = 5.0f;
    // 加速値 限界値を2で割り = 0.5秒で最高速
    protected const float AddPosZAccele = AddPosZLimitSpd * 2.0f;
    // 減衰量
    protected const float PosZDampRate = 0.12f;

    // 傾きX(前方)
    protected float m_AddRotX = 0.0f;
    // 限界値
    protected const float AddRotXLimit = 14.0f;
    // 加速値 限界値を2で割り = 0.5秒で最高速
    protected const float AddRotXAccele = AddRotXLimit * 7.5f;
    // 減衰量
    protected const float RosXDampRate = 0.15f;

    // 傾きZ(横)
    protected float m_AddRotZ = 0.0f;
    // 限界値
    protected const float AddRotZLimit = 14.0f;
    // 加速値 限界値を2で割り = 0.5秒で最高速
    protected const float AddRotZAccele = AddRotZLimit * 7.5f;
    // 減衰量
    protected const float RotZDampRate = 0.15f;
    #endregion

    //================================================
    // [///]
    //================================================
    // 初期位置
    protected Vector3 m_StPos = Vector3.zero;
    protected Vector3 m_StScale = Vector3.zero;
    protected Vector3 m_PrevPos = Vector3.zero;

    protected Rigidbody m_Rigid = null;

    //  ドローンモデル
    protected GameObject m_Model = null;
    //  プロペラ
    protected DronePropellerCtrl m_PropellerCtrl = null;

    // 行動
    protected enum ActionProc
    {
        None = 0,
        InGame,
        Result,
    }
    [SerializeField]
    protected ActionProc m_ActionProc = ActionProc.None;

    // タップ
    protected Vector3 m_BeginTapPosition = Vector3.zero;// 初期タップした際のタップ位置

    protected IsCheckDrawArea m_IsTapCheck = null;

    // 入力関連
    protected bool m_IsLeftStickUse = false;
    protected bool m_IsRightStickUse = false;
    protected IsCheckDrawArea m_IsLeftStickUITapCheck = null;
    protected IsCheckDrawArea m_IsRightStickUITapCheck = null;

    // 移動保持
    protected Vector3 m_MoveTemp = Vector3.zero;

    // フラグ ニュートラルチェック
    protected bool m_IsNeutral = false;

    // 入力保持
    protected float m_PrevInput_ValLX = 0.0f;
    protected float m_PrevInput_ValLY = 0.0f;

    protected float m_PrevInput_ValRX = 0.0f;
    protected float m_PrevInput_ValRY = 0.0f;

    protected float m_Input_ValueLX = 0.0f;
    protected float m_Input_ValueLY = 0.0f;
    protected float m_Input_ValueRX = 0.0f;
    protected float m_Input_ValueRY = 0.0f;

    protected InputActionKey m_PrevInputActionKey = InputActionKey.None;


    // ログ保存用
    protected List<AppData.ReplayInfo> m_ReplayInfoList = null;

    // 通常/リプレイで相違する場合のため現在の接続モードを保持
    protected AppData.ConnectM m_CurrentConnectMode = AppData.ConnectM.Mode1;


    // 移動限界
    protected bool m_IsMoveLimitCheck = false;
    // FlyingSE
    protected bool m_IsFlyingSEPlay = false;

    // 機能関連
    public bool m_IsHeadless = false;

    // 速度変更関連
    protected int m_CurrentSpdChangeId = 0;
    protected float m_SpdChangeMag = 1.0f;

    // 機能関連の連続変更のカバー
    protected bool m_IsChangeCheck = false;

    //================================================
    // [///]
    //================================================
    protected virtual void Start()
    {
    }
    protected virtual void OnDestroy()
    {
    }

    // 生成時設定
    public virtual void SetStartInfo()
    {
        m_ReplayInfoList = new List<AppData.ReplayInfo>();

        // Phymat setting
        ProcChange_PhysicsMaterialBounciness();

        // 操作設定
        {
            var controllers = Input.GetJoystickNames();
            bool isExistController = false;
            if (controllers.Length > 0)
            {
                isExistController = !(controllers[0] == "");
            }

            if (isExistController)
            {
                ShInputManager.I.SetInputSetting(ShInputManager.InputSetting.Controller);
            }
            else
            {
                ShInputManager.I.SetInputSetting(ShInputManager.InputSetting.Keyboard);
            }
            ShInputManager.I.GetInputCanvas().SetInputSettingUI();
        }

        // タッチ用UI設定
        m_IsLeftStickUITapCheck = ShInputManager.I.GetInputCanvas().GetPadLCtrl().GetIsCheckTapArea();
        m_IsRightStickUITapCheck = ShInputManager.I.GetInputCanvas().GetPadRCtrl().GetIsCheckTapArea();
    }
    // 開始
    public virtual void SetStart()
    {
        m_StPos = transform.position;
        m_StScale = transform.localScale;

        // 物理
        m_Rigid = GetComponent<Rigidbody>();

        // モデル
        m_Model = transform.Find("Model").gameObject;

        // プロペラ
        m_PropellerCtrl = gameObject.AddComponent<DronePropellerCtrl>();
        m_PropellerCtrl.Setup();

        // 接続モードの取得
        m_CurrentConnectMode = AppData.GetCurrentConnectMode();

        m_ActionProc = ActionProc.InGame;
        m_CurrentDroneActionId = DroneActionId.Wait;
    }
    // スタート演出後のステージ処理開始
    public virtual void SetStart_ProcOn()
    {
        m_ActionProc = ActionProc.InGame;
    }

    protected virtual void Update()
    {
    }

    protected virtual void LateUpdate()
    {
        m_IsChangeCheck = false;
        m_IsMoveLimitCheck = false;
    }

    // ゲームオーバー処理
    public virtual void SetGameOver()
    {
        // 
        m_ActionProc = ActionProc.None;

        // 自分非表示
    }

    // リセット(初期化対応)
    public virtual void ProcReset()
    {
        transform.position = m_StPos;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = m_StScale;
        m_ActionProc = ActionProc.None;
        m_CurrentDroneActionId = DroneActionId.None;
    }

    protected virtual void SetProcFripSetting(InputDirId id)
    {
    }
    protected virtual void ProcFripUpdate()
    {
    }

    //================================================
    // [///] インプット後挙動設定共通関連
    //================================================
    #region DroneAction
    // 左スティックY軸挙動
    protected virtual void DroneAction_LeftStickY()
    {
    }
    // 左スティックX軸挙動
    protected virtual void DroneAction_LeftStickX()
    {
    }
    // 右スティックY軸挙動
    protected virtual void DroneAction_RightStickY()
    {
    }
    protected virtual void DroneAction_RightStickX()
    {
    }
    #endregion

    //================================================
    // [///] インプット関連
    //================================================
    // 操作
    #region InputProc
    // キーボード操作
    protected virtual void ProcInput_Keyboard()
    {
        // 毎フレーム更新 ゲームパッド情報
        ShInputManager.I.Setup_Keyboard();

        // nullチェック
        if (ShInputManager.I.IsNull_Keyboard())
        {
            m_PrevInputActionKey = InputActionKey.Null;
            AddReplayLog(InputActionKey.Null);
            return;
        }

        // 入力
        {
            // 左スティック
            // X
            float leftValX = 0.0f;
            // Y
            float leftValY = 0.0f;

            // A Key
            if (ShInputManager.I.IsKey_Pressed_LeftL())
            {
                leftValX = -1.0f;
            }
            // D Key
            if (ShInputManager.I.IsKey_Pressed_LeftR())
            {
                leftValX = 1.0f;
            }
            // W Key
            if (ShInputManager.I.IsKey_Pressed_LeftUp())
            {
                leftValY = 1.0f;
            }
            // S Key
            if (ShInputManager.I.IsKey_Pressed_LeftDown())
            {
                leftValY = -1.0f;
            }

            // 右スティック
            // X
            float rightValX = 0.0f;
            // Y
            float rightValY = 0.0f;

            // <- Key
            if (ShInputManager.I.IsKey_Pressed_RightL())
            {
                rightValX = -1.0f;
            }
            // -> Key
            if (ShInputManager.I.IsKey_Pressed_RightR())
            {
                rightValX = 1.0f;
            }
            // ↑ Key
            if (ShInputManager.I.IsKey_Pressed_RightUp())
            {
                rightValY = 1.0f;
            }
            // ↓ Key
            if (ShInputManager.I.IsKey_Pressed_RightDown())
            {
                rightValY = -1.0f;
            }

            m_Input_ValueLX = leftValX;
            m_Input_ValueLY = leftValY;
            m_Input_ValueRX = rightValX;
            m_Input_ValueRY = rightValY;

            // UI更新
            ShInputManager.I.GetInputCanvas().GetPadLCtrl().SetPad(
                m_Input_ValueLX, m_Input_ValueLY);
            // UI更新
            ShInputManager.I.GetInputCanvas().GetPadRCtrl().SetPad(
                m_Input_ValueRX, m_Input_ValueRY);

            // 挙動実行
            DroneAction_LeftStickX();
            DroneAction_LeftStickY();
            DroneAction_RightStickX();
            DroneAction_RightStickY();

            leftValX  = m_Input_ValueLX;
            leftValY  = m_Input_ValueLY;
            rightValX = m_Input_ValueRX;
            rightValY = m_Input_ValueRY;

            {
                //if (m_PrevInput_ValLX != leftValX)
                {
                    // 入力相違
                    if (ShInputManager.I.m_IsPressed_LeftL != ShInputManager.I.IsKey_Pressed_LeftL())
                    {
                        if (!ShInputManager.I.m_IsPressed_LeftL)
                        {
                            AddReplayLog(InputActionKey.Key_Pressed_LeftL, leftValX);
                        }
                        else
                        {
                            AddReplayLog(InputActionKey.Key_Release_LeftL, leftValX);
                        }
                    }
                    // 入力相違
                    if (ShInputManager.I.m_IsPressed_LeftR != ShInputManager.I.IsKey_Pressed_LeftR())
                    {
                        if (!ShInputManager.I.m_IsPressed_LeftR)
                        {
                            AddReplayLog(InputActionKey.Key_Pressed_LeftR, leftValX);
                        }
                        else
                        {
                            AddReplayLog(InputActionKey.Key_Release_LeftR, leftValX);
                        }
                    }
                    m_PrevInput_ValLX = leftValX;
                }
                //if (m_PrevInput_ValLY != leftValY)
                {
                    // 入力相違
                    if (ShInputManager.I.m_IsPressed_LeftUp != ShInputManager.I.IsKey_Pressed_LeftUp())
                    {
                        if (!ShInputManager.I.m_IsPressed_LeftUp)
                        {
                            AddReplayLog(InputActionKey.Key_Pressed_LeftUp, leftValY);
                        }
                        else
                        {
                            AddReplayLog(InputActionKey.Key_Release_LeftUp, leftValY);
                        }
                    }
                    // 入力相違
                    if (ShInputManager.I.m_IsPressed_LeftDown != ShInputManager.I.IsKey_Pressed_LeftDown())
                    {
                        if (!ShInputManager.I.m_IsPressed_LeftDown)
                        {
                            AddReplayLog(InputActionKey.Key_Pressed_LeftDown, leftValY);
                        }
                        else
                        {
                            AddReplayLog(InputActionKey.Key_Release_LeftDown, leftValY);
                        }
                    }
                    m_PrevInput_ValLY = leftValY;
                }

                //if (m_PrevInput_ValRX != rightValX)
                {
                    // 入力相違
                    if (ShInputManager.I.m_IsPressed_RightL != ShInputManager.I.IsKey_Pressed_RightL())
                    {
                        if (!ShInputManager.I.m_IsPressed_RightL)
                        {
                            AddReplayLog(InputActionKey.Key_Pressed_RightL, rightValX);
                        }
                        else
                        {
                            AddReplayLog(InputActionKey.Key_Release_RightL, rightValX);
                        }
                    }
                    // 入力相違
                    if (ShInputManager.I.m_IsPressed_RightR != ShInputManager.I.IsKey_Pressed_RightR())
                    {
                        if (!ShInputManager.I.m_IsPressed_RightR)
                        {
                            AddReplayLog(InputActionKey.Key_Pressed_RightR, rightValX);
                        }
                        else
                        {
                            AddReplayLog(InputActionKey.Key_Release_RightR, rightValX);
                        }
                    }
                    m_PrevInput_ValRX = rightValX;
                }
                //if (m_PrevInput_ValRY != rightValY)
                {
                    // 入力相違
                    if (ShInputManager.I.m_IsPressed_RightUp != ShInputManager.I.IsKey_Pressed_RightUp())
                    {
                        if (!ShInputManager.I.m_IsPressed_RightUp)
                        {
                            AddReplayLog(InputActionKey.Key_Pressed_RightUp, rightValY);
                        }
                        else
                        {
                            AddReplayLog(InputActionKey.Key_Release_RightUp, rightValY);
                        }
                    }
                    // 入力相違
                    if (ShInputManager.I.m_IsPressed_RightDown != ShInputManager.I.IsKey_Pressed_RightDown())
                    {
                        if (!ShInputManager.I.m_IsPressed_RightDown)
                        {
                            AddReplayLog(InputActionKey.Key_Pressed_RightDown, rightValY);
                        }
                        else
                        {
                            AddReplayLog(InputActionKey.Key_Release_RightDown, rightValY);
                        }
                    }
                    m_PrevInput_ValRY = rightValY;
                }
                ShInputManager.I.SetChangedIsPressed();
            }
        }
    }
    // コントローラー
    protected virtual void ProcInput_Controller()
    {
        // 毎フレーム更新 ゲームパッド情報
        ShInputManager.I.Setup_Gamepad();

        // nullチェック
        if (ShInputManager.I.IsNull_Gamepad())
        {
            AddReplayLog(InputActionKey.Null);
            return;
        }

        // 入力
        {
            // 左スティック
            // X
            float leftValX = ShInputManager.I.GetGamepadStickVal_LeftX();
            // Y
            float leftValY = ShInputManager.I.GetGamepadStickVal_LeftY();

            // 右スティック
            // X
            float rightValX = ShInputManager.I.GetGamepadStickVal_RightX();
            // Y
            float rightValY = ShInputManager.I.GetGamepadStickVal_RightY();

            m_Input_ValueLX = leftValX;
            m_Input_ValueLY = leftValY;
            m_Input_ValueRX = rightValX;
            m_Input_ValueRY = rightValY;

            // UI更新
            ShInputManager.I.GetInputCanvas().GetPadLCtrl().SetPad(
                m_Input_ValueLX, m_Input_ValueLY);
            // UI更新
            ShInputManager.I.GetInputCanvas().GetPadRCtrl().SetPad(
                m_Input_ValueRX, m_Input_ValueRY);

            // 挙動実行
            DroneAction_LeftStickX();
            DroneAction_LeftStickY();
            DroneAction_RightStickX();
            DroneAction_RightStickY();

            leftValX = m_Input_ValueLX;
            leftValY = m_Input_ValueLY;
            rightValX = m_Input_ValueRX;
            rightValY = m_Input_ValueRY;

            // 入力チェック
            {
                // 入力量が違う場合
                if (m_PrevInput_ValLX != leftValX)
                {
                    AddReplayLog(InputActionKey.Gamepad_LStick_X, leftValX);
                    m_PrevInput_ValLX = leftValX;
                }
                if (m_PrevInput_ValLY != leftValY)
                {
                    AddReplayLog(InputActionKey.Gamepad_LStick_Y, leftValY);
                    m_PrevInput_ValLY = leftValY;
                }

                if (m_PrevInput_ValRX != rightValX)
                {
                    AddReplayLog(InputActionKey.Gamepad_RStick_X, rightValX);
                    m_PrevInput_ValRX = rightValX;
                }
                if (m_PrevInput_ValRY != rightValY)
                {
                    AddReplayLog(InputActionKey.Gamepad_RStick_Y, rightValY);
                    m_PrevInput_ValRY = rightValY;
                }
            }
        }
    }
    // タッチ
    protected virtual void SetTouch()
    {
        bool isTouch0 = true;
        bool isTouch1 = true;

        // 複数タッチ処理
        for (int i = 0; i < 2; ++i)
        {
            TouchInfo info = GetTouch_malti2(i);
#if UNITY_EDITOR
            if(i > 0)
            {
                isTouch1 = false;
                // エディターの場合マウスをとっているため0のみ
                continue;
            }
#endif

            // タッチ
            if (info == TouchInfo.Began)
            {
                // UIタップチェック
                if (m_IsLeftStickUITapCheck.IsDrawArea())
                {
                    // 左側のタップか
                    if (GetTouchPosition(i).x < Screen.width / 2.0f)
                    {
                        m_IsLeftStickUse = true;
                    }
                }
                if (m_IsRightStickUITapCheck.IsDrawArea())
                {
                    // 左側のタップか
                    if (GetTouchPosition(i).x > Screen.width / 2.0f)
                    {
                        m_IsRightStickUse = true;
                    }
                }
            }
            else if (info == TouchInfo.Moved ||
                     info == TouchInfo.Stationary)
            {
                // UIタップチェック
                if (m_IsLeftStickUse)
                {
                    //Debug.Log("no : " + i + "  pos : " + GetTouchPosition(i).x + " time + " + ShInputManager.I.GetTimerCtrl().GetTime());
                    // 左側のタップか
                    if (GetTouchPosition(i).x < Screen.width / 2.0f || !m_IsRightStickUse)
                    {
                        ProcTouch_Left(ref m_BeginTapPosition, GetTouchPosition(i));
                    }
                    else
                    {
                        AddReplayLog(InputActionKey.UI_LStick_X);
                        AddReplayLog(InputActionKey.UI_LStick_Y);
                    }
                }
                if (m_IsRightStickUse)
                {
                    //Debug.Log("no : " + i + "  pos : " + GetTouchPosition(i).x + " time + " + ShInputManager.I.GetTimerCtrl().GetTime());
                    // 左側のタップか
                    if (GetTouchPosition(i).x > Screen.width / 2.0f || !m_IsLeftStickUse)
                    {
                        ProcTouch_Right(ref m_BeginTapPosition, GetTouchPosition(i));
                    }
                    else
                    {
                        AddReplayLog(InputActionKey.UI_RStick_X);
                        AddReplayLog(InputActionKey.UI_RStick_Y);
                    }
                }
            }
            else if (info == TouchInfo.Ended)
            {
                // UIタップチェック
                if (m_IsLeftStickUse)
                {
                    // 左側のタップか
                    if (GetTouchPosition(i).x < Screen.width / 2.0f)
                    {
                        m_IsLeftStickUse = false;
                        AddReplayLog(InputActionKey.UI_LStick_X);
                        AddReplayLog(InputActionKey.UI_LStick_Y);
                    }
                }
                if (m_IsRightStickUse)
                {
                    // 左側のタップか
                    if (GetTouchPosition(i).x > Screen.width / 2.0f)
                    {
                        m_IsRightStickUse = false;
                        AddReplayLog(InputActionKey.UI_RStick_X);
                        AddReplayLog(InputActionKey.UI_RStick_Y);
                    }
                }
            }
            else if(info == TouchInfo.None)
            {
                if (i == 0) isTouch0 = false;
                if (i == 1) isTouch1 = false;
            }
        }

        // 両方のタッチが無い場合
        if(!isTouch0 && !isTouch1)
        {
            {
                if ((m_Input_ValueLX == 0.0f && m_Input_ValueLY == 0.0f) || m_IsLeftStickUse)
                {
                    m_IsLeftStickUse = false;
                    AddReplayLog(InputActionKey.UI_LStick_X);
                    AddReplayLog(InputActionKey.UI_LStick_Y);

                    // UI更新
                    ShInputManager.I.GetInputCanvas().GetPadLCtrl().SetPad(
                        0.0f, 0.0f);

                   // Debug.Log("LeftNone..........");
                }
                if ((m_Input_ValueRX == 0.0f && m_Input_ValueRY == 0.0f) || m_IsRightStickUse)
                {
                    m_IsRightStickUse = false;
                    AddReplayLog(InputActionKey.UI_RStick_X);
                    AddReplayLog(InputActionKey.UI_RStick_Y);

                    // UI更新
                    ShInputManager.I.GetInputCanvas().GetPadRCtrl().SetPad(
                        0.0f, 0.0f);

                   // Debug.Log("RightNone..........");
                }
            }
        }
    }

    protected void ProcTouch_Left(ref Vector3 stTapPos, Vector3 tapPos)
    {
        // タップx/y軸のまま、移動もx/y軸を使用
        // 移動量の値取得 -----------------------------------------------      
        Vector3 nowPos = transform.position;                        // 現在位置
        Vector3 dir = (tapPos - stTapPos);                          // タップした位置からスライドした方向と大きさ
        //Vector3 direction = ((nowPos + dir) - nowPos).normalized;   // 現在位置からスライドした方向への方向

        // スライド
        float stTapToTapDist = Vector3.Distance(tapPos, stTapPos);  // スライド量
        float maxCheckDistance = (Screen.width * 0.28f);            // スライド判定最大値
                                                                    // (仮想スライドパッドの最大半径確認 : 画面横範囲 約3/10)
                                                                    // float rate = stTapToTapDist / maxCheckDistance;          // スライド量のrate

        // 原点とタップ位置の方向
        // Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, dir.y, 0.0f));
        // ↑ ------------------------------------------------------------

        // 移動/回転処理 -------------------------------------------------
        // 初期タップ位置から5以上離れていない場合移動無し
        if (stTapToTapDist > 1.0f)
        {
            InputVirtualPadUICtrl padL = ShInputManager.I.GetInputCanvas().GetPadLCtrl();

            padL.SetPad_TouchL(tapPos, tapPos.x, tapPos.y);

            // 入力
            {
                // 左スティック
                // X
                float leftValX = padL.GetTouchStickRateX();
                // Y
                float leftValY = padL.GetTouchStickRateY();

                // テスト
                {
                    leftValX = ShInputManager.I.GetStickVal_Samp(leftValX);
                    leftValY = ShInputManager.I.GetStickVal_Samp(leftValY);
                }
                // Yns.YnSys.SetDbgText(leftValX + " " + leftValY);

                m_Input_ValueLX = leftValX;
                m_Input_ValueLY = leftValY;

                // UI更新
                ShInputManager.I.GetInputCanvas().GetPadLCtrl().SetPad(
                    m_Input_ValueLX, m_Input_ValueLY);

                // 挙動実行
                DroneAction_LeftStickX();
                DroneAction_LeftStickY();

                leftValX = m_Input_ValueLX;
                leftValY = m_Input_ValueLY;

                // 入力チェック
                {
                    // 入力量が違う場合
                    if (m_PrevInput_ValLX != leftValX)
                    {
                        AddReplayLog(InputActionKey.UI_LStick_X, leftValX);
                        m_PrevInput_ValLX = leftValX;
                    }
                    if (m_PrevInput_ValLY != leftValY)
                    {
                        AddReplayLog(InputActionKey.UI_LStick_Y, leftValY);
                        m_PrevInput_ValLY = leftValY;
                    }
                }
            }

        }
    }

    protected void ProcTouch_Right(ref Vector3 stTapPos, Vector3 tapPos)
    {
        //
        //if (AppCommon.m_NowLoadSceneNo != (int)AppCommon.LoadSceneNo.ObjView) return;

        // 移動用UIのタップ中か
        //if (!m_IsMoveUITapCheck.IsDrawArea()) { return; }

        // タップx/y軸のまま、移動もx/y軸を使用
        // 移動量の値取得 -----------------------------------------------      
        Vector3 nowPos = transform.position;                        // 現在位置
        Vector3 dir = (tapPos - stTapPos);                          // タップした位置からスライドした方向と大きさ
        //Vector3 direction = ((nowPos + dir) - nowPos).normalized;   // 現在位置からスライドした方向への方向

        // スライド
        float stTapToTapDist = Vector3.Distance(tapPos, stTapPos);  // スライド量
        float maxCheckDistance = (Screen.width * 0.28f);            // スライド判定最大値
                                                                    // (仮想スライドパッドの最大半径確認 : 画面横範囲 約3/10)
                                                                    // float rate = stTapToTapDist / maxCheckDistance;          // スライド量のrate

        // 原点とタップ位置の方向
        // Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, dir.y, 0.0f));
        // ↑ ------------------------------------------------------------

        // 移動/回転処理 -------------------------------------------------
        // 初期タップ位置から5以上離れていない場合移動無し
        if (stTapToTapDist > 1.0f)
        {
            InputVirtualPadUICtrl padR = ShInputManager.I.GetInputCanvas().GetPadRCtrl();
            padR.SetPad_TouchL(tapPos, tapPos.x, tapPos.y);

            // 入力
            {
                // 右スティック
                // X
                float rightValX = padR.GetTouchStickRateX();
                // Y
                float rightValY = padR.GetTouchStickRateY();

                // テスト
                {
                    rightValX = ShInputManager.I.GetStickVal_Samp(rightValX);
                    rightValY = ShInputManager.I.GetStickVal_Samp(rightValY);
                }
                m_Input_ValueRX = rightValX;
                m_Input_ValueRY = rightValY;

                // UI更新
                ShInputManager.I.GetInputCanvas().GetPadRCtrl().SetPad(
                    m_Input_ValueRX, m_Input_ValueRY);

                // 挙動実行
                DroneAction_RightStickX();
                DroneAction_RightStickY();

                rightValX = m_Input_ValueRX;
                rightValY = m_Input_ValueRY;

                // 入力チェック
                {
                    if (m_PrevInput_ValRX != rightValX)
                    {
                        AddReplayLog(InputActionKey.UI_RStick_X, rightValX);
                        m_PrevInput_ValRX = rightValX;
                    }
                    if (m_PrevInput_ValRY != rightValY)
                    {
                        AddReplayLog(InputActionKey.UI_RStick_Y, rightValY);
                        m_PrevInput_ValRY = rightValY;
                    }
                }
            }

        }
    }
    #endregion

    //================================================
    // [///] ミッション関連
    //================================================
    // 前回の位置と今回の位置
    protected bool IsDiffMoveDir(Vector3 prevPos, Vector3 nextMoveDir)
    {
        // 前回の位置と今回の位置から判定
        Vector3 moveDir = (transform.position - prevPos);
        Vector3 temp = transform.InverseTransformPoint(prevPos + moveDir * 2.0f);
        bool isCheck = false;
        if (nextMoveDir.x == 1.0f)
        {
            if (temp.x < 0.0f)
            {
                isCheck = true;
            }
        }
        else if (nextMoveDir.x == -1.0f)
        {
            if (temp.x > 0)
            {
                isCheck = true;
            }
        }
        if (nextMoveDir.z == 1.0f)
        {
            if (temp.z < 0.0f)
            {
                isCheck = true;
            }
        }
        else if (nextMoveDir.z == -1.0f)
        {
            if (temp.z > 0)
            {
                isCheck = true;
            }
        }
        return isCheck;
    }

    #region anglecheck
    protected float GetAngle()
    {
        // ドローンの向きから方向を設定
        float rot = transform.localEulerAngles.y;
        rot = rot % 360.0f;
        return rot;
    }
    protected bool IsAngleDown()
    {
        float tempAdd = 20.0f;
        float angle = GetAngle();
        if (angle > 0.0f)
        {
            // 0 - 20 < 以上
            if (180.0f - tempAdd < angle &&
                angle < 180.0f + tempAdd)
            {
                return true;
            }
        }
        else
        {
            // -160 > 以下 && -180 < 以上
            if (-180.0f + tempAdd > angle &&
                angle > -180.0f - tempAdd)
            {
                return true;
            }
        }
        return false;
    }
    protected bool IsAngleUp()
    {
        float tempAdd = 20.0f;
        float angle = GetAngle();
        if (angle > 0.0f)
        {
            // 0 - 20 < 以上
            if (angle < 0.0f + tempAdd)
            {
                return true;
            }
            else if(360.0f - tempAdd < angle)
            {
                return true;
            }
        }
        else
        {
            if (angle > 0.0f - tempAdd)
            {
                return true;
            }
            else if(-360.0f + tempAdd > angle)
            {
                return true;
            }
        }
        return false;
    }
    protected bool IsAngleRight()
    {
        float tempAdd = 20.0f;
        float angle = GetAngle();
        if (angle > 0.0f)
        {
            if (90.0f - tempAdd < angle &&
                angle < 90.0f + tempAdd)
            {
                return true;
            }
        }
        else
        {
            if (-90.0f + tempAdd > angle &&
                angle > -90.0f - tempAdd)
            {
                return true;
            }
        }
        return false;
    }
    protected bool IsAngleLeft()
    {
        float tempAdd = 20.0f;
        float angle = GetAngle();
        if (angle > 0.0f)
        {
            if (270.0f - tempAdd < angle &&
                angle < 270.0f + tempAdd)
            {
                return true;
            }
        }
        else
        {
            if (-90.0f + tempAdd > angle &&
                angle > -90.0f - tempAdd)
            {
                return true;
            }
        }
        return false;
    }
    protected bool IsAngleRightUp()
    {
        float tempAdd = 20.0f;
        float angle = GetAngle();
        if (angle > 0.0f)
        {
            if (45.0f - tempAdd < angle &&
                angle < 45.0f + tempAdd)
            {
                return true;
            }
        }
        else
        {
            if (-45.0f + tempAdd > angle &&
                angle > -45.0f - tempAdd)
            {
                return true;
            }
        }
        return false;
    }
    protected bool IsAngleRightDown()
    {
        float tempAdd = 20.0f;
        float angle = GetAngle();
        if (angle > 0.0f)
        {
            if (135.0f - tempAdd < angle &&
                angle < 135.0f + tempAdd)
            {
                return true;
            }
        }
        else
        {
            if (-135.0f + tempAdd > angle &&
                angle > -135.0f - tempAdd)
            {
                return true;
            }
        }
        return false;
    }
    protected bool IsAngleLeftDown()
    {
        float tempAdd = 20.0f;
        float angle = GetAngle();
        if (angle > 0.0f)
        {
            if (225.0f - tempAdd < angle &&
                angle < 225.0f + tempAdd)
            {
                return true;
            }
        }
        else
        {
            if (-135.0f + tempAdd > angle &&
                angle > -135.0f - tempAdd)
            {
                return true;
            }
        }
        return false;
    }
    protected bool IsAngleLeftUp()
    {
        float tempAdd = 20.0f;
        float angle = GetAngle();
        if (angle > 0.0f)
        {
            if (315.0f - tempAdd < angle &&
                angle < 315.0f + tempAdd)
            {
                return true;
            }
        }
        else
        {
            if (-45.0f + tempAdd > angle &&
                angle > -45.0f - tempAdd)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    //================================================
    // [///] ログ
    //================================================
    public virtual void AddReplayLog(InputActionKey key, float value = 0.0f)
    {
        float time = ShInputManager.I.GetTimerCtrl().GetTime();
        // 時間がログ保存時間上限の場合は保存しない
        if (time >= LogSaveLimitTime)
        {
            //Debug.Log(" ログ保存時間オーバーです ");
            return;
        }
        // 時間停止中の場合保存しない
        else if (!ShInputManager.I.GetTimerCtrl().m_IsUpdate)
        {
            return;
        }

        string keyStr = ((int)key).ToString();
        AppData.ReplayInfo info = new AppData.ReplayInfo(time, keyStr, value);
        m_ReplayInfoList.Add(info);

        // debug
        // Debug.Log("time : " + time + " key : " + keyStr+"(" + key.ToString() +")");
    }
    // ローカル保存
    public virtual void SaveLocalReplayData()
    {
    }

    //================================================
    // [///] ドローン挙動
    //================================================
    #region DroneProc
    // ドローン状態：モーターONに変更
    protected void ProcDrone_ChangeDroneAction_MoterOn()
    {
        // プロペラ設定
        m_PropellerCtrl.ProcMonterOn();

        m_IsNeutral = false;

        // se
        if (AppData.DroneFlightSound)
        {
            CoreManager.I.AudioComp.PlaySe(AudioId.moter.ToString(), true);
        }

        // 変更
        m_CurrentDroneActionId = DroneActionId.MonterOn;
    }
    // ドローン状態：飛行に変更
    protected void ProcDrone_ChangeDroneAction_Fly()
    {
        m_IsNeutral = false;

        // 物理
        m_Rigid.useGravity = false;
        m_Rigid.linearVelocity = Vector3.zero;


        // 飛行中SE
        if (!m_IsFlyingSEPlay)
        {
            if (AppData.DroneFlightSound)
            {
                // se
                CoreManager.I.AudioComp.StopSe(AudioId.moter.ToString(), true);
                // takeoffの重複対処
                CoreManager.I.AudioComp.StopSe(AudioId.takeoff.ToString(), false);

                m_IsFlyingSEPlay = true;
                CoreManager.I.AudioComp.PlaySe(AudioId.takeoff.ToString(), false, -1, false, true);

                StartCoroutine(ProcDrone_FlyingSESet());
            }
        }

        // 変更
        m_CurrentDroneActionId = DroneActionId.Fly;
    }
    private IEnumerator ProcDrone_FlyingSESet()
    {
        yield return new WaitForSeconds(2.5f);// GD_Audio.GetData(AudioId.takeoff.ToString()).m_Clip.length);

        if (AppData.DroneFlightSound)
        {
            if (m_IsFlyingSEPlay && m_CurrentDroneActionId == DroneActionId.Fly && AppData.m_PlayMode <= AppData.PlayMode.Replay)
            {
                if (MainGame.MG_Mediator.StateCtrl.IsMainUpdate())
                {
                    if (!CoreManager.I.AudioComp.IsPlaySe(AudioId.flying.ToString(), true))
                    {
                        CoreManager.I.AudioComp.PlaySe(AudioId.flying.ToString(), true, -1, true);
                    }
                }
            }
            else if(m_IsFlyingSEPlay && m_CurrentDroneActionId == DroneActionId.Fly)
            {
                if (!CoreManager.I.AudioComp.IsPlaySe(AudioId.flying.ToString(), true))
                {
                    CoreManager.I.AudioComp.PlaySe(AudioId.flying.ToString(), true, -1, true);
                }
            }
        }
    }
    // ドローン状態：待機(モーター停止)に変更
    protected void ProcDrone_ChangeDroneAction_MoterOff()
    {
        // プロペラ設定
        m_PropellerCtrl.ProcMonterOff();

        if (AppData.DroneFlightSound)
        {
            // se
            CoreManager.I.AudioComp.StopSe(AudioId.moter.ToString(), true);
        }

        m_IsNeutral = false;

        // 変更
        m_CurrentDroneActionId = DroneActionId.Wait;
    }
    // ドローン 上昇/下降
    protected void ProcDrone_MoveUpDown(float val)
    {
        // スティック無反応地帯
        if (val >= 0.01f || val <= -0.01f)
        {
            // 上昇/下降
            float now = m_AddPosYSpd + AddPosYAccele * m_SpdChangeMag * val * Time.fixedDeltaTime;
            m_AddPosYSpd = Mathf.Clamp(now, -AppDebugData.AddPosYLimitSpd * m_SpdChangeMag, AppDebugData.AddPosYLimitSpd * m_SpdChangeMag);
        }
        // ニュートラル
        else
        {
            // 0に近いか
            if (-0.0001f < m_AddPosYSpd && 0.0001f > m_AddPosYSpd)
            {
                m_AddPosYSpd = 0.0f;
            }
            else
            {
                // 移動量を減らす 半減
                float change = Mathf.Lerp(m_AddPosYSpd, 0.0f, AppDebugData.PosYDampRate);
                //Debug.Log("now : " + m_AddPosYSpd + "   change : " + change);
                m_AddPosYSpd = change;
            }
        }
        m_MoveTemp = transform.localPosition + Vector3.up * m_AddPosYSpd * Time.fixedDeltaTime;

        // 移動制限
        if (m_MoveTemp.y > MoveLimitY_Plus) { m_MoveTemp.y = MoveLimitY_Plus; m_IsMoveLimitCheck = true; }
        else if (m_MoveTemp.y < MoveLimitY_Minus) { m_MoveTemp.y = MoveLimitY_Minus; m_IsMoveLimitCheck = true; }

        // 移動
        transform.localPosition = m_MoveTemp;

        if(AppData.m_PlayMode <= AppData.PlayMode.Replay)
            MainGame.MG_Mediator.MainCanvas.GetVerticalSpdInfo().SetVerticalSpd(m_AddPosYSpd);
    }
    // ドローン 横軸回転
    protected void ProcDrone_RotY(float val)
    {
        if (val >= 0.01f || val <= -0.01f)
        {
            // 左右回転
            float now = m_AddRotYSpd + AddRotYAccele * val * Time.fixedDeltaTime;
            m_AddRotYSpd = Mathf.Clamp(now, -AppDebugData.AddRotYLimitSpd, AppDebugData.AddRotYLimitSpd);
        }
        // ニュートラル
        else
        {
            // 0に近いか
            if (-0.0001f < m_AddRotYSpd && 0.0001f > m_AddRotYSpd)
            {
                m_AddRotYSpd = 0.0f;
            }
            else
            {
                // 移動量を減らす 半減
                float change = Mathf.Lerp(m_AddRotYSpd, 0.0f, AppDebugData.RotYDampRate);
                //Debug.Log("now : " + m_AddRotYSpd + "   change : " + change);
                m_AddRotYSpd = change;
            }
        }
        // 移動
        transform.localEulerAngles += Vector3.up * m_AddRotYSpd * Time.fixedDeltaTime;
    }
    // ドローン 移動X
    protected void ProcDrone_MoveX(float val)
    {
        if (val >= 0.01f || val <= -0.01f)
        {
            // 上昇/下降
            float now = m_AddPosXSpd + AddPosXAccele * m_SpdChangeMag * val * Time.fixedDeltaTime;
            m_AddPosXSpd = Mathf.Clamp(now, -AppDebugData.AddPosXLimitSpd * m_SpdChangeMag, AppDebugData.AddPosXLimitSpd * m_SpdChangeMag);
        }
        // ニュートラル
        else
        {
            // 0に近いか
            if (-0.0001f < m_AddPosXSpd && 0.0001f > m_AddPosXSpd)
            {
                m_AddPosXSpd = 0.0f;
            }
            else
            {
                // 移動量を減らす 半減
                float change = Mathf.Lerp(m_AddPosXSpd, 0.0f, AppDebugData.PosXDampRate);
                //Debug.Log("now : " + m_AddPosXSpd + "   change : " + change);
                m_AddPosXSpd = change;
            }
        }
        // 移動
        // if (AppData.m_PlayMode == AppData.PlayMode.Game_Treasure)
        // {
        //     m_MoveTemp = transform.localPosition + MainCameraManager.I.Camera.transform.right * m_AddPosXSpd * Time.fixedDeltaTime;
        // }
        // else 
        if (m_IsHeadless)
        {
            m_MoveTemp = transform.localPosition + Vector3.right * m_AddPosXSpd * Time.fixedDeltaTime;
        }
        else
        {
            m_MoveTemp = transform.localPosition + transform.right * m_AddPosXSpd * Time.fixedDeltaTime;
        }

        // 移動制限
        if(m_MoveTemp.x > MoveLimitX_PlusMinus) { m_MoveTemp.x = MoveLimitX_PlusMinus; m_IsMoveLimitCheck = true; }
        else if(m_MoveTemp.x < -MoveLimitX_PlusMinus) { m_MoveTemp.x = -MoveLimitX_PlusMinus; m_IsMoveLimitCheck = true; }
        transform.localPosition = m_MoveTemp;

        if (AppData.m_PlayMode <= AppData.PlayMode.Replay)
            MainGame.MG_Mediator.MainCanvas.GetHorizontalSpdInfo().SetHorizontalSpd(m_AddPosXSpd + m_AddPosZSpd);
    }
    // ドローン 移動Z
    protected void ProcDrone_MoveZ(float val)
    {
        if (val >= 0.01f || val <= -0.01f)
        {
            // 上昇/下降
            float now = m_AddPosZSpd + AddPosZAccele * m_SpdChangeMag * val * Time.fixedDeltaTime;
            m_AddPosZSpd = Mathf.Clamp(now, -AppDebugData.AddPosZLimitSpd * m_SpdChangeMag, AppDebugData.AddPosZLimitSpd * m_SpdChangeMag);
        }
        // ニュートラル
        else
        {
            // 0に近いか
            if (-0.0001f < m_AddPosZSpd && 0.0001f > m_AddPosZSpd)
            {
                m_AddPosZSpd = 0.0f;
            }
            else
            {
                // 移動量を減らす 半減
                float change = Mathf.Lerp(m_AddPosZSpd, 0.0f, AppDebugData.PosZDampRate);
                //Debug.Log("now : " + m_AddPosZSpd + "   change : " + change);
                m_AddPosZSpd = change;
            }
        }
        // 移動
        // if (AppData.m_PlayMode == AppData.PlayMode.Game_Treasure)
        // {
        //     Vector3 add = MainCameraManager.I.Camera.transform.forward * m_AddPosZSpd * Time.fixedDeltaTime;
        //     add.y = 0.0f;
        //     m_MoveTemp = transform.localPosition + add;
        // }
        // else 
        if (m_IsHeadless)
        {
            m_MoveTemp = transform.localPosition + Vector3.forward * m_AddPosZSpd * Time.fixedDeltaTime;
        }
        else
        {
            m_MoveTemp = transform.localPosition + transform.forward * m_AddPosZSpd * Time.fixedDeltaTime;
        }

        // 移動制限
        if (m_MoveTemp.z > MoveLimitZ_Plus) { m_MoveTemp.z = MoveLimitZ_Plus; m_IsMoveLimitCheck = true; }
        else if (m_MoveTemp.z < MoveLimitZ_Minus) { m_MoveTemp.z = MoveLimitZ_Minus; m_IsMoveLimitCheck = true; }
        transform.localPosition = m_MoveTemp;

        if (AppData.m_PlayMode <= AppData.PlayMode.Replay)
            MainGame.MG_Mediator.MainCanvas.GetHorizontalSpdInfo().SetHorizontalSpd(m_AddPosXSpd + m_AddPosZSpd);
    }
    // ドローン 移動時傾き
    protected void ProcDrone_MoveRotForward(float val)
    {
        if (val >= 0.01f || val <= -0.01f)
        {
            // 左右回転
            float now = m_AddRotX + AddRotXAccele * val * Time.fixedDeltaTime;
            if (val < 0.0f)
            {
                m_AddRotX = Mathf.Clamp(now, AppDebugData.AddRotXLimit * val, AppDebugData.AddRotXLimit * val * -1.0f);
            }
            else
            {
                m_AddRotX = Mathf.Clamp(now, AppDebugData.AddRotXLimit * val * -1.0f, AppDebugData.AddRotXLimit * val);
            }
        }
        // ニュートラル
        else
        {
            // 0に近いか
            if (-0.0001f < m_AddRotX && 0.0001f > m_AddRotX)
            {
                m_AddRotX = 0.0f;
            }
            else
            {
                // 移動量を減らす 半減
                float change = Mathf.Lerp(m_AddRotX, 0.0f, AppDebugData.RotXDampRate);
                m_AddRotX = change;
            }
        }
        // 移動
        Vector3 rot = m_Model.transform.localEulerAngles;
        //if(AppData.m_PlayMode == AppData.PlayMode.Game_Treasure)
        //{
        //    rot.x += MainCameraManager.I.Camera.transform.forward.z * m_AddRotX;
        //    rot.z += MainCameraManager.I.Camera.transform.forward.x * m_AddRotX;   // 反転
        //}
        //else 
        if (m_IsHeadless)
        {
            rot.x += transform.forward.z * m_AddRotX;
            rot.z += transform.forward.x * m_AddRotX;   // 反転
        }
        else
        {
            rot.x = m_AddRotX;
        }
        m_Model.transform.localEulerAngles = rot;
    }
    // ドローン 移動時傾き
    protected void ProcDrone_MoveRotRight(float val)
    {
        if (val >= 0.01f || val <= -0.01f)
        {
            // 左右回転
            float now = m_AddRotZ + AddRotZAccele * val * Time.fixedDeltaTime;
            if(val < 0.0f)
            {
                m_AddRotZ = Mathf.Clamp(now, AppDebugData.AddRotZLimit * val, AppDebugData.AddRotZLimit * val * -1.0f);
            }
            else
            {
                m_AddRotZ = Mathf.Clamp(now, AppDebugData.AddRotZLimit * val * -1.0f, AppDebugData.AddRotZLimit * val);
            }
        }
        // ニュートラル
        else
        {
            // 0に近いか
            if (-0.0001f < m_AddRotZ && 0.0001f > m_AddRotZ)
            {
                m_AddRotZ = 0.0f;
            }
            else
            {
                // 移動量を減らす 半減
                float change = Mathf.Lerp(m_AddRotZ, 0.0f, AppDebugData.RotZDampRate);
                m_AddRotZ = change;
            }
        }
        // 移動
        Vector3 rot = m_Model.transform.localEulerAngles;
        // if (AppData.m_PlayMode == AppData.PlayMode.Game_Treasure)
        // {
        //     rot.x = MainCameraManager.I.Camera.transform.right.z * -m_AddRotZ;
        //     rot.z = MainCameraManager.I.Camera.transform.right.x * -m_AddRotZ;   // 反転
        // }
        // else 
        if (m_IsHeadless)
        {
            rot.x = transform.right.z * -m_AddRotZ;
            rot.z = transform.right.x * -m_AddRotZ;   // 反転
        }
        else
        {
            rot.z = -m_AddRotZ; // 反転
        }
        m_Model.transform.localEulerAngles = rot;
    }
#endregion

    //=========================================================================
    // [///] バウンド設定
    //=========================================================================
    public void ProcChange_PhysicsMaterialBounciness()
    {
        foreach (BoxCollider col in GetComponents<BoxCollider>())
        {
            if (col.material)
            {
                col.material.bounciness = AppDebugData.DroneBounciness;
            }
        }
    }
}

