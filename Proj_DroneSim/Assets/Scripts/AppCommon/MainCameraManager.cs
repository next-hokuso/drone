using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// メインカメラの管理
/// </summary>
public class MainCameraManager : ShSingletonMonoBehavior<MainCameraManager>
{
    //================================================
    // [///] 定義
    //================================================
    /// <summary>
    /// 選択したゲームオブジェクト 基本的にはSystem内で使用すること
    /// </summary>
    public GameObject m_SelectGO = null;

    // 
    public Camera Camera { get; private set; } = null;
    public GameCameraController CamCtrl { get; private set; } = null;


    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================
    // --- カメラ -----------------------------------
    public void SetCamera(Camera _ctrl)
    {
        Camera = _ctrl;
        CamCtrl = Camera.GetComponent<GameCameraController>();
    }
}