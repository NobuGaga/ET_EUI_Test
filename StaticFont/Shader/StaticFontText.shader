Shader "StaticFont/StaticFontText"
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
		Cull Off
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha

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
				half4 vcolor : COLOR;
				float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				half4 vcolor : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			half _Alpha;
			fixed4 _Color;


            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.vcolor = v.vcolor * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed alpha = _Alpha;//UNITY_ACCESS_INSTANCED_PROP(Props, _Alpha);
                fixed4 col = tex2D(_MainTex, i.uv);
				col.a *= alpha;
				col *= i.vcolor;

                return col;
            }
            ENDCG
        }
    }
}
