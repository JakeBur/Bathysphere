#ifndef BATH_APPLY_FOG
#define BATH_APPLY_FOG

void ApplyFog_float(float3 Color, float3 Position, out float3 Result)
{
    Result = MixFog(ComputeFogFactor(Position);
}

#endif