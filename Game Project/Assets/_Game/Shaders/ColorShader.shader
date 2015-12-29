Shader "CustomDeferred/ColorShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_AlphaCutOff("Alpha Cutoff", Range(0.0,1.0)) = 0.0
		[NoScaleOffset] _Bump("Normal", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range(0.0,1.0)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Off
		Pass
		{
			Tags{ "LightMode" = "Deferred" }
			CGPROGRAM

			

			#include "UnityCG.cginc"
			#include "Stdfx.cginc"

			#pragma vertex VertexFunction
			#pragma fragment FragmentFunction
			
			struct VertexIn
			{
				float4 position	: POSITION;
				float4 uv		: TEXCOORD0;
				float3 normal	: NORMAL;
				float4 tangent	: TANGENT;
			};

			struct VertexOut
			{
				float4 position : SV_POSITION;
				float4 uv		: TEXCOORD0;
				float3 normal	: TEXCOORD1;
				float3 tangent	: TEXCOORD2;
				float3 binormal : TEXCOORD3;
			};

			struct FragOut
			{
				float4 diffuse	: SV_Diffuse;
				float4 specular	: SV_Specular;
				float4 normal	: SV_Normal;
				float4 emission	: SV_Emission;
			};


			// --- Globals ----

			sampler2D _MainTex;
			sampler2D _Bump;
			float4 _MainTex_ST;
			float4 _Color;
			float _AlphaCutOff;
			float _NormalScale;
			// --- Vertex Shader ----

			VertexOut VertexFunction(VertexIn inVert)
			{
				VertexOut outVert;
				
				outVert.position = mul(UNITY_MATRIX_MVP, inVert.position);

				outVert.normal = mul(float4(inVert.normal, 0), _World2Object).xyz;
				outVert.tangent = mul(_Object2World, float4(inVert.tangent.xyz, 0)).xyz; 
				outVert.binormal = cross(outVert.normal, outVert.tangent)* inVert.tangent.w;

				outVert.normal = normalize(outVert.normal);
				outVert.tangent = normalize(outVert.tangent);
				outVert.binormal = normalize(outVert.binormal);
				
				outVert.uv = inVert.uv;

				return outVert;
			}

			// --- Fragment Shader ----

			FragOut FragmentFunction(VertexOut outVert)
			{
				FragOut outFrag;

				float2 uv = _MainTex_ST.xy * outVert.uv + _MainTex_ST.zw;
				
				outFrag.diffuse = tex2D(_MainTex, uv)*_Color;
				if( outFrag.diffuse.a < _AlphaCutOff )
					discard;
				outFrag.diffuse.a = 1;
				
				float4 encodedNormal = tex2D(_Bump, uv); // Unity's normals come encoded in the ga channels
				float3 localNormal = float3(2*encodedNormal.ag - 1, 0);
				localNormal.z = sqrt(1 - dot(localNormal, localNormal));

				localNormal = localNormal*_NormalScale + float3(0, 0, 1)*(1-_NormalScale);
				
				float3x3 local2WorldTranspose = float3x3(outVert.tangent, outVert.binormal,	outVert.normal);
				float3 worldNormal = mul(localNormal, local2WorldTranspose);
				//worldNormal = normalize(worldNormal);
				worldNormal = worldNormal*0.5 + 0.5;
				outFrag.normal = half4(worldNormal,1);

				outFrag.specular = half4(0,0,0,0);
				outFrag.emission = half4(0,0,0,0);
				return outFrag;
			}

			ENDCG
		}
	}
}
