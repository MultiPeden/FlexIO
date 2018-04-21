using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;
using UnityEngine;

public class Triangulator : MonoBehaviour
{

    public bool debug;

    // The delaunay mesh
    private TriangleNet.Mesh triangulatorMesh = null;


    // Prefab which is generated for each chunk of the mesh.
    public Transform screenPrefab = null;

    public Transform handlePrefab = null;


    private Transform screen = null;

    private Mesh screenMesh = null;

    Dictionary<Vector3, List<int>> dictionary;

    Dictionary<int, Vector3> IDtoVectorDict;


    /// <summary>
    /// ///
    /// </summary>

    Vector3[] verts;
    Vector3 vertPos;
    GameObject[] handles;

    UDPScript udpScript;
    private float maxX, maxY, minY, minX;

    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("UDPScript");
        udpScript = (UDPScript)go.GetComponent(typeof(UDPScript));
        debug = false;

    }


    private void AddHandles(List<Vector2> points)
    {
        // add handles to the mesh
        verts = screenMesh.vertices;
        int i = 0;
        foreach (KeyValuePair<Vector3, List<int>> entry in dictionary)
        {
            // create handle for node
            Transform handle = Instantiate<Transform>(handlePrefab, transform.position, transform.rotation);
            // set handles position
            handle.transform.position = transform.TransformPoint(entry.Key);
            // add tag to the handle
            handle.tag = "handle";
            // add the handle as a child of the screenprefab
            handle.transform.parent = screen;
            // get the handle object
            Handle handleObj = handle.GetComponent<Handle>();
            // get vertex indices and add them to the handle


            handleObj.InitHandle(entry.Value, points.IndexOf(new Vector2(entry.Key.x, entry.Key.y)));
            i++;
        }

        Debug.Log(i);
    }




    private void Triangulate(List<Vector2> points)
    {

        // Vertex is TriangleNet.Geometry.Vertex
        Polygon polygon = new Polygon();

        maxX = -1000;
        maxY = -1000;
        minY = 1000;
        minX = 1000;
        float x, y;

        foreach (Vector2 point in points)
        {

            x = point.x;
            y = point.y;

            polygon.Add(new Vertex(x, y));

            if (x > maxX)
                maxX = x;
            if (y > maxY)
                maxY = y;
            if (x < minX)
                minX = x;
            if (y < minY)
                minY = y;



        }

        if (polygon.Count > 2)
        {

            // ConformingDelaunay is false by default; this leads to ugly long polygons at the edges
            // because the algorithm will try to keep the mesh convex
            //   TriangleNet.Meshing.ConstraintOptions options =
            //    new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = true };
            triangulatorMesh = (TriangleNet.Mesh)polygon.Triangulate();
        }
        else
        {

            Debug.Log(string.Format("Mesh needs minimum 3 points but did only receive: {0} point(s)", polygon.Count));
        }

        Debug.Log(points.Count);

    }


    private void Update()
    {
        if (debug)
        {
            DebugUpdate();
        }
        else
        {
            DefaultUpdate();
        }


    }

    private void DefaultUpdate()
    {

        if (screenMesh != null)
        {
            IRPoint[] irs = GetIrs();
            handles = GameObject.FindGameObjectsWithTag("handle");

            if (handles.Length == irs.Length)
            {

                for (int i = 0; i < handles.Length; i++)
                {

                    Handle handle = handles[i].GetComponent<Handle>();



                    IRPoint iRPoint = Array.Find(irs, element => element.id == handle.id);

                    handle.X = iRPoint.x;
                    handle.Y = iRPoint.y;
                    handle.Z = iRPoint.z;


                    List<int> indices = handle.GetIndices();


                    foreach (int index in indices)
                    {

                        verts[index] = new Vector3(iRPoint.x, iRPoint.y, iRPoint.z);
                    }
                }

                screenMesh.vertices = verts;
                screenMesh.RecalculateBounds();
                screenMesh.RecalculateNormals();
            }
            else
            {
                Debug.Log(string.Format("Lost points, had {0} points, now have {1}", handles.Length, irs.Length));
            }
        }

    }

    private void DebugUpdate()
    {
        if (screenMesh != null)
        {
            handles = GameObject.FindGameObjectsWithTag("handle");
            for (int i = 0; i < handles.Length; i++)
            {
                List<int> indices = handles[i].GetComponent<Handle>().GetIndices();
                foreach (int index in indices)
                {

                    verts[index] = handles[i].transform.localPosition;
                }
            }
            screenMesh.vertices = verts;
            screenMesh.RecalculateBounds();
            screenMesh.RecalculateNormals();
        }
    }



    public void MakeMesh()
    {

        dictionary = new Dictionary<Vector3, List<int>>();

        // Instantiate an enumerator to go over the Triangle.Net triangles - they don't
        // provide any array-like interface for indexing

        IEnumerator<Triangle> triangleEnumerator = triangulatorMesh.Triangles.GetEnumerator();


        // Vertices in the unity mesh
        List<Vector3> vertices = new List<Vector3>();

        // Per-vertex normals
        List<Vector3> normals = new List<Vector3>();

        // Per-vertex UVs - unused here, but Unity still wants them
        List<Vector2> uvs = new List<Vector2>();

        // Triangles - each triangle is made of three indices in the vertices array
        List<int> triangles = new List<int>();

        Debug.Log("triangles " + triangulatorMesh.Triangles.Count);
        for (int i = 0; i <= triangulatorMesh.Triangles.Count; i++)
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
            AppendInDict(v0, vertices.Count);
            vertices.Add(v0);
            AppendInDict(v1, vertices.Count);
            vertices.Add(v1);
            AppendInDict(v2, vertices.Count);
            vertices.Add(v2);

            // Compute the normal - flat shaded, so the vertices all have the same normal
            Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);

            // If you want to texture your terrain, UVs are important,
            // but I just use a flat color so put in dummy coords


            uvs.Add(new Vector2(Mathf.InverseLerp(minX, maxX, (float)triangle.GetVertex(2).X), Mathf.InverseLerp(minY, maxY, (float)triangle.GetVertex(2).Y)));
            uvs.Add(new Vector2(Mathf.InverseLerp(minX, maxX, (float)triangle.GetVertex(1).X), Mathf.InverseLerp(minY, maxY, (float)triangle.GetVertex(1).Y)));
            uvs.Add(new Vector2(Mathf.InverseLerp(minX, maxX, (float)triangle.GetVertex(0).X), Mathf.InverseLerp(minY, maxY, (float)triangle.GetVertex(0).Y)));
        }

        // Create the actual Unity mesh object
        screenMesh = new Mesh
        {
            vertices = vertices.ToArray(),
            uv = uvs.ToArray(),
            triangles = triangles.ToArray(),
            normals = normals.ToArray()
        };
        screenMesh.MarkDynamic();

        // Instantiate the GameObject which will display this chunk
        screen = Instantiate<Transform>(screenPrefab, transform.position, transform.rotation);
        screen.GetComponent<MeshFilter>().mesh = screenMesh;
        screen.GetComponent<MeshCollider>().sharedMesh = screenMesh;
        screen.transform.parent = transform;



    }


    private void AppendInDict(Vector3 key, int value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key].Add(value);
        }
        else
        {
            dictionary[key] = new List<int> { value };
        }
    }


    public void TaskOnClick()
    {

        if (handles != null)
        {
            foreach (GameObject handle in handles)
            {
                DestroyImmediate(handle);
            }
        }
        if (screen != null)
        {
            DestroyImmediate(screen.gameObject);
            screen = null;
        }
        if (screenMesh != null)
        {
            DestroyImmediate(screenMesh);
            screenMesh = null;
        }




        List<Vector2> points = GetPoints();

        if (points != null)
        {
            Triangulate(points);
            // create mesh from the triangulation
            MakeMesh();
            // add handles to the mesh
            AddHandles(points);
            // Debug.Log("You have clicked the button!");
        }
        else
        {
            Debug.Log("No points detected");
        }

    }

    private List<Vector2> GetPoints()
    {
        List<Vector2> points = new List<Vector2>();

        IRPoint[] irPoints = udpScript.GetIRs();
        if (irPoints != null && irPoints.Length > 0)
        {

            foreach (IRPoint irPoint in irPoints)
            {
                points.Add(new Vector2(irPoint.x, irPoint.y));
            }

            return points;
        }
        else
        {
            return null;
        }

    }


    private IRPoint[] GetIrs()
    {
        return udpScript.GetIRs();
    }


}
