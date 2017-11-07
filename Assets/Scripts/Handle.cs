using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour {

    public List<int> indices;
    public int id;


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
	void Update () {
		
	}
}
