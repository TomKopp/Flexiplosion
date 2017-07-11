sampler2D implicitInput : register(s0);
sampler2D layer_01 : register(s1);
sampler2D layer_02 : register(s2);
sampler2D layer_03 : register(s3);
sampler2D layer_04 : register(s4);
sampler2D layer_05 : register(s5);
sampler2D layer_06 : register(s6);
sampler2D layer_07 : register(s7);
float showDepth : register(c0);
float minDepth : register(c1);
float maxDepth : register(c2);
float textureWidth : register(c3);
float textureHeight : register(c4);
float blurRadius : register(c5);
// boolean: 0.0 --> do not interpolate, 1.0 do interpolate
float interpolateColor : register(c6);

float clamp(float valueToClamp)
{
	valueToClamp = valueToClamp < minDepth ? minDepth : valueToClamp;
	valueToClamp = valueToClamp > maxDepth ? maxDepth : valueToClamp;
	return valueToClamp;
}

float computeBlur(int radius, float2 texCoordsDepth)
{
	if (radius < 1)
		return tex2D(implicitInput, texCoordsDepth).x;

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
			color += (1.0 / size) * tex2D(implicitInput, texCoordsDepth + offset).x;
		}
	}

	return color;
}

float4 computeColor(float2 texCoords)
{	
	float4 imageFront = tex2D(layer_06, texCoords);
	float4 imageBack = tex2D(layer_07, texCoords);
		//int blurRadius = applyBlur ? blurRadius : 0;
	// int blurRadius = 1.0;

	float depth = computeBlur(blurRadius, texCoords);

	depth = clamp(depth);

	float factor = 1.0 / (maxDepth - minDepth);
	depth = 1.0 - ((depth - minDepth) * factor);

	if (showDepth > 0.5)
		return float4(depth, depth, depth, 1.0);

	float layerWidth = 1.0 / 6.0;
	int layer = 5;

	if (depth < layerWidth)
	{
		imageFront = tex2D(layer_01, texCoords);
		imageBack = tex2D(layer_02, texCoords);
		layer = 0;
	}
	else if (depth < 2.0f * layerWidth)
	{
		imageFront = tex2D(layer_02, texCoords);
		imageBack = tex2D(layer_03, texCoords);
		layer = 1;
	}
	else if (depth < 3.0*layerWidth)
	{
		imageFront = tex2D(layer_03, texCoords);
		imageBack = tex2D(layer_04, texCoords);
		layer = 2;
	}
	else if (depth < 4.0*layerWidth)
	{
		imageFront = tex2D(layer_04, texCoords);
		imageBack = tex2D(layer_05, texCoords);
		layer = 3;
	}
	else if (depth < 5.0*layerWidth)
	{
		imageFront = tex2D(layer_05, texCoords);
		imageBack = tex2D(layer_06, texCoords);
		layer = 4;
	}

	float blend = (depth - (layer * layerWidth)) / layerWidth;

	float4 result;

	if (interpolateColor > 0.5)
	{
		result = blend * imageBack + (1.0 - blend) * imageFront;
	}
	else
	{
		result = (depth > 0.95) ? imageBack : imageFront;
	}		

	return result;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
	return computeColor(uv);
}