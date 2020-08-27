using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralQuads : MonoBehaviour
{
    [SerializeField, HideInInspector] ComputeShader computeShader;
    [SerializeField] int width = 256;
    [SerializeField] int height = 256;
    [SerializeField] bool drawInGameView = true;
#if UNITY_EDITOR
    [SerializeField] bool drawInSceneView = true;
#endif

    [Header("Can edit at Runtime")]
    [SerializeField] float noiseScale = 0.1f;
    [SerializeField] float timeScale = 20f;
    [SerializeField] Vector2 noiseOffset = Vector2.zero;
    [SerializeField] Vector2 noiseDirecton = Vector2.one;

    Material material;
    ComputeBuffer renderData;
    ComputeBuffer args;
    uint[] tempArgs;
    Bounds bounds;

    Queue<float> avgFPS = new Queue<float>();

    void Start()
    {
        material = new Material(Shader.Find("ProceduralQuads"));
        material.enableInstancing = true;

        // This is the bounds you need to be inside to see the rendering
        bounds = new Bounds(new Vector3(width, 0f, height) / 2f, new Vector3(width, 100f, height));

        // sizeof(float) * 7 is the size of the RenderData struct in the compute/shader
        renderData = new ComputeBuffer(width * height, sizeof(float) * 7);

        // Tells the render pipeline about how many verts and instances to draw
        tempArgs = new uint[5] { (uint)(width * height), 1, 0, 0, 0 };
        args = new ComputeBuffer(5, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        args.SetData(tempArgs);

        // Set GPU memory
        material.SetBuffer("_RenderData", renderData);

        // Initialize GPU memory
        computeShader.SetInt("_Size", width);
        computeShader.SetBuffer(0, "_RenderData", renderData);
        computeShader.SetBuffer(1, "_RenderData", renderData);
        computeShader.Dispatch(0, width, height, 1);

        Application.targetFrameRate = 240;
        QualitySettings.vSyncCount = 0;
    }

    void OnDestroy()
    {
        args.Dispose();
        renderData.Dispose();
    }
    void Update()
    {
        // Alter GPU memory with compute shader
        computeShader.SetFloat("_NoiseScale", noiseScale);
        computeShader.SetFloat("_TimeScale", timeScale);
        computeShader.SetVector("_NoiseOffset", noiseOffset);
        computeShader.SetVector("_NoiseDirection", noiseDirecton.normalized);
        computeShader.SetFloat("_Time", Time.time);
        computeShader.Dispatch(1, width / 32, height / 32, 1);

        // Draw directly with data on GPU aka lockless rendering
        if (drawInGameView)
        {
            Graphics.DrawProceduralIndirect(
                material, bounds, MeshTopology.Points, args, 0, Camera.main
            );
        }
#if UNITY_EDITOR
        if (drawInSceneView)
        {
            Graphics.DrawProceduralIndirect(
                material, bounds, MeshTopology.Points, args, 0, UnityEditor.SceneView.lastActiveSceneView.camera
            );
        }
#endif

        avgFPS.Enqueue(1f / Time.deltaTime);
        if (avgFPS.Count > 60) avgFPS.Dequeue();
    }

    void OnGUI() 
    {
        Rect rect = new Rect(Vector2.zero, new Vector2(150, 25));
        GUI.Label(rect, $"CPU: {avgFPS.Average():F2} FPS", GUI.skin.box);    
    }
}
