using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainGame {
    public class DebugMainGameSettings : MonoBehaviour
    {
        //================================================
        // [///] 定義
        //================================================
        private class ContentInfo
        {
            public int m_CategoryNo = 0;
            public string m_Text = "";
            public ContentInfo(int no, string text)
            {
                m_CategoryNo = no;
                m_Text = text;
            }
        }
        private enum ContentCategoryNo
        {
            Title = 0,
            Input,
            Input2,
            Toggle,
        }
        private ContentInfo[] contentInfos =
        {
            new ContentInfo((int)ContentCategoryNo.Title,      "□Debug"),
            new ContentInfo((int)ContentCategoryNo.Input,      "  上昇下降速度     :"),
            new ContentInfo((int)ContentCategoryNo.Input2,     "  上昇下降減衰割合 :"),
            new ContentInfo((int)ContentCategoryNo.Input,      "  移動速度横       :"),
            new ContentInfo((int)ContentCategoryNo.Input2,     "  移動減衰割合横   :"),
            new ContentInfo((int)ContentCategoryNo.Input,      "  移動速度前方     :"),
            new ContentInfo((int)ContentCategoryNo.Input2,     "  移動減衰割合前方 :"),

            new ContentInfo((int)ContentCategoryNo.Input,      "  回転速度横       :"),
            new ContentInfo((int)ContentCategoryNo.Input2,     "  回転減衰割合横   :"),

            new ContentInfo((int)ContentCategoryNo.Input,      "  傾き横           :"),
            new ContentInfo((int)ContentCategoryNo.Input2,     "  傾き減衰割合横   :"),
            new ContentInfo((int)ContentCategoryNo.Input,      "  傾き前後         :"),
            new ContentInfo((int)ContentCategoryNo.Input2,     "  傾き減衰割合前後 :"),

            new ContentInfo((int)ContentCategoryNo.Input2,     "  バウンド割合     :"),
        };

        //================================================
        // [///] 定義
        //================================================
        // Game Object
        protected enum GoNo
        {
            Btn_Return,

            List_Content,

            Grp_Menu,
            Grp_Force,
        }
        protected List<GameObject> m_Gos = new List<GameObject>();
        // Text
        protected enum TextNo
        {
        }
        protected List<Text> m_Texts = new List<Text>();
        // Image
        protected enum ImageNo
        {
        }

        private MGD_DebugCtrl m_ParentCtrl = null;

        // コンテンツのリスト
        private List<ContentInfo> m_ContentList = new List<ContentInfo>();

        // UIリスト
        private List<DbgContentInfo_MainGame> m_DbgContentInfoList = new List<DbgContentInfo_MainGame>();

        //================================================
        // [///]
        //================================================
        private void Start()
        {
            // Go
            m_Gos.Clear();
            {
                m_Gos.Add(transform.Find("Grp_Menu/Btn_Return").gameObject);
                m_Gos.Add(transform.Find("Grp_Menu/SetRoot/Content").gameObject);

                m_Gos.Add(transform.Find("Grp_Menu").gameObject);
                m_Gos.Add(transform.Find("Grp_AddForce").gameObject);
            }
            // テキスト
            m_Texts.Clear();
            {
            }
            // 機能付与
            {
                GetGosList()[(int)GoNo.Btn_Return].GetComponent<Button>().onClick.AddListener(OnClick_Return);
                //transform.Find("Grp_Menu/Btn_TrainParam").GetComponent<Button>().onClick.AddListener(OnClick_TrainParam);
                transform.Find("Grp_AddForce/Btn_ReturnSelect").GetComponent<Button>().onClick.AddListener(OnClick_MainGameReset);
                //transform.Find("Grp_AddForce/Btn_SaveDataDelete").GetComponent<Button>().onClick.AddListener(OnClick_DeleteData);
                transform.Find("Grp_Menu/Btn_Reset").GetComponent<Button>().onClick.AddListener(OnClick_ResetParam);

                // リスト設定
                SetStSetting();
            }

            // 情報設定
            UpdateSetting();
        }
        public void SetInfo(MGD_DebugCtrl scene)
        {
            m_ParentCtrl = scene;
        }
        private void OnEnable()
        {
            UpdateSetting();
        }
        //================================================
        // [///] private method
        //================================================
        //
        private List<GameObject> GetGosList()
        {
            return m_Gos;
        }
        private List<Text> GetTextsList()
        {
            return m_Texts;
        }
        //
        private GameObject GetGoVal(GoNo no)
        {
            return m_Gos[(int)no];
        }
        private Text GetTextVal(TextNo no)
        {
            return m_Texts[(int)no];
        }

        // 初期設定
        private void SetStSetting()
        {
            // リストの追加
            m_ContentList.Clear();
            foreach (ContentInfo info in contentInfos)
            {
                m_ContentList.Add(info);
            }

            List<GameObject> prefabList = new List<GameObject>();
            prefabList.Add(GetGoVal(GoNo.List_Content).transform.Find("Title").gameObject);
            prefabList.Add(GetGoVal(GoNo.List_Content).transform.Find("param").gameObject);
            prefabList.Add(GetGoVal(GoNo.List_Content).transform.Find("param2").gameObject);
            prefabList.Add(GetGoVal(GoNo.List_Content).transform.Find("paramToggle").gameObject);

            // Stage分のリスト作成
            int idx = 0;
            foreach (ContentInfo info in m_ContentList)
            {
                {
                    GameObject go = Instantiate(prefabList[info.m_CategoryNo]);
                    if (go)
                    {
                        Transform t = go.transform;
                        go.name = prefabList[info.m_CategoryNo].gameObject.name;
                        t.SetParent(GetGoVal(GoNo.List_Content).transform);
                        t.localScale = Vector3.one;
            
                        // ctrl
                        DbgContentInfo_MainGame ctrl = go.AddComponent<DbgContentInfo_MainGame>();
                        ctrl.SetInfoText(info.m_Text);
                        ctrl.SetDebugMainGameSettings(this);
                        if (info.m_CategoryNo != (int)ContentCategoryNo.Title)
                        {
                            ctrl.ProcSetOnEndEdit((DbgContentInfo_MainGame.OnEditNo)idx);
                            m_DbgContentInfoList.Add(ctrl);
                            idx++;
                        }
                        else
                        {
            
                        }
                    }
                }
            }

            foreach (GameObject go in prefabList)
            {
                Destroy(go);
            }
        }
        // 更新設定
        private void UpdateSetting()
        {
            //// 更新
            //foreach (DbgContentInfo_MainGame info in m_DbgContentInfoList)
            //{
            //    info.UpdateInfo();
            //}

            // 更新
            foreach (DbgContentInfo_MainGame info in m_DbgContentInfoList)
            {
                info.UpdateInfo();
            }
        }

        public void UpdateInfo(int idx)
        {
            m_DbgContentInfoList[idx].UpdateInfo();
        }

        //================================================
        // [///] OnClick
        //================================================
        public void OnClick_Return()
        {
            // セーブ対応

            // 戻る
            m_ParentCtrl.OnClick_Return();

            // 保存
            AppCommon.Update_And_SaveGameData();
        }
        // ゲームリセット
        public void OnClick_MainGameReset()
        {
            // 戻る
            m_ParentCtrl.OnClick_Return();

            MG_Mediator.StateCtrl.SetMainGameReset();
        }
        // リセットﾊﾟﾗﾒｰﾀ
        public void OnClick_ResetParam()
        {
            // 更新
            foreach (DbgContentInfo_MainGame info in m_DbgContentInfoList)
            {
                info.ResetInfo();
            }
        }

        public void DisActiveOtherDebugUI()
        {
        }
        public void OnClick_StageInfoReset()
        {
            // データ反映
            AppData.SaveGameData();
            // ゲーム再構成
            MG_Mediator.StateCtrl.SetStageChange();
        }
        public void OnClick_ReOpen()
        {
            // メニューと追加を非表示
            GetGoVal(GoNo.Grp_Menu).SetActive(true);
            GetGoVal(GoNo.Grp_Force).SetActive(true);
        }

        // セーブデータの初期化
        public void OnClick_DeleteData()
        {
            // 初期化
            AppData.Dbg_SaveDataReset();
            if (SaveDataManager.IsExist())
            {
                SaveDataManager.Delete();
                SaveDataManager.Load();
                AppCommon.LoadGameData();
            }
            // 保存
            AppCommon.Update_And_SaveGameData();

            // 戻る
            m_ParentCtrl.OnClick_Return();

            MG_Mediator.StateCtrl.SetStageChange();
        }

        //================================================
        // [///] inputfield関連
        //================================================
    }
}