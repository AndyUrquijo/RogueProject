Shader "CustomDeferred/TestPostProcess"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_RampTexture("Ramp", 2D) = "white" {}
	}

	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Tags{ "Queue" = "Transparent+1" }
		//Tags{ "Queue" = "Transparent+1" }
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
			sampler2D _EmissiveTexture;
			sampler2D _RampTexture;

			sampler2D _CameraGBufferTexture0;
			sampler2D _CameraGBufferTexture1;
			sampler2D _CameraGBufferTexture2;
			sampler2D _CameraGBufferTexture3;

			// --- Vertex Shader ----

			VertexOut VertexFunction(VertexIn inVert)
			{
				VertexOut outVert;
				outVert.position = mul(UNITY_MATRIX_MVP, inVert.position);
				outVert.uv = inVert.uv;
				return outVert;
			}

			// --- Fragment Shader ----

			float4 FragmentFunction(VertexOut outVert) : COLOR
			{
				float4 lightColor = tex2D(_LightTexture, outVert.uv);
				float4 diffuseColor = tex2D(_CameraGBufferTexture0, outVert.uv);
				float4 emissiveColor = tex2D(_EmissiveTexture, outVert.uv);
				float lightRamped = tex2D(_RampTexture, float2(lightColor.aa)).r;
				float4 normalColor = tex2D(_CameraGBufferTexture2, outVert.uv);
				//return normalColor;
				//lightColor.rgb *= lightColor.a;
				//return float4(emissiveColor.rgb, 1);
				//return float4(diffuseColor.aaa, 1);
				//return float4(diffuseColor.rgb, 1);
				float4 finalColor;
				finalColor.a = 1;
				finalColor.rgb = diffuseColor.rgb;
				finalColor.rgb *= lightColor.rgb;
				finalColor.rgb *= lightRamped;
				
				
				float alpha = diffuseColor.a;
				finalColor = finalColor*alpha + emissiveColor*(1-alpha);
				
				return finalColor;
				
			}


			ENDCG
		}
	}
}
