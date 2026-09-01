using OpenVisionLab.Core.Integration;
using OpenVisionLab.Integration.Contracts;
using OpenVisionLab.Integration.Transport.Tcp;
using OpenVisionLab.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace OpenVisionLab
{
    internal sealed class OpenVisionTcpIntegrationController : INotifyPropertyChanged, IDisposable
    {
        internal const string SharedKeyEnvironmentVariable = "OPENVISIONLAB_TCP_SHARED_KEY";

        private readonly Dispatcher dispatcher;
        private readonly VisionToolLanguageChangeController languageChangeController;
        private readonly RelayCommand saveSettingsCommand;
        private readonly RelayCommand resetSettingsCommand;
        private readonly RelayCommand reloadKeyCommand;
        private readonly RelayCommand startCommand;
        private readonly RelayCommand stopCommand;
        private readonly RelayCommand pingCommand;
        private readonly RelayCommand refreshCommand;
        private readonly RelayCommand acknowledgeCommand;
        private readonly RelayCommand runCommand;
        private readonly RelayCommand pushCommand;
        private readonly RelayCommand pullCommand;

        private OpenVisionTcpIntegrationWindow window;
        private TwoDIntegrationTcpExchange exchange;
        private CancellationTokenSource activeOperationSource;
        private Task activeOperationTask;
        private byte[] sessionSharedKey;
        private bool hasSessionSharedKeyInput;
        private string sessionSharedKeyError = string.Empty;
        private string localExchangeRoot;
        private string listenAddress;
        private string listenPortText;
        private string peerHost;
        private string peerPortText;
        private string validationMessageText = string.Empty;
        private string keyStatusText = string.Empty;
        private string sessionStatusText = string.Empty;
        private string operationStatusText = string.Empty;
        private string lastTransferText = string.Empty;
        private string acknowledgementStatusText = "-";
        private string resultStatusText = "-";
        private string resultOutcomeText = "-";
        private string resultRunIdText = "-";
        private string resultErrorText = "-";
        private OpenVisionTcpIntegrationTransactionRow selectedTransaction;
        private bool isConfigurationValid;
        private bool isKeyValid;
        private bool isListening;
        private bool isBusy;
        private bool isStopping;
        private bool selectedAcknowledgementAccepted;
        private bool disposed;

        internal OpenVisionTcpIntegrationController(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            saveSettingsCommand = new RelayCommand(SaveSettings, () => CanSaveSettings);
            resetSettingsCommand = new RelayCommand(ResetSettings, () => CanEditSettings);
            reloadKeyCommand = new RelayCommand(RefreshValidation, () => CanEditSettings);
            startCommand = new RelayCommand(Start, () => CanStart);
            stopCommand = new RelayCommand(Stop, () => CanStop);
            pingCommand = new RelayCommand(Ping, () => CanUsePeer);
            refreshCommand = new RelayCommand(RefreshTransactions, () => CanUseSession);
            acknowledgeCommand = new RelayCommand(Acknowledge, () => CanAcknowledge);
            runCommand = new RelayCommand(Run, () => CanRun);
            pushCommand = new RelayCommand(Push, () => CanUseSelectedTransaction);
            pullCommand = new RelayCommand(Pull, () => CanUseSelectedTransaction);
            languageChangeController = VisionToolLanguageChangeController.Attach(RefreshLocalization);

            OpenVisionTcpIntegrationSettings settings =
                OpenVisionTcpIntegrationSettingsStore.Load(out string warning);
            ApplySettings(settings);
            SessionStatusText = LocalText("중지됨", "Stopped");
            OperationStatusText = string.IsNullOrWhiteSpace(warning)
                ? LocalText(
                    "설정을 복원했습니다. 수신은 시작되지 않았습니다.",
                    "Settings restored. Listening was not started.")
                : warning;
            RefreshValidation();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<OpenVisionTcpIntegrationTransactionRow> Transactions { get; } =
            new ObservableCollection<OpenVisionTcpIntegrationTransactionRow>();

        public ICommand SaveSettingsCommand => saveSettingsCommand;
        public ICommand ResetSettingsCommand => resetSettingsCommand;
        public ICommand ReloadKeyCommand => reloadKeyCommand;
        public ICommand StartCommand => startCommand;
        public ICommand StopCommand => stopCommand;
        public ICommand PingCommand => pingCommand;
        public ICommand RefreshCommand => refreshCommand;
        public ICommand AcknowledgeCommand => acknowledgeCommand;
        public ICommand RunCommand => runCommand;
        public ICommand PushCommand => pushCommand;
        public ICommand PullCommand => pullCommand;

        public string WindowTitle => LocalText("2D TCP 연동", "2D TCP Integration");
        public string ScopeText => LocalText(
            "수신·전송과 ACK·검사 실행은 각각 명시적으로 수행됩니다. 설정 복원만으로 연결하거나 검사하지 않습니다.",
            "Receive/transfer and ACK/inspection are explicit. Restoring settings never connects or runs an inspection.");
        public string SetupTitleText => LocalText("연결 설정", "Connection setup");
        public string LocalRootLabelText => LocalText("로컬 교환 Root", "Local exchange root");
        public string ListenAddressLabelText => LocalText("수신 IP", "Listen IP");
        public string ListenPortLabelText => LocalText("수신 Port", "Listen port");
        public string PeerHostLabelText => LocalText("상대 Host", "Peer host");
        public string PeerPortLabelText => LocalText("상대 Port", "Peer port");
        public string SharedKeyLabelText => LocalText("공유키 Source", "Shared-key source");
        public string SharedKeySourceText => LocalText(
            "이 세션 입력 또는 " + SharedKeyEnvironmentVariable + " (Base64, 32+ bytes)",
            "This-session input or " + SharedKeyEnvironmentVariable + " (Base64, 32+ bytes)");
        public string SaveText => LocalText("설정 저장", "Save settings");
        public string ResetText => LocalText("기본값", "Reset");
        public string ReloadKeyText => LocalText("키 다시 확인", "Reload key");
        public string StartText => LocalText("수신 시작", "Start listening");
        public string StopText => LocalText("수신 중지", "Stop listening");
        public string PingText => "Ping";
        public string InboxTitleText => LocalText("거래 Inbox", "Transaction inbox");
        public string RefreshText => LocalText("새로 고침", "Refresh");
        public string AcknowledgeText => "ACK";
        public string RunText => LocalText("검사 실행", "Run inspection");
        public string PushText => "Push";
        public string PullText => "Pull";
        public string ResultTitleText => LocalText("선택 거래 결과", "Selected transaction result");
        public string TransactionHeaderText => LocalText("거래 ID", "Transaction ID");
        public string CreatedHeaderText => LocalText("생성 시각", "Created");
        public string ProducerHeaderText => LocalText("생성 앱", "Producer");
        public string TargetHeaderText => LocalText("대상", "Target");
        public string AckHeaderText => "ACK";
        public string ResultHeaderText => LocalText("결과", "Result");

        public string LocalExchangeRoot
        {
            get => localExchangeRoot;
            set => SetSetting(ref localExchangeRoot, value);
        }

        public string ListenAddress
        {
            get => listenAddress;
            set => SetSetting(ref listenAddress, value);
        }

        public string ListenPortText
        {
            get => listenPortText;
            set => SetSetting(ref listenPortText, value);
        }

        public string PeerHost
        {
            get => peerHost;
            set => SetSetting(ref peerHost, value);
        }

        public string PeerPortText
        {
            get => peerPortText;
            set => SetSetting(ref peerPortText, value);
        }

        public string ValidationMessageText
        {
            get => validationMessageText;
            private set => SetField(ref validationMessageText, value);
        }

        public string KeyStatusText
        {
            get => keyStatusText;
            private set => SetField(ref keyStatusText, value);
        }

        public string SessionStatusText
        {
            get => sessionStatusText;
            private set => SetField(ref sessionStatusText, value);
        }

        public string OperationStatusText
        {
            get => operationStatusText;
            private set => SetField(ref operationStatusText, value);
        }

        public string LastTransferText
        {
            get => lastTransferText;
            private set => SetField(ref lastTransferText, value);
        }

        public string AcknowledgementStatusText
        {
            get => acknowledgementStatusText;
            private set => SetField(ref acknowledgementStatusText, value);
        }

        public string ResultStatusText
        {
            get => resultStatusText;
            private set => SetField(ref resultStatusText, value);
        }

        public string ResultOutcomeText
        {
            get => resultOutcomeText;
            private set => SetField(ref resultOutcomeText, value);
        }

        public string ResultRunIdText
        {
            get => resultRunIdText;
            private set => SetField(ref resultRunIdText, value);
        }

        public string ResultErrorText
        {
            get => resultErrorText;
            private set => SetField(ref resultErrorText, value);
        }

        public OpenVisionTcpIntegrationTransactionRow SelectedTransaction
        {
            get => selectedTransaction;
            set
            {
                if (!SetField(ref selectedTransaction, value))
                {
                    return;
                }

                RefreshSelectedTransactionDetails();
                RefreshCommandState();
            }
        }

        public bool IsListening
        {
            get => isListening;
            private set
            {
                if (SetField(ref isListening, value))
                {
                    OnPropertyChanged(nameof(CanEditSettings));
                    RefreshCommandState();
                }
            }
        }

        public bool IsBusy
        {
            get => isBusy;
            private set
            {
                if (SetField(ref isBusy, value))
                {
                    OnPropertyChanged(nameof(CanEditSettings));
                    RefreshCommandState();
                }
            }
        }

        public bool CanEditSettings => !IsListening && !IsBusy && !isStopping;
        public bool CanSaveSettings => CanEditSettings && isConfigurationValid;
        public bool CanStart => CanEditSettings && isConfigurationValid && isKeyValid;
        public bool CanStop => !isStopping && (IsListening || IsBusy);
        public bool CanUseSession => IsListening && !IsBusy && !isStopping;
        public bool CanUsePeer => CanUseSession && isConfigurationValid && isKeyValid;
        public bool CanUseSelectedTransaction =>
            CanUsePeer && SelectedTransaction != null;
        public bool CanAcknowledge => CanUseSelectedTransaction
            && SelectedTransaction.IsTwoDTarget
            && !SelectedTransaction.HasAcknowledgement;
        public bool CanRun => CanUseSelectedTransaction
            && SelectedTransaction.IsTwoDTarget
            && SelectedTransaction.HasAcknowledgement
            && selectedAcknowledgementAccepted
            && !SelectedTransaction.HasResult;
        internal bool CanCloseWindow => disposed || !IsListening;
        internal bool IsWindowVisibleForTest => window?.IsVisible == true;

        internal void SetSessionSharedKey(string encoded)
        {
            byte[] previous = sessionSharedKey;
            sessionSharedKey = null;
            if (previous != null)
            {
                CryptographicOperations.ZeroMemory(previous);
            }

            string candidate = encoded ?? string.Empty;
            hasSessionSharedKeyInput = !string.IsNullOrWhiteSpace(candidate);
            sessionSharedKeyError = string.Empty;
            if (hasSessionSharedKeyInput)
            {
                try
                {
                    sessionSharedKey = DecodeSharedKey(candidate);
                }
                catch (Exception exception)
                {
                    sessionSharedKeyError = exception.GetBaseException().Message;
                }
            }

            RefreshValidation();
        }

        internal void Show(Window owner)
        {
            ThrowIfDisposed();
            RefreshValidation();
            if (window == null)
            {
                window = new OpenVisionTcpIntegrationWindow(this);
                if (owner != null && owner != window)
                {
                    window.Owner = owner;
                }

                window.Show();
                return;
            }

            if (!window.IsVisible)
            {
                window.Show();
            }

            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = WindowState.Normal;
            }

            window.Activate();
        }

        internal void OnWindowClosed(OpenVisionTcpIntegrationWindow closedWindow)
        {
            if (ReferenceEquals(window, closedWindow))
            {
                window = null;
            }
        }

        internal void ReportStopRequiredBeforeClose()
        {
            OperationStatusText = LocalText(
                "수신 중에는 창을 닫을 수 없습니다. 먼저 '수신 중지'를 선택하십시오.",
                "The window cannot close while listening. Select 'Stop listening' first.");
        }

        internal Task StartAsync()
        {
            string error = string.Empty;
            if (!CanStart || !TryValidateSettings(out ValidatedSettings settings, out error))
            {
                OperationStatusText = string.IsNullOrWhiteSpace(error)
                    ? LocalText("연결 설정과 공유키를 확인하십시오.", "Check the connection settings and shared key.")
                    : error;
                return Task.CompletedTask;
            }

            return RunOperationAsync(LocalText("수신 시작", "Start listening"), async cancellationToken =>
            {
                byte[] key = ReadSharedKey();
                TwoDIntegrationTcpExchange candidate = null;
                try
                {
                    candidate = new TwoDIntegrationTcpExchange(
                        settings.LocalRoot,
                        settings.ListenAddress,
                        settings.ListenPort,
                        key);
                    candidate.RequestCompleted += OnRequestCompleted;
                    await candidate.StartAsync(cancellationToken);
                    exchange = candidate;
                    candidate = null;
                    IsListening = true;
                    SessionStatusText = string.Format(
                        LocalText("수신 중 · {0}:{1}", "Listening · {0}:{1}"),
                        settings.ListenAddress,
                        settings.ListenPort);
                    OperationStatusText = LocalText(
                        "수신을 시작했습니다. 거래 수신만으로 ACK 또는 검사는 실행되지 않습니다.",
                        "Listening started. Receiving a transaction does not create an ACK or run an inspection.");
                    RefreshTransactionsCore(null);
                }
                finally
                {
                    CryptographicOperations.ZeroMemory(key);
                    if (candidate != null)
                    {
                        candidate.RequestCompleted -= OnRequestCompleted;
                        await candidate.DisposeAsync();
                    }
                }
            });
        }

        internal async Task StopAsync()
        {
            if (!CanStop)
            {
                return;
            }

            isStopping = true;
            RefreshCommandState();
            activeOperationSource?.Cancel();
            Task operation = activeOperationTask;
            if (operation != null)
            {
                try
                {
                    await operation;
                }
                catch
                {
                }
            }

            TwoDIntegrationTcpExchange current = exchange;
            exchange = null;
            try
            {
                if (current != null)
                {
                    current.RequestCompleted -= OnRequestCompleted;
                    await current.StopAsync();
                    await current.DisposeAsync();
                }

                IsListening = false;
                SessionStatusText = LocalText("중지됨", "Stopped");
                OperationStatusText = LocalText("TCP 수신을 중지했습니다.", "TCP listening stopped.");
            }
            catch (Exception exception)
            {
                IsListening = false;
                SessionStatusText = LocalText("중지 오류", "Stop error");
                OperationStatusText = exception.GetBaseException().Message;
            }
            finally
            {
                isStopping = false;
                RefreshCommandState();
            }
        }

        internal Task PingAsync() => RunSessionOperationAsync(
            "Ping",
            async (current, transaction, peer, cancellationToken) =>
            {
                TcpIntegrationTransferReceipt receipt = await current.PingPeerAsync(peer, cancellationToken);
                ApplyReceipt(receipt);
                OperationStatusText = "Ping OK · " + receipt.PeerApplicationId;
            },
            requireSelection: false);

        internal Task PushAsync() => RunSessionOperationAsync(
            "Push",
            async (current, transaction, peer, cancellationToken) =>
            {
                TcpIntegrationTransferReceipt receipt = await current.PushTransactionAsync(
                    peer,
                    transaction.TransactionId,
                    cancellationToken);
                ApplyReceipt(receipt);
                OperationStatusText = LocalText("거래 Push 완료", "Transaction push complete");
            },
            requireSelection: true);

        internal Task PullAsync() => RunSessionOperationAsync(
            "Pull",
            async (current, transaction, peer, cancellationToken) =>
            {
                TcpIntegrationTransferReceipt receipt = await current.PullTransactionAsync(
                    peer,
                    transaction.TransactionId,
                    cancellationToken);
                ApplyReceipt(receipt);
                RefreshTransactionsCore(transaction.TransactionId);
                OperationStatusText = LocalText("거래 Pull 완료", "Transaction pull complete");
            },
            requireSelection: true);

        internal Task AcknowledgeAsync() => RunSessionOperationAsync(
            "ACK",
            (current, transaction, peer, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                current.AcknowledgeHandoff(transaction.TransactionId);
                RefreshTransactionsCore(transaction.TransactionId);
                OperationStatusText = LocalText("ACK Accepted를 생성했습니다.", "Created an Accepted ACK.");
                return Task.CompletedTask;
            },
            requireSelection: true);

        internal Task RunInspectionAsync() => RunSessionOperationAsync(
            LocalText("검사 실행", "Run inspection"),
            async (current, transaction, peer, cancellationToken) =>
            {
                IntegrationResultV2 result = await current.RunAcceptedHandoffAsync(
                    transaction.TransactionId,
                    cancellationToken: cancellationToken);
                ApplyResult(result);
                RefreshTransactionsCore(transaction.TransactionId);
                OperationStatusText = LocalText("2D 검사와 결과 기록을 완료했습니다.", "2D inspection and result recording complete.");
            },
            requireSelection: true);

        internal void RefreshTransactions()
        {
            if (!CanUseSession)
            {
                return;
            }

            try
            {
                RefreshTransactionsCore(SelectedTransaction?.TransactionId);
                OperationStatusText = LocalText("거래 목록을 새로 고쳤습니다.", "Transaction list refreshed.");
            }
            catch (Exception exception)
            {
                OperationStatusText = exception.GetBaseException().Message;
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            languageChangeController.Dispose();
            activeOperationSource?.Cancel();
            if (exchange != null)
            {
                exchange.RequestCompleted -= OnRequestCompleted;
                try
                {
                    exchange.DisposeAsync().AsTask().GetAwaiter().GetResult();
                }
                catch
                {
                }

                exchange = null;
            }

            IsListening = false;
            if (sessionSharedKey != null)
            {
                CryptographicOperations.ZeroMemory(sessionSharedKey);
                sessionSharedKey = null;
            }
            hasSessionSharedKeyInput = false;
            OpenVisionTcpIntegrationWindow currentWindow = window;
            window = null;
            currentWindow?.Close();
            activeOperationSource?.Dispose();
            activeOperationSource = null;
        }

        private Task RunSessionOperationAsync(
            string operationName,
            Func<TwoDIntegrationTcpExchange, OpenVisionTcpIntegrationTransactionRow, TcpIntegrationEndpoint, CancellationToken, Task> operation,
            bool requireSelection)
        {
            if (!CanUsePeer || (requireSelection && !CanUseSelectedTransaction))
            {
                return Task.CompletedTask;
            }

            TwoDIntegrationTcpExchange current = exchange;
            OpenVisionTcpIntegrationTransactionRow transaction = SelectedTransaction;
            if (!TryValidateSettings(out ValidatedSettings settings, out string error))
            {
                OperationStatusText = error;
                return Task.CompletedTask;
            }

            return RunOperationAsync(
                operationName,
                cancellationToken => operation(current, transaction, settings.Peer, cancellationToken));
        }

        private async Task RunOperationAsync(string operationName, Func<CancellationToken, Task> operation)
        {
            if (IsBusy || disposed)
            {
                return;
            }

            CancellationTokenSource source = new CancellationTokenSource();
            activeOperationSource = source;
            IsBusy = true;
            OperationStatusText = operationName + "...";
            Task task = Task.CompletedTask;
            try
            {
                task = operation(source.Token);
                activeOperationTask = task;
                await task;
            }
            catch (OperationCanceledException)
            {
                OperationStatusText = LocalText("작업을 취소했습니다.", "Operation cancelled.");
            }
            catch (Exception exception)
            {
                OperationStatusText = exception.GetBaseException().Message;
            }
            finally
            {
                if (ReferenceEquals(activeOperationSource, source))
                {
                    activeOperationSource = null;
                }

                if (ReferenceEquals(activeOperationTask, task))
                {
                    activeOperationTask = null;
                }

                source.Dispose();
                IsBusy = false;
            }
        }

        private void SaveSettings()
        {
            if (!TryValidateSettings(out ValidatedSettings settings, out string error))
            {
                OperationStatusText = error;
                return;
            }

            try
            {
                OpenVisionTcpIntegrationSettings saved = new OpenVisionTcpIntegrationSettings
                {
                    SchemaVersion = OpenVisionTcpIntegrationSettings.CurrentSchemaVersion,
                    LocalExchangeRoot = settings.LocalRoot,
                    ListenAddress = settings.ListenAddress.ToString(),
                    ListenPort = settings.ListenPort,
                    PeerHost = settings.Peer.Host,
                    PeerPort = settings.Peer.Port
                };
                OpenVisionTcpIntegrationSettingsStore.Save(saved);
                ApplySettings(saved);
                OperationStatusText = LocalText(
                    "설정을 저장했습니다. 공유키와 수신 상태는 저장하지 않았습니다.",
                    "Settings saved. The shared key and listening state were not saved.");
            }
            catch (Exception exception)
            {
                OperationStatusText = exception.GetBaseException().Message;
            }
        }

        private void ResetSettings()
        {
            try
            {
                OpenVisionTcpIntegrationSettings defaults = OpenVisionTcpIntegrationSettings.CreateDefault();
                OpenVisionTcpIntegrationSettingsStore.Save(defaults);
                ApplySettings(defaults);
                Transactions.Clear();
                SelectedTransaction = null;
                RefreshValidation();
                OperationStatusText = LocalText(
                    "TCP 설정을 기본값으로 되돌렸습니다. 수신은 시작되지 않았습니다.",
                    "TCP settings reset to defaults. Listening was not started.");
            }
            catch (Exception exception)
            {
                OperationStatusText = exception.GetBaseException().Message;
            }
        }

        private void ApplySettings(OpenVisionTcpIntegrationSettings settings)
        {
            localExchangeRoot = settings.LocalExchangeRoot ?? string.Empty;
            listenAddress = settings.ListenAddress ?? string.Empty;
            listenPortText = settings.ListenPort.ToString(System.Globalization.CultureInfo.InvariantCulture);
            peerHost = settings.PeerHost ?? string.Empty;
            peerPortText = settings.PeerPort.ToString(System.Globalization.CultureInfo.InvariantCulture);
            OnPropertyChanged(nameof(LocalExchangeRoot));
            OnPropertyChanged(nameof(ListenAddress));
            OnPropertyChanged(nameof(ListenPortText));
            OnPropertyChanged(nameof(PeerHost));
            OnPropertyChanged(nameof(PeerPortText));
        }

        private void RefreshValidation()
        {
            isConfigurationValid = TryValidateSettings(out _, out string configurationError);
            ValidationMessageText = configurationError;
            byte[] key = null;
            try
            {
                key = ReadSharedKey();
                isKeyValid = true;
                KeyStatusText = hasSessionSharedKeyInput
                    ? LocalText("세션 공유키 준비됨", "Session shared key ready")
                    : LocalText("환경변수 공유키 준비됨", "Environment shared key ready");
            }
            catch (Exception exception)
            {
                isKeyValid = false;
                KeyStatusText = exception.GetBaseException().Message;
            }
            finally
            {
                if (key != null)
                {
                    CryptographicOperations.ZeroMemory(key);
                }
            }

            OnPropertyChanged(nameof(CanSaveSettings));
            OnPropertyChanged(nameof(CanStart));
            RefreshCommandState();
        }

        private bool TryValidateSettings(out ValidatedSettings settings, out string error)
        {
            settings = null;
            error = string.Empty;
            string root;
            try
            {
                string candidate = (LocalExchangeRoot ?? string.Empty).Trim().Trim('"');
                if (!Path.IsPathFullyQualified(candidate) || candidate.StartsWith("\\\\", StringComparison.Ordinal))
                {
                    error = LocalText("로컬 교환 Root는 UNC가 아닌 절대 로컬 경로여야 합니다.", "Local exchange root must be an absolute local path, not UNC.");
                    return false;
                }

                root = Path.GetFullPath(candidate);
            }
            catch (Exception exception)
            {
                error = exception.GetBaseException().Message;
                return false;
            }

            if (!IPAddress.TryParse((ListenAddress ?? string.Empty).Trim(), out IPAddress ipAddress))
            {
                error = LocalText("수신 IP 주소가 올바르지 않습니다.", "Listen IP address is invalid.");
                return false;
            }

            if (!TryParsePort(ListenPortText, out int listenPort))
            {
                error = LocalText("수신 Port는 1~65535여야 합니다.", "Listen port must be between 1 and 65535.");
                return false;
            }

            string host = (PeerHost ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(host))
            {
                error = LocalText("상대 Host가 필요합니다.", "Peer host is required.");
                return false;
            }

            if (!TryParsePort(PeerPortText, out int peerPort))
            {
                error = LocalText("상대 Port는 1~65535여야 합니다.", "Peer port must be between 1 and 65535.");
                return false;
            }

            settings = new ValidatedSettings(root, ipAddress, listenPort, new TcpIntegrationEndpoint(host, peerPort));
            return true;
        }

        private static bool TryParsePort(string text, out int port) =>
            int.TryParse(
                text,
                System.Globalization.NumberStyles.None,
                System.Globalization.CultureInfo.InvariantCulture,
                out port)
            && port >= 1
            && port <= 65535;

        private byte[] ReadSharedKey()
        {
            if (hasSessionSharedKeyInput)
            {
                if (sessionSharedKey == null)
                {
                    throw new InvalidOperationException(string.IsNullOrWhiteSpace(sessionSharedKeyError)
                        ? "The session shared key is invalid."
                        : sessionSharedKeyError);
                }

                return sessionSharedKey.ToArray();
            }

            string encoded = Environment.GetEnvironmentVariable(SharedKeyEnvironmentVariable)
                ?? Environment.GetEnvironmentVariable(SharedKeyEnvironmentVariable, EnvironmentVariableTarget.User)
                ?? Environment.GetEnvironmentVariable(SharedKeyEnvironmentVariable, EnvironmentVariableTarget.Machine);
            if (string.IsNullOrWhiteSpace(encoded))
            {
                throw new InvalidOperationException(
                    SharedKeyEnvironmentVariable + " is required (Base64, 32+ bytes)." );
            }

            return DecodeSharedKey(encoded);
        }

        private static byte[] DecodeSharedKey(string encoded)
        {
            try
            {
                byte[] key = Convert.FromBase64String((encoded ?? string.Empty).Trim());
                if (key.Length < 32)
                {
                    CryptographicOperations.ZeroMemory(key);
                    throw new FormatException("The decoded shared key must contain at least 32 bytes.");
                }

                return key;
            }
            catch (FormatException exception)
            {
                throw new InvalidOperationException(
                    "The shared key must be Base64 and decode to at least 32 bytes.",
                    exception);
            }
        }

        private void RefreshTransactionsCore(Guid? transactionToSelect)
        {
            if (exchange == null)
            {
                return;
            }

            Guid? selectedId = transactionToSelect ?? SelectedTransaction?.TransactionId;
            IReadOnlyList<TwoDIntegrationTransactionSummary> summaries = exchange.DiscoverHandoffs();
            Transactions.Clear();
            foreach (TwoDIntegrationTransactionSummary summary in summaries)
            {
                Transactions.Add(new OpenVisionTcpIntegrationTransactionRow(summary));
            }

            SelectedTransaction = selectedId.HasValue
                ? Transactions.FirstOrDefault(item => item.TransactionId == selectedId.Value)
                : Transactions.FirstOrDefault();
        }

        private void RefreshSelectedTransactionDetails()
        {
            selectedAcknowledgementAccepted = false;
            AcknowledgementStatusText = "-";
            ResultStatusText = "-";
            ResultOutcomeText = "-";
            ResultRunIdText = "-";
            ResultErrorText = "-";
            if (SelectedTransaction == null || exchange == null)
            {
                return;
            }

            try
            {
                if (SelectedTransaction.HasAcknowledgement)
                {
                    IntegrationAcknowledgementV2 acknowledgement =
                        exchange.ReadAcknowledgement(SelectedTransaction.TransactionId);
                    AcknowledgementStatusText = acknowledgement.Status.ToString();
                    selectedAcknowledgementAccepted =
                        acknowledgement.Status == IntegrationAcknowledgementStatus.Accepted;
                }

                if (SelectedTransaction.HasResult)
                {
                    ApplyResult(exchange.ReadResult(SelectedTransaction.TransactionId));
                }
            }
            catch (Exception exception)
            {
                ResultErrorText = exception.GetBaseException().Message;
            }
        }

        private void ApplyResult(IntegrationResultV2 result)
        {
            ResultStatusText = result.Status.ToString();
            ResultOutcomeText = result.Outcome.ToString();
            ResultRunIdText = string.IsNullOrWhiteSpace(result.RunId) ? "-" : result.RunId;
            ResultErrorText = result.Error == null
                ? "-"
                : result.Error.Code + " · " + result.Error.Message;
        }

        private void ApplyReceipt(TcpIntegrationTransferReceipt receipt)
        {
            LastTransferText = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0} · Peer={1} · Files={2} · Bytes={3} · Idempotent={4}",
                receipt.Operation,
                receipt.PeerApplicationId,
                receipt.FilesTransferred,
                receipt.BytesTransferred,
                receipt.Idempotent);
        }

        private void OnRequestCompleted(TcpIntegrationTransferReceipt receipt)
        {
            dispatcher.BeginInvoke(new Action(() =>
            {
                if (disposed || exchange == null)
                {
                    return;
                }

                ApplyReceipt(receipt);
                try
                {
                    RefreshTransactionsCore(receipt.TransactionId);
                    OperationStatusText = LocalText(
                        "TCP 요청을 수신했습니다. 검토 후 ACK와 검사를 명시적으로 실행하십시오.",
                        "TCP request received. Review it, then explicitly ACK and run inspection.");
                }
                catch (Exception exception)
                {
                    OperationStatusText = exception.GetBaseException().Message;
                }
            }));
        }

        private void SetSetting(ref string field, string value, [CallerMemberName] string propertyName = null)
        {
            if (!SetField(ref field, value ?? string.Empty, propertyName))
            {
                return;
            }

            if (!IsListening)
            {
                Transactions.Clear();
                SelectedTransaction = null;
            }

            RefreshValidation();
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void RefreshCommandState()
        {
            saveSettingsCommand?.RaiseCanExecuteChanged();
            resetSettingsCommand?.RaiseCanExecuteChanged();
            reloadKeyCommand?.RaiseCanExecuteChanged();
            startCommand?.RaiseCanExecuteChanged();
            stopCommand?.RaiseCanExecuteChanged();
            pingCommand?.RaiseCanExecuteChanged();
            refreshCommand?.RaiseCanExecuteChanged();
            acknowledgeCommand?.RaiseCanExecuteChanged();
            runCommand?.RaiseCanExecuteChanged();
            pushCommand?.RaiseCanExecuteChanged();
            pullCommand?.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(CanEditSettings));
            OnPropertyChanged(nameof(CanSaveSettings));
            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanStop));
            OnPropertyChanged(nameof(CanUseSession));
            OnPropertyChanged(nameof(CanUsePeer));
            OnPropertyChanged(nameof(CanUseSelectedTransaction));
            OnPropertyChanged(nameof(CanAcknowledge));
            OnPropertyChanged(nameof(CanRun));
        }

        private void RefreshLocalization()
        {
            OnPropertyChanged(string.Empty);
            RefreshValidation();
            if (!IsListening)
            {
                SessionStatusText = LocalText("중지됨", "Stopped");
            }
        }

        private static string LocalText(string korean, string english) =>
            OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? korean : english;

        private async void Start() => await StartAsync();
        private async void Stop() => await StopAsync();
        private async void Ping() => await PingAsync();
        private async void Acknowledge() => await AcknowledgeAsync();
        private async void Run() => await RunInspectionAsync();
        private async void Push() => await PushAsync();
        private async void Pull() => await PullAsync();

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(OpenVisionTcpIntegrationController));
            }
        }

        private sealed record ValidatedSettings(
            string LocalRoot,
            IPAddress ListenAddress,
            int ListenPort,
            TcpIntegrationEndpoint Peer);
    }

    internal sealed class OpenVisionTcpIntegrationTransactionRow
    {
        public OpenVisionTcpIntegrationTransactionRow(TwoDIntegrationTransactionSummary summary)
        {
            ArgumentNullException.ThrowIfNull(summary);
            TransactionId = summary.Handoff.TransactionId;
            CreatedAtText = summary.Handoff.CreatedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            ProducerText = summary.Handoff.Producer.ApplicationId;
            IntegrationApplicationIdentity target = summary.Handoff.Context.ConsumerBuild;
            string shortCommit = target.SourceCommit.Length > 8
                ? target.SourceCommit.Substring(0, 8)
                : target.SourceCommit;
            TargetText = $"{target.ApplicationId} {target.ApplicationVersion} @{shortCommit} / {target.SourceState}";
            HasAcknowledgement = summary.HasAcknowledgement;
            HasResult = summary.HasResult;
            IsTwoDTarget = summary.Handoff.Context.Modality == IntegrationInspectionModality.TwoD
                && summary.Handoff.Context.InputKind == IntegrationInspectionInputKind.Image
                && string.Equals(
                    summary.Handoff.Context.ConsumerBuild.ApplicationId,
                    IntegrationApplicationIds.TwoDStudio,
                    StringComparison.Ordinal);
        }

        public Guid TransactionId { get; }
        public string TransactionIdText => TransactionId.ToString("D");
        public string CreatedAtText { get; }
        public string ProducerText { get; }
        public string TargetText { get; }
        public bool HasAcknowledgement { get; }
        public bool HasResult { get; }
        public bool IsTwoDTarget { get; }
        public string AcknowledgementText => HasAcknowledgement ? "Yes" : "No";
        public string ResultText => HasResult ? "Yes" : "No";
    }

    internal sealed class OpenVisionTcpIntegrationSettings
    {
        public const int CurrentSchemaVersion = 1;

        public int SchemaVersion { get; set; } = CurrentSchemaVersion;
        public string LocalExchangeRoot { get; set; } = string.Empty;
        public string ListenAddress { get; set; } = "127.0.0.1";
        public int ListenPort { get; set; } = 45102;
        public string PeerHost { get; set; } = "127.0.0.1";
        public int PeerPort { get; set; } = 45103;

        public static OpenVisionTcpIntegrationSettings CreateDefault() => new OpenVisionTcpIntegrationSettings
        {
            LocalExchangeRoot = Path.Combine(AppPathService.DataRootDirectory, "INTEGRATION", "2D")
        };
    }

    internal static class OpenVisionTcpIntegrationSettingsStore
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        internal static string SettingsPath => Path.Combine(
            AppPathService.DataRootDirectory,
            "CONFIG",
            "UI",
            "TcpIntegration2D.json");

        public static OpenVisionTcpIntegrationSettings Load(out string warning)
        {
            warning = string.Empty;
            if (!File.Exists(SettingsPath))
            {
                return OpenVisionTcpIntegrationSettings.CreateDefault();
            }

            try
            {
                OpenVisionTcpIntegrationSettings settings = JsonSerializer.Deserialize<OpenVisionTcpIntegrationSettings>(
                    File.ReadAllText(SettingsPath),
                    JsonOptions);
                if (settings == null
                    || settings.SchemaVersion != OpenVisionTcpIntegrationSettings.CurrentSchemaVersion)
                {
                    warning = "Saved TCP settings are missing or incompatible; defaults were restored without starting listening.";
                    return OpenVisionTcpIntegrationSettings.CreateDefault();
                }

                return settings;
            }
            catch (Exception exception)
            {
                warning = "Saved TCP settings could not be read; defaults were restored without starting listening. "
                    + exception.GetBaseException().Message;
                return OpenVisionTcpIntegrationSettings.CreateDefault();
            }
        }

        public static void Save(OpenVisionTcpIntegrationSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            string directory = Path.GetDirectoryName(SettingsPath);
            Directory.CreateDirectory(directory);
            string temporaryPath = Path.Combine(
                directory,
                ".TcpIntegration2D." + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                File.WriteAllText(temporaryPath, JsonSerializer.Serialize(settings, JsonOptions));
                File.Move(temporaryPath, SettingsPath, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }
    }
}
