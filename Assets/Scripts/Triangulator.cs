using System.Collections.Generic;
using UnityEngine;

public class Triangulator : MonoBehaviour
{


    void Start()
    {
        
        GameObject gameObject = new GameObject();
        gameObject.name = "Surf";
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        // MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        Material mat = Resources.Load("gridMaterial") as Material;
        


        Mesh mesh = meshFilter.mesh;


        List<Vector2> points = new List<Vector2>();
        List<int> indices = null;
        List<Vector3> vertices = null;

    
        //GameObject cubeA = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cubeA.transform.position = new Vector3(0, 0, 0);
        
      /*
        points.Add(new Vector2(0, 0));
        points.Add(new Vector2(0, 0.5f));
        points.Add(new Vector2(0, 1));

        points.Add(new Vector2(0.5f, 1));
        points.Add(new Vector2(0.5f, 0.5f));
        points.Add(new Vector2(0.5f, 0));

        

        points.Add(new Vector2(1, 0));
        points.Add(new Vector2(1, 0.5f));
        points.Add(new Vector2(1, 1));
        */

        
        points.Add(new Vector2(0, 0)); 
        points.Add(new Vector2(0,50));
        points.Add(new Vector2(50,50));
        points.Add(new Vector2(50,100));
        points.Add(new Vector2(0,100));
        points.Add(new Vector2(0,150));
        points.Add(new Vector2(150,150));
        points.Add(new Vector2(150,100));
        points.Add(new Vector2(100,100));
        points.Add(new Vector2(100,50));
        points.Add(new Vector2(150,50));
        points.Add(new Vector2(150,0));
        
      

        /*
        points.Add(new Vector2(50, 0));
        points.Add(new Vector2(100, 0));
        points.Add(new Vector2(50, 150));
        points.Add(new Vector2(100, 150));
        */


        Triangulation.triangulate(points, out indices, out vertices);
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
     

        //    gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

        Vector2[] uvs = new Vector2[mesh.vertices.Length];
        for(int i = 0; i < uvs.Length ; i++)
        {
            uvs[i] = new Vector2(mesh.vertices[i].x , mesh.vertices[i].y );
        }
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        //gameObject.GetComponent<MeshRenderer>().material = mat;
            meshRenderer.material = mat;
        //meshRenderer.material.mainTexture  = Resources.Load("grids") as Texture;
    }
}