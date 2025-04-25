using UnityEngine;

public class ShSingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
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
                go.name = "SingletonObject";
                instance = go.AddComponent<T>();
                if (instance == null)
                {
                    Debug.LogError(typeof(T) + "is nothing");
                }
            }
            return instance;
        }
    }
}