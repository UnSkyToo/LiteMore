Shader "Lite/CLine"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#pragma target 3.0

	float4 _Color;

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;
		return o;
	}

	float4 frag(v2f i) : SV_TARGET
	{
		fixed4 wave_color = fixed4(0, 0, 0, 0);

		// TO create the waves   
		float wave_width = 0.01f;
		float ux = i.uv.x * 2.0f - 1.0f;
		float uy = i.uv.y * 2.0f - 1.0f;
		//uv.y += 0.1;
		//for (float i = 0.0f; i < 10.0f; i++)
		//{
			uy += (0.07f * sin(ux + 1/*i*/ / 7.0f + _Time.y * 10));
			wave_width = abs(1.0f / (150.0f * uy));
			wave_color += fixed4(wave_width * 1.9f, wave_width, wave_width * 1.5f, 0);
		//}

		return wave_color;
	}
	ENDCG

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
	}
	FallBack "Diffuse"
}