Shader "Cainos/Customizable Pixel Character/Alpha Cut"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		[PerRendererData]_Alpha("Alpha", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		//Добавляем новый параметр для покраснения
		[PerRendererData]_OverlayColor("Overlay Color", Color) = (0,0,0,0)
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "PreviewType"="Plane" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Off
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0

		Pass
		{
			Name "Unlit"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _Alpha;

			//Новый параметр для наложения цвета
			uniform float4 _OverlayColor;

			inline float Dither8x8Bayer( int x, int y )
			{
				const float dither[64] = {
			 1,49,13,61,4,52,16,64,
			 33,17,45,29,36,20,48,32,
			  9,57,5,53,12,60,8,56,
			 41,25,37,21,44,28,40,24,
			  3,51,15,63,2,50,14,62,
			 35,19,47,31,34,18,46,30,
			 11,59,7,55,10,58,6,54,
			 43,27,39,23,42,26,38,22 };
				int r = y * 8 + x;
				return dither[r] / 64;
			}

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_texcoord2 = ComputeScreenPos(o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				float2 uv = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 texColor = tex2D(_MainTex, uv);

				clip(texColor.a - 0.95);

				float4 screenPos = i.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = (UNITY_NEAR_CLIP_VALUE >= 0) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float2 clipScreen = abs(ase_screenPosNorm.xy) * _ScreenParams.xy;
				float dither = Dither8x8Bayer(fmod(clipScreen.x, 8), fmod(clipScreen.y, 8));
				clip(_Alpha - dither);

				float4 finalColor = texColor;

				//Наложение покраснения (основное изменение)
				finalColor.rgb = lerp(finalColor.rgb, _OverlayColor.rgb, _OverlayColor.a);

				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	Fallback Off
}
