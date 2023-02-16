Shader "Custom/ZWrite"
{
    SubShader {
        Tags {"Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200

        // extra pass that renders to depth buffer only
        Pass {
            ZWrite On
            ColorMask 0
        }

    }
}
