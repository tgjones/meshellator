float4x4 World : WORLD;
float4x4 WorldViewProjection : WORLDVIEWPROJECTION;
float3 EyePosition;
float3 DiffuseColor;
float3 SpecularColor;
float SpecularPower = 16;
float Alpha;

float3 AmbientLightColor = float3(0.05333332f, 0.09882354f, 0.1819608f);

float3 DirLight0DiffuseColor = float3(1, 0.9607844f, 0.8078432f);
float3 DirLight0Direction = float3(-0.5265408f, -0.5735765f, -0.6275069f);
float3 DirLight0SpecularColor = float3(1, 0.9607844f, 0.8078432f);

float3 DirLight1DiffuseColor = float3(0.9647059f, 0.7607844f, 0.4078432f);
float3 DirLight1Direction = float3(0.7198464f, 0.3420201f, 0.6040227f);
float3 DirLight1SpecularColor = float3(0, 0, 0);

float3 DirLight2DiffuseColor = float3(0.3231373f, 0.3607844f, 0.3937255f);
float3 DirLight2Direction = float3(0.4545195f, -0.7660444f, 0.4545195f);
float3 DirLight2SpecularColor = float3(0.3231373f, 0.3607844f, 0.3937255f);

struct ColorPair
{
	float3 Diffuse;
	float3 Specular;
};

//-----------------------------------------------------------------------------
// Compute per-pixel lighting.
// When compiling for pixel shader 2.0, the lit intrinsic uses more slots
// than doing this directly ourselves, so we don't use the intrinsic.
// E: Eye-Vector
// N: Unit vector normal in world space
//-----------------------------------------------------------------------------
ColorPair ComputePerPixelLights(float3 E, float3 N)
{
	ColorPair result;
	
	result.Diffuse = AmbientLightColor;
	result.Specular = 0;
	
	// Light0
	float3 L = -DirLight0Direction;
	float3 H = normalize(E + L);
	float dt = max(0,dot(L,N));
    result.Diffuse += DirLight0DiffuseColor * dt;
    if (dt != 0)
		result.Specular += DirLight0SpecularColor * pow(max(0,dot(H,N)), SpecularPower);

	// Light1
	L = -DirLight1Direction;
	H = normalize(E + L);
	dt = max(0,dot(L,N));
    result.Diffuse += DirLight1DiffuseColor * dt;
    if (dt != 0)
	    result.Specular += DirLight1SpecularColor * pow(max(0,dot(H,N)), SpecularPower);
    
	// Light2
	L = -DirLight2Direction;
	H = normalize(E + L);
	dt = max(0,dot(L,N));
    result.Diffuse += DirLight2DiffuseColor * dt;
    if (dt != 0)
	    result.Specular += DirLight2SpecularColor * pow(max(0,dot(H,N)), SpecularPower);
    
    result.Diffuse *= DiffuseColor;
    //result.Diffuse += EmissiveColor;
    result.Specular *= SpecularColor;
		
	return result;
}

struct VS_IN
{
	float3 position : POSITION;
	float3 normal : NORMAL;
	float2 tex : TEXCOORD0;
};

struct PS_IN
{
	float4 pos : POSITION;
	float3 posws : TEXCOORD0;
	float3 normal : TEXCOORD1;
	float2 tex : TEXCOORD2;
};

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;
	
	output.pos = mul(float4(input.position, 1), WorldViewProjection);
	output.posws = mul(float4(input.position, 1), World).xyz;
	output.normal = input.normal;
	output.tex = input.tex;
	
	return output;
}

float4 PS(PS_IN input) : COLOR0
{
	float3 posToEye = EyePosition - input.posws.xyz;
	
	float3 N = normalize(input.normal);
	float3 E = normalize(posToEye);
	
	ColorPair lightResult = ComputePerPixelLights(E, N);
	
	//float4 diffuse = tex2D(TextureSampler, pin.TexCoord) * float4(lightResult.Diffuse * Diffuse.rgb, Diffuse.a);
	float4 diffuse = float4(lightResult.Diffuse, Alpha);
	float4 color = diffuse + float4(lightResult.Specular, 0);
	//color.rgb = lerp(color.rgb, FogColor, pin.PositionWS.w);
	color.rgb = color.rgb;
	
	return color;
}

technique Render
{
	pass P0
	{
		CullMode = None;
		//FillMode = Wireframe;

		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}