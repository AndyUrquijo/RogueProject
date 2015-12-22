Shader "Toon/CelShaded2"
{
	Properties
	{
		_MainTex("Diffuse", 2D) = "white" {}
		[NoScaleOffset] _Ramp("Ramp", 2D) = "white" {}
	}
	
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
			//Blend One One
			BlendOp Max
			CGPROGRAM
			#pragma vertex vertexFunc
			#pragma fragment fragmentFunc 
			struct VertexIn
			{
				float4 position : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct VertexOut
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				float3 normal : NORMAL;	// World Space
			};

			struct FragOut
			{
				fixed4 color : SV_TARGET;
			};

			sampler2D _MainTex;
			sampler2D _Ramp;

			VertexOut vertexFunc(VertexIn inVert)
			{
				VertexOut outVert;
				outVert.position = mul(UNITY_MATRIX_MVP, inVert.position);
				outVert.worldPosition = mul(_Object2World, inVert.position);
				outVert.uv = inVert.uv;
				outVert.normal = mul(_Object2World, float4(inVert.normal,0)).xyz;
				return outVert;
			}

			FragOut fragmentFunc(VertexOut outVert)
			{
				FragOut outFrag;
				fixed3 diffuse = tex2D(_MainTex, outVert.uv).rgb;

				float lightFactor;
				outFrag.color.a = 1.0;

				if (_WorldSpaceLightPos0.w == 0) // directional light
				{
					lightFactor = 0;
				}
				else
				{
					fixed3 lightPos = _WorldSpaceLightPos0.xyz;
					fixed3 toLight = normalize(lightPos - outVert.worldPosition);
					lightFactor = dot(outVert.normal, toLight);
					lightFactor = lightFactor*0.5 + 0.5;
					lightFactor = tex2D(_Ramp, float2(lightFactor, 0)).r;
					outFrag.color.a = lightFactor;
				}

				outFrag.color.rgb = diffuse*lightFactor;
				return outFrag;
			}
		ENDCG
	}



	}
}
