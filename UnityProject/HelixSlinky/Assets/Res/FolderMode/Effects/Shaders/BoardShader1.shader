Shader "HelixSlinky/BoardShader1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("color",COLOR) = (1,1,1,1)
		_Radius ("半径",float) = 1
		_Frequency("Frequency",float) = 1
		_Speed("speed",float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
			#include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal :NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD1;
				float4 worldVertex :TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			uniform float4 hitPoint;
			float _Radius;
			uniform float _WaveScal;
			float _Frequency, _Speed;
			fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

				float4 worldHitPoint = hitPoint;
				float4 vertexWorldPos = mul(unity_ObjectToWorld, v.vertex);
				float dis = abs(length(worldHitPoint.xy - vertexWorldPos.xy));
				v.vertex.z += sin(dis * _Frequency + _Time.y * _Speed) * _WaveScal;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = mul(unity_ObjectToWorld, v.normal);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				//归一化法线，即使在vert归一化也不行，从vert到frag阶段有差值处理，传入的法线方向并不是vertex shader直接传出的
				fixed3 worldNormal = normalize(i.worldNormal);
				//把光照方向归一化
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				//根据兰伯特模型计算像素的光照信息，小于0的部分理解为看不见，置为0
				fixed3 lambert = max(0.0, dot(worldNormal, worldLightDir));
				//最终输出颜色为lambert光强*材质diffuse颜色*光颜色
				fixed3 diffuse = lambert * _LightColor0.xyz;

				fixed4 col = tex2D(_MainTex, i.uv);

				col.rgb *= diffuse;
				col.rgb *= _Color;
                return col;
            }
            ENDCG
        }
    }
}
