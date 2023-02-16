Shader "Custom/ZWrite_Group2"
{
    SubShader {
        Tags {"Queue"="Transparent+3" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200

        // extra pass that renders to depth buffer only
        Pass {
            ZWrite On
            ColorMask 0
        }

    }
}
