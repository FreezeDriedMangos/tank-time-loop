
using Newtonsoft.Json.Linq;
using System;

[Serializable]
public class VsModeConfig
{
    public float roundLength_seconds;
    public float roundCooldown_seconds;
    public float roundEndLength_seconds;
    public int maxNumRounds;

    public float countdownWhenXSecondsLeftInRound;
}
