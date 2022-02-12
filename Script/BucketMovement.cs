using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BucketMovement : MonoBehaviour
{
    public Vector4 Y = new Vector4(0.0f,0.5f,1.5f,0.0f);
   
    public float dt = 0.02f;
    public float t = 0.0f;

    public double  l = 10;
    public double  μ = 0.05;
    public double  Mass = 3;
    public const double g = 9.81;
    public const float π =(float) Math.PI;


    public Vector4 G(Vector4 d , float t){
        float θd = d[0],  φd = d[1],θ = d[2], φ = d[3];
      
        double θdd = Math.Pow(φd,2) * Math.Cos(θ)* Math.Sin(θ) -( g / l) * Math.Sin(θ) - (μ/Mass) * θd;
        double φdd = (-2.0 * θd * φd) / Math.Tan(θ) - (μ/Mass) * φd;
        return new Vector4((float)θdd ,(float) φdd ,(float) θd ,(float) φd );
    }

    public Vector3 update(double θ , double φ){
        Vector3 posision =new Vector3(0,0,0);
        posision[0] =(float)( l * Math.Sin(θ) * Math.Cos(φ));
        posision[1] =(float) ((-1*l * Math.Cos(θ))/10 +6 );
        posision[2] =(float) (-1*l * Math.Sin(θ) * Math.Sin(φ));

        return posision;
    }

    public Vector4 RK4(float t , float dt , Vector4 y){
        Vector4 k1 = G(y,t);
	    Vector4 k2 = G(y+(float)0.5*k1*(float)dt, t+(float)0.5*(float)dt);
	    Vector4 k3 = G(y+(float)0.5*k2*(float)dt, t+(float)0.5*(float)dt);
	    Vector4 k4 = G(y+k3*(float)dt, t+dt);


	    return (float)dt * (k1 + (float)2*k2 + (float)2*k3 + k4) /(float)6;
    }

    public float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n){
    // angle in [0,180]
    float angle = Vector3.Angle(a,b);
    float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));

    // angle in [-179,180]
    float signed_angle = angle * sign;

    // angle in [0,360] (not used but included here for completeness)
    float angle360 =  (signed_angle + 180) % 360;

    return angle360;
    }


    // Update is called once per frame
    private Vector3 result= new Vector3();
    private Quaternion res;
    void Update()
    {
        transform.position = update(Y[2],Y[3]);
        t += dt;
        if(transform.position.z == 0){
            result.x = 0;
        }
        
        else
        {
            result.x = (float)-(-(10 - transform.position.z) * 9f +90f );
            
        }
        result.y = 0f;
        if (transform.position.x == 0)
        {
            result.z = 0;
        }
        else
        { 
            result.z = (float) -((10 - transform.position.x) * 9f +90f);
            result.z-=180f;
        }

        res.eulerAngles = result;
        transform.rotation=res;
        Y = Y + RK4(t, dt , Y);        
    }
}
