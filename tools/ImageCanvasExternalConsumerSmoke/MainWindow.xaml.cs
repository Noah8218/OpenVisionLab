using OpenVisionLab.ImageCanvas.External;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DrawingColor = System.Drawing.Color;
using FormsScreen = System.Windows.Forms.Screen;
using WindowInteropHelper = System.Windows.Interop.WindowInteropHelper;

namespace ImageCanvasExternalConsumerSmoke;

public partial class MainWindow : Window
{
	private readonly string[] arguments;
	private readonly ImageCanvasExternalConsumerSession session;
	private ImageCanvasExternalViewState? capturedViewState;
	private string? smokeOutputDirectory;
	private bool initialized;
	private bool smokeMode;
	private int selectionChangedCount;
	private FormsScreen testMonitor = null!;
	private int initializationAttempts;
	private bool monitorPositioned;
	private bool projectionMode;
	private string? projectionExchangeRoot;
	private string? projectionTwoDTransactionId;
	private FileSystemWatcher? projectionWatcher;
	private DispatcherTimer? projectionRefreshTimer;
	private DispatcherTimer? projectionSmokeTimeoutTimer;
	private DispatcherTimer? projectionSmokeDeadlineTimer;
	private int projectionRefreshCount;
	private int projectionMarkerCount;
	private string? projectionResultPath;
	private bool projectionApplied;

	public MainWindow(string[] arguments)
	{
		this.arguments = arguments ?? Array.Empty<string>();
		InitializeComponent();
		PlaceOnTestMonitor();
		session = new ImageCanvasExternalConsumerSession("ExternalConsumerSample");
		session.SelectionChanged += Session_SelectionChanged;
		viewerHost.Content = session.View;
		if (TryReadSmokeArguments(out string outputDirectory))
		{
			smokeMode = true;
			smokeOutputDirectory = outputDirectory;
		}
		if (TryReadProjectionSmokeArguments(out ProjectionSmokeOptions projectionOptions))
		{
			smokeMode = true;
			projectionMode = true;
			smokeOutputDirectory = projectionOptions.OutputDirectory;
			projectionExchangeRoot = projectionOptions.ExchangeRoot;
			projectionTwoDTransactionId = projectionOptions.TwoDTransactionId;
			projectionStatusText.Text = "3D→2D marker 대기 중 / Waiting for 3D→2D markers";
		}
		ContentRendered += MainWindow_ContentRendered;
		Closed += MainWindow_Closed;
	}

	private void MainWindow_ContentRendered(object? sender, EventArgs e)
	{
		if (!monitorPositioned)
		{
			PositionWindowOnTestMonitor();
			monitorPositioned = true;
		}

		if (initialized)
		{
			return;
		}

		Dispatcher.BeginInvoke(
			DispatcherPriority.ApplicationIdle,
			new Action(InitializeSample));
	}

	private void InitializeSample()
	{
		if (initialized)
		{
			return;
		}

		try
		{
			initializationAttempts++;
			VerifyBorrowAndCloneOwnership();
			Bitmap source = projectionMode
				? LoadProjectionSourceImage()
				: CreateSampleImage();
			session.LoadImage(source, "external-consumer-sample", ImageCanvasImageOwnership.TakeOwnership);
			if (!projectionMode)
			{
				session.AddOverlay(new ImageCanvasExternalOverlay(
					"candidate-accepted",
					new ImageCanvasSourceRectangle(80, 76, 286, 232),
					true,
					string.Empty,
					DrawingColor.LimeGreen));
				session.AddOverlay(new ImageCanvasExternalOverlay(
					"candidate-rejected",
					new ImageCanvasSourceRectangle(390, 214, 640, 394),
					false,
					"WidthBelowMinimum",
					DrawingColor.OrangeRed));
			}

			overlayList.ItemsSource = session.Overlays;
			if (!projectionMode)
			{
				overlayList.SelectedItem = session.Overlays[0];
			}
			initialized = true;
			if (smokeMode)
			{
				if (projectionMode)
				{
					ConfigureProjectionWatcher();
					WriteProjectionScreenReady();
					Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(ApplyLatestProjectionResult));
					Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(WaitForProjectionSmoke));
				}
				else
				{
					Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(RunSmoke));
				}
			}
		}
		catch (InvalidOperationException exception) when (exception.Message.Contains("wait until it is loaded", StringComparison.Ordinal))
		{
			if (initializationAttempts >= 20)
			{
				FailAndShutdown($"Viewer layout did not become ready after {initializationAttempts} attempts: {exception.Message}");
				return;
			}
			Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(InitializeSample));
		}
		catch (Exception exception)
		{
			FailAndShutdown("Initialization failed: " + exception);
		}
	}

	private void OverlayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (overlayList.SelectedItem is ImageCanvasExternalOverlay overlay && session.SelectOverlay(overlay.Id))
		{
			return;
		}
	}

	private void Session_SelectionChanged(object? sender, ImageCanvasSelectionChangedEventArgs e)
	{
		selectionChangedCount++;
		if (e.SelectedOverlay == null)
		{
			selectionText.Text = "선택 없음";
			selectionReasonText.Text = string.Empty;
			return;
		}

		selectionText.Text = $"{e.SelectedOverlay.Id} / {e.SelectedOverlay.StatusText}";
		selectionReasonText.Text = e.SelectedOverlay.Accepted
			? ""
			: "Reject reason: " + e.SelectedOverlay.RejectReason;
	}

	private void FitButton_Click(object sender, RoutedEventArgs e)
	{
		if (initialized)
		{
			session.FitImageToView();
		}
	}

	private void ClearSelectionButton_Click(object sender, RoutedEventArgs e)
	{
		if (initialized)
		{
			session.ClearSelection();
			overlayList.SelectedItem = null;
		}
	}

	private void CaptureViewButton_Click(object sender, RoutedEventArgs e)
	{
		if (initialized)
		{
			capturedViewState = session.CaptureViewState();
			contractText.Text = "뷰 상태 저장됨: Zoom/Pan 값이 외부 계약 DTO로 캡처되었습니다.";
		}
	}

	private void RestoreViewButton_Click(object sender, RoutedEventArgs e)
	{
		if (initialized && capturedViewState != null)
		{
			session.ApplyViewState(capturedViewState);
			contractText.Text = "뷰 상태 복원됨: 선택된 overlay ID와 geometry는 유지됩니다.";
		}
	}

	private void RunSmoke()
	{
		try
		{
			if (!string.Equals(session.SelectedOverlayId, "candidate-accepted", StringComparison.Ordinal))
			{
				throw new InvalidOperationException("Initial overlay selection did not reach the external session.");
			}

			overlayList.SelectedItem = session.Overlays[1];
			if (!string.Equals(session.SelectedOverlayId, "candidate-rejected", StringComparison.Ordinal)
				|| session.SelectedOverlay == null
				|| session.SelectedOverlay.Accepted
				|| !string.Equals(session.SelectedOverlay.RejectReason, "WidthBelowMinimum", StringComparison.Ordinal))
			{
				throw new InvalidOperationException("Rejected overlay selection/highlight metadata did not round-trip.");
			}

			ImageCanvasExternalViewState before = session.CaptureViewState();
			session.FitImageToView();
			ImageCanvasExternalViewState fit = session.CaptureViewState();
			if (fit == null || fit.Zoom <= 0)
			{
				throw new InvalidOperationException("Fit did not produce a valid view state.");
			}
			try
			{
				session.ApplyViewState(before);
			}
			catch (Exception exception)
			{
				throw new InvalidOperationException(
					$"Captured view state was invalid: Zoom={before.Zoom}, OffsetX={before.OffsetX}, OffsetY={before.OffsetY}.",
					exception);
			}
			ImageCanvasExternalViewState restored = session.CaptureViewState();
			AssertClose(before.Zoom, restored.Zoom, "Zoom");
			AssertClose(before.OffsetX, restored.OffsetX, "OffsetX");
			AssertClose(before.OffsetY, restored.OffsetY, "OffsetY");

			if (!session.SetOverlayVisible("candidate-rejected", false)
				|| !session.SetOverlayVisible("candidate-rejected", true)
				|| session.RemoveOverlay("unknown"))
			{
				throw new InvalidOperationException("Overlay visibility or unknown-id contract failed.");
			}

			ResourceSnapshot resourceBefore = CaptureResourceSnapshot();
			RunCreateDisposeCycles(100);
			ResourceSnapshot resourceAfter = CaptureResourceSnapshotAfterCollection();
			if (resourceAfter.HandleCount - resourceBefore.HandleCount > 8)
			{
				throw new InvalidOperationException(
					$"100 create/dispose cycles did not reach a handle plateau: before={resourceBefore.HandleCount}, after={resourceAfter.HandleCount}.");
			}
			string outputDirectory = smokeOutputDirectory ?? throw new InvalidOperationException("Smoke output directory is missing.");
			Directory.CreateDirectory(outputDirectory);
			// SelectionChanged updates the WPF item template on the next render pass;
			// wait for that pass so the screen evidence agrees with the external session.
			Dispatcher.Invoke(DispatcherPriority.Render, new Action(UpdateLayout));
			SaveSourceImage(outputDirectory);
			SaveWindowScreenshot(outputDirectory);
			File.WriteAllText(
				Path.Combine(outputDirectory, "contract-smoke.txt"),
				$"SelectionChangedCount={selectionChangedCount}{Environment.NewLine}"
				+ $"SelectedOverlay={session.SelectedOverlayId}{Environment.NewLine}"
				+ $"Image={session.ImageWidth}x{session.ImageHeight}{Environment.NewLine}"
				+ $"OverlayCount={session.Overlays.Count}{Environment.NewLine}"
				+ "OwnershipModes=Borrow,Clone,TakeOwnership" + Environment.NewLine
				+ $"TestMonitor={testMonitor.DeviceName}{Environment.NewLine}"
				+ $"TestMonitorBounds={testMonitor.Bounds}{Environment.NewLine}"
				+ $"WindowBounds={Left:0.##},{Top:0.##},{ActualWidth:0.##},{ActualHeight:0.##}{Environment.NewLine}"
				+ $"WindowDeviceBounds={GetWindowDeviceBoundsText()}{Environment.NewLine}"
				+ $"WindowInsideTestMonitor={IsWindowInsideTestMonitor()}{Environment.NewLine}"
				+ $"DpiScale={GetDpiScaleText()}{Environment.NewLine}"
				+ $"ResourceBefore=Handles:{resourceBefore.HandleCount};WorkingSet:{resourceBefore.WorkingSetBytes}{Environment.NewLine}"
				+ $"ResourceAfter=Handles:{resourceAfter.HandleCount};WorkingSet:{resourceAfter.WorkingSetBytes}{Environment.NewLine}"
				+ $"ResourceDelta=Handles:{resourceAfter.HandleCount - resourceBefore.HandleCount};WorkingSet:{resourceAfter.WorkingSetBytes - resourceBefore.WorkingSetBytes}{Environment.NewLine}"
				+ "ViewStateRoundTrip=PASS" + Environment.NewLine
				+ "CreateDisposeCycles=100" + Environment.NewLine
				+ "Result=PASS" + Environment.NewLine);
			System.Windows.Application.Current.Shutdown(0);
		}
		catch (Exception exception)
		{
			FailAndShutdown("Smoke failed: " + exception);
		}
	}

	private void ConfigureProjectionWatcher()
	{
		string exchangeRoot = projectionExchangeRoot ?? throw new InvalidOperationException("Projection exchange root is missing.");
		string transactionsRoot = Path.Combine(Path.GetFullPath(exchangeRoot), "transactions");
		if (!Directory.Exists(transactionsRoot))
		{
			throw new DirectoryNotFoundException("Projection transactions directory was not found: " + transactionsRoot);
		}

		projectionRefreshTimer = new DispatcherTimer(
			TimeSpan.FromMilliseconds(150),
			DispatcherPriority.DataBind,
			(_, _) =>
			{
				projectionRefreshTimer!.Stop();
				ApplyLatestProjectionResult();
			},
			Dispatcher);
		projectionWatcher = new FileSystemWatcher(transactionsRoot, "result.json")
		{
			IncludeSubdirectories = true,
			NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
		};
		projectionWatcher.Created += ProjectionWatcher_Changed;
		projectionWatcher.Changed += ProjectionWatcher_Changed;
		projectionWatcher.Renamed += ProjectionWatcher_Renamed;
		projectionWatcher.EnableRaisingEvents = true;
	}

	private void ProjectionWatcher_Changed(object sender, FileSystemEventArgs e) => ScheduleProjectionRefresh();

	private void ProjectionWatcher_Renamed(object sender, RenamedEventArgs e) => ScheduleProjectionRefresh();

	private void ScheduleProjectionRefresh()
	{
		if (Dispatcher.HasShutdownStarted || Dispatcher.HasShutdownFinished)
		{
			return;
		}

		Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
		{
			if (!IsLoaded || projectionRefreshTimer == null)
			{
				return;
			}
			projectionRefreshTimer.Stop();
			projectionRefreshTimer.Start();
		}));
	}

	private void ApplyLatestProjectionResult()
	{
		if (!initialized || projectionExchangeRoot == null || projectionTwoDTransactionId == null)
		{
			return;
		}

		try
		{
			string transactionsRoot = Path.Combine(Path.GetFullPath(projectionExchangeRoot), "transactions");
			ImageCanvasExternalProjectionResult? latest = null;
			string? latestPath = null;
			foreach (string path in Directory.EnumerateFiles(
				transactionsRoot,
				"coordinate-projection-result.json",
				SearchOption.AllDirectories))
			{
				try
				{
					ImageCanvasExternalProjectionResult candidate = ImageCanvasExternalProjectionResultReader.Read(
						path,
						projectionTwoDTransactionId,
						session.ImageWidth,
						session.ImageHeight);
					if (latest == null || candidate.RecordedAtUtc > latest.RecordedAtUtc)
					{
						latest = candidate;
						latestPath = path;
					}
				}
				catch (IOException)
				{
					// A result can be observed while the producer is still atomically replacing it.
				}
				catch (JsonException)
				{
					// Ignore unrelated or incomplete result files; the next watcher event retries.
				}
				catch (InvalidDataException)
				{
					// A mismatched transaction/result is not allowed onto the screen.
				}
			}

			if (latest == null)
			{
				projectionStatusText.Text = "3D→2D marker 대기 중 / Waiting for 3D→2D markers";
				return;
			}

			IReadOnlyList<ImageCanvasExternalProjectedPoint> visible =
				ImageCanvasExternalProjectionResultReader.GetVisibleThreeDToTwoD(latest);
			projectionMarkerCount = session.ReplaceProjectionMarkers(visible);
			overlayList.ItemsSource = null;
			overlayList.ItemsSource = session.Overlays;
			projectionRefreshCount++;
			projectionResultPath = latestPath;
			projectionApplied = projectionMarkerCount > 0;
			projectionStatusText.Text =
				$"3D→2D marker {projectionMarkerCount}/{latest.ThreeDToTwoD.Count} · {latest.Outcome}";
			contractText.Text = "3D 결과를 읽기 전용 marker로 반영했습니다. Pipeline/Run은 실행하지 않습니다.";
		}
		catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
		{
			projectionStatusText.Text = "3D→2D 결과 읽기 실패 / Projection result read failed";
			contractText.Text = exception.Message;
		}
	}

	private void WaitForProjectionSmoke()
	{
		if (!projectionMode)
		{
			return;
		}

		projectionSmokeTimeoutTimer = new DispatcherTimer(
			TimeSpan.FromMilliseconds(100),
			DispatcherPriority.ApplicationIdle,
			(_, _) =>
			{
				if (projectionApplied)
				{
					projectionSmokeTimeoutTimer!.Stop();
					RunProjectionSmoke();
				}
			},
			Dispatcher);
		projectionSmokeTimeoutTimer.Start();
		projectionSmokeDeadlineTimer = new DispatcherTimer(
			TimeSpan.FromSeconds(45),
			DispatcherPriority.ApplicationIdle,
			(_, _) =>
			{
				projectionSmokeDeadlineTimer!.Stop();
				projectionSmokeTimeoutTimer?.Stop();
				FailAndShutdown("3D→2D projection Result was not observed within 45 seconds.");
			},
			Dispatcher);
		projectionSmokeDeadlineTimer.Start();
	}

	private void RunProjectionSmoke()
	{
		try
		{
			if (projectionMarkerCount <= 0 || session.ProjectionOverlayCount != projectionMarkerCount)
			{
				throw new InvalidOperationException("The reverse-projected marker set was not rendered by ImageCanvas.");
			}
			projectionSmokeDeadlineTimer?.Stop();

			Dispatcher.Invoke(DispatcherPriority.Render, new Action(UpdateLayout));
			string outputDirectory = smokeOutputDirectory ?? throw new InvalidOperationException("Smoke output directory is missing.");
			Directory.CreateDirectory(outputDirectory);
			SaveWindowScreenshot(outputDirectory, "cross-modal-2d-screen.png");
			File.WriteAllText(
				Path.Combine(outputDirectory, "cross-modal-2d-screen-smoke.txt"),
				$"ProjectionRefreshCount={projectionRefreshCount}{Environment.NewLine}"
				+ $"ProjectionMarkerCount={projectionMarkerCount}{Environment.NewLine}"
				+ $"ProjectionResult={projectionResultPath}{Environment.NewLine}"
				+ $"Image={session.ImageWidth}x{session.ImageHeight}{Environment.NewLine}"
				+ $"TestMonitor={testMonitor.DeviceName}{Environment.NewLine}"
				+ $"TestMonitorBounds={testMonitor.Bounds}{Environment.NewLine}"
				+ $"WindowBounds={Left:0.##},{Top:0.##},{ActualWidth:0.##},{ActualHeight:0.##}{Environment.NewLine}"
				+ $"WindowDeviceBounds={GetWindowDeviceBoundsText()}{Environment.NewLine}"
				+ $"WindowInsideTestMonitor={IsWindowInsideTestMonitor()}{Environment.NewLine}"
				+ $"DpiScale={GetDpiScaleText()}{Environment.NewLine}"
				+ "PipelineExecution=NotInvoked" + Environment.NewLine
				+ "LayerMutation=NotInvoked" + Environment.NewLine
				+ "Result=PASS" + Environment.NewLine);
			System.Windows.Application.Current.Shutdown(0);
		}
		catch (Exception exception)
		{
			FailAndShutdown("Cross-modal screen smoke failed: " + exception);
		}
	}

	private void WriteProjectionScreenReady()
	{
		if (string.IsNullOrWhiteSpace(smokeOutputDirectory))
		{
			return;
		}

		Directory.CreateDirectory(smokeOutputDirectory);
		File.WriteAllText(
			Path.Combine(smokeOutputDirectory, "cross-modal-2d-screen-ready.txt"),
			$"ProjectionScreenReady=PASS{Environment.NewLine}"
			+ $"Image={session.ImageWidth}x{session.ImageHeight}{Environment.NewLine}"
			+ $"TestMonitor={testMonitor.DeviceName}{Environment.NewLine}"
			+ $"WindowInsideTestMonitor={IsWindowInsideTestMonitor()}{Environment.NewLine}");
	}

	private static void AssertNativeProjectionScreenEvidence(Bitmap capture)
	{
		int cyan = 0;
		for (int y = 0; y < capture.Height; y += 2)
		{
			for (int x = 0; x < capture.Width; x += 2)
			{
				DrawingColor color = capture.GetPixel(x, y);
				if (color.R < 100 && color.G > 120 && color.B > 180)
				{
					cyan++;
				}
			}
		}

		if (cyan < 50)
		{
			throw new InvalidOperationException($"Native screen evidence did not contain the reverse-projected cyan markers: cyan={cyan}.");
		}
	}

	private static void RunCreateDisposeCycles(int count)
	{
		for (int i = 0; i < count; i++)
		{
			using ImageCanvasExternalConsumerSession cycle = new ImageCanvasExternalConsumerSession("ExternalConsumerCycle" + i);
		}
	}

	private static ResourceSnapshot CaptureResourceSnapshot()
	{
		using Process process = Process.GetCurrentProcess();
		process.Refresh();
		return new ResourceSnapshot(process.HandleCount, process.WorkingSet64);
	}

	private static ResourceSnapshot CaptureResourceSnapshotAfterCollection()
	{
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
		GC.WaitForPendingFinalizers();
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
		return CaptureResourceSnapshot();
	}

	private void SaveSourceImage(string outputDirectory)
	{
		using Bitmap source = CreateSampleImage();
		source.Save(Path.Combine(outputDirectory, "source.png"), ImageFormat.Png);
	}

	private Bitmap LoadProjectionSourceImage()
	{
		if (!TryReadProjectionSmokeArguments(out ProjectionSmokeOptions options))
		{
			throw new InvalidOperationException("Projection smoke arguments are missing.");
		}

		using Bitmap loaded = new Bitmap(options.SourceImagePath);
		return new Bitmap(loaded);
	}

	private void SaveWindowScreenshot(string outputDirectory, string fileName = "external-consumer-selected-rejected.png")
	{
		UpdateLayout();
		if (!GetWindowRect(new WindowInteropHelper(this).Handle, out NativeRect windowRect))
		{
			throw new InvalidOperationException("Could not resolve the external consumer window device bounds.");
		}

		int width = Math.Max(1, windowRect.Right - windowRect.Left);
		int height = Math.Max(1, windowRect.Bottom - windowRect.Top);
		using Bitmap capture = new Bitmap(width, height, PixelFormat.Format32bppArgb);
		using Graphics graphics = Graphics.FromImage(capture);
		graphics.CopyFromScreen(
			(int)Math.Round(Left),
			(int)Math.Round(Top),
			0,
			0,
			new System.Drawing.Size(width, height));
		string capturePath = Path.Combine(outputDirectory, fileName);
		capture.Save(capturePath, ImageFormat.Png);
		if (projectionMode)
		{
			AssertNativeProjectionScreenEvidence(capture);
		}
		else
		{
			AssertNativeScreenEvidence(capture);
		}
	}

	private void VerifyBorrowAndCloneOwnership()
	{
		using Bitmap borrowed = CreateSampleImage();
		session.LoadImage(borrowed, "external-consumer-borrow", ImageCanvasImageOwnership.Borrow);
		if (borrowed.Width != 720 || borrowed.Height != 480)
		{
			throw new InvalidOperationException("Borrow ownership did not leave the caller bitmap usable.");
		}

		using Bitmap cloned = CreateSampleImage();
		session.LoadImage(cloned, "external-consumer-clone", ImageCanvasImageOwnership.Clone);
		if (cloned.Width != 720 || cloned.Height != 480)
		{
			throw new InvalidOperationException("Clone ownership did not leave the caller bitmap usable.");
		}
	}

	private static void AssertNativeScreenEvidence(Bitmap capture)
	{
		int green = 0;
		int red = 0;
		int yellow = 0;
		for (int y = 0; y < capture.Height; y += 2)
		{
			for (int x = 0; x < capture.Width; x += 2)
			{
				DrawingColor color = capture.GetPixel(x, y);
				if (color.G > color.R * 1.35 && color.G > color.B * 1.2)
				{
					green++;
				}
				if (color.R > color.G * 1.25 && color.R > color.B * 1.25)
				{
					red++;
				}
				if (color.R > 180 && color.G > 150 && color.B < 110)
				{
					yellow++;
				}
			}
		}

		if (green < 50 || red < 50 || yellow < 50)
		{
			throw new InvalidOperationException(
				$"Native screen evidence did not contain the expected accepted/rejected/highlight colors: green={green}, red={red}, yellow={yellow}.");
		}
	}

	private string GetWindowDeviceBoundsText()
	{
		return GetWindowRect(new WindowInteropHelper(this).Handle, out NativeRect rect)
			? $"{rect.Left},{rect.Top},{rect.Right - rect.Left},{rect.Bottom - rect.Top}"
			: "unavailable";
	}

	private string IsWindowInsideTestMonitor()
	{
		if (!GetWindowRect(new WindowInteropHelper(this).Handle, out NativeRect rect))
		{
			return "unavailable";
		}

		return rect.Left >= testMonitor.WorkingArea.Left
			&& rect.Top >= testMonitor.WorkingArea.Top
			&& rect.Right <= testMonitor.WorkingArea.Right
			&& rect.Bottom <= testMonitor.WorkingArea.Bottom
			? "PASS"
			: $"FAIL({rect.Left},{rect.Top},{rect.Right},{rect.Bottom})";
	}

	private string GetDpiScaleText()
	{
		System.Windows.Media.Matrix transform = PresentationSource.FromVisual(this)?.CompositionTarget?.TransformToDevice
			?? System.Windows.Media.Matrix.Identity;
		return $"{transform.M11:0.##}x{transform.M22:0.##}";
	}

	private static Bitmap CreateSampleImage()
	{
		Bitmap bitmap = new Bitmap(720, 480, PixelFormat.Format24bppRgb);
		using Graphics graphics = Graphics.FromImage(bitmap);
		graphics.Clear(DrawingColor.FromArgb(25, 30, 37));
		using Brush gridBrush = new SolidBrush(DrawingColor.FromArgb(45, 55, 67));
		for (int x = 0; x < bitmap.Width; x += 40)
		{
			graphics.FillRectangle(gridBrush, x, 0, 1, bitmap.Height);
		}
		for (int y = 0; y < bitmap.Height; y += 40)
		{
			graphics.FillRectangle(gridBrush, 0, y, bitmap.Width, 1);
		}
		using Brush acceptedBrush = new SolidBrush(DrawingColor.FromArgb(120, 55, 150, 75));
		using Brush rejectedBrush = new SolidBrush(DrawingColor.FromArgb(120, 160, 55, 45));
		graphics.FillRectangle(acceptedBrush, 80, 76, 206, 156);
		graphics.FillRectangle(rejectedBrush, 390, 214, 250, 180);
		return bitmap;
	}

	private bool TryReadSmokeArguments(out string outputDirectory)
	{
		int index = Array.FindIndex(arguments, value => string.Equals(value, "--smoke", StringComparison.OrdinalIgnoreCase));
		if (index < 0)
		{
			outputDirectory = string.Empty;
			return false;
		}

		outputDirectory = index + 1 < arguments.Length && !string.IsNullOrWhiteSpace(arguments[index + 1])
			? arguments[index + 1]
			: Path.Combine(Path.GetTempPath(), "OpenVisionLab-ImageCanvasExternalConsumer");
		return true;
	}

	private bool TryReadProjectionSmokeArguments(out ProjectionSmokeOptions options)
	{
		int index = Array.FindIndex(arguments, value => string.Equals(value, "--projection-smoke", StringComparison.OrdinalIgnoreCase));
		if (index < 0 || index + 4 >= arguments.Length)
		{
			options = null!;
			return false;
		}

		string outputDirectory = arguments[index + 1];
		string exchangeRoot = arguments[index + 2];
		string transactionId = arguments[index + 3];
		string sourceImagePath = arguments[index + 4];
		if (string.IsNullOrWhiteSpace(outputDirectory)
			|| string.IsNullOrWhiteSpace(exchangeRoot)
			|| !Guid.TryParse(transactionId, out _)
			|| !File.Exists(sourceImagePath))
		{
			options = null!;
			return false;
		}

		options = new ProjectionSmokeOptions(
			Path.GetFullPath(outputDirectory),
			Path.GetFullPath(exchangeRoot),
			transactionId,
			Path.GetFullPath(sourceImagePath));
		return true;
	}

	private void MainWindow_Closed(object? sender, EventArgs e)
	{
		projectionSmokeTimeoutTimer?.Stop();
		projectionSmokeDeadlineTimer?.Stop();
		projectionRefreshTimer?.Stop();
		if (projectionWatcher != null)
		{
			projectionWatcher.EnableRaisingEvents = false;
			projectionWatcher.Created -= ProjectionWatcher_Changed;
			projectionWatcher.Changed -= ProjectionWatcher_Changed;
			projectionWatcher.Renamed -= ProjectionWatcher_Renamed;
			projectionWatcher.Dispose();
			projectionWatcher = null;
		}
		session.Dispose();
	}

	private void FailAndShutdown(string message)
	{
		contractText.Text = message;
		try
		{
			if (smokeMode && !string.IsNullOrWhiteSpace(smokeOutputDirectory))
			{
				Directory.CreateDirectory(smokeOutputDirectory);
				File.WriteAllText(Path.Combine(smokeOutputDirectory, "contract-smoke.txt"), "Result=FAIL" + Environment.NewLine + message + Environment.NewLine);
			}
		}
		finally
		{
			System.Windows.Application.Current.Shutdown(1);
		}
	}

	private void PlaceOnTestMonitor()
	{
		FormsScreen[] screens = FormsScreen.AllScreens;
		testMonitor = screens.Length == 0
			? FormsScreen.PrimaryScreen ?? throw new InvalidOperationException("No Windows monitor is available.")
			: screens.Length == 2
				? screens.OrderBy(screen => (long)screen.Bounds.Width * screen.Bounds.Height)
					.ThenBy(screen => screen.Bounds.Left)
					.First()
				: screens[0];

		WindowStartupLocation = WindowStartupLocation.Manual;
		Left = 0;
		Top = 0;
	}

	private void PositionWindowOnTestMonitor()
	{
		IntPtr handle = new WindowInteropHelper(this).Handle;
		if (handle == IntPtr.Zero)
		{
			return;
		}

		if (!GetWindowRect(handle, out NativeRect current))
		{
			return;
		}

		int width = Math.Max(1, current.Right - current.Left);
		int height = Math.Max(1, current.Bottom - current.Top);
		int left = testMonitor.WorkingArea.Left + 40;
		int top = testMonitor.WorkingArea.Top + 40;
		if (left + width > testMonitor.WorkingArea.Right)
		{
			left = testMonitor.WorkingArea.Right - width;
		}
		if (top + height > testMonitor.WorkingArea.Bottom)
		{
			top = testMonitor.WorkingArea.Bottom - height;
		}

		SetWindowPos(handle, IntPtr.Zero, left, top, 0, 0, SwpNoSize | SwpNoZOrder | SwpNoActivate);
	}

	private static void AssertClose(float expected, float actual, string name)
	{
		if (Math.Abs(expected - actual) > 0.01f)
		{
			throw new InvalidOperationException($"{name} did not round-trip: {expected} / {actual}.");
		}
	}

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetWindowRect(IntPtr hWnd, out NativeRect lpRect);

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool SetWindowPos(
		IntPtr hWnd,
		IntPtr hWndInsertAfter,
		int x,
		int y,
		int cx,
		int cy,
		uint uFlags);

	private const uint SwpNoSize = 0x0001;
	private const uint SwpNoZOrder = 0x0004;
	private const uint SwpNoActivate = 0x0010;

	[StructLayout(LayoutKind.Sequential)]
	private struct NativeRect
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}

	private readonly record struct ResourceSnapshot(int HandleCount, long WorkingSetBytes);

	private sealed record ProjectionSmokeOptions(
		string OutputDirectory,
		string ExchangeRoot,
		string TwoDTransactionId,
		string SourceImagePath);
}
