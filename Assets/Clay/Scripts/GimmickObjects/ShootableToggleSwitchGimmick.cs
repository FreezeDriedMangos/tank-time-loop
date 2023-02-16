using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableToggleSwitchGimmick : MonoBehaviour
{
    GimmickMeta gm;

    List<SwitchReactor> recievers = new List<SwitchReactor>();

    bool active = false;

    bool toggling = false;

    public float speed = 10;
    float rotation = 0; 
    float targetRotation = 0;

    void Awake()
    {
        gm = GetComponent<GimmickMeta>();
        gm.Setup = Setup;
    }

    public void Setup()
    {
        foreach (GimmickMeta gm in GameObject.FindObjectsOfType<GimmickMeta>())
        {
            if (gm.metadata != this.gm.metadata) continue;

            SwitchReactor sr = gm.GetComponent<SwitchReactor>();

            if (sr == null) continue;

            recievers.Add(sr);
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (toggling) return;

        BulletController b = c.GetComponent<BulletController>();
        if (b == null) return;

        Toggle();
    }

    void Toggle()
    {
        active = !active;
        toggling = true;
        targetRotation = Mathf.Abs(targetRotation-90);
        rotation = 0;

        foreach (SwitchReactor sr in recievers)
        {
            if (sr == null) continue;
            try { if (active) sr.SwitchDisabled(); else sr.SwitchEnabled(); } catch {} // this is so dumb that I need a try/catch. For some reason unity thought sr was null sometimes
        }
    }

    void FixedUpdate() 
    {
        if (toggling)
        {
            this.transform.eulerAngles += speed * new Vector3(0, 1, 0);
            rotation += speed;

            if (rotation > 90)
            {
                this.transform.eulerAngles = new Vector3(0, targetRotation, 0);
                toggling = false;
            }
        }
    }
}
