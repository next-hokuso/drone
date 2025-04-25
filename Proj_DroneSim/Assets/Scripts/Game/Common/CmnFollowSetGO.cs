using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 設定したオブジェクトをFixedUpdateで追従する
public class CmnFollowSetGO : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    //
    private Transform m_FollowTarget = null;
    //
    private Vector3 m_OffsetPos = Vector3.zero;
    // 
    private Vector3 m_AddDir = Vector3.zero;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
    }
    void Update()
    {
    }

    //private void LateUpdate()
    //{
    //    if (m_FollowTarget)
    //    {
    //        m_AddDir = GameStaticInfo.GetCamera().transform.position;
    //        m_AddDir = m_FollowTarget.transform.position - m_AddDir;
    //        transform.position = m_FollowTarget.transform.position + m_AddDir.normalized.z * m_OffsetPos;
    //    }
    //}

    //================================================
    // [///]
    //================================================
    /// <summary>
    /// </summary>
    public void SetGO(Transform targetT, Vector3 offsetPos)
    {
        m_FollowTarget = targetT;
        m_OffsetPos = offsetPos;
    }
}