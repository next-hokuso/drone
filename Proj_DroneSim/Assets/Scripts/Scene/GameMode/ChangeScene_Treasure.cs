using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン変更時に読み込み元からシーン変更処理を書く先用
/// シーン変更時にnew GameObject(); と DontDestroyOnLoadで破棄されないようにして使用想定
/// </summary>
public class ChangeScene_Treasure : MonoBehaviour
{
    private string _sceneName = "Game_Treasure";

    //================================================
    // [///] static : シーン変更処理用のGameObject生成
    //================================================
    public static void SetChangeScene()
    {
        GameObject go = new GameObject();
        DontDestroyOnLoad(go);
        go.AddComponent<ChangeScene_Treasure>();

        AppCommon.SetCurrentSceneName("Game_Treasure");
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

        // -------------------------------------------------------------------------------------------
        // ↓ ここからScene処理
        // シーンスクリプトを付与
        MainGame.Game_TreasureBaseCtrl ctrl = appCtrl.AddComponent<MainGame.Game_TreasureBaseCtrl>();

        // 初期化処理呼び出し
        ctrl.ProcInitialize();
        while (!ctrl.IsCompleteSetup())
        {
            yield return null;
        }

        // シーン設定
        AppCommon.m_NowLoadSceneNo = (int)AppCommon.LoadSceneNo.Game_Treasure;

        // 覆い画像のフェードアウト
        ShTransitionSys.SetFillOutCover();

        // シーン処理開始
        ctrl.ProcSceneProcStart();

        // -------------------------------------------------------------------------------------------
        // 自身を削除 : 用は済
        Destroy(gameObject);
    }
}
