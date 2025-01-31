Shader "S_PixelPolyCity_Alpha_Mobile"
{
	Properties
	{
		_Tint("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("NormalMap", 2D) = "bump"{}
		_RampTex("RampTex", 2D) = "white"{}
		_EmissMap("EmissMap (RGB)", 2D) = "white" {}
		_EmissPow1("EmissPow", Range(0,1)) = 1
		_Cutoff("Cutoff",float) = 0.5
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "AlphaTest"}
			cull off
			LOD 400

			//#pragma surface surf Standard alphatest:_Cutoff vertex:vert addshadow
			CGPROGRAM
			//#pragma surface surf warp noambient 
			#pragma surface surf warp alphatest:_Cutoff 

			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _RampTex;
			sampler2D _EmissMap;
			fixed4 _Tint;
			fixed _EmissPow1;

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float2 uv_EmissMap;
			};



			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 e = tex2D(_EmissMap, IN.uv_EmissMap);
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				o.Albedo = c.rgb * _Tint;
				o.Emission = e.rgb * pow(1 - e.rgb, _EmissPow1);
				o.Alpha = c.a;
			}

			float4 Lightingwarp(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
			{
				//spec turm
				float3 H = normalize(lightDir + viewDir); //빛과 뷰의 중간값을 노말라이즈해서 
				float spec = saturate(dot(s.Normal, H)); // 노말방향과 내적하자

				//rim turm
				float rim = abs(dot(s.Normal, viewDir));

				//ramp turm
				float ndotl = dot(s.Normal, lightDir) * 0.5 + 0.5;
				float4 ramp = tex2D(_RampTex, float2(ndotl, spec));
				//float4 ramp = tex2D(_RampTex, float2(ndotl, rim));
				//float4 ramp = tex2D(_RampTex, float2(s.Normal.x, s.Normal.y));


				float4 final;
				final.rgb = (s.Albedo * ramp.rgb) + (ramp.rgb * 0.1);
				final.a = s.Alpha;
				return final;
			}

			ENDCG
		}
			FallBack "Legacy Shaders/Transparent/Cutout/VertexLit"
}