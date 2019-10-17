Shader "Custom/NoiseScreen"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "NoiseScreen"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float random(float2 p)
            {
                const float2 r = float2(
                    23.1406926327792690,  // e^pi (Gelfond's constant)
                    2.6651441426902251); // 2^sqrt(2) (Gelfond–Schneider constant)
                return frac(cos(123456789. % (1e-7 + 256. * dot(p, r))));
            }
            sampler2D _MainTex;
            fixed4 _Color;
            float4 _MainTex_ST;
            uniform float _ElapsedTime;
            uniform float _Intensity;
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color;
                return OUT;
            }
            float lastR;
            float lastG;
            float lastB;

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord)) * IN.color;
                //color = (random(IN.texcoord) + lastR , random(IN.texcoord ) , random(IN.texcoord * _Time[3]), 1);
                color = (0.5,0.2,0.3,1);
                return (0.9,0.2,0.3,0.2);
            }
        ENDCG
        }
    }
}
