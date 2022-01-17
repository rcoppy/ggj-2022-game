// Simplified VertexLit shader. Differences from regular VertexLit one:
// - no per-material color
// - no specular
// - no emission

Shader "Custom3DS/VertexLitWithAO" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_AOTex("Ambient Occlusion (RGB)", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,0.5)
	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 80

		Pass{

			/* BindChannels{
		Bind "Vertex", vertex
		Bind "normal", normal
		Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
		Bind "texcoord", texcoord1 // main uses 1st uv
	}*/
			// see https://docs.unity3d.com/Manual/ShaderTut1.html

			Material{
				Diffuse[_Color]
				Ambient[_Color]
			}
			Lighting On
			SetTexture[_AOTex]{
				constantColor[_Color]
				combine texture * primary DOUBLE, texture * constant
			}
			SetTexture[_MainTex]{
				combine texture * previous //DOUBLE // QUAD
			}

			
		}
	}
}