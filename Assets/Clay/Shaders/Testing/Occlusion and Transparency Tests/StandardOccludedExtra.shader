// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// shader made by Iron-Warrior at https://forum.unity.com/threads/render-object-behind-others-with-ztest-greater-but-ignore-self.429493/

Shader "Custom/StandardOccludedExtra"
{
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _OccludedColor("Occluded Color", Color) = (1,1,1,1)
    }
    SubShader {
   
        Pass
        {
            Tags { "Queue"="Geometry+1" }
            ZTest Greater
            ZWrite Off
 
            CGPROGRAM
            #pragma vertex vert            
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
 
            half4 _OccludedColor;
 
            float4 vert(float4 pos : POSITION) : SV_POSITION
            {
                float4 viewPos = UnityObjectToClipPos(pos);
                return viewPos;
            }
 
                half4 frag(float4 pos : SV_POSITION) : COLOR
            {
                return _OccludedColor;
            }
 
            ENDCG
        }

    }
    FallBack "Diffuse"
}
