// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "S_ToonCity_Water"
{
	Properties
	{
		_NormalMap("NormalMap", 2D) = "bump" {}
		_NormalMap1Strength("NormalMap 1 Strength", Float) = 1
		_NormalMap2Strength("NormalMap 2 Strength", Range( 0 , 1)) = 1
		_AnimatUV1XYUV2ZW("Animat UV1 (XY)  UV2 (ZW)", Vector) = (0,0,0,0)
		_UV1TilingXYScaleZW("UV1 Tiling (XY) Scale (ZW)", Vector) = (1,1,1,1)
		_UV2TilingXYScaleZW("UV2 Tiling (XY) Scale (ZW)", Vector) = (0,0,0,0)
		_LerpStrength("Lerp Strength", Range( 0 , 1)) = 0
		_DepthFadeDistance("Depth Fade Distance", Float) = 1
		_MainColor("Main Color", Color) = (0.1966892,0.490566,0.4666131,0)
		_CameraDepthFadeStrength("Camera Depth Fade Strength", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Off
		GrabPass{ }
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float eyeDepth;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _NormalMap1Strength;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthFadeDistance;
		uniform sampler2D _NormalMap;
		uniform float4 _AnimatUV1XYUV2ZW;
		uniform float4 _UV1TilingXYScaleZW;
		uniform float _NormalMap2Strength;
		uniform float4 _UV2TilingXYScaleZW;
		uniform float _LerpStrength;
		uniform float4 _MainColor;
		uniform float _CameraDepthFadeStrength;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float2 appendResult6 = (float2(ase_grabScreenPosNorm.r , ase_grabScreenPosNorm.g));
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth74 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth74 = saturate( abs( ( screenDepth74 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFadeDistance ) ) );
			float depthFade75 = distanceDepth74;
			float3 ase_worldPos = i.worldPos;
			float2 temp_output_20_0 = (ase_worldPos).xz;
			float2 appendResult26 = (float2(( _Time.x * _AnimatUV1XYUV2ZW.x ) , ( _Time.x * _AnimatUV1XYUV2ZW.y )));
			float2 appendResult31 = (float2(_UV1TilingXYScaleZW.x , _UV1TilingXYScaleZW.y));
			float2 appendResult32 = (float2(_UV1TilingXYScaleZW.z , _UV1TilingXYScaleZW.w));
			float2 UV133 = ( ( ( temp_output_20_0 + appendResult26 ) * appendResult31 ) / appendResult32 );
			float2 appendResult36 = (float2(( _Time.x * _AnimatUV1XYUV2ZW.z ) , ( _Time.x * _AnimatUV1XYUV2ZW.w )));
			float2 appendResult42 = (float2(_UV2TilingXYScaleZW.x , _UV2TilingXYScaleZW.y));
			float2 appendResult43 = (float2(_UV2TilingXYScaleZW.z , _UV2TilingXYScaleZW.w));
			float2 UV240 = ( ( ( temp_output_20_0 + appendResult36 ) * appendResult42 ) / appendResult43 );
			float3 lerpResult7 = lerp( UnpackScaleNormal( tex2D( _NormalMap, UV133 ), ( _NormalMap1Strength * depthFade75 ) ) , UnpackScaleNormal( tex2D( _NormalMap, UV240 ), ( _NormalMap2Strength * depthFade75 ) ) , _LerpStrength);
			float3 normalMapping10 = lerpResult7;
			float2 screenUV12 = ( appendResult6 - ( (normalMapping10).xy * 0.1 ) );
			float4 screenColor2 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,screenUV12);
			float3 indirectNormal71 = WorldNormalVector( i , normalMapping10 );
			Unity_GlossyEnvironmentData g71 = UnityGlossyEnvironmentSetup( 0.5, data.worldViewDir, indirectNormal71, float3(0,0,0));
			float3 indirectSpecular71 = UnityGI_IndirectSpecular( data, 1.0, indirectNormal71, g71 );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float2 appendResult64 = (float2(ase_vertexNormal.x , ase_vertexNormal.y));
			float3 appendResult68 = (float3(( appendResult64 - (normalMapping10).xy ) , ase_vertexNormal.z));
			float dotResult55 = dot( ase_worldViewDir , appendResult68 );
			float fresnel61 = pow( ( 1.0 - saturate( abs( dotResult55 ) ) ) , 1.0 );
			float cameraDepthFade81 = (( i.eyeDepth -_ProjectionParams.y - 0.0 ) / _CameraDepthFadeStrength);
			float cameraDepthFade84 = saturate( cameraDepthFade81 );
			float4 lerpResult50 = lerp( screenColor2 , ( float4( indirectSpecular71 , 0.0 ) + _MainColor ) , ( fresnel61 * depthFade75 * cameraDepthFade84 ));
			c.rgb = lerpResult50.rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float1 customPack1 : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.x = customInputData.eyeDepth;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.eyeDepth = IN.customPack1.x;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18000
388;193;1524;702;3633.475;-25.73666;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;45;-3758.057,-180.6201;Inherit;False;1433.753;1170.889;Comment;25;25;26;41;40;22;43;38;37;35;36;34;27;24;31;29;32;30;20;21;42;23;33;39;28;92;Animated UVs;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;27;-3708.056,445.8122;Inherit;False;Property;_AnimatUV1XYUV2ZW;Animat UV1 (XY)  UV2 (ZW);3;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;23;-3647.076,259.3408;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-3385.358,643.5338;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-3389.813,520.863;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-3392.989,237.3026;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-3394.284,352.6786;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;21;-3647.075,75.25781;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;36;-3230.591,569.9568;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;80;-1405.907,-1153.664;Inherit;False;817.8058;190.8356;Comment;3;74;75;77;DepthFade;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;26;-3220.573,251.5626;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;41;-3428.621,778.2689;Inherit;False;Property;_UV2TilingXYScaleZW;UV2 Tiling (XY) Scale (ZW);5;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;30;-3442.971,-130.6201;Inherit;False;Property;_UV1TilingXYScaleZW;UV1 Tiling (XY) Scale (ZW);4;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;20;-3416.095,70.89194;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-3044.951,193.9421;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;42;-3039.142,771.5265;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;31;-3063.552,-116.3731;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;-3040.396,560.7271;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-1376.58,-1096.642;Inherit;False;Property;_DepthFadeDistance;Depth Fade Distance;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;43;-2873.665,851.5374;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-2878.426,560.7272;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;32;-2886.691,-4.421854;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DepthFade;74;-1092.198,-1097.829;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-2882.782,196.4818;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;39;-2694.068,562.0439;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-812.1017,-1103.664;Inherit;False;depthFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;11;-2245.4,362.5364;Inherit;False;1257.012;612.7864;Comment;12;1;9;3;7;47;46;10;48;49;88;89;90;Normal Mapping;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;29;-2704.887,197.6469;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;90;-2131.518,882.4044;Inherit;False;75;depthFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-2226.15,781.8596;Inherit;False;Property;_NormalMap2Strength;NormalMap 2 Strength;2;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-2548.303,555.1639;Inherit;False;UV2;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;-2561.402,191.5104;Inherit;False;UV1;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-2229.18,524.2726;Inherit;False;Property;_NormalMap1Strength;NormalMap 1 Strength;1;0;Create;True;0;0;False;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-1887.898,758.2371;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-1892.513,520.4949;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;47;-2118.197,686.0508;Inherit;False;40;UV2;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-2116.597,432.3693;Inherit;False;33;UV1;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1721.578,856.1736;Inherit;False;Property;_LerpStrength;Lerp Strength;6;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1742.89,412.0646;Inherit;True;Property;_NormalMap;NormalMap;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-1743.372,638.752;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;True;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;7;-1416.221,502.7144;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;62;-2717.656,-843.2936;Inherit;False;2117.954;446.3988;Comment;14;66;53;64;65;61;59;60;58;57;56;55;54;68;70;Fresnel;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-1220.645,497.8452;Inherit;False;normalMapping;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalVertexDataNode;53;-2466.724,-737.8415;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;65;-2512.27,-572.0795;Inherit;False;10;normalMapping;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;66;-2306.561,-573.474;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;64;-2264.587,-713.7439;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;70;-2047.527,-644.4758;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;54;-1884.215,-793.2936;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;68;-1862.278,-620.9288;Inherit;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;13;-2238.686,-261.8214;Inherit;False;1250.297;531.1815;Comment;8;12;5;15;6;4;14;17;19;screen UVs;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;55;-1656.957,-706.1843;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;56;-1508.801,-706.5459;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;85;-1651.453,-1443.934;Inherit;False;1066.445;223.6284;Comment;5;84;81;83;82;87;Camera Depth Fade;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;-2104.365,12.84501;Inherit;False;10;normalMapping;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;57;-1358.001,-706.546;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-1608.447,-1305.56;Inherit;False;Constant;_CameraDepthFadeOffset;Camera Depth Fade Offset;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;19;-1883.685,13.1009;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-1613.153,-1393.934;Inherit;False;Property;_CameraDepthFadeStrength;Camera Depth Fade Strength;9;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;4;-1889.501,-211.8214;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;17;-1844.679,91.90838;Inherit;False;Constant;_constant01;constant0.1;1;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;6;-1627.579,-206.3608;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;58;-1207.201,-703.946;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-1332.001,-607.7459;Inherit;False;Constant;_FresnelPow;Fresnel Pow;7;0;Create;True;0;0;False;0;1;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;81;-1271.246,-1385.698;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1632.047,24.65295;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;5;-1430.15,-206.6478;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;87;-984.9049,-1386.75;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;59;-1022.601,-688.3459;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;72;-905.0142,53.66534;Inherit;False;10;normalMapping;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1241.013,-201.5821;Inherit;False;screenUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;-803.7402,-1390.404;Inherit;False;cameraDepthFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;61;-823.701,-693.546;Inherit;False;fresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;79;-570.0443,500.686;Inherit;False;75;depthFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectSpecularLight;71;-650.3557,57.6612;Inherit;False;Tangent;3;0;FLOAT3;0,0,1;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;51;-632.665,212.02;Inherit;False;Property;_MainColor;Main Color;8;0;Create;True;0;0;False;0;0.1966892,0.490566,0.4666131,0;0.2021627,0.5566038,0.5103326,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;18;-687.1123,-162.199;Inherit;False;12;screenUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;63;-568.9193,404.5084;Inherit;False;61;fresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;-607.9304,596.4804;Inherit;False;84;cameraDepthFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-377.0395,438.387;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;2;-483.156,-161.6237;Inherit;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-337.4359,191.6355;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-2955.614,381.9466;Inherit;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;False;0;100;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;50;-157.6396,166.1527;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;56,-91;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;S_ToonCity_Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;35;0;23;1
WireConnection;35;1;27;4
WireConnection;34;0;23;1
WireConnection;34;1;27;3
WireConnection;24;0;23;1
WireConnection;24;1;27;1
WireConnection;25;0;23;1
WireConnection;25;1;27;2
WireConnection;36;0;34;0
WireConnection;36;1;35;0
WireConnection;26;0;24;0
WireConnection;26;1;25;0
WireConnection;20;0;21;0
WireConnection;22;0;20;0
WireConnection;22;1;26;0
WireConnection;42;0;41;1
WireConnection;42;1;41;2
WireConnection;31;0;30;1
WireConnection;31;1;30;2
WireConnection;37;0;20;0
WireConnection;37;1;36;0
WireConnection;43;0;41;3
WireConnection;43;1;41;4
WireConnection;38;0;37;0
WireConnection;38;1;42;0
WireConnection;32;0;30;3
WireConnection;32;1;30;4
WireConnection;74;0;77;0
WireConnection;28;0;22;0
WireConnection;28;1;31;0
WireConnection;39;0;38;0
WireConnection;39;1;43;0
WireConnection;75;0;74;0
WireConnection;29;0;28;0
WireConnection;29;1;32;0
WireConnection;40;0;39;0
WireConnection;33;0;29;0
WireConnection;88;0;49;0
WireConnection;88;1;90;0
WireConnection;89;0;48;0
WireConnection;89;1;90;0
WireConnection;1;1;46;0
WireConnection;1;5;89;0
WireConnection;3;1;47;0
WireConnection;3;5;88;0
WireConnection;7;0;1;0
WireConnection;7;1;3;0
WireConnection;7;2;9;0
WireConnection;10;0;7;0
WireConnection;66;0;65;0
WireConnection;64;0;53;1
WireConnection;64;1;53;2
WireConnection;70;0;64;0
WireConnection;70;1;66;0
WireConnection;68;0;70;0
WireConnection;68;2;53;3
WireConnection;55;0;54;0
WireConnection;55;1;68;0
WireConnection;56;0;55;0
WireConnection;57;0;56;0
WireConnection;19;0;14;0
WireConnection;6;0;4;1
WireConnection;6;1;4;2
WireConnection;58;0;57;0
WireConnection;81;0;82;0
WireConnection;81;1;83;0
WireConnection;15;0;19;0
WireConnection;15;1;17;0
WireConnection;5;0;6;0
WireConnection;5;1;15;0
WireConnection;87;0;81;0
WireConnection;59;0;58;0
WireConnection;59;1;60;0
WireConnection;12;0;5;0
WireConnection;84;0;87;0
WireConnection;61;0;59;0
WireConnection;71;0;72;0
WireConnection;78;0;63;0
WireConnection;78;1;79;0
WireConnection;78;2;86;0
WireConnection;2;0;18;0
WireConnection;73;0;71;0
WireConnection;73;1;51;0
WireConnection;50;0;2;0
WireConnection;50;1;73;0
WireConnection;50;2;78;0
WireConnection;0;13;50;0
ASEEND*/
//CHKSM=82BEAD715E05B8F0E6A5940A980D6B8D09FF4ED1