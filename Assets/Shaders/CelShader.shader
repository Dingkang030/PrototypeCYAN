Shader "Custom/CelShadingForward" {
	Properties{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Normal("Normal (RGB)", 2D) = "bump" {}
		_AOcclusion("AOcclusion (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{
		"RenderType" = "Opaque"
	}
		LOD 200

		CGPROGRAM
		#pragma surface surf CelShadingForward
		#pragma target 3.0

	half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten) {
		half NdotL = dot(s.Normal, lightDir);
		if (NdotL <= 0.0) NdotL = 0;
		else NdotL = 1;
		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
		c.a = s.Alpha;
		return c;
	}

	sampler2D _MainTex;
	sampler2D _Normal;
	sampler2D _AOcclusion;
	fixed4 _Color;

	struct Input {
		float2 uv_MainTex;
		float2 uv_Normal;
		float2 uv_AOcclusion;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb*tex2D(_AOcclusion, IN.uv_AOcclusion).a * _Color;
		o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_Normal));
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}