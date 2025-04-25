using UnityEngine;

/// <summary>
/// 振動 呼び出し
/// </summary>
public class ShsVibrationSys : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    private static ShsVibrationProc m_Proc = null;

    //================================================
    // [///]
    //================================================
    // Start is called before the first frame update
    void Start()
    {
        m_Proc = gameObject.AddComponent<ShsVibrationProc>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // 振動 - 短い
    static public void SetVibration_Small()
    {
        m_Proc.SetVibration_Small();
    }

    // 振動 - 線描画中
    static public void SetVibration_Draw()
    {
        m_Proc.SetVibration_Draw();
    }

    // 振動 - ボールが落下しオブジェクトに接触した時
    static public void SetVibration_BallHitObject()
    {
        m_Proc.SetVibration_BallHitObject();
    }

    // 振動 - コインゲート/パネル通過時
    static public void SetVibration_CoinPanelAndGate()
    {
        m_Proc.SetVibration_CoinPanelAndGate();
    }

    // 振動 - ボールが穴に入った時
    static public void SetVibration_Goal()
    {
        m_Proc.SetVibration_Goal();
    }
}