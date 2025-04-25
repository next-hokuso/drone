using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン変更時に読み込み元からシーン変更処理を書く先用
/// シーン変更時にnew GameObject(); と DontDestroyOnLoadで破棄されないようにして使用想定
/// </summary>
public class ChangeScene_Title : MonoBehaviour
{
    private string _sceneName = "Title";

    //================================================
    // [///] static : シーン変更処理用のGameObject生成
    //================================================
    public static void SetChangeScene()
    {
        GameObject go = new GameObject();
        DontDestroyOnLoad(go);
        go.AddComponent<ChangeScene_Title>();
    }

    //================================================
    //
    // [///] シーン変更処理
    //
    //================================================
    public IEnumerator Start()
    {
        // -------------------------------------------------------------------------------------------
        // シーンの読み込み
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Single);
        yield return asyncOperation;

        // シーンチェック
        Scene scene = SceneManager.GetSceneByName(_sceneName);
        if (scene == null)
        {
            Destroy(gameObject);    // 自身を削除
            yield break;
        }

        // シーンのAppControlを取得
        GameObject appCtrl = Shs.ShSceneUtils.GetAppCtrlObj(_sceneName);

        // シーン番号設定
        AppCommon.m_NowLoadSceneNo = (int)AppCommon.LoadSceneNo.Title;

        // -------------------------------------------------------------------------------------------
        // ↓ ここからScene処理
        // シーンスクリプトを付与
        Title.Ttl_TitleBaseCtrl ctrl = appCtrl.AddComponent<Title.Ttl_TitleBaseCtrl>();

        // 初期化処理呼び出し
        ctrl.ProcInitialize();
        while (!ctrl.IsCompleteSetup())
        {
            yield return null;
        }

        // 覆い画像のフェードアウト
        ShTransitionSys.SetFillOutCover(true, 0.1f);

        // シーン処理開始
        ctrl.ProcSceneProcStart();

        // -------------------------------------------------------------------------------------------
        // 自身を削除 : 用は済
        Destroy(gameObject);
    }
}
