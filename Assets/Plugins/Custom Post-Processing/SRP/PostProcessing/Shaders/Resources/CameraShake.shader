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

                float2 shakeOffset;
                shakeOffset.x = (sin(time) + sin(time * 1.5)) * _ShakeIntensity;
                shakeOffset.y = (cos(time * 1.2) + cos(time * 0.8)) * _ShakeIntensity;

                uv += shakeOffset;

                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                return color;
            }
            ENDHLSL
        }
    }
}
