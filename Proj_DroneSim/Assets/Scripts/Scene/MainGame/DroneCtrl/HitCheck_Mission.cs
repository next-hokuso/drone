using Game;
using UnityEngine;
public class HitCheck_Mission : MonoBehaviour
{
    [SerializeField]
    private MissionHitId m_CurrentHitId = MissionHitId.None;

    public MissionHitId GetMissionHitId()
    {
        return m_CurrentHitId;
    }
    public bool CheckHitId_Equal(MissionHitId id)
    {
        return m_CurrentHitId == id;
    }
}
