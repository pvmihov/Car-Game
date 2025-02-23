using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoadColor : MonoBehaviour {
    public Material roadMaterial; // Material for the road

    public void GenerateRoadMesh(List<Vector3> leftEdge, List<Vector3> rightEdge) {
        // Ensure both edges have the same number of points
        while (leftEdge.Count < rightEdge.Count) {
            leftEdge.Add(leftEdge[leftEdge.Count - 1]); // Add the last point of the left edge
        }

        while (rightEdge.Count < leftEdge.Count) {
            rightEdge.Add(rightEdge[rightEdge.Count - 1]); // Add the last point of the right edge
        }

        // Check if the edges are valid
        if (leftEdge.Count < 2 || rightEdge.Count < 2) {
            Debug.LogError("Both edges must have at least two points.");
            return;
        }

        // Initialize arrays for mesh data
        int vertexCount = leftEdge.Count * 2;
        int triangleCount = (leftEdge.Count - 1) * 6;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[triangleCount];

        // Populate vertices
        for (int i = 0; i < leftEdge.Count; i++) {
            vertices[i * 2] = new Vector3(leftEdge[i].x,leftEdge[i].y,0);   // Add point from the left edge
            vertices[i * 2 + 1] = new Vector3(rightEdge[i].x,rightEdge[i].y,0); // Add point from the right edge
        }

        // Populate triangles
        int triangleIndex = 0;
        for (int i = 0; i < leftEdge.Count - 1; i++) {
            int v0 = i * 2;       // Current left point
            int v1 = i * 2 + 1;   // Current right point
            int v2 = (i + 1) * 2; // Next left point
            int v3 = (i + 1) * 2 + 1; // Next right point

            // First triangle
            triangles[triangleIndex++] = v0;
            triangles[triangleIndex++] = v1;
            triangles[triangleIndex++] = v2;

            // Second triangle
            triangles[triangleIndex++] = v1;
            triangles[triangleIndex++] = v3;
            triangles[triangleIndex++] = v2;
        }

        // Create the mesh
        Mesh roadMesh = new Mesh {
            vertices = vertices,
            triangles = triangles
        };
        roadMesh.RecalculateNormals();

        // Assign the mesh to the MeshFilter
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = roadMesh;

        // Assign the material
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = roadMaterial;
        meshRenderer.sortingLayerName = "Background";
        meshRenderer.sortingOrder = -1; // Lower values are rendered first
    }
}
