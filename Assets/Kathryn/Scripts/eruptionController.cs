using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eruptionController : MonoBehaviour
{
    public GameObject smoke;
    public GameObject eruption;
    public GameObject capsule;
    private GameObject smokeObj;
    

    // Start is called before the first frame update
    void Start()
    {
        smokeObj = GameObject.Instantiate(smoke, transform);
        smokeObj.GetComponent<ParticleSystem>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator OnCollisionEnter(Collision col) {
        if(col.gameObject.GetComponent<BulletController>() == null) {
            yield break;
        }

        col.gameObject.GetComponent<BulletController>().DestroyShell();
        smokeObj.GetComponent<ParticleSystem>().Stop();
        GameObject eruptionObj = GameObject.Instantiate(eruption, transform);
        yield return new WaitForSeconds(0.5f);
        GameObject capsuleObj = GameObject.Instantiate(capsule, transform);
        yield return new WaitForSeconds(4.0f);
        // eruptionObj.GetComponent<ParticleSystem>().Stop();
        foreach (Transform child in eruptionObj.transform) {
            GameObject PS = child.gameObject;
            PS.GetComponent<ParticleSystem>().Stop();
        }
        Destroy(capsuleObj);
        smokeObj.GetComponent<ParticleSystem>().Play();
        
        
        
        
    }

    

}
