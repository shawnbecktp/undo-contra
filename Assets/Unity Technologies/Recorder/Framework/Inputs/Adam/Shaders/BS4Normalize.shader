﻿Shader "Hidden/BeautyShot/Normalize" {
	Properties { _MainTex ("Texture", any) = "" {} }
	SubShader { 
		Pass {
 			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;

			float _NormalizationFactor;

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float4 mainTex = tex2D(_MainTex, i.texcoord);
				return mainTex * _NormalizationFactor;
			}
			ENDCG 

		}
	}
	Fallback Off 
}
