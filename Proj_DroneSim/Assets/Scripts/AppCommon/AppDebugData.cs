using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yns;
using Game;
using System.Numerics;

/// <summary>
/// デバッグの値と
/// アプリ内全体からアクセスする必要は無いが
/// デバッグにて値を変更し保存できるようにしたい値
/// つまり、本来は各実装に定値を持つ値をデバッグで変更したい値
/// (AppDataからデバッグ関係のローカル保持データの移行先)
/// </summary>
public class AppDebugData : YnsSingletonMonoBehaviour<AppDebugData>
{
    //================================================
    // [///] GameData
    //================================================
    // ↓ローカルデータ(各実装に定値を持つ値)
    static public float m_GameSpeedRate = 1.0f;     // ゲームのスピード倍率(加速/最高速度に影響)
    static public int m_MainSceneCnt = 0;

    // 広告関係デバッグ
    static public int BannerHeight { get; private set; } = 90;

    // ↓ Debug 関連
    static public bool m_IsCountDownEnable = true;
    static public bool m_IsDispTimerEnable = true;
    static public bool m_IsDbgAdUIEnable = false;
    static public bool m_DbgIsPassThrough_NotCombineSushi = false;    // 
    static public int m_DbgAdInstStartCheckRank = 4;    // 広告ｲﾝｽﾃ表示開始ランク


    // カメラ関連
    static public float m_DbgGetCameraDist = 140.0f;        // 距離
    static public float m_DbgGetCameraHeight = 0.0f;        // 追加高さ
    static public float m_DbgGetCameraZDist = 0.0f;         // 追加Z距離
    static public float m_DbgGetCameraRotX = 60.0f;         // RotX
    static public float m_DbgGetCameraFoV = 64.0f;          // FoV
    static public bool m_DbgCameraFollowChangePl = false;   // カメラの追従対処の変更 


    // 上昇/下降移動速度
    // 限界値
    static public float AddPosYLimitSpd = 12.0f;
    // 加速値 限界値を2で割り = 0.5秒で最高速
    static public float AddPosYAccele = AddPosYLimitSpd * 2.0f;
    // 減衰量
    static public float PosYDampRate = 0.1f;

    // 回転
    // 限界値
    static public  float AddRotYLimitSpd = 135.0f;
    // 加速値 限界値を2で割り = 0.5秒で最高速
    static public  float AddRotYAccele = AddRotYLimitSpd * 2.0f;
    // 減衰量
    static public float RotYDampRate = 0.15f;

    // 移動X
    // 限界値
    static public  float AddPosXLimitSpd = 5.0f;
    // 加速値 限界値を2で割り = 0.5秒で最高速
    static public  float AddPosXAccele = AddPosXLimitSpd * 2.0f;
    // 減衰量
    static public  float PosXDampRate = 0.12f;

    // 移動Z
    // 限界値
    static public  float AddPosZLimitSpd = 5.0f;
    // 加速値 限界値を2で割り = 0.5秒で最高速
    static public  float AddPosZAccele = AddPosZLimitSpd * 2.0f;
    // 減衰量
    static public  float PosZDampRate = 0.12f;

    // 傾きX(前方)
    // 限界値
    static public  float AddRotXLimit = 14.0f;
    // 加速値 限界値を2で割り = 0.5秒で最高速
    static public  float AddRotXAccele = AddRotXLimit * 7.5f;
    // 減衰量
    static public float RotXDampRate = 0.15f;

    // 傾きZ(横)
    // 限界値
    static public  float AddRotZLimit = 14.0f;
    // 加速値 限界値を2で割り = 0.5秒で最高速
    static public  float AddRotZAccele = AddRotZLimit * 7.5f;
    // 減衰量
    static public float RotZDampRate = 0.15f;

    // バウンド
    static public float DroneBounciness = 0.35f;

    //================================================
    // [///] セーブデータ
    //================================================
    // セーブデータからGameDataに反映
    static public void LoadGameData()
    {
        // 既存セーブデータのチェック
        if (DebugDataManager.IsExist())
        {
            if (DebugDataManager.VersionId != DebugDataManager.SaveVersionId)
            {
                // 初期化
                if (AppCommon.IsSaveDataDifferent_Init)
                {
                    Debug.Log("セーブデータバージョンが異なるので初期化します  NowID:" + DebugDataManager.VersionId + "  TargetId:" + DebugDataManager.SaveVersionId);
                    DebugDataManager.Delete();
                    DebugDataManager.Load();
                    // セーブデータバージョンIDを入れておく
                    DebugDataManager.VersionId = DebugDataManager.SaveVersionId;
                }
                else
                {
                    // // 更新処理
                    // Debug.Log("セーブデータバージョンが異なるので更新します  NowID:" + DebugDataManager.VersionId + "  TargetId:" + DebugDataManager.SaveVersionId);
                    // DebugDataManager.UpdateData();
                    // // 更新SaveDataをSave
                    // DebugDataManager.Save();
                }
            }
        }
        else
        {
            // セーブデータバージョンIDを入れておく
            DebugDataManager.VersionId = DebugDataManager.SaveVersionId;
        }

        // データ設定
        {
            // Camera
            m_DbgGetCameraDist  = DebugDataManager.CameraDist;
            m_DbgGetCameraZDist = DebugDataManager.CameraZDist;
            m_DbgGetCameraRotX  = DebugDataManager.CameraRotX;
            m_DbgGetCameraFoV  = DebugDataManager.CameraFoV;
            m_DbgCameraFollowChangePl = DebugDataManager.IsCameraFollowTarget_Player;

        }
    }

    // GameDataをセーブデータに反映 ※セーブしていない事に注意
    static public void SaveGameData()
    {
        // Camera
        DebugDataManager.CameraDist = m_DbgGetCameraDist;
        DebugDataManager.CameraZDist = m_DbgGetCameraZDist;
        DebugDataManager.CameraRotX = m_DbgGetCameraRotX;
        DebugDataManager.CameraFoV = m_DbgGetCameraFoV;
        DebugDataManager.IsCameraFollowTarget_Player = m_DbgCameraFollowChangePl;
    }

    // Excelのステージデータをデバッグに保持
    static public void SetDbgStageDataInfo()
    {
    }

    //================================================
    // [///] GameData
    //================================================
    // -----------------------------------------------
    // データ関連
    // バナーの縦サイズ登録
    public void SetBannerHeight(int height)
    {
        BannerHeight = height;
    }
    // スタートカウントダウン有無登録
    public void SetCountDownEnable(bool flag)
    {
        m_IsCountDownEnable = flag;
    }
    // スタートカウントダウン有無取得
    public bool IsCountDownEnable()
    {
        return m_IsCountDownEnable;
    }
    // スタート時からの秒数表示有無登録
    public void SetDispTimerEnable(bool flag)
    {
        m_IsDispTimerEnable = flag;
    }
    // スタート時からの秒数表示有無取得
    public bool IsDispTimerEnable()
    {
        return m_IsDispTimerEnable;
    }
}
