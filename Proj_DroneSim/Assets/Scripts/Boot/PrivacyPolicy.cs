using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yns;

public class PrivacyPolicy : MonoBehaviour
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

    //================================================
    // [///]
    //================================================
    private void Awake()
    {
    }

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("PrivacyPolicy Start");
        m_MenuPhase = MenuPhase.Start;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_MenuPhase)
        {
            case MenuPhase.Start:
                m_MenuPhase = MenuPhase.Update;
                break;

            case MenuPhase.Update:
                m_MenuPhase = MenuPhase.End;
                break;

            case MenuPhase.End:
                // @正式版
                TransitionLoad();
                m_MenuPhase = MenuPhase.None;
                break;

        }
    }

    public void TransitionLoad()
    {
        m_MenuPhase = MenuPhase.None;
        ShTransitionSys.SetYnsysLoadNextScene("MainGame", 0.0f);
        YnFade.SetFadeIn();
        ShTransitionSys.SetLoadScene(false);

        ShTransitionSys.SetTransition(0.2f, this, "ProcTransitionLoad", false, false, false);
    }

    private IEnumerator ProcTransitionLoad()
    {
        YnSys.m_AppCommon.LoadScene_MainGame();

        // 破棄
        yield break;
    }
}
