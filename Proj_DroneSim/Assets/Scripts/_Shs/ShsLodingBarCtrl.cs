using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShsLodingBarCtrl : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    //
    [SerializeField] private float m_MaxTime = 0.2f;
    //
    private Slider m_LoadingSlider = null;
    //
    private float m_Timer = 0.0f;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (ShTransitionSys.IsLoadStart())
        {
            m_Timer += Time.deltaTime;
            m_LoadingSlider.value = m_Timer / m_MaxTime;
        }
    }
    public void ProcReset()
    {
        m_LoadingSlider = GetComponent<Slider>();
        m_LoadingSlider.value = 0.0f;
        m_Timer = 0.0f;
    }
}
