using Lib.OpenCV;
using OpenVisionLab.Mvvm;
using OpenVisionLab.Mvvm.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace OpenVisionLab
{
    internal sealed class VisionToolThresholdInteractionController
    {
        private readonly ThresholdToolPresenter presenter;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly Func<bool> isSuppressed;
        private readonly Action<bool> setSuppressed;
        private readonly RadioButton rbBasic;
        private readonly RadioButton rbRange;
        private readonly RadioButton rbAdaptive;
        private readonly RadioButton rbBasicBinary;
        private readonly RadioButton rbBasicInvert;
        private readonly RadioButton rbAdaptiveMean;
        private readonly RadioButton rbAdaptiveGaussian;
        private readonly RadioButton rbAdaptiveBinary;
        private readonly RadioButton rbAdaptiveInvert;
        private readonly CheckBox chkRangeInvert;
        private readonly Slider sliderThreshold;
        private readonly Slider sliderRangeMin;
        private readonly Slider sliderRangeMax;
        private readonly Slider sliderBlockSize;
        private readonly TextBox txtThreshold;
        private readonly TextBox txtMaxValue;
        private readonly TextBox txtRangeMin;
        private readonly TextBox txtRangeMax;
        private readonly TextBox txtAdaptiveMaxValue;
        private readonly TextBox txtWeight;
        private readonly TextBox txtBlockSize;
        private readonly FrameworkElement panelBasic;
        private readonly FrameworkElement panelRange;
        private readonly FrameworkElement panelAdaptive;

        public VisionToolThresholdInteractionController(
            ThresholdToolPresenter presenter,
            VisionToolParameterChangeController parameterChangeController,
            Func<bool> isSuppressed,
            Action<bool> setSuppressed,
            RadioButton rbBasic,
            RadioButton rbRange,
            RadioButton rbAdaptive,
            RadioButton rbBasicBinary,
            RadioButton rbBasicInvert,
            RadioButton rbAdaptiveMean,
            RadioButton rbAdaptiveGaussian,
            RadioButton rbAdaptiveBinary,
            RadioButton rbAdaptiveInvert,
            CheckBox chkRangeInvert,
            Slider sliderThreshold,
            Slider sliderRangeMin,
            Slider sliderRangeMax,
            Slider sliderBlockSize,
            TextBox txtThreshold,
            TextBox txtMaxValue,
            TextBox txtRangeMin,
            TextBox txtRangeMax,
            TextBox txtAdaptiveMaxValue,
            TextBox txtWeight,
            TextBox txtBlockSize,
            FrameworkElement panelBasic,
            FrameworkElement panelRange,
            FrameworkElement panelAdaptive)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            this.parameterChangeController = parameterChangeController ?? throw new ArgumentNullException(nameof(parameterChangeController));
            this.isSuppressed = isSuppressed ?? throw new ArgumentNullException(nameof(isSuppressed));
            this.setSuppressed = setSuppressed ?? throw new ArgumentNullException(nameof(setSuppressed));
            this.rbBasic = rbBasic ?? throw new ArgumentNullException(nameof(rbBasic));
            this.rbRange = rbRange ?? throw new ArgumentNullException(nameof(rbRange));
            this.rbAdaptive = rbAdaptive ?? throw new ArgumentNullException(nameof(rbAdaptive));
            this.rbBasicBinary = rbBasicBinary ?? throw new ArgumentNullException(nameof(rbBasicBinary));
            this.rbBasicInvert = rbBasicInvert ?? throw new ArgumentNullException(nameof(rbBasicInvert));
            this.rbAdaptiveMean = rbAdaptiveMean ?? throw new ArgumentNullException(nameof(rbAdaptiveMean));
            this.rbAdaptiveGaussian = rbAdaptiveGaussian ?? throw new ArgumentNullException(nameof(rbAdaptiveGaussian));
            this.rbAdaptiveBinary = rbAdaptiveBinary ?? throw new ArgumentNullException(nameof(rbAdaptiveBinary));
            this.rbAdaptiveInvert = rbAdaptiveInvert ?? throw new ArgumentNullException(nameof(rbAdaptiveInvert));
            this.chkRangeInvert = chkRangeInvert ?? throw new ArgumentNullException(nameof(chkRangeInvert));
            this.sliderThreshold = sliderThreshold ?? throw new ArgumentNullException(nameof(sliderThreshold));
            this.sliderRangeMin = sliderRangeMin ?? throw new ArgumentNullException(nameof(sliderRangeMin));
            this.sliderRangeMax = sliderRangeMax ?? throw new ArgumentNullException(nameof(sliderRangeMax));
            this.sliderBlockSize = sliderBlockSize ?? throw new ArgumentNullException(nameof(sliderBlockSize));
            this.txtThreshold = txtThreshold ?? throw new ArgumentNullException(nameof(txtThreshold));
            this.txtMaxValue = txtMaxValue ?? throw new ArgumentNullException(nameof(txtMaxValue));
            this.txtRangeMin = txtRangeMin ?? throw new ArgumentNullException(nameof(txtRangeMin));
            this.txtRangeMax = txtRangeMax ?? throw new ArgumentNullException(nameof(txtRangeMax));
            this.txtAdaptiveMaxValue = txtAdaptiveMaxValue ?? throw new ArgumentNullException(nameof(txtAdaptiveMaxValue));
            this.txtWeight = txtWeight ?? throw new ArgumentNullException(nameof(txtWeight));
            this.txtBlockSize = txtBlockSize ?? throw new ArgumentNullException(nameof(txtBlockSize));
            this.panelBasic = panelBasic ?? throw new ArgumentNullException(nameof(panelBasic));
            this.panelRange = panelRange ?? throw new ArgumentNullException(nameof(panelRange));
            this.panelAdaptive = panelAdaptive ?? throw new ArgumentNullException(nameof(panelAdaptive));
            AttachControlBehaviors();
        }

        public void Detach()
        {
            rbBasic.Checked -= Mode_Checked;
            rbRange.Checked -= Mode_Checked;
            rbAdaptive.Checked -= Mode_Checked;
            rbBasicBinary.Checked -= Toggle_Checked;
            rbBasicInvert.Checked -= Toggle_Checked;
            rbAdaptiveMean.Checked -= Toggle_Checked;
            rbAdaptiveGaussian.Checked -= Toggle_Checked;
            rbAdaptiveBinary.Checked -= Toggle_Checked;
            rbAdaptiveInvert.Checked -= Toggle_Checked;
            chkRangeInvert.Checked -= Toggle_Checked;
            chkRangeInvert.Unchecked -= Toggle_Checked;
            InputCommandBehaviors.SetValueChangedCommand(sliderThreshold, null);
            InputCommandBehaviors.SetValueChangedCommand(sliderRangeMin, null);
            InputCommandBehaviors.SetValueChangedCommand(sliderRangeMax, null);
            InputCommandBehaviors.SetValueChangedCommand(sliderBlockSize, null);
            InputCommandBehaviors.SetTextChangedCommand(txtThreshold, null);
            InputCommandBehaviors.SetTextChangedCommand(txtMaxValue, null);
            InputCommandBehaviors.SetTextChangedCommand(txtRangeMin, null);
            InputCommandBehaviors.SetTextChangedCommand(txtRangeMax, null);
            InputCommandBehaviors.SetTextChangedCommand(txtAdaptiveMaxValue, null);
            InputCommandBehaviors.SetTextChangedCommand(txtWeight, null);
            InputCommandBehaviors.SetTextChangedCommand(txtBlockSize, null);
        }

        public void HandleModeChecked(object sender)
        {
            parameterChangeController.TryHandle(() =>
            {
                if (ReferenceEquals(sender, rbRange))
                {
                    presenter.Mode = ThresholdToolMode.Range;
                }
                else if (ReferenceEquals(sender, rbAdaptive))
                {
                    presenter.Mode = ThresholdToolMode.Adaptive;
                }
                else
                {
                    presenter.Mode = ThresholdToolMode.Threshold;
                }

                RefreshModePanels();
            }, schedulePreview: true);
        }

        public void HandleParameterChanged(object sender)
        {
            parameterChangeController.TryHandle(() => ApplyToggleParameter(sender), schedulePreview: true);
        }

        public void HandleSliderChanged(object sender)
        {
            parameterChangeController.TryHandle(() =>
            {
                VisionToolControlBinding.UpdateSliderSource(sender as Slider);
                ApplyRangeOrBlockPolicy(sender);
            }, schedulePreview: true);
        }

        public void HandleTextChanged(object sender)
        {
            parameterChangeController.TryHandle(() =>
            {
                VisionToolControlBinding.UpdateTextSource(sender as TextBox);
                ApplyRangeOrBlockPolicy(sender);
            }, schedulePreview: true);
        }

        public void RefreshModePanels()
        {
            FlushParameterBindings();
            ThresholdToolMode mode = presenter.Mode;
            VisionToolControlBinding.SetPanelVisible(panelBasic, mode == ThresholdToolMode.Threshold);
            VisionToolControlBinding.SetPanelVisible(panelRange, mode == ThresholdToolMode.Range);
            VisionToolControlBinding.SetPanelVisible(panelAdaptive, mode == ThresholdToolMode.Adaptive);
        }

        public void FlushParameterBindings()
        {
            // Threshold has several coupled controls; flush them together before deriving OpenCV properties or summary text.
            VisionToolControlBinding.UpdateToggleSources(
                rbBasicInvert,
                chkRangeInvert,
                rbAdaptiveGaussian,
                rbAdaptiveInvert);
            VisionToolControlBinding.UpdateTextSources(
                txtThreshold,
                txtMaxValue,
                txtRangeMin,
                txtRangeMax,
                txtAdaptiveMaxValue,
                txtWeight,
                txtBlockSize);
        }

        public void ConfigureBasicInvertForTest(bool invert)
        {
            RunSuppressed(() =>
            {
                presenter.Mode = ThresholdToolMode.Threshold;
                presenter.BasicInvert = invert;
                rbBasic.IsChecked = true;
                rbBasicBinary.IsChecked = !invert;
                rbBasicInvert.IsChecked = invert;
            });

            // CreateProperty flushes control bindings, so keep the visible controls and ViewModel in the same state.
            parameterChangeController.RefreshProgrammatic(RefreshModePanels);
        }

        public void ApplyBasicThresholdFromGuide(int threshold, bool invert)
        {
            int normalizedThreshold = Math.Max(0, Math.Min(255, threshold));
            RunSuppressed(() =>
            {
                presenter.Mode = ThresholdToolMode.Threshold;
                presenter.Threshold = normalizedThreshold;
                presenter.BasicInvert = invert;
                rbBasic.IsChecked = true;
                rbBasicBinary.IsChecked = !invert;
                rbBasicInvert.IsChecked = invert;
                sliderThreshold.Value = normalizedThreshold;
            });

            parameterChangeController.RefreshProgrammatic(RefreshModePanels);
        }

        private void AttachControlBehaviors()
        {
            // Toggle events must handle both user clicks and test/programmatic IsChecked changes while keeping the View free of handlers.
            rbBasic.Checked += Mode_Checked;
            rbRange.Checked += Mode_Checked;
            rbAdaptive.Checked += Mode_Checked;
            rbBasicBinary.Checked += Toggle_Checked;
            rbBasicInvert.Checked += Toggle_Checked;
            rbAdaptiveMean.Checked += Toggle_Checked;
            rbAdaptiveGaussian.Checked += Toggle_Checked;
            rbAdaptiveBinary.Checked += Toggle_Checked;
            rbAdaptiveInvert.Checked += Toggle_Checked;
            chkRangeInvert.Checked += Toggle_Checked;
            chkRangeInvert.Unchecked += Toggle_Checked;

            // Basic threshold uses a slider too; route it through the same debounced preview path as Range/Adaptive.
            InputCommandBehaviors.SetValueChangedCommand(sliderThreshold, new RelayCommand<RoutedPropertyChangedEventArgs<double>>(_ => HandleSliderChanged(sliderThreshold)));
            InputCommandBehaviors.SetValueChangedCommand(sliderRangeMin, new RelayCommand<RoutedPropertyChangedEventArgs<double>>(_ => HandleSliderChanged(sliderRangeMin)));
            InputCommandBehaviors.SetValueChangedCommand(sliderRangeMax, new RelayCommand<RoutedPropertyChangedEventArgs<double>>(_ => HandleSliderChanged(sliderRangeMax)));
            InputCommandBehaviors.SetValueChangedCommand(sliderBlockSize, new RelayCommand<RoutedPropertyChangedEventArgs<double>>(_ => HandleSliderChanged(sliderBlockSize)));
            InputCommandBehaviors.SetTextChangedCommand(txtThreshold, new RelayCommand<TextChangedEventArgs>(_ => HandleTextChanged(txtThreshold)));
            InputCommandBehaviors.SetTextChangedCommand(txtMaxValue, new RelayCommand<TextChangedEventArgs>(_ => HandleTextChanged(txtMaxValue)));
            InputCommandBehaviors.SetTextChangedCommand(txtRangeMin, new RelayCommand<TextChangedEventArgs>(_ => HandleTextChanged(txtRangeMin)));
            InputCommandBehaviors.SetTextChangedCommand(txtRangeMax, new RelayCommand<TextChangedEventArgs>(_ => HandleTextChanged(txtRangeMax)));
            InputCommandBehaviors.SetTextChangedCommand(txtAdaptiveMaxValue, new RelayCommand<TextChangedEventArgs>(_ => HandleTextChanged(txtAdaptiveMaxValue)));
            InputCommandBehaviors.SetTextChangedCommand(txtWeight, new RelayCommand<TextChangedEventArgs>(_ => HandleTextChanged(txtWeight)));
            InputCommandBehaviors.SetTextChangedCommand(txtBlockSize, new RelayCommand<TextChangedEventArgs>(_ => HandleTextChanged(txtBlockSize)));
        }

        private void Mode_Checked(object sender, RoutedEventArgs e)
        {
            HandleModeChecked(sender);
        }

        private void Toggle_Checked(object sender, RoutedEventArgs e)
        {
            HandleParameterChanged(sender);
        }

        private void ApplyToggleParameter(object sender)
        {
            if (ReferenceEquals(sender, rbBasicBinary))
            {
                presenter.BasicInvert = false;
            }
            else if (ReferenceEquals(sender, rbBasicInvert))
            {
                presenter.BasicInvert = true;
            }
            else if (ReferenceEquals(sender, rbAdaptiveMean))
            {
                presenter.AdaptiveGaussian = false;
            }
            else if (ReferenceEquals(sender, rbAdaptiveGaussian))
            {
                presenter.AdaptiveGaussian = true;
            }
            else if (ReferenceEquals(sender, rbAdaptiveBinary))
            {
                presenter.AdaptiveInvert = false;
            }
            else if (ReferenceEquals(sender, rbAdaptiveInvert))
            {
                presenter.AdaptiveInvert = true;
            }
            else
            {
                VisionToolControlBinding.UpdateToggleSource(sender as ToggleButton);
            }
        }

        private void ApplyRangeOrBlockPolicy(object sender)
        {
            if (ReferenceEquals(sender, sliderRangeMin) || ReferenceEquals(sender, txtRangeMin))
            {
                presenter.KeepRangeOrdered(preferMinimum: true);
            }
            else if (ReferenceEquals(sender, sliderRangeMax) || ReferenceEquals(sender, txtRangeMax))
            {
                presenter.KeepRangeOrdered(preferMinimum: false);
            }
            else if (ReferenceEquals(sender, sliderBlockSize) || ReferenceEquals(sender, txtBlockSize))
            {
                presenter.NormalizeBlockSize();
            }
        }

        private void RunSuppressed(Action action)
        {
            bool previousSuppressState = isSuppressed();
            setSuppressed(true);
            try
            {
                action();
            }
            finally
            {
                setSuppressed(previousSuppressState);
            }
        }
    }
}
