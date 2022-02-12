using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeMovement : MonoBehaviour
{
    // public Transform  bucket;
    private LineRenderer lr;
    public Transform Hand;

    private void Awake(){
        lr = GetComponent<LineRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0,new Vector3(0,16,0));
        lr.SetPosition(1,Hand.position);

    }
}
