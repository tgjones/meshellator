matrix WorldViewProjection : WORLDVIEWPROJECTION;

struct VertexShaderInput
{
	float3 position : POSITION;
	float3 color : COLOR0;
};

struct PixelShaderInput
{
	float4 pos : POSITION;
	float3 color : COLOR0;
};

PixelShaderInput VS(VertexShaderInput input)
{
	PixelShaderInput output = (PixelShaderInput)0;
	output.pos = mul(float4(input.position, 1), WorldViewProjection);
	output.color = input.color;
	return output;
}

float4 PS(PixelShaderInput input) : COLOR0
{
	return float4(input.color, 1);
}

technique Render
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}