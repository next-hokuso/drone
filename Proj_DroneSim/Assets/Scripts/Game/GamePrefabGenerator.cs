using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yns;
using Game;

namespace Game
{
    static public class GamePrefabGenerator
    {
        private class PrefabLocalParameter
        {
            public GoType m_PtNo;
            public Vector3 m_Pos;
            public Vector3 m_Scale;
            public Vector3 m_Angles;
            //
            public PrefabLocalParameter()
            {
                Reset();
            }
            //
            public void Reset()
            {
                m_PtNo = (GoType)0;
                m_Pos = Vector3.zero;
                m_Scale = Vector3.zero;
                m_Angles = Vector3.zero;
            }
        };

        public enum GoType
        {
            None = -1,
        }

        static private readonly string[] GoPrefabs =
        {
        };

        static private List<GameObject> m_Prefabs = null;
        static private GameObject m_goRoot = null;

        static private string GoPrefabName = "GeneratePrefab";

        static public void Setup()
        {
            if (m_Prefabs == null)
            {
                m_Prefabs = new List<GameObject>();
            }

            for (int i = 0; i < GoPrefabs.Length; ++i)
            {
                GameObject go = Resources.Load(GoPrefabs[i]) as GameObject;
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

        static public GameObject Create(GoType _type, Vector3 _pos, Quaternion? _rot=null)
        {
            GameObject go = GameObject.Instantiate(m_Prefabs[(int)_type]);
            if (go != null)
            {
                go.name = GoPrefabName;
                if (m_goRoot != null)
                {
                    go.transform.parent = m_goRoot.transform;
                }
                go.transform.position = _pos;
                if(_rot.HasValue)
                {
                    go.transform.rotation = _rot.Value;
                }

                return go;
            }
            return null;
        }
        static public GameObject CreateOnly(GoType _type, Vector3 _pos, Quaternion? _rot = null)
        {
            GameObject go = GameObject.Instantiate(m_Prefabs[(int)_type]);
            if (go != null)
            {
                go.name = GoPrefabName;
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

        static public GameObject Create(GoType _type, GameObject _go)
        {
            GameObject go = GameObject.Instantiate(m_Prefabs[(int)_type]);
            if(go != null)
            {
                go.name = GoPrefabName;
                if(m_goRoot != null)
                {
                    go.transform.parent = m_goRoot.transform;
                }
                go.transform.position = _go.transform.position;
                go.transform.rotation = _go.transform.localRotation;

                return go;
            }
            return null;
        }

        static public GameObject Create_SetWait(GoType _type, Vector3 _pos, float _waitTime)
        {
            GameObject go = GameObject.Instantiate(m_Prefabs[(int)_type]);
            if (go != null)
            {
                go.name = GoPrefabName;
                if (m_goRoot != null)
                {
                    go.transform.parent = m_goRoot.transform;
                }
                go.transform.position = _pos;
                return go;
            }
            return null;
        }
    }
}
