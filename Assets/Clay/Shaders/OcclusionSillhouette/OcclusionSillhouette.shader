Shader "OcclusionSilhouette/OcclusionSillhouette"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _ObjectsOnly ("Occludables Only Render Texture", 2D) = "white" {}
        _CoverOnly ("Occluders Only Render Texture", 2D) = "white" {}
        _ObjectsAndCover ("Both Render Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
            
            fixed4 _Color;
            sampler2D _MainTex;
            sampler2D _ObjectsOnly;
            sampler2D _ObjectsAndCover;
            sampler2D _CoverOnly;
            
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 object = tex2D(_ObjectsOnly, i.uv);
                fixed4 cover = tex2D(_CoverOnly, i.uv);
                fixed4 objectAndCover = tex2D(_ObjectsAndCover, i.uv);
                
                float sillhouette = (objectAndCover == cover)*object.a;
                
                float4 col = sillhouette * _Color;
                
                return col;
            }
            ENDCG
        }
    }
}
