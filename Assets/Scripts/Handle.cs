using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour {

    public List<int> indices;


    /*
	// Use this for initialization
	void Start () {
        indices = null;

    }
    */

    public void AddIndices(List<int> indices)
    {
        this.indices = indices;
    }

	
    public List<int> GetIndices()
    {
        return this.indices;
    }


	// Update is called once per frame
	void Update () {
		
	}
}
