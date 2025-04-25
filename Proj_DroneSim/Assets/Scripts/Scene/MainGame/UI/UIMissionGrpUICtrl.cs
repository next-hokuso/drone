using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MainGame;
using System.Collections.Generic;

public class UIMissionGrpUICtrl : ShUIBaseCtrl
{
    //================================================
    // [///] 定義
    //================================================
    private List<UIMissionFailedCtrl> m_List = null;

    //================================================
    // [///]
    //================================================
    public override void ProcInitialize()
    {
        m_List = new List<UIMissionFailedCtrl>();

        for(int i = 0; i < transform.childCount; ++i)
        {
            {
                m_List.Add(transform.GetChild(i).gameObject.AddComponent<UIMissionFailedCtrl>());
            }
        }

        foreach(UIMissionFailedCtrl ctrl in m_List)
        {
            ctrl.ProcInitialize();
        }
    }
    private void Start()
    {
    }
    private void Update()
    {
    }

    public void ProcUISetting(int point, MissionSubtractPointId id)
    {
        foreach (UIMissionFailedCtrl ctrl in m_List)
        {
            if (!ctrl.IsDisp())
            {
                ctrl.ProcUISetting(point, id);
                break;
            }
        }
    }
}
