#include "Common.hlsl"

Texture2D _texture0 : register(t0);
SamplerState _sampler : register(s0);

float4 pixel_shader(PixelShaderInput input) : SV_Target
{
	//	After interpolation the values are nor necessarily normalized
	float3 normal = normalize(input.world_normal);
	float3 to_eye = normalize(view_projection.camera_position - input.world_position);
	float3 to_light = normalize(-light.direction);

	//	Texture sample (use white if no texture)
	float4 sample = (float4)1.0f;
	if (has_texture)
		sample = _texture0.Sample(_sampler, input.textcoord0);

	float3 ambient = material_ambient.rgb;
	float3 emissive = material_emissive.rgb;
	float3 diffuse = Lambert(input.diffuse, normal, to_light);

	//	Calculate final color component
	//	We saturate ambient + diffuse to ensure there is no over-brightness on the texture sample
	//	if the sum is greater than 1 (not for HDR rendering)
	float3 color = (saturate(ambient + diffuse) * sample.rgb) * light.color.rgb + emissive;

	//	Calculate final alpha value
	float alpha = input.diffuse.a * sample.a;
	return float4(color, alpha);
}