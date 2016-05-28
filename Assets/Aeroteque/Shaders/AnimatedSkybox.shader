Shader "Custom/Animated Skybox" {

    Properties {
        _ColorBg ("Background color", Color) = (0.97, 0.67, 0.51, 0)
        _ColorStripe ("Stripe color", Color) = (0, 0.7, 0.74, 0)
        _ColorGround1 ("Bottom Color 1", Color) = (0, 0.7, 0.74, 0)
        _ColorGround2 ("Bottom Color 2", Color) = (0, 0.6, 0.74, 0)

        [Space]
        _Border ("Border", float) = 0.5

        [Space]
        _DirectionYaw ("Direction X angle", Range (0, 180)) = 0
        _DirectionPitch ("Direction Y angle", Range (0, 180)) = 0
        
        [Space]
        _BottomSpeed("Bottom color change speed", Range(0, 25)) = 5
        _StripesSpeed("Stripes speed", Range(0, 1.5)) = 0.25
        _StripesWidth("Stripes width", Range(0, 10)) = 5
        _StripesDistance("Stripes distance", Range(0, 1)) = 0.3
        
        [HideInInspector]
        _Direction ("Direction", Vector) = (0, 1, 0, 0)
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    struct appdata {
        float4 position : POSITION;
        float3 texcoord : TEXCOORD0;
    };
    
    struct v2f {
        float4 position : SV_POSITION;
        float3 texcoord : TEXCOORD0;
    };
    
    half4 _ColorStripe;
    half4 _ColorBg;
    half4 _ColorGround1;
    half4 _ColorGround2;
    half3 _Direction;
    half _Border;
    half _StripesSpeed;
    half _StripesWidth;
    half _StripesDistance;
    half _BottomSpeed;
    
    v2f vert (appdata v) {
        v2f o;
        o.position = mul(UNITY_MATRIX_MVP, v.position);
        o.texcoord = v.texcoord;
        return o;
    }
    
    fixed4 frag (v2f i) : COLOR {
        half b = dot(normalize(i.texcoord), half3(0, 1, 0)) * 0.5f + 0.5f;
        if (b < _Border) return lerp(_ColorGround1, _ColorGround2, sin(_Time.y * _BottomSpeed));
        half d = dot(normalize(i.texcoord), -1 * _Direction) * 0.5f + 0.5f;
        if ((d + _Time.y * _StripesSpeed) % _StripesDistance < 1 / _StripesWidth) return _ColorBg;
        return _ColorStripe;
    }

    ENDCG

    SubShader {
        Tags { "RenderType"="Background" "Queue"="Background" }

        Pass {
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }

    
    CustomEditor "GradientSkyboxEditor"
}
