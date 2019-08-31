using UnityEngine;
using Random = UnityEngine.Random;

public class GroundController : MonoBehaviour
{
    private const int size = 127;
    private const int col = size + 1;

    private Vector3[] vertices;
    private int[] triangles;

    // Start is called before the first frame update
    private void Start()
    {
        vertices = new Vector3[col * col];
        triangles = new int[size * size * 2 * 3];

        Random.InitState(0);
        GenerateVertices();
        GenerateTriangles();

        var mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void GenerateVertices()
    {
        for (var i = 0; i < col * col; i++)
        {
            vertices[i] = new Vector3(i / col, 0f, i % col);
        }

        var samplesize = col / 4;
        var scale = 3f;

        for (var y = 0; y < col; y += samplesize)
        {
            for (var x = 0; x < col; x += samplesize)
            {
                Sample(x, y) = Frand();
            }
        }

        while (samplesize > 1)
        {
            DiamondSquare(samplesize, scale);

            samplesize /= 2;
            scale /= 2f;
        }
    }

    private void DiamondSquare(int stepsize, float scale)
    {
        var halfstep = stepsize / 2;
        for (var y = halfstep; y < col + halfstep; y += stepsize)
        {
            for (var x = halfstep; x < col + halfstep; x += stepsize)
            {
                SampleSquare(x, y, stepsize, Frand() * scale);
            }
        }
        for (var y = 0; y < col; y += stepsize)
        {
            for (var x = 0; x < col; x += stepsize)
            {
                SampleDiamond(x + halfstep, y, stepsize, Frand() * scale);
                SampleDiamond(x, y + halfstep, stepsize, Frand() * scale);
            }
        }
    }

    private void SampleSquare(int x, int y, int size, float value)
    {
        var hs = size / 2;
        var a = Sample(x - hs, y - hs);
        var b = Sample(x + hs, y - hs);
        var c = Sample(x - hs, y + hs);
        var d = Sample(x + hs, y + hs);

        Sample(x, y) = ((a + b + c + d) / 4f) + value;
    }

    private void SampleDiamond(int x, int y, int size, float value)
    {
        var hs = size / 2;
        var a = Sample(x - hs, y);
        var b = Sample(x + hs, y);
        var c = Sample(x, y - hs);
        var d = Sample(x, y + hs);

        Sample(x, y) = ((a + b + c + d) / 4f) + value;
    }

    private ref float Sample(int x, int y) => ref vertices[(x & (col - 1)) * col + (y & (col - 1))].y;

    private float Frand() => UnityEngine.Random.Range(-1f, 1f);

    private void GenerateTriangles()
    {
        for (int i = 0, c = 0; i < col * col - col; i++)
        {
            if (i % col == 0)
            {
                continue;
            }

            if (i % 2 == 0)
            {
                triangles[c + 0] = i;
                triangles[c + 1] = i + col;
                triangles[c + 2] = i - 1 + col;
                triangles[c + 3] = i - 1 + col;
                triangles[c + 4] = i - 1;
                triangles[c + 5] = i;
            }
            else
            {
                triangles[c + 0] = i + col;
                triangles[c + 1] = i - 1 + col;
                triangles[c + 2] = i - 1;
                triangles[c + 3] = i - 1;
                triangles[c + 4] = i;
                triangles[c + 5] = i + col;
            }
            c += 6;
        }
    }
}