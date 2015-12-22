Shader "CustomDeferred/TestPostProcess"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Tags{ "Queue" = "Transparent+1" }
		Tags{ "Queue" = "Transparent+1" }
		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"
			#include "Stdfx.cginc"
			
			#pragma vertex VertexFunction
			#pragma fragment FragmentFunction

			struct VertexIn
			{
				float4 position	: POSITION;
				float4 uv		: TEXCOORD0;
			};

			struct VertexOut
			{
				float4 position : SV_POSITION;
				float4 uv		: TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _LightTexture;

			sampler2D _CameraGBufferTexture0;
			sampler2D _CameraGBufferTexture1;
			sampler2D _CameraGBufferTexture2;
			sampler2D _CameraGBufferTexture3;
			sampler2D _BlurredTexture;

			// --- Vertex Shader ----

			VertexOut VertexFunction(VertexIn inVert)
			{
				VertexOut outVert;
				outVert.position = mul(UNITY_MATRIX_MVP, inVert.position);
				outVert.uv = inVert.uv;
				return outVert;
			}

			// --- Fragment Shader ----

			half Ramp(half value)
			{
				//return value;
				if (value < 0.33)
					return 0.3;
				if (value < 0.66)
					return 0.5;
				return 1;

			}

			float4 FragmentFunction(VertexOut outVert) : COLOR
			{
				float4 lightColor = tex2D(_LightTexture, outVert.uv);
				float4 diffuseColor = tex2D(_CameraGBufferTexture0, outVert.uv);
				float lightRamped = Ramp(lightColor.a);
				//lightColor.rgb *= lightColor.a;
				return float4(diffuseColor.rgb*lightColor.rgb*lightRamped, 1);
				
			}


			ENDCG
		}
	}
}
