#include <D3DX_DXGIFormatConvert.inl>
// cbuffers
#define FRAME_SLOT 0
#define PROJECTION_SLOT 1
#define OBJECT_SLOT 2
#define MATERIALS_SLOT 3
#define FOLIAGE_SLOT 4
#define ALPHAMASK_SLOT 5
#define SKELETON_SLOT 6

#define MERGE(a,b) a##b

struct VertexShaderInput
{
	float4 position : SV_Position;
	float3 normal : NORMAL;
	//float4 color : COLOR0;
	float2 textcoord0 : TEXCOORD;

#ifdef USE_SKINNING
	uint4 blend_indices0 : BLENDINDICES0;
	
    float4 blend_weights0 : BLENDWEIGHT0;
	float4 blend_weights1 : BLENDWEIGHT1;
#endif
};

struct PixelShaderInput
{
	float4 position : SV_Position;	// interpolation of vertex * material diffuse
	float4 diffuse : COLOR;	// interpolation of vertex UV texture coordinate
	float2 textcoord0 : TEXCOORD;

	// World position and normal for lighting
	float3 world_normal : NORMAL;
	float3 world_position : WORLDPOS;
};

struct ViewProjection
{
	float3 camera_position;
	float4x4 view_proj;
};

struct WorldProjection
{
	float4x4 world;	// world matrix for calculation of the lighting in the world space
	float4x4 inv_world_transpose;	// used for bringing normals into world space, especially necessary where non-uniform scaling has been applied
};

// A simple directional light (e.g. like sun)
struct DirectionalLight
{
	float4 color;
	float3 direction;
};

// Constant buffer to be updated by application per object
//cbuffer PerObject : register(b1)
cbuffer PerObject : register(MERGE(b, FRAME_SLOT))
{
	WorldProjection world_projection;
};

//cbuffer PerFrame : register(b0)
cbuffer PerFrame : register(MERGE(b, PROJECTION_SLOT))
{
	ViewProjection view_projection;
	DirectionalLight light;
    float tessellation_factor;
};

cbuffer PerMaterial : register(MERGE(b, MATERIALS_SLOT))
{
	float4 material_ambient;
	float4 material_diffuse;
	float4 material_specular;
	float material_specular_power;
	bool has_texture;
	float4 material_emissive;
	float4x4 uv_transform;
};

cbuffer PerSkeleton : register(MERGE(b, SKELETON_SLOT))
{
	float4x4 Bones[256];
};

float3 Lambert(float4 pixel_diffuse, float3 normal, float3 to_light)
{
	//	Calculate diffuse color (Labmert's Cosine Law - dot product of the light and normal)
	//	Saturate to clamp the value between 0 and 1.
	float3 diffuse_amount = saturate(dot(normal, to_light));
	return pixel_diffuse.rgb * diffuse_amount;
}

float3 SpecularPhong(float3 normal, float3 to_light, float3 to_eye)
{
	//	R = reflection(i,n) => R = i - 2*n*dot(i,n)
	float3 reflection = reflect(-to_light, normal);

	//	Calculate the specular amount (smaller specular power = larger specular highlight). Cannot
	//	allow a power of 0 otherwise the model will appear black and white.
	float specular_amount = pow(saturate(dot(reflection, to_eye)), max(material_specular_power, 0.00001f));
	return material_specular.rgb * specular_amount;
}

float3 SpecularBlinnPhong(float3 normal, float3 to_light, float3 to_eye)
{
	//	Calculate the half vector
	float3 halfway = normalize(to_light + to_eye);
	
	//	Saturate is used to prevent backface light reflection
	//	Calculate specular (smaller power = larger highlight)
	float specular_amount = pow(saturate(dot(normal, halfway)), max(material_specular_power, 0.00001f));
	return material_specular.rgb * specular_amount;
}
