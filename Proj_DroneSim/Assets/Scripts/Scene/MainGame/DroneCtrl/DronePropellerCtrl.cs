using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DronePropellerCtrl : MonoBehaviour
{
    private List<Transform> m_PropellerList = null;
    //private List<Transform> m_PropellerCopyList = null;
    private enum PropellerId
    {
        LeftForward,
        LeftBack,
        RightForward,
        RightBack,
    }
    

    private bool m_IsMoterOn = false;
    private bool m_IsMoterOff = false;
    // private bool m_IsCopyActive = false;
    private bool m_IsMoterOff_SeStop = false;

    // spd
    [SerializeField] private float AddRotMaxVal = 325.0f * 10.0f;
    private float m_PropellerRotY = 0.0f;
    [SerializeField] private float m_AddRotVal = 0.0f;


    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================
    public void Setup()
    {
        m_PropellerList = new List<Transform>();

        // プロペラ設定
        Transform modelT = transform.Find("Model/3DDrone/Propeller");
        m_PropellerList.Add(modelT.Find("polySurface257"));
        m_PropellerList.Add(modelT.Find("polySurface258"));
        m_PropellerList.Add(modelT.Find("polySurface259"));
        m_PropellerList.Add(modelT.Find("polySurface260"));
    }

    private void Update()
    {
        if (m_IsMoterOn)
        {
            // プロペラ加速
            if(m_AddRotVal < AddRotMaxVal)
            {
                m_AddRotVal = Mathf.Clamp(m_AddRotVal + AddRotMaxVal * Time.deltaTime, 0.0f, AddRotMaxVal);
            }

            m_PropellerRotY += m_AddRotVal * Time.deltaTime;
            if(m_PropellerRotY > 30000000) { m_PropellerRotY -= 30000000; }

            // 更新
            UpdatePropeller();
        }
        else if (m_IsMoterOff)
        {
            // プロペラ停止
            if (m_AddRotVal <= AddRotMaxVal)
            {
                m_AddRotVal = Mathf.Clamp(m_AddRotVal - AddRotMaxVal * Time.fixedDeltaTime, 0.0f, AddRotMaxVal);
                if(m_IsMoterOff_SeStop)
                {
                    if (m_AddRotVal < AddRotMaxVal * 0.5f)
                    {
                        m_IsMoterOff_SeStop = false;
                        if (AppData.DroneFlightSound)
                        {
                            // プロペラ停止SE
                            MainGame.MG_Mediator.GetAudio().PlaySe(Game.AudioId.landing_propeller.ToString(), false);
                        }
                    }
                }
            }
            if(m_AddRotVal <= 0.0f)
            {
                m_AddRotVal = 0.0f;
                m_IsMoterOff = false;
            }

            m_PropellerRotY += m_AddRotVal * Time.fixedDeltaTime;
            if (m_PropellerRotY > 30000000) { m_PropellerRotY -= 30000000; }

            // 更新
            UpdatePropeller();
        }
    }

    // プロペラ更新
    private void UpdatePropeller()
    {
        // 左前/右後 : 時計回り
        m_PropellerList[(int)PropellerId.LeftForward].localEulerAngles = Vector3.up * m_PropellerRotY;
        m_PropellerList[(int)PropellerId.RightBack].localEulerAngles = Vector3.up * m_PropellerRotY;

        // 左後/右前 : 反時計回り
        m_PropellerList[(int)PropellerId.RightForward].localEulerAngles = Vector3.up * -m_PropellerRotY;
        m_PropellerList[(int)PropellerId.LeftBack].localEulerAngles = Vector3.up * -m_PropellerRotY;


        // // 一定速度以上の場合 残像用のプロペラ表示
        // if(m_AddRotVal > 360.0f)
        // {
        //     if (!m_IsCopyActive)
        //     {
        //         m_IsCopyActive = true;
        //         foreach (Transform t in m_PropellerCopyList)
        //         {
        //             t.gameObject.SetActive(true);
        //         }
        //     }
        // 
        //     foreach (Transform t in m_PropellerCopyList)
        //     {
        //         t.localEulerAngles = Vector3.up * (m_PropellerRotY + 90.0f);
        //     }
        // }
    }

    public void ProcMonterOn()
    {
        m_IsMoterOn = true;
        m_AddRotVal = 0.0f;
        // m_IsCopyActive = false;

        m_IsMoterOff = false;
    }

    public void ProcMonterOff()
    {
        m_IsMoterOff = true;
        m_IsMoterOff_SeStop = true;

        m_IsMoterOn = false;
    }

    // ProcMonterOffの後に使用すること
    public bool IsPropellerOff()
    {
        return m_IsMoterOff;
    }

    //=========================================================================
    //
    // [///] 
    //
    //=========================================================================
    // 0~1で回転させる
    public void SetPropellerRot(float val)
    {
        transform.localEulerAngles += Vector3.up * val * 180.0f * Time.fixedDeltaTime;
    }
}
