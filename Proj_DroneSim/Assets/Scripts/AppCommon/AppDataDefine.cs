using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yns;

/// <summary>
/// アプリ内のデータ定義
/// </summary>
namespace Game
{
    //======================================================================
    //
    // [///] コインClass
    //
    //======================================================================
    public class Coin
    {
        // 最大コイン枚数
        private const long MaxCoin = AppData.CoinNumLimit;
        public long Num { get; private set; } = 0;
        public Coin(long num = 0)
        {
            if(num < 0)
            {
                Debug.Log("Error:コインに0未満の値が入りました");
            }
            Num = num;
        }
        public void Add(Coin _ohter)
        {
            Num = Math.Clamp(Num + _ohter.Num, 0, MaxCoin);
        }
        public void Degree(Coin _ohter)
        {
            Num = Math.Clamp(Num - _ohter.Num, 0, MaxCoin);
        }
        public void Clear()
        {
            Num = 0;
        }
        // 加算した際の増減コイン数を取得
        public long CalcAddNum(long add)
        {
            // 上限チェック
            if (Num + add > MaxCoin)
            {
                // 差し引いた数を返す
                long overNum = ((Num + add) - MaxCoin);
                return Math.Clamp(add - overNum, 0, add);   // 0 ～ addの値まで
            }
            else
            {
                // 増減ｺｲﾝそのまま
                return add;
            }
        }
        /* 2項演算子 */
        public static long operator +(Coin val, Coin val2)
        {
            return (val.Num + val2.Num);
        }
        public static long operator -(Coin val, Coin val2)
        {
            return (val.Num - val2.Num);
        }
        // public static int operator *(Coin val, int x)
        // {
        //     return (val.Num * x);
        // }
        // public static int operator /(Coin val, int x)
        // {
        //     return (val.Num / x);
        // }
    }
    //======================================================================
    //
    // [///] レベルClass
    //
    //======================================================================
    public class Lv
    {
        // 最大レベル
        private const int Max = AppData.ParamLvLimit;
        public int Num { get; private set; } = 0;
        public Lv(int num = 0)
        {
            if (num < 0)
            {
                Debug.Log("Error:レベルに0未満の値が入りました");
            }
            Num = num;
        }
        public void Add()
        {
            Num = Math.Clamp(Num + 1, 0, Max);
        }
        public string ToDispString()
        {
            return (Num + 1).ToString();
        }
        public void Clear()
        {
            Num = 0;
        }
        // 見た目Lvにて上限チェック
        public bool IsLimit()
        {
            return (Num + 1) >= Max;
        }
    }

    //======================================================================
    //
    // [///] ステージNoクラス
    //
    //======================================================================
    public class StageNo
    {
        // ステージ最大数
        private const int MaxStageNum = 15;
        /// <summary>ステージ番号</summary>
        public int No { get; private set; } = 0;
        public StageNo(int val = 0)
        {
            if (val < 0)
            {
                Debug.Log("Error:0未満の値が入りました");
            }
            No = val;
        }
        //ステージNoの加算
        public void AddStageNo()
        {
            No = Math.Clamp(No + 1, 0, MaxStageNum);
        }
        // ステージNoの選択
        public void SelectStageNo(int _no)
        {
            No = Math.Clamp(_no, 0, MaxStageNum);
        }

        // 引数とステージNoが同じか
        public bool IsEqualStageNo(int _no)
        {
            return No == _no;
        }
        //Stageが存在しているかのチェック
        static public bool IsExistStage(int _no)
        {
            return (0 < _no || _no + 1 < MaxStageNum);
        }
    }
    //======================================================================
    //
    // [///] ステージ関係
    //
    //======================================================================
    //---------------------------------------------------------------------
    /// <summary>ステージ情報</summary>
    public class StageInfoData
    {
        public StageState State { get; private set; } = StageState.Lock; // ステージ状態
        // public StageNo m_No;       // ステージ番号
        public StageInfoData(StageState state)
        {
            State = state;
        }
        public StageInfoData(StageInfoData _data)
        {
            State = _data.State;
        }

        // ステージの設定
        public void SetData(StageInfoData _data)
        {
            State = _data.State;
        }
        // ステージのクリア設定
        public void SetStageClear()
        {
            State = StageState.Clear;
        }
        // ステージの開放設定
        public void SetStageOpen()
        {
            State = StageState.Open;
        }
        // ステージ開放ができないかの確認
        public bool IsNotStageOpen()
        {
            // ロック以外の場合開放できない
            return State != StageState.Lock;
        }
    }
    //---------------------------------------------------------------------
    /// <summary>ステージ情報リスト</summary>
    public class StageInfoList
    {
        private List<StageInfoData> m_StageInfoList;
        public StageInfoList()
        {
            m_StageInfoList = new List<StageInfoData>();
        }
        public StageInfoList(List<StageInfoData> _list)
        {
            m_StageInfoList = new List<StageInfoData>();
            foreach (StageInfoData data in _list)
            {
                StageInfoData newData = new StageInfoData(StageState.Lock);
                newData.SetData(data);
                m_StageInfoList.Add(newData);
            }
        }
        // データの取得
        private StageInfoData GetInfoData(StageNo _stageNo)
        {
            return m_StageInfoList[_stageNo.No];
        }

        // ステージ情報の存在チェック
        private bool IsStageInfoExist(StageNo _stageNo)
        {
            // リストの数がステージ番号以上か
            return _stageNo.No < m_StageInfoList.Count;
        }
        // ステージのクリア処理
        public void ProcStageClear(StageNo _stageNo)
        {
            // 情報存在チェック
            if (!IsStageInfoExist(_stageNo)) return;

            // クリア設定
            GetInfoData(_stageNo).SetStageClear();
        }
        // 次のステージの解放処理
        public void ProcNextStageOpen(StageNo _nowStageNo)
        {
            // 現在のステージ
            StageNo nextStageNo = new StageNo(_nowStageNo.No + 1);

            // 情報存在チェック
            if (!IsStageInfoExist(nextStageNo)) return;
            // 次のステージ状態が開放できるかチェック
            if (GetInfoData(nextStageNo).IsNotStageOpen()) return;

            // 開放設定
            GetInfoData(nextStageNo).SetStageOpen();
        }
        //--------------------------------------------------
        // セーブデータから取り込み用
        // ステージ状態リストの設定
        public void SetStageStateList(int[] list)
        {
            for (int i = 0; i < list.Length; ++i)
            {
                if (i >= AppData.MaxStageNum) break;

                StageInfoData data = new StageInfoData((StageState)list[i]);
                m_StageInfoList.Add(data);
            }
        }
        //--------------------------------------------------
        // セーブデータ保存用
        // ステージ状態リストのコピー
        public void CopyStageStateList(ref int[] list)
        {
            for(int i = 0; i < m_StageInfoList.Count; ++i)
            {
                if (list.Length - 1 <= i) break;
                list[i] = (int)m_StageInfoList[i].State;
            }
        }
    }
    //======================================================================
    //
    // [///] オブジェクト関係
    //
    //======================================================================
    public interface IShObjNode
    {
        /// <summary>
        /// 初期化設定(各スクリプトごとの設定は別で呼び出すこと)
        /// </summary>
        void SetInit();

        /// <summary>
        /// ゲーム開始時呼び出し処理
        /// </summary>
        void ProcGameStart();
    }
    public class IShObjNodeList
    {
        private List<IShObjNode> m_ObjList = null;
        public IShObjNodeList()
        {
            m_ObjList = new List<IShObjNode>();
        }

        // 追加
        public void Add(IShObjNode _node)
        {
            m_ObjList.Add(_node);
        }
        // 開始処理の呼び出し
        public void ProcStart()
        {
            foreach (IShObjNode node in m_ObjList)
            {
                // @memo nullチェックを変更したい
                if (node != null)
                {
                    node.ProcGameStart();
                }
            }
        }
        // リストの破棄
    }
    //======================================================================
    //
    // [///] TimerClass
    //
    //======================================================================
    public class Timer
    {
        public float TimeVal { get; private set; } = 0.0f;
        public float EndTime { get; private set; } = 0.0f;
        public bool IsEnd { get; private set; } = false;
        public Timer(float _endTime)
        {
            if (_endTime < 0)
            {
                Debug.Log("Error:_endTimeに0未満の値が入りました");
            }
            EndTime = _endTime;
        }
        // 更新
        public void Update()
        {
            TimeVal = Math.Clamp(TimeVal + Time.deltaTime, 0, EndTime);
            if(TimeVal >= EndTime)
            {
                IsEnd = true;
            }
        }
        // リセット
        public void Reset()
        {
            TimeVal = 0.0f;
            IsEnd = false;
        }
        // 変更
        public void ChangeEndTime(float _endTime)
        {
            if (_endTime < 0)
            {
                Debug.Log("Error:_endTimeに0未満の値が入りました");
            }
            EndTime = _endTime;
        }
        // 値の取得
        public float GetVal()
        {
            return TimeVal / EndTime;
        }
        // 値の取得
        public float GetOutVal()
        {
            return 1.0f - TimeVal / EndTime;
        }
    }
    public class StopWatch
    {
        public float TimeVal { get;  set; } = 0.0f;
        public float EndTime { get; private set; } = 0.0f;
        public bool IsEnd { get; private set; } = false;
        public StopWatch()
        {
        }
        // 更新
        public void Update()
        {
            TimeVal = Math.Clamp(TimeVal + Time.fixedDeltaTime, 0, 300.0f);
            if (TimeVal >= EndTime)
            {
                IsEnd = true;
            }
        }
        // リセット
        public void Reset()
        {
            TimeVal = 0.0f;
            IsEnd = false;
        }
    }
}
