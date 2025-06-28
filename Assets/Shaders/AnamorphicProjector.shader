/*
 * Author: Muhammad Farhan
 * Date: 13/5/25
 * Description: Sahder for anamorphic illusions
 */
Shader "Custom/AnamorphicProjector"
{
    Properties
    {
        _MainTex("Projection Texture", 2D) = "white" {}
        _ProjectionOrigin("Projection Origin", Vector) = (0,0,0,1)
        _ProjectionDir("Projection Direction (Forward)", Vector) = (0,0,-1,0)
        _FOV("Field of View", Float) = 60
        _Aspect("Aspect Ratio", Float) = 1.77778
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float3 _ProjectionOrigin;
            float3 _ProjectionDir;
            float _FOV;
            float _Aspect;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.worldPos = worldPos;
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                return OUT;
            }


            float4 frag(Varyings IN) : SV_Target
            {
                float3 dir = IN.worldPos - _ProjectionOrigin;

                // Project onto a screen aligned with _ProjectionDir
                float3 forward = normalize(_ProjectionDir);
                float3 up = float3(0,1,0);
                float3 right = normalize(cross(up, forward));
                up = cross(forward, right);

                float viewPlaneDist = 1.0 / tan(radians(_FOV) * 0.5);

                float x = dot(dir, right) / dot(dir, forward);
                float y = dot(dir, up) / dot(dir, forward);

                float2 uv = float2(x / _Aspect, y) * 0.5 + 0.5;

                if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
                    discard;

                return tex2D(_MainTex, uv);
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}