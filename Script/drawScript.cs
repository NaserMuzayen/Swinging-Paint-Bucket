using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawScript : MonoBehaviour
{
    
    private LineRenderer lr;
    public Transform bucket;
    public SPHManagerSingleThread s;
    private List<Vector3> points = new List<Vector3>();
    private void Awake(){
        lr = GetComponent<LineRenderer>();
    }
    
    // Update is called once per frame
    void Update()
    {   
        //draw path
        // points.Add(new Vector3(bucket.position.x,0.25f,bucket.position.z));
        Vector3 l = s.getPoint();
        if(!points.Contains(l) ){
            points.Add(l);

        }   
        lr.positionCount = points.Count;
        transform.position =points[0];
        for (int i = 1 ; i< points.Count; i++){
            lr.SetPosition(i,points[i]);
        }
        
    }
}
