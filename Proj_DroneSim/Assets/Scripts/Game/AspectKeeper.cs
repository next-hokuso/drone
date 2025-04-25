using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AspectKeeper : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    [SerializeField] private Camera m_TargetCamera = null;
    [SerializeField] private Vector2 m_AspectVec = Vector2.zero;

    //================================================
    // [///]
    //================================================
    void Start()
    {
    }

    void Update()
    {
        //���ۂ̉�ʂ̃A�X�y�N�g��
        float actualAspect = (float)Screen.width / (float)Screen.height;
        // �ړI�̃A�X�y�N�g��
        float targetAcpect = m_AspectVec.x / m_AspectVec.y;

        //���@��unity��ʂ̔䗦
        float ratio = targetAcpect / actualAspect;

        // viewport rect
        Rect viewportRect = new Rect(0, 0, 1, 1);

        if (ratio < 1)
        {
            viewportRect.width = ratio; // �g�p���鉡���ɕύX
            viewportRect.x = 0.5f - viewportRect.width * 0.5f;  // ������
        }
        else
        {
            viewportRect.height = 1 / ratio; // �g�p����c���ɕύX
            viewportRect.y = 0.5f - viewportRect.height * 0.5f;  // ������
        }

        // �J������viewport�ɓK�p
        m_TargetCamera.rect = viewportRect;

    }
}
