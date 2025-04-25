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
        //実際の画面のアスペクト比
        float actualAspect = (float)Screen.width / (float)Screen.height;
        // 目的のアスペクト比
        float targetAcpect = m_AspectVec.x / m_AspectVec.y;

        //実機とunity画面の比率
        float ratio = targetAcpect / actualAspect;

        // viewport rect
        Rect viewportRect = new Rect(0, 0, 1, 1);

        if (ratio < 1)
        {
            viewportRect.width = ratio; // 使用する横幅に変更
            viewportRect.x = 0.5f - viewportRect.width * 0.5f;  // 中央寄せ
        }
        else
        {
            viewportRect.height = 1 / ratio; // 使用する縦幅に変更
            viewportRect.y = 0.5f - viewportRect.height * 0.5f;  // 中央寄せ
        }

        // カメラのviewportに適用
        m_TargetCamera.rect = viewportRect;

    }
}
