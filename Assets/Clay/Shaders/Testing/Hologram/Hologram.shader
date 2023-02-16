Shader "Custom/Hologram"
{
    Properties
    {
        _FresnelPower ("Edge Color Power", Range(0.0,10.0)) = 1.0
        _FresnelIntensity ("Edge Color Intensity", Range(0.0,10.0)) = 1.0
        
        _Color ("Color", Color) = (1,1,1,1)
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        
        _MainTex ("Albedo (RGBA)", 2D) = "white" {}
        _ScanLines ("Scan Lines Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100
 
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
 
        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile_fog
         
                #include "UnityCG.cginc"
                
                struct appdata_t 
                {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                    float3 normal : NORMAL;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };
                
                struct v2f 
                {
                    float4 vertex : SV_POSITION;
                    float2 texcoord : TEXCOORD0;
                    float3 normal : TEXCOORD1;
                    UNITY_FOG_COORDS(1)
                    UNITY_VERTEX_OUTPUT_STEREO
                };
                
                sampler2D _MainTex;
                float4 _MainTex_ST;
                float3 viewDir;
                float _FresnelPower;
                float _FresnelIntensity;
                
                v2f vert (appdata_t v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.normal = v.normal;
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }
         
                fixed4 frag (v2f i) : SV_Target
                {
                    half rim = 1 - saturate(dot(normalize(viewDir), i.normal));
                 
                    fixed4 col = tex2D(_MainTex, i.texcoord) * pow(rim, _FresnelPower) * _FresnelIntensity;;
                    UNITY_APPLY_FOG(i.fogCoord, col);
                 
                    return col;
                }
            ENDCG
        }
    }
}
