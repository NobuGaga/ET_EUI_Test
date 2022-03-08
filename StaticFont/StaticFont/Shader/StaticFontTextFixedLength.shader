//FixedLength 表示不用顶点色
//顶点色.R 表示需要顶点位移出去
Shader "StaticFont/StaticFontTextFixedLength"
{
    Properties
    {
		_Color("Color Tint",color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_Alpha("Alpha", float) = 1
    }
    SubShader
    {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "ignoreprojector" = "true" }
		ZWrite Off
		Cull Front
		blend srcalpha oneminussrcalpha

        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			//#pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				half4 vcolor : color;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				half4 vcolor : TEXCOORD1;
                float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			fixed4 _Color;

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(fixed, _Alpha)
			UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				half offsetY = 100000 * v.vcolor.r;
				v.vertex.y += offsetY;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.vcolor = v.vcolor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed alpha = UNITY_ACCESS_INSTANCED_PROP(Props, _Alpha);
                fixed4 col = tex2D(_MainTex, i.uv);
				col.a *= alpha * i.vcolor.a;

                return col;
            }
            ENDCG
        }
    }
}
