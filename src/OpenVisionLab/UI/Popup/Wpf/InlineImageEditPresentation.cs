using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    // Main의 UI host만 사용한다. 값과 asset의 최종 writer는 기존 PG 호출자다.
    internal interface IInlineImageEditPresentation
    {
        FrameworkElement InlineViewport { get; }
        FrameworkElement InlinePatternPreview { get; }
        IReadOnlyList<TextBox> InlineFields { get; }
        int InlineRegionCount { get; }
        bool IsMultiInline { get; }
        bool IsTemplateInline { get; }
        void InitializeInline();
        void FlushInlineCoordinates();
        bool AcceptInline();
        void WholeInline();
        void AddInline();
        void DeleteInline();
        IPropertyGridImageEditView CreateLargeInlineEditor();
        void AcceptLargeInlineEditor(IPropertyGridImageEditView editor);
    }
}
