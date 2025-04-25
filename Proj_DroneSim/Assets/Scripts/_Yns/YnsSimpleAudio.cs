using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yns;
using Game;

public class YnsSimpleAudio : MonoBehaviour
{
    // 
    public class AudioInfo
    {
        public AudioSource m_AudioSource;
        public bool m_isFirst;
        public bool m_isFadeOut;
        public float m_TargetVolume;
        public AudioInfo(AudioSource _clip)
        {
            m_AudioSource = _clip;
            m_isFirst = false;
            m_isFadeOut = false;
            m_TargetVolume = 1.0f;
        }
        public void Init()
        {
            if (m_AudioSource != null)
            {
                m_AudioSource.clip = null;
                m_AudioSource.loop = false;
                m_AudioSource.priority = 256;
            }
            m_isFirst = false;
            m_isFadeOut = false;
            m_TargetVolume = 1.0f;
        }
    }

    // 最大SE数
    const int SeInfoMax = 10;

    // 
    private AudioInfo m_BgmInfo = null;
    private AudioInfo[] m_SeInfo = null;
    private AudioInfo m_SubLoopSeInfo = null;

    private float m_MasterVolume = 1.0f;
    private float m_TargetVolume = 1.0f;
    private float m_Timer = 1.0f;
    private float m_TargetTimer = 1.0f;

    enum State
    {
        Idle,
        FadeIn,
        FadeOut,
    };
    private State m_State = State.Idle;

    //
    private void Awake()
    {
        if (m_BgmInfo == null)
        {
            m_BgmInfo = new AudioInfo(gameObject.GetComponent<AudioSource>());
            if (m_BgmInfo.m_AudioSource == null)
            {
                m_BgmInfo.m_AudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        if (m_SeInfo == null)
        {
            m_SeInfo = new AudioInfo[SeInfoMax];
            for (int i = 0; i < m_SeInfo.Length; ++i)
            {
                m_SeInfo[i] = new AudioInfo(gameObject.AddComponent<AudioSource>());
            }
        }
        if (m_SubLoopSeInfo == null)
        {
            m_SubLoopSeInfo = new AudioInfo(gameObject.AddComponent<AudioSource>());
            if (m_SubLoopSeInfo.m_AudioSource == null)
            {
                m_SubLoopSeInfo.m_AudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // サウンドのフェードイン・アウト処理
        if (m_State != State.Idle)
        {
            if (m_TargetTimer == 0.0f)
            {
                m_Timer = 1.0f;
            }
            else
            {
                m_Timer += (Time.deltaTime / m_TargetTimer);
            }
            if (m_Timer >= 1.0f)
            {
                m_Timer = 1.0f;
            }
            switch (m_State)
            {
                case State.FadeIn:
                    m_MasterVolume = Mathf.LerpUnclamped(0.0f, m_TargetVolume, m_Timer);
                    break;
                case State.FadeOut:
                    m_MasterVolume = Mathf.LerpUnclamped(1.0f, m_TargetVolume, m_Timer);
                    break;
            }
            if (m_MasterVolume == m_TargetVolume)
            {
                m_State = State.Idle;
            }
        }
        if (m_BgmInfo != null)
        {
            if (m_BgmInfo.m_AudioSource != null)
            {
                if (m_BgmInfo.m_AudioSource.clip != null)
                {
                    // 再生中ならマスターボリュームに合わせる
                    if (m_BgmInfo.m_AudioSource.isPlaying)
                    {
                        m_BgmInfo.m_AudioSource.volume = m_MasterVolume;
                    }
                    else
                    // 再生終了時にCLIPを削除する
                    {
                        m_BgmInfo.Init();
                    }
                }
            }
        }
        if (m_SeInfo != null)
        {
            for (int i = 0; i < m_SeInfo.Length; ++i)
            {
                if (m_SeInfo[i].m_AudioSource != null)
                {
                    if (m_SeInfo[i].m_AudioSource.clip != null)
                    {
                        // 再生中ならマスターボリュームに合わせる
                        if (m_SeInfo[i].m_AudioSource.isPlaying)
                        {
                            //m_SeInfo[i].m_AudioSource.volume = m_MasterVolume;

                            // @takeoff用の取り急ぎ
                            if (m_SeInfo[i].m_isFadeOut)
                            {
                                if(m_SeInfo[i].m_AudioSource.time > 2.5f)
                                {
                                    m_SeInfo[i].m_AudioSource.volume = (2.8f - m_SeInfo[i].m_AudioSource.time) / 0.3f * m_SeInfo[i].m_TargetVolume;
                                }
                            }
                        }
                        else
                        // 再生終了時にCLIPを削除する ※初回フラグが立っているSEを除く
                        if (!m_SeInfo[i].m_isFirst)
                        {
                            m_SeInfo[i].Init();
                        }
                    }
                }
            }
        }
        if (m_SubLoopSeInfo != null)
        {
            if (m_SubLoopSeInfo.m_AudioSource != null)
            {
                if (m_SubLoopSeInfo.m_AudioSource.clip != null)
                {
                    // 再生中ならマスターボリュームに合わせる
                    if (m_SubLoopSeInfo.m_AudioSource.isPlaying)
                    {
                        m_SubLoopSeInfo.m_AudioSource.volume = m_MasterVolume;
                    }
                    else
                    // 再生終了時にCLIPを削除する
                    {
                        m_SubLoopSeInfo.Init();
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (m_SeInfo != null)
        {
            // 初回フラグが立っているSEをここでコールする
            for (int i = 0; i < m_SeInfo.Length; ++i)
            {
                if (m_SeInfo[i].m_isFirst)
                {
                    m_SeInfo[i].m_isFirst = false;
                    if (m_SeInfo[i].m_AudioSource != null)
                    {
                        if (m_SeInfo[i].m_AudioSource.clip != null)
                        {
                            // m_SeInfo[i].m_AudioSource.volume = m_MasterVolume;
                            m_SeInfo[i].m_AudioSource.Play();
                        }
                    }
                }
            }
        }
    }

    public void SetFadeIn(float _time = 1.0f)
    {
        m_Timer = 0.0f;
        m_TargetTimer = _time;
        m_TargetVolume = 1.0f;
        m_State = State.FadeIn;
    }

    public void SetFadeOut(float _time = 1.0f)
    {
        m_Timer = 0.0f;
        m_TargetTimer = _time;
        m_TargetVolume = 0.0f;
        m_State = State.FadeOut;
    }

    public bool IsFadeCtrl()
    {
        return m_State != State.Idle;
    }

    public void SetInit()
    {
        if (m_BgmInfo != null)
        {
            if (m_BgmInfo.m_AudioSource != null)
            {
                if (m_BgmInfo.m_AudioSource.clip != null)
                {
                    if (m_BgmInfo.m_AudioSource.isPlaying)
                    {
                        m_BgmInfo.m_AudioSource.Stop();
                        m_BgmInfo.Init();
                    }
                }
            }
        }
        if (m_SeInfo != null)
        {
            for (int i = 0; i < m_SeInfo.Length; ++i)
            {
                if (m_SeInfo[i].m_AudioSource != null)
                {
                    if (m_SeInfo[i].m_AudioSource.clip != null)
                    {
                        if (m_SeInfo[i].m_AudioSource.isPlaying)
                        {
                            m_SeInfo[i].m_AudioSource.Stop();
                            m_SeInfo[i].Init();
                        }
                    }
                }
            }
        }
        if (m_SubLoopSeInfo != null)
        {
            if (m_SubLoopSeInfo.m_AudioSource != null)
            {
                if (m_SubLoopSeInfo.m_AudioSource.clip != null)
                {
                    if (m_SubLoopSeInfo.m_AudioSource.isPlaying)
                    {
                        m_SubLoopSeInfo.m_AudioSource.Stop();
                        m_SubLoopSeInfo.Init();
                    }
                }
            }
        }
        m_MasterVolume = 1.0f;
    }

    private int GetFreeSeAudioSourceIdx(AudioClip _clip, int _prio, bool _isLoop)
    {
        int ret = -1;
        if (m_SeInfo != null)
        {
            bool isSameSeEntry = false;
            if (_clip != null)
            {
                for (int i = 0; i < m_SeInfo.Length; ++i)
                {
                    if (m_SeInfo[i].m_isFirst)
                    {
                        if (m_SeInfo[i].m_AudioSource != null)
                        {
                            if (m_SeInfo[i].m_AudioSource.clip == _clip && m_SeInfo[i].m_AudioSource.loop == _isLoop)
                            {
                                ret = -2;
                                isSameSeEntry = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!isSameSeEntry)
            {
                int minPrio = int.MinValue;
                int minIdx = int.MaxValue;
                for (int i = 0; i < m_SeInfo.Length; ++i)
                {
                    if (m_SeInfo[i].m_AudioSource != null)
                    {
                        if (m_SeInfo[i].m_AudioSource.clip == null)
                        {
                            ret = i;
                            break;
                        }
                        else
                        if (m_SeInfo[i].m_AudioSource.priority > minPrio)
                        {
                            minPrio = m_SeInfo[i].m_AudioSource.priority;
                            minIdx = i;
                        }
                    }
                }
                if (ret < 0 && _prio != 256)
                {
                    if (minPrio > _prio && minIdx < m_SeInfo.Length)
                    {
                        m_SeInfo[minIdx].m_AudioSource.Stop();
                        m_SeInfo[minIdx].Init();
                        ret = minIdx;
                    }
                }
            }
        }
        return ret;
    }

    private int GetSeAudioSourceIdx(AudioClip _clip, bool _isLoop)
    {
        int ret = -1;
        if (m_SeInfo != null)
        {
            for (int i = 0; i < m_SeInfo.Length; ++i)
            {
                if (m_SeInfo[i].m_AudioSource != null)
                {
                    if (m_SeInfo[i].m_AudioSource.clip == _clip && m_SeInfo[i].m_AudioSource.loop == _isLoop)
                    {
                        ret = i;
                        break;
                    }
                }
            }
        }
        return ret;
    }

    private bool SetSeAudioAndPlay(AudioData _audioData, bool _isLoop, float _time = -1, bool _isSeFadeIn = false, bool _isSeFadeOut = false)
    {
        bool isSet = false;
        if (m_SeInfo != null && _audioData != null)
        {
            int idx = GetFreeSeAudioSourceIdx(_audioData.m_Clip, _audioData.m_Prio, _isLoop);
            if (idx >= 0)
            {
                m_SeInfo[idx].m_AudioSource.clip = _audioData.m_Clip;
                m_SeInfo[idx].m_AudioSource.loop = _isLoop;
                m_SeInfo[idx].m_AudioSource.priority = _audioData.m_Prio;
                m_SeInfo[idx].m_isFirst = true;
                m_SeInfo[idx].m_isFadeOut = _isSeFadeOut;
                m_SeInfo[idx].m_TargetVolume = _audioData.m_Volume;
                if (_time >= 0)
                {
                    m_SeInfo[idx].m_AudioSource.time = _time;
                }
                if (_isSeFadeIn)
                {
                    StartCoroutine(ProcSeFadeIn(idx));
                }
                else
                {
                    m_SeInfo[idx].m_AudioSource.volume = _audioData.m_Volume;
                }
            }
            else
            if (idx == -1)
            {
                if (_audioData.m_Clip)
                {
                    Debug.Log("<color=red>SE AUDIO RESOURCE RANGE OVER  " + _audioData.m_Clip.name + "   Loop " + _isLoop + "</color>");
                }
                else
                {
                    Debug.Log("<color=red>SE AUDIO RESOURCE RANGE OVER & ANIM CLIP NULL</color>");
                }
            }
        }
        return isSet;
    }
    // Seのフェードイン
    private IEnumerator ProcSeFadeIn(int idx)
    {
        m_SeInfo[idx].m_AudioSource.volume = 0.0f;
        float time = 0.0f;
        float endTime = 0.5f;
        while (true)
        {
            m_SeInfo[idx].m_AudioSource.volume = time / endTime * m_SeInfo[idx].m_TargetVolume;
            time += Time.deltaTime;
            if(time > endTime)
            {
                break;
            }
            yield return null;
        }

        m_SeInfo[idx].m_AudioSource.volume = m_SeInfo[idx].m_TargetVolume;
    }

    //
    public void PlaySeId(string _id, bool _isLoop = false)
    {
        if (m_SeInfo != null)
        {
            if (SetSeAudioAndPlay(GD_Audio.GetData(_id), _isLoop))
            {
                Debug.Log("PlaySE " + _id + "　Loop " + _isLoop);
            }
        }
    }

    //
    public void PlaySeId(string _id, bool _isLoop = false, float _time = -1)
    {
        if (m_SeInfo != null)
        {
            if (SetSeAudioAndPlay(GD_Audio.GetData(_id), _isLoop, _time))
            {
                Debug.Log("PlaySE " + _id + "　Loop " + _isLoop);
            }
        }
    }

    //
    public void PlaySe(string _name, bool _isLoop = false)
    {
        if (m_SeInfo != null)
        {
            if (SetSeAudioAndPlay(GD_Audio.GetDataName(_name), _isLoop))
            {
                Debug.Log("PlaySE " + _name + "　Loop " + _isLoop);
            }
        }
    }

    //
    public void PlaySe(string _name, bool _isLoop = false, float _time = -1, bool _isSeFadeIn = false, bool _isSeFadeOut = false)
    {
        if (m_SeInfo != null)
        {
            if (SetSeAudioAndPlay(GD_Audio.GetDataName(_name), _isLoop, _time, _isSeFadeIn, _isSeFadeOut))
            {
                Debug.Log("PlaySE " + _name + "　Loop " + _isLoop);
            }
        }
    }

    //
    public bool IsPlaySe(string _name, bool _isLoop = false)
    {
        bool res = false;
        if (m_SeInfo != null)
        {
            int idx = GetSeAudioSourceIdx(GD_Audio.GetAudioClipName(_name), _isLoop);
            if (idx >= 0)
            {
                return m_SeInfo[idx].m_AudioSource.isPlaying;
            }
        }
        return res;
    }

    //
    public void StopSeId(string _id, bool _isLoop = false)
    {
        if (m_SeInfo != null)
        {
            int idx = GetSeAudioSourceIdx(GD_Audio.GetAudioClip(_id), _isLoop);
            if (idx >= 0)
            {
                m_SeInfo[idx].m_AudioSource.Stop();
                m_SeInfo[idx].Init();
            }
            else
            {
                Debug.Log("<color=yellow>SE NOT FOUND IN AUDIO RESOURCE   ID:" + _id + "</color>");
            }
        }
    }

    //
    public void StopSe(string _name, bool _isLoop = false)
    {
        if (m_SeInfo != null)
        {
            int idx = GetSeAudioSourceIdx(GD_Audio.GetAudioClipName(_name), _isLoop);
            if (idx >= 0)
            {
                m_SeInfo[idx].m_AudioSource.Stop();
                m_SeInfo[idx].Init();
            }
            else
            {
                Debug.Log("<color=yellow>SE NOT FOUND IN AUDIO RESOURCE   NAME:" + _name + "</color>");
            }
        }
    }

    //
    public void PlayBgmId(string _id, bool _isLoop = true)
    {
        if (m_BgmInfo != null)
        {
            Debug.Log("PlayBGM " + _id + "　Loop " + _isLoop);
            m_BgmInfo.m_AudioSource.clip = GD_Audio.GetAudioClip(_id);
            m_BgmInfo.m_AudioSource.loop = _isLoop;
            m_BgmInfo.m_AudioSource.priority = GD_Audio.GetAudioPriority(_id);
            m_BgmInfo.m_AudioSource.Play();
        }
    }

    //
    public void PlayBgm(string _name, bool _isLoop = true)
    {
        if (m_BgmInfo != null)
        {
            Debug.Log("PlayBGM " + _name + "　Loop " + _isLoop);
            m_BgmInfo.m_AudioSource.clip = GD_Audio.GetAudioClipName(_name);
            m_BgmInfo.m_AudioSource.loop = _isLoop;
            m_BgmInfo.m_AudioSource.priority = GD_Audio.GetAudioPriorityName(_name);
            m_BgmInfo.m_AudioSource.Play();
        }
    }

    //
    public void StopBgm()
    {
        if (m_BgmInfo != null)
        {
            m_BgmInfo.m_AudioSource.Stop();
            m_BgmInfo.Init();
        }
    }

    //
    public void PlaySubLoopSe(string _name, bool _isLoop = true)
    {
        if (m_SubLoopSeInfo != null)
        {
            m_SubLoopSeInfo.m_AudioSource.clip = GD_Audio.GetAudioClipName(_name);
            m_SubLoopSeInfo.m_AudioSource.loop = _isLoop;
            m_SubLoopSeInfo.m_AudioSource.priority = GD_Audio.GetAudioPriorityName(_name);
            m_SubLoopSeInfo.m_AudioSource.Play();
        }
    }

    //
    public void StopSubLoopSe()
    {
        if (m_SubLoopSeInfo != null)
        {
            m_SubLoopSeInfo.m_AudioSource.Stop();
            m_SubLoopSeInfo.Init();
        }
    }
}
