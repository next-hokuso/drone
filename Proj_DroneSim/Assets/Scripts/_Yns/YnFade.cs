using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yns;

public class YnFade : MonoBehaviour
{
    //================================================
    // [///]
    //================================================

    const string DefaltFadeBgName = "FadeScreenBlack";

    const float DefaultFadeTime = 0.5f;

    static YnFadeScript m_FadeScript = null;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
        GameObject go = YnSys.GetGoYnSysFadeCanvas().transform.Find(DefaltFadeBgName).gameObject;
        m_FadeScript = go.GetComponent<YnFadeScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //================================================
    // [///]
    //================================================
    // フェード中判定
    public static bool IsFadeEnd()
    {
        return m_FadeScript.IsFadeEnd();
    }

    // α値設定
    static void SetFadeAlpha(float _alpha)
    {
        m_FadeScript.SetFadeAlpha(_alpha);
    }

    // フェードイン
    public static void SetFadeIn(float _fadeTime=DefaultFadeTime)
    {
        m_FadeScript.SetFadeIn(_fadeTime);
    }

    // フェードアウト
    public static void SetFadeOut(float _fadeTime=DefaultFadeTime)
    {
        m_FadeScript.SetFadeOut(_fadeTime);
    }

}
