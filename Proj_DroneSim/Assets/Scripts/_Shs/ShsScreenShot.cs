using System.Collections;
using UnityEngine;
using System.IO;
using System;

public class ShsScreenShot : MonoBehaviour
{
    //================================================
    // [///]
    //================================================
    // 階層かつフォルダ名
    const string ScreenShotPath = "Assets/ScreenShots";
    // ファイル名 : ScreenShot_ + 000 + .png (名前 + 百桁数値 + 拡張子)
    const string ScreenShotName = "ScreenShot_";
    // 使用キー
    const KeyCode SetKeyCode = KeyCode.Space;
    // ダイアログ表示時間
    const float DialogWaitTime = 1.0f;

    private int m_ScreenShotCount = 0;
    private bool m_IsTakeAScreenShotDialog = false;

    //================================================
    // [///]
    //================================================
    IEnumerator Start()
    {
        // フォルダがない場合は作成する
        if (!Directory.Exists(ScreenShotPath))
        {
            Directory.CreateDirectory(ScreenShotPath);
        }

        yield return StartCoroutine("UpdateProc");
    }

    void Update()
    {
    }

    //================================================
    // [///] 撮影表記ダイアログ
    //================================================
    private void OnGUI()
    {
        if (m_IsTakeAScreenShotDialog)
        {
            GUI.skin.box.fontSize = 96;
            GUI.skin.box.fontStyle = FontStyle.Bold;
            float w = 900;
            float h = 244;
            float x = Screen.width / 2 - w / 2;
            float y = Screen.height / 2 - h;
            GUI.Box(new Rect(x, y, w, h), "Take a ScreenShot\nNo." + m_ScreenShotCount.ToString().PadLeft(3, '0'));
        }
    }

    //================================================
    // [///]
    //================================================
    // 更新処理
    public IEnumerator UpdateProc()
    {
        while (true)
        {
            if (Input.GetKeyDown(SetKeyCode))
            {
                m_ScreenShotCount++;
                // ファイル名 : ScreenShot_ + 000 + 日付 + .png (名前 + 百桁数値 + 日付 + 拡張子)
                string fileName = ScreenShotPath + "/";
                // 名前 + 百桁数値
                fileName += ScreenShotName + m_ScreenShotCount.ToString().PadLeft(3, '0') + "_";
                // 日付 + 拡張子
                fileName += DateTime.Now.ToString("yyyyMMddhhmmss") + ".png";
                ScreenCapture.CaptureScreenshot(fileName);

                yield return null;

                m_IsTakeAScreenShotDialog = true;

                yield return new WaitForSeconds(DialogWaitTime);

                m_IsTakeAScreenShotDialog = false;
            }
            yield return null;
        }
    }
}
