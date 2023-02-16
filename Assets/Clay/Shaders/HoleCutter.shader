// Shader (and whole easy holes setup) by u/Happylanders at https://www.reddit.com/r/Unity3D/comments/g14m68/how_to_make_in_5_steps_realistic_looking_holes/

Shader "Custom/HoleCutter"
{
    SubShader
    {
        Tags {"Queue" = "Geometry-1" }
        Lighting Off
        Pass
        {
            ZWrite On
            ZTest LEqual
            ColorMask 0
        }
    }
}
