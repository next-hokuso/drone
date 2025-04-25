using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    //================================================
    // [///] LicenseText
    //================================================
    public class TextData
    {
        public string m_TextId;
        public string m_Text;
        public TextAlignment m_TextAlignment;
        //
        public TextData(string _stringId, string _text)
        {
            Reset();
            m_TextId = _stringId;
            m_Text = _text;
        }
        //
        public void Reset()
        {
            m_TextId = "";
            m_Text = "";
            m_TextAlignment = TextAlignment.Left;
        }
    }
    //================================================
    // [///] AudioData
    //================================================
    public class AudioData
    {
        public string m_Id;
        public string m_Name;
        public int m_Prio;
        public string m_FilePath;
        public AudioClip m_Clip;
        public float m_Volume;
        //
        public AudioData(GDM_AudioData.Param _data)
        {
            Reset();
            m_Id = _data.Id;
            m_Name = _data.Name;
            m_Prio = _data.Prio;
            m_FilePath = _data.FilePath_JP;
            m_Volume = _data.Volume;
        }
        public void SetAudioClip(AudioClip _clip)
        {
            m_Clip = _clip;
        }
        //
        public void Reset()
        {
            m_Id = "";
            m_Name = "";
            m_Prio = 0;
            m_FilePath = "";
            m_Clip = null;
            m_Volume = 1.0f;
        }
    }
    public enum AudioId
    {
        None = -1,

        count,
        flying,
        landing,
        landing_propeller,
        takeoff,
        moter,
        metronome,
        BGM01,
    };
    //================================================
    // [///] MissionId
    //================================================
    public enum MissionHitId
    {
        None,
        OutArea,

        Area_H, // H範囲

        Area_A, // スクエア飛行用:範囲
        Area_B,
        Area_C,
        Area_D,
        Area_E,
        Check_AB,
        Check_BC,
        Check_CD,
        Check_DE,
        Check_EA,

        Eight_Left1,　// 8の字用:コーン位置での移動判定
        Eight_Left2,
        Eight_Left3,

        Eight_Right1,
        Eight_Right2,
        Eight_Right3,

        EndArea,
    }
    //================================================
    // [///] WindDirNo
    //================================================
    public enum WindDirNo
    {
        Up = 0,
        RightUp,
        Right,
        RightDown,

        Down,
        LeftDown,
        Left,
        LeftUp,
    }
    //================================================
    // [///] StageInfo
    //================================================
    public class StageSet
    {
        public enum Obj
        {
            None,
            Wall,           // 壊せる壁
            Bomb,           // 爆弾がある壁
            UnbreakWall,    // 壊せない壁
            OutsideWall,    // 外壁
            Goal,           // ゴール ※3セル占有
            Sell,           // SELL ※3セル占有
            Upgrade,        // UPGRADE ※3セル占有
            Start,          // 開始地点
            WarpBack,       // 開始地点へのワープ
            StageWarp,      // ステージワープ
            StageIdent,     // ステージ識別
            StageStart,     // ステージワープでの開始位置
            BreakWall0,     // 一気に壊せる壁はひとまず5パターン
            BreakWall1,
            BreakWall2,
            BreakWall3,
            BreakWall4,
            BreakWall5,
        }
        //
        public int m_MapSizeX;
        public int m_MapSizeY;
        public int m_Wall0Per;
        public int m_Wall1Per;
        public int m_Wall2Per;
        public int m_Item0Per;
        public int m_Item1Per;
        public List<Obj> m_MapObjList;
        public StageSet()
        {
            Reset();
        }
        //
        public void Reset()
        {
            m_MapSizeX = 0;
            m_MapSizeY = 0;
            m_Wall0Per = 0;
            m_Wall1Per = 0;
            m_Wall2Per = 0;
            m_Item0Per = 0;
            m_Item1Per = 0;
            m_MapObjList = new List<Obj>();
        }
    }

    //================================================
    // [///] StageSaveData
    //================================================
    public enum StageState
    {
        Lock = 0,   // ロック
        Open,       // 開放
        Clear,      // クリア
        Comingsoon, // Comingsoon

        Total,
    }
    // JsonUtility.FromJsonにて使用するためSerializable属性にしなければならない
    [System.Serializable]
    public struct StageSaveData
    {
        public int m_Hp;
        public int m_stHp;
        public StageSet.Obj m_Obj;
        public int m_WallIdx;
        public int m_MatIdx;
        public int m_ItemIdx;
        public int m_BreakWallIdx;
        public int m_StageNo;
        public bool m_IsExist;
        public bool m_IsActive;
        public bool m_IsPutBomb;
        public bool m_IsPutItem;
        public bool m_IsPutUnbreakWall;
        public bool m_IsPutComingSoonWall;
        //public bool m_IsPutCavityFind;

        public StageSaveData(
            int _hp = 0,
            int _sthp = 0,
            StageSet.Obj obj = StageSet.Obj.None,
            int _wallIdx = 0,
            int _matIdx = 0,
            int _itemIdx = -1,
            int _breakWallIdx = -1,
            int _stgno = 0,
            bool _isExist = false,
            bool _isActive = false,
            bool _isPutBomb = false,
            bool _isPutItem = false,
            bool _isPutUnbreakWall = false,
            bool _isPutComingSoonWalll = false
            //bool _isPutCavityFind = false
        )
        {
            m_Hp = _hp;
            m_stHp = _sthp;
            m_Obj = obj;
            m_WallIdx = _wallIdx;
            m_MatIdx = _matIdx;
            m_ItemIdx = _itemIdx;
            m_BreakWallIdx = _breakWallIdx;
            m_StageNo = _stgno;
            m_IsExist = _isExist;
            m_IsActive = _isActive;
            m_IsPutBomb = _isPutBomb;
            m_IsPutItem = _isPutItem;
            m_IsPutUnbreakWall = _isPutUnbreakWall;
            m_IsPutComingSoonWall = _isPutComingSoonWalll;
            //m_IsPutCavityFind = _isPutCavityFind;
        }
        public void Clear()
        {
            m_Hp = 0;
            m_stHp = 0;
            m_Obj = StageSet.Obj.None;
            m_WallIdx = 0;
            m_MatIdx = 0;
            m_ItemIdx = -1;
            m_BreakWallIdx = -1;
            m_StageNo = 0;
            m_IsExist = false;
            m_IsActive = false;
            m_IsPutBomb = false;
            m_IsPutItem = false;
            m_IsPutUnbreakWall = false;
            m_IsPutComingSoonWall = false;
            //m_IsPutCavityFind = false;
        }
        public void SetData(StageSaveData _data)
        {
            m_Hp = _data.m_Hp;
            m_stHp = _data.m_stHp;
            m_Obj = _data.m_Obj;
            m_WallIdx = _data.m_WallIdx;
            m_MatIdx = _data.m_MatIdx;
            m_ItemIdx = _data.m_ItemIdx;
            m_BreakWallIdx = _data.m_BreakWallIdx;
            m_StageNo = _data.m_StageNo;
            m_IsExist = _data.m_IsExist;
            m_IsActive = _data.m_IsActive;
            m_IsPutBomb = _data.m_IsPutBomb;
            m_IsPutItem = _data.m_IsPutItem;
            m_IsPutUnbreakWall = _data.m_IsPutUnbreakWall;
            m_IsPutComingSoonWall = _data.m_IsPutComingSoonWall;
            //m_IsPutCavityFind = _data.m_IsPutCavityFind;
        }
    }

    //================================================
    // [///] CavityFindInfo
    //================================================
    // JsonUtility.FromJsonにて使用するためSerializable属性にしなければならない
    [System.Serializable]
    public struct CavityFindInfo
    {
        public int m_StageNo;
        public int m_BreakWallIdx;
        public CavityFindInfo(int _stageNo, int _breakWallIdx)
        {
            m_StageNo = _stageNo;
            m_BreakWallIdx = _breakWallIdx;
        }
        public void Clear()
        {
            m_StageNo = 0;
            m_BreakWallIdx = 0;
        }
        public void SetData(CavityFindInfo _data)
        {
            m_StageNo = _data.m_StageNo;
            m_BreakWallIdx = _data.m_BreakWallIdx;
        }
    }
}
