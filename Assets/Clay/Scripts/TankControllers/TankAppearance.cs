using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAppearance : MonoBehaviour
{
    // parameters
    public Material[] hologramRounds;
    public Material[] rogueRounds;
    
    public Material headMaterial;
    public Material chasisMaterial;
    public Material wheelsMaterial;
    public Material detailsMaterial;
    public Material headlightsMaterial;
    
    // private stuff
    private bool setup = false;
    private MeshRenderer tankHead;
    private MeshRenderer tankChasis;
    private MeshRenderer tankTreadsR;
    private MeshRenderer tankTreadsL;

    private GameObject holoring;

    private GameObject rogueSprite;

    private bool isInDestroyed = false;
    
    public void Setup()
    {
        if (setup) return;
        setup = true;

        tankHead = transform.Find("TankRenderers/TankTurret").GetComponent<MeshRenderer>();
        tankChasis = transform.Find("TankRenderers/TankChassis").GetComponent<MeshRenderer>();
        tankTreadsL = transform.Find("TankRenderers/TankTracksLeft").GetComponent<MeshRenderer>();
        tankTreadsR = transform.Find("TankRenderers/TankTracksRight").GetComponent<MeshRenderer>();

        holoring = transform.Find("Holoring").gameObject;
        rogueSprite = transform.Find("GoneRogue Icon").gameObject;
        isInDestroyed = false;
    }

    public void SetHologram(int roundNum)
    {
        if (hologramRounds == null || hologramRounds.Length == 0) return;
        if (hologramRounds.Length <= roundNum || roundNum < 0) roundNum = 0;

        Material holo = hologramRounds[roundNum];

        Material[] holo2 = {holo, holo};
        Material[] holo3 = {holo, holo, holo};
        
        tankChasis.materials = holo3;
        tankHead.materials = holo2;
        tankTreadsR.materials = holo2;
        tankTreadsL.materials = holo2;

        rogueSprite.SetActive(false);
        isInDestroyed = false;
        
        //if (holoring != null) holoring.SetActive(true);
    }

    public void SetRogue(int roundNum)
    {
        if (isInDestroyed) return;
        if (rogueRounds == null || rogueRounds.Length == 0) return;
        if (rogueRounds.Length <= roundNum || roundNum < 0) roundNum = 0;

        Material holo = rogueRounds[roundNum];

        Material[] holo2 = {holo, holo};
        Material[] holo3 = {holo, holo, holo};
        
        tankChasis.materials = holo3;
        tankHead.materials = holo2;
        tankTreadsR.materials = holo2;
        tankTreadsL.materials = holo2;

        rogueSprite.SetActive(true);

        //if (holoring != null) holoring.SetActive(true);
    }

    public void SetDestroyed(bool hologram)
    {
        rogueSprite.SetActive(false);
        isInDestroyed = true;
    }

    public void SetNatural()
    {
        Material[] head =   {headMaterial,   detailsMaterial};
        Material[] tracks = {wheelsMaterial, detailsMaterial};
        Material[] chasis = {chasisMaterial, headlightsMaterial, detailsMaterial};
        
        tankChasis.materials = chasis;
        tankHead.materials = head;
        tankTreadsR.materials = tracks;
        tankTreadsL.materials = tracks;

        rogueSprite.SetActive(false);
        isInDestroyed = false;

        if (holoring != null) holoring.SetActive(false);
    }
}
