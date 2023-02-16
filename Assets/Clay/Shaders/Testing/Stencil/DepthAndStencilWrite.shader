Shader "Custom/DepthAndStencilWrite"
{
    Properties
    {
        [IntRange] _StencilRef ("Stencil Reference Value", Range(0, 255)) = 0
    }
    SubShader
    {
        // record whether the object was occluded
        Pass {
            Tags { "RenderType"="Opaque"}
            Blend Zero One
            ZWrite Off
            ZTest LEqual
            
            Stencil {
                Ref [_StencilRef] // this Stencil block's value (separate from the stencil's actual value)
                Comp Always  // run this Stencil block no matter what the current value of the stencil is
                Pass Keep // if the object is not occluded, write this block's value to the stencil
                ZFail Replace // if the object IS occluded, write this block's value to the stencil
            }
        }
        
        // write the depth value, ignoring all other objects in the scene (basically, render this object on top of everything else)
        Pass {
            Tags {"RenderType"="Transparent"}
            Blend Zero One
            
            ZTest Greater
            Stencil {
                Ref [_StencilRef] // this Stencil block's value (separate from the stencil's actual value)
                Comp Equal  // run this Stencil block only if the stencil value is the same as the block's
            }
            
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
        
    }
    //FallBack "Diffuse"
}
