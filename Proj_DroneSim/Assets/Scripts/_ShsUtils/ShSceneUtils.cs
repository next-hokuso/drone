using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yns;

namespace Shs
{
    /// <summary>
    /// シーン用便利メソッド
    /// </summary>
    public class ShSceneUtils
    {
        //---------------------------------------------------------------------
        // 新規シーンの読み込み
        public static IEnumerator TransitionMainGame(string _sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Single);
            yield return asyncOperation;

            // シーンチェック
            Scene scene = SceneManager.GetSceneByName(_sceneName);
            if (scene == null)
            {
                yield break;
            }

            // シーンのAppControlを取得
            GameObject appCtrl = GetAppCtrlObj(_sceneName);

            // ↓ ここからMainGame処理
            MainGame.MainGameBaseCtrl ctrl = appCtrl.AddComponent<MainGame.MainGameBaseCtrl>();
            ctrl.ProcInitialize();
            while (!ctrl.IsCompleteSetup())
            {
                yield return null;
            }

            // 覆い画像のフェードアウト
            ShTransitionSys.SetFillOutCover();

            // 開始
            ctrl.ProcSceneProcStart();
        }

        //---------------------------------------------------------------------
        // 追加シーンの読み込み
        public static IEnumerator LoadAddtiveScene(string _sceneName)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);

            // シーンが準備完了となったタイミングで直ぐにシーンが有効化されるか(アクセスは有効化されるまでできない)
            // async.allowSceneActivation = false;
            while (!async.isDone)
            {
                yield return null;
            }
            // async.allowSceneActivation = true;
        }

        //---------------------------------------------------------------------
        // シーンのAppControlを取得
        public static GameObject GetAppCtrlObj(string _sceneName)
        {
            GameObject appCtrl = GetSceneRootObj(_sceneName, "AppControl");
            if (appCtrl == null)
            {
                appCtrl = new GameObject();
                appCtrl.name = "AppControl";
            }
            return appCtrl;
        }

        //---------------------------------------------------------------------
        // シーンのRootオブジェクトの取得
        public static GameObject GetSceneRootObj(string _sceneName, string _objName)
        {
            // シーンチェック
            Scene scene = SceneManager.GetSceneByName(_sceneName);
            if (scene == null)
            {
                return null;
            }

            // 検索
            GameObject[] rootObjects = scene.GetRootGameObjects();
            GameObject findObj = null;
            foreach (GameObject obj in rootObjects)
            {
                if (obj.name == _objName)
                {
                    findObj = obj;
                    break;
                }
            }
            return findObj;
        }

        //---------------------------------------------------------------------
        // 追加シーンのカメラ破棄
        public static void AddSceneCameraDestroy(string _sceneName, string _objName)
        {
            GameObject camera = GetSceneRootObj(_sceneName, _objName);
            if (camera)
            {
                GameObject.Destroy(camera);
            }
        }
    }
}