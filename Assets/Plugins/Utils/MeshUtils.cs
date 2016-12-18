using UnityEngine;

namespace Utils {
  public static class MeshUtils {
    public static Mesh CloneMesh(Mesh mesh) {
      Mesh clone = new Mesh();
      clone.vertices = mesh.vertices;
      clone.normals = mesh.normals;
      clone.tangents = mesh.tangents;
      clone.subMeshCount = mesh.subMeshCount;
      clone.uv = mesh.uv;
      clone.uv2 = mesh.uv2;

      for (int i = 0; i < mesh.subMeshCount; i++) {
        clone.SetTriangles(mesh.GetTriangles(i), i);
      }

      clone.bindposes = mesh.bindposes;
      clone.boneWeights = mesh.boneWeights;
      clone.bounds = mesh.bounds;
      clone.colors = mesh.colors;
      clone.name = mesh.name;
      return clone;
    }

    /// <summary>
    ///   Convert a mesh to a double-sided version. Faces will be visible from the inside.
    /// </summary>
    /// <remarks>
    ///   Based on http://answers.unity3d.com/questions/280741/how-make-visible-the-back-face-of-a-mesh.html
    /// </remarks>
    public static void ConvertToDoubleSided(Mesh mesh) {
      Vector3[] vertices = mesh.vertices;
      Vector2[] uv = mesh.uv;
      Vector3[] normals = mesh.normals;

      int verticesLength = vertices.Length;

      Vector3[] newVertices = new Vector3[verticesLength*2];
      Vector2[] newUv = new Vector2[verticesLength*2];
      Vector3[] newNormals = new Vector3[verticesLength*2];

      for (int i = 0; i < verticesLength; i += 1) {
        // Duplicate vertices and UVs.
        newVertices[i] = newVertices[i + verticesLength] = vertices[i];
        newUv[i] = newUv[i + verticesLength] = uv[i];

        // Duplicate original normals...
        newNormals[i] = normals[i];
        // ...and reverse the new ones.
        newNormals[i + verticesLength] = -normals[i];
      }

      int[] triangles = mesh.triangles;

      int trianglesLength = triangles.Length;

      int[] newTriangles = new int[trianglesLength*2];

      for (int i = 0; i < trianglesLength; i += 3) {
        // Duplicate the original triangle.
        newTriangles[i] = triangles[i];
        newTriangles[i + 1] = triangles[i + 1];
        newTriangles[i + 2] = triangles[i + 2];

        int j = i + trianglesLength;

        // Reverse the triangle.
        newTriangles[j] = triangles[i] + verticesLength;
        newTriangles[j + 2] = triangles[i + 1] + verticesLength;
        newTriangles[j + 1] = triangles[i + 2] + verticesLength;
      }

      mesh.vertices = newVertices;
      mesh.uv = newUv;
      mesh.normals = newNormals;
      mesh.triangles = newTriangles;
    }

    public static void ConvertToDoubleSidedWithColor(Mesh mesh, Color color) {
      Vector3[] vertices = mesh.vertices;
      Vector2[] uv = mesh.uv;
      Vector3[] normals = mesh.normals;
      Color[] colors = mesh.colors;

      int verticesLength = vertices.Length;

      Vector3[] newVertices = new Vector3[verticesLength*2];
      Vector2[] newUv = new Vector2[verticesLength*2];
      Vector3[] newNormals = new Vector3[verticesLength*2];
      Color[] newColors = new Color[verticesLength*2];

      for (int i = 0; i < verticesLength; i += 1) {
        // Duplicate vertices and UVs.
        newVertices[i] = newVertices[i + verticesLength] = vertices[i];
        newUv[i] = newUv[i + verticesLength] = uv[i];
        newColors[i] = colors[i];
        newColors[i + verticesLength] = color;

        // Duplicate original normals...
        newNormals[i] = normals[i];

        // ...and reverse the new ones.
        newNormals[i + verticesLength] = -normals[i];
      }

      int[] triangles = mesh.triangles;

      int trianglesLength = triangles.Length;

      int[] newTriangles = new int[trianglesLength*2];

      for (int i = 0; i < trianglesLength; i += 3) {
        // Duplicate the original triangle.
        newTriangles[i] = triangles[i];
        newTriangles[i + 1] = triangles[i + 1];
        newTriangles[i + 2] = triangles[i + 2];

        int j = i + trianglesLength;

        // Reverse the triangle.
        newTriangles[j] = triangles[i] + verticesLength;
        newTriangles[j + 2] = triangles[i + 1] + verticesLength;
        newTriangles[j + 1] = triangles[i + 2] + verticesLength;
      }

      mesh.vertices = newVertices;
      mesh.uv = newUv;
      mesh.normals = newNormals;
      mesh.triangles = newTriangles;
      mesh.colors = newColors;
    }
  }
}