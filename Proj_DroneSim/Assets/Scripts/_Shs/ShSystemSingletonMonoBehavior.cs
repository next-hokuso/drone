using UnityEngine;

public class ShSystemSingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    /// <summary>
    /// InstanceをIのみで使用している
    /// </summary>
    public static T I
    {
        get
        {
            if (instance == null)
            {
                // シーンに新規GameObjectを作成してコンポーネントを付与
                GameObject go = new GameObject();
                go.name = "SystemSingletonObject";
                instance = go.AddComponent<T>();

                // GameObjectをシステム
                DontDestroyOnLoad(go);
                if (instance == null)
                {
                    Debug.LogError(typeof(T) + "is nothing");
                }
            }
            return instance;
        }
    }
}