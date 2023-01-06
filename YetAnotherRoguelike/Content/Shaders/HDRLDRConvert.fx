#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler _sampler;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(_sampler, input.TextureCoordinates);
	//float3 ldrColor = float3(color.r, color.g, color.b);
	if (color.r > 1) {
		return float4(color.r, 0, 0, 0);
	}

	return float4(color.r, color.g, color.b, 0);
}

technique HDR_TO_LDR
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};