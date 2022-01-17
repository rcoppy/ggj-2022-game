Shader "N3DS/Vegetation/Leaves"
{
	Properties {
		_Color ("Main Color", Color) = (.5, .5, .5, .5)
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
    }
    SubShader {
        
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

        Pass {
			// Set up basic lighting
			Material {
				Diffuse [_Color]
				Ambient [_Color]
			}
			Lighting On

			Cull Front
            ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha
			
            SetTexture [_MainTex] {
                combine texture * primary, texture
            }
        }

		Pass {
			// Set up basic lighting
			Material {
				Diffuse [_Color]
				Ambient [_Color]
			}
			Lighting On

			Cull Back
            ZWrite Off
						
			Blend SrcAlpha OneMinusSrcAlpha
			
            SetTexture [_MainTex] {
                combine texture * primary, texture
            }
        }
    }
}
