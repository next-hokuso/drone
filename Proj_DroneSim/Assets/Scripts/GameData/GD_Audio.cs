using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    static public class GD_Audio
    {
        static public List<AudioData> m_Datas = null;
        static public bool m_IsLoading = false;

        static private readonly string DataPath = "_GameData/AudioListData";

        static public void Setup()
        {
            if (m_Datas == null)
            {
                m_Datas = new List<AudioData>();
            }
        }

        static public IEnumerator Load()
        {
            Setup();

            if (m_Datas.Count <= 0)
            {
                // 非同期読込み開始
                ResourceRequest request = Resources.LoadAsync(DataPath, typeof(GDM_AudioData));
                // 読込み待ち
                yield return request;
                // 結果取得
                GDM_AudioData masterParams = request.asset as GDM_AudioData;
                if (masterParams != null)
                {
                    for (int i = 0; i < masterParams.sheets.Count; ++i)
                    {
                        for (int j = 0; j < masterParams.sheets[i].list.Count; ++j)
                        {
                            GDM_AudioData.Param param = masterParams.sheets[i].list[j];
                            AudioData data = new AudioData(param);
                            m_Datas.Add(data);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("オーディオクリップ読込み済");
            }

            for (int i = 0; i < m_Datas.Count; ++i)
            {
                if (m_Datas[i].m_Clip == null)
                {
                    // 非同期読込み開始
                    ResourceRequest request = Resources.LoadAsync(m_Datas[i].m_FilePath, typeof(AudioClip));
                    // 読込み待ち
                    yield return request;
                    // 結果取得
                    AudioClip masterData = request.asset as AudioClip;
                    if (masterData != null)
                    {
                        m_Datas[i].SetAudioClip(masterData);
                    }
                }
            }

            m_IsLoading = true;

            yield return null;
        }

        //
        static public AudioData GetData(string _id)
        {
            for (int i = 0; i < m_Datas.Count; ++i)
            {
                if (_id == m_Datas[i].m_Id)
                {
                    return m_Datas[i];
                }
            }
            return null;
        }

        //
        static public AudioClip GetAudioClip(string _id)
        {
            AudioData data = GetData(_id);
            if (data != null)
            {
                return data.m_Clip;
            }
            return null;
        }

        //
        static public int GetAudioPriority(string _id)
        {
            AudioData data = GetData(_id);
            if (data != null)
            {
                return data.m_Prio;
            }
            return -1;
        }

        //
        static public AudioData GetDataName(string _name)
        {
            for (int i = 0; i < m_Datas.Count; ++i)
            {
                if (_name == m_Datas[i].m_Name)
                {
                    return m_Datas[i];
                }
            }
            return null;
        }

        //
        static public AudioClip GetAudioClipName(string _name)
        {
            AudioData data = GetDataName(_name);
            if (data != null)
            {
                return data.m_Clip;
            }
            return null;
        }

        //
        static public int GetAudioPriorityName(string _name)
        {
            AudioData data = GetDataName(_name);
            if (data != null)
            {
                return data.m_Prio;
            }
            return -1;
        }

        //
        static public AudioData GetData(int _index)
        {
            if (m_Datas.Count > _index)
            {
                return m_Datas[_index];
            }
            return null;
        }

        //
        static public AudioClip GetAudioClip(int _index)
        {
            AudioData data = GetData(_index);
            if (data != null)
            {
                return data.m_Clip;
            }
            return null;
        }

        //
        static public int GetAudioPriority(int _index)
        {
            AudioData data = GetData(_index);
            if (data != null)
            {
                return data.m_Prio;
            }
            return -1;
        }
    }
}
