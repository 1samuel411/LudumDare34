Shader "Dissolve/Sprite"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		
		_Amount ("Amount", Range (0, 1)) = 0.5
		_StartAmount("StartAmount", float) = 0.1
		_Illuminate ("Illuminate", Range (0, 1)) = 0.5
		_Tile("Tile", float) = 1
		_DissColor ("DissColor", Color) = (1,1,1,1)
		_ColorAnimate ("ColorAnimate", vector) = (1,1,1,1)
		_DissolveSrc ("DissolveSrc", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
			//
			half4 _DissColor;
			half _Amount;
			half _StartAmount;
			half4 _ColorAnimate;
			half _Illuminate;
			half _Tile;
			static half3 Color = float3(1,1,1);
			//

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			//
			sampler2D _DissolveSrc;
			//
			
			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				//
				float ClipTex = tex2D (_DissolveSrc, IN.texcoord/_Tile).r ;
				float ClipAmount = ClipTex - _Amount;
				float Clip = 0;
				if (_Amount > 0)
				{
					if (ClipAmount <0)
					{
						Clip = 1; //clip(-0.1);
					
					}
					 else
					 {
					
						if (ClipAmount < _StartAmount)
						{
							if (_ColorAnimate.x == 0)
								Color.x = _DissColor.x;
							else
								Color.x = ClipAmount/_StartAmount;
				          
							if (_ColorAnimate.y == 0)
								Color.y = _DissColor.y;
							else
								Color.y = ClipAmount/_StartAmount;
				          
							if (_ColorAnimate.z == 0)
								Color.z = _DissColor.z;
							else
								Color.z = ClipAmount/_StartAmount;

							c.rgb  = (c.rgb *((Color.x+Color.y+Color.z))* Color*((Color.x+Color.y+Color.z)))/(1 - _Illuminate);
							
						
						}
					 }
				 }

				 
				if (Clip == 1)
				{
				clip(-0.1);
				}
				//
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
