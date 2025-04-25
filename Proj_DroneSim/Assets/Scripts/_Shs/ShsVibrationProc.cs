using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yns;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

/// <summary>
/// 振動 内部処理
/// </summary>
public class ShsVibrationProc: MonoBehaviour
{
    //================================================
    // [///] Vibrate
    //================================================
    // 振動ミリ秒数
    // ゴール時の振動 100 + 100 + 800 = 1000msec
    private readonly long VibrateMilliSeconds_Goal_1 = 100;

#if (UNITY_ANDROID && !UNITY_EDITOR) || UNITY_IOS
    private readonly long VibrateMilliSeconds_Draw = 5;
    private readonly long VibrateMilliSeconds_CoinPaleAndGate = 20;
    private readonly long VibrateMilliSeconds_BallHitObject = 50;
    private readonly long VibrateMilliSeconds_Goal_2 = 800;

    private readonly int VibrateiOSSoundIdSmall = 1350;
    private readonly int VibrateiOSSoundIdLarge = 1519;

    private readonly long VibrateAndroidLargeMilliSeconds = 400;
    private readonly long VibrateAndroidSmallMilliSeconds = 5;
#endif
    //================================================
    // [///]
    //================================================
    void Start()
    {
        // 振動
#if UNITY_IOS
        UnityCoreHaptics.UnityCoreHapticsProxy.CreateEngine();
#endif
    }

    // 振動 - 短い
    public void SetVibration_Small()
    {
        if (AppData.IsEnableVibration)
        {
            if (SystemInfo.supportsVibration)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                VibrationAndroid.Vibrate(VibrateAndroidSmallMilliSeconds);
#elif UNITY_IOS
                // サポートしていない場合
                if (!UnityCoreHaptics.UnityCoreHapticsProxy.SupportsCoreHaptics())
                {
                    VibrationAndroid.VivrateiOS(VibrateiOSSoundIdSmall);
                }
                else{
                    UnityCoreHaptics.UnityCoreHapticsProxy.PlayContinuousHaptics(1.0f, 1.0f, VibrateAndroidSmallMilliSeconds * 0.001f);
                }
#endif
            }
        }
    }

    // 振動 - 線描画中
    public void SetVibration_Draw()
    {
        if (AppData.IsEnableVibration)
        {
            if (SystemInfo.supportsVibration)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                VibrationAndroid.Vibrate(VibrateMilliSeconds_Draw);
#elif UNITY_IOS
                // サポートしていない場合
                if (!UnityCoreHaptics.UnityCoreHapticsProxy.SupportsCoreHaptics())
                {
                    //VibrationAndroid.VivrateiOS(VibrateiOSSoundIdSmall);
                }
                else{
                    UnityCoreHaptics.UnityCoreHapticsProxy.PlayContinuousHaptics(1.0f, 1.0f, VibrateMilliSeconds_Draw * 0.001f);
                }
#endif
            }
        }
    }

    // 振動 - ボールが落下しオブジェクトに接触した時
    public void SetVibration_BallHitObject()
    {
        if (AppData.IsEnableVibration)
        {
            if (SystemInfo.supportsVibration)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                VibrationAndroid.Vibrate(VibrateMilliSeconds_BallHitObject);
#elif UNITY_IOS
                // サポートしていない場合
                if (!UnityCoreHaptics.UnityCoreHapticsProxy.SupportsCoreHaptics())
                {
                    VibrationAndroid.VivrateiOS(VibrateiOSSoundIdSmall);
                }
                else{
                    UnityCoreHaptics.UnityCoreHapticsProxy.PlayContinuousHaptics(1.0f, 1.0f, VibrateMilliSeconds_BallHitObject * 0.001f);
                }
#endif
            }
        }
    }

    // 振動 - コインゲート/パネル通過時
    public void SetVibration_CoinPanelAndGate()
    {
        if (AppData.IsEnableVibration)
        {
            if (SystemInfo.supportsVibration)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                VibrationAndroid.Vibrate(VibrateMilliSeconds_CoinPaleAndGate);
#elif UNITY_IOS
                // サポートしていない場合
                if (!UnityCoreHaptics.UnityCoreHapticsProxy.SupportsCoreHaptics())
                {
                    VibrationAndroid.VivrateiOS(VibrateiOSSoundIdSmall);
                }
                else{
                    UnityCoreHaptics.UnityCoreHapticsProxy.PlayContinuousHaptics(1.0f, 1.0f, VibrateMilliSeconds_CoinPaleAndGate * 0.001f);
                }
#endif
            }
        }
    }

    // 振動 - ボールが穴に入った時
    public void SetVibration_Goal()
    {
        if (AppData.IsEnableVibration)
        {
            if (SystemInfo.supportsVibration)
            {
                StartCoroutine(ProcVibration_Goal());
            }
        }
    }
    private IEnumerator ProcVibration_Goal()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
                VibrationAndroid.Vibrate(VibrateMilliSeconds_Goal_1);
#elif UNITY_IOS
                // サポートしていない場合
                if (!UnityCoreHaptics.UnityCoreHapticsProxy.SupportsCoreHaptics())
                {
                    VibrationAndroid.VivrateiOS(VibrateiOSSoundIdSmall);
                }
                else{
                    UnityCoreHaptics.UnityCoreHapticsProxy.PlayContinuousHaptics(1.0f, 1.0f, VibrateMilliSeconds_Goal_1 * 0.001f);
                }
#endif

        yield return new WaitForSeconds(VibrateMilliSeconds_Goal_1 * 0.002f);

#if UNITY_ANDROID && !UNITY_EDITOR
                VibrationAndroid.Vibrate(VibrateMilliSeconds_Goal_1);
#elif UNITY_IOS
                // サポートしていない場合
                if (!UnityCoreHaptics.UnityCoreHapticsProxy.SupportsCoreHaptics())
                {
                    VibrationAndroid.VivrateiOS(VibrateiOSSoundIdSmall);
                }
                else{
                    UnityCoreHaptics.UnityCoreHapticsProxy.PlayContinuousHaptics(1.0f, 1.0f, VibrateMilliSeconds_Goal_1 * 0.001f);
                }
#endif
        yield return new WaitForSeconds(VibrateMilliSeconds_Goal_1 * 0.002f);

#if UNITY_ANDROID && !UNITY_EDITOR
                VibrationAndroid.Vibrate(VibrateMilliSeconds_Goal_2);
#elif UNITY_IOS
                // サポートしていない場合
                if (!UnityCoreHaptics.UnityCoreHapticsProxy.SupportsCoreHaptics())
                {
                    VibrationAndroid.VivrateiOS(VibrateiOSSoundIdLarge);
                }
                else{
                    UnityCoreHaptics.UnityCoreHapticsProxy.PlayContinuousHaptics(1.0f, 1.0f, VibrateMilliSeconds_Goal_2 * 0.001f);
                }
#endif
    }

}