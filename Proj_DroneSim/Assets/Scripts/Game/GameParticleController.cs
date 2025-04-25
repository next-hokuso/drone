using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParticleController : MonoBehaviour
{

    private ParticleSystem[] m_Particle = null;
    private GameObject m_goObj = null;
    // delegate宣言
    public delegate void DestroyCallback(GameObject go);
    // 破壊時のコールバックメソッドを格納する変数
    private DestroyCallback m_DestroyCallback = null;

    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem pat = gameObject.GetComponent<ParticleSystem>();
        if(pat == null)
        {
            gameObject.AddComponent<ParticleSystem>();
        }
        // 自身を含む全てのParticleSystemコンポーネント取得 ※非アクティブは取得しない
        m_Particle = gameObject.GetComponentsInChildren<ParticleSystem>(false);
        // パーティクル開始 ※子のパーティクルに関しては True にしても変わらない模様
        m_Particle[0].Play();
    }

    // Update is called once per frame
    void Update()
    {
        bool isStopped = true;
        // 子を含む全てのパーティクルが停止しているかどうか調べる
        for (int i = 0; i < m_Particle.Length; ++i)
        {
            if (!m_Particle[i].isStopped)
            {
                isStopped = false;
            }
        }
        // 全て停止していたら破壊
        if(isStopped)
        {
            // コールバックが登録されている時だけ
            if (m_DestroyCallback != null)
            {
                m_DestroyCallback(gameObject);
            }
            Destroy(gameObject);
        }
        else
        {
            if(m_goObj != null)
            {
                // 設定GameObjectに追従
                gameObject.transform.position = m_goObj.transform.position;
            }
        }
    }

    //
    public void SetObj(GameObject _go)
    {
        m_goObj = _go;
    }

    // 破壊時のコールバック登録
    public void SetDestroyCallback(DestroyCallback _destroyCallback)
    {
        m_DestroyCallback = _destroyCallback;
    }

    //
    public void SetLateCreate(float _waitTime)
    {
        StartCoroutine(LateCreate(_waitTime));
    }
    public IEnumerator LateCreate(float _waitTime)
    {
        enabled = false;

        yield return new WaitForSeconds(_waitTime);

        enabled = true;
    }
}
