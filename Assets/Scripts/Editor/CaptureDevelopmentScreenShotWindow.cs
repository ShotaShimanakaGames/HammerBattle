using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CaptureDevelopmentScreenShotWindow : EditorWindow
{
    [UnityEditor.MenuItem("Edit/Capture Screenshot Window")]
    static void OpenWindow()
    {
        GetWindow<CaptureDevelopmentScreenShotWindow>(true, "ゲーム画面 キャプチャ");
    }

    string path = "~/Desktop";

    void OnGUI()
    {
        GUILayout.Label("保存先:");
        path = GUILayout.TextField(path);
        if (GUILayout.Button("Save ScreenShot"))
        {
            Capture();
        }
    }

    void Capture()
    {
        string[] screenres = UnityStats.screenRes.Split('x');
        int width = int.Parse(screenres[0]);
        int height = int.Parse(screenres[1]);

        var filePath = string.Format(Path.Combine(path, "ScreenShot_{0}_{1}-{2}.png"), width, height, DateTime.Now.ToString("HHmmss"));
        Debug.Log(string.Format("Save a screenshot at {0}", filePath));
        ScreenCapture.CaptureScreenshot(string.Format(filePath));
    }
} 