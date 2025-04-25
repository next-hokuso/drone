using System.Collections;
using UnityEngine;
/// <summary>
/// シーンの制御スクリプト：
/// 　シーンの読み込み時に呼び出すものを抽象化
/// 　各シーンごとに読み込まれるものは継承先で実装する
/// 　(シーンのから呼び出す)
/// </summary>
public class ShSceneBaseCtrl : MonoBehaviour
{
    protected bool m_IsCompleteSceneSetup = false;

    //================================================
    //
    // [///] MonoBehaviour
    //
    //================================================
    public virtual void Start() { }
    public virtual void Update() { }

    //================================================
    //
    // [///] virtual method
    //
    //================================================
    /// <summary>シーンの初期化</summary>
    public virtual void ProcInitialize() { }

    /// <summary>シーンに必要なリソース読み込み</summary>
    public virtual IEnumerator ProcResourcesLoad() { yield break; }

    /// <summary>シーンの準備が完了したか</summary>
    public virtual bool IsCompleteSetup() { return m_IsCompleteSceneSetup; }

    /// <summary>シーンの処理開始</summary>
    public virtual void ProcSceneProcStart() { }

    /// <summary>シーンの変更処理</summary>
    public virtual void ProcChangeScene() { }
}
