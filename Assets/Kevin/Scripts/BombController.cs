using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
	private Rigidbody rb;
	private bool setup = false;
	private float scale = .5f;
	private bool hasLeftTank = false;
    public GameObject explosion;
    public GameObject explosionMarker;

    public float launchAngle;
    public float launchPower;
    public float direction;

    public TankController.Team firedBy;

    [SerializeField] private AudioSource explodeSound;

    public delegate void ExplosionNotifyDelegate(GameObject explosion);
    public ExplosionNotifyDelegate OnExplodeNotify;


    public void Setup()
    {
    	if (setup) return;
        	setup = true;

        rb = GetComponent<Rigidbody>();
    	//LaunchBomb();

        try
        {
            launchPower = GameState.config.miscConfig.bombLaunchPower;
            launchAngle = GameState.config.miscConfig.bombLaunchElevationAngle;
        }
        catch {}

        Vector3 vel;
        SphericalToCartesian(launchPower, Mathf.Deg2Rad * direction, Mathf.Deg2Rad * launchAngle, out vel);
        rb.velocity = vel;
    }

    // function by Morten Nobel-Jørgensen at https://blog.nobel-joergensen.com/2010/10/22/spherical-coordinates-in-unity/
    // note: we know how spherical coordinates work, we just didn't want to break out our
    // old textbooks to write this code
    // note: in the original code, x and z were switched. Not sure why, but that's backwards. We fixed it.
    public static void SphericalToCartesian(float radius, 
                                            float polar, 
                                            float elevation, 
                                            out Vector3 outCart){
        float a = radius * Mathf.Cos(elevation);
        outCart.z = a * Mathf.Cos(polar);
        outCart.y = radius * Mathf.Sin(elevation);
        outCart.x = a * Mathf.Sin(polar);
    }

    void LaunchBomb()
    {
    	Vector3 Vo = CalculateVelocity(this.transform.TransformDirection(Vector3.forward).normalized * 3, transform.position, 1f);
    	rb.velocity = Vo;
    }

    Vector3 CalculateVelocity(Vector3 target, Vector3 start, float time)
    {
    	// Calculate displacement between when the bomb spawns and where it ends
    	Vector3 distance = target - start;
    	Vector3 distanceXZ = distance;
    	distanceXZ.y = 0f;

    	float Sy = distance.y;
    	float Sxz = distanceXZ.magnitude;

    	float Vxz = Sxz / time;
    	float Vy = Sy / time + 0.5f + Mathf.Abs(Physics.gravity.y) * time;

    	Vector3 result = distanceXZ.normalized;
    	result *= Vxz;
    	result.y = scale * Vy;

    	return result;
    }

    // public void OnCollisionEnter(Collision col)
    // {      
    //    	Debug.Log("Collision Enterrrrr");

    //     if (!hasLeftTank) return;
                
    //    	GameObject g = GameObject.Instantiate(explosion);
	// 	g.transform.position = this.transform.position;

    //     //CheckShellCollision(col.gameObject);

    //     //DestroyShell();
    //     return;
    // }

    private void CheckShellCollision(GameObject o)
    {
        BombController b = o.GetComponent<BombController>();
        
        if (b == null) return;
    
        b.DestroyShell();
        this.DestroyShell();
    }

    private void CheckTankCollision(Collider trigger)
    {
        if (trigger.transform.parent == null) return;

        TankController tank = trigger.transform.parent.GetComponent<TankController>();
        
        if (tank == null) return;
        
        tank.SetDestroyed(true);
        this.DestroyShell();
    }

    public void OnTriggerEnter(Collider trigger)
    {
        if (!hasLeftTank) return;
                
        Explode();
    }

    public void Explode()
    {
        if (GameState.config.miscConfig.bombExplosionMarkers) 
        {
            GameObject f = Instantiate(explosionMarker);
            f.transform.position = this.transform.position;
            f.transform.parent = TankController.TracksMaster.transform;
        }

       	GameObject g = GameObject.Instantiate(explosion);
		g.transform.position = this.transform.position;
        g.GetComponent<ExplosionController>().firedBy = firedBy;

        if (OnExplodeNotify != null) OnExplodeNotify(g);

        this.DestroyShell();
    }

    public void OnTriggerExit(Collider trigger)
    {
        hasLeftTank = true;
    }

    public void OnCollisionExit(Collision col)
    {
        hasLeftTank = true;
    }

    public void DestroyShell()
    {
        if (explodeSound != null)
        {
            SFXConfig c1 = GameState.ConfigUtility.GetConfigForSFX("bomb_explodeSound");
            explodeSound.gameObject.AddComponent<DestroyOnClipFinish>();
            explodeSound.pitch = Random.Range(c1.pitchMin, c1.pitchMax);
            explodeSound.volume = PlayerPrefs.GetFloat("SFXVolume") * c1.volume;
            explodeSound.Play();
            explodeSound.transform.parent = null;
        }

        GameObject.Destroy(this.gameObject);
    }
}
