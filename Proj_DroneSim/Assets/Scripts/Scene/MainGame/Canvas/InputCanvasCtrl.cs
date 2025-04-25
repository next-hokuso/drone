using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shs;
using UnityEngine.UI;
using MainGame;
using Game;
using UnityEngine.Android;

/// <summary>
/// </summary>
public class InputCanvasCtrl : ShCanvasBaseCtrl, IShVisible
{
    //================================================
    // [///]  各シーンごとに保持する変数
    //================================================
    // IShVisibleで使用する表示変数
    public bool IsVisible { get; private set; } = false;

    // 表示制御としてリスト追加するものの設定
    private enum UINo
    {
        Canvas,

        TapPad_L,
        TapPad_R,

        // Btn_Change,

        // 機能ボタン
        Btn_Metronome,
        Btn_Grid,
        Btn_Frip,
        Btn_Spd,
        Btn_Headless,
        Btn_VisionSensor,
        Btn_AutoFly,
    }
    protected ShUIList m_UIList = new ShUIList();

    // 自身
    protected Canvas m_ThisCanvas = null;

    // TODO:他に移動
    public bool IsSelectCheck { get; private set; } = false;
    public void SetSelectCheck()
    {
        IsSelectCheck = true;
        StartCoroutine(SelectCheck());
    }
    private IEnumerator SelectCheck()
    {
        yield return new WaitForSeconds(0.4f);

        IsSelectCheck = false;
    }
    // DebugCameraからの戻り識別
    public bool IsDebugCameraReturn { get; set; } = false;
    // Tutorialの表示番号 ※外部からのアクセス専用
    public int TutorialNum { get; set; } = 0;

    // pad
    InputVirtualPadUICtrl m_PadLCtrl = null;
    InputVirtualPadUICtrl m_PadRCtrl = null;
    public IsCheckDrawArea IsTapArea_L1 { private set; get; } = null;
    public IsCheckDrawArea IsTapArea_R1 { private set; get; } = null;

    //================================================
    // [///] シーンの初期化
    //================================================
    public void Awake()
    {
    }

    public override void ProcInitialize()
    {
        // 自身
        m_ThisCanvas = Shs.ShSceneUtils.GetSceneRootObj(AppCommon.CurrentSceneName, "AddCanvas").
            transform.Find("InputCanvas").GetComponent<Canvas>();


        // 自身から子供の表示制御の付与取得
        {
            Transform canvasT = m_ThisCanvas.transform;

            // リストへの登録
            m_UIList.Clear();
            {
                m_UIList.Add(canvasT.gameObject.AddComponent<ShUIBaseCtrl>());
                m_UIList.Add(canvasT.Find("Input/Grp_TapPad_L").gameObject.AddComponent<InputVirtualPadUICtrl>());
                m_UIList.Add(canvasT.Find("Input/Grp_TapPad_R").gameObject.AddComponent<InputVirtualPadUICtrl>());

                // m_UIList.Add(canvasT.Find("Input/Btn_Key").gameObject.AddComponent<ShUITextCtrl>());

                // 機能ボタン
                m_UIList.Add(canvasT.Find("Input/Grp_Btn/Btn_Metronome").gameObject.AddComponent<UIActionBtnCtrl>());
                m_UIList.Add(canvasT.Find("Input/Grp_Btn/Btn_Grid").gameObject.AddComponent<UIActionBtnCtrl>());
                m_UIList.Add(canvasT.Find("Input/Grp_Btn/Btn_Frip").gameObject.AddComponent<UIActionBtnCtrl>());
                m_UIList.Add(canvasT.Find("Input/Grp_Btn/Btn_Spd").gameObject.AddComponent<UIActionBtnCtrl>());
                m_UIList.Add(canvasT.Find("Input/Grp_Btn/Btn_Headless").gameObject.AddComponent<UIActionBtnCtrl>());
                m_UIList.Add(canvasT.Find("Input/Grp_Btn/Btn_Vision").gameObject.AddComponent<UIActionBtnCtrl>());
                m_UIList.Add(canvasT.Find("Input/Grp_Btn/Btn_AutoFly").gameObject.AddComponent<UIActionBtnCtrl>());
            }

            // 初期化
            m_UIList.ProcInitialize();

            // 個別設定(ボタンなど) @TODO 処理を移動/改善する
            {
                // LRスティック別設定
                m_UIList.m_List[(int)UINo.TapPad_L].GetComponent<InputVirtualPadUICtrl>().ProcInitialize_LRSetting(true);
                m_UIList.m_List[(int)UINo.TapPad_R].GetComponent<InputVirtualPadUICtrl>().ProcInitialize_LRSetting(false);

                // テキスト設定
                canvasT.Find("Input/Txt_Mode").GetComponent<TMPro.TMP_Text>().text =
                    (AppData.GetCurrentConnectMode() == AppData.ConnectM.Mode1 ? "モード1" : "モード2");

                Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(
                    canvasT.Find("Btn_Key").GetComponent<Button>(), OnClick_ChangeKey);

                // 機能ボタン
                // Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(canvasT.Find("Input/Grp_Btn/Btn_Metronome").
                //     GetComponent<Button>(), OnClick_Metronome);
                // Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(canvasT.Find("Input/Grp_Btn/Btn_Grid").
                //     GetComponent<Button>(), OnClick_Grid);
                // Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(canvasT.Find("Input/Grp_Btn/Btn_Frip").
                //     GetComponent<Button>(), OnClick_Frip);
                // Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(canvasT.Find("Input/Grp_Btn/Btn_Spd").
                //     GetComponent<Button>(), OnClick_Spd);
                // Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(canvasT.Find("Input/Grp_Btn/Btn_Headless").
                //     GetComponent<Button>(), OnClick_Headless);
                // Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(canvasT.Find("Input/Grp_Btn/Btn_Vision").
                //     GetComponent<Button>(), OnClick_Vision);
                // Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(canvasT.Find("Input/Grp_Btn/Btn_AutoFly").
                //     GetComponent<Button>(), OnClick_AutoFly);
                canvasT.Find("Input/Grp_Btn/Btn_Metronome").GetComponent<Button>().onClick.AddListener(OnClick_Metronome);
                canvasT.Find("Input/Grp_Btn/Btn_Grid").GetComponent<Button>().onClick.AddListener(OnClick_Grid);
                canvasT.Find("Input/Grp_Btn/Btn_Frip").GetComponent<Button>().onClick.AddListener(OnClick_Frip);
                canvasT.Find("Input/Grp_Btn/Btn_Spd").GetComponent<Button>().onClick.AddListener(OnClick_Spd);
                canvasT.Find("Input/Grp_Btn/Btn_Headless").GetComponent<Button>().onClick.AddListener(OnClick_Headless);
                canvasT.Find("Input/Grp_Btn/Btn_Vision").GetComponent<Button>().onClick.AddListener(OnClick_Vision);
                canvasT.Find("Input/Grp_Btn/Btn_AutoFly").GetComponent<Button>().onClick.AddListener(OnClick_AutoFly);

                // 初期設定
                ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_Metronome]).SetBtnColor(AppData.MetronomeFlightSound);
                ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_Grid]).SetBtnColor(AppData.GridDisplay);
                ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_Frip]).SetBtnColor(false);
                ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_Spd]).SetBtnColor(true);
                ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_Headless]).SetBtnColor(false);
                ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_VisionSensor]).SetBtnColor(true);
                ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_AutoFly]).SetBtnColor(true);

                // Treasure
                if(AppData.m_PlayMode == AppData.PlayMode.Game_Treasure ||
                   AppData.m_PlayMode == AppData.PlayMode.Game_Irairabou)
                {
                    // canvasT.Find("Input/Btn_L").GetComponent<Button>().onClick.AddListener(OnClick_L1);
                    // canvasT.Find("Input/Btn_R").GetComponent<Button>().onClick.AddListener(OnClick_R1);
                    canvasT.Find("Input/Btn_Change").GetComponent<Button>().onClick.AddListener(OnClick_ChangeCamera);

                    IsTapArea_L1 = canvasT.Find("Input/Btn_L").gameObject.GetComponent<IsCheckDrawArea>();
                    IsTapArea_R1 = canvasT.Find("Input/Btn_R").gameObject.GetComponent<IsCheckDrawArea>();
                }

                m_PadLCtrl = m_UIList.m_List[(int)UINo.TapPad_L].GetComponent<InputVirtualPadUICtrl>();
                m_PadRCtrl = m_UIList.m_List[(int)UINo.TapPad_R].GetComponent<InputVirtualPadUICtrl>();


                // モバイル設定
                if (Yns.YnSys.m_IsMobilePlatform)
                {
                    // タップパッドを上げる
                    canvasT.Find("Input").transform.localPosition += Vector3.up * 50.0f;
                }
            }

            // 非表示
            m_UIList.Hide();
        }

        // Resorce Load
        //StartCoroutine("ProcResourcesLoad", true);
    }

    //================================================
    // [///] シーンに必要なリソース読み込み
    //================================================
    public override IEnumerator ProcResourcesLoad()
    {
        // 読み込み完了
        m_IsCompleteSceneSetup = true;

        yield break;
    }

    //================================================
    // [///] 表示切り替え
    //================================================
    void IShVisible.Show()
    {
        m_UIList.Show();

        // パッド表示設定
        if (!AppData.PadDisplay)
        {
            m_UIList.m_List[(int)UINo.TapPad_R].SetVisible(false);
            m_UIList.m_List[(int)UINo.TapPad_L].SetVisible(false);
            m_ThisCanvas.transform.Find("Input/Txt_Mode").gameObject.SetActive(false);
        }
    }
    void IShVisible.Hide()
    {
        m_UIList.Hide();
    }

    //================================================
    // [///] シーンの処理開始
    //================================================
    public override void ProcSceneProcStart()
    {
    }
    // リセット
    public override void ProcSceneReset()
    {
    }
    // クリア時
    public void ProcGameClear()
    {
    }
    // リザルト待機
    public void ProcResultWait()
    {
    }

    //================================================
    // [///] 開始演出
    //================================================

    //================================================
    // [///] TODO:実装部分のため他に移したい
    //================================================
    public InputVirtualPadUICtrl GetPadLCtrl()
    {
        return m_PadLCtrl;
        //return m_UIList.m_List[(int)UINo.TapPad_L].GetComponent<InputVirtualPadUICtrl>();
    }
    public InputVirtualPadUICtrl GetPadRCtrl()
    {
        return m_PadRCtrl;
        // return m_UIList.m_List[(int)UINo.TapPad_R].GetComponent<InputVirtualPadUICtrl>();
    }

    public void OnClick_ChangeKey()
    {
        if(ShInputManager.I.GetInputSetting() == ShInputManager.InputSetting.Controller)
        {
            ShInputManager.I.SetInputSetting(ShInputManager.InputSetting.Keyboard);
        }
        else
        {
            ShInputManager.I.SetInputSetting(ShInputManager.InputSetting.Controller);
        }
        SetInputSettingUI();
    }
    public void SetInputSettingUI()
    {
        if (ShInputManager.I.GetInputSetting() == ShInputManager.InputSetting.Controller)
        {
            m_ThisCanvas.transform.Find("Btn_Key").GetComponentInChildren<TMPro.TMP_Text>().text = "Controller";
        }
        else
        {
            m_ThisCanvas.transform.Find("Btn_Key").GetComponentInChildren<TMPro.TMP_Text>().text = "Keyboard";
        }
    }

    //================================================
    // [///] TODO:実装部分のため他に移したい2
    //================================================
    // 機能ボタン関連 (Treasure)
    public void OnClick_L1()
    {
        MGTreasure_Mediator.ObjMediator.DroneCtrl.ProcInput_UIButton_L1();
    }
    public void OnClick_R1()
    {
        MGTreasure_Mediator.ObjMediator.DroneCtrl.ProcInput_UIButton_R1();
    }
    public void OnClick_ChangeCamera()
    {
        MGTreasure_Mediator.ObjMediator.DroneCtrl.ProcInput_UIButton_CameraChange();
    }

    //================================================
    // [///] TODO:実装部分のため他に移したい2
    //================================================
    // 機能ボタン関連
    // メトロノーム
    public void OnClick_Metronome()
    {
        MG_Mediator.ObjMediator.DroneCtrl.ChangeMetronome();
    }
    public void SetBtnActive_Metronome(bool isActive)
    {
        ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_Metronome]).SetBtnColor(isActive);
    }
    // グリッド
    public void OnClick_Grid()
    {
        MG_Mediator.ObjMediator.DroneCtrl.ChangeGrid();
    }
    public void SetBtnActive_Grid(bool isActive)
    {
        ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_Grid]).SetBtnColor(isActive);
    }
    // フリップ
    public void OnClick_Frip()
    {
        MG_Mediator.ObjMediator.DroneCtrl.ChangeFrip();
    }
    public void SetBtnActive_Frip(bool isActive)
    {
        ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_Frip]).SetBtnColor(isActive);
    }
    public void SetBtnInteractable_Frip(bool isActive)
    {
        ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_Frip]).SetBtnInteractable(isActive);
    }
    // 速度変更
    public void OnClick_Spd()
    {
        MG_Mediator.ObjMediator.DroneCtrl.ChangeSpd();
    }
    public void SetBtnActive_Spd(bool isActive)
    {
        ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_Spd]).SetBtnColor(isActive);
    }
    // ヘッドレス
    public void OnClick_Headless()
    {
        MG_Mediator.ObjMediator.DroneCtrl.ChangeHeadless();
    }
    public void SetBtnActive_Headless(bool isActive)
    {
        ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_Headless]).SetBtnColor(isActive);
    }
    // ビジョンセンサー
    public void OnClick_Vision()
    {
        MG_Mediator.ObjMediator.DroneCtrl.ChangeVisionSensor();
    }
    public void SetBtnActive_VisionSensor(bool isActive)
    {
        ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_VisionSensor]).SetBtnColor(isActive);
    }
    // 自動離着陸
    public void OnClick_AutoFly()
    {
        MG_Mediator.ObjMediator.DroneCtrl.ChangeAutoFly();
    }
    public void SetBtnActive_AutoFly(bool isActive)
    {
        ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_AutoFly]).SetBtnColor(isActive);
    }
    public void SetBtnInteractable_AutoFly(bool isActive)
    {
        ((UIActionBtnCtrl)m_UIList.m_List[(int)UINo.Btn_AutoFly]).SetBtnInteractable(isActive);
    }
}
