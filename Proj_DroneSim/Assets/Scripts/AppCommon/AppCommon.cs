using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yns;
using UnityEngine.SceneManagement;
using System.Linq;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class AppCommon : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    // 色
    public Color Color_DefBtn_Enable = new Color(255.0f / 255.0f, 146.0f / 255.0f, 47.0f / 255.0f, 1.0f);
    public Color Color_DefBtn_Disable = new Color(116.0f / 255.0f, 116.0f / 255.0f, 116.0f / 255.0f, 1.0f);
    public Color Color_BlackMask = new Color(0.0f,0.0f,0.0f, 153.0f / 255.0f);

    public const bool IsSaveDataDifferent_Init = true;       // セーブデータのバージョンIDに違いがあった場合初期化するか or 更新するか

    // 保存テクスチャの定義
    public static string GetTextureSavePath() { return Application.persistentDataPath + "/" + "HOKSO/"; }

    //================================================
    // [///] 定義
    //================================================
    // 選択したオブジェクトの保持
    public static GameObject m_InGameSelectGo = null;


    //================================================
    // [///] GameData
    //================================================
    // セーブデータ関係
    private static bool m_IsAppFierst = false;      // 起動後の初回読み込み用フラグ

    //================================================
    // [///] マルチテクスチャスプライト
    //================================================
    private readonly string[] MultiTexLoadTbl = {};
    static bool m_IsMultiTexLoadEnd = false;

    // 
    class MultiTexSprite
    {
        public string m_Path;       // テクスチャパス
        public Sprite[] m_Sprites;  // スプライト名
        public MultiTexSprite(string _path, Sprite[] _sprites)
        {
            m_Path = _path;
            m_Sprites = _sprites;
        }
    }
    // マルチテクスチャスプライトリスト
    static private List<MultiTexSprite> m_MultiTexSprites = new List<MultiTexSprite>();

    //================================================
    // [///] シーン
    //================================================
    public enum LoadSceneNo
    {
        DebugSet,
        Title,
        MainGame,
        ObjView,
        MainScene,
        AsSet,
        Game_Treasure,
        Game_Endress,
        Game_StickChallenge,
        Max
    }
    static public int m_NowLoadSceneNo = (int)LoadSceneNo.Title;
    static public string m_UIAudioSEId = "";
    static public float m_UIAudioSESetTime = -1.0f;
    static public string CurrentSceneName { get; private set; } = "";
    static public void SetCurrentSceneName(string name ) { CurrentSceneName = name; }

    public GameObject[] m_RootCanvas = null;
    // 画面外用キャンバス
    static public Canvas m_LowCanvas = null;

    // 広告設定画面化
    static public bool IsAdCtrlScene()
    {
        return (m_NowLoadSceneNo == (int)LoadSceneNo.ObjView) || (m_NowLoadSceneNo == (int)LoadSceneNo.AsSet);
    }
    static public bool IsScene_ObjView()
    {
        return (m_NowLoadSceneNo == (int)LoadSceneNo.ObjView);
    }
    static public bool IsScene_AdSet()
    {
        return (m_NowLoadSceneNo == (int)LoadSceneNo.AsSet);
    }


    //================================================
    // [///] Debug
    //================================================
    // デバッグの表示をこちらに集約する(コメントアウトで無くすため)
    static public void SetDebugLog(string text)
    {
        Debug.Log(text);
    }

    //================================================
    // [///] AppCommonMethod
    //================================================
    // Timestring
    static public string GetTimeText(float timeVal)
    {
        int min = (int)(Mathf.Floor(timeVal) / 60.0f);
        float sec = Mathf.Floor(timeVal);
        sec = sec >= 60 ? sec - 60.0f * min : sec;
        float minsec = (float)min * 60.0f + sec;
        float millisec = Mathf.Floor((timeVal - minsec) * 100.0f);
        string text =
            (min < 10.0f ? "0" : "") + min.ToString() + ":" +
            (sec < 10.0f ? "0" : "") + sec.ToString() + ":" +
            (millisec < 10.0f ? "0" : "") + millisec.ToString();

        return text;
    }
    // Timestring
    static public string GetTimeText_Sec(float timeVal)
    {
        int min = (int)(Mathf.Floor(timeVal) / 60.0f);
        float sec = Mathf.Floor(timeVal);
        sec = sec >= 60 ? sec - 60.0f * min : sec;
        float minsec = (float)min * 60.0f + sec;
        float millisec = Mathf.Floor((timeVal - minsec) * 100.0f);
        string text =
            (min < 10.0f ? "0" : "") + min.ToString() + ":" +
            (sec < 10.0f ? "0" : "") + sec.ToString();

        return text;
    }
    /// <summary>
    /// <para>黒マスクの色設定</para>
    /// @detail 引数transformのimageのcolorを黒マスク設定にする
    /// </summary>
    static public void SetImageColorBlackMask(Transform t) 
    {
        t.GetComponent<Image>().color = Yns.YnSys.m_AppCommon.Color_BlackMask;
    } 

    static public string GetCommaString(long num)
    {
        return string.Format("{0:#,0}", num);
    }

    // 重み付け配列から取得チェック -----------------
    static public int GetTypeIndex(params float[] weightTable)
    {
        int tempIndex = 0;

        float totalWeight = weightTable.Sum();
        float value = UnityEngine.Random.Range(0, totalWeight + 1);
        for (int i = 0; i < weightTable.Length; ++i)
        {
            if (weightTable[i] >= value)
            {
                tempIndex = i;
                break;
            }
            value -= weightTable[i];
        }
        return tempIndex;
    }

    // ゲームデータの取得

    public void SetDelayCall(UnityEngine.Events.UnityAction call, float delay = 0.25f)
    {
        StartCoroutine(ProcDelayCall(call,delay));
    }
    // ボタン
    public IEnumerator ProcDelayCall(UnityEngine.Events.UnityAction call, float delay)
    {
        // SE設定
        ProcCallUISE();

        yield return new WaitForSeconds(delay);

        call.Invoke();
    }
    // ボタン機能時のSE設定
    public void ProcCallUISE()
    {
        switch (m_NowLoadSceneNo)
        {
            case (int)LoadSceneNo.Title:
                m_UIAudioSEId = "UITap";
                m_UIAudioSESetTime = 6.0f;
                break;
            default:
                m_UIAudioSEId = "count";
                m_UIAudioSESetTime = -1.0f;
                break;
        }
        Yns.YnSys.GetGoYnSys().GetComponent<YnsSimpleAudio>().StopSeId(m_UIAudioSEId);
        Yns.YnSys.GetGoYnSys().GetComponent<YnsSimpleAudio>().PlaySe(m_UIAudioSEId, false, m_UIAudioSESetTime);
    }

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
        // 初期化
        GameSetInitialize();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            //Debug.Log("unity-script: OnApplicationPause = " + isPaused);
            Debug.Log("バッググラウンドへいった");
            //if (!m_IsApplicationSave)
            {
                // セーブする
                Update_And_SaveGameData();
            }
            //m_IsApplicationSave = true;

        }
        else
        {
            // m_IsApplicationSave = false;
            Debug.Log("復帰した");

            // セーブする
            Update_And_SaveGameData();
        }
    }

    void OnApplicationFocus(bool focus)
    {
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        Debug.Log("アプリケーションを落とした");
        //if (!m_IsApplicationSave)
        {
            // セーブする
            Update_And_SaveGameData();
        }
        //m_IsApplicationSave = true;
#endif
    }

    private void OnDestroy()
    {
    }

    // ゲーム起動時の初期化
    public void GameSetInitialize()
    {
        // セーブデータの読み込み
        LoadGameData();

        // テクスチャ
        StartCoroutine(MultiTexLoad());

        // Prefab保持セットアップ
        Game.GamePrefabGenerator.Setup();

        // Particle保持セットアップ
        Game.GameParticleGenerator.Setup();

        // GameData読み込み
        StartCoroutine(Game.GD_Audio.Load());


        // GameData読み込み
        StartCoroutine(Game.GD_StageSet.Load());

    }

    //
    public void GameDataInitialize()
    {
        if (m_IsAppFierst)
        {
            return;
        }

        m_IsAppFierst = true;
        // GameData読み込み
        StartCoroutine(Game.GD_Audio.Load());

        // GameData読み込み
        StartCoroutine(Game.GD_StageSet.Load());

    }

    //===============================================
    // [///] セーブデータ
    //================================================
    // セーブデータからGameDataに反映
    static public void LoadGameData()
    {
        AppData.LoadGameData();
        AppDebugData.LoadGameData();
    }

    // GameDataをセーブデータに反映 ※セーブしていない事に注意
    static public void UpdateSaveGameData()
    {
        AppData.SaveGameData();
        AppDebugData.SaveGameData();
    }

    // GameDataをセーブファイルに反映 ※データ更新はしていない
    static public void SaveGameData()
    {
        SaveDataManager.Save();
        DebugDataManager.Save();
    }

    // GameDataをセーブデータに反映/セーブファイルに反映
    static public void Update_And_SaveGameData()
    {
        UpdateSaveGameData();
        SaveGameData();
    }

    //================================================
    // [///] マルチテクスチャスプライト
    //================================================
    // マルチテクスチャスプライト読み込み
    private IEnumerator MultiTexLoad()
    {
        int idx = 0;
        while (idx < MultiTexLoadTbl.Length)
        {
            LoadMultiTexSprite(MultiTexLoadTbl[idx++]);
            yield return null;
        }
        m_IsMultiTexLoadEnd = true;
    }
    static public bool IsMultiTexLoadEnd()
    {
        return m_IsMultiTexLoadEnd;
    }

    // マルチテクスチャをロードしてスプライトを保管する
    public static void LoadMultiTexSprite(string _fileName)
    {
        // Resoucesから対象のテクスチャから生成したスプライト一覧を取得
        Sprite[] sprites = Resources.LoadAll<Sprite>(_fileName);
        // リストに追加
        m_MultiTexSprites.Add(new MultiTexSprite(_fileName, sprites));
    }

    // ロードされたマルチテクスチャスプライトを解放する
    public static void ReleaseMultiTexSprite(string _fileName = "")
    {
        if (_fileName == "")
        {
            m_MultiTexSprites.Clear();
        }
        else
        {
            foreach (MultiTexSprite sp in m_MultiTexSprites)
            {
                if (sp.m_Path == _fileName)
                {
                    m_MultiTexSprites.Remove(sp);
                    break;
                }
            }
        }
    }

    // ロードされたマルチテクスチャスプライトから対応するスプライトを取得する
    public static Sprite GetMultiTexSprite(string _fileName, string _spriteName)
    {
        for (int i = 0; i < m_MultiTexSprites.Count; ++i)
        {
            if (m_MultiTexSprites[i].m_Path == _fileName)
            {
                // 対象のスプライトを取得
                return System.Array.Find<Sprite>(m_MultiTexSprites[i].m_Sprites, (sprite) => sprite.name.Equals(_spriteName));
            }
        }
        Debug.Log("<color=red>GetMultiTexSprite: not found multi tex sprite [" + _fileName + "]</color>");
        return null;
    }

    //================================================
    // [///]
    //================================================
    public static GameObject GetChildGameObject(GameObject _goParent, string _objName)
    {
        GameObject go = null;
        if (_goParent != null)
        {
            Transform[] trnsBuf = _goParent.GetComponentsInChildren<Transform>(true);
            if (trnsBuf != null)
            {
                foreach (Transform trns in trnsBuf)
                {
                    if (trns.gameObject.name == _objName)
                    {
                        go = trns.gameObject;
                        break;
                    }
                }
            }
        }
        return go;
    }

    public void InitAppCanvas()
    {
        if (YnSys.GetAppCanvas() != null)
        {
            RectTransform rt = YnSys.GetAppCanvas().GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition3D = Vector3.zero;
                rt.sizeDelta = Vector2.zero;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.localScale = Vector3.one;
            }
        }
    }

    public GameObject GetRootCanvas(string _sceneName, string _canvasName, bool _isDestroyMainCamera = true)
    {
        Scene scene = SceneManager.GetSceneByName(_sceneName);
        if (scene == null)
        {
            return null;
        }
        GameObject[] rootObjects = scene.GetRootGameObjects();
        GameObject rootCanvas = null;
        foreach (GameObject obj in rootObjects)
        {
            if (obj.name == _canvasName)
            {
                rootCanvas = obj;
            }
            else
            if (_isDestroyMainCamera && obj.name == "Main Camera")
            {
                Destroy(obj);
            }
        }
        return rootCanvas;
    }

    public void SetSceneAllObjectActiveFalse(string _sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(_sceneName);
        if (scene == null)
        {
            return;
        }
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            obj.SetActive(false);
        }
    }

    public void LoadScenes()
    {
        m_RootCanvas = new GameObject[(int)LoadSceneNo.Max];

        for (LoadSceneNo scnNo = LoadSceneNo.MainGame; scnNo < LoadSceneNo.Max; ++scnNo)
        {
            bool isDestoroyMainCamera = false;
            {
                StartCoroutine(LoadScene(scnNo.ToString(), "AppCanvas", scnNo, isDestoroyMainCamera));
            }
        }
    }

    public void LoadScenes(LoadSceneNo _no)
    {
        bool isDestoroyMainCamera = false;
        StartCoroutine(LoadScene(_no.ToString(), "AppCanvas", _no, isDestoroyMainCamera));
    }

    public void LoadScene_MainGame()
    {
        StartCoroutine(Shs.ShSceneUtils.TransitionMainGame("MainGame"));
    }

    public void UnloadScenes()
    {
        for (LoadSceneNo scnNo = LoadSceneNo.MainGame; scnNo < LoadSceneNo.Max; ++scnNo)
        {
            if (m_RootCanvas[(int)scnNo] != null)
            {
                StartCoroutine(UnloadScene(scnNo.ToString(), scnNo));
            }
        }
    }

    public void UnloadScenes(LoadSceneNo _no)
    {
        if (m_RootCanvas[(int)_no] != null)
        {
            StartCoroutine(UnloadScene(_no.ToString(), _no));
        }
    }

    public bool IsEndUnloadScenes()
    {
        bool ret = true;
        for (int i = 0; i < m_RootCanvas.Length; ++i)
        {
            if (m_RootCanvas[i] != null)
            {
                ret = false;
                break;
            }
        }
        return ret;
    }

    public bool IsEndUnloadScenes(AppCommon.LoadSceneNo _scnNo)
    {
        bool ret = true;
        if (m_RootCanvas[(int)_scnNo] != null)
        {
            ret = false;
        }
        return ret;
    }

    IEnumerator LoadScene(string _sceneName, string _canvasName, LoadSceneNo _no, bool _isDestoroyMainCamera = true)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
        yield return asyncOperation;
        m_RootCanvas[(int)_no] = GetRootCanvas(_sceneName, _canvasName, _isDestoroyMainCamera);
        SetSceneAllObjectActiveFalse(_sceneName);
    }

    IEnumerator UnloadScene(string _sceneName, LoadSceneNo _no)
    {
        // 
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(_sceneName);
        yield return asyncOperation;
        m_RootCanvas[(int)_no] = null;
    }

    // 画面外用キャンバス取得
    static public Canvas GetLowCanvas()
    {
        return m_LowCanvas;
    }

    // 画面外用キャンバス取得登録
    static public void SetLowCanvas(Canvas _canvas)
    {
        m_LowCanvas = _canvas;
    }

    // インターネット接続判定
    public static bool IsInternetReachable(bool _isLogOff = false)
    {
        bool ret = false;
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                if (!_isLogOff)
                {
                    Debug.Log("InternetCheck : internet not reachable");
                }
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                if (!_isLogOff)
                {
                    Debug.Log("InternetCheck : reachable carrier data network");
                }
                ret = true;
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                if (!_isLogOff)
                {
                    Debug.Log("InternetCheck : reachable via Wifi or cable");
                }
                ret = true;
                break;
        }

#if DEBUG_INTERNET_OFF
        ret = true;
        return ret;
#else
        return ret;
#endif
    }
}
