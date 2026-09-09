using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    /// <summary>Owns the fixed teaching routes and frame state, without executing a real Recipe.</summary>
    internal sealed class LayerRecipeLearnPresenter
    {
        private const int AnimationStepCount = 4;

        internal IReadOnlyList<string> Layers { get; } = new[] { "Main", "Pin_Binary", "Pin_Gap", "Pin_Review" };
        internal IReadOnlyList<(string Input, string Tool, string Output)> Steps { get; } = new[]
        {
            ("Main", "Threshold", "Pin_Binary"),
            ("Pin_Binary", "LineDistance", "Pin_Gap"),
            ("Main + Pin_Gap", "Overlay", "Pin_Review"),
            ("Pin_Gap", "Accept", "Inspection")
        };

        internal int SelectedStep { get; private set; } = 2;
        internal int AnimationStep { get; private set; } = 2;
        internal bool IsAnimationComplete => AnimationStep >= AnimationStepCount;
        internal string SelectedStepText => AnimationStep.ToString(CultureInfo.InvariantCulture) + " / 4";
        internal string FormulaText
        {
            get
            {
                (string Input, string Tool, string Output) step = Steps[SelectedStep - 1];
                return "Step " + SelectedStep.ToString(CultureInfo.InvariantCulture)
                    + ": Input=" + step.Input + " -> Tool=" + step.Tool + " -> Output=" + step.Output;
            }
        }

        internal string MeaningText => SelectedStep switch
        {
            1 => "첫 Step은 Main을 읽어 이진 결과 레이어를 만듭니다.",
            2 => "두 번째 Step은 앞 단계의 Binary_Output을 InputLayer로 사용해 거리를 측정합니다.",
            3 => "OverlayMerge는 Main과 Pin_Gap을 합쳐 측정 위치가 보이는 Pin_Review를 만듭니다.",
            _ => "Recipe는 Step 연결과 acceptance 기준을 함께 저장해 같은 검사를 다시 실행할 수 있게 합니다."
        };

        internal string AnimationStatusText => AnimationStep switch
        {
            0 => "0 / 4 - Main 입력부터 Layer 경로를 따라가 보세요.",
            1 => "1 / 4 - Main -> Threshold -> Pin_Binary",
            2 => "2 / 4 - Pin_Binary -> LineDistance -> Pin_Gap",
            3 => "3 / 4 - Main + Pin_Gap -> Overlay -> Pin_Review",
            _ => "4 / 4 - Pin_Gap 결과를 Acceptance 기준과 비교해 최종 OK/NG를 판단합니다."
        };

        internal void SelectStep(double value)
        {
            SelectedStep = Math.Max(1, Math.Min(4, (int)Math.Round(value)));
            AnimationStep = SelectedStep;
        }

        internal void ResetAnimation()
        {
            // Reset clears highlights while the slider and its route explanation retain their selection.
            AnimationStep = 0;
        }

        internal void AdvanceAnimation()
        {
            SelectStep(IsAnimationComplete ? 1 : AnimationStep + 1);
        }

        internal bool IsSelectedFlowCell(int index)
        {
            return AnimationStep > 0 && index / 4 == SelectedStep - 1;
        }

        internal bool IsRouteLayer(int index)
        {
            (string Input, string Tool, string Output) step = Steps[SelectedStep - 1];
            return AnimationStep > 0 && (Layers[index] == step.Input
                || Layers[index] == step.Output || step.Input.Contains(Layers[index], StringComparison.Ordinal));
        }
    }
}
