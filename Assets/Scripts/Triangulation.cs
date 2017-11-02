using UnityEngine;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

public class Triangulation
{
    public static bool triangulate(List<Vector2> points, out List<int> outIndices, out List<Vector3> outVertices)
    {
        outVertices = new List<Vector3>();
        outIndices = new List<int>();
        Polygon poly = new Polygon();


        // Points and segments
        for (int i = 0; i < points.Count; i++)
        {

            poly.Add(new Vertex(points[i].x, points[i].y));

 
            if(i == points.Count - 1)
            { // add connection from first to last point
                poly.Add(new Segment(new Vertex(points[i].x, points[i].y), new Vertex(points[0].x, points[0].y)));
            }
            else
            { // connect to next
                poly.Add(new Segment(new Vertex(points[i].x, points[i].y), new Vertex(points[i + 1].x, points[i + 1].y)));
            }

        }



    //    TriangleNet.Meshing.ConstraintOptions options =
  //  new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = true };

        var options = new ConstraintOptions() { ConformingDelaunay = true };

        var mesh = poly.Triangulate(options);

        foreach(ITriangle t in mesh.Triangles)
        {
            for(int j = 2; j>=0; j--)
            {
                bool found = false;
                for (int k = 0; k < outVertices.Count; k++)
                {
                    if ((outVertices[k].x == t.GetVertex(j).X) && (outVertices[k].z == t.GetVertex(j).Y))
                    {
                        outIndices.Add(k);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    outVertices.Add(new Vector3((float)t.GetVertex(j).X,  (float)t.GetVertex(j).Y , 0));
                    outIndices.Add(outVertices.Count - 1);
                }
            }

        }


        return true;

    }
    
}