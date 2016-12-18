Shader "Hidden/XDebug/WireShader" {
	SubShader {
		Pass {

			Tags { "RenderType"="Transparent" "Queue"="Transparent"  }
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha
			Lighting Off
			Cull Back
			ZWrite Off
			ZTest Greater
			Offset -1, -1
				
			CGPROGRAM
			#pragma vertex vert
            #pragma fragment frag

			fixed4 _Color;
			fixed4 _Up;

			struct appdata {
				float4 vertex : POSITION;
			    fixed4 color : COLOR;
				float2 scale : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

            v2f vert(appdata v) {
				v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex + _Up*v.scale.x);
				o.color = v.color*_Color;
				o.color.a *= 0.1;
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
			Blend SrcAlpha OneMinusSrcAlpha
			Lighting Off
			Cull Back 
			ZWrite Off
			ZTest LEqual
			Offset -1, -1
				
			CGPROGRAM
			#pragma vertex vert
            #pragma fragment frag

			fixed4 _Color;
			fixed4 _Up;


			struct appdata {
				float4 vertex : POSITION;
			    fixed4 color : COLOR;
				float2 scale : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

            v2f vert(appdata v) {
				v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex + _Up*v.scale.x);
				o.color = v.color * _Color;
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
