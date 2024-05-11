Shader "UI/RendererShader602"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _UVTex("Texture", 2D) = "white" {}
        _RotationAngle("Rotation Angle", Range(0, 360)) = 0
    }

        SubShader
        {
            // No culling or depth
            Cull Off ZWrite Off ZTest Always

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                uniform float _RotationAngle;
                sampler2D _MainTex;
                sampler2D _UVTex;

                v2f vert(appdata v)
                {
                    v2f o;
                    // Apply rotation transform to the vertex position
                    float angleRadians = radians(_RotationAngle);
                    float cosAngle = cos(angleRadians);
                    float sinAngle = sin(angleRadians);
                    float2x2 rotationMatrix = float2x2(cosAngle, sinAngle, -sinAngle, cosAngle);
                    float4 position = float4(v.vertex.xy, 0, 1);
                    float2 transformedPosition = mul(rotationMatrix, position.xy);
                    o.vertex = UnityObjectToClipPos(float4(transformedPosition, 0, 1));
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    fixed4 uv4 = tex2D(_UVTex, i.uv);
                    float y = 1.1643 * (col.a - 0.0625);
                    float u = (uv4.r * 15 * 16 + uv4.g * 15) / 255 - 0.5;
                    float v = (uv4.b * 15 * 16 + uv4.a * 15) / 255 - 0.5;
                    float r = y + 1.770 * u;
                    float g = y - 0.344 * u - 0.714 * v;
                    float b = y + 1.403 * v;
                    col.rgba = float4(r, g, b, 1.f);
                    return col;
                }
                ENDCG
            }
        }
}