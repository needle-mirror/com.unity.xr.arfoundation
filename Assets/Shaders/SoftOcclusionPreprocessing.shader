Shader "Occlusion/Soft/DepthPreprocessing"
{
    SubShader
    {
        Pass
        {
            Name "Environment Depth Preprocessing Pass"

            ZWrite Off

            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag

            Texture2DArray_half _EnvironmentDepthTexture;
            SamplerState sampler_EnvironmentDepthTexture;
            float4 _EnvironmentDepthTexture_TexelSize;

            struct Attributes
            {
                uint vertexId : SV_VertexID;
                uint instanceId : SV_InstanceID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                uint depthSlice : SV_RenderTargetArrayIndex;
            };

            float4 CalculateMinMaxDepth(float2 uv, float slice)
            {
                const int NUM_SAMPLES = 5;
                static const float2 offsets[NUM_SAMPLES] =
                {
                    float2(2.0f, 0.0f),
                    float2(0.0f, 2.0f),
                    float2(-2.0f, 0.0f),
                    float2(0.0f, 0.0f),
                    float2(0.0f, -2.0f)
                };

                float4 depths[NUM_SAMPLES];
                const float2 onePixelOffset = _EnvironmentDepthTexture_TexelSize.xy;

                float minDepth = 1.0f;
                float maxDepth = 0.0f;
                float depthSum = 0.0f;

                // Find the local min and max, and collect all depth samples in the sampling grid
                for (int i = 0; i < NUM_SAMPLES; ++i)
                {
                    float2 uvSample = uv + (offsets[i] + 0.5f) * onePixelOffset;
                    depths[i] = _EnvironmentDepthTexture.Gather(sampler_EnvironmentDepthTexture, float3(uvSample.x, uvSample.y, slice));
                    depthSum += dot(depths[i], float4(0.25f, 0.25, 0.25, 0.25));

                    float localMax = max(max(depths[i].x, depths[i].y), max(depths[i].z, depths[i].w));
                    float localMin = min(min(depths[i].x, depths[i].y), min(depths[i].z, depths[i].w));

                    maxDepth = max(maxDepth, localMax);
                    minDepth = min(minDepth, localMin);
                }

                float maxSumDepth = 0.0f;
                float minSumDepth = 0.0f;
                float maxSumCount = 0.0f;
                float minSumCount = 0.0f;

                const float kMaxDepthMultiplier = 0.8846f; // depths scaled symmetric to 1.15 scale. 1-1/1.15==0.13 k=1/(1+0.13)==0.8846
                const float kMinDepthMultiplier  = 1.15f;

                const float normalDepthScale = 1.0f;
                const float depthReciprocalScaleMax = 1.0f / kMaxDepthMultiplier;
                const float depthReciprocalScaleMin = 1.0f / kMinDepthMultiplier;
                const float depthMaxBase = 1.0f - normalDepthScale * depthReciprocalScaleMax;
                const float depthMinBase = 1.0f - normalDepthScale * depthReciprocalScaleMin;

                // Adjusting depth thresholds by applying reciprocal scaling to modify depth impact
                float depthThrMax = depthMaxBase + maxDepth * depthReciprocalScaleMax;
                float depthThrMin = depthMinBase + minDepth * depthReciprocalScaleMin;

                float avg = depthSum * (1.0f / float(NUM_SAMPLES));
                if (depthThrMax < minDepth && depthThrMin > maxDepth)
                {
                    // Degenerate case: the entire neighborhood is within min-max thresholds for averaging
                    // therefore minAvg == maxAvg == avg.
                    // Directly output the encoded fragColor as:
                    // (1 - minAvg, 1 - maxAvg, avg - minAvg, maxAvg - minAvg)
                    return float4(1.0f - avg, 1.0f - avg, 0.0f, 0.0f);
                }

                for (int i = 0; i < NUM_SAMPLES; ++i)
                {
                    float4 maxMask = depths[i] >= float4(depthThrMax, depthThrMax, depthThrMax, depthThrMax);
                    float4 minMask = depths[i] <= float4(depthThrMin, depthThrMin, depthThrMin, depthThrMin);
                    minSumDepth += dot(minMask, depths[i]);
                    minSumCount += dot(minMask, float4(1.0f, 1.0f, 1.0f, 1.0f));
                    maxSumDepth += dot(maxMask, depths[i]);
                    maxSumCount += dot(maxMask, float4(1.0f, 1.0f, 1.0f, 1.0f));
                    // Value used to interpolate occlusion alphas between min and max values
                }
                float minAvg = minSumDepth / minSumCount;
                float maxAvg = maxSumDepth / maxSumCount;

                return float4(1 - minAvg, 1 - maxAvg, avg - minAvg, maxAvg - minAvg);
            }

            Varyings vert(const Attributes input)
            {
                Varyings output;

                const uint vertexID = input.vertexId;
                const float2 uv = float2(vertexID << 1 & 2, vertexID & 2);
                output.uv = float2(uv.x, 1.0 - uv.y);
                output.positionCS = float4(uv * 2.0 - 1.0, 1.0, 1.0);

                output.depthSlice = input.instanceId;
                return output;
            }

            float4 frag(const Varyings input) : SV_Target
            {
                return CalculateMinMaxDepth(input.uv, input.depthSlice);
            }
            ENDCG
        }
    }
}
