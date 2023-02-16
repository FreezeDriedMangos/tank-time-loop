using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingOfTheHillPoint : MonoBehaviour
{
    public Material neutralMat;
    public Material p1Mat;
    public Material p2Mat;
    public Material p3Mat;
    public Material p4Mat;
    
    private MeshRenderer renderer;
    private int p1Claim;
    private int p2Claim;
    private int p3Claim;
    private int p4Claim;
    private int enemyClaim;
    
    List<TankController> claimingTanks = new List<TankController>();

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    public void Reset()
    {
        p1Claim = 0;
        p2Claim = 0;
        p3Claim = 0;
        p4Claim = 0;
        enemyClaim = 0;

        claimingTanks.Clear();

        UpdateMat();
    }

    void OnTriggerEnter(Collider other)
    {
        TankController t = other.GetComponent<TankController>();

        if (t == null) return;

        claimingTanks.Add(t);

        switch (t.team)
        {
            case TankController.Team.PLAYER_1: p1Claim++; break;
            case TankController.Team.PLAYER_2: p2Claim++; break;
            case TankController.Team.PLAYER_3: p3Claim++; break;
            case TankController.Team.PLAYER_4: p4Claim++; break;
            case TankController.Team.ENEMY: enemyClaim++; break;
        }
        UpdateMat();
    }

    void OnTriggerExit(Collider other)
    {
        TankController t = other.GetComponent<TankController>();

        if (t == null) return;

        claimingTanks.Remove(t);

        switch (t.team)
        {
            case TankController.Team.PLAYER_1: p1Claim--; break;
            case TankController.Team.PLAYER_2: p2Claim--; break;
            case TankController.Team.PLAYER_3: p3Claim--; break;
            case TankController.Team.PLAYER_4: p4Claim--; break;
            case TankController.Team.ENEMY: enemyClaim--; break;
        }

        UpdateMat();
    }

    void FixedUpdate()
    {
        if (whoseClaim <= 0) return;
        GameState.kingOfTheHillScores[whoseClaim-1] += Time.deltaTime;

        List<TankController> removals = new List<TankController>();
        foreach(TankController t in claimingTanks)
        {
            if (!t.destroyed) continue;
            removals.Add(t);

            switch (t.team)
            {
                case TankController.Team.PLAYER_1: p1Claim--; break;
                case TankController.Team.PLAYER_2: p2Claim--; break;
                case TankController.Team.PLAYER_3: p3Claim--; break;
                case TankController.Team.PLAYER_4: p4Claim--; break;
                case TankController.Team.ENEMY: enemyClaim--; break;
            }
        }

        foreach(TankController t in removals) { claimingTanks.Remove(t); }

        UpdateMat();
    }

    void UpdateMat()
    {
        UpdateClaim();

        switch(whoseClaim)
        {
            case 1: renderer.material = p1Mat; break;
            case 2: renderer.material = p2Mat; break;
            case 3: renderer.material = p3Mat; break;
            case 4: renderer.material = p4Mat; break;
            default: renderer.material = neutralMat; break;
        }
    }

    int whoseClaim;
    void UpdateClaim()
    {
        int biggestClaim = Mathf.Max(p1Claim, 
                           Mathf.Max(p2Claim, 
                           Mathf.Max(p3Claim, 
                           Mathf.Max(p4Claim, enemyClaim))));

        int numMatching = 0;
        if (p1Claim == biggestClaim) numMatching++;
        if (p2Claim == biggestClaim) numMatching++;
        if (p3Claim == biggestClaim) numMatching++;
        if (p4Claim == biggestClaim) numMatching++;
        if (enemyClaim == biggestClaim) numMatching++;
        
        if (numMatching != 1)
        {
            whoseClaim = -1;
            return;
        }
        
        if (p1Claim == biggestClaim) whoseClaim = 1;
        if (p2Claim == biggestClaim) whoseClaim = 2;
        if (p3Claim == biggestClaim) whoseClaim = 3;
        if (p4Claim == biggestClaim) whoseClaim = 4;
        if (enemyClaim == biggestClaim) whoseClaim = -1;
    }
}
