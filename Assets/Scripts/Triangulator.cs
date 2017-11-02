using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;

public class Triangulator : MonoBehaviour {

    // The delaunay mesh
    private TriangleNet.Mesh mesh = null;


    // Prefab which is generated for each chunk of the mesh.
    public Transform screenPrefab = null;




    // Use this for initialization
    void Start()
    {


        List<Vector2> points = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(0, 50),
            new Vector2(50, 50),
            new Vector2(50, 100),
            new Vector2(0, 100),
            new Vector2(0, 150),
            new Vector2(150, 150),
            new Vector2(150, 100),
            new Vector2(100, 100),
            new Vector2(100, 50),
            new Vector2(150, 50),
            new Vector2(150, 0)
        };
   

        // Vertex is TriangleNet.Geometry.Vertex
        Polygon polygon = new Polygon();
        foreach (Vector2 point in points)
        {
            polygon.Add(new Vertex(point.x, point.y));
        }



        // ConformingDelaunay is false by default; this leads to ugly long polygons at the edges
        // because the algorithm will try to keep the mesh convex
        TriangleNet.Meshing.ConstraintOptions options =
            new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = true };
        mesh = (TriangleNet.Mesh)polygon.Triangulate(options);




        MakeMesh();

    }


    public void MakeMesh()
    {
        // Instantiate an enumerator to go over the Triangle.Net triangles - they don't
        // provide any array-like interface for indexing

        IEnumerator<Triangle> triangleEnumerator = mesh.Triangles.GetEnumerator();


            // Vertices in the unity mesh
            List<Vector3> vertices = new List<Vector3>();

            // Per-vertex normals
            List<Vector3> normals = new List<Vector3>();

            // Per-vertex UVs - unused here, but Unity still wants them
            List<Vector2> uvs = new List<Vector2>();

            // Triangles - each triangle is made of three indices in the vertices array
            List<int> triangles = new List<int>();

  
            for (int i = 0; i < mesh.Triangles.Count; i++)
            {

            if (!triangleEnumerator.MoveNext())
                {
                    //  stop when last  
                    break;
                }

                // Get the current triangle
                Triangle triangle = triangleEnumerator.Current;

                // For the triangles to be right-side up, they need
                // to be wound in the opposite direction
                
                Vector3 v0 = new Vector3((float)triangle.GetVertex(2).X, (float)triangle.GetVertex(2).Y, 0);
                Vector3 v1 = new Vector3((float)triangle.GetVertex(1).X, (float)triangle.GetVertex(1).Y, 0);
                Vector3 v2 = new Vector3((float)triangle.GetVertex(0).X, (float)triangle.GetVertex(0).Y, 0);

            // This triangle is made of the next three vertices to be added
                triangles.Add(vertices.Count);
                triangles.Add(vertices.Count + 1);
                triangles.Add(vertices.Count + 2);

                // Add the vertices
                vertices.Add(v0);
                vertices.Add(v1);
                vertices.Add(v2);

                // Compute the normal - flat shaded, so the vertices all have the same normal
                Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0);
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);

                // If you want to texture your terrain, UVs are important,
                // but I just use a flat color so put in dummy coords
                uvs.Add(new Vector2(0.0f, 0.0f));
                uvs.Add(new Vector2(0.0f, 0.0f));
                uvs.Add(new Vector2(0.0f, 0.0f));
            }

            // Create the actual Unity mesh object
            Mesh screenMesh = new Mesh();
            screenMesh.vertices = vertices.ToArray();
            screenMesh.uv = uvs.ToArray();
            screenMesh.triangles = triangles.ToArray();
            screenMesh.normals = normals.ToArray();

            // Instantiate the GameObject which will display this chunk
            Transform screen = Instantiate<Transform>(screenPrefab, transform.position, transform.rotation);
            screen.GetComponent<MeshFilter>().mesh = screenMesh;
            screen.GetComponent<MeshCollider>().sharedMesh = screenMesh;
            screen.transform.parent = transform;
            
        
    }







}
