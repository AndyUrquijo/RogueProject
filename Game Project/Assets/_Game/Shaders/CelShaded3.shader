Shader "Toon/CelSahded3" {
   Properties {
   	_MainTex ("Diffuse", 2D) = "white" {}
    [NoScaleOffset]	_BumpMap ("Normal", 2D) = "bump" {}
   }

	CGINCLUDE // common code for all passes of all subshaders

	#include "UnityCG.cginc"
	uniform float4 _LightColor0; 
	// color of light source (from "Lighting.cginc")

	// User-specified properties
	uniform sampler2D _MainTex;   
	uniform float4 _MainTex_ST;
	uniform sampler2D _BumpMap;   

	struct vertexInput 
	{
		 float4 vertex : 	POSITION;
		 float4 texcoord : 	TEXCOORD0;
		 float3 normal : 	NORMAL;
		 float4 tangent : 	TANGENT;
	};
	struct vertexOutput 
	{
		 float4 pos : 		SV_POSITION;
		 float4 posWorld : 	TEXCOORD0;
		 float4 tex : 		TEXCOORD1;
		 float3 tangent : 	TEXCOORD2;  
		 float3 normal : 	TEXCOORD3;
		 float3 binormal :	TEXCOORD4;
	};

	struct vertexInputTex 
	{
		 float4 vertex : 	POSITION;
		 float4 texcoord : 	TEXCOORD0;
	};
	struct vertexOutputTex
	{
		 float4 pos : 		SV_POSITION;
		 float4 tex : 		TEXCOORD0;
	};
	
	vertexOutput VertexNormalsFunction(vertexInput input) 
	{
		vertexOutput output;

		output.tangent = 	normalize( mul(_Object2World, float4(input.tangent.xyz, 0)).xyz );
		output.normal = 	normalize( mul(float4(input.normal, 0), _World2Object).xyz );
		output.binormal =	normalize( cross(output.normal, output.tangent) * input.tangent.w ); 

		output.posWorld = 	mul(_Object2World, input.vertex);
		output.tex = 		input.texcoord;
		output.pos = 		mul(UNITY_MATRIX_MVP, input.vertex);
		return output;
	}
	
	vertexOutputTex VertexTexFunction(vertexInputTex input) 
	{
		vertexOutputTex output;
		output.tex = 		input.texcoord;
		output.pos = 		mul(UNITY_MATRIX_MVP, input.vertex);
		return output;
	}

	// fragment shader with ambient lighting
	float4 LitFragmentFunction(vertexOutput input) : COLOR
	{
		float2 texUV = input.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw;

		float4 diffuseColor = tex2D(_MainTex, texUV);

		float4 encodedNormal = tex2D(_BumpMap, texUV);
		float3 localCoords = float3(2 * encodedNormal.a - 1, 
			2 * encodedNormal.g - 1, 0.0);
		localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));
		// approximation without sqrt:  localCoords.z = 
		// 1.0 - 0.5 * dot(localCoords, localCoords);

		float3x3 WorldTranspose = float3x3(
		normalize(input.tangent), 
		normalize(input.binormal), 
		normalize(input.normal));
		float3 normalDirection = normalize(mul(localCoords, WorldTranspose));

		float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
		float3 lightDirection;
		float attenuation;

		if (0.0 == _WorldSpaceLightPos0.w) // directional light?
		{ 
			attenuation = 1.0; // no attenuation
			lightDirection = normalize(_WorldSpaceLightPos0.xyz);
		} 
		else // point or spot light
		{
			float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
			float distance = length(vertexToLightSource);
			attenuation = 1.0 / distance; // linear attenuation 
			lightDirection = normalize(vertexToLightSource);
		}

		float lightFactor = attenuation * max(0, dot(normalDirection, lightDirection));
		lightFactor *= dot( float3(0.3, 0.4, 0.3), _LightColor0.rgb );

		return float4(lightFactor, lightFactor, lightFactor, lightFactor);
	}
      
   	// fragment shader with ambient lighting
	float4 TexFragmentFunction(vertexOutputTex input) : COLOR
	{
		float2 texUV = input.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw;

		float3 diffuseColor = tex2D(_MainTex, texUV).rgb;

		return float4(diffuseColor, 1.0);
	}
      
   
   ENDCG


	SubShader 
	{
		Pass 
		{      
			Tags { "LightMode" = "ForwardBase" } 
			// pass for ambient light and first light source

			CGPROGRAM
			#pragma vertex VertexNormalsFunction  
			#pragma fragment LitFragmentFunction  
			ENDCG
		}

		Pass 
		{      
			Tags { "LightMode" = "ForwardAdd" } 
			// pass for additional light sources
			Blend One One // additive blending 

			CGPROGRAM
			#pragma vertex VertexNormalsFunction  
			#pragma fragment LitFragmentFunction
			ENDCG
		}
		Pass 
		{      
			Tags { "LightMode" = "Always" } 
			// pass to ramp lighting and apply diffuse texture
			//Blend  Zero One// replace 
			//Blend  One Zero// replace 
			Blend DstAlpha Zero// replace 
			// TODO:
			// Currently the previous passes are writing to the same render target as the final color ramp
			// However The resulting color cannot read directly or be used other than with blending operations.
			// Therefore, writing to a separate buffer will be required. 
			// This buffer wil be used to store a single float with the light brightness information 
			CGPROGRAM
			#pragma vertex VertexTexFunction  
			#pragma fragment TexFragmentFunction
			ENDCG
		}
	}
}