#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

struct CustomLightingData
{
    float3 normal;
};

float GenerateLighting(CustomLightingData lightingData)
{
    #ifdef SHADERGRAPH_PREVIEW
    float3 lightDirection = float3(0.5, 0.5, 0);
    float intensity = saturate(dot(lightingData.normal, lightDirection));
    return intensity;
	#else
    
    Light mainLight = GetMainLight();
    float diffuse = saturate(dot(lightingData.normal, mainLight.direction));

    return diffuse;
    #endif
}

void GenerateLighting_float(float3 Normal, out float Diffuse)
{
    CustomLightingData lightingData;
    lightingData.normal = Normal;
    
    Diffuse = GenerateLighting(lightingData);
/*#if defined(SHADERGRAPH_PREVIEW)
	Direction = half3(0.5, 0.5, 0);
	Color = 1;
	Attenuation = 1;
#else
#if SHADOWS_SCREEN
	half4 clipPos = TransformWorldToHClip(WorldPos);
	half4 shadowCoord = ComputeScreenPos(clipPos);
#else
	half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
	Light mainLight = GetMainLight(shadowCoord);
	Direction = mainLight.direction;
	Color = mainLight.color;
	Attenuation = mainLight.distanceAttenuation * mainLight.shadowAttenuation;
#endif*/

}

#endif