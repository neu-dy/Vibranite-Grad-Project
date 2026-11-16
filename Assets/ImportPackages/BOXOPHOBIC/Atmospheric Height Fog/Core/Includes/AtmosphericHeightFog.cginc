/*

// Add the following directives to your shader for directional and noise support

		#include "Assets/BOXOPHOBIC/Atmospheric Height Fog/Core/Library/AtmosphericHeightFog.cginc"


// Apply Atmospheric Height Fog to transparent shaders like this
// Where finalColor is the shader output color, fogParams.rgb is the fog color and fogParams.a is the fog mask

		float4 fogParams = GetAtmosphericHeightFog(IN.worldPos);
		return ApplyAtmosphericHeightFog(finalColor, fogParams);

*/

#ifndef ATMOSPHERIC_HEIGHT_FOG_INCLUDED
#define ATMOSPHERIC_HEIGHT_FOG_INCLUDED

half3 AHF_FogColorStart;
half3 AHF_FogColorEnd;
float4 AHF_CameraPosition;
half AHF_FogDistanceStart;
half AHF_FogDistanceEnd;
half AHF_FogDistanceFalloff;
half AHF_FogColorDuo;
half3 AHF_DirectionalColor;
half3 AHF_DirectionalDir;
half AHF_DirectionalIntensity;
half AHF_DirectionalFalloff;
half3 AHF_FogAxisOption;
half AHF_FogHeightEnd;
half AHF_FarDistanceHeight;
float AHF_FarDistanceOffset;
half AHF_FogHeightStart;
half AHF_FogHeightFalloff;
half AHF_FogLayersMode;
half AHF_NoiseScale;
half3 AHF_NoiseSpeed;
half AHF_NoiseMin;
half AHF_NoiseMax;
half AHF_NoiseDistanceEnd;
half AHF_NoiseIntensity;
half AHF_FogIntensity;
half AHF_Enabled;

float4 mod289(float4 x)
{
	return x - floor(x * 0.00346020761) * 289.0;
}
			
float4 perm(float4 x)
{
	return mod289((x * 34.0 + 1.0) * x);
}
			
half SimpleNoise3D(half3 p)
{
					// Compute integer and fractional parts.
	float3 i = floor(p);
	float3 f = p - i;
					// Cubic interpolation (smoothstep)
	f = f * f * (3.0 - 2.0 * f);
					// Build 4-component vectors using swizzling.
					// a = (i.x, i.x+1, i.y, i.y+1)
	float4 a = float4(i.x, i.x, i.y, i.y) + float4(0.0, 1.0, 0.0, 1.0);
					// Permutations along the x and y axes.
	float4 k1 = perm(a.xyxy);
	float4 k2 = perm(k1.xyxy + a.zzww);
					// Incorporate the z component.
	float4 c = k2 + float4(i.z, i.z, i.z, i.z);
	float4 k3 = perm(c);
	float4 k4 = perm(c + 1.0);
					// Map results to the [0,1] range.
	float4 o1 = frac(k3 * 0.0243902439);
	float4 o2 = frac(k4 * 0.0243902439);
					// Interpolate along z, then x, then y.
	float4 o3 = lerp(o1, o2, f.z); // f.z blends between layers.
	float2 o4 = lerp(o3.xz, o3.yw, f.x); // f.x blends between pairs.
	return lerp(o4.x, o4.y, f.y);
}

// Returns the fog color and alpha based on world position
float4 GetAtmosphericHeightFog(float3 positionWS)
{
	float4 finalColor;

	float3 WorldPosition2_g77782 = positionWS;
	float3 lerpResult785_g77782 = lerp(_WorldSpaceCameraPos, (AHF_CameraPosition).xyz, AHF_CameraPosition.w);
	float3 CameraPosition709_g77782 = lerpResult785_g77782;
	float temp_output_7_0_g77788 = AHF_FogDistanceStart;
	float temp_output_155_0_g77782 = saturate(((distance(WorldPosition2_g77782, CameraPosition709_g77782) - temp_output_7_0_g77788) / (AHF_FogDistanceEnd - temp_output_7_0_g77788)));
#ifdef AHF_DISABLE_FALLOFF
				float staticSwitch467_g77782 = temp_output_155_0_g77782;
#else
	float staticSwitch467_g77782 = (1.0 - pow((1.0 - abs(temp_output_155_0_g77782)), AHF_FogDistanceFalloff));
#endif
	half FogDistanceMask12_g77782 = staticSwitch467_g77782;
	float3 lerpResult258_g77782 = lerp(AHF_FogColorStart, AHF_FogColorEnd, ((FogDistanceMask12_g77782 * FogDistanceMask12_g77782 * FogDistanceMask12_g77782) * AHF_FogColorDuo));
	float3 normalizeResult719_g77782 = normalize((WorldPosition2_g77782 - _WorldSpaceCameraPos));
	float3 SpherePosition718_g77782 = normalizeResult719_g77782;
	float dotResult145_g77782 = dot(SpherePosition718_g77782, AHF_DirectionalDir);
	half Jitter502_g77782 = 0.0;
	float temp_output_140_0_g77782 = (saturate(((dotResult145_g77782 + Jitter502_g77782) * 0.5 + 0.5)) * AHF_DirectionalIntensity);
#ifdef AHF_DISABLE_FALLOFF
				float staticSwitch470_g77782 = temp_output_140_0_g77782;
#else
	float staticSwitch470_g77782 = pow(abs(temp_output_140_0_g77782), AHF_DirectionalFalloff);
#endif
	float DirectionalMask30_g77782 = staticSwitch470_g77782;
	float3 lerpResult40_g77782 = lerp(lerpResult258_g77782, AHF_DirectionalColor, DirectionalMask30_g77782);
#ifdef AHF_DISABLE_DIRECTIONAL
				float3 staticSwitch442_g77782 = lerpResult258_g77782;
#else
	float3 staticSwitch442_g77782 = lerpResult40_g77782;
#endif
	half3 Input_Color6_g77785 = staticSwitch442_g77782;
#ifdef UNITY_COLORSPACE_GAMMA
				float3 staticSwitch1_g77785 = Input_Color6_g77785;
#else
	float3 staticSwitch1_g77785 = (Input_Color6_g77785 * ((Input_Color6_g77785 * ((Input_Color6_g77785 * 0.305306) + 0.6821711)) + 0.01252288));
#endif
	float3 temp_output_256_0_g77782 = staticSwitch1_g77785;
	half3 Final_Color462_g77782 = temp_output_256_0_g77782;
	half3 AHF_FogAxisOption181_g77782 = AHF_FogAxisOption;
	float3 break159_g77782 = (WorldPosition2_g77782 * AHF_FogAxisOption181_g77782);
	float temp_output_7_0_g77789 = AHF_FogDistanceEnd;
	float temp_output_643_0_g77782 = saturate(((distance(WorldPosition2_g77782, CameraPosition709_g77782) - temp_output_7_0_g77789) / ((AHF_FogDistanceEnd + AHF_FarDistanceOffset) - temp_output_7_0_g77789)));
	half FogDistanceMaskFar645_g77782 = (temp_output_643_0_g77782 * temp_output_643_0_g77782);
	float lerpResult690_g77782 = lerp(AHF_FogHeightEnd, (AHF_FogHeightEnd + AHF_FarDistanceHeight), FogDistanceMaskFar645_g77782);
	float temp_output_7_0_g77802 = lerpResult690_g77782;
	float temp_output_167_0_g77782 = saturate((((break159_g77782.x + break159_g77782.y + break159_g77782.z) - temp_output_7_0_g77802) / (AHF_FogHeightStart - temp_output_7_0_g77802)));
#ifdef AHF_DISABLE_FALLOFF
				float staticSwitch468_g77782 = temp_output_167_0_g77782;
#else
	float staticSwitch468_g77782 = pow(abs(temp_output_167_0_g77782), AHF_FogHeightFalloff);
#endif
	half FogHeightMask16_g77782 = staticSwitch468_g77782;
	float lerpResult328_g77782 = lerp((FogDistanceMask12_g77782 * FogHeightMask16_g77782), saturate((FogDistanceMask12_g77782 + FogHeightMask16_g77782)), AHF_FogLayersMode);
	float mulTime204_g77782 = _TimeParameters.x * 2.0;
	float3 temp_output_197_0_g77782 = ((WorldPosition2_g77782 * (1.0 / AHF_NoiseScale)) + (-AHF_NoiseSpeed * mulTime204_g77782));
	half3 p1_g77804 = temp_output_197_0_g77782;
	half localSimpleNoise3D1_g77804 = SimpleNoise3D(p1_g77804);
	float temp_output_7_0_g77803 = AHF_NoiseMin;
	float temp_output_7_0_g77784 = AHF_NoiseDistanceEnd;
	half NoiseDistanceMask7_g77782 = saturate(((distance(WorldPosition2_g77782, CameraPosition709_g77782) - temp_output_7_0_g77784) / (0.0 - temp_output_7_0_g77784)));
	float lerpResult198_g77782 = lerp(1.0, saturate(((localSimpleNoise3D1_g77804 - temp_output_7_0_g77803) / (AHF_NoiseMax - temp_output_7_0_g77803))), (NoiseDistanceMask7_g77782 * AHF_NoiseIntensity));
	half NoiseSimplex3D24_g77782 = lerpResult198_g77782;
#ifdef AHF_DISABLE_NOISE3D
				float staticSwitch42_g77782 = lerpResult328_g77782;
#else
	float staticSwitch42_g77782 = (lerpResult328_g77782 * NoiseSimplex3D24_g77782);
#endif
	float temp_output_454_0_g77782 = (staticSwitch42_g77782 * AHF_FogIntensity);
	half Final_Alpha463_g77782 = (temp_output_454_0_g77782 * AHF_Enabled);
	float4 appendResult114_g77782 = (float4(Final_Color462_g77782, Final_Alpha463_g77782));
	float4 appendResult457_g77782 = (float4(WorldPosition2_g77782, 1.0));
#ifdef AHF_DEBUG_WORLDPOS
				float4 staticSwitch456_g77782 = appendResult457_g77782;
#else
	float4 staticSwitch456_g77782 = appendResult114_g77782;
				#endif

	finalColor = staticSwitch456_g77782;
	return finalColor;
}

// Applies the fog
float3 ApplyAtmosphericHeightFog(float3 color, float4 fog)
{
	return float3(lerp(color.rgb, fog.rgb, fog.a));
}

float4 ApplyAtmosphericHeightFog(float4 color, float4 fog)
{
	return float4(lerp(color.rgb, fog.rgb, fog.a), color.a);
}

// Shader Graph Support
void GetAtmosphericHeightFog_half(float3 positionWS, out float4 Out)
{
	Out = GetAtmosphericHeightFog(positionWS);
}

void ApplyAtmosphericHeightFog_half(float3 color, float4 fog, out float3 Out)
{
	Out = ApplyAtmosphericHeightFog(color, fog);
}

void ApplyAtmosphericHeightFog_float(float4 color, float4 fog, out float4 Out)
{
	Out = ApplyAtmosphericHeightFog(color, fog);
}

#endif