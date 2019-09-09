using UnityEngine;

namespace FIVE.SceneSystem
{
    internal class AreaData
    {
        public static int col = 127;
        public static int size = col + 1;

        private Vector3 Position;
        private Vector3[] GroundVertices;

        private int[] GroundTriangles;

        public AreaData(Vector3 pos)
        {
            Position = pos;
            GenerateGroundVertices();
            GenerateGroundTriangles();
        }

        public void ConstructArea(GameObject o)
        {
            var mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = GroundVertices;
            mesh.triangles = GroundTriangles;
            mesh.Optimize();
            mesh.RecalculateNormals();
            o.GetComponent<MeshFilter>().mesh = mesh;
            o.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        // Ground vertices generation

        private void GenerateGroundVertices()
        {
            GroundVertices = new Vector3[size * size];

            var seed = (int)Position.x ^ (int)Position.y ^ (int)Position.z;
            Random.InitState(seed);

            var s = size / (float)col;
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    GroundVertices[x * size + y] = new Vector3(x * s, 0f, y * s);
                }
            }

            var samplesize = size / 4;
            var scale = 3f;

            for (var y = 0; y < size; y += samplesize)
            {
                for (var x = 0; x < size; x += samplesize)
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

            // FIXME
            {
                for (var x = 0; x < size; x++)
                {
                    GroundVertices[x * size].y = 0.0f;
                    GroundVertices[x * size + col].y = 0.0f;
                }
                for (var y = 0; y < size; y++)
                {
                    GroundVertices[y].y = 0.0f;
                    GroundVertices[col * size + y].y = 0.0f;
                }
            }
        }

        private void DiamondSquare(int stepsize, float scale)
        {
            var halfstep = stepsize / 2;
            for (var y = halfstep; y < size + halfstep; y += stepsize)
            {
                for (var x = halfstep; x < size + halfstep; x += stepsize)
                {
                    SampleSquare(x, y, stepsize, Frand() * scale);
                }
            }
            for (var y = 0; y < size; y += stepsize)
            {
                for (var x = 0; x < size; x += stepsize)
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

        private ref float Sample(int x, int y) => ref GroundVertices[(x & (size - 1)) * size + (y & (size - 1))].y;

        private static float Frand() => Random.Range(-1f, 1f);

        // Ground triangles generation

        private void GenerateGroundTriangles()
        {
            GroundTriangles = new int[col * col * 2 * 3];

            for (int i = 0, c = 0; i < size * size - size; i++)
            {
                if (i % size == 0)
                {
                    continue;
                }

                if (i % 2 == 0)
                {
                    GroundTriangles[c + 0] = i;
                    GroundTriangles[c + 1] = i + size;
                    GroundTriangles[c + 2] = i - 1 + size;
                    GroundTriangles[c + 3] = i - 1 + size;
                    GroundTriangles[c + 4] = i - 1;
                    GroundTriangles[c + 5] = i;
                }
                else
                {
                    GroundTriangles[c + 0] = i + size;
                    GroundTriangles[c + 1] = i - 1 + size;
                    GroundTriangles[c + 2] = i - 1;
                    GroundTriangles[c + 3] = i - 1;
                    GroundTriangles[c + 4] = i;
                    GroundTriangles[c + 5] = i + size;
                }
                c += 6;
            }
        }
    }
}