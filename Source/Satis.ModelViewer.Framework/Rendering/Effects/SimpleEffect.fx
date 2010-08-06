matrix WorldViewProjection : WORLDVIEWPROJECTION;
matrix World : WORLD;

matrix LightViewProjection;

float3 AmbientLightColor = float3(0.05333332, 0.09882354, 0.1819608);

float3 LightDirection;
float3 LightDiffuseColor = float3(1, 0.9607844, 0.8078432);
float3 LightSpecularColor = float3(1, 0.9607844, 0.8078432);

float3 DiffuseColor = float3(1, 0, 0);
float3 SpecularColor = float3(1, 1, 1);
float SpecularPower = 8;
float Alpha;

float3 EyePosition;

bool SpecularEnabled;

bool ShadowsEnabled;

float ShadowMapSize;

texture ShadowMap;
sampler ShadowMapSampler = sampler_state
{
    Texture = <ShadowMap>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
	float3 position : POSITION;
	float3 normal : NORMAL;
	float2 tex : TEXCOORD0;
};

struct PixelShaderInput
{
	float4 pos : POSITION;
	float4 posws : TEXCOORD0;
	float3 normal : TEXCOORD1;
	float3 viewDirection : TEXCOORD2;
	float2 tex : TEXCOORD3;
};

struct PixelShaderInputCreateShadowMap
{
	float4 pos : POSITION;
	float2 depth : TEXCOORD0;
};

PixelShaderInputCreateShadowMap VSCreateShadowMap(VertexShaderInput input)
{
	PixelShaderInputCreateShadowMap output = (PixelShaderInputCreateShadowMap)0;
	
	output.pos = mul(float4(input.position, 1), LightViewProjection);
	output.depth = output.pos.zw;
	
	return output;
}

float2 ComputeMoments(float depth)
{
	float2 moments;  

	// First moment is the depth itself.  
	moments.x = depth;  

	// Compute partial derivatives of depth.  
	float dx = ddx(depth);  
	float dy = ddy(depth);  

	// Compute second moment over the pixel extents.  
	moments.y = depth * depth + 0.25 * (dx * dx + dy * dy);
	return moments;
}

float4 PSCreateShadowMap(PixelShaderInputCreateShadowMap input) : COLOR0
{
	float depth = input.depth.x / input.depth.y;
	depth = depth * 0.5 + 0.5; // Don't forget to move away from unit cube ([-1,1]) to [0,1] coordinate system

	return float4(ComputeMoments(depth), 0, 1);
}

PixelShaderInput VS(VertexShaderInput input)
{
	PixelShaderInput output = (PixelShaderInput)0;
	
	output.pos = mul(float4(input.position, 1), WorldViewProjection);
	output.posws = mul(float4(input.position, 1), World);
	output.normal = mul(input.normal, World);
	output.viewDirection = EyePosition - output.posws;
	output.tex = input.tex;
	
	return output;
}

float ChebyshevUpperBound(float2 moments, float t)
{
	// One-tailed inequality valid if t > Moments.x  
	float p = (t <= moments.x);

	// Compute variance.  
	float variance = moments.y - (moments.x * moments.x);  
	//float minVariance = 0.000001;
	float minVariance = 0.1;
	variance = max(variance, minVariance);

	// Compute probabilistic upper bound.  
	float d = t - moments.x;  
	float p_max = variance / (variance + d * d);  
	return max(p, p_max);
}

float4 PS(PixelShaderInput input) : COLOR0
{
	// transform from RT space to texture space.
	float4 positionInLightSpace = mul(input.posws, LightViewProjection);
	float2 shadowTexCoords = positionInLightSpace.xy;
	shadowTexCoords = shadowTexCoords * 0.5 + float2(0.5, 0.5); // I think the problem is somewhere here.
	shadowTexCoords.y = 1.0f - shadowTexCoords.y;
	shadowTexCoords = shadowTexCoords / positionInLightSpace.w;
	shadowTexCoords = shadowTexCoords - float2(0.5 / ShadowMapSize, 0.5 / ShadowMapSize);

	// We retrive the two moments previously stored (depth and depth*depth)
	float2 moments = tex2D(ShadowMapSampler, shadowTexCoords.xy).rg;

	float distanceToLightPlane = (positionInLightSpace.z / positionInLightSpace.w) * 0.5 + float2(0.5, 0.5);
	float shadow = ChebyshevUpperBound(moments, distanceToLightPlane);

	float3 normal = normalize(input.normal);
	float3 viewDirection = normalize(input.viewDirection);

	// Lambertian.
	float diffuseFactor = max(0, dot(LightDirection, normal));
	float3 diffuse = LightDiffuseColor * DiffuseColor * diffuseFactor;
	
	// Phong.
	float3 halfVector = normalize(LightDirection + viewDirection);
	float specularFactor = pow(max(0, dot(halfVector, normal)), SpecularPower);
	float3 specular = LightSpecularColor * SpecularColor * specularFactor;

	// Check to see if this pixel is in front or behind the value in the shadow map
	// The bias is used to prevent floating point errors that occur when
  // the pixel of the occluder is being drawn.
	float3 colour = AmbientLightColor;
	if (!ShadowsEnabled || shadow == 1.0)
	{
		colour += diffuse;
		if (SpecularEnabled)
			colour += specular;
	}
	else
		colour += diffuse * 0.6f; // Dim diffuse light to prevent surface appearing flat in shadow.

	return float4(colour, Alpha);
}

technique RenderShadowMap
{
	pass P0
	{
		CullMode = CW;

		VertexShader = compile vs_3_0 VSCreateShadowMap();
		PixelShader = compile ps_3_0 PSCreateShadowMap();
	}
}

technique RenderScene
{
	pass P0
	{
		CullMode = CCW;

		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}