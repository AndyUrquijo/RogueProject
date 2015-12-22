Shader "Unlit/CustomDirectionalLight"
{
	SubShader
	{
		Pass
		{
			Fog{ Mode Off }
			ZWrite Off
			ZTest Always
			Blend One One
			//Blend One One

			CGPROGRAM
			#pragma vertex VertexFunction
			#pragma fragment FragmentFunction

			float4 _CustomLightData;
			float4 _CustomLightColor;

			sampler2D _MainTex;
			sampler2D _CameraGBufferTexture0;
			sampler2D _CameraGBufferTexture1;
			sampler2D _CameraGBufferTexture2;

			struct VertexIn
			{
				float4 position	: POSITION;
				float4 uv		: TEXCOORD0;
			};

			struct VertexOut
			{
				float4 position : POSITION;
				float4 uv : TEXCOORD0;
			};

			VertexOut VertexFunction(VertexIn inVert)
			{
				VertexOut outVert;
				outVert.position = mul(UNITY_MATRIX_MVP, inVert.position);
				outVert.uv = inVert.uv;
				return outVert;
			}

			float4 FragmentFunction(VertexOut outVert) : SV_Target
			{
				float3 lightDir;
				lightDir.x = _CustomLightData.x;
				lightDir.y = _CustomLightData.y;
				lightDir.z = _CustomLightData.z;
				lightDir = normalize(lightDir);
				float intensity = _CustomLightData.w;

				half4 gbuffer2 = tex2D(_CameraGBufferTexture2, outVert.uv);



				float3 normal = gbuffer2.rgb * 2 - 1;
				normal = normalize(normal);

				float lightFactor = max(0, dot(-lightDir, normal));
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
