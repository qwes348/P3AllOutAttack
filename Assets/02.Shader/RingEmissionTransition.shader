Shader "Oniboogie/RingEmissionTransition"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (1, 1, 1, 1)
        _EmissionIntensity ("Emission Intensity", Float) = 1.0
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _InnerRadius ("Inner Radius", Range(0, 0.5)) = 0.3
        _OuterRadius ("Outer Radius", Range(0, 0.5)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Name "RingEmission"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            ColorMask RGBA

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _EmissionColor;
            float _EmissionIntensity;
            float _AlphaCutoff;
            float _InnerRadius;
            float _OuterRadius;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 centerUV = i.uv - 0.5;
                float dist = length(centerUV);

                if (dist < _InnerRadius || dist > _OuterRadius)
                    discard;

                fixed4 texColor = tex2D(_MainTex, i.uv);
                if (texColor.a < _AlphaCutoff)
                    discard;

                fixed4 emission = _EmissionColor * _EmissionIntensity;
                return emission;
            }
            ENDCG
        }
    }

    FallBack "Unlit/Transparent"
}
