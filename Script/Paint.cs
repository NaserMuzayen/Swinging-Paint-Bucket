using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : MonoBehaviour
{
    // Start is called before the first frame update
    private LineRenderer lr;
    public Transform bucket;

    private void Awake(){
        lr = GetComponent<LineRenderer>();
    }
    

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0,bucket.position);
        lr.SetPosition(1,new Vector3(bucket.position.x,0.25f,bucket.position.z));
    }
}
