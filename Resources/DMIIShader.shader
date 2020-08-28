Shader "DMIIShader" {
	Properties { }
	SubShader {
		Tags {
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile_instancing
			#include "UnityCG.cginc"

            struct vertex {
				float4 loc	: POSITION;
			};
			struct v2f {
                float4 pos : SV_POSITION;
                uint instanceID: TEXCOORD0;
            };

            struct RenderData
            {
                float3 Position;
                float Color;
            };

            StructuredBuffer<RenderData> _RenderData;

			v2f vert(vertex v, uint instanceID: SV_InstanceID) 
            {
                RenderData prop = _RenderData[instanceID];

				v2f f;
                f.instanceID = instanceID;
                f.pos = UnityObjectToClipPos(v.loc + prop.Position);

				return f;
			}

			fixed4 frag(v2f f) : SV_Target{
                float c = _RenderData[f.instanceID].Color;

				return fixed4(c, c, c, 1.0);
			}
			ENDCG
		}
	}
}