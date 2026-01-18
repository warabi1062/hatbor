using System;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hatbor.UI
{
    public class TemperatureField : VisualElement
    {
        readonly Label label;
        readonly Slider slider;
        readonly Label valueLabel;
        readonly VisualElement preview;

        public string Label
        {
            get => label.text;
            set => label.text = value;
        }

        public float Min
        {
            get => slider.lowValue;
            set => slider.lowValue = value;
        }

        public float Max
        {
            get => slider.highValue;
            set => slider.highValue = value;
        }

        public TemperatureField()
        {
            style.flexDirection = FlexDirection.Column;

            label = new Label
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
            hierarchy.Add(label);

            var sliderRow = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center
                }
            };
            hierarchy.Add(sliderRow);

            slider = new Slider(1000f, 20000f)
            {
                style =
                {
                    flexGrow = 1
                }
            };
            sliderRow.Add(slider);

            valueLabel = new Label
            {
                style =
                {
                    minWidth = 70,
                    unityTextAlign = TextAnchor.MiddleRight
                }
            };
            sliderRow.Add(valueLabel);

            preview = new VisualElement
            {
                style =
                {
                    width = 50,
                    height = 20,
                    marginLeft = 4
                }
            };
            sliderRow.Add(preview);
        }

        public IDisposable Bind(ReactiveProperty<float> property, float defaultValue)
        {
            var disposables = new CompositeDisposable();

            slider.RegisterValueChangedCallback(evt =>
            {
                property.Value = evt.newValue;
            });

            property.Subscribe(temp =>
            {
                slider.SetValueWithoutNotify(temp);
                valueLabel.text = $"{temp:F0} K";
                preview.style.backgroundColor = TemperatureToColor(temp);
            }).AddTo(disposables);

            label.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.clickCount == 2)
                {
                    property.Value = defaultValue;
                }
            });

            return disposables;
        }

        static Color TemperatureToColor(float kelvin)
        {
            return Mathf.CorrelatedColorTemperatureToRGB(kelvin);
        }
    }
}
