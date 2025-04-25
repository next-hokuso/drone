using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

/// <summary>
/// 入力の管理
/// </summary>
public class ShInputManager : ShSingletonMonoBehavior<ShInputManager>
{
    //================================================
    // [///] 定義
    //================================================
    /// <summary>
    /// 選択したゲームオブジェクト 基本的にはSystem内で使用すること
    /// </summary>
    public GameObject m_SelectGO = null;

    // 入力キャンバス
    private InputCanvasCtrl m_InputCanvasCtrl = null;
    public void SetInputCanvasCtrl(InputCanvasCtrl ctrl) { m_InputCanvasCtrl = ctrl; }
    public InputCanvasCtrl GetInputCanvas() { return m_InputCanvasCtrl; }

    // ワイプキャンバス
    private WipeCanvasCtrl m_WipeCanvasCtrl = null;
    public void SetWipeCanvasCtrl(WipeCanvasCtrl ctrl) { m_WipeCanvasCtrl = ctrl; }
    public WipeCanvasCtrl GetWipeCanvas() { return m_WipeCanvasCtrl; }

    // UIコントローラー
    private bool m_IsUseUIController = true;
    public bool IsUseUIController() { return m_IsUseUIController; }
    public void SetIsUseUIController(bool isUse) { m_IsUseUIController = isUse; }

    // Timer
    private UITimerCtrl m_TimerCtrl = null;
    public void SetTimerCtrl(UITimerCtrl ctrl) { m_TimerCtrl = ctrl; }
    public UITimerCtrl GetTimerCtrl() { return m_TimerCtrl; }

    // keyinput setting
    public enum InputSetting
    {
        Keyboard,
        Controller,
    }
    private InputSetting m_CurrentInputSetting = InputSetting.Controller;

    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================
    public void setup()
    {

    }

    // シーンの処理開始
    public void ProcSceneProcStart()
    {
        m_InputCanvasCtrl.GetComponent<IShVisible>().Show();
        // m_WipeCanvasCtrl.GetComponent<IShVisible>().Show();
    }

    public void Update()
    {
    }

    //=========================================================================
    //
    // [///] KeySettings
    //
    //=========================================================================
    public InputSetting GetInputSetting()
    {
        return m_CurrentInputSetting;
    }
    public void SetInputSetting(InputSetting setting)
    {
        m_CurrentInputSetting = setting;
    }

    public float GetStickVal_Samp(float val)
    {
        float endVal = 0.0f;
        // 計算
        float stickval = val;
        // 一定以下/以上の入力を無しとする
        if (stickval < 0.0f)
        {
            // ベース値
            float baseVal = m_StickDeadZone_Max - m_StickDeadZone_Min;
            // 実機スティック値に低値を足して
            float calc = (stickval + m_StickDeadZone_Min) / baseVal;
            endVal = Mathf.Clamp(calc, -1.0f, 0.0f);
        }
        else
        {
            // ベース値
            float baseVal = m_StickDeadZone_Max - m_StickDeadZone_Min;
            // 実機スティック値に低値を足して
            float calc = (stickval - m_StickDeadZone_Min) / baseVal;
            endVal = Mathf.Clamp(calc, 0.0f, 1.0f);
        }
        return endVal;
    }

    //=========================================================================
    //
    // [///] ゲームパッド
    //
    //=========================================================================
    private Gamepad m_CurrentGamepad = null;
    [SerializeField] private float m_StickDeadZone_Min = 0.3f;
    [SerializeField] private float m_StickDeadZone_Max = 0.99f;

    // 計算チェック
    private bool m_IsCalc_LeftX = false;
    private bool m_IsCalc_LeftY = false;
    private bool m_IsCalc_RightX = false;
    private bool m_IsCalc_RightY = false;
    private float m_LeftXVal = 0.0f;
    private float m_LeftYVal = 0.0f;
    private float m_RightXVal = 0.0f;
    private float m_RightYVal = 0.0f;

    // 更新チェック用 セットアップ
    public void Setup_Gamepad()
    {
        m_CurrentGamepad = Gamepad.current;

        m_IsCalc_LeftX = false;
        m_IsCalc_LeftY = false;
        m_IsCalc_RightX = false;
        m_IsCalc_RightY = false;
    }
    public bool IsNull_Gamepad()
    {
        return (m_CurrentGamepad == null);
    }

    // 取得
    public float GetGamepadStickVal_LeftX()
    {
        if (m_IsCalc_LeftX) return m_LeftXVal;

        // 計算
        float stickval = m_CurrentGamepad.leftStick.ReadValue().x;
        // 一定以下/以上の入力を無しとする
        if (stickval < 0.0f)
        {
            // ベース値
            float baseVal = m_StickDeadZone_Max - m_StickDeadZone_Min;
            // 実機スティック値に低値を足して
            float calc = (stickval + m_StickDeadZone_Min) / baseVal;
            m_LeftXVal = Mathf.Clamp(calc, -1.0f, 0.0f);
        }
        else
        {
            // ベース値
            float baseVal = m_StickDeadZone_Max - m_StickDeadZone_Min;
            // 実機スティック値に低値を足して
            float calc = (stickval - m_StickDeadZone_Min) / baseVal;
            m_LeftXVal = Mathf.Clamp(calc, 0.0f, 1.0f);
        }
        m_IsCalc_LeftX = true;
        return m_LeftXVal;
    }
    public float GetGamepadStickVal_LeftY()
    {
        if (m_IsCalc_LeftY) return m_LeftYVal;

        // 計算
        float stickval = m_CurrentGamepad.leftStick.ReadValue().y;
        // 一定以下/以上の入力を無しとする
        if (stickval < 0.0f)
        {
            // ベース値
            float baseVal = m_StickDeadZone_Max - m_StickDeadZone_Min;
            // 実機スティック値に低値を足して
            float calc = (stickval + m_StickDeadZone_Min) / baseVal;
            m_LeftYVal = Mathf.Clamp(calc, -1.0f, 0.0f);
        }
        else
        {
            // ベース値
            float baseVal = m_StickDeadZone_Max - m_StickDeadZone_Min;
            // 実機スティック値に低値を足して
            float calc = (stickval - m_StickDeadZone_Min) / baseVal;
            m_LeftYVal = Mathf.Clamp(calc, 0.0f, 1.0f);
        }

        m_IsCalc_LeftY = true;
        return m_LeftYVal;

    }
    public float GetGamepadStickVal_RightX()
    {
        if (m_IsCalc_RightX) return m_RightXVal;

        // 計算
        float stickval = m_CurrentGamepad.rightStick.ReadValue().x;
        // 一定以下/以上の入力を無しとする
        if (stickval < 0.0f)
        {
            // ベース値
            float baseVal = m_StickDeadZone_Max - m_StickDeadZone_Min;
            // 実機スティック値に低値を足して
            float calc = (stickval + m_StickDeadZone_Min) / baseVal;
            m_RightXVal = Mathf.Clamp(calc, -1.0f, 0.0f);
        }
        else
        {
            // ベース値
            float baseVal = m_StickDeadZone_Max - m_StickDeadZone_Min;
            // 実機スティック値に低値を足して
            float calc = (stickval - m_StickDeadZone_Min) / baseVal;
            m_RightXVal = Mathf.Clamp(calc, 0.0f, 1.0f);
        }

        m_IsCalc_RightX = true;
        return m_RightXVal;
    }
    public float GetGamepadStickVal_RightY()
    {
        if (m_IsCalc_RightY) return m_RightYVal;

        // 計算
        float stickval = m_CurrentGamepad.rightStick.ReadValue().y;
        // 一定以下/以上の入力を無しとする
        if (stickval < 0.0f)
        {
            // ベース値
            float baseVal = m_StickDeadZone_Max - m_StickDeadZone_Min;
            // 実機スティック値に低値を足して
            float calc = (stickval + m_StickDeadZone_Min) / baseVal;
            m_RightYVal = Mathf.Clamp(calc, -1.0f, 0.0f);
        }
        else
        {
            // ベース値
            float baseVal = m_StickDeadZone_Max - m_StickDeadZone_Min;
            // 実機スティック値に低値を足して
            float calc = (stickval - m_StickDeadZone_Min) / baseVal;
            m_RightYVal = Mathf.Clamp(calc, 0.0f, 1.0f);
        }

        m_IsCalc_RightY = true;
        return m_RightYVal;
    }

    //=========================================================================
    //
    // [///] Keyboard
    //
    //=========================================================================
#if false
    private Keyboard m_CurrentKeyboard = null;

    public bool m_IsPressed_A = false;
    public bool m_IsPressed_D = false;
    public bool m_IsPressed_S = false;
    public bool m_IsPressed_W = false;

    public bool m_IsPressed_Left = false;
    public bool m_IsPressed_Right = false;
    public bool m_IsPressed_Up = false;
    public bool m_IsPressed_Down = false;

    // 更新チェック用 セットアップ
    public void Setup_Keyboard()
    {
        m_CurrentKeyboard = Keyboard.current;
    }
    public bool IsNull_Keyboard()
    {
        return (m_CurrentKeyboard == null);
    }
    public void SetChangedIsPressed()
    {
        m_IsPressed_A = IsKeyboard_AKey_Pressed();
        m_IsPressed_D = IsKeyboard_DKey_Pressed();
        m_IsPressed_S = IsKeyboard_SKey_Pressed();
        m_IsPressed_W = IsKeyboard_WKey_Pressed();

        m_IsPressed_Left = IsKeyboard_LeftArrowKey_Pressed();
        m_IsPressed_Right = IsKeyboard_RightArrowKey_Pressed();
        m_IsPressed_Up = IsKeyboard_UpArrowKey_Pressed();
        m_IsPressed_Down = IsKeyboard_DownArrowKey_Pressed();
    }

    public bool IsKeyboard_AKey()
    {
        return (m_CurrentKeyboard.aKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_SKey()
    {
        return (m_CurrentKeyboard.sKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_DKey()
    {
        return (m_CurrentKeyboard.dKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_WKey()
    {
        return (m_CurrentKeyboard.wKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_AKey_Pressed()
    {
        return m_CurrentKeyboard.aKey.isPressed;
    }
    public bool IsKeyboard_SKey_Pressed()
    {
        return m_CurrentKeyboard.sKey.isPressed;
    }
    public bool IsKeyboard_DKey_Pressed()
    {
        return m_CurrentKeyboard.dKey.isPressed;
    }
    public bool IsKeyboard_WKey_Pressed()
    {
        return m_CurrentKeyboard.wKey.isPressed;
    }
    // →←↑↓
    public bool IsKeyboard_RightArrowKey()
    {
        return (m_CurrentKeyboard.rightArrowKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_LeftArrowKey()
    {
        return (m_CurrentKeyboard.leftArrowKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_UpArrowKey()
    {
        return (m_CurrentKeyboard.upArrowKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_DownArrowKey()
    {
        return (m_CurrentKeyboard.downArrowKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_RightArrowKey_Pressed()
    {
        return m_CurrentKeyboard.rightArrowKey.isPressed;
    }
    public bool IsKeyboard_LeftArrowKey_Pressed()
    {
        return m_CurrentKeyboard.leftArrowKey.isPressed;
    }
    public bool IsKeyboard_UpArrowKey_Pressed()
    {
        return m_CurrentKeyboard.upArrowKey.isPressed;
    }
    public bool IsKeyboard_DownArrowKey_Pressed()
    {
        return m_CurrentKeyboard.downArrowKey.isPressed;
    }
#endif
#if true
    //=========================================================================
    //
    // [///] Keyboard
    //
    //=========================================================================
    private Keyboard m_CurrentKeyboard = null;

    public bool m_IsPressed_LeftUp = false;
    public bool m_IsPressed_LeftDown = false;
    public bool m_IsPressed_LeftL = false;
    public bool m_IsPressed_LeftR = false;

    public bool m_IsPressed_RightUp = false;
    public bool m_IsPressed_RightDown = false;
    public bool m_IsPressed_RightL = false;
    public bool m_IsPressed_RightR = false;

    // 更新チェック用 セットアップ
    public void Setup_Keyboard()
    {
        m_CurrentKeyboard = Keyboard.current;
    }
    public bool IsNull_Keyboard()
    {
        return (m_CurrentKeyboard == null);
    }
    public void SetChangedIsPressed()
    {
        // m_IsPressed_LeftUp = AppData.GetInput(AppData.Action.LUp);
        // m_IsPressed_LeftDown = AppData.GetInput(AppData.Action.LDown);
        // m_IsPressed_LeftL = AppData.GetInput(AppData.Action.LLeft);
        // m_IsPressed_LeftR = AppData.GetInput(AppData.Action.LRight);
        // 
        // m_IsPressed_RightUp = AppData.GetInput(AppData.Action.RUp);
        // m_IsPressed_RightDown = AppData.GetInput(AppData.Action.RDown);
        // m_IsPressed_RightL = AppData.GetInput(AppData.Action.RLeft);
        // m_IsPressed_RightR = AppData.GetInput(AppData.Action.RRight);
        // 
        // Yns.YnSys.SetDbgText("ChangeIsLeftLeftL : " + (m_IsPressed_LeftL ? "true" : "false"));
        m_IsPressed_LeftUp = IsKey_Pressed_LeftUp();
        m_IsPressed_LeftDown = IsKey_Pressed_LeftDown();
        m_IsPressed_LeftL = IsKey_Pressed_LeftL();
        m_IsPressed_LeftR = IsKey_Pressed_LeftR();

        m_IsPressed_RightUp = IsKey_Pressed_RightUp();
        m_IsPressed_RightDown = IsKey_Pressed_RightDown();
        m_IsPressed_RightL = IsKey_Pressed_RightL();
        m_IsPressed_RightR = IsKey_Pressed_RightR();
    }

    public bool IsKeyboard_AKey()
    {
        return (m_CurrentKeyboard.aKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_SKey()
    {
        return (m_CurrentKeyboard.sKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_DKey()
    {
        return (m_CurrentKeyboard.dKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_WKey()
    {
        return (m_CurrentKeyboard.wKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_AKey_Pressed()
    {
        return m_CurrentKeyboard.aKey.isPressed;
    }
    public bool IsKeyboard_SKey_Pressed()
    {
        return m_CurrentKeyboard.sKey.isPressed;
    }
    public bool IsKeyboard_DKey_Pressed()
    {
        return m_CurrentKeyboard.dKey.isPressed;
    }
    public bool IsKeyboard_WKey_Pressed()
    {
        return m_CurrentKeyboard.wKey.isPressed;
    }
    // →←↑↓
    public bool IsKeyboard_RightArrowKey()
    {
        return (m_CurrentKeyboard.rightArrowKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_LeftArrowKey()
    {
        return (m_CurrentKeyboard.leftArrowKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_UpArrowKey()
    {
        return (m_CurrentKeyboard.upArrowKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_DownArrowKey()
    {
        return (m_CurrentKeyboard.downArrowKey.ReadValue() > 0.0f);
    }
    public bool IsKeyboard_RightArrowKey_Pressed()
    {
        return m_CurrentKeyboard.rightArrowKey.isPressed;
    }
    public bool IsKeyboard_LeftArrowKey_Pressed()
    {
        return m_CurrentKeyboard.leftArrowKey.isPressed;
    }
    public bool IsKeyboard_UpArrowKey_Pressed()
    {
        return m_CurrentKeyboard.upArrowKey.isPressed;
    }
    public bool IsKeyboard_DownArrowKey_Pressed()
    {
        return m_CurrentKeyboard.downArrowKey.isPressed;
    }

    // 共通:キー
    public bool IsKey_Pressed_LeftUp()
    {
        return AppData.GetInput(AppData.Action.LUp) || AppData.GetTrg_KeyOnly(AppData.Action.LUp);
        //return IsKeyboard_WKey();
    }
    public bool IsKey_Pressed_LeftDown()
    {
        return AppData.GetInput(AppData.Action.LDown) || AppData.GetTrg_KeyOnly(AppData.Action.LDown);
        //return IsKeyboard_SKey();
    }
    public bool IsKey_Pressed_LeftL()
    {
        return AppData.GetInput(AppData.Action.LLeft) || AppData.GetTrg_KeyOnly(AppData.Action.LLeft);
    }
    public bool IsKey_Pressed_LeftR()
    {
        return AppData.GetInput(AppData.Action.LRight) || AppData.GetTrg_KeyOnly(AppData.Action.LRight);
    }
    public bool IsKey_Pressed_RightUp()
    {
        return AppData.GetInput(AppData.Action.RUp) || AppData.GetTrg_KeyOnly(AppData.Action.RUp);
    }
    public bool IsKey_Pressed_RightDown()
    {
        return AppData.GetInput(AppData.Action.RDown) || AppData.GetTrg_KeyOnly(AppData.Action.RDown);
    }
    public bool IsKey_Pressed_RightL()
    {
        return AppData.GetInput(AppData.Action.RLeft) || AppData.GetTrg_KeyOnly(AppData.Action.RLeft);
    }
    public bool IsKey_Pressed_RightR()
    {
        return AppData.GetInput(AppData.Action.RRight) || AppData.GetTrg_KeyOnly(AppData.Action.RRight);
    }
    // 共通:保持フラグとキー状態に相違あるか
    public bool IsKeyDiff_LeftUp()
    {
        return m_IsPressed_LeftUp != AppData.GetInput(AppData.Action.LUp);
    }
    public bool IsKeyDiff_LeftDown()
    {
        return m_IsPressed_LeftUp != IsKey_Pressed_LeftUp();
    }
    public class KeyKeyKey
    {
        public bool m_IsPressedKey = false;
        public Key m_Key = Key.A;
        
        // value
        // 現在キー状態と保持状態に相違あるか
        public bool IsKeyDiff(UnityEngine.InputSystem.Controls.KeyControl key)
        {
            return m_IsPressedKey != key.isPressed;
        }
    }
#endif
    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================
    public void CheckPressdInput()
    {
        if(DualShockGamepad.current != null)
        {
            foreach(var key in DualShockGamepad.current.children)
            {
                if (key.IsPressed())
                {
                    //Yns.YnSys.SetDbgText("DualShock : Pressd");
                }
            }
        }
        if(Keyboard.current != null)
        {
            foreach(var key in Keyboard.current.children)
            {
                if (key.IsPressed())
                {
                    Yns.YnSys.SetDbgText("Keyboard : Pressd");
                }
            }
        }
    }
}