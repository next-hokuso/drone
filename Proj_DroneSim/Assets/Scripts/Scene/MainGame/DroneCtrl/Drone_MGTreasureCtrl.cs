using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static ShsInputController;
using UnityEngine.InputSystem.DualShock;
using MainGame;
using static AppData;

public class Drone_MGTreasureCtrl : DroneBase
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

    private Camera m_DroneCamera = null;

    // 機体揺らし
    private bool m_IsDamage = false;
    private float m_DamageTimer = 0.0f;

    // カメラ回転
    private bool m_IsCameraRot = false;

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

        m_DroneCamera = transform.Find("Camera").GetComponent<Camera>();

        // 速度設定
        AppDebugData.AddPosZLimitSpd = 10.0f;
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
        // ProcDrone_ChangeDroneAction_MoterOn();
        // ProcDrone_ChangeDroneAction_Fly();

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
                        // 1.0fまで入力した場合→モーター起動
                        if (m_Input_ValueLY >= 1.0f)
                        {
                            ProcDrone_ChangeDroneAction_MoterOn();
                        }
                    }
                    break;

                case DroneActionId.MonterOn:
                    {
                        if (m_IsNeutral)
                        {
                            // 上げる
                            if (m_Input_ValueLY >= 1.0f)
                            {
                                ProcDrone_ChangeDroneAction_Fly();
                            }
                            // 下がった場合停止
                            else if (m_Input_ValueLY <= -1.0f)
                            {
                                ProcDrone_ChangeDroneAction_MoterOff();
                            }
                        }
                        else
                        {
                            // ニュートラルに戻るまでチェック
                            if (m_Input_ValueLY <= 0.0f)
                            {
                                m_IsNeutral = true;
                            }
                        }
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
                // // 接続モード:デフォルト(接続モード2)
                // //if (m_CurrentConnectMode == AppData.ConnectM.Mode2)
                    // 左右回転
                    ProcDrone_RotY(m_Input_ValueLX);
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
                        if (m_IsNeutral)
                        {
                            // 上げる
                            if (m_Input_ValueRY >= 1.0f)
                            {
                                // 飛行に変更
                                ProcDrone_ChangeDroneAction_Fly();
                            }
                            // 下がった場合停止
                            else if (m_Input_ValueLY <= -1.0f)
                            {
                                // 停止に変更
                                ProcDrone_ChangeDroneAction_MoterOff();
                            }
                        }
                        else
                        {
                            // ニュートラルに戻るまでチェック
                            if (m_Input_ValueLY <= 0.0f)
                            {
                                m_IsNeutral = true;
                            }
                        }
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
                    ProcInput_Keyboard_ProcAction();
                }
                // コントローラー
                else
                {
                    ProcInput_Controller();
                    ProcInput_Gamepad_ProcAction();

                }
            }
            SetTouch();
            ProcInput_Touch_ProcAction();
        }


        if (m_IsDamage)
        {
            m_DamageTimer -= Time.deltaTime;
            if (m_DamageTimer < 0.0f)
            {
                m_IsDamage = false;
                m_DamageTimer = 0.0f;
                AppDebugData.AddPosZLimitSpd = 10.0f;
            }

            // 移動
            Vector3 rot = m_Model.transform.localEulerAngles;
            //rot.x = Random.Range(-10.0f, 10.0f);
            rot.z = Random.Range(-15.0f, 15.0f);
            m_Model.transform.localEulerAngles = rot;

        }


        // 位置の保存
        m_PrevPos = transform.position;
    }

    // 操作
    #region InputProc
    // →SetKeyboard / SetTouch / SetGamePad をbaseに格納
    // キーボードの機能対応
    private void ProcInput_Keyboard_ProcAction()
    {
        // 機能ボタンのチェック
        {
            //if (AppData.GetTrg_KeyOnly(AppData.Action.Metronome))
            // カメラ切り替え
            if(Input.GetKeyDown(KeyCode.R))
            {
                m_DroneCamera.gameObject.SetActive(!m_DroneCamera.gameObject.activeSelf);
            }
            else             // カメラ回転
            if (Input.GetKey(KeyCode.Q))
            {
                // カメラ回転
                MainCameraManager.I.CamCtrl.SetRotY(MainCameraManager.I.CamCtrl.GetRotY() + -1.0f * 180.0f * Time.deltaTime);
            }
            else            // カメラ回転
            if (Input.GetKey(KeyCode.E))
            {
                // カメラ回転
                MainCameraManager.I.CamCtrl.SetRotY(MainCameraManager.I.CamCtrl.GetRotY() + 1.0f * 180.0f * Time.deltaTime);
            }
        }
    }
    // ゲームパッドの機能対応
    private void ProcInput_Gamepad_ProcAction()
    {
        // 機能ボタンのチェック
        {
            // カメラ切り替え
            if (Input.GetKeyDown(KeyCode.JoystickButton7))
            {
                m_DroneCamera.gameObject.SetActive(!m_DroneCamera.gameObject.activeSelf);
            }
            else             // カメラ回転
            if (Input.GetKey(KeyCode.JoystickButton4))
            {
                // カメラ回転
                MainCameraManager.I.CamCtrl.SetRotY(MainCameraManager.I.CamCtrl.GetRotY() + -1.0f * 180.0f * Time.deltaTime);
            }
            else            // カメラ回転
            if (Input.GetKey(KeyCode.JoystickButton5))
            {
                // カメラ回転
                MainCameraManager.I.CamCtrl.SetRotY(MainCameraManager.I.CamCtrl.GetRotY() + 1.0f * 180.0f * Time.deltaTime);
            }
        }

        // パッド入力情報初期化
        AppData.ResetPadTrgs();
    }
    //画面上UIの機能対応
    private void ProcInput_Touch_ProcAction()
    {
        // 機能ボタンのチェック
        {
            // タッチチェック
            TouchInfo info = GetTouch();
            if(info == TouchInfo.Began)
            {
                if (ShInputManager.I.GetInputCanvas().IsTapArea_L1.IsDrawArea())
                {
                    m_IsCameraRot = true;
                }
                else            // カメラ回転
                if (ShInputManager.I.GetInputCanvas().IsTapArea_R1.IsDrawArea())
                {
                    m_IsCameraRot = true;
                }
            }
            else if (info == TouchInfo.Ended)
            {
                m_IsCameraRot = false;
            }

            // カメラ切り替え
            if (m_IsCameraRot)
            {
                if (ShInputManager.I.GetInputCanvas().IsTapArea_L1.IsDrawArea())
                {
                    // カメラ回転
                    MainCameraManager.I.CamCtrl.SetRotY(MainCameraManager.I.CamCtrl.GetRotY() + -1.0f * 90.0f * Time.deltaTime);
                }
                else            // カメラ回転
                if (ShInputManager.I.GetInputCanvas().IsTapArea_R1.IsDrawArea())
                {
                    // カメラ回転
                    MainCameraManager.I.CamCtrl.SetRotY(MainCameraManager.I.CamCtrl.GetRotY() + 1.0f * 90.0f * Time.deltaTime);
                }
            }
        }

        // パッド入力情報初期化
        AppData.ResetPadTrgs();
    }

    // カメラ
    public void ProcInput_UIButton_L1()
    {
        // カメラ回転
        MainCameraManager.I.CamCtrl.SetRotY(MainCameraManager.I.CamCtrl.GetRotY() + -1.0f * 180.0f * Time.deltaTime);
    }
    // カメラ
    public void ProcInput_UIButton_R1()
    {
        // カメラ回転
        MainCameraManager.I.CamCtrl.SetRotY(MainCameraManager.I.CamCtrl.GetRotY() + 1.0f * 180.0f * Time.deltaTime);
    }
    // カメラ
    public void ProcInput_UIButton_CameraChange()
    {
        m_DroneCamera.gameObject.SetActive(!m_DroneCamera.gameObject.activeSelf);
    }

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
            if (m_CurrentDroneActionId == DroneActionId.Fly)
            {
                // 地面に付いた
                m_CurrentDroneActionId = DroneActionId.MonterOn;

                m_Rigid.linearVelocity = Vector3.zero;

                // se
                // 飛行中SE
                if (m_IsFlyingSEPlay)
                   {
                       m_IsFlyingSEPlay = false;
                       if (AppData.DroneFlightSound)
                       {
                           MainGame.MG_Mediator.GetAudio().StopSe(AudioId.flying.ToString(), true);
                   
                           MainGame.MG_Mediator.GetAudio().PlaySe(AudioId.moter.ToString(), true);
                       }
                   }
                   
                   // 物理
                   m_Rigid.useGravity = true;
                   
                   m_Rigid.AddForce(Vector3.up * m_AddPosYSpd);

                // // 自動着陸のOFF
                // if (m_IsAutoLandingAction)
                // {
                //     ProcAutoFlyLandingOff();
                // 
                //     // Waitまで戻す
                //     ProcDrone_ChangeDroneAction_MoterOff();
                // }

                // リセット
                {
                    m_Model.transform.localEulerAngles = Vector3.zero;

                    m_AddPosYSpd = 0.0f;
                    m_AddPosXSpd = 0.0f;
                    m_AddPosZSpd = 0.0f;
                    m_AddRotX = 0.0f;
                    m_AddRotYSpd = 0.0f;
                    m_AddRotZ = 0.0f;

                    ShInputManager.I.GetInputCanvas().GetPadLCtrl().SetPad(
                        0.0f, 0.0f);
                    ShInputManager.I.GetInputCanvas().GetPadRCtrl().SetPad(
                        0.0f, 0.0f);

                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Treasure"))
        {
            // アイテムの消去
            other.gameObject.SetActive(false);

            // リザルト呼び出し
            MainGame.MGTreasure_Mediator.MainCanvas.SetResult(0);

            ShInputManager.I.GetTimerCtrl().ProcGameEnd();

            MGTreasure_Mediator.StateCtrl.Set6GameClear();
        }
        else if (other.transform.CompareTag("Marker"))
        {
            MGTreasure_Mediator.MainCanvas.GetTimerCtrl().AddTime_CountUp(10.0f);

            // アイテムの消去
            other.gameObject.SetActive(false);
        }
    }

    //=========================================================================
    // [///] バウンド設定
    //=========================================================================
    // →base格納
}

