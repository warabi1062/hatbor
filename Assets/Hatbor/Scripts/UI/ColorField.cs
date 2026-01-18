using System;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hatbor.UI
{
    public class ColorField : VisualElement
    {
        readonly Label label;
        readonly Slider sliderR;
        readonly Slider sliderG;
        readonly Slider sliderB;
        readonly VisualElement preview;

        public string Label
        {
            get => label.text;
            set => label.text = value;
        }

        public ColorField()
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

            preview = new VisualElement
            {
                style =
                {
                    height = 20,
                    marginBottom = 4
                }
            };
            hierarchy.Add(preview);

            sliderR = CreateSlider("R");
            sliderG = CreateSlider("G");
            sliderB = CreateSlider("B");

            hierarchy.Add(sliderR);
            hierarchy.Add(sliderG);
            hierarchy.Add(sliderB);
        }

        static Slider CreateSlider(string label)
        {
            return new Slider(label, 0f, 1f)
            {
                showInputField = true
            };
        }

        public IDisposable Bind(ReactiveProperty<Color> property)
        {
            var disposables = new CompositeDisposable();
            var updating = false;

            void UpdateProperty()
            {
                if (updating) return;
                updating = true;
                var color = new Color(sliderR.value, sliderG.value, sliderB.value);
                property.Value = color;
                preview.style.backgroundColor = color;
                updating = false;
            }

            void UpdateSliders(Color color)
            {
                if (updating) return;
                updating = true;
                sliderR.SetValueWithoutNotify(color.r);
                sliderG.SetValueWithoutNotify(color.g);
                sliderB.SetValueWithoutNotify(color.b);
                preview.style.backgroundColor = color;
                updating = false;
            }

            sliderR.RegisterValueChangedCallback(_ => UpdateProperty());
            sliderG.RegisterValueChangedCallback(_ => UpdateProperty());
            sliderB.RegisterValueChangedCallback(_ => UpdateProperty());

            property.Subscribe(UpdateSliders).AddTo(disposables);

            return disposables;
        }
    }
}