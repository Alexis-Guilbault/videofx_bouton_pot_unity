Shader "Effects/ScreenBend"
{
    Properties
    {
        _MainTex ("Screen Texture", 2D) = "white" {}
        _CenterPosition ("Click Position (UV)", Vector) = (0.5, 0.5, 0, 0)
        _BendAmount ("Bend Amount (-1 to 1)", Range(-1, 1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        ZTest Always
        Cull Off
        ZWrite Off
        
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
            float4 _MainTex_ST;  // ← Added: texture scale/offset for TRANSFORM_TEX
            float4 _CenterPosition;
            float _BendAmount;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                // Distance from click point
                float2 dir = uv - _CenterPosition.xy;
                float dist = length(dir);
                
                // Distance from click point to nearest edge
                float2 clickToEdge = float2(
                    min(_CenterPosition.x, 1.0 - _CenterPosition.x),
                    min(_CenterPosition.y, 1.0 - _CenterPosition.y)
                );
                float maxDist = min(clickToEdge.x, clickToEdge.y);
                
                // Normalize distance (0 at click point, 1 at nearest edge)
                float normalizedDist = dist / maxDist;
                
                // Non-linear falloff: exponential decay
                float falloff = pow(max(0.0, 1.0 - normalizedDist), 2.0);
                
                // Direction of displacement
                float2 displacementDir = normalize(dir);
                
                // Apply inverse mapping for stability
                float displacementStrength = _BendAmount * dist * falloff * 2;
                float2 sourceUV = uv - displacementDir * displacementStrength;
                
                // Clamp to prevent texture bleeding
                sourceUV = clamp(sourceUV, float2(0.0, 0.0), float2(1.0, 1.0));
                
                return tex2D(_MainTex, sourceUV);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}