Shader "Custom Post-Processing/SRP/Drunk"
{
    Properties
    {
        _MainTex("(RGB)", 2D) = "" {}
        _Speed("Speed", float) = 1
        _Zoom("Zoom", float) = 1
        [Toggle]_IsScreenSpace("Is Screen Space", float) = 1
    }

    SubShader
    {
        Tags{"RenderType" = "Transparent" "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True"}
        LOD 300

        Pass
        {
            Name "StandardLit"

            HLSLPROGRAM
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _MainTex_ST;
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

            float _Speed;
            float _Zoom;
            float _IsScreenSpace;

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv         : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float4 screenPos  : TEXCOORD1;
            };

            Varyings LitPassVertex(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.positionCS = vertexInput.positionCS;
                output.screenPos = ComputeScreenPos(vertexInput.positionCS);
                return output;
            }

            #define scale_uv(uv,scale,center) ((uv - center) * scale + center) 

            half4 LitPassFragment(Varyings input) : SV_Target
            {
                half4 color = half4(1, 1, 1, 1);
                float2 uv = input.screenPos.xy / input.screenPos.w;

                if (_IsScreenSpace < 0.5)
                    uv = input.uv;

                float t = _Time.y * _Speed;
                float2 center = float2(
                    sin(t * 1.25 + 75.0 + uv.y * 0.5) + sin(t * 2.75 - 18.0 - uv.x * 0.25),
                    sin(t * 1.75 - 125.0 + uv.x * 0.25) + sin(t * 2.25 + 4.0 - uv.y * 0.5)
                ) * 0.25 + 0.5;

                float z = (sin((t + 234.5) * 3.0) * 0.05 + 0.75) * _Zoom;

                float2 uv2 = scale_uv(uv, z, center);
                color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv2);

                float vignette = 1.0 - distance(uv, float2(0.5, 0.5));
                color = lerp(color, color * vignette, sin((t + 80.023) * 2.0) * 0.75);
                
                return color;
            }
            ENDHLSL
        }
    }
}
