using System;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hatbor.UI
{
    public class ColorField : PropertyField<string, TextField>
    {
        public ColorField()
        {
            field.isDelayed = true;
        }

        public IDisposable Bind(ReactiveProperty<Color> property)
        {
            var disposables = new CompositeDisposable();

            field.Bind(property, TryParseHtmlString, ToHtmlStringRGB)
                .AddTo(disposables);

            void OnFocusOut(FocusOutEvent _)
            {
                if (TryParseHtmlString(field.value, out var color) &&
                    ToHtmlStringRGB(color, out var formatted))
                {
                    field.SetValueWithoutNotify(formatted);
                }
            }
            field.RegisterCallback<FocusOutEvent>(OnFocusOut);
            Disposable.Create(() => field.UnregisterCallback<FocusOutEvent>(OnFocusOut))
                .AddTo(disposables);

            return disposables;
        }

        static bool TryParseHtmlString(string input, out Color output)
        {
            if (!input.StartsWith("#"))
            {
                input = "#" + input;
            }
            if (ColorUtility.TryParseHtmlString(input, out output))
            {
                return true;
            }
            output = Color.white;
            return false;
        }

        static bool ToHtmlStringRGB(Color input, out string output)
        {
            output = "#" + ColorUtility.ToHtmlStringRGB(input);
            return true;
        }
    }
}