//--------------------------------------------------------------------------------------
// File: Tutorial07.fx
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------
// Constant Buffer Variables
//--------------------------------------------------------------------------------------
Texture3D txThreeD : register(t0);
Texture2D txDepth : register(t1);
SamplerState samLinear : register(s0);
cbuffer ConstantBuffer : register(b0)
{
	float showDepth;
	float minDepth;
	float maxDepth;
	float textureWidth;
	float textureHeight;
	float blurRadius;
}


//--------------------------------------------------------------------------------------

float clamp(float valueToClamp)
{
	valueToClamp = valueToClamp < minDepth ? minDepth : valueToClamp;
	valueToClamp = valueToClamp > maxDepth ? maxDepth : valueToClamp;
	return valueToClamp;
}

float computeBlur(int radius, float3 texCoordsDepth)
{		
	if (radius < 1)
		return txDepth.Sample(samLinear, texCoordsDepth);

	int r = radius > 8.0 ? 8 : radius;

	float color = 0.0;

	int size = int(ceil(r)) * 2 + 1;
	size *= size;

	float pixelSizeX = 1.0 / textureWidth;
	float pixelSizeY = 1.0 / textureHeight;

	[unroll(17)]
	for (int i = -r; i <= r; i++)
	{
		[unroll(17)]
		for (int j = -r; j <= r; j++)
		{
			float2 offset = float2(i * pixelSizeX, j * pixelSizeY);
				color += (1.0 / size) * txDepth.Sample(samLinear, texCoordsDepth + offset);
		}
	}

	return color;
}

float computeDepth(float3 texCoords)
{

	float depth = computeBlur(blurRadius, texCoords);

	depth = clamp(depth);

	float factor = 1.0 / (maxDepth - minDepth);
	depth = (depth - minDepth) * factor;

	if (depth > 0.99)
		depth = 0.99;

	return 0.99 - depth;
}




//--------------------------------------------------------------------------------------
struct VS_INPUT
{
    float4 Pos : POSITION;
	float3 texcoord : TEXCOORD0;
	//float3 texcoord1 : TEXCOORD1;
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
	float3 texcoord : TEXCOORD0;
	//float3 texcoord1 : TEXCOORD1;
};



//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
PS_INPUT VS( VS_INPUT input )
{


    PS_INPUT output = (PS_INPUT)0;
	output.Pos = input.Pos;
	output.texcoord = float3(input.texcoord.x, -input.texcoord.y, input.texcoord.z);
	//output.texcoord1 = input.texcoord1;
    return output;
}




//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 PS( PS_INPUT input) : SV_Target
{
	//return float4(input.texcoord.x, input.texcoord.y, 0.0, 1.0);
	//return float4(minDepth, 0.0, 0.0, 1.0);
	//return txThreeD.Sample(samLinear, input.texcoord - float3(0, 0, -minDepth));
	// return txThreeD.Sample(samLinear, input.texcoord - float3(0, 0, -(computeDepth(input.texcoord))));

	float depth = computeDepth(input.texcoord);

	if (showDepth < 1.0)
		return txThreeD.Sample(samLinear, float3(input.texcoord.x, input.texcoord.y, depth));
	
	return float4(depth, depth, depth, 1.0);
}

//const vec2 offsetxy = vec2(375.0, 225.0);
//const float radius = 50.0;
//
//float generate_circle(vec2 position, float radius)
//{
//	return (length(gl_FragCoord.xy - position) - radius) / radius;
//}
//
//void main(void)
//{
//	float mask1 = 1.0 - generate_circle(offsetxy, radius);
//	float mask2 = 1.0 - generate_circle(offsetxy + vec2(100.0, 100.0), radius);
//	vec4 color = vec4(max(mask1, mask2));
//	gl_FragColor = color;
//}