Shader "Custom/Transent" {
	Properties {
		_Alpha("Alpha",Range(0,1)) = 0
	}
	SubShader{
		Tags { "RenderType" = "Transparent" "Opaque" = "Transparent" }
		//LOD 200
		Pass
		{
		Cull OFF
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		struct a2v
		{
			float4 pos : POSITION;
			fixed4 color : COLOR;
		};
		struct v2f
		{
			float4 vertex : SV_POSITION;
			fixed4 color : COLOR;
		};
		v2f vert(a2v i)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, i.pos);
			o.color = i.color;
			return o;
		}
		fixed4 frag(v2f i) : COLOR
		{
			return i.color;
		}
		ENDCG
		}
	}
}
