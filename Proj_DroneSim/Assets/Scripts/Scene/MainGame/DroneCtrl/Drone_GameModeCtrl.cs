using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static ShsInputController;
using UnityEngine.InputSystem.DualShock;

public class Drone_GameModeCtrl : DroneBase
{
    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================

    //================================================
    // [///]
    //================================================
    // ワイプ用カメラ
    private Camera m_WipeCamera = null;

    //================================================
    // [///]
    //================================================
    protected override void Start()
    {
    }
    protected override void OnDestroy()
    {
    }

    // 生成時設定
    public override void SetStartInfo()
    {
        base.SetStartInfo();

        // 移動制限設定
        MoveLimitZ_Plus = 999999.0f;
        MoveLimitZ_Minus = -9999999.0f;
        MoveLimitX_PlusMinus = 99999999.0f;

    }
    // 開始
    public override void SetStart()
    {
        base.SetStart();

    }
    // スタート演出後のステージ処理開始
    public override void SetStart_ProcOn()
    {
        m_ActionProc = ActionProc.InGame;
        ProcDrone_ChangeDroneAction_MoterOn();
        ProcDrone_ChangeDroneAction_Fly();

    }

    protected override void Update()
    {
    }

    protected override void LateUpdate()
    {
        m_IsChangeCheck = false;
        m_IsMoveLimitCheck = false;
    }

    // ゲームオーバー処理
    public override void SetGameOver()
    {
        // 
        m_ActionProc = ActionProc.None;

        // 自分非表示
    }

    // リセット(初期化対応)
    public override void ProcReset()
    {
        transform.position = m_StPos;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = m_StScale;
        m_ActionProc = ActionProc.None;
        m_CurrentDroneActionId = DroneActionId.None;
    }


    //================================================
    // [///] インプット後挙動設定共通関連
    //================================================
    #region DroneAction
    // 左スティックY軸挙動
    protected override void DroneAction_LeftStickY()
    {
        if (m_CurrentConnectMode == AppData.ConnectM.Mode2)
        {
            switch (m_CurrentDroneActionId)
            {
                case DroneActionId.Wait:
                    {
                        //// 1.0fまで入力した場合→モーター起動
                        //if (m_Input_ValueLY >= 1.0f)
                        //{
                        //    ProcDrone_ChangeDroneAction_MoterOn();
                        //}
                    }
                    break;

                case DroneActionId.MonterOn:
                    {
                        // if (m_IsNeutral)
                        // {
                        //     // 上げる
                        //     if (m_Input_ValueLY >= 1.0f)
                        //     {
                        //         ProcDrone_ChangeDroneAction_Fly();
                        //     }
                        //     // 下がった場合停止
                        //     else if (m_Input_ValueLY <= -1.0f)
                        //     {
                        //         ProcDrone_ChangeDroneAction_MoterOff();
                        //     }
                        // }
                        // else
                        // {
                        //     // ニュートラルに戻るまでチェック
                        //     if (m_Input_ValueLY <= 0.0f)
                        //     {
                        //         m_IsNeutral = true;
                        //     }
                        // }
                    }
                    break;

                case DroneActionId.Fly:
                    {
                        // 上昇/下降
                        ProcDrone_MoveUpDown(m_Input_ValueLY);
                    }
                    break;
            }

        }
        else if (m_CurrentConnectMode == AppData.ConnectM.Mode1)
        {
            switch (m_CurrentDroneActionId)
            {
                case DroneActionId.Fly:
                    // 右スティック
                    {
                        // 移動
                        ProcDrone_MoveZ(m_Input_ValueLY);
                        
                        // 傾き
                        ProcDrone_MoveRotForward(m_Input_ValueLY);
                    }
                    break;
            }
        }
    }
    // 左スティックX軸挙動
    protected override void DroneAction_LeftStickX()
    {
        switch (m_CurrentDroneActionId)
        {
            case DroneActionId.Fly:
                // 接続モード:デフォルト(接続モード2)
                //if (m_CurrentConnectMode == AppData.ConnectM.Mode2)
                {
                    // 左右回転
                    ProcDrone_RotY(m_Input_ValueLX);
                }
                break;
        }
    }
    // 右スティックY軸挙動
    protected override void DroneAction_RightStickY()
    {
        if (m_CurrentConnectMode == AppData.ConnectM.Mode2)
        {
            switch (m_CurrentDroneActionId)
            {
                case DroneActionId.Fly:
                    {
                        // 移動
                        ProcDrone_MoveZ(m_Input_ValueRY);
                        
                        // 傾き
                        ProcDrone_MoveRotForward(m_Input_ValueRY);
                    }
                    break;
            }
        }
        else if (m_CurrentConnectMode == AppData.ConnectM.Mode1)
        {
            switch (m_CurrentDroneActionId)
            {
                case DroneActionId.Wait:
                    {
                        // 1.0fまで入力した場合→モーター起動
                        if (m_Input_ValueRY >= 1.0f)
                        {
                            ProcDrone_ChangeDroneAction_MoterOn();
                        }
                    }
                    break;

                case DroneActionId.MonterOn:
                    {
                        // if (m_IsNeutral)
                        // {
                        //     // 上げる
                        //     if (m_Input_ValueRY >= 1.0f)
                        //     {
                        //         // 飛行に変更
                        //         ProcDrone_ChangeDroneAction_Fly();
                        //     }
                        //     // 下がった場合停止
                        //     else if (m_Input_ValueLY <= -1.0f)
                        //     {
                        //         // 停止に変更
                        //         ProcDrone_ChangeDroneAction_MoterOff();
                        //     }
                        // }
                        // else
                        // {
                        //     // ニュートラルに戻るまでチェック
                        //     if (m_Input_ValueLY <= 0.0f)
                        //     {
                        //         m_IsNeutral = true;
                        //     }
                        // }
                    }
                    break;

                case DroneActionId.Fly:
                    {
                        // 上昇/下降
                        ProcDrone_MoveUpDown(m_Input_ValueRY);
                    }
                    break;
            }
        }
    }
    protected override void DroneAction_RightStickX()
    {
        switch (m_CurrentDroneActionId)
        {
            case DroneActionId.Fly:
                // 右スティック
                {
                    // 移動
                    ProcDrone_MoveX(m_Input_ValueRX);

                    // 傾き
                    ProcDrone_MoveRotRight(m_Input_ValueRX);
                }
                break;
        }
    }
    #endregion


    //================================================
    // [///] インプット関連
    //================================================
    // 入力
    public void ProcInput()
    {
        //#if UNITY_WEBGL

        // タッチチェック
        {
            // 入力前に初期化
            m_Input_ValueLX = 0.0f;
            m_Input_ValueLY = 0.0f;
            m_Input_ValueRX = 0.0f;
            m_Input_ValueRY = 0.0f;

            if (!m_IsLeftStickUse && !m_IsRightStickUse)
            {
                // キーボード 
                if (AppData.ControllerMode == AppData.ControllerM.Keyboard)
                {
                    ProcInput_Keyboard();
                }
                // コントローラー
                else
                {
                    ProcInput_Controller();
                }
            }


            SetTouch();
        }
        // 位置の保存
        m_PrevPos = transform.position;
    }

    // 操作
    #region InputProc
    // →SetKeyboard / SetTouch / SetGamePad をbaseに格納
    #endregion

    //================================================
    // [///] ログ
    //================================================
    // →baseに格納 変化をつける場合、継承する
    public override void AddReplayLog(InputActionKey key, float value = 0.0f)
    {
    }
    // ローカル保存
    public override void SaveLocalReplayData()
    {
    }

    //================================================
    // [///] ドローン挙動
    //================================================
    // →baseに格納

    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Dummy"))
        {
        }
    }

    //=========================================================================
    // [///] バウンド設定
    //=========================================================================
    // →base格納
}

