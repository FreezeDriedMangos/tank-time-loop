using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public const float BULLET_LENGTH = 0.25f;//0.2f;

    public static List<BulletController> allBullets = new List<BulletController>();

    public TankController.Team firedBy;

    public Rigidbody rb {get; private set;}
    private bool setup = false;

    [SerializeField] private float speed      = 10;
    [SerializeField] private int   numBounces = 2;

    [SerializeField] private AudioSource destroySound;
    [SerializeField] private AudioSource bounceSound;
    
    public string bulletType;

    private bool hasLeftTank = false;
    private int  numBouncesLeft;

    private bool bouncing = false;

    
    public void Setup()
    {
        if (setup) return;
        setup = true;
        
        // float travelDir = -transform.eulerAngles.y + 90;

        allBullets.Add(this);

        numBouncesLeft = numBounces;
        rb = GetComponent<Rigidbody>();
        SetVelocity();
    }

    void OnDestroy()
    {
        allBullets.Remove(this);
    }

    private void SetVelocity()
    {
        rb.velocity = this.transform.TransformDirection(Vector3.forward * speed);
    }

    public void OnCollisionEnter(Collision col)
    {
        if (bouncing) return;
        bouncing = true;

        // col.normal
        // change this.transform.eulerAngles.y
        // change velocity respectively

        //rb.velocity = new Vector3();
        CheckShellCollision(col.gameObject);

        SetVelocity();
        Vector3 newVec = Vector3.Reflect(rb.velocity, col.contacts[0].normal);

        float newDir = Mathf.Atan2(newVec.x, newVec.z)*Mathf.Rad2Deg;

        this.transform.eulerAngles = new Vector3(0, newDir, 0);
        this.transform.position += newVec.normalized * BULLET_LENGTH; // the shell model's origin is at its tip, so if we just rotate it when it hits a wall, the whole back half of it will be inside the wall. We need to move it away from the wall a bit manually
        this.SetVelocity();

        numBouncesLeft--;
        if (numBouncesLeft <= 0)
        {
            DestroyShell();
            return;
        } 

        SFXConfig c1 = GameState.ConfigUtility.GetConfigForSFX(bulletType+"_bounceSound");
        bounceSound.volume = PlayerPrefs.GetFloat("SFXVolume") * c1.volume;
        bounceSound.pitch = Random.Range(c1.pitchMin, c1.pitchMax);
        bounceSound.Play();

        bouncing = false;
    }
    
    public void OnCollisionExit(Collision col)
    {}

    private void CheckShellCollision(GameObject o)
    {
        BulletController b = o.GetComponent<BulletController>();
        
        if (b == null) return;
    
        b.DestroyShell();
        this.DestroyShell();
    }
    
    public void OnTriggerEnter(Collider trigger)
    {
        if (!hasLeftTank) {hasLeftTank = true; return; }
        
        CheckTankCollision(trigger);
    }

    private void CheckTankCollision(Collider trigger)
    {
        if (trigger.transform.parent == null) return;

        TankController tank = trigger.transform.parent.GetComponent<TankController>();
        
        if (tank == null) return;
        
        tank.SetDestroyed(true);
        this.DestroyShell();
    }

    public void OnTriggerExit(Collider trigger)
    {
        hasLeftTank = true;
    }
    
    public void DestroyShell()
    {

        SFXConfig c1 = GameState.ConfigUtility.GetConfigForSFX(bulletType+"_destroySound");
        destroySound.gameObject.AddComponent<DestroyOnClipFinish>();
        destroySound.pitch = Random.Range(c1.pitchMin, c1.pitchMax);
        destroySound.volume = PlayerPrefs.GetFloat("SFXVolume") * c1.volume;
        destroySound.Play();
        destroySound.transform.parent = null;



        // play sound effects
        // start smoke puff effect
        GameObject.Destroy(this.gameObject);
    }
}
