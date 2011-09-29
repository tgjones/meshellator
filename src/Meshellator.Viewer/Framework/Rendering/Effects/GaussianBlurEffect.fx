// With thanks to Kyle Hayward's shadow mapping demo at
// http://graphicsrunner.blogspot.com/

// Pixel shader applies a one dimensional gaussian blur filter.
// This is used twice by the bloom postprocess, first to
// blur horizontally, and then again to blur vertically.

float2 Scale;

float2 GaussFilter[7] = 
{ 
	-3.0,	0.015625,
	-2.0,	0.09375,
	-1.0,	0.234375,
	0.0,	0.3125,
	1.0,	0.234375,
	2.0,	0.09375,
	3.0,	0.015625
};

uniform extern texture Texture;

sampler TextureSampler = sampler_state
{
	Texture = <Texture>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU  = CLAMP;
	AddressV  = CLAMP;
};

float4 GaussianBlurPixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 color = float4(0, 0, 0, 0);
	for (int i = 0; i < 7; i++)
		color += tex2D(TextureSampler, texCoord + float2(GaussFilter[i].x * Scale.x, GaussFilter[i].x * Scale.y)) * GaussFilter[i].y;
	return color;
}

technique GaussianBlur
{
	pass P0
	{
		PixelShader = compile ps_2_0 GaussianBlurPixelShader();    
	}
}