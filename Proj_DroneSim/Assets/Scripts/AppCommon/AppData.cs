using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yns;
using Game;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// アプリ内の使用データクラス
/// </summary>
public class AppData : YnsSingletonMonoBehaviour<AppData>
{
    //======================================================================
    //
    // [///] 全体定義(どこからでも見ること可能)
    //
    //======================================================================
    public const int MaxStageNum = 5;               // ステージ数
    public const int ParamLvLimit = 70;             // 所持コインキャップ
    public const long CoinNumLimit = 99999999;      // 自機レベルキャップ

    public const int MaxReplayCount = 10;           // リプレイデータ保存最大数

    // チュートリアル識別
    public enum TutorialNum
    {
        LvUp = 0,
        Stage,

        Max
    }

    [Serializable]
    // ミッションモード時のId
    public enum MissionID
    {
        SquareFly,
        EightFly,
        HappningFly,
    }

    [Serializable]
    // 接続モード
    public enum ConnectM
    {
        Mode1,
        Mode2,
    }

    [Serializable]
    // コントローラーモード
    public enum ControllerM
    {
        GamePad,
        Keyboard,
    }

    [Serializable]
    public enum PlayMode
    {
        None = -1,
        Free,
        Mission,
        Replay,

        Game_Treasure,
        Game_Irairabou,
        Game_Endress,
    }

    [Serializable]
    public class ReplayInfo
    {
        public float frame;
        public string key;
        public float value;
        public ReplayInfo(float _frame, string _key, float _value)
        {
            frame = _frame;
            key = _key;
            value = _value;
        }
    }

    [Serializable]
    public class ReceiveReplayStorageClass
    {
        public AppData.PlayMode m_PlayMode;
        public AppData.ConnectM m_ConnectMode;
        public int m_WindSeed = 0;
        public List<ReplayInfo> m_ReplayInfoList;
        public ReceiveReplayStorageClass(AppData.PlayMode _playMode, AppData.ConnectM _connectMode, int _windSeed, List<ReplayInfo> _list)
        {
            m_PlayMode = _playMode;
            m_ConnectMode = _connectMode;
            m_WindSeed = _windSeed;
            m_ReplayInfoList = _list != null ? new List<ReplayInfo>(_list) : null;
        }
    }

    public class ReceiveReplayMetaClass
    {
        public string m_FileName;
        public string m_TimeCreated;
        public int m_FileIndex;
        public bool m_isDownloaded;
        public bool m_isUsed;
        public ReceiveReplayStorageClass m_ReceiveReplayStorageInfo;
    }

    public class ReceiveReplayCountStorageClass
    {
        public List<ReceiveReplayMetaClass> m_Infos;
    }

    // キーのトリガアクション
    public enum Action
    {
        None = -1,
        Start,
        Decide = Start,
        Cancel,
        LUp,
        LDown,
        LLeft,
        LRight,
        RUp,
        RDown,
        RLeft,
        RRight,
        Metronome,
        Grid,
        Flip,
        Speed,
        Headless,
        VisionSensor,
        AutoTakeoffLanding,
        Max
    }
    public static readonly KeyCode[] InitKeyCodes = {
        KeyCode.Return,     // Deside
        KeyCode.Escape,     // Cancel
        KeyCode.W,          // LUp
        KeyCode.S,          // LDown
        KeyCode.A,          // LLeft
        KeyCode.D,          // LRight
        KeyCode.UpArrow,    // RUp
        KeyCode.DownArrow,  // RDown
        KeyCode.LeftArrow,  // RLeft
        KeyCode.RightArrow, // RRight
        KeyCode.Z,         // Metronome
        KeyCode.X,         // Grid
        KeyCode.C,         // Flip
        KeyCode.V,         // Speed
        KeyCode.B,         // Headless
        KeyCode.N,         // VisionSensor
        KeyCode.M,         // AutoTakeoffLanding

        KeyCode.JoystickButton4,    // L1
        KeyCode.JoystickButton5,    // R1
    };

    // ゲームパッドのトリガアクション
    public enum PadAction
    {
        None = -1,
        Start,
        Decide = Start,
        Cancel,
        Up,
        Down,
        Left,
        Right,
        Metronome,
        Grid,
        Flip,
        Speed,
        Headless,
        VisionSensor,
        AutoTakeoffLanding,
        Max
    }

    //================================================
    // [///] private 定義
    //================================================

    //================================================
    // [///] GameData
    //================================================
    // ↓ セーブデータ関係
    // 保存日付
    static public string m_SaveDate = "";
    // 振動の有無
    static public bool IsEnableVibration { get; private set; } = true;

    static public string EmailAddress { get; private set; } = "";
    static public string Password { get; private set; } = "";
    // オプション設定
    static public ConnectM ConnectMode { get; private set; } = ConnectM.Mode2;
    static public ControllerM ControllerMode { get; private set; } = ControllerM.GamePad;
    static public bool GridDisplay { get; private set; } = true;
    static public bool PadDisplay { get; private set; } = true;
    static public bool DroneFlightSound { get; private set; } = true;
    static public bool MetronomeFlightSound { get; private set; } = true;
    static public int MetronomeBPM { get; private set; } = 60;

    static public List<KeyCode> KeyCodes { get; private set; } = null;
    static public string PadConfig { get; set; } = string.Empty;

    // ローカルデータ
    // 起動後初のゲームか
    static public bool IsFirstGame { get; set; } = false;
    static public PlayMode m_PlayMode { get; set; } = PlayMode.None;
    //  ミッションモードのミッションId
    static public MissionID m_MissionId { get; set; } = MissionID.EightFly;
    //  リプレイ時の接続モード
    static public ConnectM m_ReplayConnectMode { get; set; } = ConnectM.Mode2;

    static public ReceiveReplayCountStorageClass m_ReceiveReplayCountStorageInfo = null;
    static public int m_SelectReplayIndex = 0;
    static public int m_ReplaySetIndex = 0;

    // GameMode


    static public bool[] PadTrgs { get; private set; } = null;

    //================================================
    // [///] セーブデータ
    //================================================
    // セーブデータからGameDataに反映
    static public void LoadGameData()
    {
        // 既存セーブデータのチェック
        if (SaveDataManager.IsExist())
        {
            if (SaveDataManager.VersionId != SaveDataManager.SaveVersionId)
            {
                // 初期化
                if (AppCommon.IsSaveDataDifferent_Init)
                {
                    Debug.Log("セーブデータバージョンが異なるので初期化します  NowID:" + SaveDataManager.VersionId + "  TargetId:" + SaveDataManager.SaveVersionId);
                    SaveDataManager.Delete();
                    SaveDataManager.Load();
                    // セーブデータバージョンIDを入れておく
                    SaveDataManager.VersionId = SaveDataManager.SaveVersionId;
                }
                else
                {
                    // // 更新処理
                    // Debug.Log("セーブデータバージョンが異なるので更新します  NowID:" + SaveDataManager.VersionId + "  TargetId:" + SaveDataManager.SaveVersionId);
                    // SaveDataManager.UpdateData();
                    // // 更新SaveDataをSave
                    // SaveDataManager.Save();
                }
            }
        }
        else
        {
            // セーブデータバージョンIDを入れておく
            SaveDataManager.VersionId = SaveDataManager.SaveVersionId;
        }

        // データ設定
        {
            m_SaveDate = SaveDataManager.SaveDate;
            EmailAddress = SaveDataManager.EmailAddress;
            Password = SaveDataManager.Password;
            ConnectMode = (ConnectM)SaveDataManager.ConnectMode;
            ControllerMode = (ControllerM)SaveDataManager.ControllerMode;
            GridDisplay = SaveDataManager.GridDisplay;
            PadDisplay = SaveDataManager.PadDisplay;
            DroneFlightSound = SaveDataManager.DroneFlightSound;
            MetronomeFlightSound = SaveDataManager.MetronomeFlightSound;
            MetronomeBPM = SaveDataManager.MetronomeBPM;

            KeyCodes = new List<KeyCode>(SaveDataManager.KeyCodes);
            PadConfig = new string(SaveDataManager.PadConfig);

            PadTrgs = new bool[(int)PadAction.Max];

            Debug.Log("SaveData:"+m_SaveDate);
            Debug.Log("EmailAddress:" + EmailAddress);
            Debug.Log("Password:" + Password);

            // パッドのバインド情報を読み込んでおく
            YnInputSystem.Load();
        }
    }

    // GameDataをセーブデータに反映 ※セーブしていない事に注意
    static public void SaveGameData()
    {
        SaveDataManager.SaveDate = m_SaveDate;
        SaveDataManager.EmailAddress = EmailAddress;
        SaveDataManager.Password = Password;

        SaveDataManager.ConnectMode = (int)ConnectMode;
        SaveDataManager.ControllerMode = (int)ControllerMode;
        SaveDataManager.GridDisplay = GridDisplay;
        SaveDataManager.PadDisplay = PadDisplay;
        SaveDataManager.DroneFlightSound = DroneFlightSound;
        SaveDataManager.MetronomeFlightSound = MetronomeFlightSound;
        SaveDataManager.MetronomeBPM = MetronomeBPM;

        SaveDataManager.KeyCodes = new List<KeyCode>(KeyCodes);
        SaveDataManager.PadConfig = new string(PadConfig);

        Debug.Log("SaveData:" + m_SaveDate);
        Debug.Log("EmailAddress:" + EmailAddress);
        Debug.Log("Password:" + Password);
    }

    // セーブデータの初期化
    static public void Dbg_SaveDataReset()
    {
        EmailAddress = "";
        Password = "";
        ConnectMode = ConnectM.Mode2;
        ControllerMode = ControllerM.GamePad;
        GridDisplay = true;
        PadDisplay = true;
        DroneFlightSound = true;
        MetronomeFlightSound = true;
        MetronomeBPM = 60;
    }

    //================================================
    // [///] リストデータのidx指定取得
    //================================================

    //================================================
    // [///] システム処理の機能フラグ関係
    //================================================
    // 振動機能オン/オフ登録
    static public void SetIsEnableVibration(bool _flag)
    {
        IsEnableVibration = _flag;
    }
    // デバッグ用:ステージ状態のリセット
    static public void Dbg_StageInfoReset()
    {
    }

    static public void SetEmailAddress(string emailStr)
    {
        EmailAddress = emailStr;
    }

    static public void SetPassword(string passwordStr)
    {
        Password = passwordStr;
    }

    static public void SetConnectMode(ConnectM mode)
    {
        ConnectMode = mode;
    }

    static public void SetControllerMode(ControllerM mode)
    {
        ControllerMode = mode;
    }

    static public void SetGridDisplay(bool flag)
    {
        GridDisplay = flag;
    }

    static public void SetPadDisplay(bool flag)
    {
        PadDisplay = flag;
    }

    static public void SetDroneFlightSound(bool flag)
    {
        DroneFlightSound = flag;
    }

    static public void SetMetronomeFlightSound(bool flag)
    {
        MetronomeFlightSound = flag;
    }

    static public void SetMetronomeBPM(int val)
    {
        MetronomeBPM = val;
    }

    /// <summary>
    /// キーボード設定の初期化
    /// </summary>
    static public void InitKeycode()
    {
        KeyCodes = new List<KeyCode>((int)Action.Max);

        for (int i = 0; i < InitKeyCodes.Length; ++i)
        {
            KeyCodes.Add(InitKeyCodes[i]);
        }
    }

    /// <summary>
    /// キーボード設定の変更　※該当キーが登録されている場合はスワップします
    /// </summary>
    /// <param name="key"></param>
    /// <param name="code"></param>
    static public void SetKeyCode(Action key, KeyCode code)
    {
        int existCodeIdx = KeyCodes.FindIndex(x => x == code);
        // 変更前と同じ場所の場合は何もしない
        if (existCodeIdx == (int)key) return;
        // 変更するキーコードが別の場所で設定されている
        if (existCodeIdx >= 0)
        {
            // 入れ替える
            KeyCodes[existCodeIdx] = KeyCodes[(int)key];
            KeyCodes[(int)key] = code;
        }
        else
        // 変更するキーコードが他の場所で設定されていないので、そのまま登録する
        {
            KeyCodes[(int)key] = code;
        }
    }

    /// <summary>
    /// アクション情報からの入力中判定
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static public bool GetInput(Action key)
    {
        return Input.GetKey(KeyCodes[(int)key]);// || GetPadTrg(key);
    }

    /// <summary>
    /// アクション情報からの離した判定
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static public bool GetKeyUp(Action key)
    {
        return Input.GetKeyUp(KeyCodes[(int)key]);// || GetPadTrg(key);
    }

    /// <summary>
    /// アクション情報からのトリガ入力判定 キーボードのみ
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static public bool GetTrg_KeyOnly(Action key)
    {
        return Input.GetKeyDown(KeyCodes[(int)key]);
    }

    /// <summary>
    /// アクション情報からのトリガ入力判定
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static public bool GetTrg(Action key)
    {
        return Input.GetKeyDown(KeyCodes[(int)key]) || GetPadTrg(key);
    }

    /// <summary>
    /// アクション情報からのゲームパッドトリガ入力判定
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static public bool GetPadTrg(Action key)
    {
        PadAction padAction = ConvertToActionFromPadAction(key);
        bool isAnalogStickTrg = false;
        switch (key)
        {
            case Action.LUp:
                isAnalogStickTrg = YnInputManager.IsTrgLPadUp();
                break;
            case Action.LDown:
                isAnalogStickTrg = YnInputManager.IsTrgLPadDown();
                break;
            case Action.LLeft:
                isAnalogStickTrg = YnInputManager.IsTrgLPadLeft();
                break;
            case Action.LRight:
                isAnalogStickTrg = YnInputManager.IsTrgLPadRight();
                break;
            //右スティックはトリガ入力させないようにする
            //case Action.RUp:
            //    isAnalogStickTrg = YnInputManager.IsTrgRPadUp();
            //    break;
            //case Action.RDown:
            //    isAnalogStickTrg = YnInputManager.IsTrgRPadDown();
            //    break;
            //case Action.RLeft:
            //    isAnalogStickTrg = YnInputManager.IsTrgRPadLeft();
            //    break;
            //case Action.RRight:
            //    isAnalogStickTrg = YnInputManager.IsTrgRPadRight();
            //    break;
        }
        return PadTrgs[(int)padAction] || isAnalogStickTrg;
    }

    /// <summary>
    /// キーボードアクション情報からゲームパッドアクション情報への変換
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static public PadAction ConvertToActionFromPadAction(Action key)
    {
        PadAction padAction = PadAction.None;
        switch (key)
        {
            case Action.Decide:
                padAction = PadAction.Decide;
                break;
            case Action.Cancel:
                padAction = PadAction.Cancel;
                break;
            case Action.LUp:
            case Action.RUp:
                padAction = PadAction.Up;
                break;
            case Action.LDown:
            case Action.RDown:
                padAction = PadAction.Down;
                break;
            case Action.LLeft:
            case Action.RLeft:
                padAction = PadAction.Left;
                break;
            case Action.LRight:
            case Action.RRight:
                padAction = PadAction.Right;
                break;
            case Action.Metronome:
                padAction = PadAction.Metronome;
                break;
            case Action.Grid:
                padAction = PadAction.Grid;
                break;
            case Action.Flip:
                padAction = PadAction.Flip;
                break;
            case Action.Speed:
                padAction = PadAction.Speed;
                break;
            case Action.Headless:
                padAction = PadAction.Headless;
                break;
            case Action.VisionSensor:
                padAction = PadAction.VisionSensor;
                break;
            case Action.AutoTakeoffLanding:
                padAction = PadAction.AutoTakeoffLanding;
                break;
        }
        return padAction;
    }

    /// <summary>
    /// パッド入力情報設定
    /// </summary>
    /// <param name="act"></param>
    static public void SetPadTrgs(PadAction act)
    {
        if (act >= PadAction.Start)
        {
            PadTrgs[(int)act] = true;
        }
    }

    /// <summary>
    /// パッド入力情報初期化
    /// </summary>
    static public void ResetPadTrgs()
    {
        PadTrgs = PadTrgs.Select(x => x = false).ToArray();
    }

    /// <summary>
    ///  接続モードを返す(リプレイ時:ログの接続モード,他:オプション接続モード)
    /// </summary>
    static public ConnectM GetCurrentConnectMode()
    {
        if(m_PlayMode == PlayMode.Replay)
        {
            if (GetReplayData(m_SelectReplayIndex) != null)
            {
                return GetReplayData(m_SelectReplayIndex).m_ConnectMode;
            }
            else
            {
                // デフォルト
                return ConnectM.Mode2;
            }
        }
        else
        {
            return ConnectMode;
        }
    }

    /// <summary>
    /// 空いているリプレイファイルインデックス値を返す　※上限は32まで、それを超えると0が返る
    /// </summary>
    /// <returns></returns>
    static private int GetFreeReplayFileIndex()
    {
        int res = 0;
        if (m_ReceiveReplayCountStorageInfo != null)
        {
            uint usedFlag = 0;
            int max = Mathf.Min(m_ReceiveReplayCountStorageInfo.m_Infos.Count, MaxReplayCount);
            int i = 0;
            for (; i <  max; ++i)
            {
                if (m_ReceiveReplayCountStorageInfo.m_Infos[i].m_isUsed)
                {
                    usedFlag |= (uint)(1 << m_ReceiveReplayCountStorageInfo.m_Infos[i].m_FileIndex);
                }
            }
            for (i = 0; i <= MaxReplayCount; ++i)
            {
                if ((uint)(usedFlag & (uint)(1 << i)) == 0)
                {
                    res = i;
                    break;
                }
            }
        }
        return res;
    }

    static int GetFreeReplayIndex()
    {
        int index = 0;
        for (; index < m_ReceiveReplayCountStorageInfo.m_Infos.Count; ++index)
        {
            if (!m_ReceiveReplayCountStorageInfo.m_Infos[index].m_isUsed)
            {
                break;
            }
        }
        if (index >= MaxReplayCount) index = 0;
        return index;
    }

    struct TimeInfo
    {
        public int year;
        public int month;
        public int day;
        public int hour;
        public int min;
        public int sec;
        public int index;
        public void SetInit(int _index)
        {
            year = 0;
            month = 0;
            day = 0;
            hour = 0;
            min = 0;
            sec = 0;
            index = _index;
        }
    }

    static int GetReplaceReplayFileIndex()
    {
        int index = 0;
        List<TimeInfo> timeList = new List<TimeInfo>();

        for (int i = 0; i < m_ReceiveReplayCountStorageInfo.m_Infos.Count; ++i)
        {
            if (m_ReceiveReplayCountStorageInfo.m_Infos[i].m_isUsed)
            {
                string timeStr = m_ReceiveReplayCountStorageInfo.m_Infos[i].m_TimeCreated;
                Match yearMat = Regex.Match(timeStr, "[0-9]+年");
                Match monthMat = Regex.Match(timeStr, "[0-9]+月");
                Match dayMat = Regex.Match(timeStr, "[0-9]+日");
                Match hourMat = Regex.Match(timeStr, "[0-9]+:");
                Match minMat = Regex.Match(timeStr, ":[0-9]+:");
                Match secMat = Regex.Match(timeStr, ":[0-9]+$");

                TimeInfo info = new TimeInfo();
                info.SetInit(i);

                if (yearMat != null)
                {

                    if (int.TryParse(yearMat.Value.Replace("年", ""), out int res))
                    {
                        info.year = res;
                    }
                }
                if (monthMat != null)
                {

                    if (int.TryParse(monthMat.Value.Replace("月", ""), out int res))
                    {
                        info.month = res;
                    }
                }
                if (dayMat != null)
                {

                    if (int.TryParse(dayMat.Value.Replace("日", ""), out int res))
                    {
                        info.day = res;
                    }
                }
                if (hourMat != null)
                {

                    if (int.TryParse(hourMat.Value.Replace(":", ""), out int res))
                    {
                        info.hour = res;
                    }
                }
                if (minMat != null)
                {

                    if (int.TryParse(minMat.Value.Replace(":", ""), out int res))
                    {
                        info.min = res;
                    }
                }
                if (secMat != null)
                {

                    if (int.TryParse(secMat.Value.Replace(":", ""), out int res))
                    {
                        info.sec = res;
                    }
                }

                Debug.Log("Replay_"+i+": "+info.year+"/"+info.month+"/"+info.day+"/"+info.hour+"/"+info.min+"/"+info.sec);

                timeList.Add(info);
            }
        }

        if (timeList.Count > 0)
        {
            // 年の最小値
            int minYear = timeList.Select(x => x.year).Min();
            List<TimeInfo> timeList2 = timeList.Where(x => x.year == minYear).ToList();

            if (timeList2.Count > 1)
            {
                // 月の最小値
                int minMonth = timeList2.Select(x => x.month).Min();
                List<TimeInfo> timeList3 = timeList2.Where(x => x.month == minMonth).ToList();
                if (timeList3.Count > 1)
                {
                    // 日付の最小値
                    int minDay = timeList3.Select(x => x.day).Min();
                    List<TimeInfo> timeList4 = timeList3.Where(x => x.day == minDay).ToList();
                    if (timeList4.Count > 1)
                    {
                        // 時間の最小値
                        int minHour = timeList4.Select(x => x.hour).Min();
                        List<TimeInfo> timeList5 = timeList4.Where(x => x.hour == minHour).ToList();
                        if (timeList5.Count > 1)
                        {
                            // 分の最小値
                            int minMin = timeList5.Select(x => x.min).Min();
                            List<TimeInfo> timeList6 = timeList5.Where(x => x.min == minMin).ToList();
                            if (timeList6.Count > 1)
                            {
                                // 秒の最小値
                                int minSec = timeList6.Select(x => x.sec).Min();
                                List<TimeInfo> timeList7 = timeList6.Where(x => x.sec == minSec).ToList();
                                index = timeList7[0].index;
                            }
                            else
                            {
                                index = timeList6[0].index;
                            }
                        }
                        else
                        {
                            index = timeList5[0].index;
                        }
                    }
                    else
                    {
                        index = timeList4[0].index;
                    }
                }
                else
                {
                    index = timeList3[0].index;
                }
            }
            else
            {
                index = timeList2[0].index;
            }
        }

        return index;
    }

    static int GetReplayIndex(ref int fileIndex)
    {
        int index = 0;
        int fileMax = 0;
        for (int i = 0; i < m_ReceiveReplayCountStorageInfo.m_Infos.Count; ++i)
        {
            if (m_ReceiveReplayCountStorageInfo.m_Infos[i].m_isUsed) ++fileMax;
        }

        if (fileMax < MaxReplayCount)
        {
            index = GetFreeReplayIndex();
            fileIndex = GetFreeReplayFileIndex();
        }
        else
        {
            index = GetReplaceReplayFileIndex();
            fileIndex = m_ReceiveReplayCountStorageInfo.m_Infos[index].m_FileIndex;
        }
        return index;
    }

    /// <summary>
    /// リプレイデータの作成
    /// </summary>
    /// <param name="playMode"></param>
    /// <param name="list"></param>
    static public void SetReplayData(PlayMode playMode, List<ReplayInfo> list, int windSeed)
    {
        if (m_ReceiveReplayCountStorageInfo == null)
        {
            m_ReceiveReplayCountStorageInfo = new ReceiveReplayCountStorageClass();
            m_ReceiveReplayCountStorageInfo.m_Infos = new List<ReceiveReplayMetaClass>();

            Debug.Log("ReceiveReplayCountStorageInfo=NULL");
        }

        Debug.Log("m_ReceiveReplayCountStorageInfo.m_Infos.Count="+ m_ReceiveReplayCountStorageInfo.m_Infos.Count);

        int fileIndex = 0;
        int index = GetReplayIndex(ref fileIndex);

        while (m_ReceiveReplayCountStorageInfo.m_Infos.Count <= index)
        {
            m_ReceiveReplayCountStorageInfo.m_Infos.Add(new ReceiveReplayMetaClass());
        }

        Debug.Log("ReplayIndex="+index);
        Debug.Log("ReplayFileIndex="+ fileIndex);

        m_ReceiveReplayCountStorageInfo.m_Infos[index].m_FileIndex = fileIndex;
        m_ReceiveReplayCountStorageInfo.m_Infos[index].m_isDownloaded = false;
        m_ReceiveReplayCountStorageInfo.m_Infos[index].m_isUsed = true;
        m_ReceiveReplayCountStorageInfo.m_Infos[index].m_ReceiveReplayStorageInfo =
            new ReceiveReplayStorageClass(playMode, ConnectMode, windSeed, new List<ReplayInfo>(list));
    }

    /// <summary>
    /// リプレイデータの取得 ※リプレイデータが作成or読み込まれている必要がある
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    static public ReceiveReplayStorageClass GetReplayData(int index)
    {
        ReceiveReplayStorageClass res = null;
        if (m_ReceiveReplayCountStorageInfo != null)
        {
            if (m_ReceiveReplayCountStorageInfo.m_Infos != null)
            {
                if (m_ReceiveReplayCountStorageInfo.m_Infos.Count > index)
                {
                    res = m_ReceiveReplayCountStorageInfo.m_Infos[index].m_ReceiveReplayStorageInfo;
                }
            }
        }
        return res;
    }

}
