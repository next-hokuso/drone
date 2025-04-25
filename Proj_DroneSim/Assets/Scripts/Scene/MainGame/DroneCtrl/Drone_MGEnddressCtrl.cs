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

public class Drone_MGEnddressCtrl : DroneBase
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

    // 機体揺らし
    private bool m_IsDamage = false;
    private float m_DamageTimer = 0.0f;

    private bool m_IsResetRigit = false;
    private float m_ResetRigitTimer = 0.0f;

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
        MoveLimitZ_Minus = -10.0f;
        MoveLimitX_PlusMinus = 10.0f;

        // 少し浮かす
        MoveLimitY_Minus = 0.5f;

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

        // ダメージ時以外は物理移動オフ
        if (m_ActionProc == ActionProc.InGame)
        {
            if (m_IsResetRigit)
            {
                m_ResetRigitTimer += Time.deltaTime;
                if(m_ResetRigitTimer > 0.15f)
                {
                    m_IsResetRigit = false;
                    m_Rigid.linearVelocity = Vector3.zero;
                }
            }

            if (m_IsDamage)
            {
            //    m_Rigid.AddForce(m_Rigid.linearVelocity, ForceMode.Force);
            }
            else
            {
                //m_Rigid.linearVelocity = Vector3.zero;
            }
        }
        else if(m_ActionProc == ActionProc.Result)
        {
            m_Rigid.linearVelocity = Vector3.zero;
        }
    }

    // ゲームオーバー処理
    public override void SetGameOver()
    {
        // 
        m_ActionProc = ActionProc.Result;
        m_Rigid.linearVelocity = Vector3.zero;

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
                        // // 1.0fまで入力した場合→モーター起動
                        // if (m_Input_ValueLY >= 1.0f)
                        // {
                        //     ProcDrone_ChangeDroneAction_MoterOn();
                        // }
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
                        // // 移動
                        // ProcDrone_MoveZ(m_Input_ValueLY);
                        // 
                        // // 傾き
                        // ProcDrone_MoveRotForward(m_Input_ValueLY);
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
                    // ProcDrone_RotY(m_Input_ValueLX);
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
                        // // 移動
                        // ProcDrone_MoveZ(m_Input_ValueRY);
                        // 
                        // // 傾き
                        // ProcDrone_MoveRotForward(m_Input_ValueRY);
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
        if(!m_IsDamage)
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

        // エンドレスモードの対応
        {
            // 前方に移動し続ける ダメージ中以外
            if (!m_IsDamage)
            {
                float moveValue = 1.0f;
                // 移動
                ProcDrone_MoveZ(moveValue);

                if (!m_IsDamage)
                {
                    // 傾き
                    ProcDrone_MoveRotForward(moveValue);
                }
            }
            else
            {
                m_Rigid.AddForce(transform.forward * AddPosZLimitSpd);
            }
        }

        // 障害物にあたった場合
        if (m_IsDamage)
        {
            m_DamageTimer -= Time.deltaTime;
            if(m_DamageTimer < 0.0f)
            {
                m_IsDamage = false;
                m_DamageTimer = 0.0f;
                AppDebugData.AddPosZLimitSpd = 10.0f;

                m_Rigid.linearVelocity = Vector3.zero;
            }

            // 移動
            Vector3 rot = m_Model.transform.localEulerAngles;
            //rot.x = Random.Range(-10.0f, 10.0f);
            rot.z = Random.Range(-15.0f, 15.0f);
            m_Model.transform.localEulerAngles = rot;

        }

        // 移動制限に引っ掛かった場合
        if(m_IsMoveLimitCheck)
        {
            m_AddPosXSpd = 0.0f;
            m_AddPosYSpd = 0.0f;
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
        // if (collision.transform.CompareTag("GameItem0"))
        // {
        //     MGEndress_Mediator.MainCanvas.GetTimerCtrl().AddTime(10.0f);
        // }
        // else if(collision.transform.CompareTag("GameOut0"))
        // {
        //     MGEndress_Mediator.MainCanvas.GetTimerCtrl().AddTime(-5.0f);
        // }
        if (collision.transform.CompareTag("GameOut0"))
        {
            MGEndress_Mediator.MainCanvas.GetTimerCtrl().AddTime_CountUp(-5.0f);

            // 障害物の非表示
            //other.gameObject.SetActive(false);
            collision.gameObject.tag = "Dummy";

            // ダメージフラグOn
            m_IsDamage = true;
            m_DamageTimer = 1.0f;

            //m_AddPosZSpd = 0.0f;
            m_AddRotZ = 0.0f;
            AppDebugData.AddPosZLimitSpd = 7.0f;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Dummy"))
        {
            Vector3 add = m_Rigid.linearVelocity * 1.0f;
            if(collision.transform.position.x < transform.position.x)
            {
                add.x += 0.1f;
            }
            else
            {
                add.x += -0.1f;
            }
            add.x = Mathf.Clamp(add.x, -3.0f, 3.0f);
            add.y = Mathf.Clamp(add.y, -2.0f, 2.0f);
            add.z = Mathf.Clamp(add.z, -1.0f, 1.0f);

            m_IsDamage = true;
            m_DamageTimer += 0.1f;

            m_Rigid.AddForce(add, ForceMode.Force);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Dummy"))
        {
            m_IsResetRigit = true;
            m_ResetRigitTimer = 0.0f;

            m_DamageTimer = 0.15f;
            // m_Rigid.linearVelocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("GameItem0"))
        {
            MGEndress_Mediator.MainCanvas.GetTimerCtrl().AddTime_CountUp(10.0f);

            // アイテムの消去
            other.gameObject.SetActive(false);
        }
        else if (other.transform.CompareTag("GameOut0"))
        {
            MGEndress_Mediator.MainCanvas.GetTimerCtrl().AddTime_CountUp(-5.0f);

            // 障害物の非表示
            //other.gameObject.SetActive(false);
            other.gameObject.tag = "Dummy";

            // ダメージフラグOn
            m_IsDamage = true;
            m_DamageTimer = 0.25f;

            // m_AddPosZSpd = 0.0f;
            m_AddRotZ = 0.0f;
            AppDebugData.AddPosZLimitSpd = 5.0f;
        }
    }

    //=========================================================================
    // [///] バウンド設定
    //=========================================================================
    // →base格納
}

