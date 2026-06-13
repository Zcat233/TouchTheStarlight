Shader "Dancing Line Fanmade/Standard/HDRColor"
{
    Properties 
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _Emission ("Emission Color", Color) = (0,0,0,0)
        _EmissionTex("Emission Texture", 2D) = "white"{}
        // 专门给触发器控制的独立强度乘数，默认是 1（保持原样不影响普通渲染）
        _EmissionIntensity("Emission Intensity Multiplier", Float) = 1.0
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        sampler2D _EmissionTex;
        fixed4 _Color;
        fixed4 _Emission;
        float _EmissionIntensity;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_EmissionTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            
            // 核心微调：最终发光 = 模板脚本控制的颜色 * 你的触发器脚本控制的强度乘数
            o.Emission = tex2D (_EmissionTex, IN.uv_EmissionTex) * _Emission * _EmissionIntensity;
        }
        ENDCG
    }
    FallBack "Diffuse"
}