using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class DMIIQuads : MonoBehaviour
{
    [SerializeField, HideInInspector] ComputeShader computeShader;
    [SerializeField] int width = 256;
    [SerializeField] int height = 256;

    [Header("Can edit at Runtime")]
    [SerializeField] float noiseScale = 0.1f;
    [SerializeField] float timeScale = 20f;
    [SerializeField] Vector2 noiseOffset = Vector2.zero;
    [SerializeField] Vector2 noiseDirecton = Vector2.one;

    Mesh quadMesh;
    Material material;
    ComputeBuffer renderData;
    ComputeBuffer args;
    uint[] tempArgs;
    Bounds bounds;
    
    Queue<float> avgFPS = new Queue<float>();

    void Start()
    {
        quadMesh = Quad(Vector2.one);
        material = new Material(Shader.Find("DMIIShader"));
        material.enableInstancing = true;

        bounds = new Bounds(new Vector3(width, 0f, height) / 2f, new Vector3(width, 100f, height));

        // Initialize GPU buffers
        int bufferSizeBytes = sizeof(float) * 4;
        renderData = new ComputeBuffer(width * height, bufferSizeBytes);

        tempArgs = new uint[5] { quadMesh.GetIndexCount(0), (uint)(width * height), 0, 0, 0 };
        args = new ComputeBuffer(5, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        args.SetData(tempArgs);

        // Set GPU memory
        material.SetBuffer("_RenderData", renderData);

        computeShader.SetVector("_PositionOffset", new Vector3(width / 2f, 0f, height / 2f));
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

        Graphics.DrawMeshInstancedIndirect(quadMesh, 0, material, bounds, args, 0, null, ShadowCastingMode.Off, false, 0, Camera.main);

        avgFPS.Enqueue(1f / Time.deltaTime);
        if (avgFPS.Count > 60) avgFPS.Dequeue();
    }

    public static Mesh Quad(Vector2 size)
    {
        Vector3 halfSize = size / 2f;

        Vector3 c1 = new Vector3(-halfSize.x, 0f, -halfSize.y);
        Vector3 c2 = new Vector3(halfSize.x, 0f, -halfSize.y);
        Vector3 c3 = new Vector3(halfSize.x, 0f, halfSize.y);
        Vector3 c4 = new Vector3(-halfSize.x, 0f, halfSize.y);

        Vector3[] vertices =
            new[]
            {
                    c1, c2, c3, c4
            };

        int[] indices =
            new[] {
                    0, 1, 2, 0, 2, 3
            };

        return CreateMesh(vertices, indices);
    }

    static Mesh CreateMesh(Vector3[] vertices, int[] indices)
    {
        var mesh = new Mesh();

        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();

        return mesh;
    }

    void OnGUI() 
    {
        Rect rect = new Rect(Vector2.zero, new Vector2(150, 50));
        GUI.Label(rect, $"FPS: {avgFPS.Average():F2}", GUI.skin.box);
    }
}