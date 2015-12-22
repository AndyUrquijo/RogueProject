
Shader "Toon/CelShaded" {
	Properties
	{
	_Diffuse("Diffuse", 2D) = "white" {}
	[NoScaleOffset]	_Bump("Bump", 2D) = "bump" {}
	[NoScaleOffset]	_Ramp("Ramp(Unused)", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1)
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		Cull Off
		//Blend Zero One
		CGPROGRAM
		#pragma surface SurfaceFunc CelShaded 
		//finalcolor:FinalColorFunc


		sampler2D _Ramp;
		float3 _Color;
		half Ramp(half value)
		{
			//return tex2D(_Ramp, float2(value,0)).r; // TODO: Fix bug pertaining texture reads

			//float numCells = 3; 
			//int iValue = value * numCells;
			//value = iValue;
			//value /= numCells;
			//return value;
			if (value < 0.33)
				return 0.00;
			if (value < 0.66)
				return 0.5;
			return 1;

		}

		half4 LightingCelShaded(SurfaceOutput surfOut, half3 lightDir, half atten)
		{
			//return half4( 0,0,0,1 );
			half intensity = _LightColor0.r*atten;
			half lightFactor = dot(surfOut.Normal, lightDir);
			//lightFactor = lightFactor*0.5 + 0.5;
			lightFactor *= intensity;
			lightFactor = saturate(lightFactor);
			lightFactor = Ramp(lightFactor);


			//half3 color = surfOut.Albedo*lightFactor*_Color;
			half3 color = float3(1,1,1)*lightFactor;
			return half4(color, lightFactor);
			//return half4(s.Albedo, lightFactor);
		}
		

		struct Input {
			float2 uv_Diffuse;
			float2 uv_Bump;
			float3 viewDir;
		};

		sampler2D _Diffuse;
		sampler2D _Bump;

		void SurfaceFunc(Input IN, inout SurfaceOutput surfOut)
		{
			fixed4 diffuse = tex2D(_Diffuse, IN.uv_Diffuse);

			//surfOut.Albedo = fixed3(1,1,1);
			surfOut.Emission = fixed3(0,0,0);
			surfOut.Albedo = diffuse.rgb;
			//surfOut.Alpha = diffuse.a;

			float3 n = UnpackNormal(tex2D(_Bump, IN.uv_Bump));
			surfOut.Normal = n;
			//surfOut.Normal = dot(IN.viewDir, float3(0, 0, 1)) > 0 ? n : -n;

		}

		
		void FinalColorFunc(Input IN, SurfaceOutput o, inout fixed4 finalColor)
		{
			o.Emission = float3(0,0,0);
			
			float lightFactor = o.Albedo.r;
			lightFactor = Ramp(lightFactor);
			//finalColor.rgb = o.Emission; 
			//finalColor.rgb = o.Albedo;
			finalColor.rgb = tex2D(_Diffuse, IN.uv_Diffuse).rgb*0.5;// *lightFactor;
			finalColor.a = lightFactor;
			//finalColor.rgb = fixed3(1,1,1)*lightFactor;
		}
		
		ENDCG
	}
		FallBack "Diffuse"
}
