﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MarchingCubes : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] float resolution = 0.5f;
    [SerializeField] float noiseScale = 0.2f;

    [SerializeField] private float heightTresshold = 0.5f;

    [SerializeField] bool visualizeNoise;
    [SerializeField] bool use3DNoise;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private float[,,] heights;

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        StartCoroutine(TestAll());
    }

    void Update()
    {
        
    }

    public void DigTerrain(Vector3 position)
    {
        int digRadius = 1; // 파괴 반경

        for (int x = -digRadius; x <= digRadius; x++)
        {
            for (int y = -digRadius; y <= digRadius; y++)
            {
                for (int z = -digRadius; z <= digRadius; z++)
                {
                    Vector3Int gridPos = new Vector3Int(
                        Mathf.RoundToInt(position.x / resolution) + x,
                        Mathf.RoundToInt(position.y / resolution) + y,
                        Mathf.RoundToInt(position.z / resolution) + z
                    );


                    // 배열 범위 체크
                    if (gridPos.x >= 0 && gridPos.x < width &&
                        gridPos.y >= 0 && gridPos.y < height &&
                        gridPos.z >= 0 && gridPos.z < width)
                    {
                        heights[gridPos.x, gridPos.y, gridPos.z] = 1.0f; // 땅을 제거하여 빈 공간으로 설정
                    }
                }
            }
        }

        MarchCubes();
        SetMesh();
    }

    private IEnumerator TestAll()
    {
        /*while (true)
        {
            SetHeights();
            MarchCubes();
            SetMesh();
            yield return new WaitForSeconds(1f);
        }*/

        SetHeights();
        MarchCubes();
        SetMesh(); yield return new WaitForSeconds(1f);

    }

    

    private void SetMesh()
    {
        Mesh mesh = new Mesh();


        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        // MeshCollider 업데이트
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }

    private void SetHeights()
    {
        heights = new float[width + 1, height + 1, width + 1];

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = height - 10; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {

                    float currentHeight = height * Mathf.PerlinNoise(x * noiseScale, z * noiseScale);
                    float distToSufrace;

                    if (y <= currentHeight - 0.5f)
                        distToSufrace = 0f;
                    else if (y > currentHeight + 0.5f)
                        distToSufrace = 1f;
                    else if (y > currentHeight)
                        distToSufrace = y - currentHeight;
                    else
                        distToSufrace = currentHeight - y;

                    heights[x, y, z] = distToSufrace;
                    
                }
            }
        }

    }

    private float PerlinNoise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);

        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }

    private int GetConfigIndex(float[] cubeCorners)
    {
        int configIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > heightTresshold)
            {
                configIndex |= 1 << i;
            }
        }

        return configIndex;
    }

    private void MarchCubes()
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    float[] cubeCorners = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), cubeCorners);
                }
            }
        }
    }

    private void MarchCube(Vector3 position, float[] cubeCorners)
    {
        int configIndex = GetConfigIndex(cubeCorners);

        if (configIndex == 0 || configIndex == 255)
        {
            return;
        }

        int edgeIndex = 0;
        for (int t = 0; t < 5; t++)
        {
            for (int v = 0; v < 3; v++)
            {
                int triTableValue = MarchingTable.Triangles[configIndex, edgeIndex];

                if (triTableValue == -1)
                {
                    return;
                }

                Vector3 edgeStart = position + MarchingTable.Edges[triTableValue, 0];
                Vector3 edgeEnd = position + MarchingTable.Edges[triTableValue, 1];

                Vector3 vertex = ((edgeStart + edgeEnd) / 2) * resolution;

                vertices.Add(vertex);
                triangles.Add(vertices.Count - 1);

                edgeIndex++;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizeNoise || !Application.isPlaying)
        {
            return;
        }

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z], 1);
                    Gizmos.DrawSphere(new Vector3(x * resolution, y * resolution, z * resolution), 0.2f * resolution);
                }
            }
        }
    }
}