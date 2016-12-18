Shader "Hidden/XDebug/PreviewShader" {
	Properties {
		_TintColor ("Color Tint", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}

	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200
		Blend SrcAlpha One
		Lighting Off
		Cull Off
		ZWrite Off
		
		CGPROGRAM
		#pragma surface surf Lambert keepalpha

 		sampler2D _MainTex;
		half4 _TintColor;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half gray =  c.r*0.2989+c.g+ 0.5870*c.b*0.1140;
			
			gray = gray *0.5;

			//o.Albedo =  gray * 0.5;
			o.Emission = _TintColor.rgb * gray*_TintColor.a;
			o.Alpha = c.a*clamp(_TintColor.a,0.125,1);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
