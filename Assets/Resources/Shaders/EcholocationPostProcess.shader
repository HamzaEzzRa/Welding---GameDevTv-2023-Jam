Shader "Custom/Echolocation Post Process"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"

            #pragma target 3.0

            #define MAX_RIPPLE_AMOUNT 50

            sampler2D _MainTex;
            sampler2D _CameraNormalsTexture;
            sampler2D _CameraDepthTexture;

            float4 _MainTex_TexelSize;

            float _Scale;
            float4 _Color;

            float _DepthThreshold;
            float _DepthNormalThreshold;
            float _DepthNormalThresholdScale;

            float _NormalThreshold;

            float4x4 _ClipToView;
            float4x4 _ClipToWorld;

            int _CurrentRippleCount;
            float3 _RippleOrigins[MAX_RIPPLE_AMOUNT];
            float _RipplePowers[MAX_RIPPLE_AMOUNT];
            float _ExteriorRippleWidths[MAX_RIPPLE_AMOUNT];
            float _InteriorRippleWidths[MAX_RIPPLE_AMOUNT];
            float _RippleDurations[MAX_RIPPLE_AMOUNT];
            float _RippleTimes[MAX_RIPPLE_AMOUNT];

            int _WithTexture;

            float2 TransformTriangleVertexToUV(float2 vertex)
            {
                float2 uv = (vertex + 1.0) * 0.5;
                return uv;
            }

            struct Attributes
            {
                float3 vertex : POSITION;
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float2 texcoordStereo : TEXCOORD1;
                float3 viewSpaceDir : TEXCOORD2;
                float3 worldDirection : TEXCOORD3;

            #if STEREO_INSTANCING_ENABLED
                uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
            #endif
            };

            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.vertex = float4(v.vertex.xy, 0.0, 1.0);
                o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);

            #if defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)
                o.viewSpaceDir = mul(o.vertex, _ClipToView).xyz;
                o.worldDirection = mul(o.vertex, _ClipToWorld).xyz;
            #else
                o.viewSpaceDir = mul(_ClipToView, o.vertex).xyz;
                o.worldDirection = mul(_ClipToWorld, o.vertex).xyz;
            #endif

            #if UNITY_UV_STARTS_AT_TOP
                o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
            #endif

                o.texcoordStereo = TransformStereoScreenSpaceTex(o.texcoord, 1.0);
                return o;
            }

            float4 Frag(Varyings i) : SV_Target
            {
                float3 tex = tex2D(_MainTex, i.texcoord).rgb;

                float halfScaleFloor = floor(_Scale * 0.5);
                float halfScaleCeil = ceil(_Scale * 0.5);

                float2 bottomLeftUV = i.texcoord - float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleFloor;
                float2 topRightUV = i.texcoord + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleCeil;
                float2 bottomRightUV = i.texcoord + float2(_MainTex_TexelSize.x * halfScaleCeil, -_MainTex_TexelSize.y * halfScaleFloor);
                float2 topLeftUV = i.texcoord + float2(-_MainTex_TexelSize.x * halfScaleFloor, _MainTex_TexelSize.y * halfScaleCeil);

                float3 normal0 = tex2D(_CameraNormalsTexture, bottomLeftUV).rgb;
                float3 normal1 = tex2D(_CameraNormalsTexture, topRightUV).rgb;
                float3 normal2 = tex2D(_CameraNormalsTexture, bottomRightUV).rgb;
                float3 normal3 = tex2D(_CameraNormalsTexture, topLeftUV).rgb;
                
                float depth0 = tex2D(_CameraDepthTexture, bottomLeftUV).r;
                float depth1 = tex2D(_CameraDepthTexture, topRightUV).r;
                float depth2 = tex2D(_CameraDepthTexture, bottomRightUV).r;
                float depth3 = tex2D(_CameraDepthTexture, topLeftUV).r;

            #if defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)
                depth0 = 1.0 - depth0;
                depth1 = 1.0 - depth1;
                depth2 = 1.0 - depth2;
                depth3 = 1.0 - depth3;
            #endif

                float3 viewNormal = normal0 * 2 - 1;
                float NdotV = 1 - dot(viewNormal, -i.viewSpaceDir);

                float normalThreshold01 = saturate((NdotV - _DepthNormalThreshold) / (1 - _DepthNormalThreshold));
                float normalThreshold = normalThreshold01 * _DepthNormalThresholdScale + 1;

                float depthThreshold = _DepthThreshold * depth0 * normalThreshold;

                float depthFiniteDifference0 = depth1 - depth0;
                float depthFiniteDifference1 = depth3 - depth2;
                float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
                edgeDepth = edgeDepth > depthThreshold ? 1 : 0;

                float3 normalFiniteDifference0 = normal1 - normal0;
                float3 normalFiniteDifference1 = normal3 - normal2;
                float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
                edgeNormal = edgeNormal > _NormalThreshold ? 1 : 0;

                float edge = max(edgeDepth, edgeNormal);

                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, bottomLeftUV).r);
                float3 worldPos = i.worldDirection * depth + _WorldSpaceCameraPos;

                float4 finalColor = 0;

                int rippleCount = min(_CurrentRippleCount, MAX_RIPPLE_AMOUNT);
                for (int i = 0; i < rippleCount; i++)
                {
                    float distanceFromRipple = length(worldPos - _RippleOrigins[i].xyz);
                    float rippleDistance = distanceFromRipple - _RippleTimes[i];
                    float rippleFadedStrength = _RipplePowers[i] * saturate(1 - distanceFromRipple / _RippleDurations[i]);

                    float exteriorHalfWidth = _ExteriorRippleWidths[i] * 0.5;
                    float interiorHalfWidth = _InteriorRippleWidths[i] * 0.5;
                    float effectiveRippleRadius = (rippleDistance > -interiorHalfWidth && rippleDistance < exteriorHalfWidth);
                    float visualRippleRadius = (rippleDistance > -exteriorHalfWidth && rippleDistance < exteriorHalfWidth);

                    float distanceFactor = (1 - abs(rippleDistance) / interiorHalfWidth) * effectiveRippleRadius;
                    float edgeDistanceFactor = edge * distanceFactor;

                    float colorContribution = (visualRippleRadius + edgeDistanceFactor) * rippleFadedStrength;
                    float4 color = _WithTexture == 0 ? _Color : float4(tex, 1);
                    finalColor += color * colorContribution;
                }

                return finalColor;
            }
            ENDCG
        }
    }
}
