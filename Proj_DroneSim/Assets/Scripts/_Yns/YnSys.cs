#define ENABLE_FPS

using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Yns
{
    public class YnSys : MonoBehaviour
    {
        //================================================
        // [///]
        //================================================
        //
        static readonly float DefaultScreenWidth = 9.0f;
        static readonly float DefaultScreenHeight = 16.0f;

        static readonly string YnSysName = "YnSys";
        static readonly string YnSysUIResourceName = "YnSysUI";
        static readonly string YnSysUIResourcePathName = "_Yns/" + YnSysUIResourceName;

        //
        static readonly string BgCanvasName = "BgCanvas";
        static readonly string AppParentCanvasName = "AppParentCanvas";
        static readonly string AppCanvasName = "AppCanvas";
        static readonly string AppDebugCanvasName = "AppDebugCanvas";

        // 
        static GameObject m_goYnSys = null;
        static GameObject m_goYnSysUI = null;
        static GameObject m_goYnSysAppParentCanvas = null;
        static GameObject m_goYnSysAppCtrl = null;
        static GameObject m_goYnSysAppCanvas = null;
        static GameObject m_goYnSysDebugTitle = null;
        static GameObject m_goYnSysDebugFPS = null;
        static GameObject m_goYnSysDebugBgBlue = null;

        static readonly string DebugMainMenuName = "DbgMainMenu";
        static readonly string DebugBlueBgName = BgCanvasName + "/BgBlue";
        static readonly string DebugTitleName = AppDebugCanvasName + "/DebugTitleText";
        static readonly string DebugFPSName = AppDebugCanvasName + "/DebugFPSText";

        public static AppCommon m_AppCommon = null;
        public static string App3DRoot = "3D_Root";

        // デバッグモード判定
        static bool m_IsDebugMode = true; //false;
        static bool m_IsFromDbgMainMenu = false;
        static bool m_IsDebugMainGame = false;

        static List<string> m_SceneNameList = null;

#if ENABLE_FPS
        static int m_FrameCount;
        static float m_PrevTime;
        static float m_Fps;
#endif

        static Text m_DbgText = null;

        static public Sh_SystemCanvasCtrl SystemCanvas { get; private set; } = null;

        static private GameObject m_goYnSysWarning = null;
        // モバイルブラウザ判定
        static public bool m_IsMobilePlatform { get; private set; } = false;
        // 縦画面判定
        static public bool m_IsVerticalScreen { get; private set; } = false;

        // .jslibからプラットフォーム識別
        [DllImport("__Internal")]
        private static extern string CheckMobilePlatformJS();

        [DllImport("__Internal")]
        private static extern void CheckOrientationJS(int instanceId, Action<int, string, bool> receiveCallback);

        // コールバックで実行元のインスタンスIDとインスタンスをマッピングするためのDictionary
        private static readonly Dictionary<int, YnSys> Instances = new();

        //================================================
        // [///]
        //================================================
        // Start is called before the first frame update
        void Start()
        {
            // フレームレート : 60 に設定
            Application.targetFrameRate = 60;

            Debug.Log("*** YnSys Start");// 遷移先シーン名
            m_SceneNameList = new List<string>();

#if AI_DEBUG
            m_IsDebugMode = true;
#endif

#if ENABLE_FPS
            m_FrameCount = 0;
            m_PrevTime = 0.0f;
#endif

            // インスタンスIDの紐づけを行う
            var instanceId = GetInstanceID();
            Instances.Add(instanceId, this);

#if UNITY_WEBGL && !UNITY_EDITOR
            string osVersion = CheckMobilePlatformJS();
            Debug.Log("OSVersion=" + osVersion);
            m_IsMobilePlatform = !string.IsNullOrEmpty(osVersion);
            Debug.Log("MobilePlatform:" + m_IsMobilePlatform.ToString());
            CheckOrientation();
#endif
        }

        // Update is called once per frame
        void Update()
        {
#if ENABLE_FPS
            m_FrameCount++;
            float time = Time.realtimeSinceStartup - m_PrevTime;
            if (time >= 0.5f)
            {
                m_Fps = m_FrameCount / time;
                m_FrameCount = 0;
                m_PrevTime = Time.realtimeSinceStartup;
                SetDebugFPS();
            }
#endif
        }

        //================================================
        // [///]
        //================================================
        // システム開始
        public static void YnSysStart()
        {
            //camera.mainを変数に格納
            Camera mainCamera = Camera.main;
            //最初に作った画面のアスペクト比 (縦基準)
            float defaultAspect = DefaultScreenHeight / DefaultScreenWidth;
            //実際の画面のアスペクト比
            float actualAspect = (float)Screen.height / (float)Screen.width;
            //実機とunity画面の比率
            float ratio = actualAspect / defaultAspect;
            //サイズ調整
            mainCamera.orthographicSize /= ratio;

            if (m_goYnSys == null)
            {
                GameObject go = new GameObject();
                go.name = YnSysName;
                go.AddComponent<YnSys>();
                go.AddComponent<YnFade>();
                go.AddComponent<ShTransitionSys>();
                go.AddComponent<SaveDataManager>();
                go.AddComponent<YnsSimpleAudio>();
                go.AddComponent<YnInputManager>();
                go.AddComponent<ShsVibrationSys>();
                m_AppCommon = go.AddComponent<AppCommon>();
                m_goYnSys = go;
                DontDestroyOnLoad(m_goYnSys);

                // CoreManagerにアクセス管理したいものを追加
                CoreManager.I.SetAudioComp(go.GetComponent<YnsSimpleAudio>());
            }
            if(m_goYnSysUI == null)
            {
                GameObject go = (GameObject)Resources.Load(YnSysUIResourcePathName);
                m_goYnSysUI = (GameObject)Instantiate(go);
                m_goYnSysUI.name = YnSysUIResourceName;
                DontDestroyOnLoad(m_goYnSysUI);

                m_goYnSysAppParentCanvas = m_goYnSysUI.transform.Find(AppParentCanvasName).gameObject;
                m_goYnSysDebugTitle = m_goYnSysUI.transform.Find(DebugTitleName).gameObject;
                m_goYnSysDebugTitle.SetActive(false);
                m_goYnSysDebugFPS = m_goYnSysUI.transform.Find(DebugFPSName).gameObject;
                m_goYnSysDebugFPS.SetActive(false);
                m_goYnSysDebugBgBlue = m_goYnSysUI.transform.Find(DebugBlueBgName).gameObject;

                // システムCanvas
                SystemCanvas = m_goYnSysUI.transform.Find("SystemCanvas").gameObject.AddComponent<Sh_SystemCanvasCtrl>();
                SystemCanvas.ProcInitialize();

                m_goYnSysWarning = SystemCanvas.transform.Find("Warning").gameObject;
                m_goYnSysWarning.SetActive(false);
            }

            // デバッグ青背景非表示
            SetDebugBlueBg(false);
        }

        //================================================
        // [///]
        //================================================
        // 
        public static void SetDebugMode(bool _flag)
        {
            m_IsDebugMode = _flag;
        }

        // 
        public static bool IsDebugMode()
        {
            return m_IsDebugMode;
        }

        // 
        public static void SetFromDbgMainMenu(bool _flag)
        {
            m_IsFromDbgMainMenu = _flag;
        }

        // 
        public static bool IsFromDbgMainMenu()
        {
            return m_IsFromDbgMainMenu;
        }

        // 
        public static void SetDebugMainGame(bool _flag)
        {
            m_IsDebugMainGame = _flag;
        }

        // 
        public static bool IsDebugMainGame()
        {
            return m_IsDebugMainGame;
        }
        //================================================
        // [///]
        //================================================
        // GameObject取得
        public static GameObject GetGoYnSys()
        {
            return m_goYnSys;
        }

        // GameObject取得
        public static GameObject GetGoYnSysUI()
        {
            return m_goYnSysUI;
        }

        // AppCommon取得
        public static AppCommon GetAppCommon()
        {
            return m_AppCommon;
        }

        // App3DRoot名取得
        public static string GetApp3DRoot()
        {
            return App3DRoot;
        }

        // GameObject取得
        public static GameObject GetGoYnSysFadeCanvas()
        {
            return m_goYnSysUI.transform.Find("FadeCanvas").gameObject;
        }

        // TransitionCanvas取得
        public static GameObject GetGoTransitionCanvas()
        {
            return m_goYnSysUI.transform.Find("TransitionCanvas").gameObject;
        }

        //================================================
        // [///]
        //================================================
        // 初回起動シーン判定
        public static bool IsFirstScene()
        {
            if(m_SceneNameList == null)
            {
                return true;
            }
            return (m_SceneNameList.Count <= 0) ? true : false;
        }

        // 次のシーン名設定
        public static void SetNextSceneName(string _sceneName)
        {
            if(m_SceneNameList.Count<=0){
                m_SceneNameList.Add(DebugMainMenuName);
            }

            m_SceneNameList.Add(_sceneName);
        }

        // 次のシーン名取得
        public static string GetNextSceneName()
        {
            return m_SceneNameList[m_SceneNameList.Count-1];
        }

        // 次のシーン読み込み
        public static void LoadNextScene()
        {
            string sceneName = m_SceneNameList[m_SceneNameList.Count-1];
            if(IsFromDbgMainMenu())
            {
                ClearSceneList();
                sceneName = DebugMainMenuName;
            }
            if (sceneName != "")
            {
                Debug.Log(sceneName + "へ遷移");
                SceneManager.LoadScene(sceneName);
            }
        }

        // 前のシーンへ
        public static void LoadPrevScene()
        {
            string sceneName = "";
            if(m_SceneNameList.Count>1)
            {
                m_SceneNameList.RemoveAt(m_SceneNameList.Count-1);
                sceneName = m_SceneNameList[m_SceneNameList.Count-1];
            }
            if (IsFromDbgMainMenu())
            {
                ClearSceneList();
                sceneName = DebugMainMenuName;
            }
            if (sceneName != "")
            {
                Debug.Log(sceneName + "へ戻る");
                SceneManager.LoadScene(sceneName);
            }
        }

        // シーンリストクリア
        public static void ClearSceneList()
        {
            for(int i=m_SceneNameList.Count-1; i>=0; --i)
            {
                m_SceneNameList.RemoveAt(i);
            }
        }

        //================================================
        // [///]
        //================================================
        // AppCanvasの親を設定する
        public static void SetAppCanvasParent()
        {
            GameObject go = GameObject.Find(AppCanvasName);
            // 親がDontDestroyOnLoad()設定されているため子にも設定されるので注意
            go.transform.SetParent(m_goYnSysAppParentCanvas.transform,false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition3D = Vector3.zero;
            rt.sizeDelta = Vector2.zero;
            rt.anchorMin = new Vector2(0.0f, 0.0f);
            rt.anchorMax = new Vector2(1.0f, 1.0f);
            rt.localScale = Vector3.one;

            Canvas canvas = go.GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 2;
            m_goYnSysAppCanvas = go;
        }

        // AppCanvasの親を解除する
        public static void ResetAppCanvasParent()
        {
              GameObject go = GameObject.Find(AppCanvasName);
              m_goYnSysAppCanvas.transform.SetParent(null);
              // DontDestroyOnLoad()を解除
              SceneManager.MoveGameObjectToScene(m_goYnSysAppCanvas, SceneManager.GetActiveScene());
              m_goYnSysAppCanvas = null;
        }

        // デバッグ用タイトル名設定
        public static void SetDebugTitleName(string _titleName)
        {
#if ENABLE_FPS
            m_goYnSysDebugTitle.SetActive(IsDebugMode());
            Text text = m_goYnSysDebugTitle.GetComponent<Text>();
            text.text = _titleName;
#endif
        }

        // デバッグ用FPS表示設定
        public static void SetDebugFPS()
        {
            m_goYnSysDebugFPS.SetActive(IsDebugMode());
#if ENABLE_FPS
            Text text = m_goYnSysDebugFPS.GetComponent<Text>();
            text.text = "FPS:" + m_Fps.ToString();
#endif
        }

        // デバッグ用青背景表示
        public static void SetDebugBlueBg(bool _flag)
        {
            m_goYnSysDebugBgBlue.SetActive(_flag);
        }

        public static GameObject GetAppCanvas()
        {
            return m_goYnSysAppCanvas;
        }

        public static void SetAppCtrl(GameObject _go)
        {
            m_goYnSysAppCtrl = _go;
        }

        public static GameObject GetAppCtrl()
        {
            return m_goYnSysAppCtrl;
        }


        // デバッグテキスト
        public static void SetDbgText(string text)
        {
            if (!m_DbgText)
            {
                m_DbgText = m_goYnSysUI.transform.Find(AppDebugCanvasName + "/DebugText").GetComponent<Text>();
            }
            m_DbgText.text = text;
        }

        /// <summary>
        /// CheckOrientationJSの実行結果のコールバック
        /// </summary>
        [MonoPInvokeCallback(typeof(Action<int, string, bool>))]
        private static void OnCheckOrientation(int instanceId, string outputStr, bool isVertical)
        {
            Debug.Log("IsVertical_CheckOrientation:" + isVertical.ToString());

            //Instances[instanceId].m_isSuccess = isSuccess;
            if (isVertical)
            {
                Instances[instanceId].CheckOrientationVertical(outputStr);
            }
            else
            {
                Instances[instanceId].CheckOrientationHorizontal(outputStr);
            }
        }

        // モバイルブラウザ用の縦画面チェック
        private void CheckOrientation()
        {
            Debug.Log("CheckOrientation_Start");
            CheckOrientationJS(GetInstanceID(), OnCheckOrientation);
        }

        // 縦画面の時に呼ばれる
        private void CheckOrientationVertical(string outputStr)
        {
            if (m_IsMobilePlatform)
            {
                m_goYnSysWarning.SetActive(true);
            }
            m_IsVerticalScreen = true;
        }

        // 横画面の時に呼ばれる
        private void CheckOrientationHorizontal(string outputStr)
        {
            if (m_IsMobilePlatform)
            {
                m_goYnSysWarning.SetActive(false);
            }
            m_IsVerticalScreen = false;
        }

    }
}
