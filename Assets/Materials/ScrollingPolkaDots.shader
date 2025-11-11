Shader "Unlit/ScrollingPolkaDots"
{
Properties
    {
        _DotColor ("Dot Color", Color) = (1, 1, 1, 1)
        _DotSize ("Dot Size", Range(0.01, 50)) = 0.2
        _Spacing ("Spacing", Range(0.1, 50)) = 1
        _ScrollSpeed ("Scroll Speed", Range(0, 5)) = 1
        _ScrollDirection ("Scroll Direction", Vector) = (1, 0, 0, 0)
        _RowOffset ("Row Offset", Range(0, 1)) = 0.5
        _ColumnOffset ("Column Offset", Range(0, 1)) = 0.0
        _AspectRatio ("Aspect Ratio", Float) = 1.0
        _TopColor ("Top Gradient Color", Color) = (0, 0, 0, 1)
        _BottomColor ("Bottom Gradient Color", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        // Enable transparency
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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

            float4 _DotColor;
            float _DotSize;
            float _Spacing;
            float _ScrollSpeed;
            float4 _ScrollDirection;
            float _RowOffset;
            float _ColumnOffset;
            float _AspectRatio;
            float4 _TopColor;
            float4 _BottomColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Correct UV aspect ratio
                float aspect = _AspectRatio;
                o.uv = float2(v.uv.x, v.uv.y * aspect);
                return o;
            }

            float circle(float2 uv, float radius)
            {
                // Return 1 inside the circle, 0 outside
                return smoothstep(radius, radius - 0.01, length(uv));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Scroll UVtime and direction
                float2 scroll = _ScrollDirection.xy * _ScrollSpeed * _Time.y;

                // Scale and scroll UV
                float2 uv = i.uv * _Spacing - scroll;

                // Apply row and column offset
                float rowOffset = (fmod(floor(uv.y), 2.0) * _RowOffset);
                float colOffset = (fmod(floor(uv.x), 2.0) * _ColumnOffset);
                uv.x += rowOffset;
                uv.y += colOffset;

                float2 uvTiled = frac(uv);// Create a grid by taking the fractionalpart
                float2 centeredUV = uvTiled - 0.5;// Move to the center of the tile

                centeredUV.x /= _AspectRatio;// Correct aspect ratio 

                float dot = circle(centeredUV, _DotSize * 0.5);// Create the polka dot shape (centered circle)

                // Create gradient background fromtop tyo bottom
                float gradientFactor = saturate(i.uv.y);
                fixed4 backgroundColor = lerp(_BottomColor, _TopColor, gradientFactor);

                // Set dot and background color b
                fixed4 col = lerp(backgroundColor, _DotColor, dot);

                return col;
            }
            ENDCG
        }
    }
}
