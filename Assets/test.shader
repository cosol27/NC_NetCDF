Shader "Custom/test"
{
	SubShader
	{
		Pass
	{
		CGPROGRAM

#pragma vertex Vert

#pragma fragment Frag

#include "UnityCG.cginc"

		struct V2F
	{
		float4 pos:POSITION;

		//float2 txr1:TEXCOORD0;

		half4 vertexColor:COLOR0;

	};


	V2F Vert(appdata_full v)
	{

		V2F output;

		output.pos = mul(UNITY_MATRIX_MVP,v.vertex);

		//output.txr1 = v.texcoord;

		output.vertexColor = v.color;

		return output;

	}


	half4 Frag(V2F i) :COLOR

	{
		return i.vertexColor;
	}


		ENDCG
	}
	}
		FallBack "Diffuse"
}
