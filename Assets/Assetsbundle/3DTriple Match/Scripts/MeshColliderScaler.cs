using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class MeshColliderScaler : MonoBehaviour
{
    float expansionFactor = 0.092f; // 设置扩大的比例系数

    void Start()
    {
        MeshCollider addMesh = gameObject.AddComponent<MeshCollider>();

        // addMesh.isTrigger = true;
        Mesh original = addMesh.sharedMesh;
        Mesh newMesh = new Mesh();

        newMesh.vertices = original.vertices;
        newMesh.uv = original.uv;
        newMesh.normals = original.normals;
        newMesh.tangents = original.tangents;
        newMesh.triangles = original.triangles;
        newMesh.bounds = original.bounds;
        newMesh.name = original.name;

        Vector3[] vertices = newMesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += newMesh.normals[i] * expansionFactor; // 将每个顶点向法线方向扩展
        }

        newMesh.vertices = vertices;
        newMesh.RecalculateBounds(); // 重新计算边界盒
        addMesh.sharedMesh = newMesh;
        addMesh.convex = true;
        addMesh.isTrigger = true;
    }
}