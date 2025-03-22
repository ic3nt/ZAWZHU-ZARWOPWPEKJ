Shader "UI/UGUI_AnimatedOutline_URP"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Range(0,10)) = 2
        _AnimationSpeed ("Animation Speed", Range(0,5)) = 1
        _ColorShiftSpeed ("Color Shift Speed", Range(0,5)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "OutlinePass"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _AnimationSpeed;
            float _ColorShiftSpeed;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 offset = float2(_OutlineThickness, _OutlineThickness) / _ScreenParams.xy;
                float timeFactor = sin((_Time.y) * _AnimationSpeed) * 0.5 + 0.5;

                float4 texColor = tex2D(_MainTex, i.uv);
                float alpha = texColor.a;

                // Вычисляем края (снаружи) без затрагивания центра
                float outline = 0.0;
                outline += tex2D(_MainTex, i.uv + float2(-offset.x, 0)).a;
                outline += tex2D(_MainTex, i.uv + float2(offset.x, 0)).a;
                outline += tex2D(_MainTex, i.uv + float2(0, -offset.y)).a;
                outline += tex2D(_MainTex, i.uv + float2(0, offset.y)).a;
                outline = smoothstep(0.0, 0.1, outline - alpha) * timeFactor;

                // Glow (мягкое свечение)
              //   float glow = _GlowIntensity;

                // Плавная смена цвета
                float3 colorShift = lerp(_OutlineColor.rgb, float3(1,0,0), sin(_Time.y * _ColorShiftSpeed) * 0.5 + 0.5);

                // Итоговый цвет (только внешняя обводка, НЕ заливает спрайт)
                float4 finalColor = texColor;
                finalColor.rgb += (colorShift, alpha * outline);  // Обводка
                finalColor.a = max(alpha, outline);  // Поддержка прозрачности

                return finalColor;
            }
            ENDHLSL
        }
    }
}
