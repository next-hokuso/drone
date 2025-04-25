using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yns;

public class YnFadeScript : MonoBehaviour
{
    //================================================
    // [///]
    //================================================

    const float DefaultFadeTime = 0.5f;

    int m_FadeState;
    float m_FadeTime;
    float m_FadeTotalTime;

    RawImage m_RawImage = null;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
        this.m_FadeState = 0;
        this.m_RawImage = this.gameObject.GetComponent<RawImage>();

        // 更新処理
        StartCoroutine("UpdateProc");
    }

    // Update is called once per frame
    void Update()
    {

    }

    //================================================
    // [///]
    //================================================
    // 更新処理
    public IEnumerator UpdateProc()
    {
        while (true)
        {
            if (this.m_FadeState != 0)
            {
                this.m_FadeTime -= Time.fixedDeltaTime;
                if (this.m_FadeTime < 0.0f)
                {
                    this.m_FadeTime = 0.0f;
                }
                float alpha = Mathf.Lerp(this.m_FadeState == 1 ? 1.0f : 0.0f,
                                         this.m_FadeState == 1 ? 0.0f : 1.0f,
                                         1.0f - (this.m_FadeTime / this.m_FadeTotalTime));
                SetFadeAlpha(alpha);

                if (this.m_FadeTime <= 0.0f)
                {
                    this.m_FadeState = 0;
                }
            }
            yield return null;
        }
    }

    //================================================
    // [///]
    //================================================
    // フェード中判定
    public bool IsFadeEnd()
    {
        return this.m_FadeState == 0 ? true : false;
    }

    // α値設定
    public void SetFadeAlpha(float _alpha)
    {
        Color col = this.m_RawImage.color;
        col.a = _alpha;
        this.m_RawImage.color = col;
        this.m_RawImage.enabled = (_alpha <= 0.0f) ? false : true;
    }

    // フェードイン
    public void SetFadeIn(float _fadeTime = DefaultFadeTime)
    {
        this.m_FadeState = 1;
        this.m_FadeTime = _fadeTime;
        this.m_FadeTotalTime = this.m_FadeTime;
        SetFadeAlpha(1.0f);
    }

    // フェードアウト
    public void SetFadeOut(float _fadeTime = DefaultFadeTime)
    {
        this.m_FadeState = 2;
        this.m_FadeTime = _fadeTime;
        this.m_FadeTotalTime = this.m_FadeTime;
        SetFadeAlpha(0.0f);
    }

}