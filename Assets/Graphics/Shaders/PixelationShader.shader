Shader "Unlit/PixelationShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _PixelationSize("Pixelation Size", Float) = 10
        _ScreenResolution("Screen Resolution", Vector) = (1920,1080,0,0)
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct appdata_t
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            float _PixelationSize;
            float2 _ScreenResolution;
            
            v2f vert(appdata_t v)
            {
                v2f o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                // Преобразуем UV в координаты экрана
                float2 screenUV = i.uv * _ScreenResolution;
                
                // Пикселизация
                screenUV = floor(screenUV / _PixelationSize) * _PixelationSize;
                
                // Обратно в [0,1]
                screenUV /= _ScreenResolution;
                
                // Получаем цвет пикселизированного изображения
                return tex2D(_MainTex, screenUV);
            }
            
            ENDHLSL
        }
    }
}
