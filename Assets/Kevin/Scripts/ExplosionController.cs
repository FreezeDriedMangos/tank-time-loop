using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public const double sizeTolerance = 0.001;

    public float expandSpeed;
    public float targetSize;
    public float duration_seconds;

    private bool setup = false;

    public TankController.Team firedBy;

    public bool isBiggerExplosion;

    private Dictionary<TankController, float> destructionTimers = new Dictionary<TankController, float>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Setup()
    {
        if(setup) return;
        setup = true;

        SetupConfig();

        StartCoroutine(ShrinkAfterTimer());
    }

    void SetupConfig()
    {
        if (GameState.config == null || GameState.config.miscConfig == null) return;
            
        duration_seconds = GameState.config.miscConfig.explosionDuration_seconds;
        expandSpeed = GameState.config.miscConfig.explosionExpandSpeed;
        targetSize = GameState.config.miscConfig.explosionExpandTargetSize;

        if (!isBiggerExplosion) return;
        
        duration_seconds = GameState.config.miscConfig.bigger_explosionDuration_seconds;
        expandSpeed = GameState.config.miscConfig.bigger_explosionExpandSpeed;
        targetSize = GameState.config.miscConfig.bigger_explosionExpandTargetSize;
    }

    IEnumerator ShrinkAfterTimer()
    {
        yield return new WaitForSeconds(duration_seconds);
        //GameObject.Destroy(this.gameObject);
        targetSize = -1;
    }

    void FixedUpdate()
    {
        Setup();

        float currentSize = this.transform.localScale.x;

        if (currentSize <= 0)
            GameObject.Destroy(this.gameObject);

        float size = Mathf.Lerp(currentSize, targetSize, expandSpeed);
        this.transform.localScale = new Vector3(size, size, size);

        List<TankController> allKeys = new List<TankController>(destructionTimers.Keys);
        foreach (TankController Key in allKeys)
        {
            destructionTimers[Key] -= Time.deltaTime;
            if (destructionTimers[Key] <= 0) Key.SetDestroyed(true);
        }
    }

    public void OnTriggerEnter(Collider trigger)
    {
        CheckTankCollision(trigger, true);
        CheckShellCollision(trigger);
        CheckBombCollision(trigger);
    }

    public void OnTriggerExit(Collider trigger)
    {
        CheckTankCollision(trigger, false);
    }

    private void CheckTankCollision(Collider trigger, bool enter)
    {
        //if (trigger.transform.parent == null) return;

        TankController tank = trigger.transform.GetComponent<TankController>();
        
        if (tank == null) return;
        
        //tank.SetDestroyed(true);
        if (enter)
            destructionTimers[tank] = GameState.config.miscConfig.explosionTankDestructionTimer_seconds;
        else
            destructionTimers.Remove(tank);
    }
    
    private void CheckShellCollision(Collider trigger)
    {
        BulletController bullet = trigger.transform.GetComponent<BulletController>();

        if (bullet == null) return;

        bullet.DestroyShell();
    }
    
    private void CheckBombCollision(Collider trigger)
    {
        BombController bomb = trigger.transform.GetComponent<BombController>();

        if (bomb == null) return;

        bomb.Explode();
    }
}
