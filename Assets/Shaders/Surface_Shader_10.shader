Shader "ShaderStudy/Surface_Shader_10" 
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		//_SubTex("Sub Tex",2D) = "white" {}
		_Speed ("speed",Range(-1,1)) = 0
	}
		SubShader
	{
	Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
	LOD 200

	CGPROGRAM

	#pragma surface surf Standard alpha:fade
	#pragma target 3.0

	sampler2D _MainTex;
	//sampler2D _SubTex;

	struct Input
	{
		float2 uv_MainTex;
		//float2 uv_SubTex;
	};

	float _Speed;

	void surf(Input IN, inout SurfaceOutputStandard o)
	{
		fixed4 c = tex2D(_MainTex , float2(IN.uv_MainTex.x + (_Time.y * _Speed), IN.uv_MainTex.y));
		//fixed4 d = tex2D(_SubTex, float2(IN.uv_SubTex.x, IN.uv_SubTex.y - _Time.y));

		o.Emission = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}

//무거운 standard 라이팅이 돌아가고 있다
//뒤에 이미지와 겹쳐도 밝아지지않는 불완전한 이펙트

