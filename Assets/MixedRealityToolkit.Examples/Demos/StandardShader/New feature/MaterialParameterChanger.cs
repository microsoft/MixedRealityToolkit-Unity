using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.UI;
using System;

/// <summary>
/// This class changes the MRTKStandardShader parameter of the material attached using PinchSlider.
/// By acquiring each method in the event of PinchSlider.sc, you can see the function of MRTKStandardShader on the actual machine.
/// </summary>
public class MaterialParameterChanger : MonoBehaviour
{
    /// <summary>
    /// parameter
    /// </summary>
    //TargetMateria;
    public Material material;


    //SliderValueText
    public List<Text> SliderValueTextList;
    private int x;

    //parameter
    private float somootheness;
    private float roundcornermargin;
    private float roundcornerradius;

    private float hoverRed;
    private float hoverGreen;
    private float hoverBlue;

    private float InnerGlowRed;
    private float InnerGlowGreen;
    private float InnerGlowBlue;
    private float innergrowpower;

    private float EnvironmentXRed;
    private float EnvironmentXGreen;
    private float EnvironmentXBlue;
    private float EnvironmentYRed;
    private float EnvironmentYGreen;
    private float EnvironmentYBlue;
    private float EnvironmentZRed;
    private float EnvironmentZGreen;
    private float EnvironmentZBlue;

    private float rimcolorred;
    private float rimcolorgreen;
    private float rimcolorblue;

    private float fadebegindistance;
    private float fadecompletedistance;
    private float fademainvalue;

    private float metalic;

    private float iridescenceintensityrange;
    private float iridescencethreshold;
    private float iridescenceangle;
    /// <summary>
    /// Gets the material of the object to which this class is attached.
    /// </summary>
    void Start()
    {
        material = this.gameObject.GetComponent<Renderer>().material;
    }

    /// <summary>
    /// MRTKStandardShader parameter value is output as text.
    /// The value of each method is set as an argument.
    /// </summary>
    /// <param name="textList">Textlist</param>
    /// <param name="xRvalue">The first parameter set in the text list</param>
    /// <param name="xGvalue">The second parameter set in the text list</param>
    /// <param name="xBvalue">The third parameter set in the text list</param>
    /// <param name="yRvalue">The fourth parameter set in the text list</param>
    /// <param name="yGvalue">The fifth parameter set in the text list</param>
    /// <param name="yBvalue">The sixth parameter set in the text list</param>
    /// <param name="zRvalue">The seventh parameter set in the text list</param>
    /// <param name="zGvalue">The eighth parameter set in the text list</param>
    /// <param name="zBvalue">The ninth parameter set in the text list</param>
    private void SetText(List<Text> textList, float xRvalue, float xGvalue, float xBvalue, float yRvalue, float yGvalue, float yBvalue, float zRvalue, float zGvalue, float zBvalue)
    {
        x = textList.Count;
        Debug.Log(textList.Count);
        if (textList.Count >= 9)
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

        if (textList.Count >= 2)
        {
            textList[0].text = xRvalue.ToString();
            textList[1].text = xGvalue.ToString();
        }
        if (textList.Count >= 3)
        {
            textList[0].text = xRvalue.ToString();
            textList[1].text = xGvalue.ToString();
            textList[2].text = xBvalue.ToString();
        }
        if (textList.Count >= 4)
        {
            textList[0].text = xRvalue.ToString();
            textList[1].text = xGvalue.ToString();
            textList[2].text = xBvalue.ToString();
            textList[3].text = yRvalue.ToString();
        }
        if (textList.Count >= 1)
        {
            textList[0].text = xRvalue.ToString();
        }

    }
    /// <summary>
    /// Change the parameter of Smoothness of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdated_Smootheness(SliderEventData eventData)
    {
        somootheness = eventData.NewValue;
        material.SetFloat("_Smoothness", somootheness);
        //Set the parameter value as text.
        SetText(SliderValueTextList, somootheness, 0, 0, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of Metallic of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdated_Metallic(SliderEventData eventData)
    {
        metalic = eventData.NewValue;
        material.SetFloat("_Metallic", metalic);
        //Set the parameter value as text.
        SetText(SliderValueTextList, 0, metalic, 0, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    ///Change the parameter of RoundCorners Margin % of MRTKStandardShader. 
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdated_RoundCorners_Margin(SliderEventData eventData)
    {
        //RoundCorners Margin value has a value from 0 to 0.5
        roundcornermargin = eventData.NewValue * 0.5f;
        material.SetFloat("_RoundCornerMargin", roundcornermargin);
        //Set the parameter value as text.
        SetText(SliderValueTextList, roundcornermargin, roundcornerradius, 0, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of RoundCornerRadius of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdated_RpundCorners_Radius(SliderEventData eventData)
    {
        //RpundCorners Radius value has a value from 0 to 0.5
        roundcornerradius = eventData.NewValue * 0.5f;
        material.SetFloat("_RoundCornerRadius", roundcornerradius);
        SetText(SliderValueTextList, roundcornermargin, roundcornerradius, 0, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of HoverLightColorRed of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_HoverColorOverride_R(SliderEventData eventData)
    {
        hoverRed = eventData.NewValue;
        material.SetFloat("_RoundCornerRadius", roundcornerradius);
        SetText(SliderValueTextList, (int)(hoverRed * 255), (int)(hoverGreen * 255), (int)(hoverBlue * 255), 0, 0, 0, 0, 0, 0);
        HoverColorOverride();
    }
    /// <summary>
    /// Change the parameter of HoverLightColorGreen of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_HoverColorOverride_G(SliderEventData eventData)
    {
        hoverGreen = eventData.NewValue;
        SetText(SliderValueTextList, (int)(hoverRed * 255), (int)(hoverGreen * 255), (int)(hoverBlue * 255), 0, 0, 0, 0, 0, 0);
        HoverColorOverride();
    }
    /// <summary>
    /// Change the parameter of HoverLightColorBlue of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_HoverColorOverride_B(SliderEventData eventData)
    {
        hoverBlue = eventData.NewValue;
        SetText(SliderValueTextList, (int)(hoverRed * 255), (int)(hoverGreen * 255), (int)(hoverBlue * 255), 0, 0, 0, 0, 0, 0);
        HoverColorOverride();
    }
    /// <summary>
    /// Change the parameter of HoveLightColor of MRTKStandardShader.
    /// </summary>
    public void HoverColorOverride()
    {
        Color color = new Color(hoverRed, hoverGreen, hoverBlue);
        material.SetColor("_HoverColorOverride", color);
    }
    /// <summary>
    /// Change the parameter of InnerGlowColorRed of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_InnerGlowColor_R(SliderEventData eventData)
    {
        InnerGlowRed = eventData.NewValue;
        InnerGlowColor();
    }
    /// <summary>
    /// Change the parameter of InnerGlowColorGreen of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_InnerGlowColor_G(SliderEventData eventData)
    {
        InnerGlowGreen = eventData.NewValue;
        InnerGlowColor();
    }
    /// <summary>
    /// Change the parameter of InnerGlowColorBlue of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_InnerGlowColor_B(SliderEventData eventData)
    {
        InnerGlowBlue = eventData.NewValue;
        InnerGlowColor();
    }
    /// <summary>
    /// Change the parameter of InnerGlowColor of MRTKStandardShader.
    /// </summary>
    public void InnerGlowColor()
    {
        Color color = new Color(InnerGlowRed, InnerGlowGreen, InnerGlowBlue);
        this.material.SetColor("_InnerGlowColor", color);
        SetText(SliderValueTextList, (int)(InnerGlowRed * 255), (int)(InnerGlowGreen * 255), (int)(InnerGlowBlue * 255), innergrowpower, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of InnerGlowPower of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_InnerGlowPower(SliderEventData eventData)
    {
        //InnerGlowPower value has a value from 2 to 32
        innergrowpower = (eventData.NewValue * 30f + 2f);
        this.material.SetFloat("_InnerGlowPower", innergrowpower);
        SetText(SliderValueTextList, (int)(InnerGlowRed * 255), (int)(InnerGlowGreen * 255), (int)(InnerGlowBlue * 255), innergrowpower, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Set the slider value to EnvironmentXRed.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_EnvironmentColor_XR(SliderEventData eventData)
    {
        EnvironmentXRed = (eventData.NewValue);
        _EnvironmentColor_X();
        SetText(SliderValueTextList, (int)(EnvironmentXRed * 255), (int)(EnvironmentXGreen * 255), (int)(EnvironmentXBlue * 255), (int)(EnvironmentYRed * 255), (int)(EnvironmentYGreen * 255), (int)(EnvironmentYBlue * 255), (int)(EnvironmentZRed * 255), (int)(EnvironmentZGreen * 255), (int)(EnvironmentZBlue * 255));
    }
    /// <summary>
    /// Set the slider value to EnvironmentXGreen.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_EnvironmentColor_XG(SliderEventData eventData)
    {
        EnvironmentXGreen = (eventData.NewValue);
        _EnvironmentColor_X();
        SetText(SliderValueTextList, (int)(EnvironmentXRed * 255), (int)(EnvironmentXGreen * 255), (int)(EnvironmentXBlue * 255), (int)(EnvironmentYRed * 255), (int)(EnvironmentYGreen * 255), (int)(EnvironmentYBlue * 255), (int)(EnvironmentZRed * 255), (int)(EnvironmentZGreen * 255), (int)(EnvironmentZBlue * 255));
    }
    /// <summary>
    /// Set the slider value to EnvironmentXBlue.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_EnvironmentColor_XB(SliderEventData eventData)
    {
        EnvironmentXBlue = (eventData.NewValue);
        _EnvironmentColor_X();
        SetText(SliderValueTextList, (int)(EnvironmentXRed * 255), (int)(EnvironmentXGreen * 255), (int)(EnvironmentXBlue * 255), (int)(EnvironmentYRed * 255), (int)(EnvironmentYGreen * 255), (int)(EnvironmentYBlue * 255), (int)(EnvironmentZRed * 255), (int)(EnvironmentZGreen * 255), (int)(EnvironmentZBlue * 255));
    }
    /// <summary>
    /// Change the parameter of EnvironmentColor Xaxis of MRTKStandardShader.
    /// </summary>
    public void _EnvironmentColor_X()
    {
        Color color = new Color(EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue);
        this.material.SetColor("_EnvironmentColorX", color);
    }
    /// <summary>
    /// Set the slider value to EnvironmentYRed.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_EnvironmentColor_YR(SliderEventData eventData)
    {
        EnvironmentYRed = eventData.NewValue;
        _EnvironmentColor_Y();
        SetText(SliderValueTextList, (int)(EnvironmentXRed * 255), (int)(EnvironmentXGreen * 255), (int)(EnvironmentXBlue * 255), (int)(EnvironmentYRed * 255), (int)(EnvironmentYGreen * 255), (int)(EnvironmentYBlue * 255), (int)(EnvironmentZRed * 255), (int)(EnvironmentZGreen * 255), (int)(EnvironmentZBlue * 255));
    }
    /// <summary>
    /// Set the slider value to EnvironmentYGreen.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_EnvironmentColor_YG(SliderEventData eventData)
    {
        EnvironmentYGreen = eventData.NewValue;
        _EnvironmentColor_Y();
        SetText(SliderValueTextList, (int)(EnvironmentXRed * 255), (int)(EnvironmentXGreen * 255), (int)(EnvironmentXBlue * 255), (int)(EnvironmentYRed * 255), (int)(EnvironmentYGreen * 255), (int)(EnvironmentYBlue * 255), (int)(EnvironmentZRed * 255), (int)(EnvironmentZGreen * 255), (int)(EnvironmentZBlue * 255));
    }
    /// <summary>
    /// Set the slider value to EnvironmentYGreen.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_EnvironmentColor_YB(SliderEventData eventData)
    {
        EnvironmentYBlue = eventData.NewValue;
        _EnvironmentColor_Y();
        SetText(SliderValueTextList, (int)(EnvironmentXRed * 255), (int)(EnvironmentXGreen * 255), (int)(EnvironmentXBlue * 255), (int)(EnvironmentYRed * 255), (int)(EnvironmentYGreen * 255), (int)(EnvironmentYBlue * 255), (int)(EnvironmentZRed * 255), (int)(EnvironmentZGreen * 255), (int)(EnvironmentZBlue * 255));
    }
    /// <summary>
    ///Change the parameter of EnvironmentColor Yaxis of MRTKStandardShader.
    /// </summary>
    public void _EnvironmentColor_Y()
    {
        Color color = new Color(EnvironmentXRed, EnvironmentXGreen, EnvironmentXBlue);
        this.material.SetColor("_EnvironmentColorY", color);
    }
    /// <summary>
    /// Set the slider value to EnvironmentZRed.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_EnvironmentColor_ZR(SliderEventData eventData)
    {
        EnvironmentZRed = eventData.NewValue;
        _EnvironmentColor_Z();
        SetText(SliderValueTextList, (int)(EnvironmentXRed * 255), (int)(EnvironmentXGreen * 255), (int)(EnvironmentXBlue * 255), (int)(EnvironmentYRed * 255), (int)(EnvironmentYGreen * 255), (int)(EnvironmentYBlue * 255), (int)(EnvironmentZRed * 255), (int)(EnvironmentZGreen * 255), (int)(EnvironmentZBlue * 255));
    }
    /// <summary>
    /// Set the slider value to EnvironmentZGreen.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_EnvironmentColor_ZG(SliderEventData eventData)
    {
        EnvironmentZGreen = eventData.NewValue;
        _EnvironmentColor_Z();
        SetText(SliderValueTextList, (int)(EnvironmentXRed * 255), (int)(EnvironmentXGreen * 255), (int)(EnvironmentXBlue * 255), (int)(EnvironmentYRed * 255), (int)(EnvironmentYGreen * 255), (int)(EnvironmentYBlue * 255), (int)(EnvironmentZRed * 255), (int)(EnvironmentZGreen * 255), (int)(EnvironmentZBlue * 255));
    }
    /// <summary>
    /// Set the slider value to EnvironmentZBlue.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_EnvironmentColor_ZB(SliderEventData eventData)
    {
        EnvironmentZBlue = eventData.NewValue;
        _EnvironmentColor_Z();
        SetText(SliderValueTextList, (int)(EnvironmentXRed * 255), (int)(EnvironmentXGreen * 255), (int)(EnvironmentXBlue * 255), (int)(EnvironmentYRed * 255), (int)(EnvironmentYGreen * 255), (int)(EnvironmentYBlue * 255), (int)(EnvironmentZRed * 255), (int)(EnvironmentZGreen * 255), (int)(EnvironmentZBlue * 255));
    }
    /// <summary>
    /// Change the parameter of EnvironmentColor Zaxis of MRTKStandardShader.
    /// </summary>
    public void _EnvironmentColor_Z()
    {
        Color color = new Color(EnvironmentZRed, EnvironmentZGreen, EnvironmentZBlue);
        this.material.SetColor("_EnvironmentColorZ", color);
    }
    /// <summary>
    /// Change the parameter of NormalMapScale of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_NormalMapScale(SliderEventData eventData)
    {
        material.SetFloat("_NormalMapScale", eventData.NewValue);
        SetText(SliderValueTextList, eventData.NewValue, 0, 0, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Set the slider value to rimcolorred.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdate_RimLightColor_R(SliderEventData eventData)
    {
        rimcolorred = eventData.NewValue;
        RimLightColor();
        SetText(SliderValueTextList, rimcolorred, rimcolorgreen, rimcolorblue, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Set the slider value to rimcolorgreen
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdate_RimLightColor_G(SliderEventData eventData)
    {
        rimcolorgreen = eventData.NewValue;
        RimLightColor();
        SetText(SliderValueTextList, (int)(rimcolorred * 255), (int)(rimcolorgreen * 255), (int)(rimcolorblue * 255), 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Set the slider value to rimcolorblue
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdate_RimLightColor_B(SliderEventData eventData)
    {
        rimcolorblue = eventData.NewValue;
        RimLightColor();
        SetText(SliderValueTextList, (int)(rimcolorred * 255), (int)(rimcolorgreen * 255), (int)(rimcolorblue * 255), 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of RimLightColor of MRTKStandardShader.
    /// </summary>
    public void RimLightColor()
    {
        Color color = new Color(rimcolorred, rimcolorgreen, rimcolorblue);
        this.material.SetColor("_RimColor", color);
    }
    /// <summary>
    /// Change the parameter of RimPower of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdate_RimPower(SliderEventData eventData)
    {
        material.SetFloat("_RimPower", eventData.NewValue);
        SetText(SliderValueTextList, (int)(rimcolorred * 255), (int)(rimcolorgreen * 255), (int)(rimcolorblue * 255), (eventData.NewValue), 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of FadeBegindistance of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdate_FadeBeginDistance(SliderEventData eventData)
    {
        //FadeBeginDistance value has a value from 0 to 10
        fadebegindistance = eventData.NewValue * 10f;
        material.SetFloat("_FadeBeginDistance", fadebegindistance);
        SetText(SliderValueTextList, fadebegindistance, fadecompletedistance, fademainvalue, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of FadeCompleteDistance of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdate_FadeCompleteDistance(SliderEventData eventData)
    {
        //FadeCompleteDistance value has a value from 0 to 10
        fadecompletedistance = eventData.NewValue * 10f;
        material.SetFloat("_FadeCompleteDistance", fadecompletedistance);
        SetText(SliderValueTextList, fadebegindistance, fadecompletedistance, fademainvalue, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of FadeMainValue of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdate_FadeMainValue(SliderEventData eventData)
    {
        fademainvalue = eventData.NewValue;
        material.SetFloat("_FadeMainValue", fademainvalue);
        SetText(SliderValueTextList, fadebegindistance, fadecompletedistance, fademainvalue, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of EdgeSmothingValue of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdate_EdgeSmoothingValue(SliderEventData eventData)
    {
        //EdgeSmoothing value has a value from 0 to 2
        material.SetFloat("_EdgeSmoothingValue", (eventData.NewValue * 0.2f));
        SetText(SliderValueTextList, (eventData.NewValue * 0.2f), 0, 0, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of VertexExtrusionValue of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdate_VertexExtrusionValue(SliderEventData eventData)
    {
        material.SetFloat("_VertexExtrusionValue", eventData.NewValue);
        SetText(SliderValueTextList, eventData.NewValue, 0, 0, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of RefractiveIndex of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdate_RefractiveIndex(SliderEventData eventData)
    {
        //RefractiveIndex value has a value from 0 to 3
        material.SetFloat("_RefractiveIndex", (eventData.NewValue * 3f));
        SetText(SliderValueTextList, (eventData.NewValue * 3f), 0, 0, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of AlbedoAlphaCutoff of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdate_Cutoff(SliderEventData eventData)
    {
        material.SetFloat("_Cutoff", eventData.NewValue);
        SetText(SliderValueTextList, eventData.NewValue, 0, 0, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of IridescenceIntensityRange of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_IridescenceIntensityRange(SliderEventData eventData)
    {
        iridescenceintensityrange = eventData.NewValue;
        material.SetFloat("_IridescenceIntensity", iridescenceintensityrange);
        SetText(SliderValueTextList, iridescenceintensityrange, iridescencethreshold, iridescenceangle, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of IridescenceThresholdThreshold of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_IridescenceThreshold(SliderEventData eventData)
    {
        iridescencethreshold = eventData.NewValue;
        material.SetFloat("_IridescenceThreshold", iridescencethreshold);
        SetText(SliderValueTextList, iridescenceintensityrange, iridescencethreshold, iridescenceangle, 0, 0, 0, 0, 0, 0);
    }
    /// <summary>
    /// Change the parameter of IridescenceAngle of MRTKStandardShader.
    /// </summary>
    /// <param name="eventData">SliderValue</param>
    public void OnSliderUpdatedColor_IridescenceAngle(SliderEventData eventData)
    {
        //IridescenceAngle value has a value from -0.78 to 0.78
        iridescenceangle = (eventData.NewValue * 1.56f - 0.78f);
        material.SetFloat("_IridescenceAngle", iridescenceangle);
        SetText(SliderValueTextList, iridescenceintensityrange, iridescencethreshold, iridescenceangle, 0, 0, 0, 0, 0, 0);
    }
}

