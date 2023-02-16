using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CanvasFullsizer : MonoBehaviour
{


    RectTransform prt = null;
    RectTransform rt = null;

    // Start is called before the first frame update
    void Start()
    {
        prt = this.transform.parent.GetComponent<RectTransform>();
        rt  = this.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rt.sizeDelta = new Vector2(prt.rect.width, prt.rect.height);
    }
}
