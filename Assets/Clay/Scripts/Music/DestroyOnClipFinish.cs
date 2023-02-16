using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnClipFinish : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake() 
    {
        StartCoroutine("DestroyOnClipFinishMethod");    
    }

    IEnumerator DestroyOnClipFinishMethod()
    {
        yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
        GameObject.Destroy(this.gameObject);
    }
}
