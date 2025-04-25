using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yns;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    // 処理フェーズ値
    private enum MenuPhase
    {
        None,
        Start,
        Update,
        End,
    }
    // 処理フェーズ
    private MenuPhase m_MenuPhase = MenuPhase.None;

    private float m_Timer = 0.0f;
    
    private Image m_Logo = null;

    //================================================
    // [///]
    //================================================
    private void Awake()
    {
    }

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("SplashScreen Start");
        m_MenuPhase = MenuPhase.Start;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_MenuPhase)
        {
            case MenuPhase.Start:
                //StartCoroutine("ProcFadeIn");
                ShTransitionSys.SetFillOutCover();
                m_MenuPhase = MenuPhase.Update;
                break;

            case MenuPhase.Update:
                m_Timer += Time.deltaTime;
                if (m_Timer > 1.5f)
                {
                    m_Timer = 0.0f;
                    // StartCoroutine("ProcFadeOut");
                    m_MenuPhase = MenuPhase.End;
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

    public void TransitionLoad()
    {
        m_MenuPhase = MenuPhase.None;

        //#if UNITY_WEBGL && !UNITY_EDITOR
            ShTransitionSys.SetYnsysLoadNextScene("Login", 0.25f);
            ShTransitionSys.SetLoadScene(true);
            ShTransitionSys.ChangeCoverColorBlack();
            ShTransitionSys.SetTransition(0.5f, this, "ProcTransitionLoad1", false, false, false);
        //#else
        //    ShTransitionSys.SetYnsysLoadNextScene("MainGame", 0.25f);
        //    ShTransitionSys.SetLoadScene(false);
        //    ShTransitionSys.ChangeCoverColorWhite();
        //    ShTransitionSys.SetTransition(0.5f, this, "ProcTransitionLoad2", false, false, false);
        //#endif

    }

    private IEnumerator ProcTransitionLoad1()
    {
        // 破棄
        yield break;
    }
    private IEnumerator ProcTransitionLoad2()
    {
        ChangeScene_Title.SetChangeScene();

        // 破棄
        yield break;
    }
}
