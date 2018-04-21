using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour
{

    public List<int> indices;
    public int id;
    private float x;
    private float y;
    private float z;

    public float X
    {
        get
        {
            return x;
        }

        set
        {
            x = value;
        }
    }

    public float Y
    {
        get
        {
            return y;
        }

        set
        {
            y = value;
        }
    }

    public float Z
    {
        get
        {
            return z;
        }

        set
        {
            z = value;
        }
    }


    /*
    // Use this for initialization
    void Start () {
        indices = null;

    }
    */

    public void InitHandle(List<int> indices, int id)
    {
        this.indices = indices;
        this.id = id;
        this.x = 0;
        this.y = 0;
        this.z = 0;
    }


    public List<int> GetIndices()
    {
        return this.indices;
    }

    public int GetId()
    {
        return this.id;
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = new Vector3(x, y, z);
    }
}
