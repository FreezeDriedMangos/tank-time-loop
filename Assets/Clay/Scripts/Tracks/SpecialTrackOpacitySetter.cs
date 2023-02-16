using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTrackOpacitySetter : MonoBehaviour
{
    void Awake()
    {
        this.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, GameState.config.miscConfig.specialTracksOpacity);
    }
}
