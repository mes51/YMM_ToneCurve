struct SplinePoint
{
    float X;

    float A;

    float B;

    float C;

    float D;
};

cbuffer constants : register(b0)
{
    int Channel;
    
    int PointCount;
    
    SplinePoint Points[256];
};

Texture2D InputTexture : register(t0);

SamplerState InputSampler : register(s0);

float mapToCurve(float inValue)
{
    if (PointCount < 2)
    {
        return inValue;
    }

    SplinePoint firstPoint = Points[0];
    if (firstPoint.X > inValue)
    {
        return firstPoint.D;
    }

    int count = PointCount - 1;
    for (int i = 0; i < count; i++)
    {
        SplinePoint targetPoint = Points[i];
        if (targetPoint.X <= inValue && Points[i + 1].X >= inValue)
        {
            float dx = inValue - targetPoint.X;
            return saturate(((targetPoint.A * dx + targetPoint.B) * dx + targetPoint.C) * dx + targetPoint.D);
        }
    }

    return Points[count].D;
}

float4 main(float4 pos : SV_POSITION, float4 posScene : SCENE_POSITION, float4 uv0 : TEXCOORD0) : SV_Target
{
    float4 color = InputTexture.Sample(InputSampler, uv0.xy);

    switch (Channel)
    {
        case 0:
            if (color.a > 0.0)
            {
                color.r = mapToCurve(color.r / color.a) * color.a;
            }
            break;
        case 1:
            if (color.a > 0.0)
            {
                color.g = mapToCurve(color.g / color.a) * color.a;
            }
            break;
        case 2:
            if (color.a > 0.0)
            {
                color.b = mapToCurve(color.b / color.a) * color.a;
            }
            break;
        case 3:
            if (color.a > 0.0)
            {
                color.rgb /= color.a;
            }
            color.a = mapToCurve(color.a);
            color.rgb *= color.a;
            break;
    }

    return color;
}