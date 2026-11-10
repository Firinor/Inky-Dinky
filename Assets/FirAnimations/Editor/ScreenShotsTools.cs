using UnityEngine;
using UnityEditor;
using System.IO;

public class ScreenShotsTools : EditorWindow
{
    private int screenshotWidth = 1080;
    private int screenshotHeight = 1920;
    private int captureCount = 1;
    private string screenshotName = "Screenshot";
    private string savePath = "Screenshots";
    private bool useTransparentBackground = false;
    private bool captureGameView = true;

    [MenuItem("Tools/FirUtility/Screenshot Tool")]
    public static void ShowWindow()
    {
        GetWindow<ScreenShotsTools>("Screenshot Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Screenshot Settings", EditorStyles.boldLabel);

        screenshotWidth = EditorGUILayout.IntField("Width", screenshotWidth);
        screenshotHeight = EditorGUILayout.IntField("Height", screenshotHeight);
        
        captureCount = EditorGUILayout.IntField("Capture Count", captureCount);
        if (captureCount < 1) captureCount = 1;

        screenshotName = EditorGUILayout.TextField("File Name", screenshotName);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        useTransparentBackground = EditorGUILayout.Toggle("Transparent Background", useTransparentBackground);
        captureGameView = EditorGUILayout.Toggle("Capture Game View", captureGameView);

        EditorGUILayout.Space();

        if (GUILayout.Button("Capture Screenshot", GUILayout.Height(40)))
        {
            CaptureScreenshot();
        }

        if (GUILayout.Button("Capture Multiple Screenshots", GUILayout.Height(40)))
        {
            CaptureMultipleScreenshots();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Open Screenshots Folder", GUILayout.Height(30)))
        {
            OpenScreenshotsFolder();
        }

        EditorGUILayout.HelpBox("Screenshots will be saved in: " + GetFullPath(), MessageType.Info);
    }

    private void CaptureScreenshot()
    {
        if (!Directory.Exists(GetFullPath()))
        {
            Directory.CreateDirectory(GetFullPath());
        }

        string fileName = GetFileName();
        string fullPath = Path.Combine(GetFullPath(), fileName);

        if (captureGameView)
        {
            CaptureGameView(fullPath);
        }
        else
        {
            CaptureSceneView(fullPath);
        }

        Debug.Log($"Screenshot saved: {fullPath}");
        AssetDatabase.Refresh();
    }

    private void CaptureMultipleScreenshots()
    {
        for (int i = 0; i < captureCount; i++)
        {
            string fileName = $"{screenshotName}_{i + 1:000}.png";
            string fullPath = Path.Combine(GetFullPath(), fileName);

            if (captureGameView)
            {
                CaptureGameView(fullPath);
            }
            else
            {
                CaptureSceneView(fullPath);
            }
            
            System.Threading.Thread.Sleep(100);
        }

        Debug.Log($"Captured {captureCount} screenshots");
        AssetDatabase.Refresh();
    }

    private void CaptureGameView(string path)
    {
        Vector2 gameViewSize = GetGameViewSize();
        
        RenderTexture renderTexture = new RenderTexture(screenshotWidth, screenshotHeight, 24);
        Texture2D texture = new Texture2D(screenshotWidth, screenshotHeight, TextureFormat.ARGB32, false);
        
        RenderTexture currentRT = RenderTexture.active;
        
        RenderTexture.active = renderTexture;
        
        Camera camera = Camera.main;
        if (camera != null)
        {
            RenderTexture originalRT = camera.targetTexture;
            camera.targetTexture = renderTexture;
            camera.Render();
            camera.targetTexture = originalRT;

            texture.ReadPixels(new Rect(0, 0, screenshotWidth, screenshotHeight), 0, 0);
            texture.Apply();
        }
        else
        {
            Debug.LogError("No camera found in scene!");
            return;
        }
        
        RenderTexture.active = currentRT;
        renderTexture.Release();
        
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        
        DestroyImmediate(texture);
    }

    private void CaptureSceneView(string path)
    {
        RenderTexture renderTexture = new RenderTexture(screenshotWidth, screenshotHeight, 24);
        Texture2D texture = new Texture2D(screenshotWidth, screenshotHeight, TextureFormat.ARGB32, false);
        
        Camera captureCamera = CreateCaptureCamera();

        if (captureCamera != null)
        {
            captureCamera.targetTexture = renderTexture;
            captureCamera.Render();
            captureCamera.targetTexture = null;
            
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, screenshotWidth, screenshotHeight), 0, 0);
            texture.Apply();
            
            if (captureCamera.gameObject.name.Contains("ScreenshotCamera"))
            {
                DestroyImmediate(captureCamera.gameObject);
            }
        }
        else
        {
            Debug.LogError("Failed to create capture camera!");
            return;
        }

        RenderTexture.active = null;
        renderTexture.Release();
        
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        DestroyImmediate(texture);
    }

    private Camera CreateCaptureCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            GameObject cameraGo = Instantiate(mainCamera.gameObject);
            cameraGo.name = "ScreenshotCamera_Temp";
            Camera newCamera = cameraGo.GetComponent<Camera>();
            
            newCamera.targetTexture = null;
            
            return newCamera;
        }
        
        GameObject go = new GameObject("ScreenshotCamera_Temp");
        Camera cam = go.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        
        return cam;
    }

    private Vector2 GetGameViewSize()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        if (GetSizeOfMainGameView != null)
        {
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }
        
        return new Vector2(screenshotWidth, screenshotHeight);
    }

    private string GetFileName()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return $"{screenshotName}_{timestamp}.png";
    }

    private string GetFullPath()
    {
        return Path.Combine(Application.dataPath, savePath);
    }

    private void OpenScreenshotsFolder()
    {
        string path = GetFullPath();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        System.Diagnostics.Process.Start("explorer.exe", path);
    }
}