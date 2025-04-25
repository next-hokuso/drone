using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yns;
using Game;

namespace Game
{
    static public class GameParticleGenerator
    {
        private class PartcileLocalParameter
        {
            public PtNo m_PtNo;
            public Vector3 m_Pos;
            public Vector3 m_Scale;
            public Vector3 m_Angles;
            //
            public PartcileLocalParameter()
            {
                Reset();
            }
            //
            public void Reset()
            {
                m_PtNo = (PtNo)0;
                m_Pos = Vector3.zero;
                m_Scale = Vector3.zero;
                m_Angles = Vector3.zero;
            }
        };

        public enum PtNo
        {
            None = -1,
        }

        static private readonly string[] ParticlePrefabs =
        {
        };

        static private List<GameObject> m_Prefabs = null;
        static private GameObject m_goRoot = null;

        static private string GoParticleName = "Particle";

        static public void Setup()
        {
            if (m_Prefabs == null)
            {
                m_Prefabs = new List<GameObject>();
            }

            for (int i = 0; i < ParticlePrefabs.Length; ++i)
            {
                GameObject go = Resources.Load(ParticlePrefabs[i]) as GameObject;
                if (go != null)
                {
                    m_Prefabs.Add(go);
                }
            }
        }

        static public void SetRoot(GameObject _go)
        {
            m_goRoot = _go;
        }

        static public GameObject Create(PtNo _ptNo, Vector3 _pos, Vector3? _scl = null, Quaternion? _rot=null)
        {
            GameObject go = GameObject.Instantiate(m_Prefabs[(int)_ptNo]);
            if (go != null)
            {
                go.name = GoParticleName;
                go.AddComponent<GameParticleController>();
                if (m_goRoot != null)
                {
                    go.transform.parent = m_goRoot.transform;
                }
                go.transform.position = _pos;
                if (_scl != null)
                {
                    go.transform.localScale = _scl.Value;
                }
                if (_rot.HasValue)
                {
                    go.transform.rotation = _rot.Value;
                }

                return go;
            }
            return null;
        }
        static public GameObject CreateOnly(PtNo _ptNo, Vector3 _pos, Quaternion? _rot = null)
        {
            GameObject go = GameObject.Instantiate(m_Prefabs[(int)_ptNo]);
            if (go != null)
            {
                go.name = GoParticleName;
                if (m_goRoot != null)
                {
                    go.transform.parent = m_goRoot.transform;
                }
                go.transform.position = _pos;
                if (_rot.HasValue)
                {
                    go.transform.rotation = _rot.Value;
                }

                return go;
            }
            return null;
        }

        static public GameObject Create(PtNo _ptNo, GameObject _go)
        {
            GameObject go = GameObject.Instantiate(m_Prefabs[(int)_ptNo]);
            if(go != null)
            {
                go.name = GoParticleName;
                GameParticleController pc = go.AddComponent<GameParticleController>();
                if(m_goRoot != null)
                {
                    go.transform.parent = m_goRoot.transform;
                }
                go.transform.position = _go.transform.position;
                go.transform.rotation = _go.transform.localRotation;
                pc.SetObj(_go);

                return go;
            }
            return null;
        }

        static public GameObject Create_SetWait(PtNo _ptNo, Vector3 _pos, float _waitTime)
        {
            GameObject go = GameObject.Instantiate(m_Prefabs[(int)_ptNo]);
            if (go != null)
            {
                go.name = GoParticleName;
                if (m_goRoot != null)
                {
                    go.transform.parent = m_goRoot.transform;
                }
                go.AddComponent<GameParticleController>().SetLateCreate(_waitTime);
                go.transform.position = _pos;
                return go;
            }
            return null;
        }

        // パーティクル破壊時のコールバック設定
        static public void SetDestroyCallback(GameObject _go, GameParticleController.DestroyCallback _destroyCallback)
        {
            if (_go != null)
            {
                GameParticleController controller = _go.GetComponent<GameParticleController>();
                if (controller != null)
                {
                    controller.SetDestroyCallback(_destroyCallback);
                }
            }
        }
    }
}
