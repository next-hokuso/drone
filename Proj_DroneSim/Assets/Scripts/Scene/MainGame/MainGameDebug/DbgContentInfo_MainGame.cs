using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DbgContentInfo_MainGame : DbgContentInfoBase
{
    //================================================
    // [///] 定義
    //================================================
    // 左右ボタン用の設定
    //private int m_NowIdx = 0;
    //private int m_IdxMax = 0;

    private int m_IFLimitVal_IntNum = 99999;
    private float m_IFLimitVal_FloatNum = 99999.0f;
    private bool m_isToggle = false;
    private float m_StartVal = 0;

    //================================================
    // [///] プロジェクトごとに削除
    //================================================
    public enum OnEditNo
    {
        None = -1,

        UpDown_Spd,
        UpDown_DampRate,

        MoveX_Spd,
        MoveX_DampRate,

        MoveZ_Spd,
        MoveZ_DampRate,

        RotY_Spd,
        RotY_DampRate,

        // 横
        RotZ,
        RotZ_DampRate,

        // 前後
        RotX,
        RotX_DampRate,

        Bouciness,
    }
    private OnEditNo m_ProcNo = OnEditNo.None;

    private MainGame.DebugMainGameSettings m_MainGameSettings = null;

    public void SetDebugMainGameSettings(MainGame.DebugMainGameSettings settings)
    {
        m_MainGameSettings = settings;
    }

    public void ProcSetOnEndEdit(OnEditNo no)
    {
        m_ProcNo = no;
        switch (no)
        {
            ////---------------------------------------------------------------------------
            //// 所持コイン数
            //case OnEditNo.HaveCoin:
            //    m_IFLimitVal_IntNum = (int)AppData.CoinNumLimit;
            //    SetInputFieldText(AppDebugData.m_HaveCoin.ToString());
            //    SetInputField1_OnEdit(OnEditEnd_IF1Int);
            //    break;
            //---------------------------------------------------------------------------
            // 上昇下降
            case OnEditNo.UpDown_Spd:
                //m_IFLimitVal_FloatNum = AppDebugData.AddPosYLimitSpd;
                SetInputFieldText(AppDebugData.AddPosYLimitSpd.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                break;
            case OnEditNo.UpDown_DampRate:
                m_IFLimitVal_FloatNum = 1.0f;
                SetInputFieldText(AppDebugData.PosYDampRate.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                transform.Find("Text_Add").GetComponent<Text>().text = "    0～1";
                break;

            //---------------------------------------------------------------------------
            // 移動X
            case OnEditNo.MoveX_Spd:
                //m_IFLimitVal_FloatNum = AppDebugData.AddPosXLimitSpd;
                SetInputFieldText(AppDebugData.AddPosXLimitSpd.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                break;
            case OnEditNo.MoveX_DampRate:
                m_IFLimitVal_FloatNum = 1.0f;
                SetInputFieldText(AppDebugData.PosXDampRate.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                transform.Find("Text_Add").GetComponent<Text>().text = "    0～1";
                break;

            //---------------------------------------------------------------------------
            // 移動Z
            case OnEditNo.MoveZ_Spd:
                //m_IFLimitVal_FloatNum = AppDebugData.AddPosZLimitSpd;
                SetInputFieldText(AppDebugData.AddPosZLimitSpd.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                break;
            case OnEditNo.MoveZ_DampRate:
                m_IFLimitVal_FloatNum = 1.0f;
                SetInputFieldText(AppDebugData.PosZDampRate.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                transform.Find("Text_Add").GetComponent<Text>().text = "    0～1";
                break;

            //---------------------------------------------------------------------------
            // 回転
            case OnEditNo.RotY_Spd:
                //m_IFLimitVal_FloatNum = AppDebugData.AddRotYLimitSpd;
                SetInputFieldText(AppDebugData.AddRotYLimitSpd.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                break;
            case OnEditNo.RotY_DampRate:
                m_IFLimitVal_FloatNum = 1.0f;
                SetInputFieldText(AppDebugData.RotYDampRate.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                transform.Find("Text_Add").GetComponent<Text>().text = "    0～1";
                break;

            //---------------------------------------------------------------------------
            // 回転 横
            case OnEditNo.RotZ:
                //m_IFLimitVal_FloatNum = AppDebugData.AddRotZLimit;
                SetInputFieldText(AppDebugData.AddRotZLimit.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                break;
            case OnEditNo.RotZ_DampRate:
                m_IFLimitVal_FloatNum = 1.0f;
                SetInputFieldText(AppDebugData.RotZDampRate.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                transform.Find("Text_Add").GetComponent<Text>().text = "    0～1";
                break;

            //---------------------------------------------------------------------------
            // 回転 前後
            case OnEditNo.RotX:
                //m_IFLimitVal_FloatNum = AppDebugData.AddRotXLimit;
                SetInputFieldText(AppDebugData.AddRotXLimit.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                break;
            case OnEditNo.RotX_DampRate:
                m_IFLimitVal_FloatNum = 1.0f;
                SetInputFieldText(AppDebugData.RotXDampRate.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                transform.Find("Text_Add").GetComponent<Text>().text = "    0～1";
                break;

            //---------------------------------------------------------------------------
            // バウンド
            case OnEditNo.Bouciness:
                m_IFLimitVal_FloatNum = 1.0f;
                SetInputFieldText(AppDebugData.DroneBounciness.ToString("f3"));
                SetInputField1_OnEdit(OnEditEnd_IF3Float);
                transform.Find("Text_Add").GetComponent<Text>().text = "    0～1";
                break;
        }


        m_StartVal = float.Parse(m_InputField.text);
    }
    // 表示更新
    public void UpdateInfo()
    {
        // switch (m_ProcNo)
        // {
        // }
    }
    public void ResetInfo()
    {
        switch (m_ProcNo)
        {
            //---------------------------------------------------------------------------
            case OnEditNo.UpDown_Spd:
                AppDebugData.AddPosYLimitSpd = m_StartVal;
                break;
            //---------------------------------------------------------------------------
            case OnEditNo.UpDown_DampRate:
                AppDebugData.PosYDampRate = m_StartVal;
                break;
            //---------------------------------------------------------------------------
            // 移動X
            case OnEditNo.MoveX_Spd:
                AppDebugData.AddPosXLimitSpd = m_StartVal;
                break;
            case OnEditNo.MoveX_DampRate:
                AppDebugData.PosXDampRate = m_StartVal;
                break;

            //---------------------------------------------------------------------------
            // 移動Z
            case OnEditNo.MoveZ_Spd:
                AppDebugData.AddPosZLimitSpd = m_StartVal;
                break;
            case OnEditNo.MoveZ_DampRate:
                AppDebugData.PosZDampRate = m_StartVal;
                break;

            //---------------------------------------------------------------------------
            // 回転
            case OnEditNo.RotY_Spd:
                AppDebugData.AddRotYLimitSpd = m_StartVal;
                break;
            case OnEditNo.RotY_DampRate:
                AppDebugData.RotYDampRate = m_StartVal;
                break;

            //---------------------------------------------------------------------------
            // 回転
            case OnEditNo.RotX:
                AppDebugData.AddRotXLimit = m_StartVal;
                break;
            case OnEditNo.RotX_DampRate:
                AppDebugData.RotXDampRate = m_StartVal;
                break;
            case OnEditNo.RotZ:
                AppDebugData.AddRotZLimit = m_StartVal;
                break;
            case OnEditNo.RotZ_DampRate:
                AppDebugData.RotZDampRate = m_StartVal;
                break;

            case OnEditNo.Bouciness:
                AppDebugData.DroneBounciness = m_StartVal;
                MainGame.MG_Mediator.ObjMediator.DbgDroneBouncinessSetting();
                break;
        }

        m_InputField.text = m_StartVal.ToString();
    }
    //---------------------------------------------------------------------------
    private void OnEditEnd_IF1Int(string arg)
    {
        int val = 1;
        if (!int.TryParse(arg, out val))
        {
            val = 1;
        }
        val = Mathf.Clamp(val, 1, m_IFLimitVal_IntNum);

        // switch (m_ProcNo)
        // {
        //     ////---------------------------------------------------------------------------
        //     //case OnEditNo.HaveCoin:
        //     //    AppDebugData.m_HaveCoin = val;
        //     //    break;
        //     //---------------------------------------------------------------------------
        // }

        m_InputField.text = val.ToString();
    }

    //---------------------------------------------------------------------------
    private void OnEditEnd_IF1Float(string arg)
    {
        string text = m_InputField.text;
        float val = 0;
        if (!float.TryParse(arg, out val))
        {
            val = 0;
        }
        val = Mathf.Clamp(val, 0, m_IFLimitVal_FloatNum);

        // switch (m_ProcNo)
        // {
        //     ////---------------------------------------------------------------------------
        //     //case OnEditNo.HeadingForceLvAdd:
        //     //    AppDebugData.m_HeadingForceLvAdd = val;
        //     //    break;
        // }

        m_InputField.text = val.ToString();
    }

    //---------------------------------------------------------------------------
    private void OnEditEnd_IF3Float(string arg)
    {
        string text = m_InputField.text;
        float val = 0;
        if (!float.TryParse(arg, out val))
        {
            val = 0;
        }
        val = Mathf.Clamp(val, 0, m_IFLimitVal_FloatNum);

        switch (m_ProcNo)
        {
            //---------------------------------------------------------------------------
            case OnEditNo.UpDown_Spd:
                AppDebugData.AddPosYLimitSpd = val;
                break;
            //---------------------------------------------------------------------------
            case OnEditNo.UpDown_DampRate:
                AppDebugData.PosYDampRate = val;
                break;
            //---------------------------------------------------------------------------
            // 移動X
            case OnEditNo.MoveX_Spd:
                AppDebugData.AddPosXLimitSpd = val;
                break;
            case OnEditNo.MoveX_DampRate:
                AppDebugData.PosXDampRate = val;
                break;

            //---------------------------------------------------------------------------
            // 移動Z
            case OnEditNo.MoveZ_Spd:
                AppDebugData.AddPosZLimitSpd = val;
                break;
            case OnEditNo.MoveZ_DampRate:
                AppDebugData.PosZDampRate = val;
                break;

            //---------------------------------------------------------------------------
            // 回転
            case OnEditNo.RotY_Spd:
                AppDebugData.AddRotYLimitSpd = val;
                break;
            case OnEditNo.RotY_DampRate:
                AppDebugData.RotYDampRate = val;
                break;

            //---------------------------------------------------------------------------
            // 回転
            case OnEditNo.RotX:
                AppDebugData.AddRotXLimit = val;
                break;
            case OnEditNo.RotX_DampRate:
                AppDebugData.RotXDampRate = val;
                break;
            case OnEditNo.RotZ:
                AppDebugData.AddRotZLimit = val;
                break;
            case OnEditNo.RotZ_DampRate:
                AppDebugData.RotZDampRate = val;
                break;


            case OnEditNo.Bouciness:
                AppDebugData.DroneBounciness = val;
                MainGame.MG_Mediator.ObjMediator.DbgDroneBouncinessSetting();
                break;
        }

        m_InputField.text = val.ToString();
    }

    private void OnClick_Toggle()
    {
        m_isToggle = !m_isToggle;

        string text = m_ButtonText.text;

        // switch (m_ProcNo)
        // {
        // }

        m_ButtonText.text = text;

    }

    private string GetCameraModeText()
    {
        return m_isToggle ? "オン": "オフ";
    }
}
