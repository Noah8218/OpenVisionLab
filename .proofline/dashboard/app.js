const state = {
  issues: [],
  directoryHandle: null,
  directorySignature: '',
  isLoading: false,
  filters: {
    text: '',
    status: 'open',
    risk: 'all'
  },
  sort: 'priority-asc',
  expandedIssueIds: new Set(),
  expandedEvidenceIds: new Set(),
  collapsedStatuses: new Set()
};

const elements = {
  themeToggle: document.getElementById('theme-toggle'),
  accentPicker: document.getElementById('accent-picker'),
  backgroundMode: document.getElementById('background-mode'),
  backgroundImagePicker: document.getElementById('background-image-picker'),
  backgroundImageInput: document.getElementById('background-image-input'),
  tuningPanel: document.getElementById('tuning-panel'),
  folderInput: document.getElementById('folder-input'),
  folderPicker: document.getElementById('folder-picker'),
  connectButton: document.getElementById('connect-button'),
  changeFolderButton: document.getElementById('change-folder-button'),
  folderStatus: document.getElementById('folder-status'),
  reloadButton: document.getElementById('reload-button'),
  loadStatus: document.getElementById('load-status'),
  autoRefreshStatus: document.getElementById('auto-refresh-status'),
  searchInput: document.getElementById('search-input'),
  statusFilter: document.getElementById('status-filter'),
  riskFilter: document.getElementById('risk-filter'),
  sortOrder: document.getElementById('sort-order'),
  issuesList: document.getElementById('issues-list'),
  issueGroupTemplate: document.getElementById('issue-group-template'),
  issueTemplate: document.getElementById('issue-template'),
  summaryTotal: document.getElementById('summary-total'),
  summaryOpen: document.getElementById('summary-open'),
  summaryBlocked: document.getElementById('summary-blocked'),
  summaryResolved: document.getElementById('summary-resolved'),
  summaryButtons: Array.from(document.querySelectorAll('[data-status-filter]'))
};

// 저장값은 파일 호환성을 위해 유지하고, 화면에서만 한국어로 번역합니다.
const statusLabels = {
  open: '대기',
  doing: '작업 중',
  blocked: '보류',
  resolved: '완료',
  ignored: '제외'
};

const riskLabels = {
  critical: '치명적',
  high: '높음',
  medium: '중간',
  low: '낮음'
};

const themeStorageKey = 'proofline-dashboard-theme';
const accentStorageKey = 'proofline-dashboard-accent';

const riskOrder = {
  critical: 0,
  high: 1,
  medium: 2,
  low: 3
};

const statusOrder = {
  doing: 0,
  blocked: 1,
  open: 2,
  ignored: 3,
  resolved: 4
};

// 활성 보기는 아직 조치가 끝나지 않은 작업 중 이슈와 대기 이슈를 함께 보여줍니다.
const openStatuses = new Set(['doing', 'open']);
const autoRefreshIntervalMs = 30_000;

const directoryStore = {
  dbName: 'proofline-dashboard',
  storeName: 'directory-handles',
  key: getDirectoryPickerId()
};
const backgroundModeStorageKey = `${directoryStore.key}-background-mode`;
const backgroundImageStoreKey = `${directoryStore.key}-background-image`;
let backgroundImageUrl = '';

elements.folderInput.addEventListener('change', async (event) => {
  const files = Array.from(event.target.files || [])
    .filter((file) => isIssueFileName(file.name));

  await loadMarkdownFiles(files);
});

elements.connectButton.addEventListener('click', connectIssuesDirectory);
elements.changeFolderButton.addEventListener('click', changeIssuesDirectory);
elements.themeToggle.addEventListener('click', toggleTheme);
elements.accentPicker.addEventListener('input', (event) => {
  applyAccentColor(event.target.value);

  try {
    localStorage.setItem(accentStorageKey, event.target.value);
  } catch {
    // 로컬 저장소가 막혀도 현재 화면의 사용자 색상은 유지합니다.
  }
});

// 이미지 모드는 저장된 사진을 재사용하고, 처음 선택할 때만 파일 창을 엽니다.
elements.backgroundMode.addEventListener('change', async (event) => {
  clearBackgroundError();

  if (event.target.value === 'color') {
    applyColorBackground();
    return;
  }

  await selectImageBackground();
});

// 선택한 파일은 현재 대시보드 주소에만 연결해 다른 원장의 배경과 섞이지 않게 합니다.
elements.backgroundImageInput.addEventListener('change', async (event) => {
  const [file] = Array.from(event.target.files || []);

  if (file) {
    await saveBackgroundImage(file);
  }

  event.target.value = '';
});

// 조정값은 테스트 중인 탭에만 적용하고 저장하지 않아 최종 설정과 섞이지 않게 합니다.
elements.tuningPanel.addEventListener('input', (event) => {
  const input = event.target;
  document.documentElement.style.setProperty(
    input.dataset.tuningVariable,
    `${input.value}${input.dataset.unit || ''}`
  );
  input.closest('label').querySelector('output').value = input.value;
});

for (const button of elements.summaryButtons) {
  button.addEventListener('click', () => {
    state.filters.status = button.dataset.statusFilter;
    elements.statusFilter.value = state.filters.status;
    render();
  });
}

elements.reloadButton.addEventListener('click', () => {
  loadFromDefaultSources();
});

elements.searchInput.addEventListener('input', (event) => {
  state.filters.text = event.target.value.trim().toLowerCase();
  render();
});

elements.statusFilter.addEventListener('change', (event) => {
  state.filters.status = event.target.value;
  render();
});

elements.riskFilter.addEventListener('change', (event) => {
  state.filters.risk = event.target.value;
  render();
});

elements.sortOrder.addEventListener('change', (event) => {
  state.sort = event.target.value;
  render();
});

// 숨겨진 탭에서는 파일 접근을 멈추고, 돌아온 순간 한 번 확인합니다.
document.addEventListener('visibilitychange', () => {
  if (document.visibilityState === 'visible') {
    checkForIssueChanges();
  }
});

window.setInterval(checkForIssueChanges, autoRefreshIntervalMs);

function toggleTheme() {
  const nextTheme = document.documentElement.dataset.theme === 'dark' ? 'light' : 'dark';
  document.documentElement.dataset.theme = nextTheme;

  try {
    localStorage.setItem(themeStorageKey, nextTheme);
  } catch {
    // 로컬 저장소를 막은 브라우저에서도 현재 화면의 테마 전환은 유지합니다.
  }

  updateThemeControl();
}

function updateThemeControl() {
  const isDark = document.documentElement.dataset.theme === 'dark';
  // 아이콘 버튼의 툴팁과 접근성 이름이 현재 전환 동작을 함께 설명하게 합니다.
  const actionLabel = isDark ? '라이트 모드로 전환' : '다크 모드로 전환';
  elements.themeToggle.textContent = isDark ? '라이트 모드' : '다크 모드';
  elements.themeToggle.setAttribute('aria-label', actionLabel);
  elements.themeToggle.title = actionLabel;
  elements.accentPicker.value = document.documentElement.style.getPropertyValue('--accent') || (isDark ? '#fb923c' : '#c2410c');
}

function loadAccentColor() {
  try {
    applyAccentColor(localStorage.getItem(accentStorageKey));
  } catch {
    // 저장값을 읽지 못하면 테마별 기본 강조색을 사용합니다.
  }
}

function applyAccentColor(color) {
  if (!/^#[0-9a-f]{6}$/i.test(color || '')) {
    return;
  }

  document.documentElement.style.setProperty('--accent', color);
  elements.accentPicker.value = color;
}

// 저장 모드와 이미지가 모두 유효할 때만 이미지 배경을 복원합니다.
async function loadBackground() {
  let mode = 'color';

  try {
    mode = localStorage.getItem(backgroundModeStorageKey) || 'color';
  } catch {
    // 저장소를 읽지 못하면 안정적인 기본 컬러 배경을 사용합니다.
  }

  if (mode !== 'image') {
    applyColorBackground(false);
    return;
  }

  try {
    const image = await getStoredBackgroundImage();

    if (image instanceof Blob) {
      applyImageBackground(image, false);
      return;
    }
  } catch {
    // 손상되거나 접근할 수 없는 저장값은 컬러 배경으로 되돌립니다.
  }

  applyColorBackground(false);
}

// 저장된 이미지가 없으면 네이티브 파일 선택기를 통해 최초 이미지를 받습니다.
async function selectImageBackground() {
  try {
    const image = await getStoredBackgroundImage();

    if (image instanceof Blob) {
      applyImageBackground(image);
      return;
    }
  } catch {
    applyColorBackground(false);
    showBackgroundError('저장된 배경 이미지를 읽지 못했습니다.');
    return;
  }

  elements.backgroundMode.value = 'color';
  elements.backgroundImageInput.click();
}

// 브라우저가 이미지로 식별한 파일만 기존 IndexedDB에 보관합니다.
async function saveBackgroundImage(file) {
  clearBackgroundError();

  if (!file.type.startsWith('image/')) {
    showBackgroundError('이미지 파일만 선택할 수 있습니다.');
    elements.backgroundMode.value = backgroundImageUrl ? 'image' : 'color';
    return;
  }

  let db;

  try {
    db = await openDirectoryDb();
    await putInStore(db, backgroundImageStoreKey, file);
    applyImageBackground(file);
  } catch {
    showBackgroundError('배경 이미지를 저장하지 못했습니다.');
  } finally {
    db?.close();
  }
}

// 폴더 핸들과 같은 대시보드 전용 저장소에서 이미지 Blob을 읽습니다.
async function getStoredBackgroundImage() {
  if (!('indexedDB' in window)) {
    throw new Error('IndexedDB is unavailable.');
  }

  const db = await openDirectoryDb();

  try {
    return await getFromStore(db, backgroundImageStoreKey);
  } finally {
    db.close();
  }
}

// Object URL은 현재 화면에서만 유지하고 교체 시 즉시 해제합니다.
function applyImageBackground(image, persist = true) {
  if (backgroundImageUrl) {
    URL.revokeObjectURL(backgroundImageUrl);
  }

  backgroundImageUrl = URL.createObjectURL(image);
  document.documentElement.dataset.background = 'image';
  document.documentElement.style.setProperty('--background-image', `url("${backgroundImageUrl}")`);
  elements.backgroundMode.value = 'image';
  elements.backgroundImagePicker.hidden = false;

  if (persist) {
    try {
      localStorage.setItem(backgroundModeStorageKey, 'image');
    } catch {
      // 저장이 막혀도 현재 화면의 이미지 배경은 유지합니다.
    }
  }
}

// 컬러 모드로 돌아가도 저장된 사진은 남겨 다음 전환 때 다시 사용합니다.
function applyColorBackground(persist = true) {
  if (backgroundImageUrl) {
    URL.revokeObjectURL(backgroundImageUrl);
  }

  backgroundImageUrl = '';
  document.documentElement.dataset.background = 'color';
  document.documentElement.style.removeProperty('--background-image');
  elements.backgroundMode.value = 'color';
  elements.backgroundImagePicker.hidden = true;

  if (persist) {
    try {
      localStorage.setItem(backgroundModeStorageKey, 'color');
    } catch {
      // 저장이 막혀도 현재 화면의 컬러 배경은 유지합니다.
    }
  }
}

// 네이티브 유효성 UI를 재사용해 별도 알림 컴포넌트를 만들지 않습니다.
function showBackgroundError(message) {
  elements.backgroundMode.setCustomValidity(message);
  elements.backgroundMode.reportValidity();
}

function clearBackgroundError() {
  elements.backgroundMode.setCustomValidity('');
}

async function loadMarkdownFiles(files) {
  if (files.length === 0) {
    state.issues = [];
    setFolderStatus('선택한 폴더에서 Markdown 이슈 파일을 찾지 못했습니다.', 'warning');
    render();
    return;
  }

  const loadedIssues = [];
  const errors = [];

  for (const file of files) {
    try {
      const content = await file.text();
      loadedIssues.push(parseIssueMarkdown(content, file.name));
    } catch (error) {
      errors.push(`${file.name}: ${error.message}`);
    }
  }

  state.issues = loadedIssues;
  setFolderStatus(buildLoadMessage(loadedIssues.length, errors), errors.length ? 'warning' : 'ready');
  render();
}

async function loadFromDefaultSources() {
  showLoadingState('저장된 이슈 폴더에 자동 연결하는 중입니다.');

  const directoryHandle = await getStoredDirectoryHandle();
  if (!directoryHandle) {
    showInitialSetupRequired('처음 한 번 .proofline/issues 폴더를 설정하세요.');
    return;
  }

  if (!(await hasDirectoryPermission(directoryHandle, true))) {
    showPermissionRequired('저장된 폴더가 있지만 브라우저가 자동 연결을 막았습니다. 권한을 확인하세요.');
    return;
  }

  await loadFromDirectoryHandle(directoryHandle);
}

async function connectIssuesDirectory() {
  if (!supportsFileSystemAccess()) {
    showFallbackPicker('이 브라우저는 폴더 권한 저장을 지원하지 않습니다. 대신 이슈 폴더 선택을 사용하세요.');
    return;
  }

  let directoryHandle = await getStoredDirectoryHandle();

  if (directoryHandle && await hasDirectoryPermission(directoryHandle, true)) {
    await loadFromDirectoryHandle(directoryHandle);
    return;
  }

  try {
    directoryHandle = await openIssuesDirectoryPicker(directoryHandle);

    await saveDirectoryHandle(directoryHandle);
    await loadFromDirectoryHandle(directoryHandle);
  } catch (error) {
    if (error.name === 'AbortError') {
      setFolderStatus('폴더 연결이 취소되었습니다.', 'warning');
      return;
    }

    setFolderStatus(`이슈 폴더를 연결하지 못했습니다. (${error.message})`, 'error');
  }
}

async function changeIssuesDirectory() {
  if (!supportsFileSystemAccess()) {
    showFallbackPicker('이 브라우저는 폴더 재지정을 저장할 수 없습니다. 대신 이슈 폴더 선택을 사용하세요.');
    return;
  }

  const currentHandle = await getStoredDirectoryHandle();

  try {
    const nextHandle = await openIssuesDirectoryPicker(currentHandle);
    await saveDirectoryHandle(nextHandle);
    await loadFromDirectoryHandle(nextHandle);
  } catch (error) {
    if (error.name === 'AbortError') {
      setFolderStatus('폴더 재지정이 취소되었습니다.', 'warning');
      return;
    }

    setFolderStatus(`이슈 폴더를 재지정하지 못했습니다. (${error.message})`, 'error');
  }
}

async function openIssuesDirectoryPicker(startHandle) {
  const options = {
    id: getDirectoryPickerId(),
    mode: 'read'
  };

  if (startHandle) {
    options.startIn = startHandle;
  }

  try {
    return await window.showDirectoryPicker(options);
  } catch (error) {
    if (!startHandle || error.name === 'AbortError') {
      throw error;
    }

    return window.showDirectoryPicker({
      id: options.id,
      mode: options.mode
    });
  }
}

function getDirectoryPickerId() {
  return `proofline-issues-${hashString(window.location.href)}`;
}

function hashString(value) {
  let hash = 0;

  for (let index = 0; index < value.length; index += 1) {
    hash = ((hash << 5) - hash + value.charCodeAt(index)) | 0;
  }

  return Math.abs(hash).toString(36);
}

async function loadFromDirectoryHandle(directoryHandle, silent = false) {
  if (state.isLoading) {
    return;
  }

  state.isLoading = true;
  if (!silent) {
    showLoadingState(`${getDirectoryLabel(directoryHandle)} 폴더를 읽는 중입니다.`);
  }

  const loadedIssues = [];
  const errors = [];
  const signatureParts = [];

  try {
    for await (const [fileName, handle] of directoryHandle.entries()) {
      if (handle.kind !== 'file' || !isIssueFileName(fileName)) {
        continue;
      }

      try {
        const file = await handle.getFile();
        const content = await file.text();
        signatureParts.push(`${fileName}:${file.lastModified}:${file.size}`);
        loadedIssues.push(parseIssueMarkdown(content, fileName));
      } catch (error) {
        errors.push(`${fileName}: ${error.message}`);
      }
    }
  } catch (error) {
    showPermissionRequired(`저장된 이슈 폴더를 읽지 못했습니다. 권한을 확인하세요. (${error.message})`);
    state.isLoading = false;
    return;
  }

  state.issues = loadedIssues;
  state.directoryHandle = directoryHandle;
  state.directorySignature = signatureParts.sort().join('|');
  state.isLoading = false;
  showConfiguredState(buildLoadMessage(loadedIssues.length, errors, getDirectoryLabel(directoryHandle)), errors.length ? 'warning' : 'ready');
  setAutoRefreshStatus('변경 자동 확인 켜짐 (30초)');
  render();
}

async function checkForIssueChanges() {
  if (!state.directoryHandle || state.isLoading || document.visibilityState !== 'visible') {
    return;
  }

  try {
    const nextSignature = await getDirectorySignature(state.directoryHandle);
    if (nextSignature !== state.directorySignature) {
      await loadFromDirectoryHandle(state.directoryHandle, true);
    }
  } catch {
    // 자동 확인 실패는 기존 목록을 지우지 않고 수동 다시 읽기로 복구할 수 있게 둡니다.
    setAutoRefreshStatus('자동 확인 실패. 다시 읽기를 사용하세요.');
  }
}

async function getDirectorySignature(directoryHandle) {
  const signatureParts = [];

  for await (const [fileName, handle] of directoryHandle.entries()) {
    if (handle.kind === 'file' && isIssueFileName(fileName)) {
      const file = await handle.getFile();
      signatureParts.push(`${fileName}:${file.lastModified}:${file.size}`);
    }
  }

  return signatureParts.sort().join('|');
}

function showInitialSetupRequired(message) {
  state.issues = [];
  elements.reloadButton.disabled = true;
  elements.connectButton.hidden = false;
  elements.connectButton.textContent = '초기 폴더 설정';
  elements.changeFolderButton.hidden = true;
  elements.folderPicker.hidden = true;
  setFolderStatus(message, 'needed');
  setAutoRefreshStatus('폴더 연결 후 변경을 자동 확인합니다.');
  render();
}

function showPermissionRequired(message) {
  state.issues = [];
  elements.reloadButton.disabled = false;
  elements.connectButton.hidden = false;
  elements.connectButton.textContent = '권한 확인';
  elements.changeFolderButton.hidden = false;
  elements.folderPicker.hidden = true;
  setFolderStatus(message, 'warning');
  setAutoRefreshStatus('권한 확인 후 자동 확인을 다시 시작합니다.');
  render();
}

function showConfiguredState(message, status) {
  elements.reloadButton.disabled = false;
  elements.connectButton.hidden = true;
  elements.connectButton.textContent = '초기 폴더 설정';
  elements.changeFolderButton.hidden = false;
  elements.folderPicker.hidden = true;
  setFolderStatus(message, status);
}

function showLoadingState(message) {
  elements.reloadButton.disabled = true;
  elements.folderPicker.hidden = true;
  setFolderStatus(message, 'loading');
}

function showFallbackPicker(message) {
  state.issues = [];
  elements.reloadButton.disabled = true;
  elements.connectButton.hidden = true;
  elements.changeFolderButton.hidden = true;
  elements.folderPicker.hidden = false;
  setFolderStatus(message, 'warning');
  setAutoRefreshStatus('이 브라우저에서는 수동 다시 읽기만 지원합니다.');
  render();
}

function setFolderStatus(message, status) {
  elements.folderStatus.dataset.status = status;
  elements.loadStatus.textContent = message;
}

function setAutoRefreshStatus(message) {
  elements.autoRefreshStatus.textContent = message;
}

function supportsFileSystemAccess() {
  return 'showDirectoryPicker' in window && 'indexedDB' in window;
}

async function hasDirectoryPermission(directoryHandle, requestPermission) {
  const options = { mode: 'read' };

  if ((await directoryHandle.queryPermission(options)) === 'granted') {
    return true;
  }

  if (!requestPermission) {
    return false;
  }

  try {
    return (await directoryHandle.requestPermission(options)) === 'granted';
  } catch {
    return false;
  }
}

async function getStoredDirectoryHandle() {
  if (!supportsFileSystemAccess()) {
    return null;
  }

  try {
    const db = await openDirectoryDb();
    const handle = await getFromStore(db, directoryStore.key);
    db.close();
    return handle || null;
  } catch {
    return null;
  }
}

async function saveDirectoryHandle(directoryHandle) {
  const db = await openDirectoryDb();
  await putInStore(db, directoryStore.key, directoryHandle);
  db.close();
}

function openDirectoryDb() {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open(directoryStore.dbName, 1);

    request.onupgradeneeded = () => {
      request.result.createObjectStore(directoryStore.storeName);
    };

    request.onsuccess = () => resolve(request.result);
    request.onerror = () => reject(request.error);
  });
}

function getFromStore(db, key) {
  return new Promise((resolve, reject) => {
    const transaction = db.transaction(directoryStore.storeName, 'readonly');
    const request = transaction.objectStore(directoryStore.storeName).get(key);

    request.onsuccess = () => resolve(request.result);
    request.onerror = () => reject(request.error);
    transaction.onerror = () => reject(transaction.error);
  });
}

function putInStore(db, key, value) {
  return new Promise((resolve, reject) => {
    const transaction = db.transaction(directoryStore.storeName, 'readwrite');
    transaction.objectStore(directoryStore.storeName).put(value, key);

    transaction.oncomplete = () => resolve();
    transaction.onerror = () => reject(transaction.error);
  });
}

function buildLoadMessage(count, errors, sourceLabel) {
  const sourceText = sourceLabel ? `${sourceLabel}에서 ` : '';

  if (errors.length === 0) {
    return `${sourceText}${count}개 이슈를 불러왔습니다.`;
  }

  return `${sourceText}${count}개 이슈를 불러왔고, ${errors.length}개 파일은 실패했습니다: ${errors.join('; ')}`;
}

function isIssueFileName(fileName) {
  return typeof fileName === 'string'
    && fileName.endsWith('.md')
    && !fileName.endsWith('.example.md')
    && !fileName.includes('/')
    && !fileName.includes('\\');
}

function getDirectoryLabel(directoryHandle) {
  return directoryHandle?.name || '.proofline/issues';
}

function parseIssueMarkdown(content, fileName) {
  const match = content.match(/^---\s*\n([\s\S]*?)\n---\s*\n?([\s\S]*)$/);

  if (!match) {
    throw new Error('JSON front matter가 없습니다.');
  }

  let metadata;
  try {
    metadata = JSON.parse(match[1]);
  } catch (error) {
    throw new Error(`JSON front matter 파싱 실패: ${error.message}`);
  }

  const body = match[2] || '';
  const issue = {
    fileName,
    body,
    ...metadata,
    status: normalizeStatus(metadata.status)
  };

  validateIssue(issue);
  return issue;
}

function normalizeStatus(status) {
  // 이전 원장에서 사용한 상태명은 현재 스키마의 정식 상태로 한 번만 정규화합니다.
  return status === 'in_progress' ? 'doing' : status;
}

function validateIssue(issue) {
  const requiredFields = [
    'id',
    'status',
    'title',
    'discovered_while',
    'evidence',
    'risk',
    'suggested_next_step',
    'linked_context',
    'resolved_evidence',
    'created_at',
    'updated_at'
  ];

  const missingFields = requiredFields.filter((field) => issue[field] === undefined || issue[field] === null);
  if (missingFields.length > 0) {
    throw new Error(`필수 필드 누락: ${missingFields.join(', ')}`);
  }

  if (!Array.isArray(issue.evidence)) {
    throw new Error('evidence는 배열이어야 합니다.');
  }

  if (!Array.isArray(issue.resolved_evidence)) {
    throw new Error('resolved_evidence는 배열이어야 합니다.');
  }
}

function render() {
  rememberDisclosureState();
  const visibleIssues = getVisibleIssues();
  renderSummary();
  renderIssueList(visibleIssues);
}

function rememberDisclosureState() {
  // 재렌더 직전에 현재 DOM만 읽어 자동 갱신과 필터 변경의 펼침 상태를 보존합니다.
  for (const card of elements.issuesList.querySelectorAll('[data-issue-id]')) {
    const issueId = card.dataset.issueId;
    updateOpenSet(state.expandedIssueIds, issueId, card.querySelector('.issue-disclosure').open);
    updateOpenSet(state.expandedEvidenceIds, issueId, card.querySelector('.issue-evidence').open);
  }

  for (const group of elements.issuesList.querySelectorAll('.issue-group[data-status]')) {
    updateOpenSet(state.collapsedStatuses, group.dataset.status, !group.open);
  }
}

function updateOpenSet(target, key, enabled) {
  if (enabled) {
    target.add(key);
  } else {
    target.delete(key);
  }
}

function getVisibleIssues() {
  return state.issues
    .filter((issue) => {
      const matchesStatus = state.filters.status === 'all'
        || (state.filters.status === 'open' ? openStatuses.has(issue.status) : issue.status === state.filters.status);
      if (!matchesStatus) {
        return false;
      }

      if (state.filters.risk !== 'all' && issue.risk !== state.filters.risk) {
        return false;
      }

      if (!state.filters.text) {
        return true;
      }

      return buildSearchText(issue).includes(state.filters.text);
    })
    .sort(compareIssues);
}

function buildSearchText(issue) {
  const evidenceText = issue.evidence
    .map((item) => `${item.kind || ''} ${item.location || ''} ${item.note || ''}`)
    .join(' ');

  return [
    issue.id,
    issue.status,
    issue.title,
    issue.discovered_while,
    issue.risk,
    issue.suggested_next_step,
    evidenceText,
    issue.body
  ]
    .join(' ')
    .toLowerCase();
}

function compareIssues(left, right) {
  // 상태 그룹의 위치는 사용자가 고른 항목 정렬과 무관하게 고정합니다.
  const leftStatus = statusOrder[left.status] ?? 99;
  const rightStatus = statusOrder[right.status] ?? 99;
  if (leftStatus !== rightStatus) {
    return leftStatus - rightStatus;
  }

  const [field, direction] = state.sort.split('-');
  let difference;

  if (field === 'priority') {
    difference = (riskOrder[left.risk] ?? 99) - (riskOrder[right.risk] ?? 99);
  } else {
    const key = field === 'number' ? 'id' : 'updated_at';
    difference = String(left[key]).localeCompare(String(right[key]), undefined, {
      numeric: field === 'number'
    });
  }

  const orderedDifference = direction === 'desc' ? -difference : difference;
  return orderedDifference
    || String(right.updated_at).localeCompare(String(left.updated_at))
    || String(left.id).localeCompare(String(right.id), undefined, { numeric: true });
}

function renderSummary() {
  const total = state.issues.length;
  const open = state.issues.filter((issue) => openStatuses.has(issue.status)).length;
  const blocked = state.issues.filter((issue) => issue.status === 'blocked').length;
  const resolved = state.issues.filter((issue) => issue.status === 'resolved').length;

  elements.summaryTotal.textContent = String(total);
  elements.summaryOpen.textContent = String(open);
  elements.summaryBlocked.textContent = String(blocked);
  elements.summaryResolved.textContent = String(resolved);

  for (const button of elements.summaryButtons) {
    button.setAttribute('aria-pressed', String(button.dataset.statusFilter === state.filters.status));
  }
}

function renderIssueList(issues) {
  elements.issuesList.innerHTML = '';

  if (issues.length === 0) {
    const needsFolder = elements.folderStatus.dataset.status === 'needed';
    const emptyState = document.createElement('article');
    emptyState.className = 'empty-state';
    emptyState.innerHTML = needsFolder
      ? '<h3>연결된 이슈 폴더가 없습니다.</h3><p>초기 폴더 설정에서 <code>.proofline/issues</code> 폴더를 선택하세요.</p>'
      : '<h3>표시할 이슈가 없습니다.</h3><p>필터를 바꾸거나 이슈 폴더를 다시 읽어보세요.</p>';
    elements.issuesList.appendChild(emptyState);
    return;
  }

  const groups = new Map();
  for (const issue of issues) {
    const group = groups.get(issue.status) || [];
    group.push(issue);
    groups.set(issue.status, group);
  }

  for (const [status, groupIssues] of groups) {
    elements.issuesList.appendChild(renderIssueGroup(status, groupIssues));
  }
}

function renderIssueGroup(status, issues) {
  const group = elements.issueGroupTemplate.content.firstElementChild.cloneNode(true);
  group.dataset.status = normalizeClassName(status);
  group.open = !state.collapsedStatuses.has(status);
  group.querySelector('.issue-group-title').textContent = statusLabels[status] || status;
  group.querySelector('.issue-group-count').textContent = String(issues.length);

  const rows = group.querySelector('.issue-group-rows');
  for (const issue of issues) {
    rows.appendChild(renderIssueCard(issue));
  }

  return group;
}

function renderIssueCard(issue) {
  const card = elements.issueTemplate.content.firstElementChild.cloneNode(true);
  card.classList.add(`risk-${normalizeClassName(issue.risk)}`);
  card.dataset.issueId = issue.id;

  const disclosure = card.querySelector('.issue-disclosure');
  disclosure.open = state.expandedIssueIds.has(issue.id);

  card.querySelector('.issue-id').textContent = issue.id;
  card.querySelector('.issue-title').textContent = issue.title;

  card.querySelector('.risk-label').textContent = riskLabels[issue.risk] || issue.risk;

  const updated = card.querySelector('.issue-updated');
  updated.textContent = formatIssueDate(issue.updated_at);
  updated.dateTime = String(issue.updated_at ?? '');
  card.querySelector('.issue-discovered').textContent = issue.discovered_while;
  card.querySelector('.issue-next-step').textContent = issue.suggested_next_step;
  const evidence = card.querySelector('.issue-evidence');
  evidence.open = state.expandedEvidenceIds.has(issue.id);
  card.querySelector('.issue-evidence-count').textContent = String(issue.evidence.length);
  card.querySelector('.issue-evidence-content').innerHTML = renderEvidence(issue.evidence);
  card.querySelector('.issue-body').innerHTML = renderMarkdown(issue.body);

  return card;
}

function formatIssueDate(value) {
  const timestamp = Date.parse(value);
  return Number.isNaN(timestamp)
    ? String(value ?? '')
    : new Intl.DateTimeFormat('ko-KR', { month: 'short', day: 'numeric' }).format(timestamp);
}

function renderEvidence(evidenceItems) {
  if (!evidenceItems.length) {
    return '<p class="empty-copy">등록된 근거가 없습니다.</p>';
  }

  const listItems = evidenceItems
    .map((item) => {
      const kind = escapeHtml(item.kind || '근거');
      const location = escapeHtml(item.location || '위치 미상');
      const note = escapeHtml(item.note || '');
      return `<li><div class="evidence-heading"><strong>${kind}</strong><code>${location}</code></div>${note ? `<p>${note}</p>` : ''}</li>`;
    })
    .join('');

  return `<ul class="evidence-list">${listItems}</ul>`;
}

function renderMarkdown(markdown) {
  const escaped = escapeHtml(markdown);
  const lines = escaped.split('\n');
  const html = [];
  let inList = false;
  let inCodeBlock = false;
  let codeLines = [];

  for (let index = 0; index < lines.length; index += 1) {
    const line = lines[index];

    if (line.startsWith('```')) {
      if (inCodeBlock) {
        html.push(`<pre><code>${codeLines.join('\n')}</code></pre>`);
        codeLines = [];
        inCodeBlock = false;
      } else {
        closeList();
        inCodeBlock = true;
      }
      continue;
    }

    if (inCodeBlock) {
      codeLines.push(line);
      continue;
    }

    if (isMarkdownTableRow(line) && isMarkdownTableDivider(lines[index + 1] || '')) {
      closeList();
      const headers = parseMarkdownTableRow(line);
      const rows = [];
      index += 2;

      while (index < lines.length && isMarkdownTableRow(lines[index])) {
        rows.push(parseMarkdownTableRow(lines[index]));
        index += 1;
      }

      index -= 1;
      html.push(renderMarkdownTable(headers, rows));
      continue;
    }

    if (line.startsWith('## ')) {
      closeList();
      html.push(`<h3>${renderInline(line.slice(3))}</h3>`);
      continue;
    }

    if (line.startsWith('# ')) {
      closeList();
      html.push(`<h2>${renderInline(line.slice(2))}</h2>`);
      continue;
    }

    if (line.startsWith('- ')) {
      if (!inList) {
        html.push('<ul>');
        inList = true;
      }
      html.push(`<li>${renderInline(line.slice(2))}</li>`);
      continue;
    }

    if (line.trim() === '') {
      closeList();
      continue;
    }

    closeList();
    html.push(`<p>${renderInline(line)}</p>`);
  }

  if (inCodeBlock) {
    html.push(`<pre><code>${codeLines.join('\n')}</code></pre>`);
  }

  closeList();
  return html.join('\n');

  function closeList() {
    if (inList) {
      html.push('</ul>');
      inList = false;
    }
  }
}

function isMarkdownTableRow(line) {
  return line.includes('|') && parseMarkdownTableRow(line).length > 1;
}

function isMarkdownTableDivider(line) {
  const cells = parseMarkdownTableRow(line);
  return cells.length > 1 && cells.every((cell) => /^:?-{3,}:?$/.test(cell));
}

function parseMarkdownTableRow(line) {
  // ponytail: escaped pipes are intentionally unsupported; use a GFM parser if issue bodies require them.
  return line.trim().replace(/^\|/, '').replace(/\|$/, '').split('|').map((cell) => cell.trim());
}

function renderMarkdownTable(headers, rows) {
  const head = headers.map((cell) => `<th scope="col">${renderInline(cell)}</th>`).join('');
  const body = rows.map((row) => {
    const cells = headers.map((_, index) => `<td>${renderInline(row[index] || '')}</td>`).join('');
    return `<tr>${cells}</tr>`;
  }).join('');
  return `<div class="markdown-table-wrap"><table><thead><tr>${head}</tr></thead><tbody>${body}</tbody></table></div>`;
}

function renderInline(text) {
  return text.replace(/`([^`]+)`/g, '<code>$1</code>');
}

function normalizeClassName(value) {
  return String(value || '').toLowerCase().replace(/[^a-z0-9_-]+/g, '-');
}

function escapeHtml(value) {
  return String(value ?? '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#039;');
}

loadAccentColor();
updateThemeControl();
loadBackground();
loadFromDefaultSources();
