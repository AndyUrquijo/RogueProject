Shader "Unlit/CustomPointLight"
{
	SubShader
	{
		Tags{ "RenderType" = "Transparency" }
		CGINCLUDE
#include "UnityCG.cginc"
#include "UnityDeferredLibrary.cginc"

		float4 _CustomAttenuation;
		float4 _CustomLightData;
		float4 _CustomLightColor;

		float3 ConstructWorldPos(float2 uv, float3 toCam)
		{
			// read depth and reconstruct world position
			float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
			depth = Linear01Depth(depth);
			float4 vpos = float4(toCam * depth, 1);
			return mul(_CameraToWorld, vpos).xyz;
		}
		ENDCG

		Pass
		{
			Fog{ Mode Off }
			ZWrite Off
			ZTest Always
			Blend One One
			Cull Front

			CGPROGRAM
			#pragma vertex VertexFunction
			#pragma fragment FragmentFunction
			
			#include "UnityCG.cginc"
			#include "Stdfx.cginc"


			sampler2D _MainTex;
			sampler2D _CameraGBufferTexture0;
			sampler2D _CameraGBufferTexture1;
			sampler2D _CameraGBufferTexture2;

			struct LightVertexOut
			{ 
				float4 position : POSITION;
				float4 uv : TEXCOORD0;
				float3 toCam : NORMAL;	// Vert to Camera
			};
			
			LightVertexOut VertexFunction(float4 position : POSITION)
			{
				LightVertexOut outLVert;
				outLVert.position = mul(UNITY_MATRIX_MVP, position);
				outLVert.uv = ComputeScreenPos(outLVert.position);
				outLVert.toCam = mul(UNITY_MATRIX_MV, position).xyz * float3(-1, -1, 1);
				return outLVert;
			}
			
			float4 FragmentFunction(LightVertexOut outLVert) : SV_Target
			{
				float attC = _CustomAttenuation.x;
				float attL = _CustomAttenuation.y;
				float attQ = _CustomAttenuation.z;
				float pwr =  _CustomAttenuation.w;

				float intensity = _CustomLightData.x;
				float range = _CustomLightData.y;

				outLVert.toCam = outLVert.toCam * (_ProjectionParams.z / outLVert.toCam.z);
				float2 uv = outLVert.uv.xy / outLVert.uv.w;

				half4 gbuffer2 = tex2D(_CameraGBufferTexture2, uv);

				float3 lightPos = float3(_Object2World[0][3], _Object2World[1][3], _Object2World[2][3]);
				float3 pixelPos = ConstructWorldPos(uv, outLVert.toCam);
				float3 toLight = lightPos - pixelPos;

				float3 normal = gbuffer2.rgb*2 - 1;
				normal = normalize(normal);

				float3 toLightN = normalize(toLight);
				float lightFactor = max(0, dot(toLightN, normal) );
			
				float dist = length(toLight)/ range;
				float att = (1 - pow(dist, pwr)) / (attC + attL*dist + attQ*dist*dist);

				lightFactor *= att;
				lightFactor *= intensity;

				float4 lightColor;
				lightColor.rgb = _CustomLightColor.rgb;
				lightColor.rgb *= lightFactor;
				lightColor.a = lightFactor;
				return lightColor;
			}
			ENDCG
		}
	}
}
