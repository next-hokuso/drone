using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;

// 継承専用
// 型Tは参照型
// 引数無しのコンストラクターが呼べるように new() 制約を付ける
abstract public class YnsSaveLoad<T> where T : YnsSaveLoad<T>, new()
{
    private static T instance;

    //実行中に保存されるファイルがあるパス
#if UNITY_WEBGL
    public static string nameKey = typeof(T).Name;
#else
    public static string folderPath = Application.persistentDataPath + "/Database/";
    public static string filePath = folderPath + typeof(T).Name + ".json";
#endif
    public static T Instance
    {
        get
        {
            if (null == instance)
            {
                Load();
            }
            return instance;
        }
    }

    public static void Save()
    {
        Instance._Save();
    }

    public void _Save()
    {
        // クラスのプロパティをJSON形式のデータとして作成
        string json = JsonUtility.ToJson(Instance);

#if UNITY_WEBGL
        PlayerPrefs.SetString(nameKey, json);
        PlayerPrefs.Save();
        Debug.Log("セーブデータファイルを保存しました：" + nameKey);
#else
        //json += "[END]"; // 復号化の際にPaddingされたデータを除去するためのデリミタの追記
        //                 //      Debug.Log (json);
        //string crypted = Crypt.Encrypt(json);
        
        //Debug.Log ("セーブデータ保存先フォルダ：" + folderPath);
        // フォルダーがない場合は作成する
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        BinaryWriter writer = new BinaryWriter(fileStream);
        //writer.Write(crypted);
        writer.Write(json);
        writer.Close();
        Debug.Log("セーブデータファイルを保存しました：" + filePath);
#endif
    }

    public static void Load()
    {
        //T ret = null;
        //Debug.Log(typeof(T).Name + " Load Start");
        //string json = "";

#if UNITY_WEBGL
        if (PlayerPrefs.HasKey(nameKey))
        {
            Debug.Log("セーブデータファイルは存在します：" + nameKey);

            string json = PlayerPrefs.GetString(nameKey);

            try
            {
                //　JSONデータをクラスのプロパティに設定
                instance = JsonUtility.FromJson<T>(json);
            }
            catch (ArgumentException)
            {
                Debug.Log("セーブデータファイルが不正です：" + nameKey);
                instance = new T();
            }

        }
#else
        if (File.Exists(filePath))
        {
            Debug.Log ("セーブデータファイルは存在します：" + filePath);
        
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(fileStream);
            if (reader != null)
            {
                string str = reader.ReadString();
                //string decrypted = Crypt.Decrypt(str);
        
                //decrypted = System.Text.RegularExpressions.Regex.Replace(decrypted, @"¥[END¥].*$", "");
        
                //json = decrypted;
                //instance = JsonMapper.ToObject<T>(json);
        
                try
                {
                    //　JSONデータをクラスのプロパティに設定
                    instance = JsonUtility.FromJson<T>(str);
                }
                catch (ArgumentException)
                {
                    Debug.Log("セーブデータファイルが不正です：" + filePath);
                    instance = new T();
                }
        
                reader.Close();
            }
        }
#endif
        else
        {
            Debug.Log("セーブデータファイルが存在しません");
            instance = new T();
        }
    }

#if UNITY_WEBGL
#else
    private void WriteFile(string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(JsonUtility.ToJson(Instance));
            }
        }
    }
#endif

    public static bool IsExist()
    {
#if UNITY_WEBGL
        return PlayerPrefs.HasKey(nameKey);
#else
        return File.Exists(filePath);
#endif
    }

    public static void Delete()
    {
#if UNITY_WEBGL
        if (PlayerPrefs.HasKey(nameKey))
#else
        if (File.Exists(filePath))
#endif
        {
            Debug.Log("セーブデータファイルを削除します");
#if UNITY_WEBGL
            PlayerPrefs.DeleteKey(nameKey);
            Debug.Log(nameKey + "を消去しました");
#else
            File.Delete(filePath);
            if (!File.Exists(filePath))
            {
                Debug.Log(filePath + "を消去しました");
            }
#endif
        }
        else
        {
            Debug.Log("セーブデータファイルが存在しません");
        }
    }


}