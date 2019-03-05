Shader "Custom/OutlineShader"
{
	Properties
    {
        _Color ("Base Color", Color) = (1,0.08,0.9,1)
		_MainTex("Main Texture", 2D) = "white" {} // A
        _ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
        ShadowThreshold ("Shadow Range", Range(0,1)) = 0.4
		[MaterialToggle] ReceiveShadows("Receive Shadows", Float) = 1

		//OUTLINE
		[MaterialToggle] TurnOnOutline("Turn On Outline", Float) = 1
		_OutlineTex("Outline Texture", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (0.96,0.54,0,1)
		_Outline("Outline width", Range(0.0, 5)) = 1.5
		
		_SrcBlend("Blend Source", Float) = 5
		_DstBlend("Blend Destination", Float) = 10
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
				//"Queue" = "Transparent"
				//"IgnoreProjector" = "True"
				"RenderType" = "Opaque"
			}
			Cull Off
				
			ZWrite Off // Desliga o outline em contraste com outras meshes.

			//ZTest Less
			//ZTest Greater
			//ZTest LEqual
			//ZTest GEqual 
			//ZTest Equal 
			//ZTest NotEqual 
			//ZTest Always


			//Blend[_SrcBlend][_DstBlend]
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
				float3 normal = mul((float3x3) UNITY_MATRIX_MV, v.normal);
				normal.x *= UNITY_MATRIX_P[0][0];
				normal.y *= UNITY_MATRIX_P[1][1];
				//o.vertex.xy += normal.xy * _Outline;

				if (TurnOnOutline) {
					o.vertex.xy += normal.xy * _Outline;
				}
				else {
					_Outline = 0;
				}

				o.color = _OutlineColor;
				return o;
			}
			
			half4 frag(v2f i) : COLOR {
						
				//fixed4 col = tex2D(_MainTex, i.uv); // A
				//fixed4 col = (0, 0, 0, 0);

				/*if (TurnOnOutline) {
					fixed4 colorOutline = i.color;
					float2 uvOutline = i.uv;*/

					/*#ifdef MOVEOUTLINECOLOR_ON
						uvOutline *= _Time.x * _SpeedOutline;
					#endif*/

					/*if (MoveOutlineColor){
						uvOutline *= _Time.x * _SpeedOutline;
					}
							
					//fixed4 MainTexture = tex2D(_MainTex, i.uv);
					uvOutline = uvOutline - i.uv;
					
					col = tex2D(_OutlineTex, uvOutline) * colorOutline;					
					//col -= MainTexture;
					
				}*/
				
				return _OutlineColor;
			}
			ENDCG
		}		

		 Pass
        {
			Name "BASE"
			Tags {"LightMode" = "ForwardBase"}
 
			//Cull Off
				
			//ZWrite Off // Desliga o outline em contraste com outras meshes.

			//ZTest Less
			//ZTest Greater
			//ZTest LEqual
			//ZTest GEqual 
			//ZTest Equal 
			//ZTest NotEqual 
			//ZTest Always

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
 
				#include "UnityCG.cginc"
				float4 _LightColor0;

				//#pragma multi_compile _ RECEIVESHADOWS_ON
				#pragma multi_compile_fwdbase //S
				#include "AutoLight.cginc" //S
           
			
				float ReceiveShadows;
				float4 _Color;
				float4 _ShadowColor;
				float ShadowThreshold;
 
           
				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0; // A
					float4 posWorld : TEXCOORD1;
					float3 normalDir : TEXCOORD2;
					LIGHTING_COORDS(2, 3) //S
				};
 
				v2f vert (appdata IN)
				{
					v2f OUT;

					OUT.uv = IN.uv; // A
				

				   OUT.pos = UnityObjectToClipPos(IN.vertex);
               
					TRANSFER_VERTEX_TO_FRAGMENT(OUT);
 
					float4x4 modelMatrix = unity_ObjectToWorld;
					float4x4 modelMatrixInverse = unity_WorldToObject;
 
					OUT.posWorld = mul(modelMatrix, IN.vertex);
					OUT.normalDir = normalize(
						mul(float4(IN.normal, 0.0), modelMatrixInverse).xyz
					);
 
					return OUT;
				}
 
				float4 frag (v2f IN) : COLOR
				{
					fixed4 c = tex2D(_MainTex, IN.uv); // A

					float3 normalDirection = normalize(IN.normalDir);
					float3 lightDirection;
					float attenuation;
					float3 fragmentColor;

					lightDirection = normalize(_WorldSpaceLightPos0).xyz;

					//#ifdef RECEIVESHADOWS_ON
					if (ReceiveShadows == 1)
					{
						attenuation = LIGHT_ATTENUATION(IN); //S
						fragmentColor = _LightColor0.rgb * (_ShadowColor.rgb) * _Color.rgb;
						if (attenuation * max(0.0, dot(normalDirection, lightDirection)) >= ShadowThreshold) {
							fragmentColor = _LightColor0.rgb * _Color.rgb; // lit fragment color
						}
					}
					else {
						attenuation = 1.0;
						fragmentColor = _LightColor0.rgb  * _Color.rgb;
						if (attenuation * max(0.0, dot(normalDirection, lightDirection)) >= ShadowThreshold) {
							fragmentColor = _LightColor0.rgb * _Color.rgb; // lit fragment color
						}
					}

					//float4 col = float4(fragmentColor, 1.0);
					float4 col = float4(fragmentColor, attenuation);
					col = c * col; // A
					return col;
				}
				ENDCG
		}

	}
	Fallback "Diffuse"
}