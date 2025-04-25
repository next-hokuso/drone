using Game;
using UnityEngine;
public class HitCheck_Center : MonoBehaviour
{
    private DroneCtrl m_DroneCtrl = null;

    public void SetDroneCtrl(DroneCtrl ctrl)
    {
        m_DroneCtrl = ctrl;
    }

    //
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MissionCheck"))
        {
            // ミッションモードか
            if (AppData.m_PlayMode != AppData.PlayMode.Mission) return;

            // 取得
            HitCheck_Mission hitcheckCtrl = other.GetComponent<HitCheck_Mission>();
            if (!hitcheckCtrl) return;

            // スクエア飛行
            if (AppData.m_MissionId == AppData.MissionID.SquareFly)
            {
                m_DroneCtrl.CheckMissionTrigger_Square(hitcheckCtrl);
            }
            else if (AppData.m_MissionId == AppData.MissionID.EightFly)
            {
                m_DroneCtrl.CheckMissionTrigger_Eight(hitcheckCtrl);
            }
            else if (AppData.m_MissionId == AppData.MissionID.HappningFly)
            {
                m_DroneCtrl.CheckMissionTrigger_Happning(hitcheckCtrl);
            }
        }
        // // 壁
        // else if (other.CompareTag("Wall"))
        // {
        //     // ミッションモードか
        //     if (AppData.m_PlayMode != AppData.PlayMode.Mission) return;
        // 
        //     // ミッション中止:危険な飛行,墜落,飛行空域離脱 中止
        //     FailedMission();
        // }
    }

    //
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MissionCheck"))
        {
            // 取得
            HitCheck_Mission hitcheckCtrl = other.GetComponent<HitCheck_Mission>();
            if (!hitcheckCtrl) return;

            // スクエア飛行
            if (AppData.m_MissionId == AppData.MissionID.SquareFly)
            {
                // 中央から離れた後、中央に入った場合のOutArea
                //if (m_CurrentMissionId > MissionId.Mission_Fly)
                {
                    if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.OutArea))
                    {
                        MainGame.MG_Mediator.MainCanvas.SetOutArea(false);
                        m_DroneCtrl.ProcMissionExitOutArea();
                    }
                }
            }
            // 8の字飛行
            else if (AppData.m_MissionId == AppData.MissionID.EightFly)
            {
                if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.OutArea))
                {
                    MainGame.MG_Mediator.MainCanvas.SetOutArea(false);
                    m_DroneCtrl.ProcMissionExitOutArea();
                }
            }
            // 8の字飛行
            else if (AppData.m_MissionId == AppData.MissionID.HappningFly)
            {
                if (hitcheckCtrl.CheckHitId_Equal(MissionHitId.OutArea))
                {
                    MainGame.MG_Mediator.MainCanvas.SetOutArea(false);
                    m_DroneCtrl.ProcMissionExitOutArea();
                }
            }
        }
    }

}
