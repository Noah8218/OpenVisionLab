using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeExecutionSessionViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private bool isValidationSuiteRunning;
        private bool isLocalValidationSetRunning;
        private bool isSampleCheckRunning;
        private bool isPairCheckRunning;
        private bool isCatalogBenchmarkRunning;
        private bool stopRequested;
        private string statusText = string.Empty;
        private string executionStatusText = string.Empty;
        private bool hasCatalogBenchmarkSamples;
        private OpenVisionRecipeSampleRunSummary latestSampleRunSummary = OpenVisionRecipeSampleRunSummary.Empty;
        private OpenVisionRecipePairRunSummary latestPairRunSummary = OpenVisionRecipePairRunSummary.Empty;
        private OpenVisionRecipeCatalogBenchmarkSummary latestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.Empty;
        private readonly OpenVisionRecipeValidationSetRunner validationSetRunner = new OpenVisionRecipeValidationSetRunner();
        private readonly OpenVisionRecipeSelectedSampleExecutionOwner selectedSampleExecutionOwner =
            new OpenVisionRecipeSelectedSampleExecutionOwner();
        private readonly OpenVisionRecipePairExecutionOwner pairExecutionOwner =
            new OpenVisionRecipePairExecutionOwner();
        private readonly OpenVisionRecipeCatalogExecutionOwner catalogExecutionOwner =
            new OpenVisionRecipeCatalogExecutionOwner();

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        public event EventHandler BatchRunSaved;

        // Requery commands at the existing workflow boundaries, after status and summary updates.
        public event EventHandler CommandStateChanged;

        public bool IsValidationSuiteRunning
        {
            get => isValidationSuiteRunning;
            private set => SetField(ref isValidationSuiteRunning, value);
        }

        public bool IsLocalValidationSetRunning
        {
            get => isLocalValidationSetRunning;
            private set => SetField(ref isLocalValidationSetRunning, value);
        }

        public bool IsSampleCheckRunning
        {
            get => isSampleCheckRunning;
            private set => SetField(ref isSampleCheckRunning, value);
        }

        public bool IsPairCheckRunning
        {
            get => isPairCheckRunning;
            private set => SetField(ref isPairCheckRunning, value);
        }

        public bool IsCatalogBenchmarkRunning
        {
            get => isCatalogBenchmarkRunning;
            private set => SetField(ref isCatalogBenchmarkRunning, value);
        }

        public bool StopRequested
        {
            get => stopRequested;
            private set => SetField(ref stopRequested, value);
        }

        public string StatusText
        {
            get => statusText;
            private set => SetField(ref statusText, value ?? string.Empty);
        }

        public string ExecutionStatusText
        {
            get => executionStatusText;
            private set
            {
                executionStatusText = value ?? string.Empty;
                // The shell status may have changed independently since the previous execution update.
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExecutionStatusText)));
            }
        }

        public OpenVisionRecipeSampleRunSummary LatestSampleRunSummary
        {
            get => latestSampleRunSummary;
            set => SetField(ref latestSampleRunSummary, value ?? OpenVisionRecipeSampleRunSummary.Empty);
        }

        public OpenVisionRecipePairRunSummary LatestPairRunSummary
        {
            get => latestPairRunSummary;
            set => SetField(ref latestPairRunSummary, value ?? OpenVisionRecipePairRunSummary.Empty);
        }

        public OpenVisionRecipeCatalogBenchmarkSummary LatestCatalogBenchmarkSummary
        {
            get => latestCatalogBenchmarkSummary;
            set => SetField(ref latestCatalogBenchmarkSummary, value ?? OpenVisionRecipeCatalogBenchmarkSummary.Empty);
        }

        public bool HasCatalogBenchmarkSamples
        {
            get => hasCatalogBenchmarkSamples;
            set => SetField(ref hasCatalogBenchmarkSamples, value);
        }

        public bool CanStop => IsLocalValidationSetRunning && !StopRequested;

        public async Task RunLocalValidationSetAsync(string recipeName, string pipelineName, OpenVisionRecipeValidationSetOption option)
        {
            string setName = option.Name;
            OpenVisionRecipeValidationSetRunRequest runRequest = validationSetRunner.CreateRunRequest(
                recipeName,
                pipelineName,
                option,
                () => StopRequested,
                (completed, total) =>
                {
                    StatusText = string.Format(
                        CultureInfo.CurrentCulture,
                        LocalText("로컬 세트 실행 중: {0} ({1}/{2})", "Running local set: {0} ({1}/{2})"),
                        setName,
                        completed,
                        total);
                });

            StartValidationSuite(
                true,
                LocalText("로컬 세트 실행 중: ", "Running local set: ") + runRequest.SetName);
            ExecutionStatusText = StatusText;
            CommandStateChanged?.Invoke(this, EventArgs.Empty);

            try
            {
                OpenVisionRecipeValidationSetRunResult runResult =
                    await validationSetRunner.RunAsync(runRequest);
                BatchRunSaved?.Invoke(this, EventArgs.Empty);
                StatusText = string.Format(
                    CultureInfo.CurrentCulture,
                    runResult.IsPartial
                        ? LocalText("목록 검증 중단·부분 저장: {0}/{1} 판정 일치 | {2}", "Image-list run stopped and partially saved: {0}/{1} judgments matched | {2}")
                        : LocalText("목록 검증 저장됨: {0}/{1} 판정 일치 | {2}", "Image-list run saved: {0}/{1} judgments matched | {2}"),
                    runResult.CorrectCount,
                    runResult.CompletedCount,
                    runResult.SummaryPath);
                ExecutionStatusText = StatusText;
            }
            catch (Exception ex)
            {
                StatusText = LocalText("로컬 세트 ERROR: ", "Local set ERROR: ") + ex.GetBaseException().Message;
                ExecutionStatusText = StatusText;
            }
            finally
            {
                CompleteValidationSuite();
                CommandStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task RunValidationSuiteAsync(
            string scope,
            string recipeName,
            string pipelineName,
            OpenVisionRecipeSampleOption sampleOption,
            OpenVisionRecipeValidationSetOption validationSetOption)
        {
            if (string.Equals(
                    scope,
                    OpenVisionRecipeValidationSuiteScopeOption.LocalValidationSetKey,
                    StringComparison.OrdinalIgnoreCase))
            {
                if (validationSetOption?.Set == null)
                {
                    return;
                }

                await RunLocalValidationSetAsync(recipeName, pipelineName, validationSetOption);
                return;
            }

            if (string.Equals(
                    scope,
                    OpenVisionRecipeValidationSuiteScopeOption.GoodBadPairKey,
                    StringComparison.OrdinalIgnoreCase))
            {
                StatusText = LocalText("Good/Bad suite 실행 시작.", "Started Good/Bad suite.");
                await RunSelectedSamplePairCheckAsync(recipeName, pipelineName, sampleOption);
                return;
            }

            if (string.Equals(
                    scope,
                    OpenVisionRecipeValidationSuiteScopeOption.CatalogKey,
                    StringComparison.OrdinalIgnoreCase))
            {
                StatusText = LocalText("Catalog suite 실행 시작.", "Started catalog suite.");
                await RunCatalogBenchmarkAsync(recipeName, pipelineName);
                return;
            }

            await RunSelectedSampleValidationSuiteAsync(recipeName, pipelineName, sampleOption);
        }

        public async Task RunSelectedSampleValidationSuiteAsync(string recipeName, string pipelineName, OpenVisionRecipeSampleOption sampleOption)
        {
            StartValidationSuite(
                false,
                LocalText("Selected sample suite 실행 중: ", "Running selected-sample suite: ") + sampleOption.SampleName);
            StartSampleCheck();
            LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.CreateRunning(sampleOption, recipeName, pipelineName);
            ExecutionStatusText = StatusText;
            CommandStateChanged?.Invoke(this, EventArgs.Empty);

            DateTime startedAt = DateTime.Now;
            try
            {
                (VisionPipelineSampleCheckResult result, string summaryPath) =
                    await selectedSampleExecutionOwner.RunSuiteAsync(
                        recipeName,
                        pipelineName,
                        sampleOption,
                        startedAt);
                LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.FromResult(sampleOption, recipeName, pipelineName, result);
                BatchRunSaved?.Invoke(this, EventArgs.Empty);
                StatusText = LocalText("Selected sample suite 저장됨: ", "Selected-sample suite saved: ") + summaryPath;
                ExecutionStatusText = LocalText("샘플 검사 ", "Sample check ") + result.Status + ": " + sampleOption.SampleName;
            }
            catch (Exception ex)
            {
                VisionPipelineSampleCheckResult result = VisionPipelineSampleCheckService.CreateErrorResult(
                    ex.GetBaseException().Message);
                LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.FromResult(sampleOption, recipeName, pipelineName, result);
                StatusText = LocalText("Selected sample suite ERROR: ", "Selected-sample suite ERROR: ") + result.Message;
                ExecutionStatusText = StatusText;
            }
            finally
            {
                CompleteSampleCheck();
                CompleteValidationSuite();
                CommandStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task RunSelectedSampleCheckAsync(string recipeName, string pipelineName, OpenVisionRecipeSampleOption sampleOption)
        {
            StartSampleCheck();
            LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.CreateRunning(sampleOption, recipeName, pipelineName);
            ExecutionStatusText = LocalText("샘플 검사 실행 중: ", "Running sample check: ") + sampleOption.SampleName;
            CommandStateChanged?.Invoke(this, EventArgs.Empty);

            try
            {
                VisionPipelineSampleCheckResult result =
                    await selectedSampleExecutionOwner.RunCheckAsync(recipeName, pipelineName, sampleOption);
                LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.FromResult(sampleOption, recipeName, pipelineName, result);
                ExecutionStatusText = LocalText("샘플 검사 ", "Sample check ") + result.Status + ": " + sampleOption.SampleName;
            }
            catch (Exception ex)
            {
                VisionPipelineSampleCheckResult result = VisionPipelineSampleCheckService.CreateErrorResult(
                    ex.GetBaseException().Message);
                LatestSampleRunSummary = OpenVisionRecipeSampleRunSummary.FromResult(sampleOption, recipeName, pipelineName, result);
                ExecutionStatusText = LocalText("샘플 검사 ERROR: ", "Sample check ERROR: ") + result.Message;
            }
            finally
            {
                CompleteSampleCheck();
                CommandStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task RunSelectedSamplePairCheckAsync(string recipeName, string pipelineName, OpenVisionRecipeSampleOption sampleOption)
        {
            IReadOnlyList<VisionPipelineSampleCatalogItem> pairSamples = pairExecutionOwner.GetPairSamples(sampleOption.Sample);

            StartPairCheck();
            LatestPairRunSummary = OpenVisionRecipePairRunSummary.CreateRunning(sampleOption, pipelineName, pairSamples.Count);
            ExecutionStatusText = LocalText("Good/Bad 쌍 검사 실행 중: ", "Running Good/Bad pair check: ") + sampleOption.Sample.PairGroup;
            CommandStateChanged?.Invoke(this, EventArgs.Empty);

            DateTime startedAt = DateTime.Now;
            try
            {
                (IReadOnlyList<OpenVisionRecipePairSampleRunSummary> pairResults, string summaryPath) =
                    await pairExecutionOwner.RunAsync(
                        recipeName,
                        pipelineName,
                        sampleOption,
                        pairSamples,
                        startedAt);
                LatestPairRunSummary = OpenVisionRecipePairRunSummary.FromResults(
                    sampleOption,
                    pipelineName,
                    pairResults,
                    summaryPath);
                BatchRunSaved?.Invoke(this, EventArgs.Empty);
                StatusText = LocalText("Good/Bad suite 저장됨: ", "Good/Bad suite saved: ") + summaryPath;
                ExecutionStatusText = LatestPairRunSummary.StatusText + ": " + sampleOption.Sample.PairGroup;
            }
            catch (Exception ex)
            {
                LatestPairRunSummary = OpenVisionRecipePairRunSummary.FromError(
                    sampleOption,
                    pipelineName,
                    ex.GetBaseException().Message);
                StatusText = LocalText("Good/Bad suite ERROR: ", "Good/Bad suite ERROR: ") + ex.GetBaseException().Message;
                ExecutionStatusText = LocalText("쌍 검사 ERROR: ", "Pair check ERROR: ") + ex.GetBaseException().Message;
            }
            finally
            {
                CompletePairCheck();
                CommandStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task RunCatalogBenchmarkAsync(string recipeName, string pipelineName)
        {
            IReadOnlyList<VisionPipelineSampleCatalogItem> samples = catalogExecutionOwner.GetBenchmarkSamples();
            if (samples.Count == 0)
            {
                HasCatalogBenchmarkSamples = false;
                CommandStateChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            StartCatalogBenchmark();
            LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.CreateRunning(pipelineName, samples.Count);
            ExecutionStatusText = LocalText("카탈로그 벤치마크 실행 중: ", "Running catalog benchmark: ") + pipelineName;
            CommandStateChanged?.Invoke(this, EventArgs.Empty);

            DateTime startedAt = DateTime.Now;
            try
            {
                (IReadOnlyList<VisionPipelineBatchSampleRunResult> storageResults, string summaryPath) =
                    await catalogExecutionOwner.RunAsync(
                        recipeName,
                        pipelineName,
                        samples,
                        startedAt,
                        (completed, total, results) =>
                        {
                            LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.CreateProgress(
                                pipelineName,
                                completed,
                                total,
                                results);
                        });
                LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.FromResults(
                    pipelineName,
                    storageResults,
                    summaryPath);
                BatchRunSaved?.Invoke(this, EventArgs.Empty);
                StatusText = LocalText("Catalog suite 저장됨: ", "Catalog suite saved: ") + summaryPath;
                ExecutionStatusText = LatestCatalogBenchmarkSummary.CompactText;
            }
            catch (Exception ex)
            {
                LatestCatalogBenchmarkSummary = OpenVisionRecipeCatalogBenchmarkSummary.FromError(
                    pipelineName,
                    ex.GetBaseException().Message);
                StatusText = LocalText("Catalog suite ERROR: ", "Catalog suite ERROR: ") + ex.GetBaseException().Message;
                ExecutionStatusText = LocalText("카탈로그 벤치마크 ERROR: ", "Catalog benchmark ERROR: ") + ex.GetBaseException().Message;
            }
            finally
            {
                CompleteCatalogBenchmark();
                CommandStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void StartValidationSuite(bool isLocalValidationSet, string status)
        {
            IsValidationSuiteRunning = true;
            IsLocalValidationSetRunning = isLocalValidationSet;
            StopRequested = false;
            StatusText = status;
        }

        public bool RequestStop(string status)
        {
            if (!CanStop)
            {
                return false;
            }

            StopRequested = true;
            StatusText = status;
            ExecutionStatusText = StatusText;
            CommandStateChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        private void CompleteValidationSuite()
        {
            IsValidationSuiteRunning = false;
            IsLocalValidationSetRunning = false;
            StopRequested = false;
        }

        private void StartSampleCheck()
        {
            IsSampleCheckRunning = true;
        }

        private void CompleteSampleCheck()
        {
            IsSampleCheckRunning = false;
        }

        private void StartPairCheck()
        {
            IsPairCheckRunning = true;
        }

        private void CompletePairCheck()
        {
            IsPairCheckRunning = false;
        }

        private void StartCatalogBenchmark()
        {
            IsCatalogBenchmarkRunning = true;
        }

        private void CompleteCatalogBenchmark()
        {
            IsCatalogBenchmarkRunning = false;
        }

        public void SetStatus(string value)
        {
            StatusText = value;
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionRecipeText.Local(korean, english);
        }

        private bool SetField<T>(
            ref T field,
            T value,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
