using System.Collections;
using UnityEngine;
/// <summary>
/// UI制御基礎スクリプト：
/// </summary>
public class ShUIBaseCtrl : MonoBehaviour, IShVisible
{
    //================================================
    // [///] 
    //================================================
    // IShVisibleで使用する表示変数
    public bool IsVisible { get; private set; } = false;

    //================================================
    // [///] 抽象化メソッド
    //================================================
    public virtual void ProcInitialize() { }

    //================================================
    // [///] 
    //================================================
    // 表示切り替え
    public void SetVisible(bool isVisible)
    {
        IsVisible = isVisible;
        gameObject.SetActive(IsVisible);
    }

    //================================================
    // [///] 表示切り替え
    //================================================
    void IShVisible.Show()
    {
        IsVisible = true;
        gameObject.SetActive(IsVisible);
    }
    void IShVisible.Hide()
    {
        IsVisible = false;
        gameObject.SetActive(IsVisible);
    }
}
