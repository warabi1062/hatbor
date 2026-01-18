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
        public float DefaultValue { get; }

        public SliderConfigPropertyAttribute(string label, float defaultValue, float min, float max) : base(label)
        {
            Min = min;
            Max = max;
            DefaultValue = defaultValue;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class TemperatureConfigPropertyAttribute : ConfigPropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }
        public float DefaultValue { get; }

        public TemperatureConfigPropertyAttribute(string label, float defaultValue = 6500f, float min = 1000f, float max = 20000f) : base(label)
        {
            Min = min;
            Max = max;
            DefaultValue = defaultValue;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Vector3ConfigPropertyAttribute : ConfigPropertyAttribute
    {
        public float DefaultX { get; }
        public float DefaultY { get; }
        public float DefaultZ { get; }

        public Vector3ConfigPropertyAttribute(string label, float defaultX = 0f, float defaultY = 0f, float defaultZ = 0f) : base(label)
        {
            DefaultX = defaultX;
            DefaultY = defaultY;
            DefaultZ = defaultZ;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Vector2IntConfigPropertyAttribute : ConfigPropertyAttribute
    {
        public int DefaultX { get; }
        public int DefaultY { get; }

        public Vector2IntConfigPropertyAttribute(string label, int defaultX = 0, int defaultY = 0) : base(label)
        {
            DefaultX = defaultX;
            DefaultY = defaultY;
        }
    }
}