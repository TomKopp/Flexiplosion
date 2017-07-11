sampler2D flexiInput : register(s0);
sampler2D kinectInput : register(s1);
float4 warp1 : register(c0);
float4 warp2 : register(c1);
float3 warp3 : register(c2);

float4 main(float2 kinectPoint : TEXCOORD) : COLOR
{
	float2 imagePoint;
	float4 kinectColor;
	kinectColor = tex2D(kinectInput, kinectPoint);
	float kinectZ = kinectColor.r;
	// imagePoint.x = (warp[0] * kinectPoint.x + warp[1] * kinectPoint.y + warp[2] * kinectZ + warp[3]) / (warp[8] * kinectPoint.x + warp[9] * kinectPoint.y + warp[10] * kinectZ + 1)
	// imagePoint.y = (warp[4] * kinectPoint.x + warp[5] * kinectPoint.y + warp[6] * kinectZ + warp[7]) / (warp[8] * kinectPoint.x + warp[9] * kinectPoint.y + warp[10] * kinectZ + 1)
	imagePoint.x = (warp1.x * kinectPoint.x * 640 + warp1.y * kinectPoint.y * 480 + warp1.z * kinectZ + warp1.w) / (warp3.x * kinectPoint.x * 640 + warp3.y * kinectPoint.y * 480 + warp3.z * kinectZ + 1);
	imagePoint.y = (warp2.x * kinectPoint.x * 640 + warp2.y * kinectPoint.y * 480 + warp2.z * kinectZ + warp2.w) / (warp3.x * kinectPoint.x * 640 + warp3.y * kinectPoint.y * 480 + warp3.z * kinectZ + 1);
	imagePoint.x = imagePoint.x / 1920;
	imagePoint.y = imagePoint.y / 1080;
	return tex2D(flexiInput, imagePoint);
	//return float4(warp1.x, 0.0, 0.0, 0.0);
}