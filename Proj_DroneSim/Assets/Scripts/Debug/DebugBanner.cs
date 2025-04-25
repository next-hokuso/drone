using UnityEngine;

public class DebugBanner : MonoBehaviour
{
    [SerializeField] float currentHeight = 90.0f;

    void Update()
    {
        if (currentHeight != AppDebugData.BannerHeight)
        {
            currentHeight = AppDebugData.BannerHeight;
            RectTransform rt = transform.GetComponent<RectTransform>();
            if (rt != null)
            {
                // Y座標はバナーの高さ÷２
                Vector3 pos = rt.anchoredPosition3D;
                pos.y = currentHeight / 2.0f;
                rt.anchoredPosition3D = pos;
                Vector2 wh = rt.sizeDelta;
                wh.y = currentHeight;
                rt.sizeDelta = wh;
            }
        }
    }
}
