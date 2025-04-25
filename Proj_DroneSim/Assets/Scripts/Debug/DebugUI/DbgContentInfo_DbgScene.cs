using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// DebugMenuTopで使用する用
// DbgContentInfoはParamSettingにてenum連番使用しているため分ける
public class DbgContentInfo_DbgScene : DbgContentInfoBase
{
    //================================================
    // [///] 定義
    //================================================
    // 左右ボタン用の設定
    //private int m_NowIdx = 0;
    //private int m_IdxMax = 0;


    //================================================
    // [///] プロジェクトごとに削除
    //================================================
    public enum OnEditNo
    {
        // AD
        DebugAdUI_Enable,

        // Sushi
        Sushi_NotCombineSushi,
    }
    public void ProcSetOnEndEdit(OnEditNo no)
    {
        switch (no)
        {
            ////---------------------------------------------------------------------------
            //// デバッグUI表示設定
            //case OnEditNo.DebugAdUI_Enable:
            //    SetInfoText("AD DEBUGUI ENABLE :");
            //    SetButton(AppCommon.m_IsDbgAdUIEnable ? "YES" : "NO");
            //    SetButton1_OnClick(OnButtonClick_DebugAdUI_Enable);
            //    m_Button.GetComponent<Image>().color = AppCommon.m_IsDbgAdUIEnable ?
            //        Color_DefBtn_Enable : Color_DefBtn_Disable;
            //    break;

            default:
                break;
        }
    }
    //---------------------------------------------------------------------------
    // デバッグUI表示設定
    private void OnButtonClick_DebugAdUI_Enable()
    {
        //AppCommon.m_IsDbgAdUIEnable = !AppCommon.m_IsDbgAdUIEnable;
        //// UI
        //SetButton(AppCommon.m_IsDbgAdUIEnable ? "YES" : "NO");
        //m_Button.GetComponent<Image>().color = AppCommon.m_IsDbgAdUIEnable ?
        //    Color_DefBtn_Enable : Color_DefBtn_Disable;
        //// 反映
        //GameStaticInfo.Dbg_ChangeAdBannerDisp();
    }
}
