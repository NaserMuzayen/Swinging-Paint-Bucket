using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManger : MonoBehaviour
{
    public BucketMovement bucket;
    public SPHManagerSingleThread SPH;
    public void UserInput()
    {

    }
    public Slider Mass;
    public Slider l;
    public Slider μ;
    public Slider θd;
    public Slider φd;
    public Slider θ;
    public Slider φ;
   
    public void Start()
    {
        Mass.onValueChanged.AddListener(delegate { MassValueChangeCheck(); });
        l.onValueChanged.AddListener(delegate { lValueChangeCheck(); });
        μ.onValueChanged.AddListener(delegate { μValueChangeCheck(); });
        θd.onValueChanged.AddListener(delegate { θdValueChangeCheck(); });
        φd.onValueChanged.AddListener(delegate { φdValueChangeCheck(); });
        θ.onValueChanged.AddListener(delegate { θValueChangeCheck(); });
        φ.onValueChanged.AddListener(delegate { φValueChangeCheck(); });
       
    }
    public void MassValueChangeCheck()
    {
        bucket.Mass =  Mass.value;
    }
    public void lValueChangeCheck()
    {
        bucket.l =  l.value;
    }
    public void μValueChangeCheck()
    {
        bucket.μ =  μ.value;
    }
    public void θdValueChangeCheck()
    {
        bucket.Y[0] =  θd.value;
    }
    public void φdValueChangeCheck()
    {
        bucket.Y[1] =  φd.value;
    }
    public void θValueChangeCheck()
    {
        bucket.Y[2] =  θ.value;
    }
    public void φValueChangeCheck()
    {
        bucket.Y[3]=  φ.value;
    }
    
}
