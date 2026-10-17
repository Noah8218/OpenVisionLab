using OpenVisionLab.ImageCanvas.ViewModels;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace OpenVisionLab.ImageCanvas.Views
{
	public sealed class AddRoiArrayView : Window
	{
		private AddRoiArrayViewModel subscribedViewModel;

		public AddRoiArrayView()
		{
			Width = 420;
			MinWidth = 420;
			MinHeight = 240;
			SizeToContent = SizeToContent.Height;
			WindowStyle = WindowStyle.ToolWindow;
			Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/OpenVisionLab.ImageCanvas;component/Compatibility/AddRoiArrayView.Resources.xaml", UriKind.Relative) });
			Background = (Brush)FindResource("RoiArray.SurfaceBrush");
			Foreground = (Brush)FindResource("RoiArray.TextBrush");
			Content = CreateContent();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			DataContextChanged += OnDataContextChanged;
		}

		private UIElement CreateContent()
		{
			Grid grid = new Grid { Margin = new Thickness(16) };
			for (int i = 0; i < 6; i++)
			{
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			}
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());

			AddLabel(grid, "Row", 0, 0);
			AddTextBox(grid, nameof(AddRoiArrayViewModel.Rows), 1, 0);
			AddLabel(grid, "Column", 0, 1);
			AddTextBox(grid, nameof(AddRoiArrayViewModel.Columns), 1, 1);
			AddLabel(grid, "Row Gap(mm)", 2, 0);
			AddTextBox(grid, nameof(AddRoiArrayViewModel.RowSpacing), 3, 0);
			AddLabel(grid, "Column Gap(mm)", 2, 1);
			AddTextBox(grid, nameof(AddRoiArrayViewModel.ColumnSpacing), 3, 1);

			TextBlock validation = new TextBlock { Margin = new Thickness(4, 8, 4, 4), TextWrapping = TextWrapping.Wrap, Foreground = new SolidColorBrush(Color.FromRgb(166, 35, 61)) };
			validation.SetBinding(TextBlock.TextProperty, nameof(AddRoiArrayViewModel.ValidationMessage));
			AutomationProperties.SetAutomationId(validation, "RoiArrayValidationMessage");
			AutomationProperties.SetLiveSetting(validation, AutomationLiveSetting.Polite);
			Grid.SetRow(validation, 4);
			Grid.SetColumnSpan(validation, 2);
			grid.Children.Add(validation);

			Button apply = new Button { Content = "Apply", Height = 32, Margin = new Thickness(4), IsDefault = true, Style = (Style)FindResource("RoiArray.PrimaryButtonStyle") };
			apply.SetBinding(Button.CommandProperty, nameof(AddRoiArrayViewModel.ApplyCommand));
			AutomationProperties.SetAutomationId(apply, "RoiArrayApplyButton");
			Grid.SetRow(apply, 5);
			Grid.SetColumn(apply, 0);
			grid.Children.Add(apply);

			Button cancel = new Button { Content = "Cancel", Height = 32, Margin = new Thickness(4), IsCancel = true, Style = (Style)FindResource("RoiArray.ButtonStyle") };
			cancel.SetBinding(Button.CommandProperty, nameof(AddRoiArrayViewModel.CancelCommand));
			AutomationProperties.SetAutomationId(cancel, "RoiArrayCancelButton");
			Grid.SetRow(cancel, 5);
			Grid.SetColumn(cancel, 1);
			grid.Children.Add(cancel);

			return grid;
		}

		private static void AddLabel(Grid grid, string text, int row, int column)
		{
			TextBlock label = new TextBlock { Text = text, Margin = new Thickness(4), VerticalAlignment = VerticalAlignment.Bottom };
			Grid.SetRow(label, row);
			Grid.SetColumn(label, column);
			grid.Children.Add(label);
		}

		private void AddTextBox(Grid grid, string bindingPath, int row, int column)
		{
			TextBox textBox = new TextBox { Margin = new Thickness(4), Style = (Style)FindResource("RoiArray.FieldStyle") };
			textBox.SetBinding(TextBox.TextProperty, new Binding(bindingPath) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
			AutomationProperties.SetAutomationId(textBox, "RoiArray" + bindingPath);
			Grid.SetRow(textBox, row);
			Grid.SetColumn(textBox, column);
			grid.Children.Add(textBox);
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			UpdateCloseSubscription();
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			DetachCloseSubscription();
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (IsLoaded) UpdateCloseSubscription();
		}

		private void UpdateCloseSubscription()
		{
			DetachCloseSubscription();
			subscribedViewModel = DataContext as AddRoiArrayViewModel;
			if (subscribedViewModel != null) subscribedViewModel.RequestClose += OnRequestClose;
		}

		private void DetachCloseSubscription()
		{
			if (subscribedViewModel != null)
			{
				subscribedViewModel.RequestClose -= OnRequestClose;
				subscribedViewModel = null;
			}
		}

		private void OnRequestClose(bool result) => DialogResult = result;

		protected override void OnClosed(EventArgs e)
		{
			DetachCloseSubscription();
			Loaded -= OnLoaded;
			Unloaded -= OnUnloaded;
			DataContextChanged -= OnDataContextChanged;
			base.OnClosed(e);
		}
	}
}
