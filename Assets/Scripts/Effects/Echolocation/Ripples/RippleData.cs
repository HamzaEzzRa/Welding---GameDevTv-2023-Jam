using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

using System;
using System.Collections.Generic;

[Serializable]
public class RippleData
{
    [HideInInspector] public Vector4Parameter origin;

    public FloatParameter power;
    public FloatParameter exteriorWidth;
    public FloatParameter interiorWidth;

    public FloatParameter duration;
    [HideInInspector] public FloatParameter time;

    public RippleData() { }

    public RippleData(RippleData data)
    {
        origin = new Vector4Parameter { value = data.origin.value };

        power = new FloatParameter { value = data.power.value };
        exteriorWidth = new FloatParameter { value = data.exteriorWidth.value };
        interiorWidth = new FloatParameter { value = data.interiorWidth.value };

        duration = new FloatParameter { value = data.duration.value };
        time = new FloatParameter { value = data.time.value };
    }
}

[Serializable]
public class RippleDataArray
{
    public List<Vector4Parameter> origins = new List<Vector4Parameter>();

    public List<FloatParameter> powers = new List<FloatParameter>();
    public List<FloatParameter> exteriorWidths = new List<FloatParameter>();
    public List<FloatParameter> interiorWidths = new List<FloatParameter>();

    public List<FloatParameter> durations = new List<FloatParameter>();
    public List<FloatParameter> times = new List<FloatParameter>();

    public int Count => origins.Count;

    public void AddData(RippleData data)
    {
        origins.Add(data.origin);

        powers.Add(data.power);
        exteriorWidths.Add(data.exteriorWidth);
        interiorWidths.Add(data.interiorWidth);

        durations.Add(data.duration);
        times.Add(data.time);
    }

    public void RemoveData(RippleData data)
    {
        origins.Remove(data.origin);

        powers.Remove(data.power);
        exteriorWidths.Remove(data.exteriorWidth);
        interiorWidths.Remove(data.interiorWidth);

        durations.Remove(data.duration);
        times.Remove(data.time);
    }

    public void RemoveDataAt(int index)
    {
        origins.RemoveAt(index);

        powers.RemoveAt(index);
        exteriorWidths.RemoveAt(index);
        interiorWidths.RemoveAt(index);

        durations.RemoveAt(index);
        times.RemoveAt(index);
    }
}

namespace UnityEngine.Rendering.PostProcessing
{
    [Serializable]
    public sealed class RippleDataParameter : ParameterOverride<RippleData>
    {
        public RippleDataParameter() : base() { }

        public RippleDataParameter(RippleData data) : base(data)
        {
            value.origin = data.origin;
            value.power = data.power;
            value.exteriorWidth = data.exteriorWidth;
            value.interiorWidth = data.interiorWidth;
            value.duration = data.duration;
            value.time = data.time;
        }

        public new void Override(RippleData data)
        {
            overrideState = true;

            value.origin = data.origin;
            value.power = data.power;
            value.exteriorWidth = data.exteriorWidth;
            value.interiorWidth = data.interiorWidth;
            value.duration = data.duration;
            value.time = data.time;
        }
    }

    [Serializable]
    public sealed class RippleDataArrayParameter : ParameterOverride<RippleDataArray>
    {
        public RippleDataArrayParameter() : base() { }

        public RippleDataArrayParameter(RippleDataArray dataArray) : base(dataArray)
        {
            value.origins = dataArray.origins;
            value.powers = dataArray.powers;
            value.exteriorWidths = dataArray.exteriorWidths;
            value.interiorWidths = dataArray.interiorWidths;
            value.durations = dataArray.durations;
            value.times = dataArray.times;
        }

        public new void Override(RippleDataArray dataArray)
        {
            overrideState = true;

            value.origins = dataArray.origins;
            value.powers = dataArray.powers;
            value.exteriorWidths = dataArray.exteriorWidths;
            value.interiorWidths = dataArray.interiorWidths;
            value.durations = dataArray.durations;
            value.times = dataArray.times;
        }
    }
}
