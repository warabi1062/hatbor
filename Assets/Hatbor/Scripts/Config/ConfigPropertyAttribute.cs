using System;
using UnityEngine.Scripting;

namespace Hatbor.Config
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigPropertyAttribute : PreserveAttribute
    {
        public string Label { get; }

        public ConfigPropertyAttribute(string label)
        {
            Label = label;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class FilePathConfigPropertyAttribute : ConfigPropertyAttribute
    {
        public string Extension { get; }

        public FilePathConfigPropertyAttribute(string label, string extension) : base(label)
        {
            Extension = extension;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SliderConfigPropertyAttribute : ConfigPropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }

        public SliderConfigPropertyAttribute(string label, float min, float max) : base(label)
        {
            Min = min;
            Max = max;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class TemperatureConfigPropertyAttribute : ConfigPropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }

        public TemperatureConfigPropertyAttribute(string label, float min = 1000f, float max = 20000f) : base(label)
        {
            Min = min;
            Max = max;
        }
    }
}