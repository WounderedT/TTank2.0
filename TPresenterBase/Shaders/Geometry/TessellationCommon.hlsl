struct HullShaderInput
{
    float3 world_position : POSITION;
    float4 diffuse : COLOR;
    float2 textcoord0 : TEXCOORD0;
    float3 world_normal : TEXCOORD1;
};

struct DS_ControlPointInpit
{
    float3 positon : BEZIERPOS;
    float4 diffuse : COLOR0;
};

// Triangle interpolation using Barycentric coordinates

float2 barycentric_interpolate(float2 v0, float2 v1, float2 v2, float3 barycentric)
{
    return barycentric.z * v0 + barycentric.x * v1 + barycentric.y * v2;
}

float2 barycentric_interpolate(float2 v[3], float3 barycentric)
{
    return barycentric_interpolate(v[0], v[1], v[2], barycentric);
}

float3 barycentric_interpolate(float3 v0, float3 v1, float3 v2, float3 barycentric)
{
    return barycentric.z * v0 + barycentric.x * v1 + barycentric.y * v2;
}

float3 barycentric_interpolate(float3 v[3], float3 barycentric)
{
    return barycentric_interpolate(v[0], v[1], v[2], barycentric);
}

float4 barycentric_interpolate(float4 v0, float4 v1, float4 v2, float3 barycentric)
{
    return barycentric.z * v0 + barycentric.x * v1 + barycentric.y * v2;
}

float4 barycentric_interpolate(float4 v[3], float3 barycentric)
{
    return barycentric_interpolate(v[0], v[1], v[2], barycentric);
}

// QUAD bilinear interpolation

float2 bilerp(float2 v[4], float2 uv)
{
    float2 side1 = lerp(v[0], v[1], uv.x);
    float2 side2 = lerp(v[3], v[2], uv.x);
    float2 result = lerp(side1, side2, uv.y);
    
    return result;
}

float3 bilerp(float3 v[4], float2 uv)
{
    float3 side1 = lerp(v[0], v[1], uv.x);
    float3 side2 = lerp(v[3], v[2], uv.x);
    float3 result = lerp(side1, side2, uv.y);
    
    return result;
}

float4 bilerp(float4 v[4], float2 uv)
{
    float4 side1 = lerp(v[0], v[1], uv.x);
    float4 side2 = lerp(v[3], v[2], uv.x);
    float4 result = lerp(side1, side2, uv.y);
    
    return result;
}