Shader "Hidden/XDebug/ColliderShader" {
	SubShader {
		Pass {

			Tags { "RenderType"="Transparent" "Queue"="Transparent"  }
			LOD 200
			Blend One One
			Lighting Off
			Cull Back
			ZWrite Off
			ZTest Greater
			Offset -1, -1
				
			CGPROGRAM
			#pragma vertex vert
            #pragma fragment frag


			fixed4 _Color;
			fixed3 _LightColor;
			fixed3 _LightDir;
			fixed4 _Up;

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 scale : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

            v2f vert(appdata v) {
				v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex + _Up*v.scale.x);
				float3 normal = normalize(mul((float3x3)UNITY_MATRIX_MV, v.normal));
				float3 lightDir = normalize(mul((float3x3)UNITY_MATRIX_V, _LightDir));
				float lit = dot(normal, -lightDir);

				fixed4 color = _Color;
				fixed3 lightColor = _LightColor.rgb;
				lightColor = lightColor*(lit * 2 - 1);
				o.color.rgb = (lightColor + color.rgb) * color.a*0.1;

				o.color.a =color.a * 0.1;
				return o;

            }

            fixed4 frag(v2f i) : SV_Target {
                return i.color;
            }
			ENDCG
		} 
		Pass {

			Tags { "RenderType"="Transparent" "Queue"="Transparent"  }
			LOD 200
			Blend One One
			Lighting Off
			Cull Back 
			ZWrite Off
			ZTest LEqual
			Offset -1, -1
				
			CGPROGRAM
			#pragma vertex vert
            #pragma fragment frag



			fixed4 _Color;
			fixed3 _LightColor;
			fixed3 _LightDir;
			fixed4 _Up;

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 scale : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

            v2f vert(appdata v) {
				v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex + _Up*v.scale.x);

				float3 normal = normalize(mul((float3x3)UNITY_MATRIX_MV, v.normal));
				float3 lightDir = normalize(mul((float3x3)UNITY_MATRIX_V, _LightDir));
				float lit = dot(normal, -lightDir);

				fixed4 color = _Color;
				fixed3 lightColor = _LightColor.rgb;
				lightColor = lightColor*(lit*2 - 1);
				o.color.rgb = (lightColor + color.rgb) * color.a;
				o.color.a = 1.0;

				//o.color.rgb = fixed3(1, 0, 0);

				return o;

            }

            fixed4 frag(v2f i) : SV_Target {
                return i.color;
            }
			ENDCG
		} 
	}

	FallBack "Off"
}
