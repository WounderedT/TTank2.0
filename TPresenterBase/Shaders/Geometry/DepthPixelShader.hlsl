#include "Common.hlsl"

float4 pixel_shader(PixelShaderInput input) : SV_Target
{
	//	take the (Z / W) and use as color.
	float4 output = float4(input.position.z, 0, 0, 1);
	return output;
}