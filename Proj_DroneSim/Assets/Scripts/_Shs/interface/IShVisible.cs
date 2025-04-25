/// <summary>
/// 表示切り替えようインターフェース
/// </summary>
public interface IShVisible
{
    void Show();
    void Hide();
    bool IsVisible{ get; }
}
