using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yns;

public class ShTransitionProc : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    public enum FillProcName
    {
        FillProc_Fade,
        //FillProc_Radius_InR,
    }
    private FillProcName m_NowFillProc = FillProcName.FillProc_Fade;

    //================================================
    // [///]
    //================================================
    private bool m_IsTransitionEnd = true;

    private float m_CoverTime = 0.0f;
    private float m_Timer = 0.0f;

    private Image m_CoverImage = null;
    private Color m_CoverBaseCol = Color.black;

    // 画面覆い中に行うコルーチン関連
    private MonoBehaviour m_IncludeCoroutineProc = null;
    private string m_UseCoroutine = "";

    // next scene
    private bool m_IsYnSysNextScene = false;
    private string m_YnSysNextSceneName = "";
    private float m_YnSysNextSceneWaitTime = 0.0f;

    // ロード画面
    private GameObject m_Loading = null;
    //  ロード画面有無
    private bool m_IsLoadEffect = true;
    //  ロード画面
    private bool m_IsLoadStart = false;
    //  ロード画面黒バック有無
    private bool m_IsLoadBlack = true;
    //  ローディングバー
    //private ShsLodingBarCtrl m_LoadingBarCtrl = null;

    // 画面覆い処理プロック名
    private string m_LoadSceneFillInProcName = "";
    private string m_LoadSceneFillOutProcName = "";

    // カバーのfadeOut等外したか
    public bool IsFillOut { get; private set; } = true;
    // シーンを読み込み終わったか
    public bool IsSceneLoad { get; private set; } = false;

    // addflag
    public bool m_IsLoadScene = false;
    //
    private bool m_IsFillOutAuto = false;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
        m_CoverImage = gameObject.GetComponent<Image>();
        m_CoverImage.fillAmount = 0.0f;
        m_CoverImage.gameObject.SetActive(false);

        m_Loading = transform.parent.Find("Loading").gameObject;
        //m_LoadingBarCtrl = m_Loading.GetComponentInChildren<ShsLodingBarCtrl>();

        m_LoadSceneFillInProcName = "ProcFadeInTs";
        m_LoadSceneFillOutProcName = "ProcFadeOutTs";
    }

    // Update is called once per frame
    void Update()
    {
    }

    //================================================
    // [///]
    //================================================
    // 更新処理
    private IEnumerator UpdateProc()
    {
        // 画面を覆う処理
        yield return StartCoroutine(m_LoadSceneFillInProcName);

        // 画面覆い中に行う処理
        if (m_IncludeCoroutineProc)
        {
            // ローディングバーのリセット
            //m_LoadingBarCtrl.ProcReset();
            m_Loading.SetActive(m_IsLoadEffect);

            if (m_IsLoadEffect)
            {
                // 画面の覆いをはがす処理
                yield return StartCoroutine(m_LoadSceneFillOutProcName);
                yield return null;
            }
            m_IsLoadStart = true;

            if (!string.IsNullOrEmpty(m_UseCoroutine))
            {
                yield return m_IncludeCoroutineProc.StartCoroutine(m_UseCoroutine);
            }
        }

        // 画面ロード
        if (m_IsYnSysNextScene && m_IsLoadScene)
        {
            yield return StartCoroutine("YnSysLoadNextScene");
            IsSceneLoad = true;
        }

        if (m_IsFillOutAuto)
        {
            StartCoroutine(ProcFillOut());
        }
    }

    // 更新処理(新規シーンの場合:シーンを読み込んでから初期設定するためFillOutを別にする
    private IEnumerator ProcTransitionScene()
    {
        // 画面を覆う処理
        yield return StartCoroutine(m_LoadSceneFillInProcName);

        // 画面覆い中に行う処理
        if (m_IncludeCoroutineProc)
        {
            // ローディングバーのリセット
            //m_LoadingBarCtrl.ProcReset();
            m_Loading.SetActive(m_IsLoadEffect);

            if (m_IsLoadEffect)
            {
                // 画面の覆いをはがす処理
                yield return StartCoroutine(m_LoadSceneFillOutProcName);
                yield return null;
            }
            m_IsLoadStart = true;

            yield return m_IncludeCoroutineProc.StartCoroutine(m_UseCoroutine);
        }

        // 画面ロード
        if (m_IsYnSysNextScene)
        {
            yield return StartCoroutine("YnSysLoadNextScene");
            IsSceneLoad = true;
        }

        //StartCoroutine(ProcFillOut());
    }
    // 画面の覆いを剥がす処理
    private IEnumerator ProcFillOut()
    {
        // ロード画面OFF
        if (m_Loading.activeSelf)
        {
            // 画面を覆う処理
            yield return StartCoroutine(m_LoadSceneFillInProcName);
            yield return null;

            m_Loading.SetActive(false);
            yield return null;
        }

        // 画面の覆いをはがす処理
        yield return StartCoroutine(m_LoadSceneFillOutProcName);

        // 終了処理
        m_CoverImage.gameObject.SetActive(false);

        IsFillOut = true;
        m_IsTransitionEnd = true;
    }

    //================================================
    // [///]
    //================================================
    // 遷移設定
    public void SetTransition(float _coverTime, MonoBehaviour _includeCoroutineProc = null, 
        string _coroutineName = "", bool _isLoadEff = true, bool _isLoadBlack = true, bool _isFillOutAuto=true)
    {
        // 処理中
        if (!m_IsTransitionEnd)
        {
            //return;
        }

        // 設定
        m_CoverTime = _coverTime;
        m_IncludeCoroutineProc = _includeCoroutineProc;
        m_UseCoroutine = _coroutineName;
        m_IsLoadEffect = _isLoadEff;
        m_IsLoadBlack = _isLoadBlack;
        // ロード画面黒バック
        m_Loading.transform.Find("Image").GetComponent<Image>().enabled = m_IsLoadBlack;

        // 初期化
        m_Timer = 0.0f;
        m_CoverImage.fillAmount = 0.0f;
        m_CoverImage.enabled = true; //_coverTime > 0.0f;
        m_CoverImage.gameObject.SetActive(true);
        m_IsLoadStart = false;

        // 開始
        IsFillOut = false;
        m_IsFillOutAuto = _isFillOutAuto;
        IsSceneLoad = false;
        m_IsTransitionEnd = false;
        StartCoroutine("UpdateProc");
    }
    public void SetTransition(float _coverTime, bool _isLoadEff = true, bool _isFillOutAuto = true)
    {
        // 処理中
        if (!m_IsTransitionEnd)
        {
            //return;
        }

        // 設定
        m_CoverTime = _coverTime;
        m_IncludeCoroutineProc = null;
        m_UseCoroutine = "";
        m_IsLoadEffect = _isLoadEff;
        m_IsLoadBlack = false;
        // ロード画面黒バック
        m_Loading.transform.Find("Image").GetComponent<Image>().enabled = false;

        // 初期化
        m_Timer = 0.0f;
        m_CoverImage.fillAmount = 0.0f;
        m_CoverImage.enabled = true; //_coverTime > 0.0f;
        m_CoverImage.gameObject.SetActive(true);
        m_IsLoadStart = false;

        // 開始
        IsFillOut = false;
        m_IsFillOutAuto = _isFillOutAuto;
        IsSceneLoad = false;
        m_IsTransitionEnd = false;
        StartCoroutine("UpdateProc");
    }

    // 遷移終了判定
    public bool IsTransitionEnd()
    {
        return m_IsTransitionEnd;
    }

    // ロード開始チェック
    public bool IsLoadStart()
    {
        return m_IsLoadStart;
    }

    // 画面覆いを剥がす時間の変更
    public void SetFillOutCoverTime(float _time)
    {
        m_CoverTime = _time;
    }
    //================================================
    // [///]
    //================================================
    // 画面覆い処理後、YnSys用の画面遷移を呼び出す設定
    public void SetYnsysLoadNextScene(string _nextSceneName, float _waitTime = 0.0f)
    {
        m_IsYnSysNextScene = true;
        m_YnSysNextSceneName = _nextSceneName;
        m_YnSysNextSceneWaitTime = _waitTime;
    }

    // 画面覆い処理後、シーン生成と待機
    private IEnumerator YnSysLoadNextScene()
    {
        YnSys.SetNextSceneName(m_YnSysNextSceneName);
        YnSys.LoadNextScene();

        yield return new WaitForSeconds(m_YnSysNextSceneWaitTime);

        m_IsYnSysNextScene = false;
    }

    // ロード終了,画面覆い外し
    public void ProcFillOutCover()
    {
        StartCoroutine(ProcFillOut());
    }

    //================================================
    // [///] シーンのリセット処理
    //================================================
    // シーン更新待機
    public void SetSceneReset(AppCommon.LoadSceneNo sceneNo)
    {
        StartCoroutine(ProcSceneReset(sceneNo));
    }
    private IEnumerator ProcSceneReset(AppCommon.LoadSceneNo sceneNo)
    {
        if (YnSys.m_AppCommon.m_RootCanvas[(int)sceneNo] != null)
        {
            YnSys.m_AppCommon.UnloadScenes(sceneNo);
        }

        while (true)
        {
            if (YnSys.m_AppCommon.IsEndUnloadScenes(sceneNo))
            {
                break;
            }
            yield return null;
        }

        if (YnSys.m_AppCommon.m_RootCanvas[(int)sceneNo] == null)
        {
            YnSys.m_AppCommon.LoadScenes(sceneNo);
        }
        while (true)
        {
            if (!YnSys.m_AppCommon.IsEndUnloadScenes(sceneNo))
            {
                break;
            }
            yield return null;
        }
    }

    //================================================
    // [///] 画面覆い関連
    //================================================
    // 覆いの色変更
    public enum ShTrsSys_Color
    {
        Black,
        White,
    }
    public void ChangeCoverColor(ShTrsSys_Color col)
    {
        switch (col)
        {
            case ShTrsSys_Color.Black:
                m_CoverBaseCol = Color.black;
                break;
            case ShTrsSys_Color.White:
                m_CoverBaseCol = Color.white;
                break;
        }
    }
    // 使用プロック変更
    public void ChangeFillProc(FillProcName name)
    {
        m_NowFillProc = name;
        switch (m_NowFillProc)
        {
            case FillProcName.FillProc_Fade:
                m_LoadSceneFillInProcName = "ProcFadeInTs";
                m_LoadSceneFillOutProcName = "ProcFadeOutTs";
                break;

            // case FillProcName.FillProc_Radius_InR:
            //     m_LoadSceneFillInProcName = "ProcFillInRadial360Rs";
            //     m_LoadSceneFillOutProcName = "ProcFillOutRadial360Rs";
            //     break;
        }
    }

    /// <summary>
    ///  キャンバスのイメージオブジェクトを
    ///  フェードインアウトに合わせている為
    ///  円でのフィルインアウトを使用する場合は
    ///  リソース側変更対応を行う必要がある
    ///   > Resources/_Sh/TransitionCanvas
    /// </summary>
    // イメージを円状にフィルイン
    public IEnumerator ProcFillInRadial360Rs()
    {
        // 設定
        m_Timer = 0.0f;

        m_CoverImage.type = Image.Type.Filled;
        m_CoverImage.fillMethod = Image.FillMethod.Radial360;
        m_CoverImage.fillOrigin = 2;
        m_CoverImage.fillAmount = 0.0f;
        m_CoverImage.fillClockwise = true;

        // 処理
        while (true)
        {
            m_Timer += Time.deltaTime;
            m_CoverImage.fillAmount = m_Timer / m_CoverTime;
            if (m_Timer > m_CoverTime)
            {
                m_CoverImage.fillAmount = 1.0f;
                break;
            }
            yield return null;
        }
    }
    // イメージを円状にフィルアウト
    public IEnumerator ProcFillOutRadial360Rs()
    {
        // 設定
        m_Timer = m_CoverTime;

        m_CoverImage.type = Image.Type.Filled;
        m_CoverImage.fillMethod = Image.FillMethod.Radial360;
        m_CoverImage.fillOrigin = 2;
        m_CoverImage.fillAmount = 1.0f;
        m_CoverImage.fillClockwise = false;

        // 処理
        while (true)
        {
            m_Timer -= Time.deltaTime;
            m_CoverImage.fillAmount = m_Timer / m_CoverTime;
            if (m_Timer < 0.0f)
            {
                m_CoverImage.fillAmount = 0.0f;
                break;
            }
            yield return null;
        }
    }

    // 縦
    public IEnumerator ProcFillOutVerticalTs()
    {
        // 設定
        m_Timer = m_CoverTime;

        m_CoverImage.type = Image.Type.Filled;
        m_CoverImage.fillMethod = Image.FillMethod.Vertical;
        m_CoverImage.fillOrigin = 0;
        m_CoverImage.fillAmount = 1.0f;

        // 処理
        while (true)
        {
            m_Timer -= Time.deltaTime;
            m_CoverImage.fillAmount = m_Timer / m_CoverTime;
            if (m_Timer < 0.0f)
            {
                m_CoverImage.fillAmount = 0.0f;
                break;
            }
            yield return null;
        }
    }

    // イメージを白色固定(白またはテクスチャのみ)でフェードイン
    public IEnumerator ProcFadeInTs()
    {
        // 設定
        m_Timer = 0.0f;

        // 処理
        while (true)
        {
            m_Timer += Time.deltaTime;
            m_CoverImage.color = m_CoverBaseCol - Color.black * (1.0f - m_Timer / m_CoverTime);
            if (m_Timer > m_CoverTime)
            {
                m_CoverImage.color = m_CoverBaseCol;
                break;
            }
            yield return null;
        }
    }
    // イメージを白色固定(白またはテクスチャのみ)でフェードアウト
    public IEnumerator ProcFadeOutTs()
    {
        // 設定
        m_Timer = m_CoverTime;

        // 処理
        while (true)
        {
            m_Timer -= Time.deltaTime;
            m_CoverImage.color = m_CoverBaseCol - Color.black * (1.0f - m_Timer / m_CoverTime);
            if (m_Timer < 0.0f)
            {
                m_CoverImage.color = Color.black * 0.0f;
                break;
            }
            yield return null;
        }
    }

}