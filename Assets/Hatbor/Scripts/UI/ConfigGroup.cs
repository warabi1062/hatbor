using System;
using System.Reflection;
using Hatbor.Config;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hatbor.UI
{
    public sealed class ConfigGroup : VisualElement
    {
        readonly IFileBrowser fileBrowser;

        readonly Label label;
        readonly VisualElement container;

        public ConfigGroup(IFileBrowser fileBrowser)
        {
            this.fileBrowser = fileBrowser;

            label = new Label
            {
                style =
                {
                    fontSize = 16,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginTop = 12,
                    marginBottom = 4
                }
            };
            hierarchy.Add(label);
            container = new VisualElement();
            hierarchy.Add(container);
        }

        public IDisposable Bind(IConfigurable configurable)
        {
            var configurableType = configurable.GetType();

            label.text = GetConfigGroupAttribute(configurableType).Label;

            var disposables = new CompositeDisposable();

            foreach (var p in configurableType.GetMembers())
            {
                var provider = (ICustomAttributeProvider)p;
                if (provider.GetCustomAttributes(typeof(ConfigPropertyAttribute),false) is not ConfigPropertyAttribute[] attributes || attributes.Length == 0)
                    continue;

                var property = configurableType.GetProperty(p.Name)?.GetValue(configurable);
                var attr = attributes[0];
                var (element, disposable) = CreateFieldAndBind(property, attr);
                disposable.AddTo(disposables);
                container.Add(element);
            }

            return disposables;
        }

        static ConfigGroupAttribute GetConfigGroupAttribute(ICustomAttributeProvider provider)
        {
            return provider.GetCustomAttributes(typeof(ConfigGroupAttribute),false) is ConfigGroupAttribute[] { Length: > 0 } attributes
                ? attributes[0]
                : null;
        }

        (VisualElement, IDisposable) CreateFieldAndBind(object property, ConfigPropertyAttribute attr)
        {
            return (property, attr) switch
            {
                (ReactiveProperty<bool> p, _) =>
                    CreateFieldAndBind<bool, Toggle>(p, attr.Label),
                (ReactiveProperty<float> p, TemperatureConfigPropertyAttribute a) =>
                    CreateTemperatureFieldAndBind(p, a),
                (ReactiveProperty<float> p, SliderConfigPropertyAttribute a) =>
                    CreateSliderFieldAndBind(p, a),
                (ReactiveProperty<float> p, _) =>
                    CreateFieldAndBind<float, FloatField>(p, attr.Label),
                (ReactiveProperty<int> p, _) =>
                    CreateFieldAndBind<int, IntegerField>(p, attr.Label),
                (ReactiveProperty<Vector2Int> p, Vector2IntConfigPropertyAttribute a) =>
                    CreateVector2IntFieldAndBind(p, a),
                (ReactiveProperty<Vector2Int> p, _) =>
                    CreateFieldAndBind<Vector2Int, Vector2IntField>(p, attr.Label),
                (ReactiveProperty<Vector3> p, Vector3ConfigPropertyAttribute a) =>
                    CreateVector3FieldAndBind(p, a),
                (ReactiveProperty<Vector3> p, _) =>
                    CreateFieldAndBind<Vector3, Vector3Field>(p, attr.Label),
                (ReactiveProperty<Color> p, _) =>
                    CreateColorFieldAndBind(p, attr.Label),
                (ReactiveProperty<string> p, FilePathConfigPropertyAttribute a) =>
                    CreateFilePathFieldAndBind(p, a),
                (Action p, _) =>
                    CreateButtonAndRegister(p, attr),
                _ => throw new ArgumentOutOfRangeException(nameof(property), property, null)
            };
        }

        static (VisualElement, IDisposable) CreateFieldAndBind<TValueType, TField>(
            ReactiveProperty<TValueType> property, string label)
            where TField : BaseField<TValueType>, new()
        {
            var propertyField = new PropertyField<TValueType, TField>
            {
                Label = label
            };
            return (propertyField, propertyField.Bind(property));
        }

        static (VisualElement, IDisposable) CreateSliderFieldAndBind(ReactiveProperty<float> property, SliderConfigPropertyAttribute attr)
        {
            var slider = new Slider(attr.Label, attr.Min, attr.Max)
            {
                showInputField = true
            };
            slider.labelElement.style.unityFontStyleAndWeight = FontStyle.Bold;
            var disposables = new CompositeDisposable();
            slider.RegisterValueChangedCallback(evt => property.Value = evt.newValue);
            property.Subscribe(x => slider.SetValueWithoutNotify(x)).AddTo(disposables);

            slider.labelElement.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.clickCount == 2)
                {
                    property.Value = attr.DefaultValue;
                }
            });

            return (slider, disposables);
        }

        static (VisualElement, IDisposable) CreateTemperatureFieldAndBind(ReactiveProperty<float> property, TemperatureConfigPropertyAttribute attr)
        {
            var field = new TemperatureField
            {
                Label = attr.Label,
                Min = attr.Min,
                Max = attr.Max
            };
            return (field, field.Bind(property, attr.DefaultValue));
        }

        static (VisualElement, IDisposable) CreateVector2IntFieldAndBind(ReactiveProperty<Vector2Int> property, Vector2IntConfigPropertyAttribute attr)
        {
            var propertyField = new PropertyField<Vector2Int, Vector2IntField>
            {
                Label = attr.Label
            };

            var vector2IntField = propertyField.Q<Vector2IntField>();
            vector2IntField.style.flexDirection = FlexDirection.Column;
            vector2IntField.labelElement.style.unityFontStyleAndWeight = FontStyle.Bold;

            var defaultValue = new Vector2Int(attr.DefaultX, attr.DefaultY);
            vector2IntField.labelElement.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.clickCount == 2)
                {
                    property.Value = defaultValue;
                }
            });

            var inputContainer = vector2IntField.Q(className: "unity-composite-field__input");
            if (inputContainer != null)
            {
                inputContainer.style.flexDirection = FlexDirection.Row;
                inputContainer.style.marginTop = 4;
            }

            var intFields = propertyField.Query<IntegerField>().ToList();
            foreach (var f in intFields)
            {
                f.style.flexGrow = 0;
                f.style.flexShrink = 0;
                f.style.width = 120;
                f.labelElement.style.minWidth = 15;
                f.labelElement.style.paddingRight = 4;
            }

            var defaults = new[] { attr.DefaultX, attr.DefaultY };

            for (var i = 0; i < intFields.Count && i < 2; i++)
            {
                var index = i;
                intFields[i].labelElement.RegisterCallback<ClickEvent>(evt =>
                {
                    if (evt.clickCount == 2)
                    {
                        var v = property.Value;
                        v[index] = defaults[index];
                        property.Value = v;
                    }
                });
            }

            return (propertyField, propertyField.Bind(property));
        }

        static (VisualElement, IDisposable) CreateVector3FieldAndBind(ReactiveProperty<Vector3> property, Vector3ConfigPropertyAttribute attr)
        {
            var propertyField = new PropertyField<Vector3, Vector3Field>
            {
                Label = attr.Label
            };

            var vector3Field = propertyField.Q<Vector3Field>();
            vector3Field.style.flexDirection = FlexDirection.Column;
            vector3Field.labelElement.style.unityFontStyleAndWeight = FontStyle.Bold;

            var defaultValue = new Vector3(attr.DefaultX, attr.DefaultY, attr.DefaultZ);
            vector3Field.labelElement.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.clickCount == 2)
                {
                    property.Value = defaultValue;
                }
            });

            var inputContainer = vector3Field.Q(className: "unity-composite-field__input");
            if (inputContainer != null)
            {
                inputContainer.style.flexDirection = FlexDirection.Row;
                inputContainer.style.marginTop = 4;
            }

            var floatFields = propertyField.Query<FloatField>().ToList();
            foreach (var f in floatFields)
            {
                f.style.flexGrow = 0;
                f.style.flexShrink = 0;
                f.style.width = 120;
                f.labelElement.style.minWidth = 15;
                f.labelElement.style.paddingRight = 4;
            }

            var defaults = new[] { attr.DefaultX, attr.DefaultY, attr.DefaultZ };

            for (var i = 0; i < floatFields.Count && i < 3; i++)
            {
                var index = i;
                floatFields[i].labelElement.RegisterCallback<ClickEvent>(evt =>
                {
                    if (evt.clickCount == 2)
                    {
                        var v = property.Value;
                        v[index] = defaults[index];
                        property.Value = v;
                    }
                });
            }

            return (propertyField, propertyField.Bind(property));
        }

        static (VisualElement, IDisposable) CreateColorFieldAndBind(ReactiveProperty<Color> property, string label)
        {
            var colorField = new ColorField
            {
                Label = label
            };
            return (colorField, colorField.Bind(property));
        }

        (VisualElement, IDisposable) CreateFilePathFieldAndBind(ReactiveProperty<string> property, FilePathConfigPropertyAttribute attr)
        {
            var button = new Button<string>
            {
                Label = attr.Label
            };
            return (button, button.Bind(property, () => fileBrowser.ChooseFileAsync(attr.Extension)));
        }

        static (VisualElement, IDisposable) CreateButtonAndRegister(Action onClicked, ConfigPropertyAttribute attr)
        {
            var button = new Button
            {
                text = attr.Label
            };
            var disposable = button.OnClickAsObservable().Subscribe(_ => onClicked());
            return (button, disposable);
        }
    }
}