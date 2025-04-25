using System;

public class DebugDataManager : YnsSingletonMonoBehaviour<DebugDataManager>
{
    // バージョンID(アプリバージョン_セーブデータ用更新番号)
    public static readonly string SaveVersionId = "0.0.1_9";

    //セーブしたいデータを定義するクラス
    //コンストラクタで値を初期化
    [Serializable]
    class DebugData : YnsSaveLoad<DebugData>
    {
        public string m_VersionId;          // バージョンID

        // Param関連
        // カメラ
        public float m_DbgGetCameraDist;   // 距離
        public float m_DbgGetCameraZDist;  // 追加Z距離
        public float m_DbgGetCameraRotX;    // RotX
        public float m_DbgGetCameraFoV;     // FoV
        public bool m_IsCameraFollowTarget_Player;

        public DebugData()
        {
            m_VersionId = string.Empty;

            m_DbgGetCameraDist  = 140.0f;   // 距離
            m_DbgGetCameraZDist = 0.0f;  // 追加Z距離
            m_DbgGetCameraRotX  = 60.0f;    // RotX
            m_DbgGetCameraFoV   = 64.0f;     // FoV
            m_IsCameraFollowTarget_Player = false;  // カメラのターゲット
        }
}

    // セーブデータセーブ
    public static void Save()
    {
        DebugData.Save();
    }

    // セーブデータロード
    public static void Load()
    {
        DebugData.Load();
    }

    // セーブデータがあるかどうか
    public static bool IsExist()
    {
        return DebugData.IsExist();
    }

    // セーブデータ削除
    public static void Delete()
    {
        DebugData.Delete();
    }

    // セーブデータ更新
    public static void UpdateData()
    {
        UpdateSaveData();
    }

    // 
    public static string VersionId
    {
        get { return DebugData.Instance.m_VersionId; }
        set { DebugData.Instance.m_VersionId = value; }
    }

    // Camera
    public static float CameraDist
    {
        get { return DebugData.Instance.m_DbgGetCameraDist; }
        set { DebugData.Instance.m_DbgGetCameraDist = value; }
    }
    public static float CameraZDist
    {
        get { return DebugData.Instance.m_DbgGetCameraZDist; }
        set { DebugData.Instance.m_DbgGetCameraZDist = value; }
    }
    public static float CameraRotX
    {
        get { return DebugData.Instance.m_DbgGetCameraRotX; }
        set { DebugData.Instance.m_DbgGetCameraRotX = value; }
    }
    public static float CameraFoV
    {
        get { return DebugData.Instance.m_DbgGetCameraFoV; }
        set { DebugData.Instance.m_DbgGetCameraFoV = value; }
    }
    public static bool IsCameraFollowTarget_Player
    {
        get { return DebugData.Instance.m_IsCameraFollowTarget_Player; }
        set { DebugData.Instance.m_IsCameraFollowTarget_Player = value; }
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