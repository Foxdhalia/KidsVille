Shader "Unlit/OutlineSepared"
{
 Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {} // A
        _ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
        ShadowThreshold ("Shadow Range", Range(0,1)) = 0.1
		[MaterialToggle] ReceiveShadows("Receive Shadows", Float) = 0

		//OUTLINE
		[MaterialToggle] TurnOnOutline("Turn On Outline", Float) = 1
		_OutlineTex("Outline Texture", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(0.0, 2)) = .005
		
		_SrcBlend("Blend Source", Float) = 0
		_DstBlend("Blend Destination", Float) = 0
		[MaterialToggle] MoveOutlineColor("Make outline moves", Float) = 0
		_SpeedOutline ("Speed Outline", Float) = 1
    }
		
		CGINCLUDE
			#include "UnityCG.cginc"			
			#pragma multi_compile _ TURNONOUTLINE_ON
			#pragma multi_compile _ MOVEOUTLINECOLOR_ON


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float3 normal : NORMAL;
			};

		sampler2D _MainTex; float4 _MainTex_ST;

		ENDCG // End CGINCLUDE
    
	SubShader
    {    
       
		Pass {
			Name "OUTLINE"
			Tags {
				//"LightMode" = "Always" 
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
			}
			Cull Off
				
			ZWrite Off // Desliga o outline em contraste com outras meshes.

			//ZTest Less
			//ZTest Greater
			//ZTest LEqual
			//ZTest GEqual 
			//ZTest Equal 
			//ZTest NotEqual 
			ZTest Always


			Blend[_SrcBlend][_DstBlend]
			// Tipos clássicos de Blend:
				//Blend SrcAlpha OneMinusSrcAlpha // Normal Transparency
				//Blend One One // Additive
				//Blend One OneMinusDstColor // Soft Additive
				//Blend DstColor Zero // Multiplicative
				//Blend DstColor SrcColor // 2x Multiplicative
			////////////////////////////////////////////////////////////////////


			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag					

			struct v2f
			{
				float2 uv : TEXCOORD0;				
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			float TurnOnOutline;
			sampler2D _OutlineTex;
			float4 _OutlineTex_ST;
			fixed4 _OutlineColor;
			float _Outline;
			float _SpeedOutline;

			float MoveOutlineColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);				

				float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				float2 offset = TransformViewToProjection(norm.xy); // Original	

				if (TurnOnOutline) {
					o.vertex.xy += offset * o.vertex.z * _Outline; // Original
				}
				else {
					_Outline = 0;
				}

				o.color = _OutlineColor;

				return o;
			}
			
			half4 frag(v2f i) : COLOR {
						
				//fixed4 col = tex2D(_MainTex, i.uv); // A
				fixed4 col = (0, 0, 0, 0);

				if (TurnOnOutline) {
					fixed4 colorOutline = i.color;
					float2 uvOutline = i.uv;

					/*#ifdef MOVEOUTLINECOLOR_ON
						uvOutline *= _Time.x * _SpeedOutline;
					#endif*/

					if (MoveOutlineColor){
						uvOutline *= _Time.x * _SpeedOutline;
					}
							
					//fixed4 MainTexture = tex2D(_MainTex, i.uv);
					uvOutline = uvOutline - i.uv;
					
					col = tex2D(_OutlineTex, uvOutline) * colorOutline;					
					//col -= MainTexture;
					
				}
				
				return col;
			}
			ENDCG
		}		

	}
	Fallback "Diffuse"
}
