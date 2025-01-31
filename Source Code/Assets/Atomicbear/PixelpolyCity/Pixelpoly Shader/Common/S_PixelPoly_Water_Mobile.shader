Shader "S_PixelPolyCity_Water_Mobile"
{
	Properties
	{
		_BumpMap("NormalMap", 2D) = "bump" {}
		_Cube("Cube", Cube) = "" {}
		_SPColor("Specular Color", Color) = (1,1,1,1)
		_SPPower("Specular Power", Range(50,300)) = 150
		_SPMulti("Specular Multiply", Range(1,10)) = 3
		_WaveH("Wave Height", Range(0,0.5)) = 0.1
		_WaveL("Wave Length", Range(5,20)) = 12
		_WaveT("Wave Timeing", Range(0,10)) = 1
		_Refract("Refract Strength", Range(0,0.2)) = 0.1
	}
		SubShader
		{
			//Tags { "RenderType"="Transparnet" "Queue" = "Transparent"}
			Tags {"RenderType" = "Opaque"}

			GrabPass{} //캡쳐하겠음!

			CGPROGRAM
			#pragma surface surf WaterSpecular alpha:fade vertex:vert

			samplerCUBE _Cube;
			sampler2D _BumpMap;
			sampler2D _GrabTexture; //캡쳐 샘플링
			float4 _SPColor;
			float _SPPower;
			float _SPMulti;
			float _WaveH;
			float _WaveL;
			float _WaveT;
			float _Refract;

			void vert(inout appdata_full v)
			{
				float movement;
				movement = sin(abs((v.texcoord.x * 2 - 1) * _WaveL) + _Time.y * _WaveT) * _WaveH;
				movement += sin(abs((v.texcoord.y * 2 - 1) * _WaveL) + _Time.y * _WaveT) * _WaveH;
				v.texcoord.y += movement / 2;
				//v.texcoord.xy += (movement/2, movement/2);
				//v.texcoord.xy = (movement/2, movement/2);
			}

			struct Input
			{
				float2 uv_BumpMap;
				float3 worldRefl; //월드리플렉션 데이터
				float3 viewDir;
				float4 screenPos; //캡쳐 스크린픽셀 포지션
				INTERNAL_DATA // 버텍스노말 데이터를 픽셀노말 데이터로 변환시킵니다. 
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				//o.Normal = UnpackNormal(tex2D(_BumpMap, float2(IN.uv_BumpMap.x, IN.uv_BumpMap.y + _Time.y*0.1 )));
				float3 normal1 = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap + _Time.x * 0.1));
				float3 normal2 = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap - _Time.x * 0.1));
				o.Normal = (normal1 + normal2) / 2;

				float3 refcolor = texCUBE(_Cube, WorldReflectionVector(IN, o.Normal)); //월드리플렉션 데이터를 노멀맵에 반사해서 물처럼 보이게 하자 

				//refraction term
				float3 screenUV = IN.screenPos.rgb / IN.screenPos.a;
				float3 refraction = tex2D(_GrabTexture, (screenUV.xy + o.Normal.xy * _Refract));

				//rim turm
				float rim = saturate(dot(o.Normal, IN.viewDir));
				rim = pow(1 - rim, 1.5);

				o.Emission = (refcolor * rim + refraction) * 0.5;
				//o.Alpha = saturate(rim+0.5);
				o.Alpha = 1;
			}

			float4 LightingWaterSpecular(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
			{
				//specular term
				float3 H = normalize(lightDir + viewDir);
				float spec = saturate(dot(H, s.Normal));
				spec = pow(spec, _SPPower);

				//final term
				float4 finalColor;
				finalColor.rgb = spec * _SPColor.rgb * _SPMulti;
				finalColor.a = s.Alpha + spec;

				return finalColor;
			}

			ENDCG
		}
			FallBack "Legacy Shaders/Transparent/VertexLit"
}
