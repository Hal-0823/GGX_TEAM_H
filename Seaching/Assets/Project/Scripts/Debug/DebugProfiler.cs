using UnityEngine;

public class DebugProfiler : MonoBehaviour
{
    [SerializeField] private DebugPanel debugPanel;

    private int frameCount = 0;
    private float prevTime = 0f;
    private float fps = 0f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (debugPanel == null)
        {
            Debug.LogError("DebugPanel reference is not set in DebugProfiler.");
        }
    }

    private void Update()
    {
        frameCount++;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.3f) {
            fps = frameCount / time;
            //Debug.Log(fps);

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }

        string debugInfo = $"FPS: {fps:F0}";
        string memoryInfo = $"Memory: {System.GC.GetTotalMemory(false) / (1024 * 1024):F2} MB";
        debugPanel.UpdateDebugText(debugInfo, memoryInfo);
    }
}