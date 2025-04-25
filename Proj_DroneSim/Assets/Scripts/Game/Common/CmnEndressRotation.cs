using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// ずっと回転し続ける
public class CmnEndressRotation : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    //
    private bool m_IsUpdate = false;
    //
    private Vector3 m_RotationAngleForOne = Vector3.one;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsUpdate)
        {
            transform.Rotate(m_RotationAngleForOne * Time.deltaTime);
        }
    }

    //================================================
    // [///]
    //================================================
    /// <summary>
    /// _rotationAngleForOne=1秒で回る度数,
    /// </summary>
    public void SetRotation(Vector3 _rotationAngleForOne)
    {
        //
        m_RotationAngleForOne = _rotationAngleForOne;
        m_IsUpdate = true;
    }
}