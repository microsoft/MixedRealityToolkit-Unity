using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;


public class MaterialParameterCecker : MonoBehaviour
{
    //TargetMaterial
    public Material material;
    //SliderValueText
    public Text  SliderValueText;
    public List<Text> SliderValueTextList;

    //HoverColor
    private float hoverRed;
    private float hoverGreen;
    private float hoverBule;
    
    //InnerGlowColor
    private float InnerGlowRed;
    private float InnerGlowGreen;
    private float InnerGlowBlue;

    //EnvironmentColor x
    private float EnvironmentXRed;
    private float EnvironmentXGreen;
    private float EnvironmentXBlue;

    //EnvironmentColor y
    private float EnvironmentYRed;
    private float EnvironmentYGreen;
    private float EnvironmentYBlue;

    //EnvironmentColor z
    private float EnvironmentZRed;
    private float EnvironmentZGreen;
    private float EnvironmentZBlue;


    // RimLightColor
    private float RimcolorRed;
    private float RimcolorGreen;
    private float RimcolorBlue;

    private void SetText(float fvalue)
    {
        
                SliderValueText.text = fvalue.ToString();
        Debug.Log(SliderValueText.text);
        Debug.Log(fvalue);
    }

    private void SetColorText(List<Text> textList,float xRvalue,float xGvalue, float xBvalue,float yRvalue, float yGvalue, float yBvalue,float zRvalue, float zGvalue, float zBvalue)
    {
        if (textList.Count >= 3)
        {
            textList[0].text = xRvalue.ToString();
            textList[1].text = xGvalue.ToString();
            textList[2].text = xBvalue.ToString();
            textList[3].text = yRvalue.ToString();
            textList[4].text = yGvalue.ToString();
            textList[5].text = yBvalue.ToString();
            textList[6].text = zRvalue.ToString();
            textList[7].text = zGvalue.ToString();
            textList[8].text = zBvalue.ToString();
        }
    
    }

    //Smootheness
    public void OnSliderUpdatedColor_Smootheness(SliderEventData eventData)
    {
        this.material.SetFloat("_Smoothness", eventData.NewValue);
        SetText(eventData.NewValue );
    }
    //Metallic
    public void OnSliderUpdatedColor_Metallic(SliderEventData eventData)
    {
        this.material.SetFloat("_Metallic", eventData.NewValue);
        SetText( eventData.NewValue);
    }
    //NormalMapScale
    public void OnSliderUpdatedColor_NormalMapScale(SliderEventData eventData)
    {
        this.material.SetFloat("_NormalMapScale", eventData.NewValue);
        SetText( eventData.NewValue);
    }
//RoundCorners ON/Off
    public void OnSliderUpdatedColor_UnitRadius(SliderEventData eventData)
    {
        this.material.SetFloat("_RoundCorners", eventData.NewValue);
        SetText( eventData.NewValue);
    }
    //RoundCornerRadius
    public void OnSliderUpdatedColor_RoundCornerRadius(SliderEventData eventData)
    {
        this.material.SetFloat("_RoundCornerRadius", (eventData.NewValue*0.5f));
        SetText( eventData.NewValue*0.5f);
    }
    //RoundCorners Margin %
    public void OnSliderUpdatedColor_RoundCornerMargin(SliderEventData eventData)
    {
        this.material.SetFloat("_RoundCornerMargin", (eventData.NewValue*0.5f));
        SetText( eventData.NewValue*0.5f);
    }
    //ProximityLightColorOverride
    public void OnSliderUpdatedColor_EnableProximityLightColorOverride(SliderEventData eventData)
    {
        this.material.SetFloat("_EnableProximityLightColorOverride", (eventData.NewValue ));
        SetText( eventData.NewValue);
    }
    //ProximityLightOn/Off
    public void OnSliderUpdatedColor_ProximityLight(SliderEventData eventData)
    {
        this.material.SetFloat("_ProximityLight", (eventData.NewValue));
        SetText( eventData.NewValue);
    }
    //HoverLightColorRed
    public void OnSliderUpdatedColor_HoverColorOverride_R(SliderEventData eventData)
    {
        hoverRed = (((int)eventData.NewValue)/255);
        HoverColorOverride();
        SetColorText(SliderValueTextList,hoverRed,hoverGreen,hoverBule,0,0,0,0,0,0) ;
    }
    //HoverLightColorGreen
    public void OnSliderUpdatedColor_HoverColorOverride_G(SliderEventData eventData)
    {
        hoverGreen = ((int)eventData.NewValue / 255);
        HoverColorOverride();
        SetColorText(SliderValueTextList, hoverRed, hoverGreen, hoverBule,0,0,0,0,0,0);
    }
    //HoverLightColorBlue
    public void OnSliderUpdatedColor_HoverColorOverride_B(SliderEventData eventData)
    {
        hoverBule = ((int)eventData.NewValue/255);
        HoverColorOverride();
        SetColorText(SliderValueTextList, hoverRed, hoverGreen, hoverBule,0,0,0,0,0,0);
    }
    //HoverLightColor
    public void HoverColorOverride()
    {
        Color color = new Color(hoverRed,hoverGreen,hoverBule);
        this.material.SetColor("_HoverColorOverride",color);
    }
    //InnerGlowColorRed
    public void OnSliderUpdatedColor_InnerGlowColor_R(SliderEventData eventData)
    {
        InnerGlowRed = ((int)eventData.NewValue / 255);
        InnerGlowColor();
        SetColorText(SliderValueTextList, InnerGlowRed, InnerGlowGreen, InnerGlowBlue, 0, 0, 0, 0, 0, 0);
    }
    //InnerGlowColorGreen
    public void OnSliderUpdatedColor_InnerGlowColor_G(SliderEventData eventData)
    {
        InnerGlowGreen = ((int)eventData.NewValue / 255);
        InnerGlowColor();
        SetColorText(SliderValueTextList, InnerGlowRed, InnerGlowGreen, InnerGlowBlue, 0, 0, 0, 0, 0, 0);
    }
    //InnerGlowColorBlue
    public void OnSliderUpdatedColor_InnerGlowColor_B(SliderEventData eventData)
    {
        InnerGlowBlue = ((int)eventData.NewValue / 255);
        InnerGlowColor();
        SetColorText(SliderValueTextList, InnerGlowRed, InnerGlowGreen, InnerGlowBlue, 0, 0, 0, 0, 0, 0);
    }
    //InnerGlowColor
    public void InnerGlowColor()
    {
        Color color = new Color(InnerGlowRed, InnerGlowGreen, InnerGlowBlue);
        this.material.SetColor("_InnerGlowColor", color);
    }
    //InnerGlowPower
    public void OnSliderUpdatedColor_InnerGlowPower(SliderEventData eventData)
    {
        this.material.SetFloat("_InnerGlowPower", (eventData.NewValue*30f+2f));
        SetColorText(SliderValueTextList, InnerGlowRed, InnerGlowGreen, InnerGlowBlue,(eventData.NewValue*30f+2f),0,0,0,0,0);
    }
    //IridescenceIntensityRange
    public void OnSliderUpdatedColor_IridescenceIntensityRange(SliderEventData eventData)
    {
        this.material.SetFloat("_IridescenceIntensity", eventData.NewValue );
        SetText( eventData.NewValue);
    }
    //IridescenceThresholdThreshold
    public void OnSliderUpdatedColor_IridescenceThreshold(SliderEventData eventData)
    {
        this.material.SetFloat("_IridescenceThreshold", eventData.NewValue);
        SetText( eventData.NewValue);
    }
    //IridescenceAngle
    public void OnSliderUpdatedColor_IridescenceAngle(SliderEventData eventData)
    {
        this.material.SetFloat("_IridescenceAngle",( eventData.NewValue*1.56f - 0.78f));
        SliderValueText.text = (eventData.NewValue*1.56f-0.78f).ToString();
        SetText((eventData.NewValue*1.56f-0.78f));
    }
    //EnvironmentColor XRed
    public void OnSliderUpdatedColor_EnvironmentColor_XR(SliderEventData eventData)
    {
        EnvironmentXRed = ((int)eventData.NewValue / 255);
        _EnvironmentColor_X();
        SetColorText(SliderValueTextList, EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue, EnvironmentYRed, EnvironmentYGreen, EnvironmentYBlue, EnvironmentZRed, EnvironmentZGreen, EnvironmentZBlue);
    }
    //EnvironmentColor XGreen
    public void OnSliderUpdatedColor_EnvironmentColor_XG(SliderEventData eventData)
    {
        EnvironmentXGreen = ((int)eventData.NewValue / 255);
        _EnvironmentColor_X();
        SetColorText(SliderValueTextList, EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue, EnvironmentYRed, EnvironmentYGreen, EnvironmentYBlue, EnvironmentZRed, EnvironmentZGreen, EnvironmentZBlue);
    }
    //EnvironmentColor XBlue
    public void OnSliderUpdatedColor_EnvironmentColor_XB(SliderEventData eventData)
    {
        EnvironmentXBlue = ((int)eventData.NewValue / 255);
        _EnvironmentColor_X();
        SetColorText(SliderValueTextList, EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue, EnvironmentYRed, EnvironmentYGreen, EnvironmentYBlue, EnvironmentZRed, EnvironmentZGreen, EnvironmentZBlue);
    }
    //EnvironmentColor
    public void _EnvironmentColor_X()
    {
        Color color = new Color(EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue);
        this.material.SetColor("_EnvironmentColorX", color);
    }
    //EnvironmentColor YRed
    public void OnSliderUpdatedColor_EnvironmentColor_YR(SliderEventData eventData)
    {
        EnvironmentYRed = ((int)eventData.NewValue / 255);
        _EnvironmentColor_Y();
        SetColorText(SliderValueTextList, EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue, EnvironmentYRed, EnvironmentYGreen, EnvironmentYBlue, EnvironmentZRed, EnvironmentZGreen, EnvironmentZBlue);
    }
    //EnvironmentColor YGreen
    public void OnSliderUpdatedColor_EnvironmentColor_YG(SliderEventData eventData)
    {
        EnvironmentYGreen = ((int)eventData.NewValue / 255);
        _EnvironmentColor_Y();
        SetColorText(SliderValueTextList, EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue, EnvironmentYRed, EnvironmentYGreen, EnvironmentYBlue, EnvironmentZRed, EnvironmentZGreen, EnvironmentZBlue);
    }
    //EnvironmentColor YBlue
    public void OnSliderUpdatedColor_EnvironmentColor_YB(SliderEventData eventData)
    {
        EnvironmentYBlue = ((int)eventData.NewValue / 255);
        _EnvironmentColor_Y();
        SetColorText(SliderValueTextList, EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue, EnvironmentYRed, EnvironmentYGreen, EnvironmentYBlue, EnvironmentZRed, EnvironmentZGreen, EnvironmentZBlue);
    }
    //EnvironmentColor
    public void _EnvironmentColor_Y()
    {
        Color color = new Color(EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue);
        this.material.SetColor("_EnvironmentColorY", color);
    }
    //EnvironmentColor ZRed
    public void OnSliderUpdatedColor_EnvironmentColor_ZR(SliderEventData eventData)
    {
        EnvironmentZRed = ((int)eventData.NewValue / 255);
        _EnvironmentColor_Z();
        SetColorText(SliderValueTextList, EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue, EnvironmentYRed, EnvironmentYGreen, EnvironmentYBlue, EnvironmentZRed, EnvironmentZGreen, EnvironmentZBlue);
    }
    //EnvironmentColor ZGreen
    public void OnSliderUpdatedColor_EnvironmentColor_ZG(SliderEventData eventData)
    {
        EnvironmentZGreen = ((int)eventData.NewValue / 255);
        _EnvironmentColor_Z();
        SetColorText(SliderValueTextList, EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue, EnvironmentYRed, EnvironmentYGreen, EnvironmentYBlue, EnvironmentZRed, EnvironmentZGreen, EnvironmentZBlue);
    }
    //EnvironmentColor ZBlue
    public void OnSliderUpdatedColor_EnvironmentColor_ZB(SliderEventData eventData)
    {
        EnvironmentZBlue = ((int)eventData.NewValue / 255);
        _EnvironmentColor_Z();
        SetColorText(SliderValueTextList, EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue, EnvironmentYRed, EnvironmentYGreen, EnvironmentYBlue, EnvironmentZRed, EnvironmentZGreen, EnvironmentZBlue);
    }
    //EnvironmentColor
    public void _EnvironmentColor_Z()
    {
        Color color = new Color(EnvironmentZRed, EnvironmentZGreen, EnvironmentZBlue);
        this.material.SetColor("_EnvironmentColorZ", color);
    }
    //DirectionalLight
    public void OnSliderUpdate_DirectionalLight(SliderEventData eventData)
    {
        this.material.SetFloat("_DirectionalLight", eventData.NewValue);
        SetText( eventData.NewValue);
    }
    //NormalMap
    public void OnSliderUpdate_NormalMapScale(SliderEventData eventData)
    {
        this.material.SetFloat("_NormalMapScale", eventData.NewValue);
        SetText( eventData.NewValue);
    }
    //RimLightColorRed
    public void OnSliderUpdate_RimLightColor_R(SliderEventData eventData)
    {
        RimcolorRed = ((int)eventData.NewValue / 255);
        RimLightColor();
        SetColorText(SliderValueTextList, RimcolorRed, RimcolorGreen, RimcolorBlue,0,0,0,0,0,0);
    }
    //RimLightColorGreen
    public void OnSliderUpdate_RimLightColor_G(SliderEventData eventData)
    {
        RimcolorGreen = ((int)eventData.NewValue / 255);
        RimLightColor();
        SetColorText(SliderValueTextList, RimcolorRed, RimcolorGreen, RimcolorBlue, 0, 0, 0, 0, 0, 0);
    }
    //RimLightColorBlue
    public void OnSliderUpdate_RimLightColor_B(SliderEventData eventData)
    {
        RimcolorBlue = ((int)eventData.NewValue / 255);
        RimLightColor();
        SetColorText(SliderValueTextList, RimcolorRed, RimcolorGreen, RimcolorBlue, 0, 0, 0, 0, 0, 0);
    }
    public void RimLightColor()
    {
        Color color = new Color(RimcolorRed, RimcolorGreen, RimcolorBlue);
        this.material.SetColor("_RimColor",color);
    }
    //RimPower
    public void OnSliderUpdate_RimPower(SliderEventData eventData)
    {
        this.material.SetFloat("_RimPower", eventData.NewValue);
        SetColorText(SliderValueTextList, RimcolorRed, RimcolorGreen, RimcolorBlue,(eventData.NewValue),0,0,0,0,0);
    }
    //FadeBegindistance
    public void OnSliderUpdate_FadeBegindistane(SliderEventData eventData)
    {
        this.material.SetFloat("_FadeBeginDistance",eventData.NewValue);
        SetText( eventData.NewValue);
    }
    //FadeCompleteComplete
    public void OnSliderUpdate_FadeBCompleteDistance(SliderEventData eventData)
    {
        this.material.SetFloat("_FadeCompleteDistance", eventData.NewValue);
        SetText( eventData.NewValue);
    }
    //FadeMainValue
    public void OnSliderUpdate_FadeMainValue(SliderEventData eventData)
    {
        this.material.SetFloat("_FadeMainValue",eventData.NewValue);
        SetText( eventData.NewValue);
    }
    //EdgeSmoothingValue
    public void OnSliderUpdate_EdgeSmoothingValue(SliderEventData eventData)
    {
        this.material.SetFloat("_EdgeSmoothingValue", (eventData.NewValue*0.2f));
        SetText( (eventData.NewValue*0.2f));
    }
    //VertexExtrusionValue
    public void OnSliderUpdate_VertexExtrusionValue(SliderEventData eventData)
    {
        this.material.SetFloat("_VertexExtrusionValue", eventData.NewValue);
        SetText( eventData.NewValue);
    }
    //RefractiveIndex
    public void OnSliderUpdate_RefractiveIndex(SliderEventData eventData)
    {
        this.material.SetFloat("_RefractiveIndex", (eventData.NewValue*3f));
        SetText( (eventData.NewValue*3f)); ;
    }
    //AlbedoAlphaCutoff
    public void OnSliderUpdate_Cutoff(SliderEventData eventData)
    {
        this.material.SetFloat("_Cutoff", eventData.NewValue);
        SetText( eventData.NewValue);
    }
    //ClippingBorder
    public void OnSliderUpdate_ClippingBorder(SliderEventData eventData)
    {
        this.material.SetFloat("_ClippingBorderWidth", eventData.NewValue);
        SetText( eventData.NewValue);
    }
}

