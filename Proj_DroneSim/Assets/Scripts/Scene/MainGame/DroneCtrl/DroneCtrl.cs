using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static ShsInputController;
using UnityEngine.InputSystem.DualShock;

public class DroneCtrl : DroneBase
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

    // リプレイ関連
    private bool m_Replay_IsEnd = false;
    private int m_Replay_NowIndex = 0;
    private AppData.ReplayInfo m_Replay_CurrentInfo = null;

    // ミッション関連
    private MissionId m_CurrentMissionId = MissionId.Mission_Fly;
    private int m_MissionCnt = 0;
    private int m_MissionSubCnt = 0;
    //  移動限界
    //private bool m_IsMoveLimitCheck = false;
    //  スコア
    private int m_MissionPoint = 100;
    //  アウト区間
    private bool m_IsOutAreaIn = false;
    //  次の移動方向
    private bool m_IsNextMoveDirChecking = false;
    private bool m_IsNextMoveDirError = false;
    private Vector3 m_NextMoveDir = Vector3.zero;
    // private Vector3 m_PrevPos = Vector3.zero;
    private float m_MoveDistCheck = 0.0f;
    // 間区間での機首方向不良チェック
    private bool m_IsMissionDirCheck = false;
    private bool m_IsEightFly_StR = false;
    // 離陸後の高さ
    private bool m_IsMissionFlyCheck = false;
    private float m_MissionFlyHeight = 0.0f;
    // ふらつき
    private float m_MissionHuratukiCheckTime = 0.0f;
    private bool m_IsMissionHuratukiInputPosX1 = false;
    private bool m_IsMissionHuratukiInputPosX2 = false;
    private bool m_IsMissionHuratukiInputPosZ1 = false;
    private bool m_IsMissionHuratukiInputPosZ2 = false;
    private bool m_IsMissionHuratukiInputRotY1 = false;
    private bool m_IsMissionHuratukiInputRotY2 = false;
    // 処理終了
    private bool m_IsMissionEnd = false;

    // 機能関連
    public bool m_IsMetronome = false;
    public bool m_IsGrid = false;
    public bool m_IsFripWait = false;
    // public bool m_IsHeadless = false; ->baseに追加
    public bool m_IsVisionSensor = true;        // ビジョンセンサー

    // ビジョンセンサー用風関連
    private int m_WindSeed = 0;
    private Vector3 m_WindAddForce = Vector3.zero;
    private int m_WindChangeCnt = 0;
    private float m_WindUpdateTime = 0.0f;
    private float m_WindSpd = 0.0f;
    private float m_WindNextSpd = 0.0f;
    private Vector3 m_WindNextAddForce = Vector3.zero;

    // フリップ関連
    private bool m_IsFripAction = false;
    private InputDirId m_FripDirId = InputDirId.Down;
    private float m_FripTimer = 0.0f;
    private Vector3 m_FripRotSt = Vector3.zero;
    private Vector3 m_FripRotEnd = Vector3.zero;
    private bool m_FripIsRotEnd = false;

    // メトロノーム関連
    private DroneMetronomeCtrl m_MetronomeCtrl = null;

    // 自動離着陸関連
    private bool m_IsAutoFly_MonterOnWait = false;
    private bool m_IsAutoFlyAction = false;
    private bool m_IsAutoLandingAction = false;

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

        // 速度設定
        AppDebugData.AddPosZLimitSpd = 5.0f;

        // 移動限界(コライダーで設定)
        MoveLimitY_Plus = 9999.0f;
        MoveLimitY_Minus = -9999.0f;

        // 中心当たり判定設定
        transform.Find("Center").gameObject.AddComponent<HitCheck_Center>().SetDroneCtrl(this);

        // ワイプ設定
        {
            // camera
            m_WipeCamera = transform.Find("Camera").GetComponent<Camera>();
            RenderTexture texture = Resources.Load("Texture/WipeRenderTexture") as RenderTexture;
            transform.Find("Camera").GetComponent<Camera>().targetTexture = texture;

            ShInputManager.I.GetWipeCanvas().SetDroneCameraWipeRenderTexture(texture);

            RenderTexture texture2 = Resources.Load("Texture/Wipe2RenderTexture") as RenderTexture;
            ShInputManager.I.GetWipeCanvas().SetGameCameaWipeRenderTexture(texture2);
        }

        // 機能設定
        {
            m_IsMetronome = AppData.MetronomeFlightSound;
            m_IsGrid = AppData.GridDisplay;
            m_MetronomeCtrl = gameObject.AddComponent<DroneMetronomeCtrl>();
        }

        // UI設定
        MainGame.MG_Mediator.MainCanvas.GetVerticalSpdInfo().SetVerticalSpd(0.0f);
        MainGame.MG_Mediator.MainCanvas.GetHorizontalSpdInfo().SetHorizontalSpd(0.0f);
        MainGame.MG_Mediator.MainCanvas.GetPointInfo().SetPoint(m_MissionPoint);
        MainGame.MG_Mediator.MainCanvas.GetTimeInfo().SetTimeText(0);
        MainGame.MG_Mediator.MainCanvas.GetMissionInfo().SetCurrentMissionText(0);
    }
    // 開始
    public override void SetStart()
    {
        base.SetStart();

        // リプレイ用：ログが存在するか
        if(AppData.m_PlayMode == AppData.PlayMode.Replay)
        {
            if (AppData.GetReplayData(AppData.m_SelectReplayIndex).m_ReplayInfoList == null) {
                m_Replay_IsEnd = true;
                // 戻る処理
                StartCoroutine(Proc_ReplayEnd());
            }

            // 風設定
            {
                m_WindSeed = AppData.GetReplayData(AppData.m_SelectReplayIndex).m_WindSeed;
                // Debug.Log("WindSeed : " + m_WindSeed);
                ProcWindUpdate();
            }
            // メトロノーム設定
            if (m_IsMetronome)
            {
                m_MetronomeCtrl.SetMetronome(false);
            }
        }
        else
        {
            // 風設定
            {
                m_WindSeed = Random.Range(100, 999);
                // Debug.Log("WindSeed : " + m_WindSeed);
                ProcWindUpdate();
            }
            // メトロノーム設定
            if (m_IsMetronome)
            {
                m_MetronomeCtrl.SetMetronome(true);
            }
        }

        // 接続モードの取得
        m_CurrentConnectMode = AppData.GetCurrentConnectMode();

        m_ActionProc = ActionProc.InGame;
        m_CurrentDroneActionId = DroneActionId.Wait;

        // メトロノーム設定:リプレイ用に設定しておく
        if (m_IsMetronome)
        {
            AddReplayLog(InputActionKey.Proc_Metronome_On);
        }
    }
    // スタート演出後のステージ処理開始
    public override void SetStart_ProcOn()
    {
        m_ActionProc = ActionProc.InGame;
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
    // [///] リプレイ関連
    //================================================
    // リプレイ終了時
    private IEnumerator Proc_ReplayEnd()
    {
        yield return new WaitForSeconds(1.0f);

        MainGame.MG_Mediator.MainCanvas.OnClick_ReturnScene();
    }

#if true
    // リプレイ中処理(MG_ObjMediatorから呼び出している)
    public void ProcReplay_Update()
    {
        if (m_Replay_IsEnd) return;
    
        float nowTime = ShInputManager.I.GetTimerCtrl().GetTime();
        float prevFrame = 0.0f;
        bool isChange = false;
    
    Loop:
        // エンドチェック
        if (m_Replay_NowIndex >= AppData.GetReplayData(AppData.m_SelectReplayIndex).m_ReplayInfoList.Count)
        {
            m_Replay_IsEnd = true;
    
            // TimerStop
            ShInputManager.I.GetTimerCtrl().ProcGameEnd();
    
            // UI
            m_Input_ValueLX = 0.0f;
            m_Input_ValueLY = 0.0f;
            m_Input_ValueRX = 0.0f;
            m_Input_ValueRY = 0.0f;
            // UI更新
            ShInputManager.I.GetInputCanvas().GetPadLCtrl().SetPad(
                m_Input_ValueLX, m_Input_ValueLY);
            ShInputManager.I.GetInputCanvas().GetPadRCtrl().SetPad(
                m_Input_ValueRX, m_Input_ValueRY);


            // 戻る処理
            StartCoroutine(Proc_ReplayEnd());

            return;
        }
        // ログ
        m_Replay_CurrentInfo = AppData.GetReplayData(AppData.m_SelectReplayIndex).m_ReplayInfoList[m_Replay_NowIndex];
    
        // ループ時:
        if (isChange)
        {
            // 秒数が違った場合:次のフレームに回す
            if (prevFrame != m_Replay_CurrentInfo.frame)
            {
                goto LoopEnd;
            }
        }
    
        // ログの時間を超えたら
        if (nowTime >= m_Replay_CurrentInfo.frame)
        {
            prevFrame = m_Replay_CurrentInfo.frame;
            isChange = true;
            // 実行
            int key = int.Parse(m_Replay_CurrentInfo.key);
            Replay_ValueSetting(key);

            // debug
            //Debug.Log("time : " + nowTime + " key : " + key + "(" + (InputActionKey)key + ") val : " + m_Replay_CurrentInfo.value);

            m_Replay_NowIndex++;
            goto Loop;
        }

    LoopEnd:
        if (!m_IsFripAction && !m_IsAutoFly_MonterOnWait)
        {
            // 変更があった場合
            if (isChange)
            {
                // ShInputManager.I.GetTimerCtrl().SetTime(prevFrame);

                // UI更新
                ShInputManager.I.GetInputCanvas().GetPadLCtrl().SetPad(
                    m_Input_ValueLX, m_Input_ValueLY);
                ShInputManager.I.GetInputCanvas().GetPadRCtrl().SetPad(
                    m_Input_ValueRX, m_Input_ValueRY);

                DroneAction_LeftStickX();
                DroneAction_LeftStickY();
                DroneAction_RightStickX();
                DroneAction_RightStickY();
            }
            // 変更が無い場合
            else
            {
                // 前回入力のまま挙動実行
                DroneAction_LeftStickX();
                DroneAction_LeftStickY();
                DroneAction_RightStickX();
                DroneAction_RightStickY();
            }
        }

        // 自動離陸の終了確認
        if (m_IsAutoFlyAction)
        {
            // 6.0f + dampで = 8.9f程
            if (transform.position.y > 6.0f)
            {
                ProcAutoFlyLandingOff();
            }
        }

        // フリップの更新
        if (m_IsFripAction)
        {
            ProcFripUpdate();
        }

        // // 風の更新 @20240819 プレイ中一定に変更(更新無し)
        // if (m_WindUpdateTime < ShInputManager.I.GetTimerCtrl().GetTime())
        // {
        //     ProcWindUpdate();
        // }


        // 挙動後に風の影響の対応
        if (!m_IsVisionSensor)
        {
            // 飛行中
            if (m_CurrentDroneActionId == DroneActionId.Fly)
            {
                // ビジョンセンサーOFF時に移動させる
                transform.localPosition += m_WindAddForce * Time.fixedDeltaTime;
            }
        }
    }
#endif

    private void Replay_ValueSetting(int key)
    {
        switch ((InputActionKey)key)
        {
            case InputActionKey.Null:
            case InputActionKey.Replay_End:
                m_Input_ValueLX = 0.0f;
                m_Input_ValueLY = 0.0f;
                m_Input_ValueRX = 0.0f;
                m_Input_ValueRY = 0.0f;
                break;

            // LXChange --------------------------------------------
            // 共通
            case InputActionKey.LStick_X:
            case InputActionKey.Key_Pressed_LeftL:
            case InputActionKey.Key_Release_LeftL:
            case InputActionKey.Key_Pressed_LeftR:
            case InputActionKey.Key_Release_LeftR:
            // gamepad
            case InputActionKey.Gamepad_LStick_X:
            // Keyboard
            case InputActionKey.Keyboard_Pressed_A:
            case InputActionKey.Keyboard_Pressed_D:
            case InputActionKey.Keyboard_Release_A:
            case InputActionKey.Keyboard_Release_D:
            // UI
            case InputActionKey.UI_LStick_X:
                m_Input_ValueLX = m_Replay_CurrentInfo.value;
                break;

            // LYChange --------------------------------------------
            // 共通
            case InputActionKey.LStick_Y:
            case InputActionKey.Key_Pressed_LeftUp:
            case InputActionKey.Key_Release_LeftUp:
            case InputActionKey.Key_Pressed_LeftDown:
            case InputActionKey.Key_Release_LeftDown:
            // gamepad
            case InputActionKey.Gamepad_LStick_Y:
            // Keyboard
            case InputActionKey.Keyboard_Pressed_W:
            case InputActionKey.Keyboard_Pressed_S:
            case InputActionKey.Keyboard_Release_W:
            case InputActionKey.Keyboard_Release_S:
            // UI
            case InputActionKey.UI_LStick_Y:
                m_Input_ValueLY = m_Replay_CurrentInfo.value;
                break;

            // RXChange --------------------------------------------
            // 共通
            case InputActionKey.RStick_X:
            case InputActionKey.Key_Pressed_RightL:
            case InputActionKey.Key_Release_RightL:
            case InputActionKey.Key_Pressed_RightR:
            case InputActionKey.Key_Release_RightR:
            // gamepad
            case InputActionKey.Gamepad_RStick_X:
            // Keyboard
            case InputActionKey.Keyboard_Pressed_LeftArrow:
            case InputActionKey.Keyboard_Pressed_RightArrow:
            case InputActionKey.Keyboard_Release_LeftArrow:
            case InputActionKey.Keyboard_Release_RightArrow:
            // UI
            case InputActionKey.UI_RStick_X:
                m_Input_ValueRX = m_Replay_CurrentInfo.value;
                break;

            // RYChange --------------------------------------------
            // 共通
            case InputActionKey.RStick_Y:
            case InputActionKey.Key_Pressed_RightUp:
            case InputActionKey.Key_Release_RightUp:
            case InputActionKey.Key_Pressed_RightDown:
            case InputActionKey.Key_Release_RightDown:
            // gamepad
            case InputActionKey.Gamepad_RStick_Y:
            // Keyboard
            case InputActionKey.Keyboard_Pressed_UpArrow:
            case InputActionKey.Keyboard_Pressed_DownArrow:
            case InputActionKey.Keyboard_Release_UpArrow:
            case InputActionKey.Keyboard_Release_DownArrow:
            // UI
            case InputActionKey.UI_RStick_Y:
                m_Input_ValueRY = m_Replay_CurrentInfo.value;
                break;


            // 他 機能
            // ビジョンセンサー
            case InputActionKey.Proc_VisionSensor_On:
                m_IsVisionSensor = true;
                break;
            case InputActionKey.Proc_VisionSensor_Off:
                m_IsVisionSensor = false;
                break;
            // フリップ    
            case InputActionKey.Proc_Frip_Forward:
                SetProcFripSetting(InputDirId.Up);
                break;
            case InputActionKey.Proc_Frip_Back:
                SetProcFripSetting(InputDirId.Down);
                break;
            case InputActionKey.Proc_Frip_Right:
                SetProcFripSetting(InputDirId.Right);
                break;
            case InputActionKey.Proc_Frip_Left:
                SetProcFripSetting(InputDirId.Left);
                break;
            // メトロノーム
            case InputActionKey.Proc_Metronome_On:
                m_MetronomeCtrl.SetMetronome(true);
                break;
            case InputActionKey.Proc_Metronome_Off:
                m_MetronomeCtrl.SetMetronome(false);
                break;
            // ヘッドレス
            case InputActionKey.Proc_Headless_On:
                m_IsHeadless = true;
                break;
            case InputActionKey.Proc_Headless_Off:
                m_IsHeadless = false;
                break;
            // 自動離着陸
            case InputActionKey.Proc_AutoFly_On:
                m_IsAutoFlyAction = true;
                break;
            case InputActionKey.Proc_AutoFly_On_MonterOnWait:
                m_IsAutoFlyAction = true;
                m_IsAutoFly_MonterOnWait = true;
                // プロペラ設定
                m_PropellerCtrl.ProcMonterOn();
                m_IsNeutral = false;
                // StartCoroutine(ProcAutoFlyPropellerWait());
                break;
            case InputActionKey.Proc_AutoFly_Off:
                m_IsAutoFlyAction = false;
                break;
            case InputActionKey.Proc_AutoFly_On_MonterOnWait_Off:
                m_IsAutoFly_MonterOnWait = false;
                ProcDrone_ChangeDroneAction_Fly();
                break;
            case InputActionKey.Proc_AutoLanding_On:
                m_IsAutoLandingAction = true;
                break;
            // 速度
            case InputActionKey.Proc_Spd_Change1:
                m_SpdChangeMag = 1.0f;
                break;
            case InputActionKey.Proc_Spd_Change2:
                m_SpdChangeMag = 1.25f;
                break;
            case InputActionKey.Proc_Spd_Change3:
                m_SpdChangeMag = 1.5f;
                break;
        }
    }


    //================================================
    // [///] フリップ関連
    //================================================
    protected override void SetProcFripSetting(InputDirId id)
    {
        m_IsFripWait = false;

        m_AddPosYSpd = 0.0f;
        m_AddPosXSpd = 0.0f;
        m_AddPosZSpd = 0.0f;
        m_AddRotX = 0.0f;
        m_AddRotYSpd = 0.0f;
        m_AddRotZ = 0.0f;
        m_Input_ValueLX = 0.0f;
        m_Input_ValueLY = 0.0f;
        m_Input_ValueRX = 0.0f;
        m_Input_ValueRY = 0.0f;

        ShInputManager.I.GetInputCanvas().GetPadLCtrl().SetPad(0.0f, 0.0f);
        ShInputManager.I.GetInputCanvas().GetPadRCtrl().SetPad(0.0f, 0.0f);

        // ヘッドレス時
        if (m_IsHeadless)
        {
            // ドローンの向きから方向を設定
            float rot = transform.localEulerAngles.y;
            rot = rot % 360.0f;
            if (rot > 0.0f)
            {
                if (rot < 45.0f)    // ↑
                {
                    // そのまま
                }
                else if (rot < 135.0f)   // →
                {
                    switch (id)
                    {
                        case InputDirId.Up: id = InputDirId.Left; break;
                        case InputDirId.Down: id = InputDirId.Right; break;
                        case InputDirId.Right: id = InputDirId.Up; break;
                        case InputDirId.Left: id = InputDirId.Down; break;
                    }
                }
                else if (rot < 225.0f)   // ↓
                {
                    switch (id)
                    {
                        case InputDirId.Up: id = InputDirId.Down; break;
                        case InputDirId.Down: id = InputDirId.Up; break;
                        case InputDirId.Right: id = InputDirId.Left; break;
                        case InputDirId.Left: id = InputDirId.Right; break;
                    }
                }
                else if (rot < 325.0f)   // ←
                {
                    switch (id)
                    {
                        case InputDirId.Up: id = InputDirId.Right; break;
                        case InputDirId.Down: id = InputDirId.Left; break;
                        case InputDirId.Right: id = InputDirId.Down; break;
                        case InputDirId.Left: id = InputDirId.Up; break;
                    }
                }
                else if (rot < 360.0f)   // ↑
                {
                    // そのまま
                }
            }
            else
            {
                if (rot > -45.0f)    // ↑
                {
                    // そのまま
                }
                else if (rot > -135.0f)   // ←
                {
                    switch (id)
                    {
                        case InputDirId.Up: id = InputDirId.Right; break;
                        case InputDirId.Down: id = InputDirId.Left; break;
                        case InputDirId.Right: id = InputDirId.Down; break;
                        case InputDirId.Left: id = InputDirId.Up; break;
                    }
                }
                else if (rot > -225.0f)   // ↓
                {
                    switch (id)
                    {
                        case InputDirId.Up: id = InputDirId.Down; break;
                        case InputDirId.Down: id = InputDirId.Up; break;
                        case InputDirId.Right: id = InputDirId.Left; break;
                        case InputDirId.Left: id = InputDirId.Right; break;
                    }
                }
                else if (rot > -325.0f)   // →
                {
                    switch (id)
                    {
                        case InputDirId.Up: id = InputDirId.Left; break;
                        case InputDirId.Down: id = InputDirId.Right; break;
                        case InputDirId.Right: id = InputDirId.Up; break;
                        case InputDirId.Left: id = InputDirId.Down; break;
                    }
                }
                else if (rot > -360.0f)   // ↑
                {
                    // そのまま
                }
            }
        }

        // リプレイログ
        AddReplayLog(InputActionKey.Proc_Frip_Forward + (int)id);

        m_FripDirId = id;
        {
            switch (m_FripDirId)
            {
                case InputDirId.Up:
                    m_FripRotSt = Vector3.right * -0.0001f;
                    m_FripRotEnd = Vector3.right * -359.99f;
                    break;
                case InputDirId.Down:
                    m_FripRotSt = Vector3.right * 0.0001f;
                    m_FripRotEnd = Vector3.right * 359.99f;
                    break;
                case InputDirId.Right:
                    m_FripRotSt = Vector3.forward * 0.0001f;
                    m_FripRotEnd = Vector3.forward * 359.99f;
                    break;
                case InputDirId.Left:
                    m_FripRotSt = Vector3.forward * -0.0001f;
                    m_FripRotEnd = Vector3.forward * -359.99f;
                    break;
            }
        }

        m_FripIsRotEnd = false;
        m_FripTimer = 0.0f;
        m_IsFripAction = true;

        // UIのボタン動作設定
        ShInputManager.I.GetInputCanvas().SetBtnInteractable_Frip(false);
        ShInputManager.I.GetInputCanvas().SetBtnActive_Frip(false);
    }

    protected override void ProcFripUpdate()
    {
        if (m_FripTimer < 1.0f)
        {
            // 回転
            if (0.2f < m_FripTimer && m_FripTimer < 0.8f)
            {
                float tempTime = m_FripTimer - 0.2f;
                m_Model.transform.localEulerAngles = Vector3.Lerp(m_FripRotSt, m_FripRotEnd, tempTime / 0.6f);
            }
            else if (m_FripTimer > 0.8f)
            {
                if (!m_FripIsRotEnd)
                {
                    m_FripIsRotEnd = true;
                    m_Model.transform.localEulerAngles = Vector3.zero;
                }
            }

            // 位置更新
            if (m_FripTimer < 0.25f)
            {
                transform.position += Vector3.up * Time.fixedDeltaTime * 4.0f * 2.5f;
            }
            else if (m_FripTimer > 0.75f)
            {
                transform.position += Vector3.down * Time.fixedDeltaTime * 4.0f * 2.25f;
            }
            else
            {
                // 位置
                switch (m_FripDirId)
                {
                    case InputDirId.Up:
                        transform.position += transform.forward * -1.0f * Time.fixedDeltaTime * 3.0f;
                        break;

                    case InputDirId.Down:
                        transform.position += transform.forward * 1.0f * Time.fixedDeltaTime * 3.0f;
                        break;

                    case InputDirId.Right:
                        transform.position += transform.right * -1.0f * Time.fixedDeltaTime * 3.0f;
                        break;

                    case InputDirId.Left:
                        transform.position += transform.right * 1.0f * Time.fixedDeltaTime * 3.0f;
                        break;
                }
                if (m_FripTimer < 0.5f)
                {
                    transform.position += Vector3.up * Time.fixedDeltaTime * 6.0f;
                }
                else if (m_FripTimer > 0.5f)
                {
                    transform.position += Vector3.down * Time.fixedDeltaTime * 6.0f;
                }
            }
        }
        else
        {
            if (m_FripTimer > 1.0f)
            {
                m_Model.transform.localEulerAngles = Vector3.zero;
                m_IsFripAction = false;
                // UIのボタン動作設定
                ShInputManager.I.GetInputCanvas().SetBtnInteractable_Frip(true);
            }
        }

        m_FripTimer += Time.fixedDeltaTime;
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
                        // 自動離陸
                        if (m_IsAutoFlyAction)
                        {
                            // 下入力があった場合：キャンセル
                            if (m_Input_ValueLY < -0.2f)
                            {
                                ProcAutoFlyLandingOff();
                            }
                            else
                            {
                                // 強制上昇
                                m_Input_ValueLY = 1.0f;
                            }
                        }
                        // 自動着陸
                        if (m_IsAutoLandingAction)
                        {
                            // 上入力があった場合：キャンセル
                            if (m_Input_ValueLY > 0.2f)
                            {
                                ProcAutoFlyLandingOff();
                            }
                            else
                            {
                                // 強制下降
                                m_Input_ValueLY = -1.0f;
                            }
                        }

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
                        // フリップ
                        if (m_IsFripWait && !m_IsFripAction)
                        {
                            if (m_Input_ValueLY > 0.4f)
                            {
                                SetProcFripSetting(InputDirId.Down);
                            }
                            else if(m_Input_ValueLY < -0.4f)
                            {
                                SetProcFripSetting(InputDirId.Up);
                            }
                        }

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
                        // フリップ
                        if (m_IsFripWait && !m_IsFripAction)
                        {
                            if (m_Input_ValueRY > 0.4f)
                            {
                                SetProcFripSetting(InputDirId.Down);
                            }
                            else if (m_Input_ValueRY < -0.4f)
                            {
                                SetProcFripSetting(InputDirId.Up);
                            }
                        }

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
                        // 自動離陸
                        if (m_IsAutoFlyAction)
                        {
                            // 下入力があった場合：キャンセル
                            if(m_Input_ValueRY < -0.2f)
                            {
                                ProcAutoFlyLandingOff();
                            }
                            else
                            {
                                // 強制上昇
                                m_Input_ValueRY = 1.0f;
                            }
                        }
                        // 自動着陸
                        if (m_IsAutoLandingAction)
                        {
                            // 上入力があった場合：キャンセル
                            if (m_Input_ValueRY > 0.2f)
                            {
                                ProcAutoFlyLandingOff();
                            }
                            else
                            {
                                // 強制下降
                                m_Input_ValueRY = -1.0f;
                            }
                        }

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
                    // フリップ
                    if (m_IsFripWait && !m_IsFripAction)
                    {
                        // →
                        if (m_Input_ValueRX > 0.4f)
                        {
                            SetProcFripSetting(InputDirId.Left);
                        }
                        else if (m_Input_ValueRX < -0.4f)
                        {
                            SetProcFripSetting(InputDirId.Right);
                        }
                    }

                    // 移動
                    ProcDrone_MoveX(m_Input_ValueRX);

                    // 傾き
                    ProcDrone_MoveRotRight(m_Input_ValueRX);
                }
                break;
        }
    }
    #endregion

    // 風の更新
    private void ProcWindUpdate()
    {
        {
            m_WindChangeCnt++;

            // 時間更新
            float time = (m_WindSeed + m_WindSeed * m_WindChangeCnt * 7) % 90;
            m_WindUpdateTime += time * 0.1f;


            // 方向設定
            WindDirNo no = (WindDirNo)((m_WindSeed + m_WindSeed * m_WindChangeCnt * 3) % 8);
            switch (no)
            {
                case WindDirNo.Up:          m_WindNextAddForce = Vector3.forward; break;
                case WindDirNo.Down:        m_WindNextAddForce = Vector3.back; break;
                case WindDirNo.Left:        m_WindNextAddForce = Vector3.left; break;
                case WindDirNo.Right:       m_WindNextAddForce = Vector3.right; break;

                case WindDirNo.RightUp:     m_WindNextAddForce = Vector3.right + Vector3.forward; break;
                case WindDirNo.RightDown:   m_WindNextAddForce = Vector3.right + Vector3.back; break;
                case WindDirNo.LeftUp:      m_WindNextAddForce = Vector3.left + Vector3.forward; break;
                case WindDirNo.LeftDown:    m_WindNextAddForce = Vector3.left + Vector3.back; break;
            }
            //Debug.Log("WindUpdate!!!!!! Dir : " + no.ToString());
            //Debug.Log("WindUpdate!!!!!! Next : " + m_WindUpdateTime.ToString("f2"));

            // 速度設定 0.5 ～ 3.0
            float spd = (m_WindSeed + m_WindSeed * m_WindChangeCnt * 5) % 25;
            m_WindNextSpd = 0.5f + spd * 0.1f;

            // UI
            MainGame.MG_Mediator.MainCanvas.SetWindUI(m_WindNextSpd, no);

            StartCoroutine(ProcWindDirUpdate());
        }
    }
    private IEnumerator ProcWindDirUpdate()
    {
        float addTime = 0.0f;
        float endTime = 1.0f;

        float stSpd = m_WindSpd;
        Vector3 stAddForce = m_WindAddForce;
        while (true)
        {
            addTime += Time.fixedDeltaTime;
            m_WindSpd = Vector2.Lerp(Vector2.one * stSpd, Vector2.one * m_WindNextSpd, addTime / endTime).x;
            m_WindAddForce = Vector3.Lerp(stAddForce, m_WindNextAddForce, addTime / endTime);
            if(addTime > endTime)
            {
                m_WindSpd = m_WindNextSpd;
                m_WindAddForce = m_WindNextAddForce;
                break;
            }
            yield return null;
        }
    }

    // auto
    private void ProcAutoFlyLandingOff()
    {
        m_IsAutoFlyAction = false;
        m_IsAutoLandingAction = false;
        // UI
        ShInputManager.I.GetInputCanvas().SetBtnInteractable_AutoFly(true);
        AddReplayLog(InputActionKey.Proc_AutoFly_Off);
    }

    //================================================
    // [///] インプット関連
    //================================================
    // 入力
    public void ProcInput()
    {
        // ミッションの終了判定:終了している場合入力無効
        if (m_IsMissionEnd) return;

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
                    if (!m_IsFripAction && !m_IsAutoFly_MonterOnWait)
                    {
                        ProcInput_Keyboard();
                    }
                    ProcInput_Keyboard_ProcAction();
                }
                // コントローラー
                else
                {
                    if (!m_IsFripAction && !m_IsAutoFly_MonterOnWait)
                    {
                        ProcInput_Controller();
                    }
                    ProcInput_Gamepad_ProcAction();
                }
            }

            SetTouch();
        }

        // 自動離陸の終了確認
        if (m_IsAutoFlyAction)
        {
            // 6.0f + dampで = 8.9f程
            if(transform.position.y > 6.0f) {
                ProcAutoFlyLandingOff();
            }
        }

        // フリップの更新
        if (m_IsFripAction)
        {
            ProcFripUpdate();
        }

        // ミッションの更新
        if(AppData.m_PlayMode == AppData.PlayMode.Mission)
        {
            ProcMissionCheck();
        }

        // // 風の更新 @20240819 プレイ中一定に変更(更新無し)
        // if (m_WindUpdateTime < ShInputManager.I.GetTimerCtrl().GetTime())
        // {
        //     ProcWindUpdate();
        // }


        // 挙動後に風の影響の対応
        if (!m_IsVisionSensor)
        {
            // 飛行中
            if (m_CurrentDroneActionId == DroneActionId.Fly)
            {
                // ビジョンセンサーOFF時に移動させる
                transform.localPosition += m_WindAddForce * Time.fixedDeltaTime;
            }
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
            if (AppData.GetTrg_KeyOnly(AppData.Action.Metronome))
            {
                ChangeMetronome();
            }
            else if (AppData.GetTrg_KeyOnly(AppData.Action.Grid))
            {
                ChangeGrid();
            }
            else if (AppData.GetTrg_KeyOnly(AppData.Action.Flip))
            {
                ChangeFrip();
            }
            else if (AppData.GetTrg_KeyOnly(AppData.Action.Speed))
            {
                ChangeSpd();
            }
            else if (AppData.GetTrg_KeyOnly(AppData.Action.Headless))
            {
                ChangeHeadless();
            }
            else if (AppData.GetTrg_KeyOnly(AppData.Action.VisionSensor))
            {
                ChangeVisionSensor();
            }
            else if (AppData.GetTrg_KeyOnly(AppData.Action.AutoTakeoffLanding))
            {
                ChangeAutoFly();
            }
        }
    }
    // ゲームパッドの機能対応
    private void ProcInput_Gamepad_ProcAction()
    {
        // 機能ボタンのチェック
        {
            if (AppData.GetPadTrg(AppData.Action.Metronome))
            {
                ChangeMetronome();
            }
            else if (AppData.GetPadTrg(AppData.Action.Grid))
            {
                ChangeGrid();
            }
            else if (AppData.GetPadTrg(AppData.Action.Flip))
            {
                ChangeFrip();
            }
            else if (AppData.GetPadTrg(AppData.Action.Speed))
            {
                ChangeSpd();
            }
            else if (AppData.GetPadTrg(AppData.Action.Headless))
            {
                ChangeHeadless();
            }
            else if (AppData.GetPadTrg(AppData.Action.VisionSensor))
            {
                ChangeVisionSensor();
            }
            else if (AppData.GetPadTrg(AppData.Action.AutoTakeoffLanding))
            {
                ChangeAutoFly();
            }
        }

        // パッド入力情報初期化
        AppData.ResetPadTrgs();
    }
    #endregion

    //================================================
    // [///] ミッション
    //================================================
    private void ProcMissionCheck()
    {
        // 離陸の場合
        if(m_CurrentMissionId == MissionId.Mission_Fly)
        {
            if (transform.position.y > 6.0f)
            {
                m_IsNextMoveDirChecking = true;

                if (AppData.m_MissionId == AppData.MissionID.SquareFly)
                {
                    UpdateMission(MissionId.Mission_A, 1);
                }
                else if (AppData.m_MissionId == AppData.MissionID.EightFly)
                {
                    m_IsNextMoveDirChecking = false;
                    UpdateMission(MissionId.Mission_Eight1, 1);
                }
                else if (AppData.m_MissionId == AppData.MissionID.HappningFly)
                {
                    UpdateMission(MissionId.Mission_A, 1);
                }
            }
        }
        // 着陸の場合
        else if(m_CurrentMissionId == MissionId.Mission_LandingAndStop)
        {
            if (m_CurrentDroneActionId == DroneActionId.Wait)
            {
                if (m_PropellerCtrl.IsPropellerOff())
                {
                    CompleteMission();
                }
            }
        }
        else
        {
            // ふらつき
            {
                // 1秒チェック
                m_MissionHuratukiCheckTime += Time.fixedDeltaTime;
                if(m_MissionHuratukiCheckTime > 1.0f)
                {
                    m_MissionHuratukiCheckTime = 0.0f;
                    m_IsMissionHuratukiInputPosX1 = false;
                    m_IsMissionHuratukiInputPosX2 = false;
                    m_IsMissionHuratukiInputPosZ1 = false;
                    m_IsMissionHuratukiInputPosZ2 = false;
                    m_IsMissionHuratukiInputRotY1 = false;
                    m_IsMissionHuratukiInputRotY2 = false;
                }
                else
                {
                    // 横
                    if (m_Input_ValueRX > 0.2f) m_IsMissionHuratukiInputPosX1 = true;
                    else if (m_Input_ValueRX < -0.2f) m_IsMissionHuratukiInputPosX2 = true;
                    if (m_IsMissionHuratukiInputPosX1 && m_IsMissionHuratukiInputPosX2)
                    {
                        // ふらつき
                        SetUpdateMissionPoint_Huratuki();
                        m_MissionHuratukiCheckTime = 1.0f;  // 次にまわす
                    }

                    // 横
                    if (m_Input_ValueLX > 0.2f) m_IsMissionHuratukiInputRotY1 = true;
                    else if (m_Input_ValueLX < -0.2f) m_IsMissionHuratukiInputRotY2 = true;
                    if (m_IsMissionHuratukiInputRotY1 && m_IsMissionHuratukiInputRotY2)
                    {
                        // ふらつき
                        SetUpdateMissionPoint_Huratuki();
                        m_MissionHuratukiCheckTime = 1.0f;  // 次にまわす
                    }

                    if (AppData.ConnectMode == AppData.ConnectM.Mode2)
                    {
                        // 縦
                        if (m_Input_ValueRY > 0.2f) m_IsMissionHuratukiInputPosZ1 = true;
                        else if (m_Input_ValueRY < -0.2f) m_IsMissionHuratukiInputPosZ2 = true;
                        if (m_IsMissionHuratukiInputPosZ1 && m_IsMissionHuratukiInputPosZ2)
                        {
                            // ふらつき
                            SetUpdateMissionPoint_Huratuki();
                            m_MissionHuratukiCheckTime = 1.0f;  // 次にまわす
                        }
                    }
                }
            }

            // 高さ
            if (!m_IsMissionFlyCheck)
            {
                if(m_AddPosYSpd <= 0.0f)
                {
                    m_IsMissionFlyCheck = true;
                    m_MissionFlyHeight = transform.position.y;
                }
            }
            else
            {
                if (-0.5f > (transform.position.y - m_MissionFlyHeight) ||
                   0.5f < (transform.position.y - m_MissionFlyHeight))
                {
                    // ふらつき
                    SetUpdateMissionPoint_Huratuki();
                    m_MissionFlyHeight = transform.position.y;
                }
            }
        }

        // 壁に当たった(移動限界)
        if (m_IsMoveLimitCheck)
        {
            // ミッション中止:危険な飛行,墜落,飛行空域離脱 中止
            FailedMission();
        }

        // 減点項目
        // if (AppData.m_MissionId == AppData.MissionID.SquareFly)
        {
            // 逆方向に移動したか
            if (m_IsNextMoveDirChecking && !m_IsNextMoveDirError)
            {
                if (IsDiffMoveDir(m_PrevPos, m_NextMoveDir))
                {
                    m_IsNextMoveDirError = true;
                    // 減点
                    SetUpdateMissionPoint_DefectiveDir();
                }
            }
        }

        // タイマー:時間停止中かつリザルト表示されていない場合
        if (ShInputManager.I.GetTimerCtrl().GetTime() >= 300.0f && !m_IsMissionEnd)
        {
            // ミッション終了
            FailedMission();
        }
    }

    // 得点減点:飛行経路逸脱
    private void SetUpdateMissionPoint_Outarea()
    {
        if (m_IsMissionEnd) return;
        
        // スコア減算
        m_MissionPoint += -5;
        // UI
        MainGame.MG_Mediator.MainCanvas.GetPointInfo().SetPoint(m_MissionPoint);
        // UI
        MainGame.MG_Mediator.MainCanvas.SetMissionMinus(-5, MissionSubtractPointId.OutArea);
    }
    // 得点減点:指示と異なる飛行
    private void SetUpdateMissionPoint_DiffInstruction()
    {
        if (m_IsMissionEnd) return;

        // スコア減算
        m_MissionPoint += -5;
        // UI
        MainGame.MG_Mediator.MainCanvas.GetPointInfo().SetPoint(m_MissionPoint);
        // UI
        MainGame.MG_Mediator.MainCanvas.SetMissionMinus(-5, MissionSubtractPointId.DiffInstruction);

        // ui
        Debug.Log("減点 : " + MissionSubtractPointId.DiffInstruction);
    }
    // 得点減点:機首方向不良
    private void SetUpdateMissionPoint_DefectiveDir()
    {
        if (m_IsMissionEnd) return;

        // スコア減算
        m_MissionPoint += -1;
        // UI
        MainGame.MG_Mediator.MainCanvas.GetPointInfo().SetPoint(m_MissionPoint);
        // UI
        MainGame.MG_Mediator.MainCanvas.SetMissionMinus(-1, MissionSubtractPointId.DefectiveDir);

        // ui
        Debug.Log("減点 : " + MissionSubtractPointId.DefectiveDir);
    }
    // 得点減点:ふらつき
    private void SetUpdateMissionPoint_Huratuki()
    {
        if (m_IsMissionEnd) return;

        // スコア減算
        m_MissionPoint += -1;
        // UI
        MainGame.MG_Mediator.MainCanvas.GetPointInfo().SetPoint(m_MissionPoint);
        // UI
        MainGame.MG_Mediator.MainCanvas.SetMissionMinus(-1, MissionSubtractPointId.Huratuki);

        // ui
        Debug.Log("減点 : " + MissionSubtractPointId.Huratuki);
    }
    // 得点減点:不円滑
    private void SetUpdateMissionPoint_HuEnkatu()
    {
        if (m_IsMissionEnd) return;

        // スコア減算
        m_MissionPoint += -1;
        // UI
        MainGame.MG_Mediator.MainCanvas.GetPointInfo().SetPoint(m_MissionPoint);
        // UI
        MainGame.MG_Mediator.MainCanvas.SetMissionMinus(-1, MissionSubtractPointId.HuEnkatu);

        // ui
        Debug.Log("減点 : " + MissionSubtractPointId.HuEnkatu);
    }

    //================================================
    // [///] ログ
    //================================================
    // →baseに格納 変化をつける場合、継承する
    // public override void AddReplayLog(InputActionKey key, float value = 0.0f)
    // {
    // }
    // // ローカル保存
    // public override void SaveLocalReplayData()
    // {
    // }

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

                // ミッション時
                if(AppData.m_PlayMode == AppData.PlayMode.Mission)
                {
                    // 着陸時
                    if(m_CurrentMissionId == MissionId.Mission_LandingAndStop)
                    {
                        // 移動があった場合ふらつき判定
                        if(m_AddPosXSpd > 0.02f && m_AddPosZSpd > 0.02f)
                        {
                            //減点
                            SetUpdateMissionPoint_Huratuki();
                        }
                        // 回転があった場合ふらつき判定
                        if (m_AddRotYSpd > 0.02f)
                        {
                            //減点
                            SetUpdateMissionPoint_Huratuki();
                        }
                    }
                }

                // m_Rigid.velocity = Vector3.zero;

                // se
                // 飛行中SE
                // if (m_IsFlyingSEPlay)
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

                // 自動着陸のOFF
                if (m_IsAutoLandingAction)
                {
                    ProcAutoFlyLandingOff();

                    // Waitまで戻す
                    ProcDrone_ChangeDroneAction_MoterOff();
                }

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

                    MainGame.MG_Mediator.MainCanvas.GetVerticalSpdInfo().SetVerticalSpd(0.0f);
                    MainGame.MG_Mediator.MainCanvas.GetVerticalSpdInfo().SetHorizontalSpd(0.0f);
                }
            }
        }
    }
    // スクエア飛行
    public void CheckMissionTrigger_Square(HitCheck_Mission hitcheckCtrl)
    {
        // outarea 地点A～H地点まで 中心はアウト
        if (1 < m_MissionCnt && m_MissionCnt < 7)
        {
            if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.OutArea))
            {
                if (!m_IsOutAreaIn)
                {
                    m_IsOutAreaIn = true;
                    // アウト地点に入った
                    MainGame.MG_Mediator.MainCanvas.SetOutArea(true);
                    // 減算
                    SetUpdateMissionPoint_Outarea();
                }
            }
        }

        switch (m_CurrentMissionId)
        {
            case MissionId.Mission_A:
                if (m_MissionCnt == 1)
                {
                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_A))
                    {
                        // 機首が異なる
                        if (!IsAngleDown())
                        {
                            // 減点
                            SetUpdateMissionPoint_DiffInstruction();
                        }

                        // 次の方向
                        m_NextMoveDir = Vector3.right;

                        UpdateMission(MissionId.Mission_B, 2);
                    }
                }
                else
                {
                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_A))
                    {
                        // 機首が異なる
                        if (!IsAngleRight())
                        {
                            // 減点
                            SetUpdateMissionPoint_DiffInstruction();
                        }

                        // 次の方向
                        m_NextMoveDir = Vector3.forward;

                        UpdateMission(MissionId.Mission_H, 7);
                    }
                    break;
                }
                break;

            case MissionId.Mission_B:
                if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_B))
                {
                    // 機首が異なる
                    if (!IsAngleRight())
                    {
                        // 減点
                        SetUpdateMissionPoint_DiffInstruction();
                    }

                    // 次の方向
                    m_NextMoveDir = Vector3.forward;

                    UpdateMission(MissionId.Mission_C, 3);
                }
                else if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Check_AB))
                {
                    if (!m_IsMissionDirCheck)
                    {
                        m_IsMissionDirCheck = true;
                        // 機首が異なる
                        if (!IsAngleRight())
                        {
                            // 減点
                            SetUpdateMissionPoint_DefectiveDir();
                        }
                    }
                }
                break;
            case MissionId.Mission_C:
                if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_C))
                {
                    // 機首が異なる
                    if (!IsAngleUp())
                    {
                        // 減点
                        SetUpdateMissionPoint_DiffInstruction();
                    }

                    // 次の方向
                    m_NextMoveDir = Vector3.left;

                    UpdateMission(MissionId.Mission_D, 4);
                }
                else if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Check_BC))
                {
                    if (!m_IsMissionDirCheck)
                    {
                        m_IsMissionDirCheck = true;
                        // 機首が異なる
                        if (!IsAngleUp())
                        {
                            // 減点
                            SetUpdateMissionPoint_DefectiveDir();
                        }
                    }
                }
                break;
            case MissionId.Mission_D:
                if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_D))
                {
                    // 機首が異なる
                    if (!IsAngleLeft())
                    {
                        // 減点
                        SetUpdateMissionPoint_DiffInstruction();
                    }

                    // 次の方向
                    m_NextMoveDir = Vector3.back;

                    UpdateMission(MissionId.Mission_E, 5);
                }
                else if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Check_CD))
                {
                    if (!m_IsMissionDirCheck)
                    {
                        m_IsMissionDirCheck = true;
                        // 機首が異なる
                        if (!IsAngleLeft())
                        {
                            // 減点
                            SetUpdateMissionPoint_DefectiveDir();
                        }
                    }
                }
                break;
            case MissionId.Mission_E:
                if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_E))
                {
                    // 機首が異なる
                    if (!IsAngleDown())
                    {
                        // 減点
                        SetUpdateMissionPoint_DiffInstruction();
                    }

                    // 次の方向
                    m_NextMoveDir = Vector3.right;

                    UpdateMission(MissionId.Mission_A, 6);
                }
                else if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Check_DE))
                {
                    if (!m_IsMissionDirCheck)
                    {
                        m_IsMissionDirCheck = true;
                        // 機首が異なる
                        if (!IsAngleDown())
                        {
                            // 減点
                            SetUpdateMissionPoint_DefectiveDir();
                        }
                    }
                }
                break;
            case MissionId.Mission_H:
                if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_H))
                {
                    // 機首が異なる
                    if (!IsAngleUp())
                    {
                        // 減点
                        SetUpdateMissionPoint_DiffInstruction();
                    }

                    m_IsNextMoveDirChecking = false;

                    UpdateMission(MissionId.Mission_LandingAndStop, 8);
                }
                break;
        }
    }
    // 8の字飛行
    public void CheckMissionTrigger_Eight(HitCheck_Mission hitcheckCtrl)
    {
        {
            if (AppData.m_MissionId == AppData.MissionID.EightFly)
            {
                // outarea
                {
                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.OutArea))
                    {
                        if (!m_IsOutAreaIn)
                        {
                            m_IsOutAreaIn = true;
                            // アウト地点に入った
                            MainGame.MG_Mediator.MainCanvas.SetOutArea(true);
                            // 減点
                            SetUpdateMissionPoint_Outarea();
                        }
                    }
                }

                switch (m_CurrentMissionId)
                {
                    case MissionId.Mission_Eight1:  // 8の字0.5周まで
                    case MissionId.Mission_Eight3:
                        if (m_MissionSubCnt == 0)
                        {
                            // 左上
                            if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Eight_Left3))
                            {
                                m_IsEightFly_StR = false;

                                // 
                                m_MissionSubCnt = 1;
                                Debug.Log("Check : " + m_MissionSubCnt);

                                // 機首が異なる
                                if (!IsAngleLeft())
                                {
                                    // 減点
                                    SetUpdateMissionPoint_DefectiveDir();
                                }

                                // 次の方向
                                m_NextMoveDir = Vector3.down;
                            }
                            if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Eight_Right3))
                            {
                                m_IsEightFly_StR = true;

                                // 
                                m_MissionSubCnt = 1;
                                Debug.Log("Check : " + m_MissionSubCnt);

                                // 機首が異なる
                                if (!IsAngleRight())
                                {
                                    // 減点
                                    SetUpdateMissionPoint_DefectiveDir();
                                }

                                // 次の方向
                                m_NextMoveDir = Vector3.down;
                            }
                        }
                        // 中央は共通
                        else if (m_MissionSubCnt == 3)
                        {
                            // 中央
                            if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_H))
                            {
                                // 
                                m_MissionSubCnt = 0;
                                Debug.Log("Check : " + m_MissionSubCnt);

                                // 機首が異なる
                                if (!IsAngleUp())
                                {
                                    // 減点
                                    SetUpdateMissionPoint_DiffInstruction();
                                }

                                // 次の方向
                                m_NextMoveDir = Vector3.left;

                                if (m_CurrentMissionId == MissionId.Mission_Eight1)
                                {
                                    UpdateMission(MissionId.Mission_Eight2, 2);
                                }
                                else
                                {
                                    UpdateMission(MissionId.Mission_Eight4, 4);
                                }
                            }
                        }
                        else
                        {
                            // 右からか
                            if (m_IsEightFly_StR)
                            {
                                if (m_MissionSubCnt == 1)
                                {
                                    // 右
                                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Eight_Right2))
                                    {
                                        // 
                                        m_MissionSubCnt = 2;
                                        Debug.Log("Check : " + m_MissionSubCnt);

                                        // 機首が異なる
                                        if (!IsAngleDown())
                                        {
                                            // 減点
                                            SetUpdateMissionPoint_DefectiveDir();
                                        }

                                        // 次の方向
                                        m_NextMoveDir = Vector3.left;
                                    }
                                }
                                else if (m_MissionSubCnt == 2)
                                {
                                    // 右下
                                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Eight_Right1))
                                    {
                                        // 
                                        m_MissionSubCnt = 3;
                                        Debug.Log("Check : " + m_MissionSubCnt);

                                        // 機首が異なる
                                        if (!IsAngleLeft())
                                        {
                                            // 減点
                                            SetUpdateMissionPoint_DefectiveDir();
                                        }

                                        // 次の方向
                                        m_NextMoveDir = Vector3.up;
                                    }
                                }
                            }
                            // ←からか
                            else if (!m_IsEightFly_StR)
                            {
                                if (m_MissionSubCnt == 1)
                                {
                                    // 右
                                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Eight_Left2))
                                    {
                                        // 
                                        m_MissionSubCnt = 2;
                                        Debug.Log("Check : " + m_MissionSubCnt);

                                        // 機首が異なる
                                        if (!IsAngleDown())
                                        {
                                            // 減点
                                            SetUpdateMissionPoint_DefectiveDir();
                                        }

                                        // 次の方向
                                        m_NextMoveDir = Vector3.right;
                                    }
                                }
                                else if (m_MissionSubCnt == 2)
                                {
                                    // 右下
                                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Eight_Left1))
                                    {
                                        // 
                                        m_MissionSubCnt = 3;
                                        Debug.Log("Check : " + m_MissionSubCnt);

                                        // 機首が異なる
                                        if (!IsAngleRight())
                                        {
                                            // 減点
                                            SetUpdateMissionPoint_DefectiveDir();
                                        }

                                        // 次の方向
                                        m_NextMoveDir = Vector3.up;
                                    }
                                }
                            }
                        }
                        break;

                    case MissionId.Mission_Eight2:  // 8の字1.0周まで
                    case MissionId.Mission_Eight4:
                        // 中央は共通
                        if (m_MissionSubCnt == 3)
                        {
                            // 中央
                            if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_H))
                            {
                                // 
                                m_MissionSubCnt = 0;
                                Debug.Log("Check : " + m_MissionSubCnt);

                                // 機首が異なる
                                if (!IsAngleDown())
                                {
                                    // 減点
                                    SetUpdateMissionPoint_DiffInstruction();
                                }

                                if (m_IsEightFly_StR)
                                {
                                    // 次の方向
                                    m_NextMoveDir = Vector3.left;
                                }
                                else
                                {
                                    // 次の方向
                                    m_NextMoveDir = Vector3.right;
                                }

                                if (m_CurrentMissionId == MissionId.Mission_Eight2)
                                {
                                    UpdateMission(MissionId.Mission_Eight3, 3);
                                }
                                else
                                {
                                    m_IsNextMoveDirChecking = false;

                                    UpdateMission(MissionId.Mission_LandingAndStop, 5);
                                }
                            }
                        }
                        else
                        {
                            // 右からか：の場合左した
                            if (m_IsEightFly_StR)
                            {
                                if (m_MissionSubCnt == 0)
                                {
                                    // 左下
                                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Eight_Left3))
                                    {
                                        // 
                                        m_MissionSubCnt = 1;
                                        Debug.Log("Check : " + m_MissionSubCnt);

                                        // 機首が異なる
                                        if (!IsAngleLeft())
                                        {
                                            // 減点
                                            SetUpdateMissionPoint_DefectiveDir();
                                        }

                                        // 次の方向
                                        m_NextMoveDir = Vector3.up;
                                    }
                                }
                                else if (m_MissionSubCnt == 1)
                                {
                                    // 右
                                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Eight_Left2))
                                    {
                                        // 
                                        m_MissionSubCnt = 2;
                                        Debug.Log("Check : " + m_MissionSubCnt);

                                        // 機首が異なる
                                        if (!IsAngleUp())
                                        {
                                            // 減点
                                            SetUpdateMissionPoint_DefectiveDir();
                                        }

                                        // 次の方向
                                        m_NextMoveDir = Vector3.right;
                                    }
                                }
                                else if (m_MissionSubCnt == 2)
                                {
                                    // 右上
                                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Eight_Left1))
                                    {
                                        // 
                                        m_MissionSubCnt = 3;
                                        Debug.Log("Check : " + m_MissionSubCnt);

                                        // 機首が異なる
                                        if (!IsAngleRight())
                                        {
                                            // 減点
                                            SetUpdateMissionPoint_DefectiveDir();
                                        }

                                        // 次の方向
                                        m_NextMoveDir = Vector3.down;
                                    }
                                }
                            }
                            // ←からか:の場合次は右下から
                            else if (!m_IsEightFly_StR)
                            {
                                if (m_MissionSubCnt == 0)
                                {
                                    // 右上
                                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Eight_Right3))
                                    {
                                        // 
                                        m_MissionSubCnt = 1;
                                        Debug.Log("Check : " + m_MissionSubCnt);

                                        // 機首が異なる
                                        if (!IsAngleRight())
                                        {
                                            // 減点
                                            SetUpdateMissionPoint_DefectiveDir();
                                        }

                                        // 次の方向
                                        m_NextMoveDir = Vector3.up;
                                    }
                                }
                                else if (m_MissionSubCnt == 1)
                                {
                                    // 右
                                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Eight_Right2))
                                    {
                                        // 
                                        m_MissionSubCnt = 2;
                                        Debug.Log("Check : " + m_MissionSubCnt);

                                        // 機首が異なる
                                        if (!IsAngleUp())
                                        {
                                            // 減点
                                            SetUpdateMissionPoint_DefectiveDir();
                                        }

                                        // 次の方向
                                        m_NextMoveDir = Vector3.left;
                                    }
                                }
                                else if (m_MissionSubCnt == 2)
                                {
                                    // 右下
                                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Eight_Right1))
                                    {
                                        // 
                                        m_MissionSubCnt = 3;
                                        Debug.Log("Check : " + m_MissionSubCnt);

                                        // 機首が異なる
                                        if (!IsAngleLeft())
                                        {
                                            // 減点
                                            SetUpdateMissionPoint_DefectiveDir();
                                        }

                                        // 次の方向
                                        m_NextMoveDir = Vector3.down;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
    // 異常事態における飛行
    public void CheckMissionTrigger_Happning(HitCheck_Mission hitcheckCtrl)
    {
        // outarea 地点A～H地点まで 中心はアウト
        if (1 < m_MissionCnt && m_MissionCnt < 5)
        {
            if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.OutArea))
            {
                if (!m_IsOutAreaIn)
                {
                    m_IsOutAreaIn = true;
                    // アウト地点に入った
                    MainGame.MG_Mediator.MainCanvas.SetOutArea(true);
                    // 減算
                    SetUpdateMissionPoint_Outarea();
                }
            }
            // 飛行空域離脱
            if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.EndArea))
            {
                // 飛行空域外になった
                // ミッション中止:危険な飛行,墜落,飛行空域離脱 中止
                FailedMission();
            }
        }

        switch (m_CurrentMissionId)
        {
            case MissionId.Mission_A:
                if (m_MissionCnt == 1)
                {
                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_A))
                    {
                        // 機首が異なる
                        if (!IsAngleUp())
                        {
                            // 減点
                            SetUpdateMissionPoint_DiffInstruction();
                        }

                        // 次の方向
                        m_NextMoveDir = Vector3.up;

                        UpdateMission(MissionId.Mission_B, 2);
                    }
                }
                break;

            case MissionId.Mission_B:
                if (m_MissionCnt == 2)
                {
                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_B))
                    {
                        // 機首が異なる
                        if (!IsAngleUp())
                        {
                            // 減点
                            SetUpdateMissionPoint_DiffInstruction();
                        }

                        // 次の方向
                        m_NextMoveDir = Vector3.up;

                        UpdateMission(MissionId.Mission_E, 3);
                    }
                    else if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Check_AB))
                    {
                        if (!m_IsMissionDirCheck)
                        {
                            m_IsMissionDirCheck = true;
                            // 機首が異なる
                            if (!IsAngleUp())
                            {
                                // 減点
                                SetUpdateMissionPoint_DefectiveDir();
                            }
                        }
                    }
                }
                break;
            case MissionId.Mission_E:
                if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_E))
                {
                    // 機首が異なる
                    if (!IsAngleUp())
                    {
                        // 減点
                        SetUpdateMissionPoint_DiffInstruction();
                    }

                    // 次の方向
                    m_NextMoveDir = Vector3.up;

                    UpdateMission(MissionId.Mission_HappningEtoB, 4);
                }
                else if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_A))
                {
                    if (!m_IsMissionDirCheck)
                    {
                        m_IsMissionDirCheck = true;
                        // 機首が異なる
                        if (!IsAngleUp())
                        {
                            // 減点
                            SetUpdateMissionPoint_DefectiveDir();
                        }
                    }
                }
                break;

            case MissionId.Mission_HappningEtoB:
                if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Check_AB))
                {
                    // 機首が異なる
                    if (!IsAngleUp())
                    {
                        // 減点
                        SetUpdateMissionPoint_DiffInstruction();
                    }

                    // 次の方向
                    m_NextMoveDir = Vector3.up;

                    UpdateMission(MissionId.Mission_H, 5);
                }
                else if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_A))
                {
                    if (!m_IsMissionDirCheck)
                    {
                        m_IsMissionDirCheck = true;
                        // 機首が異なる
                        if (!IsAngleUp())
                        {
                            // 減点
                            SetUpdateMissionPoint_DefectiveDir();
                        }
                    }
                }
                break;
            case MissionId.Mission_H:
                if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.Area_H))
                {
                    // 機首が異なる
                    if (!IsAngleUp())
                    {
                        // 減点
                        SetUpdateMissionPoint_DiffInstruction();
                    }

                    m_IsNextMoveDirChecking = false;

                    //UpdateMission(MissionId.Mission_LandingAndStop, 6);
                    CompleteMission();
                }
                break;
        }
    }
    public void ProcMissionExitOutArea()
    {
        m_IsOutAreaIn = false;
    }

    // ミッション更新
    private void UpdateMission(MissionId id, int cnt)
    {
        // ミッション更新
        m_CurrentMissionId = id;
        m_MissionCnt = cnt;
        MainGame.MG_Mediator.MainCanvas.GetMissionInfo().SetCurrentMissionText(m_MissionCnt);

        // 更新時SE
        MainGame.MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);

        m_IsMissionDirCheck = false;
        m_IsNextMoveDirError = false;
    }
    private void CompleteMission()
    {
        if (m_IsMissionEnd) return;

        m_IsMissionEnd = true;

        // ミッション完了
        MainGame.MG_Mediator.MainCanvas.GetMissionInfo().SetCompleteMissionText();

        // リプレイログ用:エンド
        AddReplayLog(InputActionKey.Replay_End);

        // タイマーを停止
        MainGame.MG_Mediator.MainCanvas.GetTimerCtrl().ProcGameEnd();

        // メトロノーム停止
        m_MetronomeCtrl.SetStop();

        // リザルト呼び出し
        MainGame.MG_Mediator.MainCanvas.SetResult(m_MissionPoint);
    }
    private void FailedMission()
    {
        if (m_IsMissionEnd) return;

        m_IsMissionEnd = true;

        // ミッション完了
        MainGame.MG_Mediator.MainCanvas.GetMissionInfo().SetStopMissionText();

        // リプレイログ用:エンド
        AddReplayLog(InputActionKey.Replay_End);

        // タイマーを停止
        MainGame.MG_Mediator.MainCanvas.GetTimerCtrl().ProcGameEnd();

        // メトロノーム停止
        m_MetronomeCtrl.SetStop();

        // リザルト呼び出し
        MainGame.MG_Mediator.MainCanvas.SetResult(m_MissionPoint, true);
    }


    //=========================================================================
    // [///] バウンド設定
    //=========================================================================
    // →base格納

    #region  ActionMethod
    //=========================================================================
    //
    // [///] 機能切り替えメソッド
    //
    //=========================================================================
    // ビジョンセンサー
    public void ChangeVisionSensor()
    {
        if (AppData.m_PlayMode == AppData.PlayMode.Replay) return;
        if (!MainGame.MG_Mediator.StateCtrl.IsMainUpdate()) return;
        if (m_IsChangeCheck) return;
        m_IsChangeCheck = true;

        m_IsVisionSensor = !m_IsVisionSensor;
        AddReplayLog(m_IsVisionSensor ? InputActionKey.Proc_VisionSensor_On : InputActionKey.Proc_VisionSensor_Off);
        // UI
        ShInputManager.I.GetInputCanvas().SetBtnActive_VisionSensor(m_IsVisionSensor);
        // SE
        ProcActionBtnSE();
    }
    // フリップ
    public void ChangeFrip()
    {
        if (AppData.m_PlayMode == AppData.PlayMode.Replay) return;
        if (!MainGame.MG_Mediator.StateCtrl.IsMainUpdate()) return;
        // フリップ中も変えられる
        // if (m_IsFripAction) return;
        if (m_IsChangeCheck) return;
        m_IsChangeCheck = true;

        m_IsFripWait = !m_IsFripWait;
        // UI
        ShInputManager.I.GetInputCanvas().SetBtnActive_Frip(m_IsFripWait);
        // SE
        ProcActionBtnSE();
    }
    // メトロノーム
    public void ChangeMetronome()
    {
        if (AppData.m_PlayMode == AppData.PlayMode.Replay) return;
        if (!MainGame.MG_Mediator.StateCtrl.IsMainUpdate()) return;
        if (m_IsMissionEnd) return;
        if (m_IsChangeCheck) return;
        m_IsChangeCheck = true;

        m_IsMetronome = !m_IsMetronome;
        AddReplayLog(m_IsMetronome ? InputActionKey.Proc_Metronome_On : InputActionKey.Proc_Metronome_Off);
        // UI
        ShInputManager.I.GetInputCanvas().SetBtnActive_Metronome(m_IsMetronome);
        // SE
        ProcActionBtnSE();

        // メトロノーム設定
        {
            m_MetronomeCtrl.SetMetronome(m_IsMetronome);
        }
    }
    // グリッド
    public void ChangeGrid()
    {
        if (AppData.m_PlayMode == AppData.PlayMode.Replay) return;
        if (!MainGame.MG_Mediator.StateCtrl.IsMainUpdate()) return;
        if (m_IsChangeCheck) return;
        m_IsChangeCheck = true;

        m_IsGrid = !m_IsGrid;
        // UI
        ShInputManager.I.GetInputCanvas().SetBtnActive_Grid(m_IsGrid);
        ShInputManager.I.GetWipeCanvas().SetGrid(m_IsGrid);
        MainGame.MG_Mediator.MainCanvas.SetGrid(m_IsGrid);
        // SE
        ProcActionBtnSE();

        // セーブデータ反映
        AppData.SetGridDisplay(m_IsGrid);
        AppCommon.Update_And_SaveGameData();
    }
    // 速度
    public void ChangeSpd()
    {
        if (AppData.m_PlayMode == AppData.PlayMode.Replay) return;
        if (!MainGame.MG_Mediator.StateCtrl.IsMainUpdate()) return;
        if (m_IsChangeCheck) return;
        m_IsChangeCheck = true;

        m_CurrentSpdChangeId++;
        if (m_CurrentSpdChangeId >= 3) m_CurrentSpdChangeId = 0;
        switch (m_CurrentSpdChangeId)
        {
            case 0:
                m_SpdChangeMag = 1.0f;
                AddReplayLog(InputActionKey.Proc_Spd_Change1);
                break;
            case 1:
                m_SpdChangeMag = 1.25f;
                AddReplayLog(InputActionKey.Proc_Spd_Change2);
                break;
            case 2:
                m_SpdChangeMag = 1.5f;
                AddReplayLog(InputActionKey.Proc_Spd_Change3);
                break;
        }
        // SE
        ProcActionBtnSE();
    }
    // ヘッドレス
    public void ChangeHeadless()
    {
        if (AppData.m_PlayMode == AppData.PlayMode.Replay) return;
        if (!MainGame.MG_Mediator.StateCtrl.IsMainUpdate()) return;
        if (m_IsChangeCheck) return;
        m_IsChangeCheck = true;

        m_IsHeadless = !m_IsHeadless;
        AddReplayLog(m_IsHeadless ? InputActionKey.Proc_Headless_On : InputActionKey.Proc_Headless_Off);
        // UI
        ShInputManager.I.GetInputCanvas().SetBtnActive_Headless(m_IsHeadless);
        // SE
        ProcActionBtnSE();
    }
    // 自動離着陸
    public void ChangeAutoFly()
    {
        if (AppData.m_PlayMode == AppData.PlayMode.Replay) return;
        if (!MainGame.MG_Mediator.StateCtrl.IsMainUpdate()) return;
        if (m_IsChangeCheck) return;
        m_IsChangeCheck = true;

        // 飛行中の場合:着陸
        if (m_CurrentDroneActionId == DroneActionId.Fly)
        {
            if (m_IsAutoFlyAction) return;
            if (m_IsAutoLandingAction) return;
            m_IsAutoLandingAction = true;
            AddReplayLog(InputActionKey.Proc_AutoLanding_On);
            // UI
            ShInputManager.I.GetInputCanvas().SetBtnInteractable_AutoFly(false);
            // SE
            ProcActionBtnSE();
        }
        // 他：離陸
        else
        {
            if (m_IsAutoFlyAction) return;
            m_IsAutoFlyAction = true;
            // UI
            ShInputManager.I.GetInputCanvas().SetBtnInteractable_AutoFly(false);
            switch (m_CurrentDroneActionId)
            {
                case DroneActionId.Wait:    // 待機中
                    // プロペラ設定
                    m_PropellerCtrl.ProcMonterOn();
                    m_IsNeutral = false;
                    m_IsAutoFly_MonterOnWait = true;
                    AddReplayLog(InputActionKey.Proc_AutoFly_On_MonterOnWait);
                    StartCoroutine(ProcAutoFlyPropellerWait());
                    break;

                case DroneActionId.MonterOn:
                    AddReplayLog(InputActionKey.Proc_AutoFly_On);
                    ProcDrone_ChangeDroneAction_Fly();
                    break;
            }
            // SE
            ProcActionBtnSE();
        }

    }
    // 機能ボタン実行時のSE
    private void ProcActionBtnSE()
    {
        // SE
        MainGame.MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }
    private IEnumerator ProcAutoFlyPropellerWait()
    {
        yield return new WaitForSeconds(0.5f);

        // rog
        AddReplayLog(InputActionKey.Proc_AutoFly_On_MonterOnWait_Off);

        m_IsAutoFly_MonterOnWait = false;
        ProcDrone_ChangeDroneAction_Fly();
    }
    #endregion
}

