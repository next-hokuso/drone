//#define DEBUG_DISP_STATUS_TEXT
//#define ERROR_TEST_CREATE_USER_STORAGE
//#define ERROR_TEST_CREATE_UPDATE_PROFILE
//#define ERROR_TEST_CREATE_SEND_EMAIL
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yns;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
using AOT;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using System.Globalization;
using MainGame;
using Game;
using System.Linq;

public class Login : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    const string MailConfirmMessage = "確認用のメールを送りました。\n※確認が完了すると先に進みます";
    const string MailConfirmCancelMessage = "ユーザーによりキャンセルされました。\nもう一度最初からやり直してください。";
    const string DeleteConfirmMessage = "ユーザー情報を削除しますか？\n<color=red>※この操作は取り消せません</color>";
    const string LogoutConfirmMessage = "ログアウトしますか？";
    const string ChangeMailConfirmCancelMessage = "ユーザーによりキャンセルされました。";
    const string ReloadExpireTokenMessage = "ユーザー認証情報の期限が切れました。\nログアウトします。\n";
    const string ReplayUploadingMessage = "リプレイデータをアップロードしています。";
    const string KeySetMessage = "該当するキーを押してください。";
    const string KeyInitializeConfirmMessage = "キーボードの設定を初期状態に戻しますか？";
    const string PadSetMessage = "該当するボタンを押してください。";
    const string PadInitializeConfirmMessage = "ゲームパッドの設定を初期状態に戻しますか？";

    const string UserNameHeader = "<sprite name=\"circle\">";

    readonly string[] ReplayModes = { "フリーモード", "ミッションモード" };

    const float CreateEmailVerifiedWait = 5.0f;
    const float ChangeEmailVerifiedWait = 5.0f;
    const float ReloadUserInfoWait = 5.0f;

    const int NameCharacterLimit = 16;

    readonly string[] OptionHtmlColor = { "#EE7800", "#323232" };   // 選択・非選択

    const int OptionMetronomeBPMIndex = 7;
    const int OptionMetronomeBPMAdd = 10;
    const int OptionMetronomeBPMLow = 30;
    const int OptionMetronomeBPMHigh = 120;

    // 処理フェーズ値
    private enum MenuPhase
    {
        None,
        Start,
        Start_User_Start,
        Start_User_Update,
        Start_UserInfo_Start,
        Start_UserInfo_Update,
        Start_SendReplay_Update,
        Start_ReplayInfo_Start,
        Start_ReplayInfo_Update,
        Start_Start,
        Start_Update,
        Create_Start,
        Create_Update,
        Create_UserInfo_Start,
        Create_UserInfo_Update,
        Create_Profile_Start,
        Create_Profile_Update,
        Create_SendEmail_Start,
        Create_SendEmail_Update,
        Create_EmailVerified_Start,
        Create_EmailVerified_Update,
        Login_Start,
        Login_Update,
        Login_UserInfo_Start,
        Login_UserInfo_Update,
        Login_ReplayInfo_Start,
        Login_ReplayInfo_Update,
        Loggedin_Start,
        Loggedin_Update,
        Play_Start,
        Play_Update,
        Option_Start,
        Option_Update,
        Account_Start,
        Account_Update,
        Account_Proc,
        Account_EmailVerified_Start,
        Account_EmailVerified_Update,
        Logout_Update,
        Dialog_Update,
        Dialog_Keyboard,
        Dialog_Gamepad,
        Dialog_Delay,
        Replay_Send_Start,
        Replay_Send_Update,
        Replay_Get_Start,
        Replay_Get_Update,
        Error_DeleteUserInfo_Start,
        Error_DeleteUserInfo_Update,
        Error_DeleteUser_Start,
        Error_DeleteUser_Update,

        Game_Start,
        Game_Update,
        Game_TransGame1,
        Game_TransGame2,

        Update,
        End,
    }
    // 処理フェーズ
    private MenuPhase m_MenuPhase = MenuPhase.None;
    private MenuPhase m_ReturnMenuPhase = MenuPhase.None;
    private MenuPhase m_ErrorMenuPhase = MenuPhase.None;
    private MenuPhase m_ConfirmYMenuPhase = MenuPhase.None;
    private MenuPhase m_ConfirmNMenuPhase = MenuPhase.None;

    private enum ReloadPhase
    {
        None,
        Start,
        Confirm,
        End,
    }
    private ReloadPhase m_ReloadPhase = ReloadPhase.None;

    private enum ReplayPhase
    {
        Wait,
        Get_Start,
        Get_Update,
        Send_Start,
        Send_Update,
    }
    private ReplayPhase m_ReplayPhase = ReplayPhase.Wait;

    private enum DialogType
    {
        Type1 = 0,      // ログイン用
        Type2,          // ユーザー作成用
        Type3,          // ユーザー作成時のメール確認用
        Type4,          // ユーザー削除時の確認用
        Type5,          // ログアウト時の確認用
        Type6,          // キーボード設定初期化時の確認用
        Type7,          // ゲームパッド設定初期化時の確認用
    }

    private enum InputType
    {
        Type1,          // 縦選択
        Type2,          // 横選択
    }

    private enum ContentType
    {
        Type1,          // 職業選択
        Type2,          // キーボード設定
        Type3,          // ゲームパッド設定
        Type4,          // 年代選択（モバイル）
        Type5,          // 職業選択（モバイル）
        Type6,          // アカウント選択（モバイル）
        Type7,          // オプション設定（モバイル）
        Type8,          // キーボード設定（モバイル）
        Type9,          // ゲームパッド設定（モバイル）
        Type10,         // リプレイ選択
        Type11,         // リプレイ選択（モバイル）
    }

    private float m_Timer = 0.0f;
    private float m_ReloadTimer = 0.0f;

    private TextMeshProUGUI m_UserNameText = null;
    private TextMeshProUGUI m_StatusText = null;
    private TextMeshProUGUI m_StatusText2 = null;

    private Transform m_trGrpStart = null;
    private Transform m_trGrpCreate = null;
    private Transform m_trGrpLogin = null;
    private Transform m_trGrpLoggedin = null;
    private Transform m_trGrpPlay = null;
    private Transform m_trGrpOption = null;
    private Transform m_trGrpAccount = null;
    private Transform m_trGrpDialog = null;
    private Transform m_trPlayReplaySelect = null;
    private Transform m_trPlayMissionSelect = null;
    private Transform m_trGrpGame = null;

    private Button m_Btn_Start_Login = null;
    private Button m_Btn_Start_Create = null;

    private string m_CreateEmailStr = "";
    private string m_CreateNameStr = "";
    private string m_CreateGenderStr = "";
    private string m_CreateAgeStr = "";
    private string m_CreateWorkStr = "";
    private string m_CreatePasswordStr = "";
    private TMP_InputField m_InputField_CreateEmail = null;
    //private TMP_InputField m_InputField_CreateName = null;
    private TMP_InputField m_InputField_CreatePassword = null;
    private CustomInputButton m_InputButton_CreateEmail = null;
    private CustomInputButton m_InputButton_CreateName = null;
    private CustomInputButton m_InputButton_CreatePassword = null;
    private Toggle m_Toggle_CreatePassword = null;
    private Button m_Btn_Create_Gender = null;
    private Button m_Btn_Create_Age = null;
    private Button m_Btn_Create_Work = null;
    private Button m_Btn_Create_Check = null;
    private Button m_Btn_Create_Back = null;
    private GameObject m_CreateTouchArea = null;

    private TextMeshProUGUI m_CreateGenderText = null;
    private List<Button> m_CreateGenderBtns = null;
    private List<TextMeshProUGUI> m_CreateGenderTexts = null;
    private Transform m_trCreateGender = null;
    private TextMeshProUGUI m_CreateAgeText = null;
    private List<Button> m_CreateAgeBtns = null;
    private List<TextMeshProUGUI> m_CreateAgeTexts = null;
    private Transform m_trCreateAge = null;
    private TextMeshProUGUI m_CreateWorkText = null;
    private List<Button> m_CreateWorkBtns = null;
    private List<TextMeshProUGUI> m_CreateWorkTexts = null;
    private Transform m_trCreateWork = null;

    private string m_DeleteUserPasswordStr = "";

    private string m_LoginEmailStr = "";
    private string m_LoginPasswordStr = "";
    private TMP_InputField m_InputField_LoginEmail = null;
    private TMP_InputField m_InputField_LoginPassword = null;
    private CustomInputButton m_InputButton_LoginEmail = null;
    private CustomInputButton m_InputButton_LoginPassword = null;
    private Toggle m_Toggle_LoginPassword = null;
    private Button m_Btn_Login_Check = null;
    private Button m_Btn_Login_Back = null;
    private GameObject m_LoginTouchArea = null;

    private Button m_Btn_Loggedin_Play = null;
    private Button m_Btn_Loggedin_Option = null;
    private Button m_Btn_Loggedin_Account = null;
    private Button m_Btn_Loggedin_Gamemode = null;

    private Button m_Btn_Play_Free = null;
    private Button m_Btn_Play_Mission = null;
    private Button m_Btn_Play_Replay = null;
    private Button m_Btn_Play_Back = null;
    private Button m_Btn_ReplaySelect_Back = null;
    private Coroutine m_crGetReplayDataWait = null;
    private List<Button> m_ReplayBtns = null;
    private Button m_Btn_MissionSelect_Back = null;
    private Button m_Btn_MissionSelect_Square = null;
    private Button m_Btn_MissionSelect_Eight = null;
    private Button m_Btn_MissionSelect_Happning = null;
    private List<Button> m_MissionBtns = null;

    private Button m_Btn_Gamemode_Treasure = null;
    private Button m_Btn_Gamemode_Iraira = null;
    private Button m_Btn_Gamemode_Endless = null;
    private Button m_Btn_Gamemode_Back = null;

    private List<GameObject> m_goOptionBtns = null;
    private Transform m_trOptionKeyboard = null;
    private List<Button> m_OptionKeyboardBtns = null;
    private Transform m_trOptionGamepad = null;
    private List<Button> m_OptionGamepadBtns = null;
    private Slider m_OptionBPMSlider = null;
    private TextMeshProUGUI m_OptionBPMText = null;

    private string m_AccountNameStr = "";
    private string m_AccountEmailStr = "";
    private string m_AccountPasswordStr = "";
    private string m_AccountNowPasswordStr = "";
    private TMP_InputField m_InputField_AccountEmail = null;
    //private TMP_InputField m_InputField_AccountName = null;
    private TMP_InputField m_InputField_AccountGender = null;
    private TMP_InputField m_InputField_AccountAge = null;
    private TMP_InputField m_InputField_AccountWork = null;
    private TMP_InputField m_InputField_AccountPassword = null;
    private TMP_InputField m_InputField_AccountNowPassword = null;
    private CustomInputButton m_InputButton_AccountEmail = null;
    private CustomInputButton m_InputButton_AccountName = null;
    private CustomInputButton m_InputButton_AccountPassword = null;
    private CustomInputButton m_InputButton_AccountNowPassword = null;
    private Toggle m_Toggle_AccountPassword = null;
    private Toggle m_Toggle_AccountNowPassword = null;
    private Button m_Btn_Account_Change = null;
    private Button m_Btn_Account_Back = null;
    private Button m_Btn_Account_Logout = null;
    private Button m_Btn_Account_Delete = null;
    private GameObject m_AccountTouchArea = null;
    private bool m_isChangedEmail = false;

    private DialogType m_DialogType = DialogType.Type1;
    private Button m_Btn_Dialog_Cancel = null;
    private Button m_Btn_Dialog_ConfirmY = null;
    private Button m_Btn_Dialog_ConfirmN = null;
    private GameObject m_DialogConfirm = null;

    private AppData.PlayMode m_prePlayMode = AppData.PlayMode.None;
    private bool m_isMobilePlatform = false;
    private bool m_isChecking = false;
    private bool m_isSuccess = false;
    private bool m_isReceived = false;
    private bool m_isCancelFromUser = false;
    private bool m_isEndChangeEmailVerified = false;

    private bool m_isReloadSuccess = false;
    private bool m_isReloadReceived = false;
    private bool m_isReloadExpired = false;

    private bool m_isReplaySuccess = false;
    private bool m_isReplayReceived = false;
    private bool m_isGetReplayData = false;
    private bool m_isSendReplayData = false;
    private int m_ReplayIndex = -1;

    private bool m_isDisableInputAll = false;
    private bool m_isEnableInputField = false;
    private int m_SelectedKeyboardIndex = 0;

    // パッド、キーボード、マウスで選択するGameObject
    public enum Select
    {
        None = -1,
        Start,
        Create,
        CreateGender,
        CreateAge,
        CreateWork,
        Login,
        Loggedin,
        Play,
        PlayReplay,
        Option,
        OptionKeyboard,
        OptionGamepad,
        Account,
        Dialog,
        Gamemode,
    }
    // 外部アクセス可能なようにpublicにする
    public GameObject m_goStart_Select = null;
    public GameObject m_goCreate_Select = null;
    public GameObject m_goCreate_Gender = null;
    public GameObject m_goCreate_Age = null;
    public GameObject m_goCreate_Work = null;
    public GameObject m_goLogin_Select = null;
    public GameObject m_goLoggedin_Select = null;
    public GameObject m_goPlay_Select = null;
    public GameObject m_goPlay_Replay = null;
    public GameObject m_goPlay_Mission = null;
    public GameObject m_goGamemode_Select = null;
    public GameObject m_goOption_Select = null;
    public GameObject m_goOption_Keyboard = null;
    public GameObject m_goOption_Gamepad = null;
    public GameObject m_goAccount_Select = null;
    public GameObject m_goDialog_Select = null;

    private List<GameObject> m_goStartSelectList;
    private List<GameObject> m_goCreateSelectList;
    private List<GameObject> m_goLoginSelectList;
    private List<GameObject> m_goLoggedinSelectList;
    private List<GameObject> m_goPlaySelectList;
    private List<GameObject> m_goGamemodeSelectList;
    private List<GameObject> m_goAccountSelectList;

    private List<GameObject> m_goExcludeDesideSeList;

    [DllImport("__Internal")]   // jslib内の関数を呼び出すためのattribute
    private static extern void FB_InitializeAppJS();

    [DllImport("__Internal")]
    private static extern void FB_SignInJS(int instanceId, Action<int, string, bool> receiveCallback, string emailStr, string passwordStr);

    [DllImport("__Internal")]
    private static extern void FB_RegisterUserJS(int instanceId, Action<int, string, bool> receiveCallback, string emailStr, string passwordStr);

    [DllImport("__Internal")]
    private static extern void FB_CreateUserInfoStorageJS(int instanceId, Action<int, string, bool> receiveCallback, string uid, string genderStr, string ageStr, string workStr);

    [DllImport("__Internal")]
    private static extern void FB_GetUserInfoStorageJS(int instanceId, Action<int, string, bool> receiveCallback, string uid);

    [DllImport("__Internal")]
    private static extern void FB_DeleteUserInfoStorageJS(int instanceId, Action<int, string, bool> receiveCallback, string uid);

    [DllImport("__Internal")]
    private static extern void FB_UpdateProfileJS(int instanceId, Action<int, string, bool> receiveCallback, string displayNameStr, string photoURLStr = "");

    [DllImport("__Internal")]
    private static extern void FB_SendEmailVerificationJS(int instanceId, Action<int, string, bool> receiveCallback);

    [DllImport("__Internal")]
    private static extern void FB_DeleteUserJS(int instanceId, Action<int, string, bool> receiveCallback, string emailStr, string passwordStr);

    [DllImport("__Internal")]
    private static extern void FB_VerifyBeforeUpdateEmailJS(int instanceId, Action<int, string, bool> receiveCallback, string emailStr, string passwordStr, string newEmailStr);

    [DllImport("__Internal")]
    private static extern void FB_CreateEmailVerifiedJS(int instanceId, Action<int, string, bool> receiveCallback);

    [DllImport("__Internal")]
    private static extern void FB_GetCurrentUserJS(int instanceId, Action<int, string, bool> receiveCallback);

    [DllImport("__Internal")]
    private static extern void FB_GetCurrentUserReloadJS(int instanceId, Action<int, string, bool> receiveCallback);

    [DllImport("__Internal")]
    private static extern void FB_SignOutJS(int instanceId, Action<int, string, bool> receiveCallback);

    [DllImport("__Internal")]
    private static extern void FB_UpdatePasswordJS(int instanceId, Action<int, string, bool> receiveCallback, string emailStr, string passwordStr, string newPasswordStr);

    [DllImport("__Internal")]
    private static extern void FB_ChangeEmailVerifiedJS(int instanceId, Action<int, string, bool> receiveCallback, string emailStr, string passwordStr);

    [DllImport("__Internal")]
    private static extern void FB_ReauthenticateWithCredentialJS(int instanceId, Action<int, string, bool> receiveCallback, string emailStr, string passwordStr);

    [DllImport("__Internal")]
    private static extern void FB_CreateReplayInfoStorageJS(int instanceId, Action<int, string, bool> receiveCallback, string uid, string replayStr, int index);

    [DllImport("__Internal")]
    private static extern void FB_GetReplayInfoStorageJS(int instanceId, Action<int, string, bool> receiveCallback, string uid, int index);

    [DllImport("__Internal")]
    private static extern void FB_GetReplayInfoCountStorageJS(int instanceId, Action<int, string, bool> receiveCallback, string uid);

    [DllImport("__Internal")]
    private static extern void FB_OnAuthStateChangedJS(int instanceId, Action<int, string, bool> receiveCallback);

    // コールバックで実行元のインスタンスIDとインスタンスをマッピングするためのDictionary
    private static readonly Dictionary<int, Login> Instances = new();

    // firebase.authアカウント情報受信クラス
    // ※FirebasePlugin.jslib内で定義している receiveUserClass に合わせる
    class ReceiveUserClass
    {
        public string displayName;
        public string email;
        public string uid;
        public ReceiveUserClass(string _displayName, string _email, string _uid)
        {
            displayName = _displayName;
            email = _email;
            uid = _uid;
        }
    }
    private ReceiveUserClass m_ReceiveUserInfo = null;
    private ReceiveUserClass m_ReceiveUserInfoReload = null;

    // firebase.storageアカウント情報受信クラス
    // ※FirebasePlugin.jslib内で定義している receiveUserStorageClass に合わせる
    class ReceiveUserStorageClass
    {
        public string gender;
        public string age;
        public string work;
        public ReceiveUserStorageClass(string _gender, string _age, string _work)
        {
            gender = _gender;
            age = _age;
            work = _work;
        }
    }
    private ReceiveUserStorageClass m_ReceiveUserStorageInfo = null;

    private string m_ReplayStr = "";

    private System.Diagnostics.Stopwatch m_Stopwatch = new System.Diagnostics.Stopwatch();

    //================================================
    // [///]
    //================================================
    private void Awake()
    {
    }

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Login Start");
        m_MenuPhase = MenuPhase.Start;

        m_isMobilePlatform = YnSys.m_IsMobilePlatform;
        //m_isMobilePlatform = true;

        ////リプレイデータ作成のテスト
        //DummySetReceiveReplayCountStorageInfo();
        //
        //List<AppData.ReplayInfo> dummyReplayList = new List<AppData.ReplayInfo>();
        //dummyReplayList.Add(new AppData.ReplayInfo(0, "test", 0));
        //AppData.SetReplayData(AppData.PlayMode.Free, dummyReplayList, 0);

        GameObject goAppCanvas = YnSys.m_AppCommon.GetRootCanvas("Login", "AppCanvas", false);

        m_UserNameText = goAppCanvas.transform.Find("User/Text").GetComponent<TextMeshProUGUI>();
        m_StatusText = goAppCanvas.transform.Find("Status/Text_Val").GetComponent<TextMeshProUGUI>();
        m_StatusText2 = goAppCanvas.transform.Find("Text_Val").GetComponent<TextMeshProUGUI>();
#if DEBUG_DISP_STATUS_TEXT
        m_StatusText.transform.parent.gameObject.SetActive(true);
#else
        m_StatusText.transform.parent.gameObject.SetActive(false);
#endif

        m_trGrpStart = GetTransform(goAppCanvas, "Grp_Start");
        m_trGrpCreate = GetTransform(goAppCanvas, "Grp_Create");
        m_trGrpLogin = GetTransform(goAppCanvas, "Grp_Login");
        m_trGrpLoggedin = GetTransform(goAppCanvas, "Grp_Loggedin");
        m_trGrpPlay = GetTransform(goAppCanvas, "Grp_Play");
        m_trGrpOption = GetTransform(goAppCanvas, "Grp_Option");
        m_trGrpAccount = GetTransform(goAppCanvas, "Grp_Account");
        m_trGrpDialog = GetTransform(goAppCanvas, "Grp_Dialog");
        m_trGrpGame = GetTransform(goAppCanvas, "Grp_Game");

        // スタート画面
        {
            m_Btn_Start_Login = m_trGrpStart.Find("Btn_Login").GetComponentInChildren<Button>();
            m_Btn_Start_Login.onClick.AddListener(OnClick_Start_Login);
            SetTapOverComponent(m_Btn_Start_Login.gameObject, Select.Start);
            m_Btn_Start_Create = m_trGrpStart.Find("Btn_Create").GetComponent<Button>();
            m_Btn_Start_Create.onClick.AddListener(OnClick_Start_Create);
            SetTapOverComponent(m_Btn_Start_Create.gameObject, Select.Start);

            // ボタンリスト
            m_goStartSelectList = new List<GameObject>(2);
            m_goStartSelectList.Add(m_Btn_Start_Login.gameObject);
            m_goStartSelectList.Add(m_Btn_Start_Create.gameObject);
        }
        // ユーザー作成画面
        {
            Transform baseTr = m_trGrpCreate.Find("Content");
            // メールアドレス
            m_InputField_CreateEmail = baseTr.Find("Email/InputField").GetComponent<TMP_InputField>();
            m_InputField_CreateEmail.onSelect.AddListener(ProcOnSelect_Email);
            m_InputField_CreateEmail.gameObject.SetActive(!m_isMobilePlatform);
            m_InputField_CreateEmail.onEndEdit.AddListener(ProcOnEndEdit_Email);
            m_InputField_CreateEmail.onValueChanged.AddListener(ProcOnValueChanged_Email);
            m_InputField_CreateEmail.characterLimit = 32;        // 仮：Eメール文字数制限
            SetTapOverComponent(m_InputField_CreateEmail.gameObject, Select.Create);
            m_InputButton_CreateEmail = baseTr.Find("Email/Button").GetComponent<CustomInputButton>();
            m_InputButton_CreateEmail.gameObject.SetActive(m_isMobilePlatform);
            m_InputButton_CreateEmail.onClick.AddListener(OnClick_Email);
            SetTapOverComponent(m_InputButton_CreateEmail.gameObject, Select.Create);
            // ニックネーム
            //m_InputField_CreateName = baseTr.Find("NickName/InputField").GetComponent<TMP_InputField>();
            //m_InputField_CreateName.onEndEdit.AddListener(ProcOnEndEdit_Name);
            //m_InputField_CreateName.onValueChanged.AddListener(ProcOnValueChanged_Name);
            //m_InputField_CreateName.characterLimit = 32;       // 仮：名前文字数制限
            m_InputButton_CreateName = baseTr.Find("NickName/Button").GetComponent<CustomInputButton>();
            m_InputButton_CreateName.gameObject.SetActive(true);
            m_InputButton_CreateName.onClick.AddListener(OnClick_Name);
            SetTapOverComponent(m_InputButton_CreateName.gameObject, Select.Create);
            // 性別
            m_Btn_Create_Gender = baseTr.Find("Gender/Button").GetComponent<Button>();
            m_Btn_Create_Gender.onClick.AddListener(OnClick_CreateGender);
            m_CreateGenderText = baseTr.Find("Gender/Button").GetComponentInChildren<TextMeshProUGUI>();
            SetTapOverComponent(m_Btn_Create_Gender.gameObject, Select.Create);
            // 年代
            m_Btn_Create_Age = baseTr.Find("Age/Button").GetComponent<Button>();
            m_Btn_Create_Age.onClick.AddListener(OnClick_CreateAge);
            m_CreateAgeText = baseTr.Find("Age/Button").GetComponentInChildren<TextMeshProUGUI>();
            SetTapOverComponent(m_Btn_Create_Age.gameObject, Select.Create);
            // 職業
            m_Btn_Create_Work = baseTr.Find("Work/Button").GetComponent<Button>();
            m_Btn_Create_Work.onClick.AddListener(OnClick_CreateWork);
            m_CreateWorkText = baseTr.Find("Work/Button").GetComponentInChildren<TextMeshProUGUI>();
            SetTapOverComponent(m_Btn_Create_Work.gameObject, Select.Create);

            // パスワード
            m_InputField_CreatePassword = baseTr.Find("Password/InputField").GetComponent<TMP_InputField>();
            m_InputField_CreatePassword.onSelect.AddListener(ProcOnSelect_Password);
            m_InputField_CreatePassword.gameObject.SetActive(!m_isMobilePlatform);
            m_InputField_CreatePassword.onEndEdit.AddListener(ProcOnEndEdit_Password);
            m_InputField_CreatePassword.onValueChanged.AddListener(ProcOnValueChanged_Password);
            m_InputField_CreatePassword.characterLimit = 16;        // 仮：パスワード文字数制限
            SetTapOverComponent(m_InputField_CreatePassword.gameObject, Select.Create);
            m_InputButton_CreatePassword = baseTr.Find("Password/Button").GetComponent<CustomInputButton>();
            m_InputButton_CreatePassword.gameObject.SetActive(m_isMobilePlatform);
            m_InputButton_CreatePassword.onClick.AddListener(OnClick_Password);
            SetTapOverComponent(m_InputButton_CreatePassword.gameObject, Select.Create);

            m_Toggle_CreatePassword = baseTr.Find("Password/Toggle")?.GetComponent<Toggle>();
            if (m_Toggle_CreatePassword)
            {
                m_Toggle_CreatePassword.onValueChanged.AddListener(ProcOnValueChanged_PasswordToggle);
            }

            m_Btn_Create_Check = baseTr.Find("Create").GetComponentInChildren<Button>();
            m_Btn_Create_Check.onClick.AddListener(OnClick_CreateCheck);
            m_Btn_Create_Check.interactable = false;             // 初期状態は非押下
            SetTapOverComponent(m_Btn_Create_Check.gameObject, Select.Create);
            m_Btn_Create_Back = m_trGrpCreate.Find("Btn_Back").GetComponent<Button>();
            m_Btn_Create_Back.onClick.AddListener(OnClick_BackStart);
            SetTapOverComponent(m_Btn_Create_Back.gameObject, Select.Create);

            // ボタンリスト
            m_goCreateSelectList = new List<GameObject>(8);
            m_goCreateSelectList.Add(m_isMobilePlatform ? m_InputButton_CreateEmail.gameObject : m_InputField_CreateEmail.gameObject);
            m_goCreateSelectList.Add(m_InputButton_CreateName.gameObject);
            m_goCreateSelectList.Add(m_Btn_Create_Gender.gameObject);
            m_goCreateSelectList.Add(m_Btn_Create_Age.gameObject);
            m_goCreateSelectList.Add(m_Btn_Create_Work.gameObject);
            m_goCreateSelectList.Add(m_isMobilePlatform ? m_InputButton_CreatePassword.gameObject : m_InputField_CreatePassword.gameObject);
            m_goCreateSelectList.Add(m_Btn_Create_Check.gameObject);
            m_goCreateSelectList.Add(m_Btn_Create_Back.gameObject);

            m_trCreateGender = m_trGrpCreate.Find("Gender");
            {
                string[] _tbl = { "Btn_Male" , "Btn_Female" };
                m_CreateGenderBtns = new List<Button>(_tbl.Length + 1);
                m_CreateGenderTexts = new List<TextMeshProUGUI>(_tbl.Length);
                Button _btn;
                for (int i = 0; i < _tbl.Length; ++i)
                {
                    Transform tr = m_trCreateGender.Find(_tbl[i]);
                    _btn = tr.GetComponentInChildren<Button>();
                    int i1 = i;
                    _btn.onClick.AddListener(() => OnClick_CreateGenders(i1));
                    m_CreateGenderBtns.Add(_btn);
                    m_CreateGenderTexts.Add(_btn.transform.GetComponentInChildren<TextMeshProUGUI>());
                    SetTapOverComponent(_btn.gameObject, Select.CreateGender);
                }
                _btn = m_trCreateGender.Find("Btn_Back").GetComponent<Button>();
                _btn.onClick.AddListener(OnClick_CreateBack);
                m_CreateGenderBtns.Add(_btn);
                SetTapOverComponent(_btn.gameObject, Select.CreateGender);

                m_goCreate_Gender = m_CreateGenderBtns[0].gameObject;
            }

            m_trCreateAge = m_trGrpCreate.Find("Age");
            {
                string[] _tbl = { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                m_CreateAgeBtns = new List<Button>(_tbl.Length + 1);
                m_CreateAgeTexts = new List<TextMeshProUGUI>(_tbl.Length);
                Transform trBase = m_trCreateAge.Find("Scroll View/Viewport/Content");
                Button _btn;
                for (int i = 0; i < _tbl.Length; ++i)
                {
                    Transform tr = trBase.Find(_tbl[i] + "/Button");
                    _btn = tr.GetComponent<Button>();
                    int i1 = i;
                    _btn.onClick.AddListener(() => OnClick_CreateAges(i1));
                    m_CreateAgeBtns.Add(_btn);
                    m_CreateAgeTexts.Add(_btn.transform.GetComponentInChildren<TextMeshProUGUI>());
                    SetTapOverComponent(_btn.gameObject, Select.CreateAge);
                }
                _btn = m_trCreateAge.Find("Btn_Back").GetComponent<Button>();
                _btn.onClick.AddListener(OnClick_CreateBack);
                m_CreateAgeBtns.Add(_btn);
                SetTapOverComponent(_btn.gameObject, Select.CreateAge);

                m_goCreate_Age = m_CreateAgeBtns[0].gameObject;
            }

            m_trCreateWork = m_trGrpCreate.Find("Work");
            {
                string[] _tbl = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" };
                m_CreateWorkBtns = new List<Button>(_tbl.Length + 1);
                m_CreateWorkTexts = new List<TextMeshProUGUI>(_tbl.Length);
                Transform trBase = m_trCreateWork.Find("Scroll View/Viewport/Content");
                Button _btn;
                for (int i = 0; i < _tbl.Length; ++i)
                {
                    Transform tr = trBase.Find(_tbl[i] + "/Button");
                    _btn = tr.GetComponent<Button>();
                    int i1 = i;
                    _btn.onClick.AddListener(() => OnClick_CreateWorks(i1));
                    m_CreateWorkBtns.Add(_btn);
                    m_CreateWorkTexts.Add(_btn.transform.GetComponentInChildren<TextMeshProUGUI>());
                    SetTapOverComponent(_btn.gameObject, Select.CreateWork);
                }
                _btn = m_trCreateWork.Find("Btn_Back").GetComponent<Button>();
                _btn.onClick.AddListener(OnClick_CreateBack);
                m_CreateWorkBtns.Add(_btn);
                SetTapOverComponent(_btn.gameObject, Select.CreateWork);

                m_goCreate_Work = m_CreateWorkBtns[0].gameObject;
            }

            m_CreateTouchArea = m_trGrpCreate.Find("TouchArea").gameObject;
            m_CreateTouchArea.SetActive(false);
        }
        // ログイン画面
        {
            Transform baseTr = m_trGrpLogin;
            // メールアドレス
            m_InputField_LoginEmail = baseTr.Find("Email/InputField").GetComponent<TMP_InputField>();
            m_InputField_LoginEmail.onSelect.AddListener(ProcOnSelect_Email);
            m_InputField_LoginEmail.gameObject.SetActive(!m_isMobilePlatform);
            m_InputField_LoginEmail.onEndEdit.AddListener(ProcOnEndEdit_Email);
            m_InputField_LoginEmail.onValueChanged.AddListener(ProcOnValueChanged_Email);
            m_InputField_LoginEmail.characterLimit = 32;        // 仮：Eメール文字数制限
            SetTapOverComponent(m_InputField_LoginEmail.gameObject, Select.Login);
            m_InputButton_LoginEmail = baseTr.Find("Email/Button").GetComponent<CustomInputButton>();
            m_InputButton_LoginEmail.gameObject.SetActive(m_isMobilePlatform);
            m_InputButton_LoginEmail.onClick.AddListener(OnClick_Email);
            SetTapOverComponent(m_InputButton_LoginEmail.gameObject, Select.Login);
            // パスワード
            m_InputField_LoginPassword = baseTr.Find("Password/InputField").GetComponent<TMP_InputField>();
            m_InputField_LoginPassword.onSelect.AddListener(ProcOnSelect_Password);
            m_InputField_LoginPassword.gameObject.SetActive(!m_isMobilePlatform);
            m_InputField_LoginPassword.onEndEdit.AddListener(ProcOnEndEdit_Password);
            m_InputField_LoginPassword.onValueChanged.AddListener(ProcOnValueChanged_Password);
            m_InputField_LoginPassword.characterLimit = 16;        // 仮：パスワード文字数制限
            SetTapOverComponent(m_InputField_LoginPassword.gameObject, Select.Login);
            m_InputButton_LoginPassword = baseTr.Find("Password/Button").GetComponent<CustomInputButton>();
            m_InputButton_LoginPassword.gameObject.SetActive(m_isMobilePlatform);
            m_InputButton_LoginPassword.onClick.AddListener(OnClick_Password);
            SetTapOverComponent(m_InputButton_LoginPassword.gameObject, Select.Login);

            m_Toggle_LoginPassword = baseTr.Find("Password/Toggle")?.GetComponent<Toggle>();
            if (m_Toggle_LoginPassword)
            {
                m_Toggle_LoginPassword.onValueChanged.AddListener(ProcOnValueChanged_PasswordToggle);
            }

            m_Btn_Login_Check = baseTr.Find("Login").GetComponentInChildren<Button>();
            m_Btn_Login_Check.onClick.AddListener(OnClick_LoginCheck);
            m_Btn_Login_Check.interactable = false;              // 初期状態は非押下
            SetTapOverComponent(m_Btn_Login_Check.gameObject, Select.Login);
            m_Btn_Login_Back = baseTr.Find("Btn_Back").GetComponent<Button>();
            m_Btn_Login_Back.onClick.AddListener(OnClick_BackStart);
            SetTapOverComponent(m_Btn_Login_Back.gameObject, Select.Login);

            // ボタンリスト
            m_goLoginSelectList = new List<GameObject>(4);
            m_goLoginSelectList.Add(m_isMobilePlatform ? m_InputButton_LoginEmail.gameObject : m_InputField_LoginEmail.gameObject);
            m_goLoginSelectList.Add(m_isMobilePlatform ? m_InputButton_LoginPassword.gameObject : m_InputField_LoginPassword.gameObject);
            m_goLoginSelectList.Add(m_Btn_Login_Check.gameObject);
            m_goLoginSelectList.Add(m_Btn_Login_Back.gameObject);

            m_LoginTouchArea = m_trGrpLogin.Find("TouchArea").gameObject;
            m_LoginTouchArea.SetActive(false);
        }
        // ログイン後画面
        {
            m_Btn_Loggedin_Play = m_trGrpLoggedin.Find("Btn_Play").GetComponentInChildren<Button>();
            m_Btn_Loggedin_Play.onClick.AddListener(OnClick_Loggedin_Play);
            SetTapOverComponent(m_Btn_Loggedin_Play.gameObject, Select.Loggedin);
            m_Btn_Loggedin_Option = m_trGrpLoggedin.Find("Btn_Option").GetComponentInChildren<Button>();
            m_Btn_Loggedin_Option.onClick.AddListener(OnClick_Loggedin_Option);
            SetTapOverComponent(m_Btn_Loggedin_Option.gameObject, Select.Loggedin);
            m_Btn_Loggedin_Account = m_trGrpLoggedin.Find("Btn_Account").GetComponentInChildren<Button>();
            m_Btn_Loggedin_Account.onClick.AddListener(OnClick_Loggedin_Account);
            SetTapOverComponent(m_Btn_Loggedin_Account.gameObject, Select.Loggedin);
            m_Btn_Loggedin_Gamemode = m_trGrpLoggedin.Find("Btn_Game").GetComponentInChildren<Button>();
            m_Btn_Loggedin_Gamemode.onClick.AddListener(OnClick_Loggedin_Game);
            SetTapOverComponent(m_Btn_Loggedin_Gamemode.gameObject, Select.Loggedin);

            // ボタンリスト
            m_goLoggedinSelectList = new List<GameObject>(4);
            m_goLoggedinSelectList.Add(m_Btn_Loggedin_Play.gameObject);
            m_goLoggedinSelectList.Add(m_Btn_Loggedin_Option.gameObject);
            m_goLoggedinSelectList.Add(m_Btn_Loggedin_Account.gameObject);
            // テスト:ゲームモードの追加
            m_goLoggedinSelectList.Add(m_Btn_Loggedin_Gamemode.gameObject);

            //{
            //    Button btn = m_trGrpLoggedin.Find("Btn_Test").GetComponentInChildren<Button>();
            //    btn.onClick.AddListener(OnClick_Replay_Test);
            //}
        }
        // プレイ画面
        {
            m_Btn_Play_Free = m_trGrpPlay.Find("Free").GetComponentInChildren<Button>();
            m_Btn_Play_Free.onClick.AddListener(OnClick_Play_Free);
            SetTapOverComponent(m_Btn_Play_Free.gameObject, Select.Play);
            m_Btn_Play_Mission = m_trGrpPlay.Find("Mission").GetComponentInChildren<Button>();
            m_Btn_Play_Mission.onClick.AddListener(OnClick_Play_Mission);
            SetTapOverComponent(m_Btn_Play_Mission.gameObject, Select.Play);
            m_Btn_Play_Replay = m_trGrpPlay.Find("Replay").GetComponentInChildren<Button>();
            m_Btn_Play_Replay.onClick.AddListener(OnClick_Play_Replay);
            SetTapOverComponent(m_Btn_Play_Replay.gameObject, Select.Play);
            m_Btn_Play_Back = m_trGrpPlay.Find("Btn_Back").GetComponent<Button>();
            m_Btn_Play_Back.onClick.AddListener(OnClick_BackLoggedin);
            SetTapOverComponent(m_Btn_Play_Back.gameObject, Select.Play);

            // ボタンリスト
            m_goPlaySelectList = new List<GameObject>(4);
            m_goPlaySelectList.Add(m_Btn_Play_Free.gameObject);
            m_goPlaySelectList.Add(m_Btn_Play_Mission.gameObject);
            m_goPlaySelectList.Add(m_Btn_Play_Replay.gameObject);
            m_goPlaySelectList.Add(m_Btn_Play_Back.gameObject);

            m_trPlayReplaySelect = m_trGrpPlay.Find("ReplaySelect");
            m_Btn_ReplaySelect_Back = m_trPlayReplaySelect.Find("Btn_Back").GetComponent<Button>();
            m_Btn_ReplaySelect_Back.onClick.AddListener(OnClick_Replay_Back);
            SetTapOverComponent(m_Btn_ReplaySelect_Back.gameObject, Select.PlayReplay);
            m_ReplayBtns = new List<Button>();

            m_trPlayMissionSelect = m_trGrpPlay.Find("MissionSelect");
            m_Btn_MissionSelect_Back = m_trPlayMissionSelect.Find("Btn_Back").GetComponent<Button>();
            m_Btn_MissionSelect_Back.onClick.AddListener(OnClick_Mission_Back);
            m_Btn_MissionSelect_Square = m_trPlayMissionSelect.Find("Btn_Square").GetComponentInChildren<Button>();
            m_Btn_MissionSelect_Square.onClick.AddListener(OnClick_Mission_Square);
            m_Btn_MissionSelect_Eight = m_trPlayMissionSelect.Find("Btn_Eight").GetComponentInChildren<Button>();
            m_Btn_MissionSelect_Eight.onClick.AddListener(OnClick_Mission_Eight);
            m_Btn_MissionSelect_Happning = m_trPlayMissionSelect.Find("Btn_Happning").GetComponentInChildren<Button>();
            m_Btn_MissionSelect_Happning.onClick.AddListener(OnClick_Mission_Happning);
            m_MissionBtns = new List<Button>();
            m_MissionBtns.Add(m_Btn_MissionSelect_Square);
            m_MissionBtns.Add(m_Btn_MissionSelect_Eight);
            m_MissionBtns.Add(m_Btn_MissionSelect_Happning);
            m_MissionBtns.Add(m_Btn_MissionSelect_Back);
        }
        // ゲームモード選択画面
        {
            m_Btn_Gamemode_Treasure = m_trGrpGame.Find("Treasure").GetComponentInChildren<Button>();
            m_Btn_Gamemode_Treasure.onClick.AddListener(OnClick_Game_Treasure);
            SetTapOverComponent(m_Btn_Gamemode_Treasure.gameObject, Select.Gamemode);
            m_Btn_Gamemode_Iraira = m_trGrpGame.Find("Irairabou").GetComponentInChildren<Button>();
            m_Btn_Gamemode_Iraira.onClick.AddListener(OnClick_Game_Irairabou);
            SetTapOverComponent(m_Btn_Gamemode_Iraira.gameObject, Select.Gamemode);
            m_Btn_Gamemode_Endless = m_trGrpGame.Find("Endress").GetComponentInChildren<Button>();
            m_Btn_Gamemode_Endless.onClick.AddListener(OnClick_Game_Endress);
            SetTapOverComponent(m_Btn_Gamemode_Endless.gameObject, Select.Gamemode);
            m_Btn_Gamemode_Back = m_trGrpGame.Find("Btn_Back").GetComponent<Button>();
            m_Btn_Gamemode_Back.onClick.AddListener(OnClick_BackLoggedin);
            SetTapOverComponent(m_Btn_Gamemode_Back.gameObject, Select.Gamemode);

            // ボタンリスト
            m_goGamemodeSelectList = new List<GameObject>(4);
            m_goGamemodeSelectList.Add(m_Btn_Gamemode_Treasure.gameObject);
            m_goGamemodeSelectList.Add(m_Btn_Gamemode_Iraira.gameObject);
            m_goGamemodeSelectList.Add(m_Btn_Gamemode_Endless.gameObject);
            m_goGamemodeSelectList.Add(m_Btn_Gamemode_Back.gameObject);

#if false
            m_trPlayReplaySelect = m_trGrpPlay.Find("ReplaySelect");
            m_Btn_ReplaySelect_Back = m_trPlayReplaySelect.Find("Btn_Back").GetComponent<Button>();
            m_Btn_ReplaySelect_Back.onClick.AddListener(OnClick_Replay_Back);
            SetTapOverComponent(m_Btn_ReplaySelect_Back.gameObject, Select.PlayReplay);
            m_ReplayBtns = new List<Button>();

            m_trPlayMissionSelect = m_trGrpPlay.Find("MissionSelect");
            m_Btn_MissionSelect_Back = m_trPlayMissionSelect.Find("Btn_Back").GetComponent<Button>();
            m_Btn_MissionSelect_Back.onClick.AddListener(OnClick_Mission_Back);
            m_Btn_MissionSelect_Square = m_trPlayMissionSelect.Find("Btn_Square").GetComponentInChildren<Button>();
            m_Btn_MissionSelect_Square.onClick.AddListener(OnClick_Mission_Square);
            m_Btn_MissionSelect_Eight = m_trPlayMissionSelect.Find("Btn_Eight").GetComponentInChildren<Button>();
            m_Btn_MissionSelect_Eight.onClick.AddListener(OnClick_Mission_Eight);
            m_Btn_MissionSelect_Happning = m_trPlayMissionSelect.Find("Btn_Happning").GetComponentInChildren<Button>();
            m_Btn_MissionSelect_Happning.onClick.AddListener(OnClick_Mission_Happning);
            m_MissionBtns = new List<Button>();
            m_MissionBtns.Add(m_Btn_MissionSelect_Square);
            m_MissionBtns.Add(m_Btn_MissionSelect_Eight);
            m_MissionBtns.Add(m_Btn_MissionSelect_Happning);
            m_MissionBtns.Add(m_Btn_MissionSelect_Back);
#endif
        }
        // オプション画面
        {
            Transform baseTr = m_trGrpOption.Find("Scroll View/Viewport/Content");
            string[] _tbl = { "Btn_Mode", "Btn_Controller", "Space1", "Btn_Grid", "Btn_Pad", "Btn_Fly", "Btn_Metronome", "Btn_Bpm" };
            m_goOptionBtns = new List<GameObject>(_tbl.Length + 1);
            Button btn;
            for (int i = 0; i < _tbl.Length; ++i)
            {
                if (i == OptionMetronomeBPMIndex)
                {
                    m_OptionBPMSlider = baseTr.Find(_tbl[i]).GetComponentInChildren<Slider>();
                    m_OptionBPMSlider.onValueChanged.AddListener(ProcOnValueChanged_BPM);
                    m_OptionBPMText = m_OptionBPMSlider.transform.Find("Img_1").GetComponentInChildren<TextMeshProUGUI>();
                    m_goOptionBtns.Add(m_OptionBPMSlider.gameObject);
                    SetTapOverComponent(m_OptionBPMSlider.gameObject, Select.Option);
                }
                else
                {
                    btn = baseTr.Find(_tbl[i]).GetComponentInChildren<Button>();
                    int i1 = i;
                    btn.onClick.AddListener(() => OnClick_OptionBtn(i1));
                    m_goOptionBtns.Add(btn.gameObject);
                    SetTapOverComponent(btn.gameObject, Select.Option);
                }
            }
            btn = m_trGrpOption.Find("Btn_Back").GetComponent<Button>();
            btn.onClick.AddListener(OnClick_BackLoggedin);
            m_goOptionBtns.Add(btn.gameObject);
            SetTapOverComponent(btn.gameObject, Select.Option);

            // オプションボタン周りの初期表示
            SetOptionBtns();
            // キーボード設定
            m_trOptionKeyboard = m_trGrpOption.Find("Keyboard");
            SetOptionKeyboardBtns();
            m_trOptionKeyboard.gameObject.SetActive(false);
            // ゲームパッド設定
            m_trOptionGamepad = m_trGrpOption.Find("Gamepad");
            SetOptionGamepadBtns();
            m_trOptionGamepad.gameObject.SetActive(false);
        }
        // アカウント設定画面
        {
            string findStr = (m_isMobilePlatform ? "Scroll View/Viewport/" : "") + "Content";
            Transform baseTr = m_trGrpAccount.Find(findStr);
            // メールアドレス
            m_InputField_AccountEmail = baseTr.Find("Email/InputField").GetComponent<TMP_InputField>();
            m_InputField_AccountEmail.onSelect.AddListener(ProcOnSelect_Email);
            m_InputField_AccountEmail.gameObject.SetActive(!m_isMobilePlatform);
            m_InputField_AccountEmail.onEndEdit.AddListener(ProcOnEndEdit_Email);
            m_InputField_AccountEmail.onValueChanged.AddListener(ProcOnValueChanged_Email);
            m_InputField_AccountEmail.characterLimit = 32;        // 仮：Eメール文字数制限
            SetTapOverComponent(m_InputField_AccountEmail.gameObject, Select.Account);
            m_InputButton_AccountEmail = baseTr.Find("Email/Button").GetComponent<CustomInputButton>();
            m_InputButton_AccountEmail.gameObject.SetActive(m_isMobilePlatform);
            m_InputButton_AccountEmail.onClick.AddListener(OnClick_Email);
            SetTapOverComponent(m_InputButton_AccountEmail.gameObject, Select.Account);
            // ニックネーム
            //m_InputField_AccountName = baseTr.Find("NickName/InputField").GetComponent<TMP_InputField>();
            //m_InputField_AccountName.onEndEdit.AddListener(ProcOnEndEdit_Name);
            //m_InputField_AccountName.onValueChanged.AddListener(ProcOnValueChanged_Name);
            //m_InputField_AccountName.characterLimit = 32;       // 仮：名前文字数制限
            m_InputButton_AccountName = baseTr.Find("NickName/Button").GetComponent<CustomInputButton>();
            m_InputButton_AccountName.gameObject.SetActive(true);
            m_InputButton_AccountName.onClick.AddListener(OnClick_Name);
            SetTapOverComponent(m_InputButton_AccountName.gameObject, Select.Account);
            // 性別
            m_InputField_AccountGender = baseTr.Find("Gender/InputField").GetComponent<TMP_InputField>();
            // 年代
            m_InputField_AccountAge = baseTr.Find("Age/InputField").GetComponent<TMP_InputField>();
            // 職業
            m_InputField_AccountWork = baseTr.Find("Work/InputField").GetComponent<TMP_InputField>();
            // 新しいパスワード
            m_InputField_AccountPassword = baseTr.Find("NewPassword/InputField").GetComponent<TMP_InputField>();
            m_InputField_AccountPassword.onSelect.AddListener(ProcOnSelect_Password);
            m_InputField_AccountPassword.gameObject.SetActive(!m_isMobilePlatform);
            m_InputField_AccountPassword.onEndEdit.AddListener(ProcOnEndEdit_Password);
            m_InputField_AccountPassword.onValueChanged.AddListener(ProcOnValueChanged_Password);
            m_InputField_AccountPassword.characterLimit = 16;        // 仮：パスワード文字数制限
            SetTapOverComponent(m_InputField_AccountPassword.gameObject, Select.Account);
            m_InputButton_AccountPassword = baseTr.Find("NewPassword/Button").GetComponent<CustomInputButton>();
            m_InputButton_AccountPassword.gameObject.SetActive(m_isMobilePlatform);
            m_InputButton_AccountPassword.onClick.AddListener(OnClick_Password);
            SetTapOverComponent(m_InputButton_AccountPassword.gameObject, Select.Account);

            m_Toggle_AccountPassword = baseTr.Find("NewPassword/Toggle")?.GetComponent<Toggle>();
            if (m_Toggle_AccountPassword)
            {
                m_Toggle_AccountPassword.onValueChanged.AddListener(ProcOnValueChanged_PasswordToggle);
            }

            // 現在のパスワード
            m_InputField_AccountNowPassword = baseTr.Find("Password/InputField").GetComponent<TMP_InputField>();
            m_InputField_AccountNowPassword.onSelect.AddListener(ProcOnSelect_NowPassword);
            m_InputField_AccountNowPassword.gameObject.SetActive(!m_isMobilePlatform);
            m_InputField_AccountNowPassword.onEndEdit.AddListener(ProcOnEndEdit_NowPassword);
            m_InputField_AccountNowPassword.onValueChanged.AddListener(ProcOnValueChanged_NowPassword);
            m_InputField_AccountNowPassword.characterLimit = 16;        // 仮：パスワード文字数制限
            SetTapOverComponent(m_InputField_AccountNowPassword.gameObject, Select.Account);
            m_InputButton_AccountNowPassword = baseTr.Find("Password/Button").GetComponent<CustomInputButton>();
            m_InputButton_AccountNowPassword.gameObject.SetActive(m_isMobilePlatform);
            m_InputButton_AccountNowPassword.onClick.AddListener(OnClick_NowPassword);
            SetTapOverComponent(m_InputButton_AccountNowPassword.gameObject, Select.Account);

            m_Toggle_AccountNowPassword = baseTr.Find("Password/Toggle")?.GetComponent<Toggle>();
            if (m_Toggle_AccountNowPassword)
            {
                m_Toggle_AccountNowPassword.onValueChanged.AddListener(ProcOnValueChanged_NowPasswordToggle);
            }

            m_Btn_Account_Change = baseTr.Find("Change").GetComponentInChildren<Button>();
            m_Btn_Account_Change.onClick.AddListener(OnClick_Account_Change);
            m_Btn_Account_Change.interactable = false;             // 初期状態は非押下
            SetTapOverComponent(m_Btn_Account_Change.gameObject, Select.Account);
            m_Btn_Account_Back = m_trGrpAccount.Find("Btn_Back").GetComponent<Button>();
            m_Btn_Account_Back.onClick.AddListener(OnClick_BackLoggedin);
            SetTapOverComponent(m_Btn_Account_Back.gameObject, Select.Account);
            m_Btn_Account_Logout = baseTr.Find("Logout/Btn_Logout").GetComponent<Button>();
            m_Btn_Account_Logout.onClick.AddListener(OnClick_Logout);
            SetTapOverComponent(m_Btn_Account_Logout.gameObject, Select.Account);
            m_Btn_Account_Delete = baseTr.Find("Delete/Btn_Delete").GetComponent<Button>();
            m_Btn_Account_Delete.onClick.AddListener(OnClick_Delete);
            SetTapOverComponent(m_Btn_Account_Delete.gameObject, Select.Account);

            // ボタンリスト
            m_goAccountSelectList = new List<GameObject>(8);
            m_goAccountSelectList.Add(m_isMobilePlatform ? m_InputButton_AccountEmail.gameObject : m_InputField_AccountEmail.gameObject);
            m_goAccountSelectList.Add(m_InputButton_AccountName.gameObject);
            m_goAccountSelectList.Add(m_isMobilePlatform ? m_InputButton_AccountPassword.gameObject : m_InputField_AccountPassword.gameObject);
            m_goAccountSelectList.Add(m_isMobilePlatform ? m_InputButton_AccountNowPassword.gameObject : m_InputField_AccountNowPassword.gameObject);
            m_goAccountSelectList.Add(m_Btn_Account_Change.gameObject);
            m_goAccountSelectList.Add(m_Btn_Account_Logout.gameObject);
            m_goAccountSelectList.Add(m_Btn_Account_Delete.gameObject);
            m_goAccountSelectList.Add(m_Btn_Account_Back.gameObject);

            m_AccountTouchArea = m_trGrpAccount.Find("TouchArea").gameObject;
            m_AccountTouchArea.SetActive(false);
        }
        // ダイアログ
        {
            m_Btn_Dialog_Cancel = m_trGrpDialog.Find("Btn_Back").GetComponent<Button>();
            m_Btn_Dialog_Cancel.onClick.AddListener(OnClick_Dialog_Cancel);
            m_Btn_Dialog_Cancel.gameObject.SetActive(false);
            SetTapOverComponent(m_Btn_Dialog_Cancel.gameObject, Select.Dialog);
            m_DialogConfirm = m_trGrpDialog.Find("Btn_YesNo").gameObject;
            m_Btn_Dialog_ConfirmY = m_DialogConfirm.transform.Find("Btn_Yes").GetComponentInChildren<Button>();
            m_Btn_Dialog_ConfirmY.onClick.AddListener(OnClick_Dialog_ConfirmY);
            SetTapOverComponent(m_Btn_Dialog_ConfirmY.gameObject, Select.Dialog);
            m_Btn_Dialog_ConfirmN = m_DialogConfirm.transform.Find("Btn_No").GetComponentInChildren<Button>();
            m_Btn_Dialog_ConfirmN.onClick.AddListener(OnClick_Dialog_ConfirmN);
            SetTapOverComponent(m_Btn_Dialog_ConfirmN.gameObject, Select.Dialog);
            m_DialogConfirm.SetActive(false);
        }

        m_goExcludeDesideSeList = new List<GameObject>();
        m_goExcludeDesideSeList.Add(m_InputField_CreateEmail.gameObject);
        m_goExcludeDesideSeList.Add(m_InputButton_CreateEmail.gameObject);
        m_goExcludeDesideSeList.Add(m_InputButton_CreateName.gameObject);
        m_goExcludeDesideSeList.Add(m_InputField_CreatePassword.gameObject);
        m_goExcludeDesideSeList.Add(m_InputButton_CreatePassword.gameObject);
        m_goExcludeDesideSeList.Add(m_InputField_AccountEmail.gameObject);
        m_goExcludeDesideSeList.Add(m_InputButton_AccountEmail.gameObject);
        m_goExcludeDesideSeList.Add(m_InputButton_AccountName.gameObject);
        m_goExcludeDesideSeList.Add(m_InputField_AccountPassword.gameObject);
        m_goExcludeDesideSeList.Add(m_InputButton_AccountPassword.gameObject);
        m_goExcludeDesideSeList.Add(m_InputField_AccountNowPassword.gameObject);
        m_goExcludeDesideSeList.Add(m_InputButton_AccountNowPassword.gameObject);

        m_trGrpStart.gameObject.SetActive(false);
        m_trGrpCreate.gameObject.SetActive(false);
        m_trGrpLogin.gameObject.SetActive(false);
        m_trGrpLoggedin.gameObject.SetActive(false);
        m_trGrpPlay.gameObject.SetActive(false);
        m_trGrpOption.gameObject.SetActive(false);
        m_trGrpAccount.gameObject.SetActive(false);
        m_trGrpDialog.gameObject.SetActive(false);
        m_trPlayReplaySelect.gameObject.SetActive(false);
        m_trGrpGame.gameObject.SetActive(false);

        // インスタンスIDの紐づけを行う
        var instanceId = GetInstanceID();
        Instances.Add(instanceId, this);

        // 前回ログインしたメアドとパスワードをローカルから取得
        m_LoginEmailStr = AppData.EmailAddress;
        m_LoginPasswordStr = AppData.Password;

        SetDefaultSelect();

        //// BGM:
        //MG_Mediator.GetAudio().PlayBgm(AudioId.BGM01.ToString());

        // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
        FB_InitializeAppJS();
        //ActivateOnAuthStateChanged();
#endif

        ShTransitionSys.SetFillOutCover();
    }

    // Update is called once per frame
    void Update()
    {
        // ユーザー情報の監視
        // ※期限切れの場合はダイアログを出してログアウトさせる
        ReloadUserInfo();
        // リプレイデータの読み込み
        ReplayData();
        switch (m_MenuPhase)
        {
            case MenuPhase.Start:

                if (string.IsNullOrEmpty(m_LoginEmailStr) && string.IsNullOrEmpty(m_LoginPasswordStr))
                {
                    // スタート画面へ
                    ChangeMenuPhase(MenuPhase.Start_Start);
                }
                else
                {
                    // ユーザー再認証　※期限切れの可能性も考慮して、カレントユーザー取得ではなく、ユーザー再認証とする
                    ChangeMenuPhase(MenuPhase.Start_User_Start);
                }
                break;

            case MenuPhase.Start_User_Start:
                m_Timer = 0.0f;

                // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                ReauthenticateWithCredential();
#else
                // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
                m_isReceived = true;
                m_isSuccess = true;
                SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif

                m_MenuPhase = MenuPhase.Start_User_Update;
                break;

            case MenuPhase.Start_User_Update:
                if (m_isReceived && ShTransitionSys.IsTransitionEnd())
                {
                    bool isSelectPlayMode = false;
                    // ログイン中
                    if (m_isSuccess)
                    {
                        Debug.Log("PlayMode is " + AppData.m_PlayMode.ToString());
                        if (AppData.m_PlayMode != AppData.PlayMode.None)
                        {
                            m_prePlayMode = AppData.m_PlayMode;
                            AppData.m_PlayMode = AppData.PlayMode.None;
                            isSelectPlayMode = true;
                        }
                    }
                    if (isSelectPlayMode)
                    {
                        ChangeMenuPhase(MenuPhase.Start_UserInfo_Start);
                    }
                    else
                    {
                        // スタート画面へ
                        ChangeMenuPhase(MenuPhase.Start_Start);
                    }
                }
                break;

            case MenuPhase.Start_UserInfo_Start:
                m_Timer = 0.0f;

                // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                GetUserInfoStorage();
#else
                // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
                m_isReceived = true;
                m_isSuccess = true;

                // ダミーで入れる
                DummySetReceiveReplayCountStorageInfo();

                SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif

                m_MenuPhase = MenuPhase.Start_UserInfo_Update;
                break;

            case MenuPhase.Start_UserInfo_Update:
                if (m_isReceived)
                {
                    // 成功
                    if (m_isSuccess)
                    {
                        switch (m_prePlayMode)
                        {
                            case AppData.PlayMode.Free:
                                m_goPlay_Select = m_goPlaySelectList[0];
                                break;
                            case AppData.PlayMode.Mission:
                                m_goPlay_Select = m_goPlaySelectList[1];
                                break;
                            case AppData.PlayMode.Replay:
                                m_goPlay_Select = m_goPlaySelectList[2];
                                break;
                            case AppData.PlayMode.Game_Treasure:
                                m_goGamemode_Select = m_goGamemodeSelectList[0];
                                break;
                            case AppData.PlayMode.Game_Irairabou:
                                m_goGamemode_Select = m_goGamemodeSelectList[1];
                                break;
                            case AppData.PlayMode.Game_Endress:
                                m_goGamemode_Select = m_goGamemodeSelectList[2];
                                break;
                        }

                        if (!isSendReplayData())
                        {
                            // リプレイデータ送信開始
                            m_isSendReplayData = true;
                            m_MenuPhase = MenuPhase.Start_SendReplay_Update;
                            // アップロード中
                            SetDialog(true, ReplayUploadingMessage, DialogType.Type1, MenuPhase.None, MenuPhase.None, -1.0f);
                        }
                        else
                        {
                            m_isChecking = false;
                            // ユーザー名表示
                            SetUserName();
                            if (IsPlayMode(m_prePlayMode))
                            {
                                // プレイモード選択画面へ
                                ChangeMenuPhase(MenuPhase.Play_Start);
                            }
                            else
                            {
                                // ゲームモード選択画面へ
                                ChangeMenuPhase(MenuPhase.Game_Start);
                            }
                            m_prePlayMode = AppData.PlayMode.None;
                        }
                    }
                    else
                    {
                        // エラー時にはスタート画面にする
                        ChangeMenuPhase(MenuPhase.Start_Start);
                    }
                }
                break;

            case MenuPhase.Start_SendReplay_Update:
                if (!m_isSendReplayData)
                {
                    SetDialog(false);
                    m_MenuPhase = MenuPhase.Start_ReplayInfo_Start;
                }
                break;

            case MenuPhase.Start_ReplayInfo_Start:
                m_Timer = 0.0f;

                // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                GetReplayInfoCountStorage();
#else
                // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
                m_isReceived = true;
                m_isSuccess = true;
                SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif
                m_MenuPhase = MenuPhase.Start_ReplayInfo_Update;
                break;

            case MenuPhase.Start_ReplayInfo_Update:
                if (m_isReceived)
                {
                    m_isChecking = false;
                    // リプレイデータ読み込み開始
                    m_isGetReplayData = true;
                    // ユーザー名表示
                    SetUserName();
                    if (IsPlayMode(m_prePlayMode))
                    {
                        // プレイモード選択画面へ
                        ChangeMenuPhase(MenuPhase.Play_Start);
                    }
                    else
                    {
                        // ゲームモード選択画面へ
                        ChangeMenuPhase(MenuPhase.Game_Start);
                    }
                    m_prePlayMode = AppData.PlayMode.None;
                }
                break;

            case MenuPhase.Start_Start:
                ClearDisableInput();
                SetStatusText("select login or create");
                SetSelectGameObject(m_goStart_Select);
                m_MenuPhase = MenuPhase.Start_Update;
                break;

            case MenuPhase.Start_Update:
                InputCtrl();
                break;

            case MenuPhase.Create_Start:
                m_Btn_Create_Check.interactable = IsEnableCreateCheck();

                // ユーザー作成が有効な場合には、初期カーソル位置をそちらに合わせる
                if (m_Btn_Create_Check.interactable)
                {
                    m_goCreate_Select = m_goCreateSelectList[m_goCreateSelectList.Count - 2];
                }
                // それ以外の場合はメールアドレスボタンが初期カーソル位置
                else
                {
                    m_goCreate_Select = m_goCreateSelectList[0];
                }

                ClearDisableInput();
                // 戻るボタン有効
                m_isChecking = false;
                // 入力可にする
                m_CreateTouchArea.SetActive(false);

                //// 職業選択のスクロール位置を最初に戻しておく
                //SetContentPosition(m_trCreateWork.Find("Scroll View/Viewport/Content").gameObject, ContentType.Type1, 0);

                m_Timer = 0.0f;

                SetStatusText("create user");

                m_isReceived = false;
                m_isSuccess = false;

                SetSelectGameObject(m_goCreate_Select);

                m_MenuPhase = MenuPhase.Create_Update;
                break;

            case MenuPhase.Create_Update:
                InputCtrl();
                if (m_isReceived)
                {
                    // ユーザー作成成功
                    if (m_isSuccess)
                    {
                        // この時点ではパスワードは保存されていないので、エラーによる削除用のパスワードを保存しておく
                        m_DeleteUserPasswordStr = m_CreatePasswordStr;

                        // ユーザー専用のストレージ作成へ
                        m_MenuPhase = MenuPhase.Create_UserInfo_Start;
                    }
                }
                break;

            case MenuPhase.Create_UserInfo_Start:
                {
                    m_Timer = 0.0f;
                    // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                    CreateUserInfoStorage();
#else
                    // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
                    m_isReceived = true;
                    m_isSuccess = true;
                    SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif
                    m_MenuPhase = MenuPhase.Create_UserInfo_Update;
                }
                break;

            case MenuPhase.Create_UserInfo_Update:
                if (m_isReceived)
                {
                    // firebase.storageへユーザー情報jsonのアップロード成功
                    if (m_isSuccess)
                    {
                        if (string.IsNullOrEmpty(m_CreateNameStr))
                        {
                            // ユーザー登録確認メール送信処理へ
                            m_MenuPhase = MenuPhase.Create_SendEmail_Start;
                        }
                        else
                        {
                            // FB_RegisterUserJS で作成したユーザーの DisplayName は null なので
                            // ユーザープロパティを更新する形で DisplayName を割り当てる
                            m_AccountNameStr = m_CreateNameStr;
                            m_MenuPhase = MenuPhase.Create_Profile_Start;
                        }
                    }
                }
                break;

            case MenuPhase.Create_Profile_Start:
                {
                    m_Timer = 0.0f;
                    // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                    UpdateProfile();
#else
                    // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
                    m_isReceived = true;
                    m_isSuccess = true;
                    SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif
                    m_MenuPhase = MenuPhase.Create_Profile_Update;
                }
                break;

            case MenuPhase.Create_Profile_Update:
                if (m_isReceived)
                {
                    // ユーザープロファイル更新成功
                    if (m_isSuccess)
                    {
                        // ユーザー登録確認メール送信処理へ
                        m_MenuPhase = MenuPhase.Create_SendEmail_Start;
                    }
                }
                break;

            case MenuPhase.Create_SendEmail_Start:
                {
                    m_Timer = 0.0f;
                    // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                    SendEmailVerification();
#else
                    // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
                    m_isReceived = true;
                    m_isSuccess = true;
                    SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif
                    m_MenuPhase = MenuPhase.Create_SendEmail_Update;
                }
                break;

            case MenuPhase.Create_SendEmail_Update:
                if (m_isReceived)
                {
                    // ユーザー登録確認メール送信成功
                    if (m_isSuccess)
                    {
                        // Eメール確認処理へ
                        m_MenuPhase = MenuPhase.Create_EmailVerified_Start;
                    }
                }
                break;

            case MenuPhase.Create_EmailVerified_Start:
                {
                    ClearDisableInput();

                    // ダイアログタイマー無効、キャンセル付き
                    SetDialog(true, MailConfirmMessage, DialogType.Type3, MenuPhase.Create_Start, MenuPhase.None, -1);

                    m_Timer = 0.0f;
                    m_isCancelFromUser = false;

                    // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                    CreateEmailVerified();
#else
                    // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
                    m_isReceived = true;
                    m_isSuccess = true;
                    SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif
                    m_MenuPhase = MenuPhase.Create_EmailVerified_Update;
                }
                break;

            case MenuPhase.Create_EmailVerified_Update:
                if (m_isReceived)
                {
                    // ユーザーによるキャンセル優先
                    if (m_isCancelFromUser)
                    {

                    }
                    else
                    // Eメール確認待ち
                    if (m_isSuccess)
                    {
                        SetDialog(false);
                        // ユーザー作成成功した時のメアドとパスワード保存
                        AppData.SetEmailAddress(m_CreateEmailStr);
                        AppData.SetPassword(m_CreatePasswordStr);
                        AppCommon.Update_And_SaveGameData();
                        m_isChecking = false;
                        // ログイン中処理へ
                        ChangeMenuPhase(MenuPhase.Loggedin_Start);
                    }
                }
                break;

            case MenuPhase.Login_Start:
                m_InputField_LoginEmail.text = m_LoginEmailStr;
                m_InputField_LoginPassword.text = m_LoginPasswordStr;
                SetInputButtonStr(m_InputButton_LoginEmail, m_LoginEmailStr);
                SetInputButtonStrPassword(m_InputButton_LoginPassword, m_LoginPasswordStr);

                m_Btn_Login_Check.interactable = IsEnableLoginCheck();

                // ログインチェックが有効ならそちらにカーソルを合わせる
                if (m_Btn_Login_Check.interactable)
                {
                    m_goLogin_Select = m_goLoginSelectList[m_goLoginSelectList.Count - 2];
                }
                else
                // 戻るボタンにカーソルが合っていたら、メールアドレスのボタンに変更する
                if (m_goLogin_Select == m_Btn_Login_Back.gameObject)
                {
                    m_goLogin_Select = m_goLoginSelectList[0];
                }

                ClearDisableInput();
                // 戻るボタン有効
                m_isChecking = false;
                // 入力可にする
                m_LoginTouchArea.SetActive(false);

                m_Timer = 0.0f;

                SetStatusText("enter email and password");

                m_isReceived = false;
                m_isSuccess = false;

                SetSelectGameObject(m_goLogin_Select);

                m_MenuPhase = MenuPhase.Login_Update;
                break;

            case MenuPhase.Login_Update:
                InputCtrl();
                if (m_isReceived)
                {
                    // サインイン成功
                    if (m_isSuccess)
                    {
                        // ログイン成功した時のメアドとパスワード保存
                        AppData.SetEmailAddress(m_LoginEmailStr);
                        AppData.SetPassword(m_LoginPasswordStr);
                        AppCommon.Update_And_SaveGameData();
                        // ストレージからのユーザー情報取得処理へ
                        m_MenuPhase = MenuPhase.Login_UserInfo_Start;
                    }
                }
                break;

            case MenuPhase.Login_UserInfo_Start:
                m_Timer = 0.0f;

                // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                GetUserInfoStorage();
#else
                // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
                m_isReceived = true;
                m_isSuccess = true;
                SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif

                m_MenuPhase = MenuPhase.Login_UserInfo_Update;
                break;

            case MenuPhase.Login_UserInfo_Update:
                if (m_isReceived)
                {
                    m_isChecking = false;
                    // 成功
                    if (m_isSuccess)
                    {
                        m_MenuPhase = MenuPhase.Login_ReplayInfo_Start;
                    }
                }
                break;

            case MenuPhase.Login_ReplayInfo_Start:
           m_Timer = 0.0f;

                // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                GetReplayInfoCountStorage();
#else
                // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
                m_isReceived = true;
                m_isSuccess = true;
                // ダミーで入れる
                DummySetReceiveReplayCountStorageInfo();
                SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif

                m_MenuPhase = MenuPhase.Login_ReplayInfo_Update;
                break;

            case MenuPhase.Login_ReplayInfo_Update:
                if (m_isReceived)
                {
                    m_isChecking = false;
                    // リプレイデータ読み込み開始
                    m_isGetReplayData = true;
                    // ログイン中処理へ
                    ChangeMenuPhase(MenuPhase.Loggedin_Start);
                }
                break;

            case MenuPhase.Loggedin_Start:
                m_Timer = 0.0f;

                SetSelectGameObject(m_goLoggedin_Select);

                if (m_ReceiveUserInfo != null)
                {
                    SetUserName();
                    SetStatusText("Loggedin:[" + m_ReceiveUserInfo.displayName + "]");
                }
                else
                {
                    SetStatusText("Loggedin info is null");
                }

                ClearDisableInput();
                m_isReceived = false;
                m_isSuccess = false;

                m_MenuPhase = MenuPhase.Loggedin_Update;
                break;

            case MenuPhase.Loggedin_Update:
                InputCtrl();
                break;

            case MenuPhase.Play_Start:
                if (!isGetReplayData())
                {
                    if (m_crGetReplayDataWait != null)
                    {
                        StopCoroutine(m_crGetReplayDataWait);
                        m_crGetReplayDataWait = null;
                        Debug.Log("delete coroutine [crGetReplayDataWait]");
                    }
                    m_crGetReplayDataWait = StartCoroutine(crGetReplayDataWait());
                }
                else
                {
                    GameObject goAlert = m_trGrpPlay.Find("Alert").gameObject;
                    goAlert.SetActive(false);
                }

                m_Btn_Play_Replay.interactable = isEnableReplayData();

                // 一度もゲームパッドのボタンを押さずにプレイする事があるので、ここでチェック
                CheckConnectGamePad();

                // 戻るボタンが選択、又はリプレイデータが無いのにリプレイが選択されていた場合には、フリーモードに変更する
                if ((m_goPlay_Select == m_Btn_Play_Back.gameObject) || 
                    (m_goPlay_Select == m_Btn_Play_Replay.gameObject && !m_Btn_Play_Replay.interactable))
                {
                    m_goPlay_Select = m_goPlaySelectList[0];
                }

                SetSelectGameObject(m_goPlay_Select);

                ClearDisableInput();
                m_Timer = 0.0f;
                m_isReceived = false;
                m_isSuccess = false;

                m_MenuPhase = MenuPhase.Play_Update;
                break;

            case MenuPhase.Play_Update:
                InputCtrl();
                break;

            case MenuPhase.Option_Start:
                // 戻るボタンが選択してあった場合には、オプション項目の最初のボタンに戻す
                if (m_goOption_Select == m_goOptionBtns[m_goOptionBtns.Count - 1].gameObject)
                {
                    m_goOption_Select = m_goOptionBtns[0].gameObject;
                }

                SetSelectGameObject(m_goOption_Select);

                ClearDisableInput();
                m_Timer = 0.0f;
                m_isReceived = false;
                m_isSuccess = false;

                m_MenuPhase = MenuPhase.Option_Update;
                break;

            case MenuPhase.Option_Update:
                InputCtrl();
                break;

            case MenuPhase.Account_Start:
                // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                m_AccountEmailStr = m_ReceiveUserInfo.email;
                m_AccountNameStr = m_ReceiveUserInfo.displayName;
                string accountGenderStr = m_ReceiveUserStorageInfo != null ? m_ReceiveUserStorageInfo.gender : "";
                string accountAgeStr = m_ReceiveUserStorageInfo != null ? m_ReceiveUserStorageInfo.age : "";
                string accountWorkStr = m_ReceiveUserStorageInfo != null ? m_ReceiveUserStorageInfo.work : "";
                m_AccountPasswordStr = "";
                m_AccountNowPasswordStr = "";
#else
                m_AccountEmailStr = "";
                m_AccountNameStr = "";
                string accountGenderStr = "";
                string accountAgeStr = "";
                string accountWorkStr = "";
                m_AccountPasswordStr = "";
                m_AccountNowPasswordStr = "";
#endif

                m_InputField_AccountEmail.text = m_AccountEmailStr;
                //m_InputField_AccountName.text = m_AccountNameStr;
                m_InputField_AccountGender.text = accountGenderStr;
                m_InputField_AccountAge.text = accountAgeStr;
                m_InputField_AccountWork.text = accountWorkStr;
                m_InputField_AccountPassword.text = m_AccountPasswordStr;
                m_InputField_AccountNowPassword.text = m_AccountNowPasswordStr;

                SetInputButtonStr(m_InputButton_AccountEmail, m_AccountEmailStr);
                SetInputButtonStr(m_InputButton_AccountName, m_AccountNameStr);
                SetInputButtonStr(m_InputButton_AccountPassword, m_AccountPasswordStr);
                SetInputButtonStr(m_InputButton_AccountNowPassword, m_AccountNowPasswordStr);

                m_Btn_Account_Change.interactable = IsEnableChangeCheck();

                // 戻るボタンが選択、又は変更・保存ボタンが選択されていてかつ非アクティブの場合、メールアドレスのボタンに合わせる
                if ((m_goAccount_Select == m_Btn_Account_Back.gameObject) ||
                    (m_goAccount_Select == m_Btn_Account_Change.gameObject && !IsEnableChangeCheck()))
                {
                    m_goAccount_Select = m_goAccountSelectList[0];
                }

                ClearDisableInput();
                // 戻るボタン有効
                m_isChecking = false;
                // 入力可にする
                m_AccountTouchArea.SetActive(false);

                SetUserName();

                SetStatusText("account settings");

                SetScrollViewContentY(m_trGrpAccount);

                // 削除用のパスワードを保存しておく
                m_DeleteUserPasswordStr = AppData.Password;

                SetSelectGameObject(m_goAccount_Select);

                m_Timer = 0.0f;
                m_isReceived = false;
                m_isSuccess = false;

                m_MenuPhase = MenuPhase.Account_Update;
                break;

            case MenuPhase.Account_Update:
                InputCtrl();
                break;

            case MenuPhase.Account_Proc:
                if (m_isReceived)
                {
                    // ユーザー情報変更成功
                    if (m_isSuccess)
                    {
                        m_Timer = 0.0f;

                        // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                        if (m_isChangedEmail)
                        {
                            m_MenuPhase = MenuPhase.Account_EmailVerified_Start;
                        }
                        else
                        if (!ChangeUserInfo())
                        {
                            // アカウント処理へ（表示内容の更新）
                            ChangeMenuPhase(MenuPhase.Account_Start);
                        }
#else
                        // アカウント処理へ（表示内容の更新）
                        ChangeMenuPhase(MenuPhase.Account_Start);
#endif
                    }
                }
                break;

            case MenuPhase.Account_EmailVerified_Start:
                {
                    ClearDisableInput();

                    // ダイアログタイマー無効、キャンセル付き
                    SetDialog(true, MailConfirmMessage, DialogType.Type3, MenuPhase.Create_Start, MenuPhase.None, -1);

                    m_Timer = 0.0f;
                    m_isCancelFromUser = false;
                    m_isEndChangeEmailVerified = false;

                    // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                    ChangeEmailVerified();
#else
                    // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
                    m_isReceived = true;
                    m_isSuccess = true;
                    m_isEndChangeEmailVerified = true;
                    SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif
                    m_MenuPhase = MenuPhase.Account_EmailVerified_Update;
                }
                break;

            case MenuPhase.Account_EmailVerified_Update:
                if (m_isReceived)
                {
                    // ユーザーによるキャンセル優先
                    if (m_isCancelFromUser)
                    {

                    }
                    else
                    // Eメール確認待ち
                    if (m_isSuccess && m_isEndChangeEmailVerified)
                    {
                        SetDialog(false);

                        m_isChangedEmail = false;

                        // 変更したメールアドレスを保存
                        AppData.SetEmailAddress(m_AccountEmailStr);
                        AppCommon.Update_And_SaveGameData();
                        // ログイン時のメールアドレスも変更しておく
                        m_LoginEmailStr = m_AccountEmailStr;

                        // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                        if (ChangeUserInfo())
                        {
                        }
                        else
#endif
                        {
                            ChangeMenuPhase(MenuPhase.Account_Start);
                        }
                    }
                }
                break;

            case MenuPhase.Logout_Update:
                if (m_isReceived)
                {
                    // 受信したユーザー情報の削除
                    m_ReceiveUserInfo = null;
                    m_ReceiveUserStorageInfo = null;
                    // 受信したリプレイデータ情報の削除
                    AppData.m_ReceiveReplayCountStorageInfo = null;

                    m_CreateEmailStr = "";
                    m_CreateNameStr = "";
                    m_CreateGenderStr = "";
                    m_CreateAgeStr = "";
                    m_CreateWorkStr = "";
                    m_CreatePasswordStr = "";

                    // ユーザー名表示 ※非表示になる
                    SetUserName();

                    // 成否に関わらずスタート画面に戻す
                    ChangeMenuPhase(MenuPhase.Start_Start);
                }
                break;

            case MenuPhase.Dialog_Update:
                InputCtrl();
                // 確認待ち、等の無限待ち
                if (m_Timer < 0.0f)
                {
                }
                else
                if (m_Timer > 0.0f)
                {
                    m_Timer -= Time.deltaTime;
                    if (m_Timer <= 0.0f)
                    {
                        SetDialog(false);
                        m_Timer = 0.0f;
                        m_MenuPhase = m_ErrorMenuPhase != MenuPhase.None ? m_ErrorMenuPhase : m_ReturnMenuPhase;
                    }
                }
                break;

            case MenuPhase.Dialog_Keyboard:
                {
                    if (Input.anyKeyDown)
                    {
                        KeyCode newCode = KeyCode.None;
                        foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
                        {
                            if (Input.GetKeyDown(code))
                            {
                                newCode = code;
                                Debug.Log(code);
                                break;
                            }
                            if (code == KeyCode.Menu) break;
                        }
                        if (newCode != KeyCode.None)
                        {
                            SetDialog(false);
                            ClearDisableInput();

                            AppData.SetKeyCode((AppData.Action)m_SelectedKeyboardIndex, newCode);
                            SetKeyboardBtnsText();

                            m_MenuPhase = MenuPhase.Option_Update;
                        }
                    }
                }
                break;

            case MenuPhase.Dialog_Gamepad:
                if (!YnInputSystem.IsRebinding)
                {
                    SetDialog(false);

                    // ゲームパッドバインド情報のセーブ
                    YnInputSystem.Save();
                    SetGamepadBtnsText();

                    // Action.Enable() 直後は謎の入力が入っていたりするので、少しウェイトを入れる
                    m_Timer = 0.0f;
                    m_MenuPhase = MenuPhase.Dialog_Delay;
                }
                break;

            case MenuPhase.Dialog_Delay:
                m_Timer += Time.deltaTime;
                if (m_Timer > 0.5f)
                {
                    m_Timer = 0.0f;
                    ClearDisableInput();
                    m_MenuPhase = MenuPhase.Option_Update;
                }
                break;

            case MenuPhase.Replay_Send_Start:
                m_Stopwatch.Restart();
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                CreateReplayInfoStorage(0);
#else
                m_isReceived = true;
                m_isSuccess = true;
#endif
                m_MenuPhase = MenuPhase.Replay_Send_Update;
                break;
            case MenuPhase.Replay_Send_Update:
                if (m_isReceived)
                {
                    if (m_isSuccess)
                    {
                        m_MenuPhase = MenuPhase.Replay_Get_Start;
                        Debug.Log("アップロード経過時間:" + m_Stopwatch.Elapsed);
                        m_Stopwatch.Stop();
                    }
                }
                break;
            case MenuPhase.Replay_Get_Start:
                m_Stopwatch.Restart();
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                GetReplayInfoStorage(0);
#else
                m_isReceived = true;
                m_isSuccess = true;
#endif
                m_MenuPhase = MenuPhase.Replay_Get_Update;
                break;
            case MenuPhase.Replay_Get_Update:
                if (m_isReceived)
                {
                    if (m_isSuccess)
                    {
                        ChangeMenuPhase(MenuPhase.Loggedin_Start);
                        Debug.Log("ダウンロード経過時間:" + m_Stopwatch.Elapsed);
                        m_Stopwatch.Stop();
                    }
                }
                break;


            // ユーザー情報（firebase.storage）の削除
            case MenuPhase.Error_DeleteUserInfo_Start:
                m_isReceived = false;
                m_isSuccess = false;

                DeleteUserInfoStorage();

                m_MenuPhase = MenuPhase.Error_DeleteUserInfo_Update;
                break;

            case MenuPhase.Error_DeleteUserInfo_Update:
                if (m_isReceived)
                {
                    // 成否に関わらず、続けてユーザー情報（firebase.auth）を削除する
                    m_MenuPhase = MenuPhase.Error_DeleteUser_Start;
                }
                break;

            // ユーザー情報（firebase.auth）の削除
            case MenuPhase.Error_DeleteUser_Start:
                m_isReceived = false;
                m_isSuccess = false;

                DeleteUser();

                m_MenuPhase = MenuPhase.Error_DeleteUser_Update;
                break;

            case MenuPhase.Error_DeleteUser_Update:
                if (m_isReceived)
                {
                    SetUserName();
                    // セーブデ－タ初期化
                    AppData.Dbg_SaveDataReset();
                    // 保存
                    AppCommon.Update_And_SaveGameData();
                    // 受信したリプレイデータ情報の削除
                    AppData.m_ReceiveReplayCountStorageInfo = null;

                    m_CreateEmailStr = "";
                    m_CreateNameStr = "";
                    m_CreateGenderStr = "";
                    m_CreateAgeStr = "";
                    m_CreateWorkStr = "";
                    m_CreatePasswordStr = "";
                    m_LoginEmailStr = "";
                    m_LoginPasswordStr = "";
                    // 成否に関わらず、指定されたフェーズに戻る
                    ChangeMenuPhase(m_ReturnMenuPhase);
                }
                break;

            case MenuPhase.Game_Start:
                // 一度もゲームパッドのボタンを押さずにプレイする事があるので、ここでチェック
                CheckConnectGamePad();

                // // 戻るボタンが選択、又はリプレイデータが無いのにリプレイが選択されていた場合には、フリーモードに変更する
                // if ((m_goPlay_Select == m_Btn_Play_Back.gameObject) ||
                //     (m_goPlay_Select == m_Btn_Play_Replay.gameObject && !m_Btn_Play_Replay.interactable))
                // {
                //     m_goPlay_Select = m_goPlaySelectList[0];
                // }

                SetSelectGameObject(m_goGamemode_Select);

                ClearDisableInput();
                m_Timer = 0.0f;
                m_isReceived = false;
                m_isSuccess = false;

                m_MenuPhase = MenuPhase.Game_Update;
                break;

            case MenuPhase.Game_Update:
                InputCtrl();
                break;

            case MenuPhase.Game_TransGame1:
                {
                    m_Timer -= Time.deltaTime;
                    if (m_Timer <= 0.0f)
                    {
                        m_Timer = 0.0f;
                        // StartCoroutine("ProcFadeOut");
                        m_MenuPhase = MenuPhase.Game_TransGame2;
                    }
                }
                break;

            case MenuPhase.Game_TransGame2:
                m_Timer += Time.deltaTime;
                if (m_Timer > 0.5f)
                {
                    // @正式版
                    TransitionLoad_Game();
                    m_MenuPhase = MenuPhase.None;
                }
                break;


            case MenuPhase.Update:
                {
                    m_Timer -= Time.deltaTime;
                    if (m_Timer <= 0.0f)
                    {
                        m_Timer = 0.0f;
                        // StartCoroutine("ProcFadeOut");
                        m_MenuPhase = MenuPhase.End;
                    }
                }
                break;

            case MenuPhase.End:
                m_Timer += Time.deltaTime;
                if (m_Timer > 0.5f)
                {
                    // @正式版
                    TransitionLoad();
                    m_MenuPhase = MenuPhase.None;
                }
                break;

        }
    }

    private void LateUpdate()
    {
        // パッド入力情報初期化
        AppData.ResetPadTrgs();
    }

    public void TransitionLoad()
    {
        m_MenuPhase = MenuPhase.None;
        ShTransitionSys.SetYnsysLoadNextScene("MainGame", 0.25f);
        ShTransitionSys.SetLoadScene(false);
        ShTransitionSys.ChangeCoverColorBlack();

        ShTransitionSys.SetTransition(0.5f, this, "ProcTransitionLoad", false, false, false);
    }

    private IEnumerator ProcTransitionLoad()
    {
        ChangeScene_MainGame.SetChangeScene();

        // 破棄
        yield break;
    }

    public void TransitionLoad_Game()
    {
        m_MenuPhase = MenuPhase.None;
        ShTransitionSys.SetYnsysLoadNextScene("MainGame", 0.25f);
        ShTransitionSys.SetLoadScene(false);
        ShTransitionSys.ChangeCoverColorBlack();

        ShTransitionSys.SetTransition(0.5f, this, "ProcTransitionLoad_Game", false, false, false);
    }

    private IEnumerator ProcTransitionLoad_Game()
    {
        if (AppData.m_PlayMode == AppData.PlayMode.Game_Endress)
        {
            ChangeScene_Endress.SetChangeScene();
        }
        else if(AppData.m_PlayMode == AppData.PlayMode.Game_Irairabou)
        {
            ChangeScene_StickChallenge.SetChangeScene();
        }
        else
        {
            ChangeScene_Treasure.SetChangeScene();
        }

        // 破棄
        yield break;
    }


    private Transform GetTransform(GameObject goBaseCanvas, string name)
    {
        string findStr = m_isMobilePlatform ? name + "M" : name;
        return goBaseCanvas.transform.Find(findStr);
    }

    private void ChangeMenuPhase(MenuPhase phase)
    {
        m_trGrpStart.gameObject.SetActive(false);
        m_trGrpCreate.gameObject.SetActive(false);
        m_trGrpLogin.gameObject.SetActive(false);
        m_trGrpLoggedin.gameObject.SetActive(false);
        m_trGrpPlay.gameObject.SetActive(false);
        m_trGrpOption.gameObject.SetActive(false);
        m_trGrpAccount.gameObject.SetActive(false);
        m_trGrpGame.gameObject.SetActive(false);
        switch (phase)
        {
            // スタート画面
            case MenuPhase.Start_Start:
                m_trGrpStart.gameObject.SetActive(true);
                break;
            // アカウント作成画面
            case MenuPhase.Create_Start:
                m_trGrpStart.gameObject.SetActive(true);
                m_trGrpCreate.gameObject.SetActive(true);
                m_Btn_Create_Check.interactable = IsEnableCreateCheck();
                break;
            // ログイン画面
            case MenuPhase.Login_Start:
                m_trGrpStart.gameObject.SetActive(true);
                m_trGrpLogin.gameObject.SetActive(true);
                break;
            // ログイン中画面
            case MenuPhase.Loggedin_Start:
                m_trGrpLoggedin.gameObject.SetActive(true);
                break;
            // プレイモード選択画面
            case MenuPhase.Play_Start:
                m_trGrpLoggedin.gameObject.SetActive(true);
                m_trGrpPlay.gameObject.SetActive(true);
                m_Btn_Play_Replay.interactable = isEnableReplayData();
                break;
            // オプション画面
            case MenuPhase.Option_Start:
                m_trGrpLoggedin.gameObject.SetActive(true);
                m_trGrpOption.gameObject.SetActive(true);
                break;
            // アカウント画面
            case MenuPhase.Account_Start:
                m_trGrpLoggedin.gameObject.SetActive(true);
                m_trGrpAccount.gameObject.SetActive(true);
                break;
            // ゲームモード選択画面
            case MenuPhase.Game_Start:
                m_trGrpGame.gameObject.SetActive(true);
                break;
        }
        m_MenuPhase = phase;
    }

    private void SetDialog(
        bool isEnable,
        string dispStr = "",
        DialogType type = DialogType.Type1,
        MenuPhase returnMenuPhase = MenuPhase.None,
        MenuPhase errorMenuPhase = MenuPhase.None,
        float wait = 2.0f)
    {
        SetConfirmDialog(isEnable, dispStr, type, returnMenuPhase, MenuPhase.None, MenuPhase.None, errorMenuPhase, wait);
    }

    private void SetConfirmDialog(
        bool isEnable,
        string dispStr = "",
        DialogType type = DialogType.Type4,
        MenuPhase returnMenuPhase = MenuPhase.None,
        MenuPhase confirmYMenuPhase = MenuPhase.None,
        MenuPhase confirmNMenuPhase = MenuPhase.None,
        MenuPhase errorMenuPhase = MenuPhase.None,
        float wait = -1)
    {
        if (!isEnable)
        {
            m_trGrpDialog.gameObject.SetActive(false);
            return;
        }

        string[] _txtTbl = { "Text_Val" };
        List<TextMeshProUGUI> txtList = new List<TextMeshProUGUI>(_txtTbl.Length);

        foreach (string txtStr in _txtTbl)
        {
            txtList.Add(m_trGrpDialog.Find(txtStr).GetComponent<TextMeshProUGUI>());
            txtList[txtList.Count - 1].gameObject.SetActive(false);
        }

        m_Timer = wait;
        m_DialogType = type;

        switch (m_DialogType)
        {
            case DialogType.Type1:
            case DialogType.Type2:
                txtList[0].text = dispStr;
                txtList[0].gameObject.SetActive(true);
                m_Btn_Dialog_Cancel.gameObject.SetActive(false);
                m_DialogConfirm.SetActive(false);
                break;
            case DialogType.Type3:
                txtList[0].text = dispStr;
                txtList[0].gameObject.SetActive(true);
                m_Btn_Dialog_Cancel.gameObject.SetActive(true);
                m_DialogConfirm.SetActive(false);
                m_goDialog_Select = m_Btn_Dialog_Cancel.gameObject;
                StartCoroutine(crDialogSelectDelayDisp());
                break;
            case DialogType.Type4:
            case DialogType.Type5:
            case DialogType.Type6:
            case DialogType.Type7:
                txtList[0].text = dispStr;
                txtList[0].gameObject.SetActive(true);
                m_Btn_Dialog_Cancel.gameObject.SetActive(false);
                m_DialogConfirm.SetActive(true);
                m_goDialog_Select = m_Btn_Dialog_ConfirmN.gameObject;
                StartCoroutine(crDialogSelectDelayDisp());
                break;
        }

        m_trGrpDialog.gameObject.SetActive(true);

        m_ReturnMenuPhase = returnMenuPhase;
        m_ConfirmYMenuPhase = confirmYMenuPhase;
        m_ConfirmNMenuPhase = confirmNMenuPhase;
        m_ErrorMenuPhase = errorMenuPhase;
        if (!(returnMenuPhase == MenuPhase.None && errorMenuPhase == MenuPhase.None))
        {
            m_MenuPhase = MenuPhase.Dialog_Update;
        }
    }

    private IEnumerator crDialogSelectDelayDisp()
    {
        yield return null;
        SetSelectGameObject(m_goDialog_Select);
    }

    private void SetUserName()
    {
        if (m_ReceiveUserInfo != null)
        {
            m_UserNameText.text = UserNameHeader + m_ReceiveUserInfo.displayName;
        }
        else
        {
            m_UserNameText.text = "";
        }
    }

    private string GetInputButtonStr(CustomInputButton btn)
    {
        TextMeshProUGUI text = btn.transform.GetComponentInChildren<TextMeshProUGUI>();
        return text.text;
    }

    private void SetInputButtonStr(CustomInputButton btn, string str)
    {
        TextMeshProUGUI text = btn.transform.GetComponentInChildren<TextMeshProUGUI>();
        text.text = str;
    }

    private void SetInputButtonStrPassword(CustomInputButton btn, string str)
    {
        TextMeshProUGUI text = btn.transform.GetComponentInChildren<TextMeshProUGUI>();
        if (!string.IsNullOrEmpty(str))
        {
            char[] chr = str.ToCharArray();
            for (int i = 0; i < chr.Length; ++i) chr[i] = '*';
            str = chr.ArrayToString();
        }
        text.text = str;
    }

    private void SetScrollViewContentY(Transform baseTr, float val = 0.0f)
    {
        Transform contentTr = baseTr.Find("Scroll View/Viewport/Content");
        if (contentTr)
        {
            RectTransform rt = contentTr.GetComponent<RectTransform>();
            if (rt)
            {
                Vector2 pos = rt.anchoredPosition;
                pos.y = val;
                rt.anchoredPosition = pos;
            }
        }
    }

    /// <summary>
    /// ログイン中のユーザー情報監視
    /// </summary>
    private void ReloadUserInfo()
    {
        bool isProc = false;
        switch (m_MenuPhase)
        {
            // 指定フェーズ中のみ監視
            case MenuPhase.Loggedin_Start:
            case MenuPhase.Loggedin_Update:
            case MenuPhase.Play_Start:
            case MenuPhase.Play_Update:
            case MenuPhase.Option_Start:
            case MenuPhase.Option_Update:
            case MenuPhase.Account_Start:
            case MenuPhase.Account_Update:
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                isProc = true;
#endif
                break;
            default:
                m_ReloadTimer = 0.0f;
                m_ReloadPhase = ReloadPhase.Start;
                break;
        }
        if (isProc)
        {
            switch (m_ReloadPhase)
            {
                case ReloadPhase.Start:
                    m_ReloadTimer += Time.deltaTime;
                    if (m_ReloadTimer > ReloadUserInfoWait)
                    {
                        GetCurrentUserReload();
                        m_ReloadPhase = ReloadPhase.Confirm;
                    }
                    break;
                case ReloadPhase.Confirm:
                    if (m_isReloadReceived)
                    {
                        // ユーザートークンが期限切れの場合はダイアログ表示後にログアウトさせる
                        if (m_isReloadExpired)
                        {
                            SignOut();
                            SetDialog(true, ReloadExpireTokenMessage, DialogType.Type2, MenuPhase.Logout_Update);
                        }
                        m_ReloadTimer = 0.0f;
                        m_ReloadPhase = ReloadPhase.Start;
                    }
                    break;
            }
        }
    }

    private void ReplayData()
    {
        bool isProc = false;
        switch (m_MenuPhase)
        {
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
            // 指定フェーズ中のみ
            case MenuPhase.Start_Start:
            case MenuPhase.Start_Update:
            case MenuPhase.Start_SendReplay_Update:
            case MenuPhase.Create_Start:
            case MenuPhase.Create_Update:
            case MenuPhase.Loggedin_Start:
            case MenuPhase.Loggedin_Update:
            case MenuPhase.Play_Start:
            case MenuPhase.Play_Update:
            case MenuPhase.Option_Start:
            case MenuPhase.Option_Update:
            case MenuPhase.Account_Start:
            case MenuPhase.Account_Update:
            case MenuPhase.Game_Start:
            case MenuPhase.Game_Update:
                isProc = true;
        break;
#endif
            default:
                m_ReplayPhase = ReplayPhase.Wait;
                m_isGetReplayData = false;
                m_isSendReplayData = false;
                m_ReplayIndex = -1;
                break;
        }
        if (isProc)
        {
            switch (m_ReplayPhase)
            {
                case ReplayPhase.Wait:
                    if (m_isSendReplayData)
                    {
                        m_ReplayPhase = ReplayPhase.Send_Start;
                    }
                    else
                    if (m_isGetReplayData)
                    {
                        m_ReplayPhase = ReplayPhase.Get_Start;
                    }
                    break;
                case ReplayPhase.Send_Start:
                    if (m_isSendReplayData)
                    {
                        if (AppData.m_ReceiveReplayCountStorageInfo != null)
                        {
                            for (int i = 0; i < AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count; ++i)
                            {
                                if (AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_isUsed && !AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_isDownloaded)
                                {
                                    m_ReplayIndex = i;

                                    m_ReplayStr = JsonUtility.ToJson(AppData.GetReplayData(i)/*, true*/);

                                    CreateReplayInfoStorage(AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_FileIndex);
                                    m_ReplayPhase = ReplayPhase.Send_Update;
                                    break;
                                }
                            }
                            // 全てのリプレイデータを送信済み
                            if (m_ReplayPhase != ReplayPhase.Send_Update)
                            {
                                Debug.Log("--- all replay data uploaded ---");
                                m_isSendReplayData = false;
                            }
                        }
                        else
                        {
                            m_isSendReplayData = false;
                        }
                    }
                    break;
                case ReplayPhase.Send_Update:
                    if (m_isReplayReceived)
                    {
                        if (m_ReplayIndex >= 0 && AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count > m_ReplayIndex)
                        {
                            AppData.m_ReceiveReplayCountStorageInfo.m_Infos[m_ReplayIndex].m_isDownloaded = true;
                        }
                        m_ReplayPhase = ReplayPhase.Wait;
                    }
                    break;
                case ReplayPhase.Get_Start:
                    if (m_isGetReplayData)
                    {
                        if (AppData.m_ReceiveReplayCountStorageInfo != null)
                        {
                            for (int i = 0; i < AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count; ++i)
                            {
                                if (AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_isUsed && !AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_isDownloaded)
                                {
                                    m_ReplayIndex = i;
                                    GetReplayInfoStorage(AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_FileIndex);
                                    m_ReplayPhase = ReplayPhase.Get_Update;
                                    break;
                                }
                            }
                            // 全てのリプレイデータがDL済み
                            if (m_ReplayPhase != ReplayPhase.Get_Update)
                            {
                                Debug.Log("--- all replay data downloaded ---");
                                m_isGetReplayData = false;
                            }
                        }
                        else
                        {
                            m_isGetReplayData = false;
                        }
                    }
                    break;
                case ReplayPhase.Get_Update:
                    if (m_isReplayReceived)
                    {
                        if (m_ReplayIndex >= 0 && AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count > m_ReplayIndex)
                        {
                            AppData.m_ReceiveReplayCountStorageInfo.m_Infos[m_ReplayIndex].m_isDownloaded = true;
                        }
                        m_ReplayPhase = ReplayPhase.Wait;
                    }
                    break;
            }
        }
    }

    private bool isGetReplayData()
    {
        bool res = true;
        if (AppData.m_ReceiveReplayCountStorageInfo != null)
        {
            for (int i = 0; i < AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count; ++i)
            {
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                if (!AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_isDownloaded)
                {
                    res = false;
                    break;
                }
#endif
            }
        }
        return res;
    }

    private bool isSendReplayData()
    {
        bool res = true;
        if (AppData.m_ReceiveReplayCountStorageInfo != null)
        {
            for (int i = 0; i < AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count; ++i)
            {
                if (!AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_isDownloaded)
                {
                    res = false;
                    break;
                }
            }
        }
        return res;
    }

    private void SetSelectGameObject(GameObject go)
    {
        //if (!m_isMobilePlatform)
        {
            if (EventSystem.current.currentSelectedGameObject != go)
            {
                EventSystem.current.SetSelectedGameObject(go);
            }
        }
    }

    private GameObject GetInputGameObject(
        GameObject[] tbl,
        GameObject goSelect,
        bool isUp,
        bool isDown,
        bool isLeft = false,
        bool isRight = false,
        InputType inputType = InputType.Type1,
        GameObject content = null,
        ContentType contentType = ContentType.Type1)
    {
        GameObject go = null;
        int index = -1;
        for (int i = 0; i < tbl.Length; ++i)
        {
            if (tbl[i] == goSelect)
            {
                index = i;
                break;
            }
        }
        if (index >= 0)
        {
            bool isSet = false;
            bool isNegative = false;
            if (inputType == InputType.Type1)
            {
                isSet = isUp || isDown;
                isNegative = isUp;
            }
            else
            if (inputType == InputType.Type2)
            {
                isSet = isLeft || isRight;
                isNegative = isLeft;
            }
            if (isSet)
            {
                if (isNegative)
                {
                    if (--index < 0) index = tbl.Length - 1;
                    Button btn = tbl[index].GetComponent<Button>();
                    if (btn != null)
                    {
                        if (!btn.interactable)
                        {
                            if (--index < 0) index = tbl.Length - 1;
                        }
                    }
                }
                else
                {
                    if (++index >= tbl.Length) index = 0;
                    Button btn = tbl[index].GetComponent<Button>();
                    if (btn != null)
                    {
                        if (!btn.interactable)
                        {
                            if (++index >= tbl.Length) index = 0;
                        }
                    }
                }
                go = tbl[index];
                if (content)
                {
                    SetContentPosition(content, contentType, index);
                }
            }
            else
            {
                go = tbl[index];
            }
        }
        else
        {
            go = goSelect;
        }
        return go;
    }

    private void SetContentPosition(GameObject goContent, ContentType contentType, int index)
    {
        RectTransform rt = goContent.GetComponent<RectTransform>();
        if (rt)
        {
            switch (contentType)
            {
                // 職業選択
                case ContentType.Type1:
                    if (index <= 12)
                    {
                        Vector2 pos = rt.anchoredPosition;
                        pos.y = 500.0f * (float)index / 12.0f;
                        rt.anchoredPosition = pos;
                    }
                    break;
                // キーボード設定
                case ContentType.Type2:
                    if (index <= 17)
                    {
                        Vector2 pos = rt.anchoredPosition;
                        pos.y = 640.0f * (float)index / 17.0f;
                        rt.anchoredPosition = pos;
                    }
                    break;
                // ゲームパッド設定
                case ContentType.Type3:
                    if (index <= 13)
                    {
                        Vector2 pos = rt.anchoredPosition;
                        pos.y = 320.0f * (float)index / 13.0f;
                        rt.anchoredPosition = pos;
                    }
                    break;
                // 年代選択（モバイル）
                case ContentType.Type4:
                    if (index <= 8)
                    {
                        Vector2 pos = rt.anchoredPosition;
                        pos.y = 270.0f * (float)index / 8.0f;
                        rt.anchoredPosition = pos;
                    }
                    break;
                // 職業選択（モバイル）
                case ContentType.Type5:
                    if (index <= 12)
                    {
                        Vector2 pos = rt.anchoredPosition;
                        pos.y = 760.0f * (float)index / 12.0f;
                        rt.anchoredPosition = pos;
                    }
                    break;
                // アカウント選択（モバイル）
                case ContentType.Type6:
                    if (index <= 6)
                    {
                        Vector2 pos = rt.anchoredPosition;
                        pos.y = 500.0f * (float)index / 6.0f;
                        rt.anchoredPosition = pos;
                    }
                    break;
                // オプション設定（モバイル）
                case ContentType.Type7:
                    if (index <= 6)
                    {
                        Vector2 pos = rt.anchoredPosition;
                        pos.y = 360.0f * (float)index / 6.0f;
                        rt.anchoredPosition = pos;
                    }
                    break;
                // キーボード設定（モバイル）
                case ContentType.Type8:
                    if (index <= 17)
                    {
                        Vector2 pos = rt.anchoredPosition;
                        pos.y = 1000.0f * (float)index / 17.0f;
                        rt.anchoredPosition = pos;
                    }
                    break;
                // ゲームパッド設定（モバイル）
                case ContentType.Type9:
                    if (index <= 13)
                    {
                        Vector2 pos = rt.anchoredPosition;
                        pos.y = 600.0f * (float)index / 13.0f;
                        rt.anchoredPosition = pos;
                    }
                    break;
                // リプレイ選択
                case ContentType.Type10:
                // リプレイ選択（モバイル）
                case ContentType.Type11:
                    {
                        int max = AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count - 1;
                        if (max < 0) max = 0;
                        float height = max <= 5 ? 0.0f : (max - 5) * 125;
                        if (index <= max)
                        {
                            Vector2 pos = rt.anchoredPosition;
                            if (max > 0)
                            {
                                pos.y = height * (float)index / (float)max;
                            }
                            else
                            {
                                pos.y = 0;
                            }
                            rt.anchoredPosition = pos;
                        }
                    }
                    break;
            }
        }
    }

    private GameObject[] CnvButtonToGameObject(List<Button> list)
    {
        int count = list.Count;
        GameObject[] res = new GameObject[count];
        for (int i = 0; i < count; ++i)
        {
            res[i] = list[i].gameObject;
        }
        return res;
    }

    private void InputCtrl()
    {
        // モバイルブラウザで縦画面の時は入力無効にする
        if (m_isMobilePlatform)
        {
            if (YnSys.m_IsVerticalScreen) return;
        }
        // 入力無効状態
        if (m_isDisableInputAll) return;
        // リプレイの読み込み中
        if (m_trGrpPlay.Find("Alert").gameObject.activeSelf) return;
        // InputFieldの入力受付中
        if (m_isEnableInputField)
        {
            // GamePadの決定を押すと、InputFieldの入力終了扱いにする
            if (AppData.GetPadTrg(AppData.Action.Decide))
            {
                switch (m_MenuPhase)
                {
                    case MenuPhase.Login_Update:
                        if (m_goLogin_Select == m_InputField_LoginEmail.gameObject)
                        {
                            m_InputField_LoginEmail.DeactivateInputField();
                        }
                        if (m_goLogin_Select == m_InputField_LoginPassword.gameObject)
                        {
                            m_InputField_LoginPassword.DeactivateInputField();
                        }
                        break;
                    case MenuPhase.Create_Update:
                        if (m_goCreate_Select == m_InputField_CreateEmail.gameObject)
                        {
                            m_InputField_CreateEmail.DeactivateInputField();
                        }
                        if (m_goCreate_Select == m_InputField_CreatePassword.gameObject)
                        {
                            m_InputField_CreatePassword.DeactivateInputField();
                        }
                        break;
                    case MenuPhase.Account_Update:
                        if (m_goAccount_Select == m_InputField_AccountEmail.gameObject)
                        {
                            m_InputField_AccountEmail.DeactivateInputField();
                        }
                        if (m_goAccount_Select == m_InputField_AccountPassword.gameObject)
                        {
                            m_InputField_AccountPassword.DeactivateInputField();
                        }
                        if (m_goAccount_Select == m_InputField_AccountNowPassword.gameObject)
                        {
                            m_InputField_AccountNowPassword.DeactivateInputField();
                        }
                        break;
                }
            }
            return;
        }

        bool isDecide = AppData.GetTrg(AppData.Action.Decide);
        bool isCancel = AppData.GetTrg(AppData.Action.Cancel);
        bool isUp = AppData.GetTrg(AppData.Action.LUp) || AppData.GetTrg(AppData.Action.RUp);
        bool isDown = AppData.GetTrg(AppData.Action.LDown) || AppData.GetTrg(AppData.Action.RDown);
        bool isLeft = AppData.GetTrg(AppData.Action.LLeft) || AppData.GetTrg(AppData.Action.RLeft);
        bool isRight = AppData.GetTrg(AppData.Action.LRight) || AppData.GetTrg(AppData.Action.RRight);

        GameObject go = null;

        if (isDecide)
        {
            switch (m_MenuPhase)
            {
                case MenuPhase.Start_Update:
                    go = m_goStart_Select;
                    break;
                case MenuPhase.Create_Update:
                    if (m_trCreateGender.gameObject.activeSelf)
                    {
                        go = m_goCreate_Gender;
                        SetSelectGameObject(m_goCreate_Select);
                    }
                    else
                    if (m_trCreateAge.gameObject.activeSelf)
                    {
                        go = m_goCreate_Age;
                        SetSelectGameObject(m_goCreate_Select);
                    }
                    else
                    if (m_trCreateWork.gameObject.activeSelf)
                    {
                        go = m_goCreate_Work;
                        SetSelectGameObject(m_goCreate_Select);
                    }
                    else
                    {
                        go = m_goCreate_Select;
                    }
                    break;
                case MenuPhase.Login_Update:
                    go = m_goLogin_Select;
                    break;
                case MenuPhase.Loggedin_Update:
                    go = m_goLoggedin_Select;
                    break;
                case MenuPhase.Play_Update:
                    if (m_trPlayReplaySelect.gameObject.activeSelf)
                    {
                        go = m_goPlay_Replay;
                    }
                    else if (m_trPlayMissionSelect.gameObject.activeSelf)
                    {
                        go = m_goPlay_Mission;
                    }
                    else
                    {
                        go = m_goPlay_Select;
                    }
                    break;
                case MenuPhase.Option_Update:
                    if (m_trOptionGamepad.gameObject.activeSelf)
                    {
                        go = m_goOption_Gamepad;
                    }
                    else
                    if (m_trOptionKeyboard.gameObject.activeSelf)
                    {
                        go = m_goOption_Keyboard;
                    }
                    else
                    {
                        go = m_goOption_Select;
                    }
                    break;
                case MenuPhase.Account_Update:
                    go = m_goAccount_Select;
                    break;
                case MenuPhase.Dialog_Update:
                    if (m_DialogType >= DialogType.Type4 && m_DialogType <= DialogType.Type7)
                    {
                        go = m_goDialog_Select;
                    }
                    break;
                case MenuPhase.Game_Update:
                    if (m_goGamemode_Select == m_Btn_Gamemode_Iraira.gameObject)
                    {
                        go = null;
                    }
                    else
                    {
                        go = m_goGamemode_Select;
                    }
                    break;
            }
            if (go != null)
            {
                // 該当ボタンを実行する
                ExecuteEvents.Execute(target: go,
                                      eventData: new PointerEventData(EventSystem.current),
                                      functor: ExecuteEvents.pointerClickHandler);

                if (CheckDesideSe(go))
                {
                    // SE:
                    MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
                }
            }
        }
        else
        if (isCancel)
        {
            switch (m_MenuPhase)
            {
                case MenuPhase.Create_Update:
                    if (m_trCreateGender.gameObject.activeSelf)
                    {
                        go = m_CreateGenderBtns[m_CreateGenderBtns.Count - 1].gameObject;
                        SetSelectGameObject(m_goCreate_Select);
                    }
                    else
                    if (m_trCreateAge.gameObject.activeSelf)
                    {
                        go = m_CreateAgeBtns[m_CreateAgeBtns.Count - 1].gameObject;
                        SetSelectGameObject(m_goCreate_Select);
                    }
                    else
                    if (m_trCreateWork.gameObject.activeSelf)
                    {
                        go = m_CreateWorkBtns[m_CreateWorkBtns.Count - 1].gameObject;
                        SetSelectGameObject(m_goCreate_Select);
                    }
                    else
                    {
                        go = m_Btn_Create_Back.gameObject;
                    }
                    break;
                case MenuPhase.Login_Update:
                    go = m_Btn_Login_Back.gameObject;
                    break;
                case MenuPhase.Play_Update:
                    if (m_trPlayReplaySelect.gameObject.activeSelf)
                    {
                        go = m_Btn_ReplaySelect_Back.gameObject;
                        SetSelectGameObject(m_goPlay_Select);
                    }
                    else
                    if (m_trPlayMissionSelect.gameObject.activeSelf)
                    {
                        go = m_Btn_MissionSelect_Back.gameObject;
                        SetSelectGameObject(m_goPlay_Select);
                    }
                    else
                    {
                        go = m_Btn_Play_Back.gameObject;
                    }
                    break;
                case MenuPhase.Option_Update:
                    if (m_trOptionGamepad.gameObject.activeSelf)
                    {
                        go = m_OptionGamepadBtns[m_OptionGamepadBtns.Count - 1].gameObject;
                    }
                    else
                    if (m_trOptionKeyboard.gameObject.activeSelf)
                    {
                        go = m_OptionKeyboardBtns[m_OptionKeyboardBtns.Count - 1].gameObject;
                    }
                    else
                    {
                        go = m_goOptionBtns[m_goOptionBtns.Count - 1].gameObject;
                    }
                    break;
                case MenuPhase.Account_Update:
                    go = m_Btn_Account_Back.gameObject;
                    break;
                case MenuPhase.Dialog_Update:
                    if (m_DialogType == DialogType.Type3)
                    {
                        go = m_Btn_Dialog_Cancel.gameObject;
                    }
                    else
                    if (m_DialogType >= DialogType.Type4 && m_DialogType <= DialogType.Type7)
                    {
                        go = m_Btn_Dialog_ConfirmN.gameObject;
                    }
                    break;
                case MenuPhase.Game_Update:
                    go = m_Btn_Gamemode_Back.gameObject;
                    break;
            }
            if (go != null)
            {
                // 該当ボタンを実行する
                ExecuteEvents.Execute(target: go,
                                      eventData: new PointerEventData(EventSystem.current),
                                      functor: ExecuteEvents.pointerClickHandler);
                // SE:
                MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
            }
        }
        else
        if (isUp || isDown || isLeft || isRight)
        {
            switch (m_MenuPhase)
            {
                case MenuPhase.Start_Update:
                    {
                        m_goStart_Select = GetInputGameObject(m_goStartSelectList.ToArray(), m_goStart_Select, isUp, isDown);
                        go = m_goStart_Select;
                    }
                    break;
                case MenuPhase.Create_Update:
                    if (m_trCreateGender.gameObject.activeSelf)
                    {
                        GameObject[] _tbl = CnvButtonToGameObject(m_CreateGenderBtns);
                        m_goCreate_Gender = GetInputGameObject(_tbl, m_goCreate_Gender, isUp, isDown);
                        go = m_goCreate_Gender;
                    }
                    else
                    if (m_trCreateAge.gameObject.activeSelf)
                    {
                        GameObject[] _tbl = CnvButtonToGameObject(m_CreateAgeBtns);
                        if (m_isMobilePlatform)
                        {
                            GameObject goContent = m_trCreateAge.Find("Scroll View/Viewport/Content").gameObject;
                            m_goCreate_Age = GetInputGameObject(_tbl, m_goCreate_Age, isUp, isDown, false, false, InputType.Type1, goContent, ContentType.Type4);
                        }
                        else
                        {
                            m_goCreate_Age = GetInputGameObject(_tbl, m_goCreate_Age, isUp, isDown);
                        }
                        go = m_goCreate_Age;
                    }
                    else
                    if (m_trCreateWork.gameObject.activeSelf)
                    {
                        GameObject[] _tbl = CnvButtonToGameObject(m_CreateWorkBtns);
                        GameObject goContent = m_trCreateWork.Find("Scroll View/Viewport/Content").gameObject;
                        ContentType contentType = m_isMobilePlatform ? ContentType.Type5 : ContentType.Type1;
                        m_goCreate_Work = GetInputGameObject(_tbl, m_goCreate_Work, isUp, isDown, false, false, InputType.Type1, goContent, contentType);
                        go = m_goCreate_Work;
                    }
                    else
                    {
                        m_goCreate_Select = GetInputGameObject(m_goCreateSelectList.ToArray(), m_goCreate_Select, isUp, isDown);
                        go = m_goCreate_Select;
                    }
                    break;
                case MenuPhase.Login_Update:
                    {
                        m_goLogin_Select = GetInputGameObject(m_goLoginSelectList.ToArray(), m_goLogin_Select, isUp, isDown);
                        go = m_goLogin_Select;
                    }
                    break;
                case MenuPhase.Loggedin_Update:
                    {
                        m_goLoggedin_Select = GetInputGameObject(m_goLoggedinSelectList.ToArray(), m_goLoggedin_Select, isUp, isDown);
                        go = m_goLoggedin_Select;
                    }
                    break;
                case MenuPhase.Play_Update:
                    if (m_trPlayReplaySelect.gameObject.activeSelf)
                    {
                        GameObject[] _tbl = CnvButtonToGameObject(m_ReplayBtns);
                        GameObject goContent = m_trPlayReplaySelect.Find("Scroll View/Viewport/Content").gameObject;
                        ContentType contentType = m_isMobilePlatform ? ContentType.Type11 : ContentType.Type10;
                        m_goPlay_Replay = GetInputGameObject(_tbl, m_goPlay_Replay, isUp, isDown, false, false, InputType.Type1, goContent, contentType);
                        go = m_goPlay_Replay;
                    }
                    else if (m_trPlayMissionSelect.gameObject.activeSelf)
                    {
                        GameObject[] _tbl = CnvButtonToGameObject(m_MissionBtns);
                        //GameObject goContent = m_trPlayReplaySelect.Find("Scroll View/Viewport/Content").gameObject;
                        m_goPlay_Mission = GetInputGameObject(_tbl, m_goPlay_Mission, isUp, isDown/*, goContent*/);
                        go = m_goPlay_Mission;
                    }
                    else
                    {
                        m_goPlay_Select = GetInputGameObject(m_goPlaySelectList.ToArray(), m_goPlay_Select, isUp, isDown, isLeft, isRight, InputType.Type2);
                        go = m_goPlay_Select;
                    }
                    break;
                case MenuPhase.Option_Update:
                    if (m_trOptionGamepad.gameObject.activeSelf)
                    {
                        GameObject[] _tbl = CnvButtonToGameObject(m_OptionGamepadBtns);
                        GameObject goContent = m_trOptionGamepad.Find("Scroll View/Viewport/Content").gameObject;
                        ContentType contentType = m_isMobilePlatform ? ContentType.Type9 : ContentType.Type3;
                        m_goOption_Gamepad = GetInputGameObject(_tbl, m_goOption_Gamepad, isUp, isDown, false, false, InputType.Type1, goContent, contentType);
                        go = m_goOption_Gamepad;
                    }
                    else
                    if (m_trOptionKeyboard.gameObject.activeSelf)
                    {
                        GameObject[] _tbl = CnvButtonToGameObject(m_OptionKeyboardBtns);
                        GameObject goContent = m_trOptionKeyboard.Find("Scroll View/Viewport/Content").gameObject;
                        ContentType contentType = m_isMobilePlatform ? ContentType.Type8: ContentType.Type2;
                        m_goOption_Keyboard = GetInputGameObject(_tbl, m_goOption_Keyboard, isUp, isDown, false, false, InputType.Type1, goContent, contentType);
                        go = m_goOption_Keyboard;
                    }
                    else
                    {
                        // メトロノームBPM選択中のみ
                        if (m_goOptionBtns[OptionMetronomeBPMIndex] == m_goOption_Select)
                        {
                            SelectMetronomeBPM(isLeft, isRight);
                        }
                        if (m_isMobilePlatform)
                        {
                            GameObject goContent = m_trGrpOption.Find("Scroll View/Viewport/Content").gameObject;
                            m_goOption_Select = GetInputGameObject(m_goOptionBtns.ToArray(), m_goOption_Select, isUp, isDown, false, false, InputType.Type1, goContent, ContentType.Type7);
                        }
                        else
                        {
                            m_goOption_Select = GetInputGameObject(m_goOptionBtns.ToArray(), m_goOption_Select, isUp, isDown);
                        }
                        go = m_goOption_Select;
                    }
                    break;
                case MenuPhase.Account_Update:
                    {
                        if (m_isMobilePlatform)
                        {
                            GameObject goContent = m_trGrpAccount.Find("Scroll View/Viewport/Content").gameObject;
                            m_goAccount_Select = GetInputGameObject(m_goAccountSelectList.ToArray(), m_goAccount_Select, isUp, isDown, false, false, InputType.Type1, goContent, ContentType.Type6);
                        }
                        else
                        {
                            m_goAccount_Select = GetInputGameObject(m_goAccountSelectList.ToArray(), m_goAccount_Select, isUp, isDown);
                        }
                        go = m_goAccount_Select;
                    }
                    break;
                case MenuPhase.Dialog_Update:
                    if (m_DialogType == DialogType.Type3)
                    {
                        GameObject[] _tbl = { m_Btn_Dialog_Cancel.gameObject };
                        m_goDialog_Select = GetInputGameObject(_tbl, m_goDialog_Select, isUp, isDown);
                        go = m_goDialog_Select;
                    }
                    else
                    if (m_DialogType >= DialogType.Type4 && m_DialogType <= DialogType.Type7)
                    {
                        GameObject[] _tbl = { m_Btn_Dialog_ConfirmY.gameObject, m_Btn_Dialog_ConfirmN.gameObject };
                        m_goDialog_Select = GetInputGameObject(_tbl, m_goDialog_Select, isUp, isDown, isLeft, isRight, InputType.Type2);
                        go = m_goDialog_Select;
                    }
                    break;
                case MenuPhase.Game_Update:
                    m_goGamemode_Select = GetInputGameObject(m_goGamemodeSelectList.ToArray(), m_goGamemode_Select, isUp, isDown, isLeft, isRight, InputType.Type2);
                    go = m_goGamemode_Select;
                    break;
            }
            if (go != null)
            {
                SetSelectGameObject(go);
            }
        }
    }

    private bool IsPlayMode(AppData.PlayMode playMode)
    {
        return playMode == AppData.PlayMode.Free || playMode == AppData.PlayMode.Mission || playMode == AppData.PlayMode.Replay;
    }

    private bool CheckDesideSe(GameObject go)
    {
        int idx = m_goExcludeDesideSeList.FindIndex(x => x == go);
        return idx < 0;
    }

    private void SetDefaultSelect()
    {
        m_goStart_Select = m_goStartSelectList[0];
        m_goCreate_Select = m_goCreateSelectList[0];
        m_goLogin_Select = m_goLoginSelectList[0];
        m_goLoggedin_Select = m_goLoggedinSelectList[0];
        m_goPlay_Select = m_goPlaySelectList[0];
        m_goOption_Select = m_goOptionBtns[0].gameObject;
        m_goAccount_Select = m_goAccountSelectList[0];
        m_goGamemode_Select = m_goGamemodeSelectList[0];
    }

    private void SetTapOverComponent(GameObject go, Select select)
    {
        TapOverComponent tap = go.AddComponent<TapOverComponent>();
        tap.SetLogin(this, select);
    }

    /// <summary>
    /// 入力終了と同時にキートリガを有効にすると同一フレームでキートリガが反応してしまうので、1フレーム待ってから入力有効にする
    /// </summary>
    /// <returns></returns>
    private void SetDelayDisableInputAll()
    {
        StartCoroutine(crDelayDisableInputAll());
    }
    private IEnumerator crDelayDisableInputAll()
    {
        yield return null;
        m_isDisableInputAll = false;
    }

    /// <summary>
    /// 入力終了と同時にキートリガを有効にすると同一フレームでキートリガが反応してしまうので、1フレーム待ってから入力有効にする
    /// </summary>
    /// <returns></returns>
    private void SetDelayDisableInputMove()
    {
        StartCoroutine(crDelayDisableInputMove());
    }
    private IEnumerator crDelayDisableInputMove()
    {
        yield return null;
        m_isEnableInputField = false;
    }

    private void ClearDisableInput()
    {
        m_isDisableInputAll = false;
        m_isEnableInputField = false;
    }

    /// <summary>
    /// ゲームパッド接続チェック ※このチェックをする前に必ずゲームパッドの入力をさせる事
    /// </summary>
    private void CheckConnectGamePad()
    {
        if (AppData.ControllerMode == AppData.ControllerM.GamePad)
        {
            var controllers = Input.GetJoystickNames();
            bool isExistController = false;
            if (controllers.Length > 0)
            {
                isExistController = !(controllers[0] == "");
            }

            if (!isExistController)
            {
                AppData.SetControllerMode(AppData.ControllerM.Keyboard);
                AppCommon.Update_And_SaveGameData();
            }
        }
    }

    //================================================
    // [///] JavaScript側から呼ばれるコールバック
    //================================================

    // MEMO:static関数のみ利用可能

    /// <summary>
    /// FB_SignInJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))] // jslibからのコールバックとして利用する際は本attributeを付与する
    private static void OnSignIn(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_SignIn:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessSignIn(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorSignIn(outputStr);
        }
    }

    /// <summary>
    /// FB_RegisterUserJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnRegisterUser(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_RegisterUser:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessRegisterUser(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorRegisterUser(outputStr);
        }
    }

    /// <summary>
    /// FB_CreateUserInfoStorageJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnCreateUserInfoStorage(int instanceId, string outputStr, bool isSuccess)
    {
#if ERROR_TEST_CREATE_USER_STORAGE
        // ここでfalseにしても、isSuccessがtrueの時はユーザー情報がstorageに作成されてしまっている事に注意
        isSuccess = false;
#endif
        Debug.Log("IsSuccess_CreateUserInfoStorage:" + isSuccess.ToString());

        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessCreateUserInfoStorage(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorCreateUserInfoStorage(outputStr);
        }
    }

    /// <summary>
    /// FB_GetUserInfoStorageJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnGetUserInfoStorage(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_GetUserInfoStorage:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessGetUserInfoStorage(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorGetUserInfoStorage(outputStr);
        }
    }

    /// <summary>
    /// FB_DeleteUserInfoStorageJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnDeleteUserInfoStorage(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_DeleteUserStorage:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessDeleteUserInfoStorage(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorDeleteUserInfoStorage(outputStr);
        }
    }

    /// <summary>
    /// FB_UpdateProfileJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnUpdateProfile(int instanceId, string outputStr, bool isSuccess)
    {
#if ERROR_TEST_CREATE_UPDATE_PROFILE
        isSuccess = false;
#endif
        Debug.Log("IsSuccess_UpdateProfile:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessUpdateProfile(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorUpdateProfile(outputStr);
        }
    }

    /// <summary>
    /// FB_SendEmailVerificationJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnSendEmailVerification(int instanceId, string outputStr, bool isSuccess)
    {
#if ERROR_TEST_CREATE_SEND_EMAIL
        isSuccess = false;
#endif
        Debug.Log("IsSuccess_SendEmailVerification:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessSendEmailVerification(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorSendEmailVerification(outputStr);
        }
    }

    /// <summary>
    /// FB_DeleteUserJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnDeleteUser(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_DeleteUser:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessDeleteUser(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorDeleteUser(outputStr);
        }
    }

    /// <summary>
    /// FB_UpdateEmailJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnUpdateEmail(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_UpdateEmail:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessUpdateEmail(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorUpdateEmail(outputStr);
        }
    }

    /// <summary>
    /// FB_CreateEmailVerifiedJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnCreateEmailVerified(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_CreateEmailVerified:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessCreateEmailVerified(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorCreateEmailVerified(outputStr);
        }
    }

    /// <summary>
    /// FB_GetCurrentUserJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnGetCurrentUser(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_GetCurrentUser:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessGetCurrentUser(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorGetCurrentUser(outputStr);
        }
    }

    /// <summary>
    /// FB_GetCurrentUserReloadJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnGetCurrentUserReload(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_GetCurrentUserReload:" + isSuccess.ToString());
        Instances[instanceId].m_isReloadSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessGetCurrentUserReload(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorGetCurrentUserReload(outputStr);
        }
    }

    /// <summary>
    /// FB_SignOutJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnSignOut(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_SignOut:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessSignOut(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorSignOut(outputStr);
        }
    }

    /// <summary>
    /// FB_UpdatePasswordJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnUpdatePassword(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_UpdatePassword:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessUpdatePassword(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorUpdatePassword(outputStr);
        }
    }

    /// <summary>
    /// FB_ChangeEmailVerifiedJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnChangeEmailVerified(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_ChangeEmailVerified:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessChangeEmailVerified(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorChangeEmailVerified(outputStr);
        }
    }

    /// <summary>
    /// FB_ReauthenticateWithCredentialJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnReauthenticateWithCredential(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_ReauthenticateWithCredential:" + isSuccess.ToString());
        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessReauthenticateWithCredential(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorReauthenticateWithCredential(outputStr);
        }
    }

    /// <summary>
    /// FB_CreateReplayInfoStorageJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnCreateReplayInfoStorage(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_CreateReplayInfoStorage:" + isSuccess.ToString());

        Instances[instanceId].m_isReplaySuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessCreateReplayInfoStorage(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorCreateReplayInfoStorage(outputStr);
        }
    }

    /// <summary>
    /// FB_GetReplayInfoStorageJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnGetReplayInfoStorage(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_GetReplayInfoStorage:" + isSuccess.ToString());

        Instances[instanceId].m_isReplaySuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessGetReplayInfoStorage(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorGetReplayInfoStorage(outputStr);
        }
    }

    /// <summary>
    /// FB_GetReplayInfoCountStorageJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnGetReplayInfoCountStorage(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("IsSuccess_GetReplayInfoCountStorage:" + isSuccess.ToString());

        Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessGetReplayInfoCountStorage(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorGetReplayInfoCountStorage(outputStr);
        }
    }

    /// <summary>
    /// FB_OnAuthStateChangedJSの実行結果のコールバック
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
    private static void OnAuthStateChanged(int instanceId, string outputStr, bool isSuccess)
    {
        Debug.Log("---Received_OnAuthStateChanged:" + isSuccess.ToString() + "---");
        //Instances[instanceId].m_isSuccess = isSuccess;
        if (isSuccess)
        {
            Instances[instanceId].SuccessOnAuthStateChanged(outputStr);
        }
        else
        {
            Instances[instanceId].ErrorOnAuthStateChanged(outputStr);
        }
    }

    //================================================
    // [///] AddListener系
    //================================================

    // storage 4.66GBが上限

    private void OnClick_Replay_Test()
    {
        //List<AppData.ReplayInfo> list = new List<AppData.ReplayInfo>();
        // 5分= 60frame*60s*5=18000
        // →JSONサイズ 498.95KB   送信1.8s 受信0.9s
        // →4.66GB/499KB ≒ 9792人
        // 10分= 60frame*60s*10=36000
        // →JSONサイズ 1008.72KB   送信2.45s 受信1.1s
        // →4.66GB/1009KB ≒ 4842人
        // 20分= 60frame*60s*20=72000
        // →JSONサイズ 1.98MB   送信3.3s 受信1.3s
        // →4.66GB/1.98MB ≒ 2410人
        // 100000 2.78MB
        //for (int i = 0; i < 18000; ++i)
        //{
        //    list.Add(new AppData.ReplayInfo(i, UnityEngine.Random.Range(10, 20).ToString()));
        //}
        //int setIndex = 0;
        //AppData.SetReplayData(AppData.PlayMode.Free, list, setIndex);
        //m_ReplayStr = JsonUtility.ToJson(AppData.GetReplayData(setIndex)/*, true*/);
        //ChangeMenuPhase(MenuPhase.Replay_Send_Start);
    }



    private void OnClick_Email()
    {
        string beforeStr, outStr;
        switch (m_MenuPhase)
        {
            case MenuPhase.Login_Update:
                beforeStr = GetInputButtonStr(m_InputButton_LoginEmail);
                outStr = WebNativeDialog.OpenNativeStringDialog(m_InputButton_LoginEmail.displayInputText, beforeStr);
                SetInputButtonStr(m_InputButton_LoginEmail, outStr);
                ProcOnEndEdit_Email(outStr);
                break;
            case MenuPhase.Create_Update:
                beforeStr = GetInputButtonStr(m_InputButton_CreateEmail);
                outStr = WebNativeDialog.OpenNativeStringDialog(m_InputButton_CreateEmail.displayInputText, beforeStr);
                SetInputButtonStr(m_InputButton_CreateEmail, outStr);
                ProcOnEndEdit_Email(outStr);
                break;
            case MenuPhase.Account_Update:
                beforeStr = GetInputButtonStr(m_InputButton_AccountEmail);
                outStr = WebNativeDialog.OpenNativeStringDialog(m_InputButton_AccountEmail.displayInputText, beforeStr);
                SetInputButtonStr(m_InputButton_AccountEmail, outStr);
                ProcOnEndEdit_Email(outStr);
                break;
        }
     }

    private void OnClick_Password()
    {
        string beforeStr, outStr;
        switch (m_MenuPhase)
        {
            case MenuPhase.Login_Update:
                beforeStr = m_LoginPasswordStr/*GetInputButtonStr(m_InputButton_LoginPassword)*/;
                outStr = WebNativeDialog.OpenNativeStringDialog(m_InputButton_LoginPassword.displayInputText, beforeStr);
                SetInputButtonStrPassword(m_InputButton_LoginPassword, outStr);
                ProcOnEndEdit_Password(outStr);
                break;
            case MenuPhase.Create_Update:
                beforeStr = m_CreatePasswordStr/*GetInputButtonStr(m_InputButton_CreatePassword)*/;
                outStr = WebNativeDialog.OpenNativeStringDialog(m_InputButton_CreatePassword.displayInputText, beforeStr);
                SetInputButtonStrPassword(m_InputButton_CreatePassword, outStr);
                ProcOnEndEdit_Password(outStr);
                break;
            case MenuPhase.Account_Update:
                beforeStr = m_AccountPasswordStr/*GetInputButtonStr(m_InputButton_AccountPassword)*/;
                outStr = WebNativeDialog.OpenNativeStringDialog(m_InputButton_AccountPassword.displayInputText, beforeStr);
                SetInputButtonStrPassword(m_InputButton_AccountPassword, outStr);
                ProcOnEndEdit_Password(outStr);
                break;
        }
    }

    private void OnClick_Name()
    {
        string beforeStr, outStr;
        switch (m_MenuPhase)
        {
            case MenuPhase.Login_Update:
                break;
            case MenuPhase.Create_Update:
                beforeStr = GetInputButtonStr(m_InputButton_CreateName);
                outStr = WebNativeDialog.OpenNativeStringDialog(m_InputButton_CreateName.displayInputText, beforeStr);
                SetInputButtonStr(m_InputButton_CreateName, outStr);
                ProcOnEndEdit_Name(outStr);
                m_goCreate_Select = m_InputButton_CreateName.gameObject;
                break;
            case MenuPhase.Account_Update:
                beforeStr = GetInputButtonStr(m_InputButton_AccountName);
                outStr = WebNativeDialog.OpenNativeStringDialog(m_InputButton_AccountName.displayInputText, beforeStr);
                SetInputButtonStr(m_InputButton_AccountName, outStr);
                ProcOnEndEdit_Name(outStr);
                m_goAccount_Select = m_InputButton_AccountName.gameObject;
                break;
        }
    }

    private void OnClick_NowPassword()
    {
        string beforeStr, outStr;
        beforeStr = m_AccountNowPasswordStr;
        outStr = WebNativeDialog.OpenNativeStringDialog(m_InputButton_AccountNowPassword.displayInputText, beforeStr);
        SetInputButtonStrPassword(m_InputButton_AccountNowPassword, outStr);
        ProcOnEndEdit_NowPassword(outStr);
    }

    /// <summary>
    /// ダイアログの戻るボタン押下
    /// </summary>
    private void OnClick_Dialog_Cancel()
    {
        // 既にキャンセル済み
        if (m_isCancelFromUser) return;
        if (m_MenuPhase == MenuPhase.Create_EmailVerified_Update &&
            m_isReceived &&
            m_isSuccess)
        {
            // 確認完了済みなのでキャンセルさせない
            return;
        }
        else
        if (m_MenuPhase == MenuPhase.Account_EmailVerified_Update &&
            m_isReceived &&
            m_isSuccess)
        {
            // 確認完了済みなのでキャンセルさせない
            return;
        }

        m_isCancelFromUser = true;

        if (m_MenuPhase == MenuPhase.Create_EmailVerified_Update)
        {
            // ダイアログ表示後、ユーザー情報（firebase.storage、firebase.auth）を削除する
            SetDialog(true, MailConfirmCancelMessage, DialogType.Type2, MenuPhase.Create_Start, MenuPhase.Error_DeleteUserInfo_Start);
        }
        else
        if (m_MenuPhase == MenuPhase.Account_EmailVerified_Update)
        {
            // ダイアログを表示してアカウント変更画面に戻す
            SetDialog(true, ChangeMailConfirmCancelMessage, DialogType.Type2, MenuPhase.Account_Start);
        }
    }

    private void OnClick_Dialog_ConfirmY()
    {
        bool isDefault = false;
        switch (m_DialogType)
        {
            case DialogType.Type4:
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                // この先のMenuPhaseでstorageとuserの削除処理が走る
                m_MenuPhase = m_ConfirmYMenuPhase;
#else
                // TODO:EDITOR だと先に進めないので、強制的に削除成功状態にする
                SetStatusText("UNITY EDITOR is not execute .jslib file");
                ChangeMenuPhase(m_ReturnMenuPhase);
#endif
                isDefault = true;
                break;
            case DialogType.Type5:
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
                SignOut();
                m_MenuPhase = m_ConfirmYMenuPhase;
#else
                // TODO:EDITOR だと先に進めないので、強制的にログアウト成功状態にする
                SetStatusText("UNITY EDITOR is not execute .jslib file");
                ChangeMenuPhase(m_ReturnMenuPhase);
#endif
                break;
            case DialogType.Type6:
                // キーボードの設定初期化
                AppData.InitKeycode();
                // キーボード設定のボタン表示更新
                SetKeyboardBtnsText();

                m_goOption_Keyboard = m_OptionKeyboardBtns[0].gameObject;
                StartCoroutine(crKeyboardDelayDisp());
                // キーボード設定のスクロール位置を最初に戻しておく
                SetContentPosition(m_trOptionKeyboard.Find("Scroll View/Viewport/Content").gameObject, ContentType.Type2, 0);
                m_MenuPhase = m_ConfirmYMenuPhase;
                break;
            case DialogType.Type7:
                // ゲームパッドの設定初期化
                YnInputSystem.ResetOverrides();
                // ゲームパッドバインド情報のセーブ
                YnInputSystem.Save();
                // ゲームパッド設定のボタン表示更新
                SetGamepadBtnsText();

                m_goOption_Gamepad = m_OptionGamepadBtns[0].gameObject;
                StartCoroutine(crGamepadDelayDisp());
                // ゲームパッド設定のスクロール位置を最初に戻しておく
                SetContentPosition(m_trOptionGamepad.Find("Scroll View/Viewport/Content").gameObject, ContentType.Type3, 0);
                m_MenuPhase = m_ConfirmYMenuPhase;
                break;
        }

        if (isDefault)
        {
            // ボタン設定をデフォルトに戻しておく
            SetDefaultSelect();
        }

        SetDialog(false);
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    private void OnClick_Dialog_ConfirmN()
    {
        //// アカウント中のみ
        //    m_Btn_Account_Change.interactable = IsEnableChangeCheck();
        //    m_Btn_Account_Back.interactable = true;
        //    m_Btn_Account_Logout.interactable = true;
        //    m_Btn_Account_Delete.interactable = true;
        //
        //  // 戻るボタン有効
        //  m_isChecking = false;
        //  // 入力可にする
        //  m_AccountTouchArea.SetActive(false);

        SetDialog(false);

        m_MenuPhase = m_ConfirmNMenuPhase;
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ユーザー作成画面で性別ボタン押下
    /// </summary>
    private void OnClick_CreateGender()
    {
        m_trCreateGender.gameObject.SetActive(true);
        // 戻るボタンが選択されていたら、男性ボタンに変更する
        if (m_goCreate_Gender == m_CreateGenderBtns[m_CreateGenderBtns.Count - 1].gameObject)
        {
            m_goCreate_Gender = m_CreateGenderBtns[0].gameObject;
        }
        StartCoroutine(crCeateGenderDelayDisp());

        m_goCreate_Select = m_Btn_Create_Gender.gameObject;
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    private IEnumerator crCeateGenderDelayDisp()
    {
        yield return null;
        SetSelectGameObject(m_goCreate_Gender);
    }

    /// <summary>
    /// ユーザー作成画面で性別ボタン押下
    /// </summary>
    /// <param name="val"></param>
    private void OnClick_CreateGenders(int val)
    {
        m_CreateGenderStr = m_CreateGenderTexts[val].text;
        m_CreateGenderText.text = m_CreateGenderStr;
        m_trCreateGender.gameObject.SetActive(false);

        m_goCreate_Gender = m_CreateGenderBtns[val].gameObject;
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ユーザー作成画面で性別、年代、職業選択から戻るボタン押下
    /// </summary>
    private void OnClick_CreateBack()
    {
        if (m_trCreateGender.gameObject.activeSelf)
        {
            m_goCreate_Gender = m_CreateGenderBtns[m_CreateGenderBtns.Count - 1].gameObject;
        }
        else
        if (m_trCreateAge.gameObject.activeSelf)
        {
            m_goCreate_Age = m_CreateAgeBtns[m_CreateAgeBtns.Count - 1].gameObject;
        }
        else
        if (m_trCreateWork.gameObject.activeSelf)
        {
            m_goCreate_Work = m_CreateWorkBtns[m_CreateWorkBtns.Count - 1].gameObject;
        }
        m_trCreateGender.gameObject.SetActive(false);
        m_trCreateAge.gameObject.SetActive(false);
        m_trCreateWork.gameObject.SetActive(false);
        SetSelectGameObject(m_goCreate_Select);
    }

    /// <summary>
    /// ユーザー作成画面で年代ボタン押下
    /// </summary>
    private void OnClick_CreateAge()
    {
        m_trCreateAge.gameObject.SetActive(true);
        // 戻るボタンが選択されていたら、１０代ボタンに変更する
        if (m_goCreate_Age == m_CreateAgeBtns[m_CreateAgeBtns.Count - 1].gameObject)
        {
            m_goCreate_Age = m_CreateAgeBtns[0].gameObject;
        }
        StartCoroutine(crCeateAgeDelayDisp());

        m_goCreate_Select = m_Btn_Create_Age.gameObject;
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    private IEnumerator crCeateAgeDelayDisp()
    {
        yield return null;
        SetSelectGameObject(m_goCreate_Age);
    }

    /// <summary>
    /// ユーザー作成画面で年代ボタン押下
    /// </summary>
    /// <param name="val"></param>
    private void OnClick_CreateAges(int val)
    {
        m_CreateAgeStr = m_CreateAgeTexts[val].text;
        m_CreateAgeText.text = m_CreateAgeStr;
        m_trCreateAge.gameObject.SetActive(false);

        m_goCreate_Age = m_CreateAgeBtns[val].gameObject;
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ユーザー作成画面で職業ボタン押下
    /// </summary>
    private void OnClick_CreateWork()
    {
        m_trCreateWork.gameObject.SetActive(true);
        // 戻るボタンが選択されていたら、１０代ボタンに変更する
        if (m_goCreate_Work == m_CreateWorkBtns[m_CreateWorkBtns.Count - 1].gameObject)
        {
            m_goCreate_Work = m_CreateWorkBtns[0].gameObject;
        }
        StartCoroutine(crCeateWorkDelayDisp());

        m_goCreate_Select = m_Btn_Create_Work.gameObject;
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    private IEnumerator crCeateWorkDelayDisp()
    {
        yield return null;
        SetSelectGameObject(m_goCreate_Work);
    }

    /// <summary>
    /// ユーザー作成画面で職業ボタン押下
    /// </summary>
    /// <param name="val"></param>
    private void OnClick_CreateWorks(int val)
    {
        m_CreateWorkStr = m_CreateWorkTexts[val].text;
        m_CreateWorkText.text = m_CreateWorkStr;
        m_trCreateWork.gameObject.SetActive(false);

        m_goCreate_Work= m_CreateWorkBtns[val].gameObject;
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ログインボタン押下
    /// </summary>
    private void OnClick_Start_Login()
    {
        m_goStart_Select = m_Btn_Start_Login.gameObject;
        ChangeMenuPhase(MenuPhase.Login_Start);
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ユーザー作成ボタン押下
    /// </summary>
    private void OnClick_Start_Create()
    {
        m_goStart_Select = m_Btn_Start_Create.gameObject;
        ChangeMenuPhase(MenuPhase.Create_Start);
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// Nameで使用しているInputFiledの入力確定時
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnEndEdit_Name(string arg)
    {
        // nullチェック挿入
        if (!string.IsNullOrEmpty(arg))
        {
            // 入力文字列が最大文字数以上の時のみ
            if (arg.Length >= NameCharacterLimit)
            {
                arg = arg.Substring(0, NameCharacterLimit);
            }
        }

        switch (m_MenuPhase)
        {
            // ユーザー作成
            case MenuPhase.Create_Update:
            // ユーザー情報変更
            case MenuPhase.Account_Update:
                switch (m_MenuPhase)
                {
                    case MenuPhase.Create_Update:
                        m_CreateNameStr = arg;
                        // ユーザー作成チェックボタンの有効化
                        m_Btn_Create_Check.interactable = IsEnableCreateCheck();
                        break;
                    case MenuPhase.Account_Update:
                        m_AccountNameStr = arg;
                        // ユーザー情報変更ボタンの有効化
                        m_Btn_Account_Change.interactable = IsEnableChangeCheck();
                        break;
                }
                break;
        }

    }

    /// <summary>
    /// Genderで使用しているInputFiledの入力確定時
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnEndEdit_Gender(string arg)
    {
        m_CreateGenderStr = arg;
    }

    /// <summary>
    /// Ageで使用しているInputFiledの入力確定時
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnEndEdit_Age(string arg)
    {
        m_CreateAgeStr = arg;
    }

    /// <summary>
    /// Workで使用しているInputFiledの入力確定時
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnEndEdit_Work(string arg)
    {
        m_CreateWorkStr = arg;
    }


    private void ProcOnSelect_Email(string arg)
    {
        m_isEnableInputField = true;

        switch (m_MenuPhase)
        {
            // ユーザー作成
            case MenuPhase.Create_Update:
                m_goCreate_Select = m_InputField_CreateEmail.gameObject;
                break;
            // ログイン
            case MenuPhase.Login_Update:
                m_goLogin_Select = m_InputField_LoginEmail.gameObject;
                break;
            // ユーザー情報変更
            case MenuPhase.Account_Update:
                m_goAccount_Select = m_InputField_AccountEmail.gameObject;
                break;
        }
    }

    private void ProcOnSelect_Password(string arg)
    {
        m_isEnableInputField = true;

        switch (m_MenuPhase)
        {
            // ユーザー作成
            case MenuPhase.Create_Update:
                m_goCreate_Select = m_InputField_CreatePassword.gameObject;
                break;
            // ログイン
            case MenuPhase.Login_Update:
                m_goLogin_Select = m_InputField_LoginPassword.gameObject;
                break;
            // ユーザー情報変更
            case MenuPhase.Account_Update:
                m_goAccount_Select = m_InputField_AccountPassword.gameObject;
                break;
        }
    }

    private void ProcOnSelect_NowPassword(string arg)
    {
        m_isEnableInputField = true;
        m_goAccount_Select = m_InputField_AccountNowPassword.gameObject;
    }

    /// <summary>
    /// Emailで使用しているInputFiledの入力確定時
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnEndEdit_Email(string arg)
    {
        int characterLimit = 0;
        string beforeStr = "";
        switch (m_MenuPhase)
        {
            // ユーザー作成
            case MenuPhase.Create_Update:
                characterLimit = m_InputField_CreateEmail.characterLimit;
                beforeStr = m_CreateEmailStr;
                break;
            // ログイン
            case MenuPhase.Login_Update:
                characterLimit = m_InputField_LoginEmail.characterLimit;
                break;
            // ユーザー情報変更
            case MenuPhase.Account_Update:
                characterLimit = m_InputField_AccountEmail.characterLimit;
                break;
        }

        // nullチェック挿入
        if (!string.IsNullOrEmpty(arg) && characterLimit > 0)
        {
            // 入力文字列が最大文字数以上の時のみ
            if (arg.Length >= characterLimit)
            {
                arg = arg.Substring(0, characterLimit);
            }
        }

        switch (m_MenuPhase)
        {
            // ユーザー作成
            case MenuPhase.Create_Update:
                m_CreateEmailStr = arg;
                // ユーザー作成チェックボタンの有効化
                m_Btn_Create_Check.interactable = IsEnableCreateCheck();
                SetDelayDisableInputMove();
                break;
            // ログイン
            case MenuPhase.Login_Update:
                m_LoginEmailStr = arg;
                // ログインチェックボタンの有効化
                m_Btn_Login_Check.interactable = IsEnableLoginCheck();
                SetDelayDisableInputMove();
                break;
            // ユーザー情報変更
            case MenuPhase.Account_Update:
                m_AccountEmailStr = arg;
                // ユーザー情報変更ボタンの有効化
                m_Btn_Account_Change.interactable = IsEnableChangeCheck();
                SetDelayDisableInputMove();
                break;
        }
        //// SE:
        //MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// Passwordで使用しているInputFiledの入力確定時
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnEndEdit_Password(string arg)
    {
        //if (string.IsNullOrEmpty(arg))
        //{
        //    CmnInputFieled_SelectTextClear selectTextClear = m_InputField.gameObject.GetComponent<CmnInputFieled_SelectTextClear>();
        //    m_InputField.text = selectTextClear.m_PrevText;
        //    return;
        //}

        bool isSet = false;

        switch (m_MenuPhase)
        {
            // ユーザー作成
            case MenuPhase.Create_Update:
            // ユーザー情報変更
            case MenuPhase.Account_Update:
                if (string.IsNullOrEmpty(arg))
                {
                    // 未入力時
                    isSet = true;
                }
                else
                // 半角英数字のみ
                if (Regex.IsMatch(arg, @"^[0-9a-zA-Z]+$"))
                {
                    isSet = true;
                    // 1～7文字
                    if (arg.Length >= 1 && arg.Length < 8)
                    {
                        Debug.Log("password must have at least 8 characters");
                    }
                }
                else
                {
                    Debug.Log("invalid password characters");
                }
                {
                    switch (m_MenuPhase)
                    {
                        case MenuPhase.Create_Update:
                            if (isSet) m_CreatePasswordStr = arg;
                            // ユーザー作成チェックボタンの有効化
                            m_Btn_Create_Check.interactable = IsEnableCreateCheck();
                            SetDelayDisableInputMove();
                            break;
                        case MenuPhase.Account_Update:
                            if (isSet) m_AccountPasswordStr = arg;
                            // ユーザー情報変更ボタンの有効化
                            m_Btn_Account_Change.interactable = IsEnableChangeCheck();
                            SetDelayDisableInputMove();
                            break;
                    }
                }
                break;
            // ログイン
            case MenuPhase.Login_Update:
                m_LoginPasswordStr = arg;
                // ログインチェックボタンの有効化
                m_Btn_Login_Check.interactable = IsEnableLoginCheck();
                SetDelayDisableInputMove();
                break;
        }
        //// SE:
        //MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// NowPasswordで使用しているInputFiledの入力確定時
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnEndEdit_NowPassword(string arg)
    {
        if (string.IsNullOrEmpty(arg))
        {
            // 未入力の場合何もしない
        }
        else
        // 8文字必須
        if (arg.Length >= 1 && arg.Length < 8)
        {
            SetStatusText("password must have at least 8 characters");
        }
        else
        // 半角英数字のみ
        if (Regex.IsMatch(arg, @"^[0-9a-zA-Z]+$"))
        {
            m_AccountNowPasswordStr = arg;
        }

        // ユーザー情報変更ボタンの有効化
        m_Btn_Account_Change.interactable = IsEnableChangeCheck();
        SetDelayDisableInputMove();
        //// SE:
        //MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// Nameで使用しているInputFiledの入力確定時
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnValueChanged_Name(string arg)
    {
        switch (m_MenuPhase)
        {
            // ユーザー作成
            case MenuPhase.Create_Update:
                m_CreateNameStr = arg;
                // ユーザー作成チェックボタンの有効化
                m_Btn_Create_Check.interactable = IsEnableCreateCheck();
                break;
            // ユーザー情報変更
            case MenuPhase.Account_Update:
                m_AccountNameStr = arg;
                // ユーザー情報変更ボタンの有効化
                m_Btn_Account_Change.interactable = IsEnableChangeCheck();
                break;
            // ログイン
            case MenuPhase.Login_Update:
                break;
        }
    }

    /// <summary>
    /// Genderで使用しているInputFiledの入力確定時
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnValueChanged_Gender(string arg)
    {
        m_CreateGenderStr = arg;
    }

    /// <summary>
    /// Ageで使用しているInputFiledの入力確定時
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnValueChanged_Age(string arg)
    {
        m_CreateAgeStr = arg;
    }

    /// <summary>
    /// Workで使用しているInputFiledの入力確定時
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnValueChanged_Work(string arg)
    {
        m_CreateWorkStr = arg;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnValueChanged_Email(string arg)
    {
        m_isEnableInputField = true;

        switch (m_MenuPhase)
        {
            // ユーザー作成
            case MenuPhase.Create_Update:
                m_CreateEmailStr = arg;
                // ユーザー作成チェックボタンの有効化
                m_Btn_Create_Check.interactable = IsEnableCreateCheck();
                break;
            // ログイン
            case MenuPhase.Login_Update:
                m_LoginEmailStr = arg;
                // ログインチェックボタンの有効化
                m_Btn_Login_Check.interactable = IsEnableLoginCheck();
                break;
            // ユーザー情報変更
            case MenuPhase.Account_Update:
                m_AccountEmailStr = arg;
                // ユーザー情報変更ボタンの有効化
                m_Btn_Account_Change.interactable = IsEnableChangeCheck();
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arg"></param>
    private void ProcOnValueChanged_Password(string arg)
    {
        m_isEnableInputField = true;

        switch (m_MenuPhase)
        {
            // ユーザー作成
            case MenuPhase.Create_Update:
                m_CreatePasswordStr = arg;
                // ユーザー作成チェックボタンの有効化
                m_Btn_Create_Check.interactable = IsEnableCreateCheck();
                break;
            // ユーザー情報変更
            case MenuPhase.Account_Update:
                m_AccountPasswordStr = arg;
                // ユーザー情報変更ボタンの有効化
                m_Btn_Account_Change.interactable = IsEnableChangeCheck();
                break;
            // ログイン
            case MenuPhase.Login_Update:
                m_LoginPasswordStr = arg;
                // ログインチェックボタンの有効化
                m_Btn_Login_Check.interactable = IsEnableLoginCheck();
                break;
        }
    }

    private void ProcOnValueChanged_NowPassword(string arg)
    {
        m_isEnableInputField = true;
        m_AccountNowPasswordStr = arg;
        // ユーザー情報変更ボタンの有効化
        m_Btn_Account_Change.interactable = IsEnableChangeCheck();
    }

    private void SetPasswordTextDisplayY(TMP_InputField inputField, bool flag)
    {
        Transform tr = inputField.gameObject.transform.Find("Text Area/Text");
        if (tr)
        {
            RectTransform rt = tr.GetComponent<RectTransform>();
            Vector3 pos = rt.anchoredPosition3D;
            pos.y = flag ? 10.0f : -12.0f;
            rt.anchoredPosition3D = pos;
        }
    }

    /// <summary>
    /// オプション画面メトロノームBPMの値変更時
    /// </summary>
    /// <param name="val"></param>
    private void ProcOnValueChanged_BPM(float val)
    {
        AppData.SetMetronomeBPM((int)val);
        m_OptionBPMText.text = AppData.MetronomeBPM.ToString();
    }

    /// <summary>
    /// オプション画面メトロノームBPMの値変更（カーソルキー又はGamepad）
    /// </summary>
    /// <param name="isLeft"></param>
    /// <param name="isRight"></param>
    private void SelectMetronomeBPM(bool isLeft, bool isRight)
    {
        int val = AppData.MetronomeBPM;
        if (isLeft) val -= OptionMetronomeBPMAdd;
        if (isRight) val += OptionMetronomeBPMAdd;
        val = Mathf.Max(val, OptionMetronomeBPMLow);
        val = Mathf.Min(val, OptionMetronomeBPMHigh);
        AppData.SetMetronomeBPM(val);
        m_OptionBPMSlider.value = val;
        UpdateOptionBtns(OptionMetronomeBPMIndex, false);
    }


    /// <summary>
    /// Password文字表示・非表示切り替え
    /// </summary>
    /// <param name="flag"></param>
    private void ProcOnValueChanged_PasswordToggle(bool flag)
    {
        //Debug.Log("ToggleReceive:" + flag);
        //Debug.Log("ToggleIsOn:" + m_Toggle_LoginPassword.isOn);

        switch (m_MenuPhase)
        {
            // ユーザー作成
            case MenuPhase.Create_Update:
                if (flag)
                {
                    m_InputField_CreatePassword.contentType = TMP_InputField.ContentType.Alphanumeric;
                }
                else
                {
                    m_InputField_CreatePassword.contentType = TMP_InputField.ContentType.Password;
                }
                SetPasswordTextDisplayY(m_InputField_CreatePassword, flag);
                break;
            // ログイン
            case MenuPhase.Login_Update:
                if (flag)
                {
                    m_InputField_LoginPassword.contentType = TMP_InputField.ContentType.Alphanumeric;
                }
                else
                {
                    m_InputField_LoginPassword.contentType = TMP_InputField.ContentType.Password;
                }
                SetPasswordTextDisplayY(m_InputField_LoginPassword, flag);
                break;
            // ユーザー情報変更
            case MenuPhase.Account_Update:
                if (flag)
                {
                    m_InputField_AccountPassword.contentType = TMP_InputField.ContentType.Alphanumeric;
                }
                else
                {
                    m_InputField_AccountPassword.contentType = TMP_InputField.ContentType.Password;
                }
                SetPasswordTextDisplayY(m_InputField_AccountPassword, flag);
                break;
        }

        // PasswordのInputFiled表示更新
        StartCoroutine(Reload_PasswordInputField());
    }

    /// <summary>
    /// PasswordのInputFiled表示更新
    /// </summary>
    /// <returns></returns>
    private IEnumerator Reload_PasswordInputField()
    {
        switch (m_MenuPhase)
        {
            // ユーザー作成
            case MenuPhase.Create_Update:
                // 未入力なら何もしない
                if (string.IsNullOrEmpty(m_InputField_CreatePassword.text)) yield break;

                m_InputField_CreatePassword.ActivateInputField();
                yield return null;
                m_InputField_CreatePassword.MoveTextEnd(true);
                //yield return null;
                //m_InputField_CreatePassword.DeactivateInputField();
                break;
            // ログイン
            case MenuPhase.Login_Update:
                // 未入力なら何もしない
                if (string.IsNullOrEmpty(m_InputField_LoginPassword.text)) yield break;

                m_InputField_LoginPassword.ActivateInputField();
                yield return null;
                m_InputField_LoginPassword.MoveTextEnd(true);
                //yield return null;
                //m_InputField_LoginPassword.DeactivateInputField();
                break;
            // ユーザー情報変更
            case MenuPhase.Account_Update:
                // 未入力なら何もしない
                if (string.IsNullOrEmpty(m_InputField_AccountPassword.text)) yield break;

                m_InputField_AccountPassword.ActivateInputField();
                yield return null;
                m_InputField_AccountPassword.MoveTextEnd(true);
                //yield return null;
                //m_InputField_AccountPassword.DeactivateInputField();
                break;
        }
    }

    /// <summary>
    /// NowPassword文字表示・非表示切り替え
    /// </summary>
    /// <param name="flag"></param>
    private void ProcOnValueChanged_NowPasswordToggle(bool flag)
    {
        //Debug.Log("ToggleReceive:" + flag);
        //Debug.Log("ToggleIsOn:" + m_Toggle_LoginPassword.isOn);

        if (flag)
        {
            m_InputField_AccountNowPassword.contentType = TMP_InputField.ContentType.Alphanumeric;
        }
        else
        {
            m_InputField_AccountNowPassword.contentType = TMP_InputField.ContentType.Password;
        }
        SetPasswordTextDisplayY(m_InputField_AccountPassword, flag);

        // PasswordのInputFiled表示更新
        StartCoroutine(Reload_NowPasswordInputField());
    }

    /// <summary>
    /// PasswordのInputFiled表示更新
    /// </summary>
    /// <returns></returns>
    private IEnumerator Reload_NowPasswordInputField()
    {
        // 未入力なら何もしない
        if (string.IsNullOrEmpty(m_InputField_AccountNowPassword.text)) yield break;

        m_InputField_AccountNowPassword.ActivateInputField();
        yield return null;
        m_InputField_AccountNowPassword.MoveTextEnd(true);
        //yield return null;
        //m_InputField_AccountNowPassword.DeactivateInputField();
    }

    /// <summary>
    /// ユーザー作成ボタン押下
    /// </summary>
    private void OnClick_CreateCheck()
    {
        if (IsEnableCreateCheck())
        {
            // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
            RegisterUser();
#else
            // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
            m_isReceived = true;
            m_isSuccess = true;
            SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif
        }
        else
        {
            SetStatusText("EMAIL or PASSWORD is empty");
        }

        //if (m_Toggle_CreatePassword) m_Toggle_CreatePassword.interactable = false;

        // 戻るボタン無効化
        m_isChecking = true;
        // 入力不可にする
        m_CreateTouchArea.SetActive(true);

        m_goCreate_Select = m_goCreateSelectList[m_goCreateSelectList.Count - 2];
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// スタートへ戻るボタン押下
    /// </summary>
    private void OnClick_BackStart()
    {
        if (m_isChecking) return;

        switch (m_MenuPhase)
        {
            // ユーザー作成
            case MenuPhase.Create_Update:
                // ユーザー作成からの戻り時は、EMAILとPASSWORDを未入力状態にしておく
                m_InputField_CreateEmail.text = "";
                //m_InputField_CreateName.text = "";
                m_CreateGenderText.text = "";
                m_CreateAgeText.text = "";
                m_CreateWorkText.text = "";
                m_InputField_CreatePassword.text = "";

                SetInputButtonStr(m_InputButton_CreateEmail, "");
                SetInputButtonStr(m_InputButton_CreateName, "");
                SetInputButtonStr(m_InputButton_CreatePassword, "");

                m_CreateNameStr = "";
                m_CreateEmailStr = "";
                m_CreatePasswordStr = "";

                //if (m_Toggle_CreatePassword) m_Toggle_CreatePassword.interactable = false;

                m_goCreate_Select = m_goCreateSelectList[m_goCreateSelectList.Count - 1];
                break;
            // ログイン
            case MenuPhase.Login_Update:
                m_goLogin_Select = m_goLoginSelectList[m_goLoginSelectList.Count - 1];
                break;
        }

        ChangeMenuPhase(MenuPhase.Start_Start);
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ログインチェックボタン押下
    /// </summary>
    private void OnClick_LoginCheck()
    {
        if (IsEnableLoginCheck())
        {
            // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
            SignIn();
#else
            // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
            m_isReceived = true;
            m_isSuccess = true;
            SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif
        }
        else
        {
            SetStatusText("EMAIL or PASSWORD is empty");
        }

        //m_InputField_LoginEmail.interactable = false;
        //m_InputField_LoginPassword.interactable = false;
        //if (m_Toggle_LoginPassword) m_Toggle_LoginPassword.interactable = false;

        // 戻るボタン無効化
        m_isChecking = true;
        // 入力不可にする
        m_LoginTouchArea.SetActive(true);

        m_goLogin_Select = m_goLoginSelectList[m_goLoginSelectList.Count - 2];
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ログイン中のプレイボタン押下
    /// </summary>
    private void OnClick_Loggedin_Play()
    {
        ChangeMenuPhase(MenuPhase.Play_Start);

        m_goLoggedin_Select = m_goLoggedinSelectList[0];
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ログイン中のオプションボタン押下
    /// </summary>
    private void OnClick_Loggedin_Option()
    {
        ChangeMenuPhase(MenuPhase.Option_Start);

        // オプション画面を表示する直前にチェック
        CheckConnectGamePad();
        // コントローラーモード設定表示更新
        UpdateOptionBtns(1, false);

        m_goLoggedin_Select = m_goLoggedinSelectList[1];
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ログイン中のアカウント設定ボタン押下
    /// </summary>
    private void OnClick_Loggedin_Account()
    {
        ChangeMenuPhase(MenuPhase.Account_Start);

        m_goLoggedin_Select = m_goLoggedinSelectList[2];
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// プレイ中のフリーボタン押下
    /// </summary>
    private void OnClick_Play_Free()
    {
        if (m_MenuPhase < MenuPhase.Update)
        {
            AppData.m_PlayMode = AppData.PlayMode.Free;

            m_Timer = 0.0f;
            m_MenuPhase = MenuPhase.Update;
            // SE:
            MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
        }
    }

    /// <summary>
    /// プレイ中のミッションボタン押下
    /// </summary>
    private void OnClick_Play_Mission()
    {
        // if (m_MenuPhase < MenuPhase.Update)
        // {
        //     AppData.m_PlayMode = AppData.PlayMode.Mission;
        // 
        //     m_Timer = 0.0f;
        //     m_MenuPhase = MenuPhase.Update;
        //     // SE:
        //     MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
        // }

        m_trPlayMissionSelect.gameObject.SetActive(true);

        m_goPlay_Mission = m_MissionBtns[0].gameObject;
        m_goPlay_Select = m_goPlaySelectList[1];

        StartCoroutine(crMissionDelayDisp());

        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    private IEnumerator crMissionDelayDisp()
    {
        yield return null;
        SetSelectGameObject(m_goPlay_Mission);
    }


    /// <summary>
    /// プレイ中のリプレイボタン押下
    /// </summary>
    private void OnClick_Play_Replay()
    {
        if (AppData.m_ReceiveReplayCountStorageInfo != null)
        {
            //bool isDownloaded = isGetReplayData();
            SetReplayDataList(true/*isDownloaded*/);
            //if (!isDownloaded)
            //{
            //    if (m_crGetReplayDataWait != null)
            //    {
            //        StopCoroutine(m_crGetReplayDataWait);
            //        m_crGetReplayDataWait = null;
            //        Debug.Log("delete coroutine [crGetReplayDataWait]");
            //    }
            //    m_crGetReplayDataWait = StartCoroutine(crGetReplayDataWait());
            //}

            ContentType contentType = m_isMobilePlatform ? ContentType.Type11 : ContentType.Type10;

            // リプレイ選択のスクロール位置を最初に戻しておく
            SetContentPosition(m_trPlayReplaySelect.Find("Scroll View/Viewport/Content").gameObject, contentType, 0);

            m_goPlay_Select = m_goPlaySelectList[2];
            // SE:
            MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
        }
    }

    private void SetReplayDataList(bool isDisp)
    {
        ScrollRect sr = m_trPlayReplaySelect.Find("Scroll View").GetComponent<ScrollRect>();

        sr.gameObject.SetActive(isDisp);

        m_trPlayReplaySelect.gameObject.SetActive(true);

        if (isDisp)
        {
            Transform trBase = sr.transform.Find("Viewport/Content");
            GameObject goBaseBtn = trBase.Find("Btn_Replay").gameObject;

            // 多重登録させないようにリスト上にある replay_xx を消す
            RectTransform[] rtButtons = trBase.GetComponentsInChildren<RectTransform>();
            if (rtButtons != null && rtButtons.Length > 0)
            {
                foreach (RectTransform rt in rtButtons)
                {
                    if (rt.name.Contains("replay_"))
                    {
                        Destroy(rt.gameObject);
                    }
                }
            }

            goBaseBtn.SetActive(false);

            m_ReplayBtns.Clear();

            RectTransform contentRt = trBase.GetComponent<RectTransform>();
            Vector2 size = contentRt.sizeDelta;
            size.y = AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count * 125;
            contentRt.sizeDelta = size;

            for (int i = 0; i < AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count; ++i)
            {
                GameObject go = Instantiate(goBaseBtn);
                if (go)
                {
                    go.transform.SetParent(trBase);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    go.name = AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_FileName;
                    go.SetActive(true);
                    Button _btn = go.GetComponentInChildren<Button>();
                    int i1 = i;
                    _btn.onClick.AddListener(() => OnClick_ReplayButtons(i1));
                    m_ReplayBtns.Add(_btn);
                    SetTapOverComponent(_btn.gameObject, Select.PlayReplay);

                    TextMeshProUGUI text = _btn.transform.Find("Text1").GetComponentInChildren<TextMeshProUGUI>();
                    text.text = AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_FileName;
                    text = _btn.transform.Find("Text2").GetComponentInChildren<TextMeshProUGUI>();
                    text.text = ReplayModes[(int)AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_ReceiveReplayStorageInfo.m_PlayMode];
                    text = _btn.transform.Find("Text3").GetComponentInChildren<TextMeshProUGUI>();
                    text.text = AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_TimeCreated;
                }
            }

            // 戻るボタンを加えておく
            m_ReplayBtns.Add(m_Btn_ReplaySelect_Back);

            sr.vertical = AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count >= 7;

            m_goPlay_Replay = m_ReplayBtns[0].gameObject;

            StartCoroutine(crReplayDelayDisp());
        }

    }

    private IEnumerator crReplayDelayDisp()
    {
        yield return null;
        SetSelectGameObject(m_goPlay_Replay);
    }

    /// <summary>
    /// 全てのリプレイデータが読み終わるまで待機するコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator crGetReplayDataWait()
    {
        GameObject goAlert = m_trGrpPlay.Find("Alert").gameObject;
        goAlert.SetActive(true);

        while (true)
        {
            if (isGetReplayData())
            {
                goAlert.SetActive(false);
                break;
            }
            yield return null;
        }

        m_crGetReplayDataWait = null;
    }

    /// <summary>
    /// 個別のリプレイボタン押下
    /// </summary>
    /// <param name="index"></param>
    private void OnClick_ReplayButtons(int index)
    {
        if (m_MenuPhase < MenuPhase.Update)
        {
            Debug.Log("play replay:" + AppData.m_ReceiveReplayCountStorageInfo.m_Infos[index].m_FileName);

            AppData.m_SelectReplayIndex = index;
            AppData.m_PlayMode = AppData.PlayMode.Replay;
        
            m_Timer = 0.0f;
            m_MenuPhase = MenuPhase.Update;
            // SE:
            MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
        }
    }

    /// <summary>
    /// リプレイ選択から戻るボタン押下
    /// </summary>
    private void OnClick_Replay_Back()
    {
        if (m_crGetReplayDataWait != null)
        {
            StopCoroutine(m_crGetReplayDataWait);
            m_crGetReplayDataWait = null;
            Debug.Log("delete coroutine [crGetReplayDataWait]");
        }
        m_trPlayReplaySelect.gameObject.SetActive(false);
        ChangeMenuPhase(MenuPhase.Play_Start);

        m_goPlay_Replay = m_Btn_ReplaySelect_Back.gameObject;
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ミッションモードボタン押下
    /// </summary>
    /// <param name="index"></param>
    private void OnClick_Mission_Square()
    {
        if (m_MenuPhase < MenuPhase.Update)
        {
            AppData.m_PlayMode = AppData.PlayMode.Mission;
            AppData.m_MissionId = AppData.MissionID.SquareFly;

            m_Timer = 0.0f;
            m_MenuPhase = MenuPhase.Update;
            // SE:
            MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
        }
    }
    /// <summary>
    /// ミッションモードボタン押下
    /// </summary>
    /// <param name="index"></param>
    private void OnClick_Mission_Eight()
    {
        if (m_MenuPhase < MenuPhase.Update)
        {
            AppData.m_PlayMode = AppData.PlayMode.Mission;
            AppData.m_MissionId = AppData.MissionID.EightFly;

            m_Timer = 0.0f;
            m_MenuPhase = MenuPhase.Update;
            // SE:
            MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
        }
    }
    /// <summary>
    /// ミッションモードボタン押下
    /// </summary>
    /// <param name="index"></param>
    private void OnClick_Mission_Happning()
    {
        if (m_MenuPhase < MenuPhase.Update)
        {
            AppData.m_PlayMode = AppData.PlayMode.Mission;
            AppData.m_MissionId = AppData.MissionID.HappningFly;

            m_Timer = 0.0f;
            m_MenuPhase = MenuPhase.Update;
            // SE:
            MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
        }
    }

    /// <summary>
    /// ミッション選択から戻るボタン押下
    /// </summary>
    private void OnClick_Mission_Back()
    {
        m_trPlayMissionSelect.gameObject.SetActive(false);
        ChangeMenuPhase(MenuPhase.Play_Start);

        m_goPlay_Mission = m_MissionBtns[0].gameObject;
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// メニュー：ゲームモードボタン押下
    /// </summary>
    private void OnClick_Loggedin_Game()
    {
        ChangeMenuPhase(MenuPhase.Game_Start);

        m_goLoggedin_Select = m_goLoggedinSelectList[3];
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }
    /// <summary>
    /// ゲームモードメニュー：宝探しボタン押下
    /// </summary>
    private void OnClick_Game_Treasure()
    {
        if (m_MenuPhase < MenuPhase.Update)
        {
            AppData.m_PlayMode = AppData.PlayMode.Game_Treasure;

            m_Timer = 0.0f;
            m_MenuPhase = MenuPhase.Game_TransGame1;
            // SE:
            MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
        }
    }
    /// <summary>
    /// ゲームモードメニュー：イライラ棒ボタン押下
    /// </summary>
    private void OnClick_Game_Irairabou()
    {
        if (m_MenuPhase < MenuPhase.Update)
        {
            AppData.m_PlayMode = AppData.PlayMode.Game_Irairabou;

            m_Timer = 0.0f;
            m_MenuPhase = MenuPhase.Game_TransGame1;
            // SE:
            MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
        }
    }
    /// <summary>
    /// ゲームモードメニュー：エンドレスボタン押下
    /// </summary>
    private void OnClick_Game_Endress()
    {
        if (m_MenuPhase < MenuPhase.Update)
        {
            AppData.m_PlayMode = AppData.PlayMode.Game_Endress;

            m_Timer = 0.0f;
            m_MenuPhase = MenuPhase.Game_TransGame1;
            // SE:
            MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
        }
    }


    /// <summary>
    /// ユーザー情報変更開始ボタン押下
    /// </summary>
    private void OnClick_Account_Change()
    {
        if (IsEnableChangeCheck())
        {
            // EDITOR上でコールするとエラーになるので囲う
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
            ChangeUserInfo();
#else
            // TODO:EDITOR だと先に進めないからどうするか。。。ひとまず無条件で進めてみる
            m_isReceived = true;
            m_isSuccess = true;
            SetStatusText("UNITY EDITOR is not execute .jslib file");
#endif
        }
        else
        {
            SetStatusText("not entered");
        }

        //if (m_Toggle_AccountPassword) m_Toggle_AccountPassword.interactable = false;
        //if (m_Toggle_AccountNowPassword) m_Toggle_AccountNowPassword.interactable = false;

        // 戻るボタン無効化
        m_isChecking = true;
        // 入力不可にする
        m_AccountTouchArea.SetActive(true);

        m_goAccount_Select = m_goAccountSelectList[4];
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ログイン状態へ戻るボタン押下
    /// </summary>
    private void OnClick_BackLoggedin()
    {
        m_InputField_AccountEmail.text = "";
        //m_InputField_AccountName.text = "";
        m_InputField_AccountPassword.text = "";
        m_InputField_AccountNowPassword.text = "";

        SetInputButtonStr(m_InputButton_AccountEmail, "");
        SetInputButtonStr(m_InputButton_AccountName, "");
        SetInputButtonStr(m_InputButton_AccountPassword, "");
        SetInputButtonStr(m_InputButton_AccountNowPassword, "");

        m_AccountNameStr = "";
        m_AccountEmailStr = "";
        m_AccountPasswordStr = "";
        m_AccountNowPasswordStr = "";

        //if (m_Toggle_AccountPassword) m_Toggle_AccountPassword.interactable = false;
        //if (m_Toggle_AccountNowPassword) m_Toggle_AccountNowPassword.interactable = false;

        // 戻るボタン無効化
        m_isChecking = true;
        // 入力不可にする
        m_AccountTouchArea.SetActive(true);

        switch (m_MenuPhase)
        {
            case MenuPhase.Play_Update:
                m_goPlay_Select = m_goPlaySelectList[m_goPlaySelectList.Count - 1];
                break;
            case MenuPhase.Option_Update:
                // Option情報保存
                AppCommon.Update_And_SaveGameData();
                m_goOption_Select = m_goOptionBtns[m_goOptionBtns.Count - 1].gameObject;
                break;
            case MenuPhase.Account_Update:
                m_goAccount_Select = m_goAccountSelectList[m_goAccountSelectList.Count - 1];
                break;
        }

        ChangeMenuPhase(MenuPhase.Loggedin_Start);
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ログアウトボタン押下
    /// </summary>
    private void OnClick_Logout()
    {
        // 戻るボタン無効化
        m_isChecking = true;
        // 入力不可にする
        m_AccountTouchArea.SetActive(true);

        // 確認ダイアログ表示で、「はい」を選択時、ログアウトを実行する
        SetConfirmDialog(true, LogoutConfirmMessage, DialogType.Type5, MenuPhase.Start_Start, MenuPhase.Logout_Update, MenuPhase.Account_Start);

        m_goAccount_Select = m_goAccountSelectList[5];
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// ユーザー削除ボタン押下
    /// </summary>
    private void OnClick_Delete()
    {
        // 戻るボタン無効化
        m_isChecking = true;
        // 入力不可にする
        m_AccountTouchArea.SetActive(true);

        // 確認ダイアログ表示で、「はい」を選択時、ユーザー情報（firebase.storage、firebase.auth）を削除する
        SetConfirmDialog(true, DeleteConfirmMessage, DialogType.Type4, MenuPhase.Start_Start, MenuPhase.Error_DeleteUserInfo_Start, MenuPhase.Account_Start);

        m_goAccount_Select = m_goAccountSelectList[6];
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    /// <summary>
    /// 個別のオプションボタン表示
    /// </summary>
    /// <param name="optionIdx"></param>
    /// <param name="index"></param>
    private void SetOptionBtn(int optionIdx, int index)
    {
        Image img1 = m_goOptionBtns[optionIdx].transform.Find("Img_1").GetComponent<Image>();
        TextMeshProUGUI text1 = m_goOptionBtns[optionIdx].transform.Find("Img_1").GetComponentInChildren<TextMeshProUGUI>();
        if (optionIdx == OptionMetronomeBPMIndex)
        {
            m_OptionBPMSlider.value = index;
            m_OptionBPMText.text = index.ToString();
        }
        else
        {
            Image img2 = m_goOptionBtns[optionIdx].transform.Find("Img_2").GetComponent<Image>();
            TextMeshProUGUI text2 = m_goOptionBtns[optionIdx].transform.Find("Img_2").GetComponentInChildren<TextMeshProUGUI>();
            if (ColorUtility.TryParseHtmlString(OptionHtmlColor[index == 0 ? 0 : 1], out Color img1Col))
            {
                img1.color = img1Col;
            }
            if (ColorUtility.TryParseHtmlString(OptionHtmlColor[index == 1 ? 0 : 1], out Color img2Col))
            {
                img2.color = img2Col;
            }
            Color col = text1.color;
            col.a = index == 0 ? 1.0f : 0.5f;
            text1.color = col;
            col = text2.color;
            col.a = index == 1 ? 1.0f : 0.5f;
            text2.color = col;
        }
    }

    /// <summary>
    /// 全オプションボタンの表示
    /// </summary>
    private void SetOptionBtns()
    {
        // 戻るボタン分を除外
        for (int i = 0; i < m_goOptionBtns.Count - 1; ++i)
        {
            UpdateOptionBtns(i, false);
        }
    }

    /// <summary>
    /// オプションボタン更新
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="isUpdate"></param>
    private void UpdateOptionBtns(int idx, bool isUpdate)
    {
        int val = 0;
        bool flag = false;

        switch (idx)
        {
            // 接続モード
            case 0:
                val = (int)AppData.ConnectMode;
                break;
            // コントローラーモード
            case 1:
                val = (int)AppData.ControllerMode;
                break;
            // グリッド表示
            case 3:
                flag = AppData.GridDisplay;
                break;
            // パッド表示
            case 4:
                flag = AppData.PadDisplay;
                break;
            // ドローン飛行音
            case 5:
                flag = AppData.DroneFlightSound;
                break;
            // メトロノーム飛行音
            case 6:
                flag = AppData.MetronomeFlightSound;
                break;
            // メトロノームBPM
            case 7:
                val = AppData.MetronomeBPM;
                isUpdate = false;
                break;
        }

        if (isUpdate)
        {
            if (val == 0) val = 1;
            else val = 0;
            flag = !flag;
        }

        switch (idx)
        {
            // 接続モード
            case 0:
                AppData.SetConnectMode((AppData.ConnectM)val);
                SetOptionBtn(idx, (int)AppData.ConnectMode);
                break;
            // コントローラーモード
            case 1:
                AppData.SetControllerMode((AppData.ControllerM)val);
                SetOptionBtn(idx, (int)AppData.ControllerMode);
                break;
            // コントローラー設定
            case 2:
                if (isUpdate)
                {
                    if (AppData.ControllerMode == AppData.ControllerM.GamePad)
                    {
                        m_trOptionGamepad.gameObject.SetActive(true);
                        // 戻るボタンが選択してある場合
                        if (m_goOption_Gamepad == m_OptionGamepadBtns[m_OptionGamepadBtns.Count - 1].gameObject)
                        {
                            m_goOption_Gamepad = m_OptionGamepadBtns[0].gameObject;
                            // ゲームパッド設定のスクロール位置を最初に戻しておく
                            SetContentPosition(m_trOptionGamepad.Find("Scroll View/Viewport/Content").gameObject, ContentType.Type3, 0);
                        }
                        StartCoroutine(crGamepadDelayDisp());
                    }
                    else
                    {
                        m_trOptionKeyboard.gameObject.SetActive(true);
                        // 戻るボタンが選択してある場合
                        if (m_goOption_Keyboard == m_OptionKeyboardBtns[m_OptionKeyboardBtns.Count - 1].gameObject)
                        {
                            m_goOption_Keyboard = m_OptionKeyboardBtns[0].gameObject;
                            // キーボード設定のスクロール位置を最初に戻しておく
                            SetContentPosition(m_trOptionKeyboard.Find("Scroll View/Viewport/Content").gameObject, ContentType.Type2, 0);
                        }
                        StartCoroutine(crKeyboardDelayDisp());
                    }
                }
                break;
            // グリッド表示
            case 3:
                AppData.SetGridDisplay(flag);
                SetOptionBtn(idx, AppData.GridDisplay ? 0 : 1);
                break;
            // パッド表示
            case 4:
                AppData.SetPadDisplay(flag);
                SetOptionBtn(idx, AppData.PadDisplay ? 0 : 1);
                break;
            // ドローン飛行音
            case 5:
                AppData.SetDroneFlightSound(flag);
                SetOptionBtn(idx, AppData.DroneFlightSound ? 0 : 1);
                break;
            // ドローン飛行音
            case 6:
                AppData.SetMetronomeFlightSound(flag);
                SetOptionBtn(idx, AppData.MetronomeFlightSound ? 0 : 1);
                break;
            // メトロノームBPM
            case 7:
                AppData.SetMetronomeBPM(val);
                SetOptionBtn(idx, AppData.MetronomeBPM);
                break;
        }
    }

    /// <summary>
    /// オプションにあるボタン押下
    /// </summary>
    /// <param name="idx"></param>
    private void OnClick_OptionBtn(int idx)
    {
        // メトロノームBPM以外
        if (idx != 7)
        {
            UpdateOptionBtns(idx, true);
            // SE:
            MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
        }
    }

    private IEnumerator crKeyboardDelayDisp()
    {
        yield return null;
        SetSelectGameObject(m_goOption_Keyboard);
    }

    private void SetKeyboardBtnText(int index)
    {
        GameObject goBtn = m_OptionKeyboardBtns[index].gameObject;
        TextMeshProUGUI text = goBtn.transform.Find("Text2").GetComponent<TextMeshProUGUI>();
        text.text = AppData.KeyCodes[index].ToString();
    }

    private void SetKeyboardBtnsText()
    {
        for (int i = 0; i < (int)AppData.Action.Max; ++i)
        {
            SetKeyboardBtnText(i);
        }
    }

    /// <summary>
    /// キーボードに設定されているボタンの登録
    /// </summary>
    private void SetOptionKeyboardBtns()
    {
        Transform baseTr = m_trOptionKeyboard.Find("Scroll View/Viewport/Content");
        m_OptionKeyboardBtns = new List<Button>((int)AppData.Action.Max + 2);
        Button btn;
        for (int i = 0; i < (int)AppData.Action.Max; ++i)
        {
            btn = baseTr.Find("Btn_Key" + i).GetComponentInChildren<Button>();
            int i1 = i;
            btn.onClick.AddListener(() => OnClick_OptionKeyboardBtn(i1));
            m_OptionKeyboardBtns.Add(btn);
            SetTapOverComponent(btn.gameObject, Select.OptionKeyboard);
        }
        btn = baseTr.Find("Btn_Initialize").GetComponentInChildren<Button>();
        btn.onClick.AddListener(OnClick_KeyboardInitialize);
        m_OptionKeyboardBtns.Add(btn);
        SetTapOverComponent(btn.gameObject, Select.OptionKeyboard);

        btn = m_trOptionKeyboard.Find("Btn_Back").GetComponent<Button>();
        btn.onClick.AddListener(OnClick_BackOption);
        m_OptionKeyboardBtns.Add(btn);
        SetTapOverComponent(btn.gameObject, Select.OptionKeyboard);

        m_goOption_Keyboard = m_OptionKeyboardBtns[0].gameObject;

        // キーボード設定の初期表示
        SetKeyboardBtnsText();
    }

    private void OnClick_OptionKeyboardBtn(int idx)
    {
        m_SelectedKeyboardIndex = idx;

        // 入力無効
        m_isDisableInputAll = true;

        // ダイアログタイマー無効
        SetDialog(true, KeySetMessage, DialogType.Type1, MenuPhase.None, MenuPhase.None, -1);

        m_MenuPhase = MenuPhase.Dialog_Keyboard;
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    private void OnClick_KeyboardInitialize()
    {
        // ダイアログタイマー無効
        SetConfirmDialog(true, KeyInitializeConfirmMessage, DialogType.Type6, MenuPhase.Option_Update, MenuPhase.Option_Update, MenuPhase.Option_Update);
    }

    private void OnClick_BackOption()
    {
        // キーボード・ゲームパッド設定情報保存
        AppCommon.Update_And_SaveGameData();
        m_trOptionKeyboard.gameObject.SetActive(false);
        m_trOptionGamepad.gameObject.SetActive(false);
        ChangeMenuPhase(MenuPhase.Option_Start);
        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    private IEnumerator crGamepadDelayDisp()
    {
        yield return null;
        SetSelectGameObject(m_goOption_Gamepad);
    }

    private void SetGamepadBtnText(int index)
    {
        GameObject goBtn = m_OptionGamepadBtns[index].gameObject;
        TextMeshProUGUI text = goBtn.transform.Find("Text2").GetComponent<TextMeshProUGUI>();
        text.text = YnInputSystem.GetBindingDisplayString((AppData.PadAction)index);
    }

    private void SetGamepadBtnsText()
    {
        for (int i = 0; i < (int)AppData.PadAction.Max; ++i)
        {
            SetGamepadBtnText(i);
        }
    }

    private void SetOptionGamepadBtns()
    {
        Transform baseTr = m_trOptionGamepad.Find("Scroll View/Viewport/Content");
        m_OptionGamepadBtns = new List<Button>((int)AppData.PadAction.Max + 2);
        Button btn;
        for (int i = 0; i < (int)AppData.PadAction.Max; ++i)
        {
            btn = baseTr.Find("Btn_Pad" + i).GetComponentInChildren<Button>();
            int i1 = i;
            btn.onClick.AddListener(() => OnClick_OptionGamepadBtn(i1));
            m_OptionGamepadBtns.Add(btn);
            SetTapOverComponent(btn.gameObject, Select.OptionGamepad);
        }
        btn = baseTr.Find("Btn_Initialize").GetComponentInChildren<Button>();
        btn.onClick.AddListener(OnClick_GamepadInitialize);
        m_OptionGamepadBtns.Add(btn);
        SetTapOverComponent(btn.gameObject, Select.OptionGamepad);

        btn = m_trOptionGamepad.Find("Btn_Back").GetComponent<Button>();
        btn.onClick.AddListener(OnClick_BackOption);
        m_OptionGamepadBtns.Add(btn);
        SetTapOverComponent(btn.gameObject, Select.OptionGamepad);

        m_goOption_Gamepad = m_OptionGamepadBtns[0].gameObject;

        // ゲームパッド設定の初期表示
        SetGamepadBtnsText();
    }

    private void OnClick_OptionGamepadBtn(int idx)
    {
        // 入力無効
        m_isDisableInputAll = true;

        // ダイアログタイマー無効
        SetDialog(true, PadSetMessage, DialogType.Type1, MenuPhase.None, MenuPhase.None, -1);

        m_MenuPhase = MenuPhase.Dialog_Gamepad;

        YnInputSystem.StartRebinding((AppData.PadAction)idx);

        // SE:
        MG_Mediator.GetAudio().PlaySe(AudioId.count.ToString(), false);
    }

    private void OnClick_GamepadInitialize()
    {
        // ダイアログタイマー無効
        SetConfirmDialog(true, PadInitializeConfirmMessage, DialogType.Type7, MenuPhase.Option_Update, MenuPhase.Option_Update, MenuPhase.Option_Update);
    }

    //================================================
    // [///] Private Method
    //================================================

    // STATUSテキスト設定
    private void SetStatusText(string dispStr)
    {
        m_StatusText.text = dispStr;
    }

    // TODO:ユーザー作成時で必須項目の入力確認 ※今のところはメアドとパスワード
    private bool IsEnableCreateCheck()
    {
        bool isEmail = !string.IsNullOrEmpty(m_CreateEmailStr);
        bool isPassword = !string.IsNullOrEmpty(m_CreatePasswordStr) && m_CreatePasswordStr.Length >= 8;    // 半角英数字
        return isEmail && isPassword;
    }

    // ログイン必須項目の入力確認
    private bool IsEnableLoginCheck()
    {
        bool isEmail = !string.IsNullOrEmpty(m_LoginEmailStr);
        bool isPassword = !string.IsNullOrEmpty(m_LoginPasswordStr) && m_LoginPasswordStr.Length >= 8;    // 半角英数字;
        return isEmail && isPassword;
    }

    // ユーザー情報変更時の入力確認
    private bool IsEnableChangeCheck()
    {
#if UNITY_WEBGL && !UNITY_EDITOR && !DISABLE_FIREBASE
        bool isName = m_ReceiveUserInfo.displayName != m_AccountNameStr;
        bool isEmail = m_ReceiveUserInfo.email != m_AccountEmailStr;
#else
        bool isName = !string.IsNullOrEmpty(m_AccountNameStr);   // TODO:日本語の時の文字数制限どうするか…
        bool isEmail = !string.IsNullOrEmpty(m_AccountEmailStr);
#endif
        bool isPassword = !string.IsNullOrEmpty(m_AccountPasswordStr) && m_AccountPasswordStr.Length >= 8;    // 半角英数字
        bool isNowPassword = !string.IsNullOrEmpty(m_AccountNowPasswordStr) && m_AccountNowPasswordStr.Length >= 8;    // 半角英数字

        if (m_AccountPasswordStr == m_AccountNowPasswordStr)
        {
            isPassword = false;
        }

        //Debug.Log("IsEnableChangeCheck:" + ((isName || isEmail || isPassword) && isNowPassword).ToString() );

        return (isName || isEmail || isPassword) && isNowPassword;
    }

    // リプレイデータの有無
    private bool isEnableReplayData()
    {
        return AppData.m_ReceiveReplayCountStorageInfo != null && AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count > 0;
    }

    // サインイン
    private void SignIn()
    {
        SetStatusText("SignIn_Start");
        m_isSuccess = false;
        m_isReceived = false;
        FB_SignInJS(GetInstanceID(), OnSignIn, m_LoginEmailStr, m_LoginPasswordStr);
    }

    // サインイン成功時
    private void SuccessSignIn(string outputStr)
    {
        m_ReceiveUserInfo = JsonUtility.FromJson<ReceiveUserClass>(outputStr);
        Debug.Log("DisplayName:" + m_ReceiveUserInfo.displayName);
        Debug.Log("Email:" + m_ReceiveUserInfo.email);
        Debug.Log("UID:" + m_ReceiveUserInfo.uid);

        m_isReceived = true;

        SetStatusText("SignIn_Success:" + m_ReceiveUserInfo.displayName);
    }

    // サインインでのエラー時
    private void ErrorSignIn(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        string dispStr;

        switch (outputStr)
        {
            case "auth/invalid-email":
                dispStr = "そのメールアドレスは無効です。";
                break;
            case "auth/user-disabled":
                dispStr = "そのメールアドレスに対応する\nユーザーが無効になっています。";
                break;
            case "auth/user-not-found":
                dispStr = "そのメールアドレスに対応する\nユーザーが見付かりません。";
                break;
            case "auth/wrong-password":
                dispStr = "そのメールアドレスに対応する\nパスワードが間違っています。";
                break;
            case "auth/invalid-login-credentials":
                dispStr = "メールアドレス、又はパスワードが\n間違っています。";
                break;
            case "auth/too-many-requests":
                dispStr = "ログイン試行が何度も失敗したため\nこのアカウントへのアクセスは\n一時的に無効になっています。\n後でもう一度試してみてください。";
                break;
            default:
                dispStr = "エラーが発生しました。\nもう一度最初からやり直してください。";
                break;
        }

        SetDialog(true, dispStr, DialogType.Type1, MenuPhase.Login_Start);
    }

    // ユーザー登録
    private void RegisterUser()
    {
        SetStatusText("RegisterUser_Start");
        m_isSuccess = false;
        m_isReceived = false;
        FB_RegisterUserJS(GetInstanceID(), OnRegisterUser, m_CreateEmailStr, m_CreatePasswordStr);
    }

    // ユーザー登録成功時
    private void SuccessRegisterUser(string outputStr)
    {
        m_ReceiveUserInfo = JsonUtility.FromJson<ReceiveUserClass>(outputStr);
        Debug.Log("DisplayName:" + m_ReceiveUserInfo.displayName);
        Debug.Log("Email:" + m_ReceiveUserInfo.email);
        Debug.Log("UID:" + m_ReceiveUserInfo.uid);

        m_isReceived = true;

        SetStatusText("RegisterUser_Success:[" + m_ReceiveUserInfo.email+"]");
    }

    // ユーザー登録でのエラー時
    private void ErrorRegisterUser(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        string dispStr;

        switch (outputStr)
        {
            case "auth/email-already-in-use":
                dispStr = "そのメールアドレスは使われています。";
                break;
            case "auth/invalid-email":
                dispStr = "そのメールアドレスは無効です。";
                break;
            case "auth/operation-not-allowed":
                dispStr = "プロセスが許可されていません。";
                break;
            case "auth/weak-password":
                dispStr = "パスワードの強度が不足しています。";
                break;
            default:
                dispStr = "エラーが発生しました。\nもう一度最初からやり直してください。";
                break;
        }

        SetDialog(true, dispStr, DialogType.Type2, MenuPhase.Create_Start);
    }

    // ストレージにユーザー情報を作成する
    private void CreateUserInfoStorage()
    {
        SetStatusText("CreateUserInfoStorage_Start");
        m_isSuccess = false;
        m_isReceived = false;
        FB_CreateUserInfoStorageJS(GetInstanceID(), OnCreateUserInfoStorage, m_ReceiveUserInfo.uid, m_CreateGenderStr, m_CreateAgeStr, m_CreateWorkStr);
    }

    // ストレージへのユーザー情報作成成功時
    private void SuccessCreateUserInfoStorage(string outputStr)
    {
        m_ReceiveUserStorageInfo = JsonUtility.FromJson<ReceiveUserStorageClass>(outputStr);
        Debug.Log("Gender:" + m_ReceiveUserStorageInfo.gender);
        Debug.Log("Age:" + m_ReceiveUserStorageInfo.age);
        Debug.Log("Work:" + m_ReceiveUserStorageInfo.work);

        m_isReceived = true;

        SetStatusText("CreateUserInfoStorage_Success:" + m_ReceiveUserStorageInfo.gender);
    }

    // ストレージへのユーザー情報作成エラー時
    private void ErrorCreateUserInfoStorage(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        m_isReceived = true;

        string dispStr;

        switch (outputStr)
        {
            default:
                dispStr = "エラーが発生しました。\nもう一度最初からやり直してください。";
                break;
        }

        // ダイアログ表示後、ユーザー情報（firebase.auth）を削除する
        SetDialog(true, dispStr, DialogType.Type2, MenuPhase.Create_Start, MenuPhase.Error_DeleteUser_Start);
    }

    // ストレージからユーザー情報を取得
    private void GetUserInfoStorage()
    {
        SetStatusText("GetUserInfoStorage_Start");
        m_isSuccess = false;
        m_isReceived = false;
        FB_GetUserInfoStorageJS(GetInstanceID(), OnGetUserInfoStorage, m_ReceiveUserInfo.uid);
    }

    // ストレージからユーザー情報取得成功時
    private void SuccessGetUserInfoStorage(string outputStr)
    {
        m_ReceiveUserStorageInfo = JsonUtility.FromJson<ReceiveUserStorageClass>(outputStr);
        Debug.Log("Gender:" + m_ReceiveUserStorageInfo.gender);
        Debug.Log("Age:" + m_ReceiveUserStorageInfo.age);
        Debug.Log("Work:" + m_ReceiveUserStorageInfo.work);

        m_isReceived = true;

        SetStatusText("GetUserInfoStorage_Success:" + m_ReceiveUserStorageInfo.gender);
    }

    // ストレージからユーザー情報取得エラー時
    private void ErrorGetUserInfoStorage(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        m_isReceived = true;

        string dispStr;

        switch (outputStr)
        {
            default:
                dispStr = "エラーが発生しました。\nもう一度最初からやり直してください。";
                break;
        }

        switch (m_MenuPhase)
        {
            // サインイン時
            case MenuPhase.Login_UserInfo_Start:
            case MenuPhase.Login_UserInfo_Update:
                // もう一度最初から
                SetDialog(true, dispStr, DialogType.Type1, MenuPhase.Login_Start);
                break;
            // 開始時
            case MenuPhase.Start_UserInfo_Start:
            case MenuPhase.Start_UserInfo_Update:
                // エラーが起きても無視
                break;
        }
    }

    // ストレージからユーザー情報を削除
    private void DeleteUserInfoStorage()
    {
        SetStatusText("DeleteUserInfoStorage_Start");
        m_isSuccess = false;
        m_isReceived = false;
        FB_DeleteUserInfoStorageJS(GetInstanceID(), OnDeleteUserInfoStorage, m_ReceiveUserInfo.uid);
    }

    // ストレージからユーザー情報削除成功時
    private void SuccessDeleteUserInfoStorage(string outputStr)
    {
        m_ReceiveUserStorageInfo = null;

        m_isReceived = true;

        SetStatusText("DeleteUserInfoStorage_Success");
    }

    // ストレージからユーザー情報削除エラー時
    private void ErrorDeleteUserInfoStorage(string outputStr)
    {
        m_ReceiveUserStorageInfo = null;

        Debug.Log("ErrorCode:" + outputStr);

        m_isReceived = true;

        SetStatusText("DeleteUserInfoStorage_Error:" + outputStr);
    }

    // ユーザープロファイル更新
    private void UpdateProfile()
    {
        SetStatusText("UpdateProfile_Start");
        m_isSuccess = false;
        m_isReceived = false;
        FB_UpdateProfileJS(GetInstanceID(), OnUpdateProfile, m_AccountNameStr);
    }

    // プロファイル更新成功時
    private void SuccessUpdateProfile(string outputStr)
    {
        m_ReceiveUserInfo = JsonUtility.FromJson<ReceiveUserClass>(outputStr);
        Debug.Log("DisplayName:" + m_ReceiveUserInfo.displayName);
        Debug.Log("Email:" + m_ReceiveUserInfo.email);
        Debug.Log("UID:" + m_ReceiveUserInfo.uid);

        m_isReceived = true;

        SetStatusText("UpdateProfile_Success:" + m_ReceiveUserInfo.displayName);
    }

    // プロファイル更新でのエラー時
    private void ErrorUpdateProfile(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        string dispStr;

        switch (outputStr)
        {
            default:
                dispStr = "エラーが発生しました。\nもう一度最初からやり直してください。";
                break;
        }

        switch (m_MenuPhase)
        {
            // ユーザー作成時
            case MenuPhase.Create_Profile_Update:
                // ダイアログ表示後、ユーザー情報（firebase.storage、firebase.auth）を削除する
                SetDialog(true, dispStr, DialogType.Type2, MenuPhase.Create_Start, MenuPhase.Error_DeleteUserInfo_Start);
                break;
            // ユーザー情報変更時
            case MenuPhase.Account_Proc:
                // TODO:仮設定
                SetDialog(true, dispStr, DialogType.Type2, MenuPhase.Account_Start);
                break;
        }
    }

    // ユーザー登録確認メール送信
    private void SendEmailVerification()
    {
        SetStatusText("SendEmailVerification_Start");
        m_isSuccess = false;
        m_isReceived = false;
        FB_SendEmailVerificationJS(GetInstanceID(), OnSendEmailVerification);
    }

    // ユーザー登録確認メール送信成功時
    private void SuccessSendEmailVerification(string outputStr)
    {
        var newTestClass = JsonUtility.FromJson<ReceiveUserClass>(outputStr);
        Debug.Log("DisplayName:" + newTestClass.displayName);
        Debug.Log("Email:" + newTestClass.email);
        Debug.Log("UID:" + newTestClass.uid);

        m_isReceived = true;

        SetStatusText("SendEmailVerification_Success:" + newTestClass.email);
    }

    // ユーザー登録確認メール送信でのエラー時
    private void ErrorSendEmailVerification(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        string dispStr;

        switch (outputStr)
        {
            default:
                dispStr = "エラーが発生しました。\nもう一度最初からやり直してください。";
                break;
        }

        // ここでのエラーはないようだが、一応設定しておく

        // ダイアログ表示後、ユーザー情報（firebase.storage、firebase.auth）を削除する
        SetDialog(true, dispStr, DialogType.Type2, MenuPhase.Create_Start, MenuPhase.Error_DeleteUserInfo_Start);
    }

    // ユーザー作成時のメール認証
    private void CreateEmailVerified()
    {
        SetStatusText("Confirm_SendEmailVerified");
        m_isSuccess = false;
        m_isReceived = false;
        StartCoroutine(crCreateEmailVerified());
    }

    // 成功orエラーをコルーチンで待つ
    private IEnumerator crCreateEmailVerified()
    {
        while (!m_isCancelFromUser)
        {
            m_isReceived = false;
            FB_CreateEmailVerifiedJS(GetInstanceID(), OnCreateEmailVerified);
            yield return new WaitUntil(() => m_isReceived);
            // 認証成功していたら終了
            if (m_isSuccess) break;
            // 連続して確認すると負荷がかかるので、ある程度の時間をおく
            float time = 0.0f;
            while (time < CreateEmailVerifiedWait)
            {
                time += Time.deltaTime;
                if (m_isCancelFromUser) break;
                yield return null;
            }
        }
    }

    // メール承認確認時
    private void SuccessCreateEmailVerified(string outputStr)
    {
        m_ReceiveUserInfo = JsonUtility.FromJson<ReceiveUserClass>(outputStr);
        Debug.Log("DisplayName:" + m_ReceiveUserInfo.displayName);
        Debug.Log("Email:" + m_ReceiveUserInfo.email);
        Debug.Log("UID:" + m_ReceiveUserInfo.uid);

        m_isReceived = true;

        SetStatusText("CreateEmailVerified_Success:" + m_ReceiveUserInfo.email);
    }

    // メール承認未確認時
    private void ErrorCreateEmailVerified(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        m_isReceived = true;

        //SetStatusText("CreateEmailVerified_Error:" + outputStr);
    }


    // ユーザー削除
    private void DeleteUser()
    {
        SetStatusText("DeleteUser_Start");
        m_isSuccess = false;
        m_isReceived = false;
        FB_DeleteUserJS(GetInstanceID(), OnDeleteUser, m_ReceiveUserInfo.email, m_DeleteUserPasswordStr);
    }

    // ユーザー削除成功時
    private void SuccessDeleteUser(string outputStr)
    {
        m_ReceiveUserInfo = null;

        m_isReceived = true;

        SetStatusText("DeleteUser_Success:user deleted");
    }

    // ユーザー削除でのエラー時
    private void ErrorDeleteUser(string outputStr)
    {
        m_ReceiveUserInfo = null;

        Debug.Log("ErrorCode:" + outputStr);

        m_isReceived = true;

        SetStatusText("DeleteUser_Error:" + outputStr);
    }

    // ユーザーのメールアドレス更新
    private void UpdateEmail()
    {
        SetStatusText("UpdateEmail_Start");
        m_isSuccess = false;
        m_isReceived = false;
        FB_VerifyBeforeUpdateEmailJS(GetInstanceID(), OnUpdateEmail, m_ReceiveUserInfo.email, m_AccountNowPasswordStr, m_AccountEmailStr);
    }

    // ユーザーのメールアドレス更新成功時
    private void SuccessUpdateEmail(string outputStr)
    {
        m_ReceiveUserInfo = JsonUtility.FromJson<ReceiveUserClass>(outputStr);
        Debug.Log("DisplayName:" + m_ReceiveUserInfo.displayName);
        Debug.Log("Email:" + m_ReceiveUserInfo.email);
        Debug.Log("UID:" + m_ReceiveUserInfo.uid);

        m_isReceived = true;

        m_isChangedEmail = true;

        SetStatusText("UpdateEmail_Success:" + m_ReceiveUserInfo.email);
    }

    // ユーザーのメールアドレス更新でのエラー時
    private void ErrorUpdateEmail(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        m_isReceived = true;

        string dispStr;

        switch (outputStr)
        {
            default:
                dispStr = "エラーが発生しました。\nもう一度最初からやり直してください。";
                break;
        }

        // TODO:仮設定
        SetDialog(true, dispStr, DialogType.Type2, MenuPhase.Account_Start);
    }


    // メールアドレス変更時のメール認証
    private void ChangeEmailVerified()
    {
        SetStatusText("ChangeEmailVerified_Start");
        m_isSuccess = false;
        m_isReceived = false;
        StartCoroutine(crChangeEmailVerified());
    }

    // 成功orエラーをコルーチンで待つ
    private IEnumerator crChangeEmailVerified()
    {
        m_isEndChangeEmailVerified = false;
        while (!m_isCancelFromUser)
        {
            m_isReceived = false;
            // ユーザー情報が新しいメアドに変更されるまで待つ
            FB_ChangeEmailVerifiedJS(GetInstanceID(), OnChangeEmailVerified, m_AccountEmailStr, m_AccountNowPasswordStr);
            yield return new WaitUntil(() => m_isReceived);
            // 新しいメアドに変更されていたら終了
            if (m_isSuccess) break;
            // 連続して確認すると負荷がかかるので、ある程度の時間をおく
            float time = 0.0f;
            while (time < ChangeEmailVerifiedWait)
            {
                time += Time.deltaTime;
                if (m_isCancelFromUser)
                {
                    Debug.Log("--- change email verified cancel ---");
                    break;
                }
                yield return null;
            }
        }
        m_isEndChangeEmailVerified = true;
    }

    // メール承認確認時
    private void SuccessChangeEmailVerified(string outputStr)
    {
        m_ReceiveUserInfo = JsonUtility.FromJson<ReceiveUserClass>(outputStr);
        Debug.Log("DisplayName:" + m_ReceiveUserInfo.displayName);
        Debug.Log("Email:" + m_ReceiveUserInfo.email);
        Debug.Log("UID:" + m_ReceiveUserInfo.uid);

        m_isReceived = true;

        SetStatusText("ChangeEmailVerified_Success:" + m_ReceiveUserInfo.email);
    }

    // メール承認未確認時
    private void ErrorChangeEmailVerified(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        m_isReceived = true;

        //SetStatusText("CreateEmailVerified_Error:" + outputStr);
    }


    private bool ChangeUserInfo()
    {
        bool flag = false;
        if (m_ReceiveUserInfo.displayName != m_AccountNameStr)
        {
            UpdateProfile();
            flag = true;
        }
        else
        if (m_ReceiveUserInfo.email != m_AccountEmailStr)
        {
            UpdateEmail();
            flag = true;
        }
        else
        if (m_AccountPasswordStr != m_AccountNowPasswordStr)
        {
            UpdatePassword();
            flag = true;
        }
        if (flag)
        {
            m_MenuPhase = MenuPhase.Account_Proc;
        }
        return flag;
    }


    // カレントユーザー取得
    private void GetCurrentUser()
    {
        SetStatusText("GetCurrentUser_Start");
        m_isReceived = false;
        m_isSuccess = false;
        FB_GetCurrentUserJS(GetInstanceID(), OnGetCurrentUser);
    }

    // 
    private void SuccessGetCurrentUser(string outputStr)
    {
        m_ReceiveUserInfo = JsonUtility.FromJson<ReceiveUserClass>(outputStr);
        Debug.Log("DisplayName:" + m_ReceiveUserInfo.displayName);
        Debug.Log("Email:" + m_ReceiveUserInfo.email);
        Debug.Log("UID:" + m_ReceiveUserInfo.uid);

        m_isReceived = true;

        SetStatusText("GetCurrentUser_Success:" + m_ReceiveUserInfo.displayName);
    }

    // 
    private void ErrorGetCurrentUser(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        m_isReceived = true;

        SetStatusText("GetCurrentUser_Error:" + outputStr);
    }

    // カレントユーザー取得（リロード付き）
    private void GetCurrentUserReload()
    {
        SetStatusText("GetCurrentUserReload_Start");
        m_isReloadReceived = false;
        m_isReloadSuccess = false;
        m_isReloadExpired = false;
        FB_GetCurrentUserReloadJS(GetInstanceID(), OnGetCurrentUserReload);
    }

    // 
    private void SuccessGetCurrentUserReload(string outputStr)
    {
        m_ReceiveUserInfoReload = JsonUtility.FromJson<ReceiveUserClass>(outputStr);
        Debug.Log("DisplayName:" + m_ReceiveUserInfoReload.displayName);
        Debug.Log("Email:" + m_ReceiveUserInfoReload.email);
        Debug.Log("UID:" + m_ReceiveUserInfoReload.uid);

        m_isReloadReceived = true;

        SetStatusText("GetCurrentUserReload_Success:" + m_ReceiveUserInfoReload.displayName);
    }

    // 
    private void ErrorGetCurrentUserReload(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        m_isReloadReceived = true;

        switch (outputStr)
        {
            case "auth/user-token-expired":
                // 指定された Firebase ID トークンは期限切れです
                m_isReloadExpired = true;
                break;
            default:
                break;
        }
    }

    // サインアウト
    private void SignOut()
    {
        SetStatusText("SignOut_Start");
        m_isSuccess = false;
        m_isReceived = false;
        FB_SignOutJS(GetInstanceID(), OnSignOut);
    }

    // サインアウト成功時
    private void SuccessSignOut(string outputStr)
    {
        m_ReceiveUserInfo = null;

        m_isReceived = true;

        SetStatusText("SignOut_Success");
    }

    // サインアウトでのエラー時
    private void ErrorSignOut(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        //string dispStr;
        //
        //switch (outputStr)
        //{
        //    case "auth/invalid-email":
        //        dispStr = "そのメールアドレスは無効です。";
        //        break;
        //    case "auth/user-disabled":
        //        dispStr = "そのメールアドレスに対応する\nユーザーが無効になっています。";
        //        break;
        //    case "auth/user-not-found":
        //        dispStr = "そのメールアドレスに対応する\nユーザーが見付かりません。";
        //        break;
        //    case "auth/wrong-password":
        //        dispStr = "そのメールアドレスに対応する\nパスワードが間違っています。";
        //        break;
        //    case "auth/invalid-login-credentials":
        //        dispStr = "メールアドレス、又はパスワードが\n間違っています。";
        //        break;
        //    default:
        //        dispStr = "エラーが発生しました。\nもう一度最初からやり直してください。";
        //        break;
        //}
        //
        //SetDialog(true, dispStr, DialogType.Type1, MenuPhase.Login_Start);

        SetStatusText(outputStr);
    }

    // ユーザーのパスワード更新
    private void UpdatePassword()
    {
        SetStatusText("UpdatePassword_Start");
        m_isSuccess = false;
        m_isReceived = false;
        FB_UpdatePasswordJS(GetInstanceID(), OnUpdatePassword, m_AccountEmailStr, m_AccountNowPasswordStr, m_AccountPasswordStr);
    }

    // ユーザーのパスワード更新成功時
    private void SuccessUpdatePassword(string outputStr)
    {
        m_ReceiveUserInfo = JsonUtility.FromJson<ReceiveUserClass>(outputStr);
        Debug.Log("DisplayName:" + m_ReceiveUserInfo.displayName);
        Debug.Log("Email:" + m_ReceiveUserInfo.email);
        Debug.Log("UID:" + m_ReceiveUserInfo.uid);

        m_isReceived = true;

        // 更新されたパスワードを保存
        AppData.SetPassword(m_AccountPasswordStr);
        AppCommon.Update_And_SaveGameData();
        // 連続で起動しないように現在のパスワードを変更しておく
        m_AccountNowPasswordStr = m_AccountPasswordStr;
        // ログイン時のパスワードも変更しておく
        m_LoginPasswordStr = m_AccountPasswordStr;

        SetStatusText("UpdatePassword_Success:" + m_ReceiveUserInfo.displayName);
    }

    // ユーザーのパスワード更新でのエラー時
    private void ErrorUpdatePassword(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        m_isReceived = true;

        string dispStr;

        switch (outputStr)
        {
            case "auth/invalid-email":
                dispStr = "そのメールアドレスは無効です。";
                break;
            case "auth/user-disabled":
                dispStr = "そのメールアドレスに対応する\nユーザーが無効になっています。";
                break;
            case "auth/user-not-found":
                dispStr = "そのメールアドレスに対応する\nユーザーが見付かりません。";
                break;
            case "auth/wrong-password":
                dispStr = "そのメールアドレスに対応する\nパスワードが間違っています。";
                break;
            case "auth/invalid-login-credentials":
                dispStr = "メールアドレス、又はパスワードが\n間違っています。";
                break;
            case "auth/too-many-requests":
                dispStr = "ログイン試行が何度も失敗したため\nこのアカウントへのアクセスは\n一時的に無効になっています。\n後でもう一度試してみてください。";
                break;
            default:
                dispStr = "エラーが発生しました。\nもう一度最初からやり直してください。";
                break;
        }

        // TODO:仮設定
        SetDialog(true, dispStr, DialogType.Type2, MenuPhase.Account_Start);
    }


    // ユーザー再認証
    private void ReauthenticateWithCredential()
    {
        SetStatusText("ReauthenticateWithCredential_Start");
        m_isReceived = false;
        m_isSuccess = false;
        FB_ReauthenticateWithCredentialJS(GetInstanceID(), OnReauthenticateWithCredential, m_LoginEmailStr, m_LoginPasswordStr);
    }

    // 
    private void SuccessReauthenticateWithCredential(string outputStr)
    {
        m_ReceiveUserInfo = JsonUtility.FromJson<ReceiveUserClass>(outputStr);
        Debug.Log("DisplayName:" + m_ReceiveUserInfo.displayName);
        Debug.Log("Email:" + m_ReceiveUserInfo.email);
        Debug.Log("UID:" + m_ReceiveUserInfo.uid);

        m_isReceived = true;

        SetStatusText("ReauthenticateWithCredential_Success:" + m_ReceiveUserInfo.displayName);
    }

    // 
    private void ErrorReauthenticateWithCredential(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        m_isReceived = true;

        SetStatusText("ReauthenticateWithCredential_Error:" + outputStr);
    }



    // ストレージにリプレイ情報を作成する
    private void CreateReplayInfoStorage(int index)
    {
        SetStatusText("CreateReplayInfoStorage_Start");
        m_isReplaySuccess = false;
        m_isReplayReceived = false;
        FB_CreateReplayInfoStorageJS(GetInstanceID(), OnCreateReplayInfoStorage, m_ReceiveUserInfo.uid, m_ReplayStr, index);
    }

    // ストレージへのリプレイ情報作成成功時
    private void SuccessCreateReplayInfoStorage(string outputStr)
    {
        //m_ReceiveReplayStorageInfo = JsonUtility.FromJson<ReceiveReplayStorageClass>(outputStr);

        //Debug.Log("ReplayInfoCount:" + m_ReceiveReplayStorageInfo.m_ReplayInfoList.Count);

        m_isReplayReceived = true;

        SetStatusText("CreateReplayInfoStorage_Success");
    }

    // ストレージへのリプレイ情報作成エラー時
    private void ErrorCreateReplayInfoStorage(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        m_isReplayReceived = true;

        //string dispStr;
        //
        //switch (outputStr)
        //{
        //    default:
        //        dispStr = "エラーが発生しました。\nもう一度最初からやり直してください。";
        //        break;
        //}

        SetDialog(true, outputStr, DialogType.Type2, MenuPhase.Start_Start);
    }

    // ストレージにあるリプレイ情報を取得する
    private void GetReplayInfoStorage(int index)
    {
        SetStatusText("GetReplayInfoStorage_Start");
        m_isReplaySuccess = false;
        m_isReplayReceived = false;
        FB_GetReplayInfoStorageJS(GetInstanceID(), OnGetReplayInfoStorage, m_ReceiveUserInfo.uid, index);
    }

    // ストレージにあるリプレイ情報取得成功時
    private void SuccessGetReplayInfoStorage(string outputStr)
    {
        if (AppData.m_ReceiveReplayCountStorageInfo != null)
        {
            if (m_ReplayIndex >= 0 && AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count > m_ReplayIndex)
            {
                AppData.m_ReceiveReplayCountStorageInfo.m_Infos[m_ReplayIndex].m_ReceiveReplayStorageInfo = JsonUtility.FromJson<AppData.ReceiveReplayStorageClass>(outputStr);

                Debug.Log("ReplayIndex" + m_ReplayIndex);
                Debug.Log("ReplayInfoMode:" + AppData.m_ReceiveReplayCountStorageInfo.m_Infos[m_ReplayIndex].m_ReceiveReplayStorageInfo.m_PlayMode.ToString());
                Debug.Log("ReplayInfoCount:" + AppData.m_ReceiveReplayCountStorageInfo.m_Infos[m_ReplayIndex].m_ReceiveReplayStorageInfo.m_ReplayInfoList.Count);
            }
        }
        //m_ReceiveReplayStorageInfo = JsonUtility.FromJson<ReceiveReplayStorageClass>(outputStr);
        //
        //Debug.Log("ReplayInfoMode:" + m_ReceiveReplayStorageInfo.m_PlayMode.ToString());
        //Debug.Log("ReplayInfoCount:" + m_ReceiveReplayStorageInfo.m_ReplayInfoList.Count);
        //
        //int dispCnt = m_ReceiveReplayStorageInfo.m_ReplayInfoList.Count > 5 ? 5 : m_ReceiveReplayStorageInfo.m_ReplayInfoList.Count;
        //for (int i = 0; i < dispCnt; ++i)
        //{
        //    Debug.Log("Frame[" + i + "]:" + m_ReceiveReplayStorageInfo.m_ReplayInfoList[i].frame);
        //    Debug.Log("Key[" + i + "]:" + m_ReceiveReplayStorageInfo.m_ReplayInfoList[i].key);
        //}
        //if (m_ReceiveReplayStorageInfo.m_ReplayInfoList.Count > 10)
        //{
        //    for (int i = m_ReceiveReplayStorageInfo.m_ReplayInfoList.Count - 5; i < m_ReceiveReplayStorageInfo.m_ReplayInfoList.Count; ++i)
        //    {
        //        Debug.Log("Frame[" + i + "]:" + m_ReceiveReplayStorageInfo.m_ReplayInfoList[i].frame);
        //        Debug.Log("Key[" + i + "]:" + m_ReceiveReplayStorageInfo.m_ReplayInfoList[i].key);
        //    }
        //}

        m_isReplayReceived = true;

        SetStatusText("GetReplayInfoStorage_Success");
    }

    // ストレージにあるリプレイ情報取得エラー時
    private void ErrorGetReplayInfoStorage(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        m_isReplayReceived = true;

        //string dispStr;
        //
        //switch (outputStr)
        //{
        //    default:
        //        dispStr = "エラーが発生しました。\nもう一度最初からやり直してください。";
        //        break;
        //}

        SetDialog(true, outputStr, DialogType.Type2, MenuPhase.Loggedin_Start);
    }

    // ストレージにあるリプレイ情報数を取得する
    private void GetReplayInfoCountStorage()
    {
        SetStatusText("GetReplayInfoCountStorage_Start");
        m_isSuccess = false;
        m_isReceived = false;
        //AppData.m_ReceiveReplayCountStorageInfo = null;
        FB_GetReplayInfoCountStorageJS(GetInstanceID(), OnGetReplayInfoCountStorage, m_ReceiveUserInfo.uid);
    }

    // ストレージにあるリプレイ情報数取得成功時
    private void SuccessGetReplayInfoCountStorage(string outputStr)
    {
        if (outputStr != null)
        {

            if (AppData.m_ReceiveReplayCountStorageInfo == null)
            {
                AppData.m_ReceiveReplayCountStorageInfo = new AppData.ReceiveReplayCountStorageClass();
                AppData.m_ReceiveReplayCountStorageInfo.m_Infos = new List<AppData.ReceiveReplayMetaClass>();
            }

            // ["replay_0.json timeCreated:2024-07-16T04:59:47.221Z","replay_1.json timeCreated:2024-07-16T02:44:36.853Z"]

            Debug.Log("ReceiveReplayCountStorageInfo.m_Infos.Count=" + AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count);
            
            for (int i = 0; i < AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count; ++i)
            {
                Debug.Log("ReplayFileName=" + AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_FileName);
                Debug.Log("ReplayFileDate=" + AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_TimeCreated);
                Debug.Log("ReplayFileIndex=" + AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_FileIndex);
            }

            string[] splits = outputStr.Split(",");
            if (splits != null)
            {
                foreach (string fstMat in splits)
                {
                    Match fileMat = Regex.Match(fstMat, "replay_[0-9]+");
                    Match dateMat = Regex.Match(fstMat, "timeCreated:[0-9-T:Z.]+");
                    AppData.ReceiveReplayMetaClass metaInfo = new AppData.ReceiveReplayMetaClass();
                    if (fileMat != null)
                    {
                        metaInfo.m_FileName = fileMat.Value;

                        if (int.TryParse(fileMat.Value.Replace("replay_", ""), out int res))
                        {
                            metaInfo.m_FileIndex = res;
                        }
                    }
                    if (dateMat != null)
                    {
                        //https://atmarkit.itmedia.co.jp/ait/articles/0409/03/news087.html#datetimeoffset
                        DateTimeOffset dto;
                        string formatStr = "yyyy-MM-ddTHH:mm:ss.fffZ";
                        if (DateTimeOffset.TryParseExact(dateMat.Value.Replace("timeCreated:", ""), formatStr, null, DateTimeStyles.AssumeLocal, out dto))
                        {
                            //Debug.Log($"{dto} (local time={dto.ToLocalTime():F})");
                            metaInfo.m_TimeCreated = $"{dto.ToLocalTime():F}";
                        }
                    }
                    metaInfo.m_isUsed = true;
                    metaInfo.m_isDownloaded = false;

                    bool isAdd = true;

                    if (AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count > 0)
                    {
                        for (int i = 0; i < AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count; ++i)
                        {
                            // 既に作成済みの場合はタイムスタンプだけ更新
                            if (AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_FileIndex == metaInfo.m_FileIndex)
                            {
                                AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_FileName = metaInfo.m_FileName;
                                AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_TimeCreated = metaInfo.m_TimeCreated;
                                isAdd = false;
                                break;
                            }
                        }
                    }

                    if (isAdd)
                    {
                        AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Add(metaInfo);
                    }
                }
            }

            AppData.m_ReceiveReplayCountStorageInfo.m_Infos =
                AppData.m_ReceiveReplayCountStorageInfo.m_Infos.OrderBy(x => x.m_FileIndex).ToList();

            for (int i = 0; i < AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Count; ++i)
            {
                Debug.Log("ReplayFileName=" + AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_FileName);
                Debug.Log("ReplayFileDate=" + AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_TimeCreated);
                Debug.Log("ReplayFileIndex=" + AppData.m_ReceiveReplayCountStorageInfo.m_Infos[i].m_FileIndex);
            }
        }

        m_isReceived = true;

        SetStatusText("GetReplayInfoCountStorage_Success");
    }

    // ストレージにあるリプレイ情報数取得エラー時
    private void ErrorGetReplayInfoCountStorage(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        m_isReceived = true;

        //string dispStr;
        //
        //switch (outputStr)
        //{
        //    default:
        //        dispStr = "エラーが発生しました。\nもう一度最初からやり直してください。";
        //        break;
        //}

        //SetDialog(true, outputStr, DialogType.Type2, MenuPhase.Loggedin_Start);
    }



    // Authオブジェクトオブザーバー起動
    private void ActivateOnAuthStateChanged()
    {
        Debug.Log("OnAuthStateChanged_Start");
        FB_OnAuthStateChangedJS(GetInstanceID(), OnAuthStateChanged);
    }

    // 
    private void SuccessOnAuthStateChanged(string outputStr)
    {
        ReceiveUserClass userInfo = JsonUtility.FromJson<ReceiveUserClass>(outputStr);
        Debug.Log("---OnAuthStateChanged---");
        Debug.Log("DisplayName:" + userInfo.displayName);
        Debug.Log("Email:" + userInfo.email);
        Debug.Log("UID:" + userInfo.uid);
        Debug.Log("------------------------");

        //SetStatusText("UpdateEmail_Success:" + m_ReceiveUserInfo.email);
    }

    // 
    private void ErrorOnAuthStateChanged(string outputStr)
    {
        Debug.Log("ErrorCode:" + outputStr);

        //SetStatusText("UpdateEmail_Error:" + outputStr);
        Debug.Log("OnAuthStateChanged:" + outputStr);
    }

    private void DummySetReceiveReplayCountStorageInfo()
    {
        //AppData.m_ReceiveReplayCountStorageInfo = new AppData.ReceiveReplayCountStorageClass();
        //AppData.m_ReceiveReplayCountStorageInfo.m_Infos = new List<AppData.ReceiveReplayMetaClass>();
        //{
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Add(new AppData.ReceiveReplayMetaClass());
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[0].m_FileName = "replay_4";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[0].m_FileIndex = 4;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[0].m_TimeCreated = "2024年8月19日 11:40:04";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[0].m_isDownloaded = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[0].m_isUsed = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[0].m_ReceiveReplayStorageInfo = new AppData.ReceiveReplayStorageClass(AppData.PlayMode.Free, AppData.ConnectM.Mode1, 0, null);
        //
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Add(new AppData.ReceiveReplayMetaClass());
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[1].m_FileName = "replay_2";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[1].m_FileIndex = 2;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[1].m_TimeCreated = "2024年8月19日 11:00:41";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[1].m_isDownloaded = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[1].m_isUsed = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[1].m_ReceiveReplayStorageInfo = new AppData.ReceiveReplayStorageClass(AppData.PlayMode.Free, AppData.ConnectM.Mode1, 0, null);
        //    
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Add(new AppData.ReceiveReplayMetaClass());
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[2].m_FileName = "replay_5";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[2].m_FileIndex = 5;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[2].m_TimeCreated = "2024年8月19日 11:00:41";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[2].m_isDownloaded = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[2].m_isUsed = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[2].m_ReceiveReplayStorageInfo = new AppData.ReceiveReplayStorageClass(AppData.PlayMode.Free, AppData.ConnectM.Mode1, 0, null);
        //    
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Add(new AppData.ReceiveReplayMetaClass());
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[3].m_FileName = "replay_9";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[3].m_FileIndex = 9;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[3].m_TimeCreated = "2024年8月19日 16:09:14";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[3].m_isDownloaded = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[3].m_isUsed = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[3].m_ReceiveReplayStorageInfo = new AppData.ReceiveReplayStorageClass(AppData.PlayMode.Free, AppData.ConnectM.Mode1, 0, null);
        //    
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Add(new AppData.ReceiveReplayMetaClass());
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[4].m_FileName = "replay_0";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[4].m_FileIndex = 0;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[4].m_TimeCreated = "2024年8月19日 11:00:41";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[4].m_isDownloaded = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[4].m_isUsed = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[4].m_ReceiveReplayStorageInfo = new AppData.ReceiveReplayStorageClass(AppData.PlayMode.Free, AppData.ConnectM.Mode1, 0, null);
        //    
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Add(new AppData.ReceiveReplayMetaClass());
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[5].m_FileName = "replay_3";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[5].m_FileIndex = 2;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[5].m_TimeCreated = "2024年8月19日 11:00:41";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[5].m_isDownloaded = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[5].m_isUsed = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[5].m_ReceiveReplayStorageInfo = new AppData.ReceiveReplayStorageClass(AppData.PlayMode.Free, AppData.ConnectM.Mode1, 0, null);
        //    
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Add(new AppData.ReceiveReplayMetaClass());
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[6].m_FileName = "replay_1";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[6].m_FileIndex = 1;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[6].m_TimeCreated = "2024年8月19日 11:00:41";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[6].m_isDownloaded = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[6].m_isUsed = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[6].m_ReceiveReplayStorageInfo = new AppData.ReceiveReplayStorageClass(AppData.PlayMode.Free, AppData.ConnectM.Mode1, 0, null);
        //    
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Add(new AppData.ReceiveReplayMetaClass());
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[7].m_FileName = "replay_6";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[7].m_FileIndex = 6;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[7].m_TimeCreated = "2024年8月19日 11:00:41";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[7].m_isDownloaded = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[7].m_isUsed = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[7].m_ReceiveReplayStorageInfo = new AppData.ReceiveReplayStorageClass(AppData.PlayMode.Free, AppData.ConnectM.Mode1, 0, null);
        //    
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Add(new AppData.ReceiveReplayMetaClass());
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[8].m_FileName = "replay_7";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[8].m_FileIndex = 7;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[8].m_TimeCreated = "2024年8月19日 11:00:41";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[8].m_isDownloaded = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[8].m_isUsed = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[8].m_ReceiveReplayStorageInfo = new AppData.ReceiveReplayStorageClass(AppData.PlayMode.Free, AppData.ConnectM.Mode1, 0, null);
        //    
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos.Add(new AppData.ReceiveReplayMetaClass());
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[9].m_FileName = "replay_8";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[9].m_FileIndex = 8;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[9].m_TimeCreated = "2024年8月19日 11:00:41";
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[9].m_isDownloaded = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[9].m_isUsed = true;
        //    AppData.m_ReceiveReplayCountStorageInfo.m_Infos[9].m_ReceiveReplayStorageInfo = new AppData.ReceiveReplayStorageClass(AppData.PlayMode.Free, AppData.ConnectM.Mode1, 0, null);
        //}
    }

}
