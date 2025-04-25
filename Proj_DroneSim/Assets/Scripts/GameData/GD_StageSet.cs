using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GD_StageSet : MonoBehaviour
    {
        // 格納リスト
        static public List<StageSet> m_Datas = null;
        static public bool m_IsLoading = false;
        // リソース
        static private readonly string DataPath = "_GameData/StageSetList";
        //
        static public void Setup()
        {
            if (m_Datas == null)
            {
                m_Datas = new List<StageSet>();
            }
        }
        //
        static public IEnumerator Load()
        {
            Setup();

            if (m_Datas.Count <= 0)
            {
                // 非同期読込み開始
                ResourceRequest request = Resources.LoadAsync(DataPath, typeof(GDM_StageSet));
                // 読込み待ち
                yield return request;
                // 結果取得
                GDM_StageSet masterDatas = request.asset as GDM_StageSet;
                if (masterDatas != null)
                {
                    // --- シートごとの読み込み ---------------------------------
                    for (int i = 0; i < masterDatas.sheets.Count; ++i)
                    {
                        // ステージデータ
                        StageSet data = new StageSet();

                        // 列チェック
                        for (int j = 0; j < masterDatas.sheets[i].list.Count; ++j)
                        {
                            // 列データ
                            GDM_StageSet.Param param = masterDatas.sheets[i].list[j];

                            if (j == 0)
                            {
                                data.m_MapSizeX = int.Parse(param.X0a);
                                data.m_MapSizeY = int.Parse(param.X0b);
                                data.m_Wall0Per = int.Parse(param.X1a);
                                data.m_Wall1Per = int.Parse(param.X1b);
                                data.m_Wall2Per = int.Parse(param.X2a);
                                data.m_Item0Per = int.Parse(param.X2b);
                                data.m_Item1Per = int.Parse(param.X3a);
                            }
                            else if (j <= data.m_MapSizeY*2)
                            {
                                for (int k = 0; k < data.m_MapSizeX*2; ++k)
                                {
                                    if (k % 2 == 0 && j % 2 == 0)
                                    {
                                        data.m_MapObjList.Add(GetStageParamData(param, k));
                                    }
                                }
                            }
                            //  ↓ -------------------------------------------------------------
                            // 終了チェック
                            else
                            {
                                break;
                            }
                        }

                        // ステージデータを保持
                        {
                            m_Datas.Add(data);
                        }
                    }
                }
            }

            m_IsLoading = true;
        }

        // リスト取得
        static public List<StageSet> GetDatas()
        {
            return m_Datas;
        }

        // データ取得
        static public StageSet GetData(int _index)
        {
            return m_Datas[_index];
        }

        // //================================================
        // // [///] データ設定関連
        // //================================================
        static private StageSet.Obj GetParamObj(string str)
        {
            StageSet.Obj obj = StageSet.Obj.None;
            switch (str)
            {
                case "TR":
                    obj = StageSet.Obj.Wall;
                    break;

                // case "WA":
                //     obj = StageSet.Obj.Wall;
                //     break;
                // case "BO":
                //     obj = StageSet.Obj.Bomb;
                //     break;
                // case "UW":
                //     obj = StageSet.Obj.UnbreakWall;
                //     break;
                // case "OW":
                //     obj = StageSet.Obj.OutsideWall;
                //     break;
                // case "GO":
                //     obj = StageSet.Obj.Goal;
                //     break;
                // case "W0":
                //     obj = StageSet.Obj.BreakWall0;
                //     break;
                // case "W1":
                //     obj = StageSet.Obj.BreakWall1;
                //     break;
                // case "W2":
                //     obj = StageSet.Obj.BreakWall2;
                //     break;
                // case "W3":
                //     obj = StageSet.Obj.BreakWall3;
                //     break;
                // case "W4":
                //     obj = StageSet.Obj.BreakWall4;
                //     break;
                // case "W5":
                //     obj = StageSet.Obj.BreakWall5;
                //     break;
                // case "SE":
                //     obj = StageSet.Obj.Sell;
                //     break;
                // case "UP":
                //     obj = StageSet.Obj.Upgrade;
                //     break;
                // case "ST":
                //     obj = StageSet.Obj.Start;
                //     break;
                // case "WB":
                //     obj = StageSet.Obj.WarpBack;
                //     break;
                // case "SW":
                //     obj = StageSet.Obj.StageWarp;
                //     break;
                // case "SI":
                //     obj = StageSet.Obj.StageIdent;
                //     break;
                // case "SS":
                //     obj = StageSet.Obj.StageStart;
                //     break;
            }
            return obj;
        }

        static private StageSet.Obj GetStageParamData(GDM_StageSet.Param _param, int _index)
        {
            switch (_index)
            {
                case 0: return GetParamObj(_param.X0a);
                case 1: return GetParamObj(_param.X0b);
                case 2: return GetParamObj(_param.X1a);
                case 3: return GetParamObj(_param.X1b);
                case 4: return GetParamObj(_param.X2a);
                case 5: return GetParamObj(_param.X2b);
                case 6: return GetParamObj(_param.X3a);
                case 7: return GetParamObj(_param.X3b);
                case 8: return GetParamObj(_param.X4a);
                case 9: return GetParamObj(_param.X4b);
                case 10: return GetParamObj(_param.X5a);
                case 11: return GetParamObj(_param.X5b);
                case 12: return GetParamObj(_param.X6a);
                case 13: return GetParamObj(_param.X6b);
                case 14: return GetParamObj(_param.X7a);
                case 15: return GetParamObj(_param.X7b);
                case 16: return GetParamObj(_param.X8a);
                case 17: return GetParamObj(_param.X8b);
                case 18: return GetParamObj(_param.X9a);
                case 19: return GetParamObj(_param.X9b);
                case 20: return GetParamObj(_param.X10a);
                case 21: return GetParamObj(_param.X10b);
                case 22: return GetParamObj(_param.X11a);
                case 23: return GetParamObj(_param.X11b);
                case 24: return GetParamObj(_param.X12a);
                case 25: return GetParamObj(_param.X12b);
                case 26: return GetParamObj(_param.X13a);
                case 27: return GetParamObj(_param.X13b);
                case 28: return GetParamObj(_param.X14a);
                case 29: return GetParamObj(_param.X14b);
                case 30: return GetParamObj(_param.X15a);
                case 31: return GetParamObj(_param.X15b);
                case 32: return GetParamObj(_param.X16a);
                case 33: return GetParamObj(_param.X16b);
                case 34: return GetParamObj(_param.X17a);
                case 35: return GetParamObj(_param.X17b);
                case 36: return GetParamObj(_param.X18a);
                case 37: return GetParamObj(_param.X18b);

                    // case 19: return GetParamObj(_param.X19);
                    // case 20: return GetParamObj(_param.X20);
                    // case 21: return GetParamObj(_param.X21);
                    // case 22: return GetParamObj(_param.X22);
                    // case 23: return GetParamObj(_param.X23);
                    // case 24: return GetParamObj(_param.X24);
                    // case 25: return GetParamObj(_param.X25);
                    // case 26: return GetParamObj(_param.X26);
                    // case 27: return GetParamObj(_param.X27);
                    // case 28: return GetParamObj(_param.X28);
                    // case 29: return GetParamObj(_param.X29);
                    // case 30: return GetParamObj(_param.X30);
                    // case 31: return GetParamObj(_param.X31);
                    // case 32: return GetParamObj(_param.X32);
                    // case 33: return GetParamObj(_param.X33);
                    // case 34: return GetParamObj(_param.X34);
                    // case 35: return GetParamObj(_param.X35);
                    // case 36: return GetParamObj(_param.X36);
            }
            return 0;
        }
    }
}
