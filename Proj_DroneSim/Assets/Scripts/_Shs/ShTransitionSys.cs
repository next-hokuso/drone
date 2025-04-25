using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yns;

public class ShTransitionSys : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    // 画面を覆う/はがす時間
    private const float DefaultCoverTime = 0.5f;
    private const string DefaltCoverName = "Image_Cover";

    private static ShTransitionProc m_Proc = null;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
        GameObject go = YnSys.GetGoTransitionCanvas().transform.Find(DefaltCoverName).gameObject;
        m_Proc = go.GetComponent<ShTransitionProc>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    //================================================
    // [///]
    //================================================
    // 遷移中判定
    public static bool IsTransitionEnd()
    {
        return m_Proc.IsTransitionEnd();
    }

    // 遷移中判定
    public static bool IsLoadStart()
    {
        return m_Proc.IsLoadStart();
    }

    // シーンを読み込んだか
    public static bool IsLoadScene()
    {
        return m_Proc.IsSceneLoad;
    }

    // 画面の覆いがはずれたか
    public static bool IsFillOutCover()
    {
        return m_Proc.IsFillOut;
    }

    // 画面覆いを剥がす呼び出し
    public static void SetFillOutCover(bool _isSetCoverTime = false, float _changeCoverTime = 1.0f)
    {
        if (_isSetCoverTime)
        {
            m_Proc.SetFillOutCoverTime(_changeCoverTime);
        }
        m_Proc.ProcFillOutCover();
    }

    // 仮
    public static void SetLoadScene(bool isFlag)
    {
        m_Proc.m_IsLoadScene = isFlag;
    }

    /// <summary>
    /// 遷移設定
    /// </summary>
    /// <param name="_coverTime">画面を覆う/覆いをはがすのにかかる時間</param>
    /// <param name="_includeCoroutineProc">画面を覆った後はがすまでに入れるコルーチン処理があるスクリプト</param>
    /// <param name="_coroutineName">呼び出すコルーチン処理名</param>
    public static void SetTransition(
        float _coverTime = DefaultCoverTime, MonoBehaviour _includeCoroutineProc = null,
        string _coroutineName = "", bool _isLoadEff = true, bool _isLoadBlack = true, bool _isFillOutAuto=true)
    {
        // 保険
        if (!_includeCoroutineProc)
        {
            _coroutineName = "";
        }

        m_Proc.SetTransition(_coverTime, _includeCoroutineProc, _coroutineName, _isLoadEff, _isLoadBlack, _isFillOutAuto);
    }

    /// <summary>
    /// 遷移設定2 : 画面を覆ってロード画面を表示するまでのみ
    /// </summary>
    /// <param name="_coverTime">画面を覆う/覆いをはがすのにかかる時間</param>
    public static void SetTransition(float _coverTime = DefaultCoverTime,
        bool _isLoadEff = true, bool _isFillOutAuto = true)
    {
        m_Proc.SetTransition(_coverTime, _isLoadEff, _isFillOutAuto);
    }

    /// <summary>
    /// YnSysの画面呼び出し     @@@@@@仮
    /// シーンに乗っけたシーンに遷移する時想定の遷移処理のため
    /// YnSysの画面破棄処理とはかみ合っていないため
    /// 使用を推奨しない
    /// 使用する場合、SetTransitionの_coverTimeに齟齬が発生するため気を付けること
    /// </summary>
    /// <param name="_nextSceneName"></param>
    /// <param name="_waitTime"></param>
    public static void SetYnsysLoadNextScene(string _nextSceneName, float _waitTime = 0.0f)
    {
        m_Proc.SetYnsysLoadNextScene(_nextSceneName, _waitTime);
    }

    /// <summary>
    /// シーンのリセット処理
    /// </summary>
    /// <param name="sceneNo"></param>
    public static void SetSceneReset(AppCommon.LoadSceneNo sceneNo)
    {
        m_Proc.SetSceneReset(sceneNo);
    }

    /// <summary>
    /// 画面覆い処理の変更
    /// </summary>
    /// <param name="name"></param>
    public static void ChangeFillProc(ShTransitionProc.FillProcName name)
    {
        m_Proc.ChangeFillProc(name);
    }

    //================================================
    // [///] カスタム設定
    //================================================
    // 画面覆い画像の色変更
    public static void ChangeCoverColorBlack()
    {
        m_Proc.ChangeCoverColor(ShTransitionProc.ShTrsSys_Color.Black);
    }
    public static void ChangeCoverColorWhite()
    {
        m_Proc.ChangeCoverColor(ShTransitionProc.ShTrsSys_Color.White);
    }
}
