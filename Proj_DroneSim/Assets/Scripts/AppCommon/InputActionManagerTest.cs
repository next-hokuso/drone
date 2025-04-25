using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

/// <summary>
/// 入力の管理
/// </summary>
public class InputActionManagerTest : ShSingletonMonoBehavior<InputActionManagerTest>
{
    //================================================
    // [///] 定義
    //================================================
    /// <summary>
    /// 選択したゲームオブジェクト 基本的にはSystem内で使用すること
    /// </summary>
    public GameObject m_SelectGO = null;


    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================
    public void setup()
    {

    }

    public void Update()
    {
        CheckPressdInput();
    }


    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================
    public void CheckPressdInput()
    {
        if(DualShockGamepad.current != null)
        {
            foreach(var key in DualShockGamepad.current.children)
            {
                if (key.IsPressed())
                {
                    Yns.YnSys.SetDbgText("DualShock : Pressd");
                }
            }
        }
        if(Keyboard.current != null)
        {
            foreach(var key in Keyboard.current.children)
            {
                if (key.IsPressed())
                {
                    Yns.YnSys.SetDbgText("Keyboard : Pressd");
                }
            }
        }
    }
}