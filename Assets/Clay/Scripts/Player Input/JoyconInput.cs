using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconInput : MonoBehaviour
{
    public Joycon joycon;
    public Player player;

    private bool hasRecentered = false;

    [SerializeField] private GameObject aimPoint;
    [SerializeField] private GameObject aimRotator;

    public Vector2 move;

    public float sensitivity = 1.5f;

    private Vector3 previousCursor;
    

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (joycon == null) return;

        player.onMoveNotify = false;

        if (!hasRecentered || joycon.GetButtonDown(Joycon.Button.CAPTURE) || joycon.GetButtonDown(Joycon.Button.HOME))
        {
            Debug.Log("recentering");

            hasRecentered = true;
            joycon.Recenter();
            aimRotator.transform.eulerAngles = new Vector3();
            transform.position = Camera.main.transform.position;
        }

        if (player == null) return;
        
        if (joycon.GetButtonDown(Joycon.Button.PLUS) || joycon.GetButtonDown(Joycon.Button.MINUS))
            player.OnPause();

        if (joycon.GetButtonDown(Joycon.Button.SHOULDER_2))
            player.OnFireShell();
        
        if (joycon.GetButtonDown(Joycon.Button.SHOULDER_1))
            player.OnFireRocket();
        
        if (joycon.GetButtonDown(Joycon.Button.SL) || joycon.GetButtonDown(Joycon.Button.SR) || joycon.GetButtonDown(Joycon.Button.DPAD_DOWN))
            player.OnFireBomb();

        // unused buttons that should still trigger input
        if (joycon.GetButtonDown(Joycon.Button.DPAD_RIGHT) || joycon.GetButtonDown(Joycon.Button.DPAD_LEFT) || joycon.GetButtonDown(Joycon.Button.DPAD_UP) || joycon.GetButtonDown(Joycon.Button.MINUS) || joycon.GetButtonDown(Joycon.Button.PLUS))
            player.onInputNotify();

        float[] stick = joycon.GetStick();
        move = new Vector2(stick[0], stick[1]);//new Vector2(Mathf.Round(stick[0]), Mathf.Round(stick[1]));
        player.OnMove(move);

        //Debug.Log(joycon.GetVector());

        aimRotator.transform.localRotation = joycon.GetVector();

        // aimRotator.transform.eulerAngles = new Vector3(rotatorDummy.transform.eulerAngles.z, rotatorDummy.transform.eulerAngles.x, rotatorDummy.transform.eulerAngles.y);


        if (player.cursor == null) return;
        Vector3 screenCenter = new Vector3(Screen.width/2f, Screen.height/2f, 0);
        Vector3 delta = Camera.main.WorldToScreenPoint(aimPoint.transform.position) - screenCenter;
        Vector3 adjustedPos = sensitivity*delta + screenCenter;
        Vector3 clampedPos = new Vector3(Mathf.Max(0, Mathf.Min(Screen.width, adjustedPos.x)), Mathf.Max(0, Mathf.Min(Screen.height, adjustedPos.y)), 0); 
        
        Vector3 averagedPos = LerpVector(clampedPos, previousCursor, 0.8f);//0.5f * (clampedPos + previousCursor);
        previousCursor = averagedPos;

        if (GameState.config.miscConfig.JoyconMotionDampening)
            player.cursor.SetPos(averagedPos);
        else
            player.cursor.SetPos(clampedPos);
            
        // player.tank.AimAt(player.cursor.GetAimLocation());
    }

    public Vector3 LerpVector(Vector3 from, Vector3 to, float t)
    {
        return new Vector3(
            Mathf.Lerp(from.x, to.x, t),
            Mathf.Lerp(from.y, to.y, t),
            Mathf.Lerp(from.z, to.z, t)
        );
    }
}
