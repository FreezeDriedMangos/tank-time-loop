Shader "OcclusionSilhouette/OcclusionSillhouetteHelper" 
{
    SubShader
    {
        Tags { "OcclusionSilhouette"="Occludable1" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 vert(float4 pos : POSITION) : SV_POSITION
            {
                float4 viewPos = UnityObjectToClipPos(pos);
                return viewPos;
            }

            half4 frag(float4 pos : SV_POSITION) : COLOR
            {
                float4 col = (1,0,0,1);
                col.a = 1;
                col.r = 1;
                col.gb = 0;
                return col;
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags { "OcclusionSilhouette"="Occludable2" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 vert(float4 pos : POSITION) : SV_POSITION
            {
                float4 viewPos = UnityObjectToClipPos(pos);
                return viewPos;
            }

            half4 frag(float4 pos : SV_POSITION) : COLOR
            {
                float4 col = (0,1,0,1);
                col.a = 1;
                col.g = 1;
                col.rb = 0;
                return col;
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags { "OcclusionSilhouette"="Occluder" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 vert(float4 pos : POSITION) : SV_POSITION
            {
                float4 viewPos = UnityObjectToClipPos(pos);
                return viewPos;
            }

            half4 frag(float4 pos : SV_POSITION) : COLOR
            {
                float4 col = (1,1,1,1);
                col.a = 1;
                col.rgb = 1;
                return col;
            }
            ENDCG
        }
    }
}
