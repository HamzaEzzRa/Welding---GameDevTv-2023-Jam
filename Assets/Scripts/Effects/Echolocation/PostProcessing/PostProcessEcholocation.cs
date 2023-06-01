using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

using System;
using System.Linq;

[Serializable]
[PostProcess(typeof(PostProcessEcholocationRenderer), PostProcessEvent.BeforeStack, "Custom/Post Process Echolocation")]
public sealed class PostProcessEcholocation : PostProcessEffectSettings
{
    public BoolParameter withTexture = new BoolParameter { value = false };

    public ColorParameter color = new ColorParameter { value = Color.white };
    [Tooltip("Number of pixels between samples that are tested for an edge. When this value is 1, tested samples are adjacent.")]
    public IntParameter scale = new IntParameter { value = 2 };
    [Tooltip("Difference between depth values, scaled by the current depth, required to draw an edge.")]
    public FloatParameter depthThreshold = new FloatParameter { value = 7f };
    [Range(0, 1), Tooltip("The value at which the dot product between the surface normal and the view direction will affect " +
        "the depth threshold. This ensures that surfaces at right angles to the camera require a larger depth threshold to draw " +
        "an edge, avoiding edges being drawn along slopes.")]
    public FloatParameter depthNormalThreshold = new FloatParameter { value = 0.5f };
    [Tooltip("Scale the strength of how much the depthNormalThreshold affects the depth threshold.")]
    public FloatParameter depthNormalThresholdScale = new FloatParameter { value = 7 };
    [Range(0, 1), Tooltip("Larger values will require the difference between normals to be greater to draw an edge.")]
    public FloatParameter normalThreshold = new FloatParameter { value = 0.4f };

    [Header("Ripple Debugging")]
    public IntParameter rippleCount = new IntParameter { value = 0 };
    public RippleDataArrayParameter rippleDataArray = new RippleDataArrayParameter();
}

public sealed class PostProcessEcholocationRenderer : PostProcessEffectRenderer<PostProcessEcholocation>
{
    public Shader PostProcessShader
    {
        get
        {
            if (cachedShader == null)
            {
                cachedShader = Resources.Load<Shader>("Shaders/EcholocationPostProcess");
            }
            return cachedShader;
        }
    }

    [SerializeField, HideInInspector] private Shader cachedShader;

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(PostProcessShader);
        sheet.properties.SetFloat("_WithTexture", settings.withTexture.value ? 1 : 0);

        sheet.properties.SetColor("_Color", settings.color);
        sheet.properties.SetFloat("_Scale", settings.scale);
        sheet.properties.SetFloat("_DepthThreshold", settings.depthThreshold);
        sheet.properties.SetFloat("_DepthNormalThreshold", settings.depthNormalThreshold);
        sheet.properties.SetFloat("_DepthNormalThresholdScale", settings.depthNormalThresholdScale);
        sheet.properties.SetFloat("_NormalThreshold", settings.normalThreshold);

        sheet.properties.SetFloat("_CurrentRippleCount", settings.rippleCount);

        Vector4[] origins = settings.rippleDataArray.value.origins.Select(x => x.value).ToArray();
        Array.Resize(ref origins, RippleManager.MAX_RIPPLE_AMOUNT);
        sheet.properties.SetVectorArray("_RippleOrigins", origins);

        float[] powers = settings.rippleDataArray.value.powers.Select(x => x.value).ToArray();
        Array.Resize(ref powers, RippleManager.MAX_RIPPLE_AMOUNT);
        sheet.properties.SetFloatArray("_RipplePowers", powers);

        float[] exteriorWidths = settings.rippleDataArray.value.exteriorWidths.Select(x => x.value).ToArray();
        Array.Resize(ref exteriorWidths, RippleManager.MAX_RIPPLE_AMOUNT);
        sheet.properties.SetFloatArray("_ExteriorRippleWidths", exteriorWidths);

        float[] interiorWidths = settings.rippleDataArray.value.interiorWidths.Select(x => x.value).ToArray();
        Array.Resize(ref interiorWidths, RippleManager.MAX_RIPPLE_AMOUNT);
        sheet.properties.SetFloatArray("_InteriorRippleWidths", interiorWidths);

        float[] durations = settings.rippleDataArray.value.durations.Select(x => x.value).ToArray();
        Array.Resize(ref durations, RippleManager.MAX_RIPPLE_AMOUNT);
        sheet.properties.SetFloatArray("_RippleDurations", durations);

        float[] times = settings.rippleDataArray.value.times.Select(x => x.value).ToArray();
        Array.Resize(ref times, RippleManager.MAX_RIPPLE_AMOUNT);
        sheet.properties.SetFloatArray("_RippleTimes", times);

        Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, true).inverse;
        sheet.properties.SetMatrix("_ClipToView", clipToView);
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D11 || SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D12)
        {
            sheet.properties.SetMatrix("_ClipToWorld", (clipToView.inverse * context.camera.worldToCameraMatrix).inverse);
        }
        else
        {
            sheet.properties.SetMatrix("_ClipToWorld", (context.camera.worldToCameraMatrix.inverse * clipToView.inverse).inverse);
        }

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
