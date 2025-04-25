using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ターゲットとの距離による子供のGameObject表示・非表示
/// </summary>
public class CmnDistanceChecker : MonoBehaviour
{
    [SerializeField] GameObject TargetObj = null;
    // ターゲットとの距離
    public float Distance = 0.0f;
    // 非表示にするターゲット距離
    [SerializeField] float TargetDistance = 150.0f;

    List<Transform> m_TransList = null;

    bool m_isInit = false;
    bool m_isActive = false;

    void Start()
    {
        Transform[] trans = transform.GetComponentsInChildren<Transform>(true);
        if (trans != null && trans.Length > 0)
        {
            m_TransList = trans.ToList();
            // 直下の子供のみ ※自身と子供の子供は除外
            m_TransList = m_TransList.FindAll(x => (x.parent == transform && x != transform));
        }
        m_isInit = false;
    }

    void Update()
    {
        if (m_TransList == null || m_TransList.Count == 0)
        {
            Start();
        }
        if (TargetObj != null && m_TransList != null && m_TransList.Count > 0)
        {
            Vector3 targetPos = TargetObj.transform.position;
            Vector3 currentPos = transform.position;
            Distance = Vector3.Distance(targetPos, currentPos);
            SetActiveObjs(Distance < TargetDistance);
        }
    }

    void SetActiveObjs(bool isActive)
    {
        if (!m_isInit || m_isActive != isActive)
        {
            m_isActive = isActive;
            m_isInit = true;

            m_TransList.ForEach(x => x.gameObject.SetActive(isActive));
        }
    }

    public GameObject GetTargetObj()
    {
        return TargetObj;
    }

    public void SetTargetObj(GameObject target)
    {
        TargetObj = target;
    }

    public float GetTargetDistance()
    {
        return TargetDistance;
    }

    public void SetTargetDistance(float dist)
    {
        TargetDistance = dist;
    }
}
