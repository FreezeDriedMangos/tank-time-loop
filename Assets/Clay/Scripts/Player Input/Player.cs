using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // notes:
    // Mouse.current.position.ReadValue() -> returns a Vector2 containing the current mouse pos

    public CursorController cursor;
    public TankController tank;

    public Joycon joycon { get { return GetComponent<JoyconInput>().joycon; } set { GetComponent<JoyconInput>().joycon = value; }}
    public bool hasJoycon; // for inspector exposure

    public float aimSpeed = 1;
    private Vector2 aimDirection;

    private bool oldInput = true;
    private bool setAim = false;
    private Vector2 aimLoc;

    private bool rotateAim = false;
    private float currentlyRotating = 0;
    private float aimRotation;
    public float rotateAimSensitivity = 0.05f;

    private bool setRotateAimThroughSetAim = false;


    public delegate void Notifier();
    public Notifier onInputNotify;
    public Notifier onFireNotify;
    public Notifier onShootRocketNotify;


    public static int numJoycons = 0;


    public MenuNavigator meuNavigator;

    
    private void Awake() 
    {
        PlayerInput inp = GetComponent<PlayerInput>();
        string scheme = inp.currentControlScheme;
        this.gameObject.name = "Player - " + scheme;    

        // if (scheme == "Joycon L" || scheme == "Joycon R")
        //     aimSpeed *= 10;

        if (JoyconManager.Instance != null && JoyconManager.Instance.gameObject.active && JoyconManager.Instance.enabled)
        {
            
            //Player[] ps = GameObject.FindObjectsOfType<Player>();
            Player p = this;//.GetComponent<Player>(); //ps[ps.Length-1];

            Debug.Log("player might joycon: " + p.name + " ; " + scheme + " ; " + scheme.Substring(0, 14));
            if (scheme.StartsWith("Joycon"))
            {
                Debug.Log("HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
                p.joycon = JoyconManager.Instance.j[numJoycons++];
                Debug.Log("player recieved joycon " + (numJoycons-1));
                p.hasJoycon = true;
            }
        }

        //meuNavigator = GameObject.FindObjectOfType<MenuNavigator>();
        OnSceneFinishedLoading(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        meuNavigator = GameObject.FindObjectOfType<MenuNavigator>();
    }

    // 
    // Events
    //
    public void OnMove(InputAction.CallbackContext context) { if (hasJoycon) return; OnMove(context.ReadValue<Vector2>(), context); }

    public void OnFireShell(InputAction.CallbackContext context)
    {
        if (hasJoycon) return; 
        if (!context.started) return;
        if (onInputNotify != null) onInputNotify();
        //if (meuNavigator != null) meuNavigator.Select(this);
        if (tank == null) return;
        //context.action;

        OnFireShell();
    }

    public void OnFireRocket(InputAction.CallbackContext context)
    {
        if (hasJoycon) return; 
        if (!context.started) return;
        if (onInputNotify != null) onInputNotify();
        if (meuNavigator != null) meuNavigator.Select(this);
        if (tank == null) return;

        OnFireRocket();
    }

    public void OnFireBomb(InputAction.CallbackContext context)
    {
        if (hasJoycon) return; 
        if (!context.started) return;
        if (onInputNotify != null) onInputNotify();
        if (meuNavigator != null) meuNavigator.Select(this);
        if (tank == null) return;

        OnFireBomb();
    }

    public void OnAim(InputAction.CallbackContext context) { if (hasJoycon) return; OnAim(context.ReadValue<Vector2>()); }

    public void OnRotateAim(InputAction.CallbackContext context) { if (hasJoycon) return; OnRotateAim(context.ReadValue<float>()); }

    public void OnSetAim(InputAction.CallbackContext context) { if (hasJoycon) return; OnSetAim(context.ReadValue<Vector2>()); }

    public void OnSetRotateAim(InputAction.CallbackContext context) { if (hasJoycon) return; Debug.Log("What value type? " + context.valueType); OnSetRotateAim(context.ReadValue<Vector2>()); }
    
    public void OnPause(InputAction.CallbackContext context)
    {
        if (hasJoycon) return; 
        if (!context.started) return;
        if (onInputNotify != null) onInputNotify();
        Debug.Log("PAUSING~!!!!!!!!!!");

        OnPause();
        // try  
        // {
        //     GameObject.FindObjectOfType<PauseMenu>().TogglePaused(this);
        // }
        // catch (System.Exception e) { Debug.LogError(e); }
    }
    
    // 
    // Events
    //

    const float UI_MOVE_SELECTION_COOLDOWN = 0.15f;
    private void MoveUISelect(Vector2 v)
    {
        if (v == Vector2.zero) return;
        if (uiMoveTimer > 0) return;
        uiMoveTimer = UI_MOVE_SELECTION_COOLDOWN;
        meuNavigator.MoveSelection(this, v);
    }

    // 
    // Handlers
    //
    public void OnPause()
    {
        GameObject.FindObjectOfType<PauseMenu>().TogglePaused(this);
    }

    public bool onMoveNotify = true;

    public void OnMove(Vector2 v, InputAction.CallbackContext context)
    {
        if (onMoveNotify) if (onInputNotify != null) onInputNotify();
        if (meuNavigator != null /*&& context.phase == InputActionPhase.Performed*/) MoveUISelect(v);
        if (tank == null) return;

        tank.MoveInDirection(v);
    }

    public void OnMove(Vector2 v)
    {
        if (onMoveNotify) if (onInputNotify != null) onInputNotify();
        if (meuNavigator != null) MoveUISelect(v);
        if (tank == null) return;

        tank.MoveInDirection(v);
    }

    public void OnFireShell()
    {
        if (onInputNotify != null) onInputNotify();
        //if (hasJoycon && meuNavigator != null) meuNavigator.Select(this);
        if (tank == null) return;

        tank.FireShell();
    }

    public void OnFireRocket()
    {
        if (onInputNotify != null) onInputNotify();
        if (hasJoycon && meuNavigator != null) meuNavigator.Select(this);
        if (tank == null) return;
        tank.FireRocket();
    }

    public void OnFireBomb()
    {
        if (onInputNotify != null) onInputNotify();
        if (hasJoycon && meuNavigator != null) meuNavigator.Select(this);
        if (tank == null) return;
        tank.FireBomb();
    }

    public void OnAim(Vector2 v)
    {
        if (onInputNotify != null) onInputNotify();
        aimDirection = v;
        oldInput = true;
    }

    public void OnRotateAim(float v)
    {
        if (onInputNotify != null) onInputNotify();
        rotateAim = true;
        currentlyRotating = v;
    }

    public void OnSetAim(Vector2 v)
    {
        //if (onInputNotify != null) onInputNotify();
        setAim = true;
        aimLoc = v;
    }

    Vector2 screenLoc;
      Vector2 aimRotationStick;

    float uiMoveTimer = 0;

    public void OnSetRotateAim(Vector2 v)
    {
        Debug.Log("Hit the function: " + v);
        if (v == Vector2.zero) return;
        if (onInputNotify != null) onInputNotify();
        // rotateAim = true;
        // Vector2 input = v;//new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));//v;
        // aimRotation = Mathf.Rad2Deg * Mathf.Atan2(input.y, input.x);

        // kinda working
        //aimLoc = Camera.main.WorldToScreenPoint(tank.transform.position + new Vector3(v.x, 0, v.y)); //new Vector2(tank.transform.position.x, tank.transform.position.z) + v;
        //setAim = true;

        // aimLoc = Camera.main.WorldToScreenPoint(tank.transform.position + new Vector3(v.x, 0, v.y)); //new Vector2(tank.transform.position.x, tank.transform.position.z) + v;
        // setRotateAimThroughSetAim = true;
        
        //Vector2 input = v;//new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));//v;
        //aimRotation = Mathf.Rad2Deg * Mathf.Atan2(input.y, input.x);
        //setRotateAimThroughSetAim = true;

        
        //rotateAim = false;
        //currentlyRotating = 0;

        oldInput = false;
        setRotateAimThroughSetAim = true;

        // Vector3 rotation = new Vector3(v.x, 0, v.y);

        // RaycastHit hit;
        // Physics.Raycast(cursor.tank.ShellSpawnLocation, rotation, out hit, 200, LayerMask.GetMask("Arena Objects"));

        // screenLoc = Camera.main.WorldToScreenPoint(hit.point);

        // cursor.SetPos(screenLoc);


          aimRotationStick = v;

    }
    // 
    // Handlers
    //

    Vector3 lastCamPos;
    Vector3 lastCamRot;
    void FixedUpdate() 
    {
        //if (aimDirection == Vector2.zero) return;

        if(uiMoveTimer > 0) uiMoveTimer -= Time.deltaTime;

        // for motion control aiming
        if (lastCamPos != Camera.main.transform.position || lastCamRot != Camera.main.transform.eulerAngles)
        {
            lastCamPos = Camera.main.transform.position;
            lastCamRot = Camera.main.transform.eulerAngles;
            this.transform.eulerAngles = Camera.main.transform.eulerAngles;
            this.transform.position    = Camera.main.transform.position;
        }

        DoAim();
    }

    void DoAim()
    {
        if (cursor == null) return;

        if (hasJoycon)
        {
            tank.AimAt(cursor.GetAimLocation());
            return;
        }

        if (setAim)
            cursor.SetPos(new Vector3(Mathf.Max(0, Mathf.Min(Screen.width, aimLoc.x)), Mathf.Max(0, Mathf.Min(Screen.height, aimLoc.y)), 0));//aimLoc);
        else if (setRotateAimThroughSetAim)
        {
              Vector3 rotation = new Vector3(aimRotationStick.x, 0, aimRotationStick.y);

              //Debug.Log("ROTATING AIM TO BE " + rotation);

              RaycastHit hit;
              Physics.Raycast(cursor.tank.ShellSpawnLocation, rotation, out hit, 200, LayerMask.GetMask("Arena Objects"));

              //Debug.Log("DECIEDE AIM POINT: " + hit.point);

              screenLoc = Camera.main.WorldToScreenPoint(hit.point);

            // cursor.SetPos(screenLoc);
            cursor.SetPos(screenLoc);
            //DoRotateCursor();
            // float x = Mathf.Max(0, Mathf.Min(Screen.width, aimLoc.x));
            // float y = Mathf.Max(0, Mathf.Min(Screen.height, aimLoc.y));

            // aim

            // cursor.SetPos(new Vector3(x, y, 0));
        }    
        else if (rotateAim)
        {
            aimRotation += currentlyRotating*rotateAimSensitivity;
            DoRotateCursor();
        }
        else if (oldInput)
            cursor.MovePos(aimDirection * aimSpeed);

        if (tank == null) return;
        
        tank.AimAt(cursor.GetAimLocation());
    }

    void DoRotateCursor()
    {
        Vector3 rotation = new Vector3(Mathf.Cos(aimRotation), 0, Mathf.Sin(aimRotation));

        RaycastHit hit;
        Physics.Raycast(cursor.tank.ShellSpawnLocation, rotation, out hit, 200, LayerMask.GetMask("Arena Objects"));

        Vector2 screenLoc = Camera.main.WorldToScreenPoint(hit.point);

        cursor.SetPos(screenLoc);
    }




    public void DebugInput(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<float>());
    }
}
