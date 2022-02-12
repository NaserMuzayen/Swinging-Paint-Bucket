using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SPHManagerSingleThread : MonoBehaviour
{
    public Transform bucket;
    private Vector3 point = new Vector3();
    private struct SPHParticle
    {
        public Vector3 position;

        public Vector3 velocity;
        public Vector3 forcePhysic;
        public Vector3 forceHeading;

        public float density;
        public float pressure;

        public int parameterID;

        public GameObject go;
        


        public void Init(Vector3 _position, int _parameterID, GameObject _go)
        {
            position = _position;
            parameterID = _parameterID;
            go = _go;

            velocity = Vector3.zero;
            forcePhysic = Vector3.zero;
            forceHeading = Vector3.zero;
            density = 0.0f;
            pressure = 0.0f;
        }
    }



    [System.Serializable]
    private struct SPHParameters
    {
        #pragma warning disable 0649 
        public float particleRadius;
        public float smoothingRadius;
        public float smoothingRadiusSq;
        public float restDensity;
        public float gravityMult;
        public float particleMass;
        public float particleViscosity;
        public float particleDrag;
        #pragma warning restore 0649
    }



    private struct SPHCollider
    {
        public Vector3 position;
        public Vector3 right;
        public Vector3 up;
        public Vector2 scale;

        public void Init(Transform _transform)
        {
            position = _transform.position;
            right = _transform.right;
            up = _transform.up;
            scale = new Vector2(_transform.lossyScale.x / 2f, _transform.lossyScale.y / 2f);
        }
    }



    // Consts
    private static Vector3 GRAVITY = new Vector3(0.0f, -9.81f, 0.0f);
    private const float GAS_CONST = 2000.0f;
    private const float DT = 0.0008f;
    private const float BOUND_DAMPING = -0.5f;

    // Properties
    [Header("Import")]
    [SerializeField] private GameObject character0Prefab = null;

    [Header("Parameters")]
    [SerializeField] private int parameterID = 0;
    [SerializeField] private SPHParameters[] parameters = null;

    [Header("Properties")]
    [SerializeField] private int amount = 250;
    [SerializeField] private int rowSize = 16;

    // Data
    private List<SPHParticle> particles = new List<SPHParticle>();

    public Vector3 getPoint()
    {
        return point;
    }



    private void Update()
    {
        InitSPH();
        ComputeDensityPressure();
        ComputeForces();
        Integrate();
        ComputeColliders();

        ApplyPosition();
        Delete();
    }



 private void InitSPH() 
    { 
         
        for (int i = 0; i < amount; i++) 
        { 
            float jitter = (Random.value * 2f - 1f) * parameters[parameterID].particleRadius * 0.1f; 
            float x =  bucket.position.x;
            float y =  bucket.position.y; 
            float z = bucket.position.z;
 
            GameObject go = Instantiate(character0Prefab); 
            go.transform.localScale = Vector3.one * parameters[parameterID].particleRadius; 
            go.transform.position = new Vector3(x + jitter, y, z + jitter); 

            SPHParticle p = new SPHParticle(); 
            p.Init(new Vector3(x, y, z), parameterID, go);
            particles.Add(p); 
        } 
    }

    private static bool Intersect(SPHCollider collider, Vector3 position, float radius, out Vector3 penetrationNormal, out Vector3 penetrationPosition, out float penetrationLength)
    {
        Vector3 colliderProjection = collider.position - position;

        penetrationNormal = Vector3.Cross(collider.right, collider.up);
        penetrationLength = Mathf.Abs(Vector3.Dot(colliderProjection, penetrationNormal)) - (radius / 2.0f);
        penetrationPosition = collider.position - colliderProjection;

        return penetrationLength < 0.0f
            && Mathf.Abs(Vector3.Dot(colliderProjection, collider.right)) < collider.scale.x
            && Mathf.Abs(Vector3.Dot(colliderProjection, collider.up)) < collider.scale.y;
    }



    private static Vector3 DampVelocity(SPHCollider collider, Vector3 velocity, Vector3 penetrationNormal, float drag)
    {
        Vector3 newVelocity = Vector3.Dot(velocity, penetrationNormal) * penetrationNormal * BOUND_DAMPING
                            + Vector3.Dot(velocity, collider.right) * collider.right * drag
                            + Vector3.Dot(velocity, collider.up) * collider.up * drag;
        newVelocity = Vector3.Dot(newVelocity, Vector3.forward) * Vector3.forward
                    + Vector3.Dot(newVelocity, Vector3.right) * Vector3.right
                    + Vector3.Dot(newVelocity, Vector3.up) * Vector3.up;
        return newVelocity;
    }



    private void ComputeColliders()
    {
        // Get colliders
        GameObject[] collidersGO = GameObject.FindGameObjectsWithTag("SPHCollider");
        SPHCollider[] colliders = new SPHCollider[collidersGO.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].Init(collidersGO[i].transform);
        }

        for (int i = 0; i < particles.Count; i++)
        {
            SPHParticle p = particles[i];
            for (int j = 0; j < colliders.Length; j++)
            {
                // Check collision
                Vector3 penetrationNormal;
                Vector3 penetrationPosition;
                float penetrationLength;
                if (Intersect(colliders[j], p.position, parameters[p.parameterID].particleRadius, out penetrationNormal, out penetrationPosition, out penetrationLength))
                {
                    p.velocity = DampVelocity(colliders[j], p.velocity, penetrationNormal, 1.0f - parameters[p.parameterID].particleDrag);
                    p.position = penetrationPosition - penetrationNormal * Mathf.Abs(penetrationLength);
                }
            }
            particles[i]= p;
        }
    }



    private void Integrate()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            SPHParticle p =particles[i];
            p.velocity += DT * (p.forcePhysic) / p.density;
            p.position += DT * (p.velocity );
            particles[i] = p;
        }
    }



    private void ComputeDensityPressure()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            SPHParticle p =particles[i];
            p.density = 0.0f;

            for (int j = 0; j < particles.Count; j++)
            {
                 SPHParticle pj =particles[j];
                Vector3 rij = pj.position - p.position;
                float r2 = rij.sqrMagnitude;

                if (r2 < parameters[p.parameterID].smoothingRadiusSq)
                {
                    p.density += parameters[p.parameterID].particleMass * (315.0f / (64.0f * Mathf.PI * Mathf.Pow(parameters[p.parameterID].smoothingRadius, 9.0f))) * Mathf.Pow(parameters[p.parameterID].smoothingRadiusSq - r2, 3.0f);
                }
                particles[j] = pj;
            }

            p.pressure = GAS_CONST * (p.density - parameters[p.parameterID].restDensity);
            particles[i] = p;
        }
    }



    private void ComputeForces()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            SPHParticle p =particles[i];
            Vector3 forcePressure = Vector3.zero;
            Vector3 forceViscosity = Vector3.zero;

            // Physics
            for (int j = 0; j < particles.Count; j++)
            {
                SPHParticle pj =particles[j];
                if (i == j) continue;

                Vector3 rij = pj.position - p.position;
                float r2 = rij.sqrMagnitude;
                float r = Mathf.Sqrt(r2);

                if (r < parameters[p.parameterID].smoothingRadius)
                {
                    forcePressure += -rij.normalized * parameters[p.parameterID].particleMass * (p.pressure + pj.pressure) / (2.0f * pj.density) * (-45.0f / (Mathf.PI * Mathf.Pow(parameters[p.parameterID].smoothingRadius, 6.0f))) * Mathf.Pow(parameters[p.parameterID].smoothingRadius - r, 2.0f);

                    forceViscosity += parameters[p.parameterID].particleViscosity * parameters[p.parameterID].particleMass * (pj.velocity - p.velocity) / pj.density * (45.0f / (Mathf.PI * Mathf.Pow(parameters[p.parameterID].smoothingRadius, 6.0f))) * (parameters[p.parameterID].smoothingRadius - r);
                }
                particles[j] = pj;
            }

            Vector3 forceGravity = GRAVITY * p.density * parameters[p.parameterID].gravityMult;

            // Apply
            p.forcePhysic = forcePressure + forceViscosity + forceGravity;
            particles[i] = p;
        }
    }



    private void ApplyPosition()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            SPHParticle p =particles[i];
            if(p.position.y<=0.25) p.position.y =0.25f;
            p.go.transform.position = p.position;
            particles[i] = p;
        }
    }
    private int  num =0;
    private void Delete(){
        if(particles.Count == 50){
            particles.Remove(particles[0]);
        }
    }
    public void de(float value){
            GRAVITY = new Vector3(0.0f, -value, 0.0f);
    }
}