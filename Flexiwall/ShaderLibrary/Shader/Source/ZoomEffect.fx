sampler2D implicitInput : register(s0);
sampler2D image_layer : register(s1);
float showDepth : register(c0);
float minDepth : register(c1);
float maxDepth : register(c2);
float textureWidth : register(c3);
float textureHeight : register(c4);
float blurRadius : register(c5);

float interpolateColor : register(c6);

float lens_x : register(c7);
float lens_y : register(c8);


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
	float depth = computeBlur(blurRadius, texCoords);

	depth = clamp(depth);

	float factor = 1.0 / (maxDepth - minDepth);
	depth = 1.0 - ((depth - minDepth) * factor);

	if (showDepth > 0.5)
		return float4(depth, depth, depth, 1.0);

	float zoomfaktor = 1 + depth;

	float lensX_new = lens_x / textureWidth;
	float lensY_new = lens_y / textureHeight;

	float x = texCoords.x - lensX_new;
	float y = texCoords.y - lensY_new;

	float _x = lensX_new + (x / zoomfaktor);
	float _y = lensY_new + (y / zoomfaktor);

	float2 newPixelCoord = { _x, _y };

	return tex2D(image_layer, newPixelCoord);

}

float4 main(float2 uv : TEXCOORD) : COLOR
{
	return computeColor(uv);
}