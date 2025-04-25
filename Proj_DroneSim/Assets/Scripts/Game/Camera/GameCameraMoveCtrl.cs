using System.Collections;
using UnityEngine;
using System.IO;
using Game;
using System.Collections.Generic;

public class GameCameraMoveCtrl : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    //
    private const float CheckUnderPosZ = 13.1f;
    private const float CheckTopPosZ = 0.0f;
    private const float CheckRightPosX = 5.0f;
    private const float CheckLeftPosX = 5.0f;


    // 
    private bool m_IsUpdate = false;
    private bool m_IsLockMoveX = false;
    private bool m_IsLockMoveY = false;

    //
    //private float m_CheckTimer = 0.0f;

    //
    private Vector3 m_StPos = Vector3.zero;

    // target
    [SerializeField] private GameObject m_Player = null;
    private Vector3 m_MoveBeforePos = Vector3.zero;

    //
    [SerializeField] private float m_StPlayerToThisDist = 0.0f;
    [SerializeField] private bool m_IsChangeRot = true;

    //
    private Vector3 m_AddEffPos = Vector3.zero;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
        m_StPos = transform.position;
    }

    private void LateUpdate()
    {
    }

    public void ProcCameraTargetUpdate()
    {
        if (m_IsUpdate)
        {
            Vector3 plPos = m_Player.transform.position;
            Vector3 myPos = transform.position;

            /// 追従
            if (!m_IsLockMoveX)
            {
                myPos.x = plPos.x;
            }
            if (!m_IsLockMoveY)
            {
                myPos.y = plPos.y;
            }
            myPos.z = plPos.z + m_StPlayerToThisDist;


            // add
            myPos += m_AddEffPos;

            transform.position = myPos;
        }
    }
    public bool IsChangeRot()
    {
        return m_IsChangeRot;
    }

    //================================================
    // [///]
    //================================================

    //================================================
    // [///]
    //================================================
    //
    public void SetInfo(GameObject player, bool isReset=false)
    {
        if (isReset)
        {
            transform.position = Vector3.zero;
        }

        m_Player = player;

        m_IsUpdate = true;
    }

    // ゲーム開始時設定
    public void SetStartInfo()
    {
    }

    // ゲーム敗北時
    public void SetGameOverInfo()
    {
    }

    // isUPdate
    public void SetUpdate(bool isUpdate)
    {
        m_IsUpdate = isUpdate;
    }

    // 停止
    public void SetGoalEnd()
    {
        m_IsUpdate = false;
    }

    // 停止
    public void SetProcEnd()
    {
        m_IsUpdate = false;
    }

    // リセット
    public void SetReset()
    {
        transform.position = m_StPos;
    }

    // 基準初期地点更新
    public void SetStPos( Vector3 pos)
    {
        m_StPos = pos;
    }

    // 設定
    public void ForcePosSet(Vector3 pos)
    {
        transform.position = pos;
    }

    // X移動設定
    public void SetIsLockMoveX(bool isLock)
    {
        m_IsLockMoveX = isLock;
    }
    // Y移動設定
    public void SetIsLockMoveY(bool isLock)
    {
        m_IsLockMoveY = isLock;
    }
}
