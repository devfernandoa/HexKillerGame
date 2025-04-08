Shader "Custom/PixelArtShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Float) = 1.0
        _Color ("Color", Color) = (1, 1, 1, 1) // Add this line
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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

            sampler2D _MainTex;
            float _PixelSize;
            float4 _Color; // Add this line

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pixelatedUV = floor(i.uv * _PixelSize) / _PixelSize;
                fixed4 col = tex2D(_MainTex, pixelatedUV);
                return col * _Color; // Multiply by the color
            }
            ENDCG
        }
    }
}   