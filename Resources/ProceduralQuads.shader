Shader "ProceduralQuads" {
	Properties { }
	SubShader {
		Tags {
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
        }
        Cull Off
        ZWrite Off
        // Lighting Off
        // Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
            #pragma geometry geom
			#pragma fragment frag
            #pragma multi_compile_instancing
			#include "UnityCG.cginc"

			struct v2g {
                float4 pos : SV_POSITION;
                uint vertexID: TEXCOORD0;
            };
			struct g2f {
				float4 pos: SV_POSITION;
                uint vertexID: TEXCOORD0;
			};
            struct RenderData {
                float3 Position;
                float Color;
            };

			StructuredBuffer<RenderData> _RenderData;

			v2g vert(uint vertexID: SV_VertexID) 
            {
                RenderData prop = _RenderData[vertexID];

				v2g f;
                f.vertexID = vertexID;
                f.pos = float4(prop.Position, 1.0);

				return f;
			}

            [maxvertexcount(6)]
            void geom(point v2g input[1], inout TriangleStream<g2f> triangleStream)
            {
                const float4 offset0 = float4(-0.5, 0, -0.5, 0);
                const float4 offset1 = float4(-0.5, 0, 0.5, 0);
                const float4 offset2 = float4(0.5, 0, 0.5, 0);
                const float4 offset3 = float4(0.5, 0, -0.5, 0);

                float4 center = input[0].pos;
                uint vid = input[0].vertexID;

                g2f vd1;
                vd1.pos = UnityObjectToClipPos(center + offset0);
                vd1.vertexID = vid;

                g2f vd2;
                vd2.pos = UnityObjectToClipPos(center + offset1);
                vd2.vertexID = vid;

                g2f vd3;
                vd3.pos = UnityObjectToClipPos(center + offset2);
                vd3.vertexID = vid;
                
                g2f vd4;
                vd4.pos = UnityObjectToClipPos(center + offset3);
                vd4.vertexID = vid;

                triangleStream.Append(vd1);
                triangleStream.Append(vd2);
                triangleStream.Append(vd3);

                triangleStream.Append(vd1);
                triangleStream.Append(vd3);
                triangleStream.Append(vd4);

                triangleStream.RestartStrip(); 
            }

			fixed4 frag(g2f f) : SV_Target{
                float c = _RenderData[f.vertexID].Color;

				return fixed4(c, c, c, 1.0);
			}
			ENDCG
		}
	}
}