Shader "ColaFrameWork/MobileUIBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Size("Blur", Range(0, 8)) =1
	}
	
	CGINCLUDE
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		uniform half4 _MainTex_TexelSize;
	ENDCG
	
	SubShader {
	  ZTest Always Cull Off ZWrite Off Blend Off
	  Fog { Mode off }  
	Pass
	  {
		  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"
		  struct appdata_t {
		  float4 vertex : POSITION;
		  float2 texcoord: TEXCOORD0;
	  };

	  struct v2f {
		  float4 vertex : POSITION;
		  float4 uvgrab : TEXCOORD0;
		  float2 uvmain : TEXCOORD1;
	  };

	  //sampler2D _MainTex;
	  float4 _MainTex_ST;
	  v2f vert(appdata_t v) {
		  v2f o;
		  o.vertex = UnityObjectToClipPos(v.vertex);

#if UNITY_UV_STARTS_AT_TOP
		  float scale = -1.0;
#else
		  float scale = 1.0;
#endif

		  o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
		  o.uvgrab.zw = o.vertex.zw;



		  o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);

#if SHADER_API_D3D9
		  if (_MainTex_TexelSize.y < 0.0)
			  o.uvgrab.y = 1.0 - o.uvgrab.y;
#endif

		  return o;
	  }

	  sampler2D _VBlur;
	  float4 _VBlur_TexelSize;
	  float _Size;

	  half4 frag(v2f i) : COLOR
	  {
		 float alpha = tex2D(_MainTex, i.uvmain).a;
		// return half4(alpha, 0, 0, 0);
		 half4 sum = half4(0,0,0,0);
#define GRABPIXEL(weight,kernely) tex2Dproj( _MainTex, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _MainTex_TexelSize.y * kernely * _Size * alpha, i.uvgrab.z, i.uvgrab.w))) * weight
		 sum += GRABPIXEL(0.05, -4.0);
		 sum += GRABPIXEL(0.09, -3.0);
		 sum += GRABPIXEL(0.12, -2.0);
		 sum += GRABPIXEL(0.15, -1.0);
		 sum += GRABPIXEL(0.18,  0.0);
		 sum += GRABPIXEL(0.15, +1.0);
		 sum += GRABPIXEL(0.12, +2.0);
		 sum += GRABPIXEL(0.09, +3.0);
		 sum += GRABPIXEL(0.05, +4.0);
		 sum.rgb *= 0.9;
		 sum.a = 1;
		return sum;
	  }
		  ENDCG
	  }

	  Pass
	  {
		  CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"
		  struct appdata_t {
		  float4 vertex : POSITION;
		  float2 texcoord: TEXCOORD0;
	  };

	  struct v2f {
		  float4 vertex : POSITION;
		  float4 uvgrab : TEXCOORD0;
		  float2 uvmain : TEXCOORD1;
	  };

	  // sampler2D _MainTex;
	  float4 _MainTex_ST;

	  v2f vert(appdata_t v)
	  {
		  v2f o;
		  o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
		  float scale = -1.0;
#else
		  float scale = 1.0;
#endif
		  o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
		  o.uvgrab.zw = o.vertex.zw;

		  o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);

#if SHADER_API_D3D9
		  if (_MainTex_TexelSize.y < 0.0)
			  o.uvgrab.y = 1.0 - o.uvgrab.y;
#endif
		  return o;
	  }

	  sampler2D _HBlur;
	  float4 _HBlur_TexelSize;
	  float _Size;

	  half4 frag(v2f i) : COLOR
	  {
		  float alpha = tex2D(_MainTex, i.uvmain).a;
		  half4 sum = half4(0,0,0,0);
#define GRABPIXEL(weight,kernelx) tex2Dproj( _MainTex, UNITY_PROJ_COORD(float4(i.uvgrab.x + _MainTex_TexelSize.x * kernelx * _Size * alpha, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
		  sum += GRABPIXEL(0.05, -4.0);
		  sum += GRABPIXEL(0.09, -3.0);
		  sum += GRABPIXEL(0.12, -2.0);
		  sum += GRABPIXEL(0.15, -1.0);
		  sum += GRABPIXEL(0.18,  0.0);
		  sum += GRABPIXEL(0.15, +1.0);
		  sum += GRABPIXEL(0.12, +2.0);
		  sum += GRABPIXEL(0.09, +3.0);
		  sum += GRABPIXEL(0.05, +4.0);
		  sum.rgb *= 0.9;
		  sum.a = 1;
		  return sum;
	  }
		  ENDCG
	  }
	}
	//FallBack Off
}
