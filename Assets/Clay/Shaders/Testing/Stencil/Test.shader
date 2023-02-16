Shader "Custom/Test"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _OccludedColor ("Occluded Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        
        [IntRange] _StencilRef ("Stencil Reference Value", Range(0, 255)) = 0
    }
    SubShader
    {
        // record whether the object was occluded
        Pass {
            Tags { "RenderType"="Opaque" "Queue"="Geometry+1"}
            Blend Zero One
            ZWrite Off
            //ZTest Less
            
            Stencil {
                Ref [_StencilRef] // this Stencil block's value (separate from the stencil's actual value)
                Comp Always  // run this Stencil block no matter what the current value of the stencil is
                Pass Keep // if the object is not occluded, write this block's value to the stencil
                ZFail Replace // if the object IS occluded, write this block's value to the stencil
            }
        }
        
//         // write the depth value, ignoring all other objects in the scene (basically, render this object on top of everything else)
//         Pass {
//             CGPROGRAM     
//             #pragma vertex vert   
//             #pragma fragment frag
//             
//             //sampler2D _MainTex;
//                 
//             struct fragOutput {
//                 //fixed4 color : SV_Target;
//                 float depth:SV_Depth;
//             };
//              
//             struct v2f {
//                 float4 vertex : SV_POSITION;
//                 half2 texcoord : TEXCOORD0;
//                 float depth : SV_Depth;
//             };
//  
//             float4 vert(float4 pos : POSITION) : SV_POSITION
//             {
//                 float4 viewPos = UnityObjectToClipPos(pos);
//                 return viewPos;
//             }
// 
//             fragOutput frag (float depth : SV_Depth)
//             {
//                 fragOutput o;
//                 //o.color = tex2D(_MainTex, i.uv);
//                 o.depth = depth;
//                 return o;
//             }
//  
//             ENDCG
//         }

        // write the depth value, ignoring all other objects in the scene (basically, render this object on top of everything else)
        Pass {
            Tags { "Queue"="Overlay+2" "RenderType"="Transparent"}
            Blend Zero One
            
            ZTest Greater
            
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f {
                float4 pos : SV_POSITION;
                float2 depth : TEXCOORD0;
            };

            v2f vert (appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_DEPTH(o.depth);
                return o;
            }

            half4 frag(v2f i) : SV_Target {
                UNITY_OUTPUT_DEPTH(i.depth);
            }
            ENDCG
        }
        
//         // make the object not occluded, except by itself
//         Pass {
//             Tags { "RenderType"="Opaque" "Queue"="Overlay+1"}
//             Blend Zero One
//             
//             ZWrite On
//         }
        
        // if the object originally wasn't occluded, render normally
        Pass
        {
            Tags { "Queue"="Overlay+2" "RenderType"="Transparent"}
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            
            Stencil {
                Ref [_StencilRef]      // declare the desired stencil value
                Comp notequal             // only render when stencil value is equal to the desired
            }
            
            CGPROGRAM
            #pragma vertex vert            
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
 
            half4 _Color;
 
            float4 vert(float4 pos : POSITION) : SV_POSITION
            {
                float4 viewPos = UnityObjectToClipPos(pos);
                return viewPos;
            }
 
            half4 frag(float4 pos : SV_POSITION) : COLOR
            {
                return _Color;
            }
 
            ENDCG
        }
        
        // if the object was occluded, render the transparent thing
        Pass
        {
            Tags { "Queue"="Overlay+2" "RenderType"="Transparent"}
            ZWrite On
            
            Blend SrcAlpha OneMinusSrcAlpha
            
            Stencil {
                Ref [_StencilRef]      // declare the desired stencil value
                Comp Equal             // only render when stencil value is equal to the desired
            }
            
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
        
        
        
//         // 
//         // Standard Surface Shader below
//         //
//         
//         Tags { "RenderType"="Opaque" "Queue"="Geometry+1"}
//         
//         ZTest Always
//         
//         Stencil {
//             Ref [_StencilRef]      // declare the desired stencil value
//             Comp Equal             // only render when stencil value is equal to the desired
//         }
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
    //FallBack "Diffuse"
}
