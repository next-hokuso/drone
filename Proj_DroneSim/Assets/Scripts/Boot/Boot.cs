using MainGame;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Yns;

public class Boot : MonoBehaviour
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
    void Start()
    {
        // 1203x677

        Debug.Log("Boot Start");
        YnSys.YnSysStart();

        m_MenuPhase = MenuPhase.Start;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_MenuPhase)
        {
            case MenuPhase.Start:
                // Audio
                MG_Mediator.SetAudio(Yns.YnSys.GetGoYnSys().GetComponent<YnsSimpleAudio>());
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

        #if true //UNITY_WEBGL && !UNITY_EDITOR
            ShTransitionSys.SetYnsysLoadNextScene("Login", 0.25f);
            YnFade.SetFadeIn();
            ShTransitionSys.SetLoadScene(true);
            ShTransitionSys.ChangeCoverColorBlack();
            ShTransitionSys.SetTransition(0.5f, this, "ProcTransitionLoad", false, false, false);
        #else
            ShTransitionSys.SetYnsysLoadNextScene("MainGame", 0.25f);
            YnFade.SetFadeIn();
            ShTransitionSys.SetLoadScene(false);
            ShTransitionSys.ChangeCoverColorBlack();
            ShTransitionSys.SetTransition(0.5f, this, "ProcTransitionLoad2", false, false, false);
        #endif
    }

    private IEnumerator ProcTransitionLoad()
    {
        // 破棄
        yield break;
    }
    private IEnumerator ProcTransitionLoad2()
    {
        AppData.m_PlayMode = AppData.PlayMode.Game_Treasure;
        ChangeScene_Treasure.SetChangeScene();

        // 破棄
        yield break;
    }
}
