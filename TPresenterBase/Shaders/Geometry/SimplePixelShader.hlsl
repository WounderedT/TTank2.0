#include "Common.hlsl"

// interpolated color
float4 pixel_shader(PixelShaderInput input) : SV_Target
{
	return input.diffuse;
}