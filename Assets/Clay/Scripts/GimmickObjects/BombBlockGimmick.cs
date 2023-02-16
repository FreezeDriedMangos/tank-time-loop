using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBlockGimmick : MonoBehaviour
{

    void Awake()
    {
        SwitchReactor sr = GetComponent<SwitchReactor>();

        sr.SwitchEnabled = StartTimer;
        sr.SwitchDisabled = null;
        
        dc.OnBlowUp = BlowUpAnyway;
    }

    bool timerStarted = false;
    float timeLeft = 0;
    bool haveBlownUp = false;
    public float explodeTimer = 2.5f;
    public GameObject explosionPrefab;

    public GameObject explosionMarkerPrefab;

    public destructionController dc;

    public UnityEngine.UI.Slider[] sliders;

    public void StartTimer()
    {
        timerStarted = true;
        timeLeft = explodeTimer;

        foreach(UnityEngine.UI.Slider s in sliders)
            s.value = 100f * (explodeTimer-timeLeft) / explodeTimer;
    }

    void FixedUpdate()
    {
        if(!timerStarted) return;
        if(haveBlownUp) return;

        timeLeft -= Time.deltaTime;

        foreach(UnityEngine.UI.Slider s in sliders)
            s.value = 100f * (explodeTimer-timeLeft) / explodeTimer;

        if(timeLeft <= 0)
        {
            timerStarted = false;
            if (!haveBlownUp)
            {
                Explode();
            }
        }
    }

    void BlowUpAnyway()
    {
        if (!haveBlownUp)
        {
            Explode();
        }
    }

    void Explode()
    {
        foreach(UnityEngine.UI.Slider s in sliders)
            s.value = 0;

        GameObject g = GameObject.Instantiate(explosionPrefab);
        g.transform.position = this.transform.position;
        haveBlownUp = true;

        GameObject m = GameObject.Instantiate(explosionMarkerPrefab);
        m.transform.position = this.transform.position + new Vector3(0, 0.1f, 0);
        if (TankController.TracksMaster != null)
            m.transform.parent = TankController.TracksMaster.transform;
    }
}
