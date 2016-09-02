Shader "CustomDeferred/ApplyLight"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_RampTexture("Ramp", 2D) = "white" {}
	}

	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Tags{ "Queue" = "Transparent" }
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
			sampler2D _DiffuseTexture;
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


			float SmoothStep(float x0, float x1, float y0, float y1, float x)
			{
				x = saturate((x - x0) / (x1 - x0));
				x = x*x*(3 - 2 * x);
				x = x*(y1 - y0) + y0;
				return x;
			}

			float Ramp(float value)
			{
				//todo PUT THIS IN INSPECTOR
				float borders[2] = { 0.35, 0.78 };
				float values[3] = { 0.17, 0.51, 0.8 };
				float del = 0.005f;

				int i = 0;
				for (; i < 2; i++)
				{
					float x0 = borders[i] - del;
					float x1 = borders[i] + del;
					float y0 = values[i];
					float y1 = values[i+1];
					if (value < x1)
						return SmoothStep(x0, x1, y0, y1, value);
				}

				return values[i];

			}



			// --- Fragment Shader ----

			float4 FragmentFunction(VertexOut outVert) : COLOR
			{
				float4 lightColor = tex2D(_LightTexture, outVert.uv);
				float4 diffuseColor = tex2D(_CameraGBufferTexture0, outVert.uv);
				float4 worldPosColor = tex2D(_CameraGBufferTexture1, outVert.uv);
				float4 emissiveColor = tex2D(_EmissiveTexture, outVert.uv);
				

				float lightRamped = Ramp(lightColor.a);
				float4 normalColor = tex2D(_CameraGBufferTexture2, outVert.uv);

				worldPosColor.rgb *= 0.1;
				
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
