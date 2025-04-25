using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace MainGame
{
    public class MG_Mediator
    {
        //================================================
        // [///]
        //================================================
        // ルート
        static public Transform RootT { get; private set; } = null;
        // ゲーム状態管理
        static public MG_GameStateCtrl StateCtrl { get; private set; } = null;
        // オブジェクト仲介
        static public MG_ObjMediator ObjMediator { get; private set; } = null;
        // キャンバス
        static public MG_CanvasCtrl MainCanvas { get; private set; } = null;
        // キャンバス
        static public MG_ScnCanvasCtrl ScnCanvas { get; private set; } = null;
        // キャンバス
        static public MG_ResultCanvasCtrl ResultCanvas { get; private set; } = null;
        // キャンバス
        static public MG_NetworkCheckCanvasCtrl NetworkCheckCanvas { get; private set; } = null;
        // オプション操作
        static public MG_SettingCtrl SettingCtrl { get; private set; } = null;
        // タイトルキャンバス
        static public Ttl_CanvasCtrl TitleCanvas { get; private set; } = null;

        // カメラ
        static private Camera m_GameCamera = null;
        static private GameCameraController m_GameCamearaCtrl = null;
        static private Camera m_UICamera = null;
        static private Camera m_ForwardEffCamera = null;

        // Audio
        static private YnsSimpleAudio m_Audio = null;

        // メインゲームデバッグ
        static private MGD_DebugCtrl m_MainGameDebugCtrl = null;

        //================================================
        // [///]
        //================================================
        static public void Setup()
        {
        }

        // --- ルート ---------------------------------------
        static public void SetRootT(Transform _rootTransform)
        {
            RootT = _rootTransform;
        }
        // --- オペレーター --------------------------------------
        static public void SetGameStateCtrl(MG_GameStateCtrl _state)
        {
            StateCtrl = _state;
        }
        // --- オブジェクト仲介 -----------------------------
        static public void SetObjMediator(MG_ObjMediator _objMediator)
        {
            ObjMediator = _objMediator;
        }
        // --- キャンバス -----------------------------
        static public void SetCanvas(MG_CanvasCtrl _ctrl)
        {
            MainCanvas = _ctrl;
        }
        // --- キャンバス -----------------------------
        static public void SetScnCanvas(MG_ScnCanvasCtrl _ctrl)
        {
            ScnCanvas = _ctrl;
        }
        // --- リザルトキャンバス -----------------------------
        static public void SetResultCanvas(MG_ResultCanvasCtrl _ctrl)
        {
            ResultCanvas = _ctrl;
        }
        // --- オフライン警告キャンバス -----------------------------
        static public void SetNetworkCheckCanvas(MG_NetworkCheckCanvasCtrl _ctrl)
        {
            NetworkCheckCanvas = _ctrl;
        }
        // --- 設定キャンバス -----------------------------
        static public void SetSettingCtrl(MG_SettingCtrl _ctrl)
        {
            SettingCtrl = _ctrl;
        }
        // --- タイトルキャンバス -----------------------------
        static public void SetTitleCanvas(Ttl_CanvasCtrl _ctrl)
        {
            TitleCanvas = _ctrl;
        }



        // デバッグメニューの取得
        static public void SetMainGameDebug(MGD_DebugCtrl ctrl)
        {
            m_MainGameDebugCtrl = ctrl;
        }
        // デバッグメニュー:広告Banner表示の変更
        static public void Dbg_ChangeAdBannerDisp()
        {
            if (!m_MainGameDebugCtrl) return;
        
            m_MainGameDebugCtrl.Dbg_ChangeAdBannerDisp();
        }
        // デバッグメニュー:広告インステ表示チェックの呼び出し
        static public void Dbg_CallAdInterstitialDisp()
        {
            if (!m_MainGameDebugCtrl) return;
        
            m_MainGameDebugCtrl.Dbg_SetIntersitialCheck();
        }
        // --- カメラ -----------------------------------
        static public void SetCamera(Camera _ctrl)
        {
            m_GameCamera = _ctrl;
            m_GameCamearaCtrl = m_GameCamera.GetComponent<GameCameraController>();
        }
        static public Camera GetCamera()
        {
            return m_GameCamera;
        }
        static public GameCameraController GetCameraController()
        {
            return m_GameCamearaCtrl;
        }
        static public void SetUICamera(Camera _ctrl)
        {
            m_UICamera = _ctrl;
        }
        static public Camera GetUICamera()
        {
            return m_UICamera;
        }
        static public void SetForwardEffCamera(Camera _ctrl)
        {
            m_ForwardEffCamera = _ctrl;
        }
        static public Camera GetForwardEffCamera()
        {
            return m_ForwardEffCamera;
        }
        // --- Audio -----------------------------------
        static public void SetAudio(YnsSimpleAudio _ctrl)
        {
            m_Audio = _ctrl;
        }
        static public YnsSimpleAudio GetAudio()
        {
            return m_Audio;
        }
        static public void SetPlaySe(AudioId id, bool isLoop = false)
        {
            //m_Audio.PlaySe(id.ToString(), isLoop);
        }
    }
}