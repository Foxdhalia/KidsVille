Shader "Custom/RampColorShader"
{
    Properties
    {
		_Color("Base Color", Color) = (1,0.08,0.9,1)
        _MainTex ("Texture", 2D) = "white" {}
		_ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
        ShadowThreshold ("Shadow Range", Range(0,1)) = 0.4
		[MaterialToggle] ReceiveShadows("Receive Shadows", Float) = 1

		// Ramp
		[MaterialToggle] RampActive("Ramp Effect", Float) = 1
        _RampTex ("Ramp Texture", 2D) = "white" {}
		_RampColor("Ramp Color", Color) = (1, 1, 1, 1)
		_RampOpacity("Ramp Opacity", Range(0, 1)) = 0.4
		_RampSpeed("Ramp Speed", Float) = 1
		//_ScrollX("Scroll X", Range(-5,5)) = 1
		//_ScrollY("Scroll Y", Range(-5,5)) = 1
		
    }
    SubShader
    {
       
       Pass
        {

			Tags{ "RenderType" = "Transparent" }

			Blend SrcAlpha OneMinusSrcAlpha // Normal Transparency
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
 
				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;				
					float2 uv3 : TEXCOORD3;				
					float3 normal : NORMAL;
				};

				sampler2D _MainTex; float4 _MainTex_ST;
				sampler2D _RampTex; float4 _RampTex_ST;
			
				float ReceiveShadows;
				float RampActive;
				float4 _Color;
				float4 _RampColor;
				float4 _ShadowColor;
				float ShadowThreshold;
				float _RampOpacity;
 
           
				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0; // A
					float4 posWorld : TEXCOORD1;
					float3 normalDir : TEXCOORD2;
					float2 uv3 : TEXCOORD3;
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

				float _ScrollX;
				float _RampSpeed;
				//float _ScrollY;
		
 
				float4 frag (v2f IN) : COLOR
				{
					fixed4 c = tex2D(_MainTex, IN.uv); // A

					float3 normalDirection = normalize(IN.normalDir);
					float3 lightDirection;
					float attenuation;
					float3 fragmentColor;

					lightDirection = normalize(_WorldSpaceLightPos0).xyz;
										
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
					
					float4 col = float4(fragmentColor, attenuation);
					col = c * col;
					
					if (RampActive) {
						//_ScrollX = _RampSpeed * _Time.y;
						_ScrollX = _RampSpeed + sin(_Time.y);
						
						float2 uvRamp = float2(IN.uv3.x + _ScrollX, IN.uv3.y);
						fixed4 rampTex = tex2D(_RampTex, uvRamp) * _RampColor; // A
						rampTex *=  _RampOpacity;
						col += rampTex;
					}

					
					return col;
				}
				ENDCG
		}

	}
	Fallback "Diffuse"
}