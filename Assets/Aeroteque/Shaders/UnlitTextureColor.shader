Shader "Custom/UnlitTextureColor" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Alpha("Alpha", Range(0, 1)) = 1
	}
	
	Category {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
			
		SubShader {
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
			    	float3 normal : NORMAL;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				half _Alpha;
				
				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					fixed3 textureColor = tex2D(_MainTex, i.uv);
					return fixed4(textureColor, _Alpha);
				}
				
				ENDCG
			}	
		}
	}
}
