Shader "Custom/FlatTransparentMultiMesh_Group2"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags{ "Queue" = "Transparent+4" "IgnoreProjector" = "False" "RenderType" = "Transparent" }
 
         /////////////////////////////////////////////////////////
         /// First Pass
         /////////////////////////////////////////////////////////
 
         Pass {
             // Only render alpha channel
             ColorMask A
             Blend SrcAlpha OneMinusSrcAlpha
 
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
 
             fixed4 _Color;
 
             float4 vert(float4 vertex : POSITION) : SV_POSITION {
                 return UnityObjectToClipPos(vertex);
             }
 
             fixed4 frag() : SV_Target {
                 return _Color;
             }
 
             ENDCG
         }
 
         /////////////////////////////////////////////////////////
         /// Second Pass
         /////////////////////////////////////////////////////////
 
         Pass {
             // Now render color channel
             ColorMask RGB
             Blend SrcAlpha OneMinusSrcAlpha
 
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
 
             sampler2D _MainTex;
             fixed4 _Color;
 
             struct appdata {
                 float4 vertex : POSITION;
                 float2 uv : TEXCOORD0;
             };
 
             struct v2f {
                 float2 uv : TEXCOORD0;
                 float4 vertex : SV_POSITION;
             };
 
             v2f vert(appdata v) {
                 v2f o;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.uv = v.uv;
                 return o;
             }
 
             fixed4 frag(v2f i) : SV_Target{
                 fixed4 col = _Color * tex2D(_MainTex, i.uv);
                 return col;
             }
             ENDCG
         }
         
    }
    FallBack "Diffuse"
}
