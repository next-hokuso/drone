using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveDataManager : YnsSingletonMonoBehaviour<SaveDataManager>
{
    // バージョンID(アプリバージョン_セーブデータ用更新番号)
    public static readonly string SaveVersionId = "0.0.1_6";
    //セーブしたいデータを定義するクラス
    //コンストラクタで値を初期化
    [Serializable]
    class SaveData : YnsSaveLoad<SaveData>
    {
        public string m_VersionId;          // バージョンID
        public string m_SaveDate;           // 保存日付

        public string m_EmailAddress;       // メールーアドレス
        public string m_Password;           // パスワード

        public int m_ConnectMode;           // 接続モード
        public int m_ControllerMode;        // コントローラー設定
        public bool m_GridDisplay;          // グリッド表示
        public bool m_PadDisplay;           // パッド表示
        public bool m_DroneFlightSound;     // ドローン飛行音
        public bool m_MetronomeFlightSound; // メトロノーム飛行音
        public int m_MetronomeBPM;          // メトロノームBPM

        public List<KeyCode> m_KeyCodes = null; // キーボード設定リスト
        public string m_PadConfig;          // パッドバインド情報JSON

        //
        public SaveData()
        {
            m_VersionId = string.Empty;
            m_SaveDate = "";

            m_EmailAddress = "";
            m_Password = "";

            m_ConnectMode = (int)AppData.ConnectM.Mode2;
            m_ControllerMode = 0;
            m_GridDisplay = false;
            m_PadDisplay = true;
            m_DroneFlightSound = true;
            m_MetronomeFlightSound = true;
            m_MetronomeBPM = 60;

            m_KeyCodes = new List<KeyCode>((int)AppData.Action.Max);
            for (int i = 0; i < AppData.InitKeyCodes.Length; ++i)
            {
                m_KeyCodes.Add(AppData.InitKeyCodes[i]);
            }

            m_PadConfig = string.Empty;
        }
    }

    // セーブデータセーブ
    public static void Save()
    {
        SaveData.Save();
    }

    // セーブデータロード
    public static void Load()
    {
        SaveData.Load();
    }

    // セーブデータがあるかどうか
    public static bool IsExist()
    {
        return SaveData.IsExist();
    }

    // セーブデータ削除
    public static void Delete()
    {
        SaveData.Delete();
    }

    // セーブデータ更新
    public static void UpdateData()
    {
        UpdateSaveData();
    }

    // 
    public static string VersionId
    {
        get { return SaveData.Instance.m_VersionId; }
        set { SaveData.Instance.m_VersionId = value; }
    }
    // 保存日付
    public static string SaveDate
    {
        get { return SaveData.Instance.m_SaveDate; }
        set { SaveData.Instance.m_SaveDate = value; }
    }

    // メールアドレス
    public static string EmailAddress
    {
        get { return SaveData.Instance.m_EmailAddress; }
        set { SaveData.Instance.m_EmailAddress = value; }
    }

    // パスワード
    public static string Password
    {
        get { return SaveData.Instance.m_Password; }
        set { SaveData.Instance.m_Password = value; }
    }

    // 接続モード
    public static int ConnectMode
    {
        get { return SaveData.Instance.m_ConnectMode; }
        set { SaveData.Instance.m_ConnectMode = value; }
    }

    // コントローラーモード
    public static int ControllerMode
    {
        get { return SaveData.Instance.m_ControllerMode; }
        set { SaveData.Instance.m_ControllerMode = value; }
    }

    // グリッド表示
    public static bool GridDisplay
    {
        get { return SaveData.Instance.m_GridDisplay; }
        set { SaveData.Instance.m_GridDisplay = value; }
    }

    // パッド表示
    public static bool PadDisplay
    {
        get { return SaveData.Instance.m_PadDisplay; }
        set { SaveData.Instance.m_PadDisplay = value; }
    }

    // ドローン飛行音
    public static bool DroneFlightSound
    {
        get { return SaveData.Instance.m_DroneFlightSound; }
        set { SaveData.Instance.m_DroneFlightSound = value; }
    }

    // メトロノーム飛行音
    public static bool MetronomeFlightSound
    {
        get { return SaveData.Instance.m_MetronomeFlightSound; }
        set { SaveData.Instance.m_MetronomeFlightSound = value; }
    }

    // メトロノームBPM
    public static int MetronomeBPM
    {
        get { return SaveData.Instance.m_MetronomeBPM; }
        set { SaveData.Instance.m_MetronomeBPM = value; }
    }

    // キーボード設定リスト
    public static List<KeyCode> KeyCodes
    {
        get { return SaveData.Instance.m_KeyCodes; }
        set { SaveData.Instance.m_KeyCodes = value; }
    }

    // パッドバインド情報JSON
    public static string PadConfig
    {
        get { return SaveData.Instance.m_PadConfig; }
        set { SaveData.Instance.m_PadConfig = value; }
    }

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
    }

    //================================================
    // [///] セーブデータバージョンアップ
    //================================================
    private static void UpdateSaveData()
    {
    }
}