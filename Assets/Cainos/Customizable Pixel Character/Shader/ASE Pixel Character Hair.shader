Shader "Cainos/Customizable Pixel Character/Hair"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_SkinShadeTex("Skin Shade Tex", 2D) = "white" {}
		_RampTex("Ramp Tex", 2D) = "white" {}
		_HairValueTex("Hair Value Texture", 2D) = "white" {}
		_Alpha("Alpha", Float) = 1
		_OverlayColor("Overlay Color", Color) = (1, 0, 0, 0)
		[HideInInspector] _texcoord("", 2D) = "white" {}
	}

	SubShader
	{
		LOD 0
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

		Cull Off
		Lighting Off
		ZWrite On
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord1 : TEXCOORD1;
			};

			uniform fixed4 _Color;
			uniform float _EnableExternalAlpha;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform sampler2D _HairValueTex;
			uniform float4 _HairValueTex_ST;
			uniform sampler2D _RampTex;
			uniform sampler2D _SkinShadeTex;
			uniform float4 _SkinShadeTex_ST;
			uniform float _Alpha;
			uniform float4 _OverlayColor;

			inline float Dither8x8Bayer(int x, int y)
			{
				const float dither[64] = {
				 1,49,13,61,4,52,16,64,
				33,17,45,29,36,20,48,32,
				 9,57,5,53,12,60,8,56,
				41,25,37,21,44,28,40,24,
				 3,51,15,63,2,50,14,62,
				35,19,47,31,34,18,46,30,
				11,59,7,55,10,58,6,54,
				43,27,39,23,42,26,38,22
				};
				int r = y * 8 + x;
				return dither[r] / 64;
			}

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

				float4 ase_clipPos = UnityObjectToClipPos(IN.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				OUT.ase_texcoord1 = screenPos;

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				float2 uv_HairValueTex = IN.texcoord.xy * _HairValueTex_ST.xy + _HairValueTex_ST.zw;
				float4 tex2DNode7 = tex2D(_HairValueTex, uv_HairValueTex);
				clip(tex2DNode7.a - 0.01);

				float grayscale11 = Luminance(tex2DNode7.rgb);
				#ifdef UNITY_COLORSPACE_GAMMA
				float staticSwitch49 = grayscale11;
				#else
				float staticSwitch49 = pow(grayscale11, 0.32);
				#endif
				float Greyscale13 = staticSwitch49;
				float2 appendResult16 = float2(Greyscale13, Greyscale13);
				float4 color19 = IsGammaSpace() ? float4(0.68,0.52,0.4,1) : float4(0.4200333,0.233022,0.1328683,1);
				float2 uv_SkinShadeTex = IN.texcoord.xy * _SkinShadeTex_ST.xy + _SkinShadeTex_ST.zw;
				float luminance65 = Luminance(tex2D(_SkinShadeTex, uv_SkinShadeTex).rgb);
				float3 temp_cast_2 = (luminance65).xxx;
				half3 linearToGamma68 = temp_cast_2;
				linearToGamma68 = half3(LinearToGammaSpaceExact(linearToGamma68.r), LinearToGammaSpaceExact(linearToGamma68.g), LinearToGammaSpaceExact(linearToGamma68.b));
				float3 temp_cast_3 = (luminance65).xxx;
				#ifdef UNITY_COLORSPACE_GAMMA
				float3 staticSwitch67 = temp_cast_3;
				#else
				float3 staticSwitch67 = linearToGamma68;
				#endif
				float4 lerpResult24 = lerp(tex2D(_RampTex, appendResult16), color19, float4(staticSwitch67, 0.0));
				float4 HairColor22 = lerpResult24;
				float IsShadow31 = (( (tex2DNode7.r - tex2DNode7.g) < 0.01 ? 1.0 : 0.0) * ((tex2DNode7.r - tex2DNode7.b) < 0.01 ? 1.0 : 0.0));
				float4 lerpResult33 = lerp(tex2DNode7, HairColor22, IsShadow31);

				float4 screenPos = IN.ase_texcoord1;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = (UNITY_NEAR_CLIP_VALUE >= 0) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float2 clipScreen76 = abs(ase_screenPosNorm.xy) * _ScreenParams.xy;
				float dither76 = Dither8x8Bayer(fmod(clipScreen76.x, 8), fmod(clipScreen76.y, 8));
				clip(_Alpha - dither76);

				fixed4 c = lerpResult33;
				c.rgb *= c.a;

				//Наложение эффекта покраснения
				c.rgb = lerp(c.rgb, _OverlayColor.rgb, _OverlayColor.a);

				return c;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	Fallback Off
}
