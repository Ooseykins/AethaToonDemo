using System;
using UnityEngine;
using UnityEditor;

public static class AethaToonSnapshot
{
    private static string OutputPath => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)+"\\AethaToon Snapshots\\";
    private static string _mostRecentFile;
    private static Vector2Int _mostRecentSize;
    private const string Hotkey = " %g";
    private static bool AutoOpenRecentFile => EditorPrefs.GetBool("AethaToonAutoOpen", false);

    [MenuItem("AethaToon Snapshot/Open screenshot folder", priority = 0)]
    static void OpenFolder()
    {
        System.IO.Directory.CreateDirectory(OutputPath);
        Application.OpenURL(OutputPath);
    }
    
    [MenuItem("AethaToon Snapshot/Open most recent screenshot", priority = 1)]
    static void OpenRecent()
    {
        if (!string.IsNullOrEmpty(_mostRecentFile))
        {
            Application.OpenURL(_mostRecentFile);
        }
    }
    
    [MenuItem("AethaToon Snapshot/Automatically open new screenshots", priority = 2)]
    static void ToggleAutoOpen()
    {
        EditorPrefs.SetBool("AethaToonAutoOpen", !AutoOpenRecentFile);
        Menu.SetChecked("AethaToon Snapshot/Automatically open new screenshots", AutoOpenRecentFile);
    }
    
    [MenuItem("AethaToon Snapshot/Automatically open new screenshots", true)]
    static bool ValidateToggleAutoOpen()
    {
        Menu.SetChecked("AethaToon Snapshot/Automatically open new screenshots", AutoOpenRecentFile);
        return true;
    }

    [MenuItem("AethaToon Snapshot/Repeat most recent"+Hotkey, priority = 20)]
    static void SnapshotRepeat()
    {
        Snapshot(_mostRecentSize);
    }
    
    [MenuItem("AethaToon Snapshot/Screen size", priority = 21)]
    static void SnapshotScreen()
    {
        Snapshot(new Vector2Int(Screen.width,Screen.height));
    }
    
    [MenuItem("AethaToon Snapshot/Repeat most recent"+Hotkey, true)]
    static bool ValidateSnapshotRepeat()
    {
        return _mostRecentSize.x != 0 && _mostRecentSize.y != 0;
    }
    
    [MenuItem("AethaToon Snapshot/512x512 Square", priority = 50)]
    static void Snapshot512X512()
    {
        Snapshot(new Vector2Int(512,512));
    }
    
    [MenuItem("AethaToon Snapshot/1024x1024 Square", priority = 51)]
    static void Snapshot1024X1024()
    {
        Snapshot(new Vector2Int(1024,1024));
    }
    
    [MenuItem("AethaToon Snapshot/2096x2096 Square", priority = 52)]
    static void Snapshot2096X2096()
    {
        Snapshot(new Vector2Int(1024,1024));
    }
    
    [MenuItem("AethaToon Snapshot/1280x720 16:9 Landscape", priority = 100)]
    static void Snapshot1280X720()
    {
        Snapshot(new Vector2Int(1280,720));
    }
    
    [MenuItem("AethaToon Snapshot/1920x1080 16:9 Landscape", priority = 101)]
    static void Snapshot1920X1080()
    {
        Snapshot(new Vector2Int(1920,1080));
    }
    
    [MenuItem("AethaToon Snapshot/3840x2160 16:9 4k Landscape", priority = 102)]
    static void Snapshot3840X2160()
    {
        Snapshot(new Vector2Int(3840,2160));
    }
    
    [MenuItem("AethaToon Snapshot/720x1280 9:16 Portrait", priority = 150)]
    static void Snapshot720X1280()
    {
        Snapshot(new Vector2Int(720, 1280));
    }
    
    [MenuItem("AethaToon Snapshot/1080x1920 9:16 Portrait", priority = 150)]
    static void Snapshot1080X1920()
    {
        Snapshot(new Vector2Int(1080, 1920));
    }
    
    [MenuItem("AethaToon Snapshot/2160x3840 9:16 4k Portrait", priority = 150)]
    static void Snapshot2160X3840()
    {
        Snapshot(new Vector2Int(2160, 3840));
    }
    
    static void Snapshot(Vector2Int resolution)
    {
        if (!Camera.main)
        {
            return;
        }
        var camRt = Camera.main.targetTexture;
        var rt = RenderTexture.GetTemporary(resolution.x,resolution.y);
        Camera.main.targetTexture = rt;
        Camera.main.Render();
        Texture2D frame = new Texture2D(rt.width, rt.height);
        RenderTexture.active = rt;
        frame.ReadPixels(new Rect(0,0, rt.width, rt.height), 0, 0);
        frame.Apply();
        RenderTexture.active = null;
        if (camRt != rt)
        {
            Camera.main.targetTexture = camRt;
        }
        
        string path = OutputPath + DateTime.Now.ToString("s").Replace(":", "-") + ".png";
        _mostRecentFile = path;
        System.IO.Directory.CreateDirectory(OutputPath);
        System.IO.File.WriteAllBytes(path, frame.EncodeToPNG());
        if (AutoOpenRecentFile)
        {
            OpenRecent();
        }
        _mostRecentSize = resolution;
    }
}
