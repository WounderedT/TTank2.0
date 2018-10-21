#include "../Common.hlsl"
#include "TessellationCommon.hlsl"

// Should be placed somewhere else.
uint4 unpack_uint(uint4 input)
{
    uint4 result = uint4(input.x & 255, input.x >> 8 & 255, input.x >> 16 & 255, input.x >> 24 & 255);
    return result;
}

void skin_vertex(float4 weights0, float4 weights1, uint4 bones0, inout float4 position, inout float3 normal)
{
    if (weights0.x != 0)
	{
		float4x4 skinTransform = (float4x4)0;

        uint4 boneIndices0 = unpack_uint(bones0.x);
        uint4 boneIndices1 = unpack_uint(bones0.y);

        skinTransform = Bones[boneIndices0.x] * weights0.x
            + Bones[boneIndices0.y] * weights0.y
            + Bones[boneIndices0.z] * weights0.z
            + Bones[boneIndices0.w] * weights0.w;
        
        skinTransform = skinTransform + Bones[boneIndices1.x] * weights1.x
            + Bones[boneIndices1.y] * weights1.y
            + Bones[boneIndices1.z] * weights1.z
            + Bones[boneIndices1.w] * weights1.w;


        position = mul(float4(position.xyz, 1), skinTransform);

		//	we assume that skin transform includes only uinform scaling (if any)
		normal = mul(normal, (float3x3)skinTransform);
	}
}

PixelShaderInput vertex_shader(VertexShaderInput input) 
{
	PixelShaderInput result = (PixelShaderInput)0;

#ifdef USE_SKINNING
	skin_vertex(input.blend_weights0, input.blend_weights1, input.blend_indices0, input.position, input.normal);
#endif	

	result.world_position = mul(input.position, world_projection.world).xyz;
	result.position = mul(float4(result.world_position.xyz, 1), view_projection.view_proj);
	result.diffuse = material_diffuse;
	// Apply material UV transformation
	result.textcoord0 = mul(float4(input.textcoord0.x, input.textcoord0.y, 0, 1), (float4x2)uv_transform).xy;
	//result.textcoord0 = float2(1, 1);

	// We use the inverse transpose of the world so that if there is non uniform
	// scaling the normal is transformed correctly. We also use a 3x3 so that 
	// the normal is not affected by translation (i.e. a vector has the same direction
	// and magnitude regardless of translation)
	result.world_normal = mul(input.normal, (float3x3)world_projection.inv_world_transpose);	// transform normal to world space

	return result;
}

HullShaderInput tessellation_pass(VertexShaderInput input)
{
    HullShaderInput result = (HullShaderInput) 0;

#ifdef USE_SKINNING
	skin_vertex(input.blend_weights0, input.blend_weights1, input.blend_indices0, input.position, input.normal);
#endif	

    result.textcoord0 = mul(float4(input.textcoord0.x, input.textcoord0.y, 0, 1), (float4x2) uv_transform).xy;
    result.diffuse = material_diffuse;

    result.world_normal = mul(input.normal, (float3x3) world_projection.inv_world_transpose);
    result.world_position = mul(input.position, world_projection.world).xyz;

    return result;
}