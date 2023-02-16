using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TankController))]
public class SimplePlayerController : MonoBehaviour
{
    TankController tc;
    public CursorController cc = null;
    public float secondsHeldToFireRocket = 1;

    private bool canFire = false;
    private bool canFireBomb = false;
    private float rocketFireTimer = 0;

    public GameObject canvasPrefab; // because I don't know how to create UI stuff otherwise
    public GameObject playerCursorPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    void Setup()
    {
        tc = GetComponent<TankController>();

        if (cc == null)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            Transform c = null;
            if (canvases.Length <= 0)
                c = GameObject.Instantiate(canvasPrefab).transform;
            else
                c = canvases[0].transform;

            cc = GameObject.Instantiate(playerCursorPrefab).GetComponent<CursorController>();
            cc.transform.SetParent(c, false);    
            cc.tank = tc.GetComponent<TankTransform>();
            cc.Setup();
        }
    }

    // Update is called once per frame
    void Update()
    {
        tc.MoveInDirection(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        tc.AimAt(cc.GetAimLocation());

        if (Input.GetAxisRaw("Fire1") > 0.5)
        {
            if (!canFire)
            {
                canFire = true;
                rocketFireTimer = secondsHeldToFireRocket;
            }
            else if (rocketFireTimer > 0)
            {
                rocketFireTimer -= Time.deltaTime;
            }
        }
        else if(canFire)
        {
            canFire = false;
            if (rocketFireTimer <= 0)
                tc.FireRocket();
            else
                tc.FireShell();
        }

        if (Input.GetAxisRaw("Fire2") > 0.5)
        {
            canFireBomb = true;
        }
        else if(canFireBomb)
        {
            canFireBomb = false;
            tc.FireBomb();
        }
    }
}
