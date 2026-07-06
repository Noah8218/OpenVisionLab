using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class VisionToolDoubleInputCustomToolShell : UserControl
    {
        private readonly DockedInspectorLayoutController layoutController;

        public static readonly DependencyProperty TitleIconKindProperty =
            DependencyProperty.Register(
                nameof(TitleIconKind),
                typeof(PackIconMaterialKind),
                typeof(VisionToolDoubleInputCustomToolShell),
                new PropertyMetadata(PackIconMaterialKind.CalculatorVariant));

        public static readonly DependencyProperty ParameterContentProperty =
            DependencyProperty.Register(
                nameof(ParameterContent),
                typeof(object),
                typeof(VisionToolDoubleInputCustomToolShell),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsDockedInspectorModeProperty =
            DependencyProperty.Register(
                nameof(IsDockedInspectorMode),
                typeof(bool),
                typeof(VisionToolDoubleInputCustomToolShell),
                new PropertyMetadata(false, OnIsDockedInspectorModeChanged));

        public VisionToolDoubleInputCustomToolShell()
        {
            InitializeComponent();
            layoutController = new DockedInspectorLayoutController(this);
            ApplyDockedInspectorMode();
        }

        public PackIconMaterialKind TitleIconKind
        {
            get => (PackIconMaterialKind)GetValue(TitleIconKindProperty);
            set => SetValue(TitleIconKindProperty, value);
        }

        public object ParameterContent
        {
            get => GetValue(ParameterContentProperty);
            set => SetValue(ParameterContentProperty, value);
        }

        public bool IsDockedInspectorMode
        {
            get => (bool)GetValue(IsDockedInspectorModeProperty);
            set => SetValue(IsDockedInspectorModeProperty, value);
        }

        public HeaderedContentControl InputAGroup => gbInputA;
        public HeaderedContentControl InputBGroup => gbInputB;
        public HeaderedContentControl OutputLayerGroup => gbOutputLayer;
        public TextBlock TitleText => txtTitle;
        public TextBlock AddPipelineText => txtAddPipelineText;
        public TextBlock RunPreviewText => txtRunPreviewText;
        public TextBlock RunOffsetText => txtRunOffsetText;
        public TextBlock StatusText => txtStatus;
        public TextBlock SummaryText => txtSummary;
        public Border SummaryHost => bdSummary;
        public Border StatusHost => bdStatus;
        public Border InputAPreviewFrame => bdInputAPreview;
        public VisionToolInlinePreviewSlot InputAPreview => imgInputA;
        public Border InputBPreviewFrame => bdInputBPreview;
        public VisionToolInlinePreviewSlot InputBPreview => imgInputB;
        public Border OutputPreviewFrame => bdOutputPreview;
        public VisionToolInlinePreviewSlot OutputPreview => imgOutputPreview;
        public ComboBox InputAComboBox => cbInputA;
        public ComboBox InputBComboBox => cbInputB;
        public ComboBox OutputLayerComboBox => cbOutputLayer;
        public Button LoadInputAImageButton => btnLoadInputAImage;
        public Button LoadInputBImageButton => btnLoadInputBImage;
        public Button CreateOutputLayerButton => btnCreateOutputLayer;
        public Button RunPreviewButton => btnRunPreview;
        public Button RunOffsetButton => btnRunOffset;
        public Button AddPipelineButton => btnAddPipeline;

        public void SetInputBPreviewVisible(bool visible)
        {
            layoutController?.SetInputBPreviewVisible(visible);
        }

        public void SetOffsetActionsVisible(bool useOffsetMode)
        {
            layoutController?.SetOffsetActionsVisible(useOffsetMode);
        }

        private static void OnIsDockedInspectorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VisionToolDoubleInputCustomToolShell shell)
            {
                shell.ApplyDockedInspectorMode();
            }
        }

        private void ApplyDockedInspectorMode()
        {
            layoutController?.Apply();
        }
    }
}
