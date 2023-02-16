using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iceCubeColliderController : MonoBehaviour
{

    public bool colFlag = false;
    public iceCubeController parent;
    public string dirName;
    public string oppositeDirName;
    public string boundaryName;

    // Start is called before the first frame update
    void Start()
    {
        parent.canMoveInDir[dirName] = true;

        //if (parent.transform.Find("Boundaries/"+boundaryName).gameObject.activeSelf) { parent.canMoveInDir[dirName] = false; collisionCounter++; }
    }

    // Update is called once per frame
    void Update()
    {   
    }

    int collisionCounter = 0;

    void OnTriggerEnter(Collider col) {
        if(col.gameObject.GetComponent<BulletController>() == null) {
            parent.canMoveInDir[oppositeDirName] = false;
            collisionCounter++;
            return;
        }
        col.gameObject.GetComponent<BulletController>().DestroyShell();
        colFlag = true;
        parent.moveObj(dirName);
    }

    void OnTriggerExit(Collider col) {
        if(col.gameObject.GetComponent<BulletController>() == null) {
            collisionCounter--;
            if (collisionCounter <= 0) parent.canMoveInDir[oppositeDirName] = true;
            return;
        }
        colFlag = false;
        //parent.moveObj(dirName);
    }
}
