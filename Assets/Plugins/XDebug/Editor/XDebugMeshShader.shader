Shader "Hidden/XDebug/MeshShader" {
	SubShader{
		Tags{ "RenderType" = "Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent" }

		Pass {

			Blend SrcAlpha One
			LOD 200
			Lighting Off
			Cull Back
			ZWrite Off
			ZTest Greater
			//Offset - 1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Color;
			fixed3 _LightColor;
			fixed4 _LightDir;
			fixed4 _Up;

			struct appdata {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				fixed4 color : COLOR;
				float2 scale : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

			v2f vert(appdata v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex + _Up*v.scale.x);

				float3 normal = normalize(mul(UNITY_MATRIX_MVP, v.normal));
				float3 lightDir = normalize(mul(UNITY_MATRIX_VP, _LightDir));
				float lit = dot(normal, -lightDir);

				fixed4 color = _Color;
				fixed3 lightColor = _LightColor.rgb;
				lightColor = lightColor*(lit * 2 - 1);
				o.color.rgb = (lightColor + color.rgb) *  v.color.rgb;

				o.color.a = color.a* 0.1;

				return o;

			}

			fixed4 frag(v2f i) : SV_Target {
				return i.color;
			}
				ENDCG
		}


		Pass{
				
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200
			Lighting Off
			Cull Back
			ZWrite Off
			ZTest LEqual

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
				fixed4 color : COLOR;
				float2 scale : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

            v2f vert(appdata v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex + _Up*v.scale.x);
				
				//float3 normal = normalize(mul(_Object2World, v.normal));
				//float3 lightDir = _LightDir;// normalize(mul(UNITY_MATRIX_VP, _LightDir));

				float3 normal = normalize(mul((float3x3)UNITY_MATRIX_MV, v.normal));
				float3 lightDir = normalize(mul((float3x3)UNITY_MATRIX_V, _LightDir));

				float lit = dot(normal, -lightDir);
				fixed4 color = _Color;
				fixed3 lightColor = _LightColor.rgb;
				lightColor = lightColor*(lit - 0.5);
				o.color.rgb = (color.rgb + lightColor)*v.color.rgb;
				o.color.a = color.a;
				//o.color.rgb = normal;

				return o;

            }

            fixed4 frag(v2f i) : SV_Target {
                return i.color;
            }
			ENDCG
		} 
	}

	//FallBack "Off"
}
