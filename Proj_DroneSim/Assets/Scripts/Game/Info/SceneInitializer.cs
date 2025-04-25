using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public abstract void ProcInit();
    /// <summary>
    /// リソース読み込み
    /// </summary>
    public abstract void ProcLoadResources();

}