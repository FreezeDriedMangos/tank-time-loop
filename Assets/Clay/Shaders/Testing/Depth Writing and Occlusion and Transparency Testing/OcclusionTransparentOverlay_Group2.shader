Shader "Custom/OcclusionTransparentOverlay_Group2"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _OccludedColor("Occluded Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags{ "Queue" = "Geometry-1" "IgnoreProjector" = "False" "RenderType" = "Transparent" }
 
         
//          Pass {
//             ZWrite Off
//              
//             CGPROGRAM
//              #pragma vertex vert
//              #pragma fragment frag
//  
//              fixed4 _Color;
//  
//              float4 vert(float4 vertex : POSITION) : SV_POSITION {
//                  return UnityObjectToClipPos(vertex);
//              }
//  
//              fixed4 frag() : SV_Target {
//                  return _Color;
//              }
//             
//              ENDCG
//          }
         /////////////////////////////////////////////////////////
         /// First Pass
         /////////////////////////////////////////////////////////
 
         Pass {
             // Only render alpha channel
             ColorMask A
             Blend SrcAlpha OneMinusSrcAlpha
 
             ZWrite Off
    
             Offset [_Offset], [_Offset]
 
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
 
             fixed4 _OccludedColor;
 
             float4 vert(float4 vertex : POSITION) : SV_POSITION {
                 return UnityObjectToClipPos(vertex);
             }
 
             fixed4 frag() : SV_Target {
                 return _OccludedColor;
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
 
             ZWrite Off
            
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
 
             sampler2D _MainTex;
             fixed4 _OccludedColor;
 
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
                 // render to stencil with color white
                 fixed4 col = _OccludedColor * tex2D(_MainTex, i.uv);
                 return col;
             }
             ENDCG
         }
         
//          Pass {
//             ZTest Always
//             
//             // render from stencil, multiply color by _OccludedColor
//          }
    
//         Tags { "RenderType"="Opaque" }
//         LOD 200
// 
//         CGPROGRAM
//         // Physically based Standard lighting model, and enable shadows on all light types
//         #pragma surface surf Standard fullforwardshadows
// 
//         // Use shader model 3.0 target, to get nicer looking lighting
//         #pragma target 3.0
// 
//         sampler2D _MainTex;
// 
//         struct Input
//         {
//             float2 uv_MainTex;
//         };
// 
//         half _Glossiness;
//         half _Metallic;
//         fixed4 _Color;
// 
//         // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
//         // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
//         // #pragma instancing_options assumeuniformscaling
//         UNITY_INSTANCING_BUFFER_START(Props)
//             // put more per-instance properties here
//         UNITY_INSTANCING_BUFFER_END(Props)
// 
//         void surf (Input IN, inout SurfaceOutputStandard o)
//         {
//             // Albedo comes from a texture tinted by color
//             fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
//             o.Albedo = c.rgb;
//             // Metallic and smoothness come from slider variables
//             o.Metallic = _Metallic;
//             o.Smoothness = _Glossiness;
//             o.Alpha = c.a;
//         }
//         ENDCG
    }
    FallBack "Diffuse"
}
