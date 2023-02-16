using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CursorController))]
public class CursorMouseControl : MonoBehaviour
{
    private CursorController cc;
    private bool setup = false;
    public bool isJustForMenus = false;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    void Setup()
    {
        if (setup) return;
        setup = true;
        
        Cursor.visible = false;

        cc = GetComponent<CursorController>();
        cc.Setup();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 screenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (isJustForMenus)
        {
            this.transform.position = screenPos;
            return;
        }
        // Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        cc.SetPos(screenPos);
    }
}
