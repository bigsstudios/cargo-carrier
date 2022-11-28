using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    [RequireComponent(typeof(MultiImageTargetGraphics))]
    public class MultiImageButton : Button
    {
        private Graphic[] _graphics;
        private MultiImageTargetGraphics _targetGraphics;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (!GetGraphics())
                return;

            var targetColor =
                state switch
                {
                    SelectionState.Disabled => colors.disabledColor,
                    SelectionState.Highlighted => colors.highlightedColor,
                    SelectionState.Normal => colors.normalColor,
                    SelectionState.Pressed => colors.pressedColor,
                    SelectionState.Selected => colors.selectedColor,
                    _ => Color.white
                };

            foreach (var graphic in _graphics)
                graphic.CrossFadeColor(targetColor, instant ? 0 : colors.fadeDuration, true, true);
        }

        private bool GetGraphics()
        {
            if (!_targetGraphics) _targetGraphics = GetComponent<MultiImageTargetGraphics>();
            _graphics = _targetGraphics!.GetTargetGraphics;
            return _graphics is { Length: > 0 };
        }
    }
}