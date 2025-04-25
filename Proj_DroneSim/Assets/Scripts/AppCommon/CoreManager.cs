using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

/// <summary>
/// CoreManager : Systemで付与するAudioSystemなどへのアクセス管理
/// </summary>
public class CoreManager : ShSystemSingletonMonoBehavior<CoreManager>
{
    //================================================
    // [///] 定義
    //================================================
    /// <summary>
    /// 選択したゲームオブジェクト 基本的にはSystem内で使用すること
    /// </summary>
    public GameObject m_SelectGO = null;

    // システムに付与したAudio
    public YnsSimpleAudio AudioComp { get; private set; } = null;
    public void SetAudioComp(YnsSimpleAudio audioComp) {  AudioComp = audioComp; }


    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================
    public void Start()
    {
    }

    public void Update()
    {
    }


    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================
}