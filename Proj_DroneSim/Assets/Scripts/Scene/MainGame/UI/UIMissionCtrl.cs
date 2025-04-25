using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MainGame;

public enum MissionId
{
    Mission_None,
    Mission_Fly,
    Mission_LandingAndStop,
    Mission_A,
    Mission_B,
    Mission_C,
    Mission_D,
    Mission_E,
    Mission_H,

    Mission_Eight1,
    Mission_Eight2,
    Mission_Eight3,
    Mission_Eight4,

    Mission_HappningEtoB,
}

public enum MissionSubtractPointId
{
    OutArea,            // 飛行経路逸脱
    DiffInstruction,    // 指示と異なる飛行
    DefectiveDir,       // 機首方向不良
    Huratuki,           // ふらつき
    HuEnkatu,           // 不円滑
}

public class UIMissionCtrl : ShUIBaseCtrl
{
    string[] textList_SquareFly =
    {
        "離陸してください",
        "A地点に機首を向けて移動してください",
        "B地点に機首を向けて移動してください",
        "C地点に機首を向けて移動してください",
        "D地点に機首を向けて移動してください",
        "E地点に機首を向けて移動してください",
        "A地点に機首を向けて移動してください",
        "着陸地点上空に機首を向けて移動してください",
        "着陸し、プロペラを停止させてください",
    };
    string[] textList_EightFly =
    {
        "離陸してください",
        "8の字飛行を2周行ってください",
        "残り1.5周です",
        "残り1.0周です",
        "残り0.5周です",
        "着陸し、プロペラを停止させてください",
    };
    string[] textList_Happenings =
    {
        "離陸してください",
        "A地点に移動してください\n(※機首は前に向けたまま移動です)",
        "B地点に移動してください",
        "E地点に移動してください",
        "B地点に移動してください", // ドローンがAとBの中間に移動完了した場合→次へ
        "着陸地点に移動してください",               
    };
    public TMPro.TMP_Text m_Text = null;

    public override void ProcInitialize()
    {
        m_Text = transform.Find("Text").GetComponent<TMPro.TMP_Text>();
        m_Text.text = "";
    }

    //
    public void Update()
    {
    }

    //
    public void SetCurrentMissionText(int idx)
    {
        if (AppData.m_MissionId == AppData.MissionID.SquareFly)
        {
            if (idx >= textList_SquareFly.Length) return;
            m_Text.text = textList_SquareFly[idx];
        }
        else if(AppData.m_MissionId == AppData.MissionID.EightFly)
        {
            if (idx >= textList_EightFly.Length) return;
            m_Text.text = textList_EightFly[idx];
        }
        else if(AppData.m_MissionId == AppData.MissionID.HappningFly)
        {
            if (idx >= textList_Happenings.Length) return;
            m_Text.text = textList_Happenings[idx];
        }
    }

    // ミッション完了
    public void SetCompleteMissionText()
    {
        m_Text.text = "ミッション完了";
    }

    // ミッション完了
    public void SetStopMissionText()
    {
        m_Text.text = "ミッション中止";
    }
}
