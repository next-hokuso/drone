using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace MainGame
{
    public class MGEndress_Mediator
    {
        //================================================
        // [///]
        //================================================
        // ルート
        static public Transform RootT { get; private set; } = null;
        // ゲーム状態管理
        static public MGEndress_GameStateCtrl StateCtrl { get; private set; } = null;
        // オブジェクト仲介
        static public MGEndress_ObjMediator ObjMediator { get; private set; } = null;
        // キャンバス
        static public MGEndress_CanvasCtrl MainCanvas { get; private set; } = null;

        // カメラ
        static private Camera m_GameCamera = null;
        static private GameCameraController m_GameCamearaCtrl = null;
        static private Camera m_UICamera = null;
        static private Camera m_ForwardEffCamera = null;

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
        static public void SetGameStateCtrl(MGEndress_GameStateCtrl _state)
        {
            StateCtrl = _state;
        }
        // --- オブジェクト仲介 -----------------------------
        static public void SetObjMediator(MGEndress_ObjMediator _objMediator)
        {
            ObjMediator = _objMediator;
        }
        // --- キャンバス -----------------------------
        static public void SetCanvas(MGEndress_CanvasCtrl _ctrl)
        {
            MainCanvas = _ctrl;
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
    }
}