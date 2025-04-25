using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shs;
using UnityEngine.UI;
using MainGame;
using Game;
using UnityEngine.Android;

/// <summary>
/// </summary>
public class WipeCanvasCtrl : ShCanvasBaseCtrl, IShVisible
{
    //================================================
    // [///]  各シーンごとに保持する変数
    //================================================
    // IShVisibleで使用する表示変数
    public bool IsVisible { get; private set; } = false;

    // 表示制御としてリスト追加するものの設定
    private enum UINo
    {
        Canvas,

        Wipe,
        Wipe2,

        // グリッド
        Grp_Grid,
    }
    protected ShUIList m_UIList = new ShUIList();

    // 自身
    protected Canvas m_ThisCanvas = null;

    // ワイプ
    private bool m_IsCurrentWipeGameCam = false;
    // ドローンカメラ
    private GameObject m_DroneCamera = null;
    private Camera m_GameCamera = null;

    RenderTexture m_DroneRenderTexture = null;
    RenderTexture m_GameRenderTexture = null;

    //================================================
    // [///] シーンの初期化
    //================================================
    public void Awake()
    {
    }

    public override void ProcInitialize()
    {
        // 自身
        m_ThisCanvas = Shs.ShSceneUtils.GetSceneRootObj("MainGame", "AddCanvas").
            transform.Find("WipeCanvas").GetComponent<Canvas>();


        // 自身から子供の表示制御の付与取得
        {
            Transform canvasT = m_ThisCanvas.transform;

            // リストへの登録
            m_UIList.Clear();
            {
                m_UIList.Add(canvasT.gameObject.AddComponent<ShUIBaseCtrl>());
                m_UIList.Add(canvasT.Find("Wipe").gameObject.AddComponent<ShUIBaseCtrl>());
                m_UIList.Add(canvasT.Find("Wipe2").gameObject.AddComponent<ShUIBaseCtrl>());

                // grid
                m_UIList.Add(canvasT.Find("Grp_Grid").gameObject.AddComponent<ShUIBaseCtrl>());
            }

            // 初期化
            m_UIList.ProcInitialize();

            // 個別設定(ボタンなど) @TODO 処理を移動/改善する
            {
                Transform camRoot = Shs.ShSceneUtils.GetSceneRootObj("MainGame", "CamRoot").transform;
                m_DroneCamera = camRoot.Find("ObjCamera2_Drone").gameObject;
                m_DroneCamera.SetActive(false);

                m_GameCamera = camRoot.Find("ObjCamera").GetComponent<Camera>();

                Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(
                    canvasT.Find("Wipe").GetComponent<Button>(), OnClick_Wipe);
                Shs.ShUIUtils.ProcAddListner_And_BtnDefaultSetting(
                    canvasT.Find("Wipe2").GetComponent<Button>(), OnClick_Wipe);

                // モバイル設定
                if (Yns.YnSys.m_IsMobilePlatform)
                {
                    // 表示を上げる
                    canvasT.Find("Img_BG").transform.localPosition += Vector3.up * 50.0f;
                    canvasT.Find("Wipe").transform.localPosition += Vector3.up * 50.0f;
                    canvasT.Find("Wipe2").transform.localPosition += Vector3.up * 50.0f;
                    canvasT.Find("Grp_Grid").transform.localPosition += Vector3.up * 50.0f;
                }
            }

            // 非表示
            m_UIList.Hide();
        }

        // Resorce Load
        //StartCoroutine("ProcResourcesLoad", true);
    } 

    //================================================
    // [///] シーンに必要なリソース読み込み
    //================================================
    public override IEnumerator ProcResourcesLoad()
    {
        // 読み込み完了
        m_IsCompleteSceneSetup = true;

        yield break;
    }

    //================================================
    // [///] 表示切り替え
    //================================================
    void IShVisible.Show()
    {
        m_UIList.Show();

        m_UIList.m_List[(int)UINo.Wipe2].gameObject.SetActive(false);

        // Grid設定:ドローンカメラの場合
        if (!m_IsCurrentWipeGameCam)
        {
            m_UIList.m_List[(int)UINo.Grp_Grid].SetVisible(AppData.GridDisplay);
        }
    }
    void IShVisible.Hide()
    {
        m_UIList.Hide();
    }

    //================================================
    // [///] シーンの処理開始
    //================================================
    public override void ProcSceneProcStart()
    {
    }
    // リセット
    public override void ProcSceneReset()
    {
    }
    // クリア時
    public void ProcGameClear()
    {
    }
    // リザルト待機
    public void ProcResultWait()
    {
    }

    //================================================
    // [///] 開始演出
    //================================================

    //================================================
    // [///] TODO:実装部分のため他に移したい
    //================================================
    public void SetDroneCameraWipeRenderTexture(RenderTexture texture)
    {
        m_DroneRenderTexture = texture;
        m_UIList.m_List[(int)UINo.Wipe].GetComponent<RawImage>().texture = texture;
    }
    public void SetGameCameaWipeRenderTexture(RenderTexture texture)
    {
        m_GameRenderTexture = texture;
        m_UIList.m_List[(int)UINo.Wipe2].GetComponent<RawImage>().texture = texture;
    }

    // ワイプ変更
    public void SetChangeWipe()
    {
        // ゲームカメラ/ドローンカメラワイプ
        if (m_IsCurrentWipeGameCam)
        {
            // drone cam on
            m_DroneCamera.SetActive(true);
            // gamecameraにrendertexture設定
            m_GameCamera.targetTexture = m_GameRenderTexture;
            m_UIList.m_List[(int)UINo.Wipe2].gameObject.SetActive(true);

            // gamecamera off
            m_UIList.m_List[(int)UINo.Wipe].gameObject.SetActive(false);
        }
        // ドローンカメラ/ゲームカメラワイプ
        else
        {
            // gamecamera off
            // gamecameraのrendertexture設定はずす
            m_GameCamera.targetTexture = null;
            m_UIList.m_List[(int)UINo.Wipe].gameObject.SetActive(true);

            // drone off
            m_DroneCamera.SetActive(false);
            m_UIList.m_List[(int)UINo.Wipe2].gameObject.SetActive(false);
        }

        // グリッドの反映
        MainGame.MG_Mediator.MainCanvas.SetCurrentWipeGameCamera(m_IsCurrentWipeGameCam);
        SetGrid(AppData.GridDisplay);
    }
    // グリッド
    public void SetGrid(bool isFlag)
    {
        m_UIList.m_List[(int)UINo.Grp_Grid].SetVisible(isFlag && !m_IsCurrentWipeGameCam);
    }
    //================================================
    // [///] TODO:実装部分のため他に移したい2
    //================================================
    private void OnClick_Wipe()
    {
        // if (AppData.m_PlayMode != AppData.PlayMode.Replay) return;

        m_IsCurrentWipeGameCam = !m_IsCurrentWipeGameCam;
        SetChangeWipe();
    }
}
