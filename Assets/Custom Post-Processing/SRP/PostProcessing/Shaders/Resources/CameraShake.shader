Shader "Custom Post-Processing/SRP/CameraShake"
{
    Properties
    {
        _MainTex("(RGB)", 2D) = "" {}
        _ShakeIntensity("Shake Intensity", float) = 0.1
        _ShakeSpeed("Shake Speed", float) = 10
    }

    SubShader
    {
        Tags{"RenderType" = "Transparent" "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True"}
        LOD 300

        Pass
        {
            Name "CameraShakePass"

            HLSLPROGRAM
            #pragma vertex VertexFunction
            #pragma fragment FragmentFunction

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _ShakeIntensity;
            float _ShakeSpeed;

            float RandomNoise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv         : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            Varyings VertexFunction(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.uv = input.uv;
                output.positionCS = vertexInput.positionCS;
                return output;
            }

            half4 FragmentFunction(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                float time = _Time.y * _ShakeSpeed;
                float timeScaled = floor(time * 10.0) * 0.1;

                float shakeIntensity = lerp(0.01, 1.5, _ShakeIntensity * 0.02);  // 0.1 - минимальная, 1.5 - максимальная интенсивность
                float shakeSpeed = lerp(0.01, 2.0, _ShakeSpeed * 0.02);  // 0.1 - медленно, 2.0 - быстро

                float2 noiseInput = float2(timeScaled, timeScaled * 1.3);
                float2 shakeOffset;
                shakeOffset.x = (RandomNoise(noiseInput) - 0.5) * 2.0 * shakeIntensity * shakeSpeed;
                shakeOffset.y = (RandomNoise(noiseInput.yx) - 0.5) * 2.0 * shakeIntensity * shakeSpeed;

                uv += shakeOffset;

                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                return color;
            }
            ENDHLSL
        }
    }
}
