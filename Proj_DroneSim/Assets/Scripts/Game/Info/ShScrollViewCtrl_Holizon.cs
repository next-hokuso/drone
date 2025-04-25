using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using static ShsInputController;

// 横一列のビューコントロール
public class ShScrollViewCtrl_Holizon : MonoBehaviour
{
    //================================================
    // [///] 定義
    //================================================
    // フリックチェック位置更新時間
    private const float FlickCheckPosUpdateTime = 1.0f;
    // フリック判定値距離
    private const float FlickCheckDist = 60.0f;
    // フリック判定値角度
    private const float FlickCheckAngle = 48.0f;

    //================================================
    // [///] 定義
    //================================================
    private ShScrollRect m_ScrollRect = null;
    private Scrollbar m_ScrollBar = null;

    private bool m_IsMoving = false;
    private bool m_IsEndCheck = false;

    //------------------------------------------------
    // コンテンツ(対応によって分かれる    
    // フリック関連
    private bool m_IsProcFlickWait = true;
    private Vector3 m_FlickCheckPos = Vector3.zero;
    private float m_FlickCheckPosUpdateTimer = 0.0f;

    // タップ制限
    private GameObject m_NoTapBGObj = null;
    private bool m_IsNoTapActive = true;
    private float m_NoTapActiveTimer = 0.0f;

    // 現在のページ
    private int m_NowPage = 0;
    private int m_MaxPage = 0;

    // インジケーター設定
    private List<Image> m_IndicatorList = null;
    //------------------------------------------------

    //================================================
    // [///]
    //================================================
    private void Start()
    {
    }
    private void OnEnable()
    {
        // スクロールUIが表示された際にページ更新を行う
        {
            // 移動量設定
            float val = (float)m_NowPage / (float)m_MaxPage;
            SetScrollBarValue(val);
            // スクロールの位置設定
            m_ScrollRect.horizontalNormalizedPosition = val;
        }
    }
    private void Update()
    {
        // フリックチェック
        UpdateFlickCheck();

        // タップ制限の解除処理
        UpdateNoTapActive();
    }
    // フリックチェック
    private void UpdateFlickCheck()
    {
        TouchInfo info = GetTouch();
        if (info == TouchInfo.Began)
        {
            m_IsEndCheck = false;

            m_IsProcFlickWait = true;
            // 初期化しておく
            m_FlickCheckPosUpdateTimer = 0.0f;
            m_FlickCheckPos = GetTouchPosition();
        }
        else if (info == TouchInfo.Ended)
        {
            if (m_IsProcFlickWait && !m_IsMoving)
            {
                CheckFlickMove(GetTouchPosition());
            }
        }
        else if (info == TouchInfo.None || info == TouchInfo.Canceled)
        {
            m_IsProcFlickWait = false;
        }

        // フリック関連
        if (m_IsProcFlickWait)
        {
            m_FlickCheckPosUpdateTimer += Time.deltaTime;
            if (m_FlickCheckPosUpdateTimer > FlickCheckPosUpdateTime)
            {
                m_FlickCheckPosUpdateTimer = 0.0f;
                m_FlickCheckPos = GetTouchPosition();
            }
        }
    }
    // タップ制限の解除処理
    private void UpdateNoTapActive()
    {
        if (m_IsNoTapActive)
        {
            m_NoTapActiveTimer += Time.deltaTime;
            if (m_NoTapActiveTimer > FlickCheckPosUpdateTime)
            {
                m_IsNoTapActive = false;
                m_NoTapActiveTimer = 0.0f;
                if (m_NoTapBGObj)
                {
                    m_NoTapBGObj.SetActive(false);
                }
            }
        }
    }

    // スクロールバーのValue変更
    private void SetScrollBarValue(float value)
    {
        // 1ページのみの場合スクロールバー効かないため判定
        if(m_ScrollBar && !IsOnePage())
        {
            m_ScrollBar.value = value;
        }
    }

    // 1ページチェック
    private bool IsOnePage()
    {
        return m_MaxPage == 0;
    }

    //================================================
    // [///]
    //================================================
    // 情報設定
    public void SetInfo(int pageMaxNum)
    {
        // ページ数取得
        m_NowPage = 0;
        m_MaxPage = pageMaxNum - 1;

        // スクロールビューの取得
        m_ScrollRect = GetComponent<ShScrollRect>();

        // スクロールバーの取得/設定
        m_ScrollBar = m_ScrollRect.horizontalScrollbar;
        m_ScrollBar.onValueChanged.AddListener((value) => { OnValueChanged_ScrollBar((float)value); });

        // 対応によって変わる ---------------------------------------------------------------------
        {
            m_IsEndCheck = false;
            m_IsMoving = false;

            // インジケーターの設定
            {
                m_IndicatorList = new List<Image>();
                GameObject imagePre = transform.parent.Find("Grp_Indicator").GetChild(0).gameObject;
                m_IndicatorList.Add(imagePre.GetComponent<Image>());

                // ページ分〇画像を増やす
                for (int i = 1; i <= m_MaxPage; ++i)
                {
                    GameObject go = Instantiate(imagePre);
                    if (go)
                    {
                        go.transform.SetParent(imagePre.transform.parent);
                        go.transform.localEulerAngles = Vector3.zero;
                        go.transform.localScale = Vector3.one;

                        m_IndicatorList.Add(go.GetComponent<Image>());
                    }
                }

                // 1ページのみの場合は非表示
                if(IsOnePage())
                {
                    imagePre.SetActive(false);
                }

                SetIndicator();
            }

            m_NoTapBGObj = transform.Find("Img_TapGuard").gameObject;
            m_NoTapBGObj.SetActive(true);
            m_IsNoTapActive = true;
            m_NoTapActiveTimer = 0.0f;

            // スクロール停止 / フリックで操作
            m_ScrollRect.m_IsDragUpdateStop = true;
        }
    }
    // コンテンツT取得
    public Transform GetContentTransform()
    {
        return m_ScrollRect.content;
    }

    // フリック
    public void CheckFlickMove(Vector3 tapPos)
    {
        m_IsProcFlickWait = false;
        float dist = Vector3.Distance(tapPos,  m_FlickCheckPos);

        //float abs_x = Mathf.Abs(dist.x);
        //float abs_y = Mathf.Abs(dist.y);
        float threshold = FlickCheckDist;

        // 距離チェック
        if ((dist >= threshold))
        {
            // 角度チェック
            Vector3 dir = (tapPos - m_FlickCheckPos);
            Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0.0f, dir.y));
            rot.eulerAngles = Vector3.up * (int)rot.eulerAngles.y;
            float checkAngle = rot.eulerAngles.y;

            // 方向チェック
            if ((0 <= checkAngle) || (checkAngle <= 360))
            {
                // 右 48以上96以下
                if(FlickCheckAngle <= checkAngle && checkAngle <= FlickCheckAngle * 2.0f)
                {
                    if (IsChangeLeft())
                    {
                        m_NowPage--;
                        StartCoroutine(ProcChangeContent());
                    }
                }
                // 左 264.0f ～ 312.0f 
                else 
                if((360.0f - FlickCheckAngle * 2.0f <= checkAngle) &&
                   (360.0f - FlickCheckAngle >= checkAngle))
                {
                    if (IsChangeRight())
                    {
                        m_NowPage++;
                        StartCoroutine(ProcChangeContent());
                    }
                }
            }
        }
    }
    // ページ加算できるかチェック
    private bool IsChangeRight()
    {
        // ページ加算チェック
        return m_NowPage + 1 <= m_MaxPage;
    }
    // ページ減算できるかチェック
    private bool IsChangeLeft()
    {
        // ページ減算チェック
        return m_NowPage - 1 >= 0;
    }

    // 横移動
    public float OnValueChanged_ScrollBar(float value)
    {
        // 移動中
        if (m_IsMoving)
        {
            return m_ScrollBar.value;
        }

        // 移動フラグ設定
        m_IsEndCheck = true;

        float val = value;
        SetScrollBarValue(val);
        return val;
    }

    // スクロール処理
    private IEnumerator ProcScroll(float endVal)
    {
        float timer = 0.0f;
        float endTime = 0.25f;

        float nowVal = m_ScrollBar.value;
        float addVal = endVal - nowVal;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > endTime) { break; }

            SetScrollBarValue(nowVal + (timer / endTime * addVal));
            yield return null;
        }
        SetScrollBarValue(endVal);
    }

    // チェンジ
    private IEnumerator ProcChangeContent()
    {
        if (IsOnePage()) yield break;

        m_IsMoving = true;

        SetIndicator();

        // 移動量取得
        float val = (float)m_NowPage / (float)m_MaxPage;
        yield return ProcScroll(val);

        // コンテンツを変更
        m_IsMoving = false;
    }

    // ページに合ったスクロール値を設定
    public void SetScrollValue(int pageNum)
    {
        m_NowPage = Mathf.Clamp(pageNum, 0 , m_MaxPage);

        SetIndicator();

        // 移動量取得
        float val = (float)m_NowPage / (float)m_MaxPage;
        SetScrollBarValue(val);
    }

    //================================================
    // [///] コンテンツ変更
    //================================================
    // インジケーターの色設定
    private void SetIndicator()
    {
        int idx = 0;
        Color col = Color.green;
        foreach(Image img in m_IndicatorList)
        {
            if(idx == m_NowPage)
            {            
                ColorUtility.TryParseHtmlString("#1BB5AB", out col);
                img.color = col;
            }
            else
            {
                ColorUtility.TryParseHtmlString("#4F4F4F", out col);
                img.color = col;
            }

            idx++;
        }
    }
        
    //================================================
    // [///] 表示更新
    //================================================
    // 情報設定
    public void SetInit()
    {
        m_IsEndCheck = false;
        m_IsMoving = false;
        SetScrollBarValue(0.0f);

        m_NoTapBGObj = transform.Find("Img_NoTapBG").gameObject;
        m_NoTapBGObj.SetActive(true);
        m_IsNoTapActive = true;
        m_NoTapActiveTimer = 0.0f;
    }

    //// 外部からの変更
    //public void SetChangeLeft()
    //{
    //    m_IsEndCheck = false;
    //    StartCoroutine(ProcChangeContent_Left());
    //}
}
