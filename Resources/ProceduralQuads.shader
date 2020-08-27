Shader "ProceduralQuads" {
	Properties {
        
	}
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
                uint vertexID: TEXCOORD3;
			};

            struct RenderData
            {
                float3 Position;
                float4 Color;
            };

			StructuredBuffer<RenderData> _RenderData;

			v2g vert(appdata_full v, uint vertexID: SV_VertexID) 
            {
				v2g f;
                
                RenderData prop = _RenderData[vertexID];

                f.vertexID = vertexID;
                f.pos = float4(prop.Position, 1.0);

				return f;
			}

            [maxvertexcount(6)]
            void geom(point v2g input[1], inout TriangleStream<g2f> triangleStream)
            {
                RenderData prop = _RenderData[input[0].vertexID];

                float4 center = input[0].pos;

                float4 c1 = center + float4(-0.5, 0, -0.5, 0);
                float4 c2 = center + float4(-0.5, 0, 0.5, 0);
                float4 c3 = center + float4(0.5, 0, 0.5, 0);
                float4 c4 = center + float4(0.5, 0, -0.5, 0);

                g2f vd1 = (g2f)0;
                vd1.pos = UnityObjectToClipPos(c1);
                vd1.vertexID = input[0].vertexID;
                g2f vd2 = (g2f)0;
                vd2.pos = UnityObjectToClipPos(c2);
                vd2.vertexID = input[0].vertexID;
                g2f vd3 = (g2f)0;
                vd3.pos = UnityObjectToClipPos(c3);
                vd3.vertexID = input[0].vertexID;
                g2f vd4 = (g2f)0;
                vd4.pos = UnityObjectToClipPos(c4);
                vd4.vertexID = input[0].vertexID;

                triangleStream.Append(vd1);
                triangleStream.Append(vd2);
                triangleStream.Append(vd3);

                triangleStream.Append(vd1);
                triangleStream.Append(vd3);
                triangleStream.Append(vd4);

                triangleStream.RestartStrip();
            }

			float4 frag(g2f f) : SV_Target{
                RenderData prop = _RenderData[f.vertexID];

                float4 c = prop.Color;

				return c;
			}
			ENDCG
		}
	}
}