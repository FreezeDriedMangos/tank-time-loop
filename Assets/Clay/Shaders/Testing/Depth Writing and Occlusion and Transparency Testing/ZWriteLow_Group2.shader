Shader "Custom/ZWriteLow_Group2"
{
    SubShader {
        Tags {"Queue"="Geometry-2" "IgnoreProjector"="False" "RenderType"="Transparent"}


        // extra pass that renders to depth buffer only
        Pass {
            ZWrite On
            
            ColorMask 0
        }

    }
}
