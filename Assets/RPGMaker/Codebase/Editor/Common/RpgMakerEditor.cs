using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Lib.Auth;
using RPGMaker.Codebase.CoreSystem.Lib.Migration;
using RPGMaker.Codebase.Runtime.Addon;
using RPGMaker.Codebase.CoreSystem.Service.MapManagement;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Common.Window;
using RPGMaker.Codebase.Editor.Common.Window.ModalWindow;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using RPGMaker.Codebase.CoreSystem.Lib.Auth.UnityConnection;
using System.Reflection;

namespace RPGMaker.Codebase.Editor.Common
{
    public class RpgMakerEditorParam : ScriptableSingleton<RpgMakerEditorParam>
    {
        public RpgMakerEditor RpgMakerEditor;
        public string         ActiveBuildSetting;
        public bool           IsWindowInitialized;
    }

    public class RpgMakerEditor
    {
        //----------------------------------------------------------------------------------------------------------------------------------
        //
        // properties / consts
        //
        //----------------------------------------------------------------------------------------------------------------------------------
        internal const string RpgMakerUniteMenuItemPath = "Window/RPG Maker/Mode/RPG Maker Focused Mode";
        internal const string UnityEditorMenuItemPath = "Window/RPG Maker/Mode/Unity Editor";
        internal const string RpgMakerUniteWindowMenuItemPath = "Window/RPG Maker/Mode/RPG Maker+Unity Editor";

        private static MenuWindow   _menuWindow;
        private static EditorWindow _gameView;

        private static int _initialized;
        private static bool _isStart;
        private static bool _isStartMenu;
        private static double _timeSinceStartUp;
        public static bool IsImportEffekseer = false;

        //----------------------------------------------------------------------------------------------------------------------------------
        //
        // methods
        //
        //----------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
        // 起動時の初期化処理
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Unity立ち上げ時の処理
        /// </summary>
        [InitializeOnLoadMethod]
        public static void InitializeOnLoad() {

            // This script is intended to run on Editor startup only.
            // Do nothing when entering playmode
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            // Already initialized, skip the work
            if(RpgMakerEditorParam.instance.RpgMakerEditor != null) {
                return;
            }

            // テンプレートPJを、Unityのインストールフォルダへコピーする
            TemplateInstallHelper.InstallRPGMakerUniteTemplates();

            // unitypackage からの初回インストール時の場合の、Storageインポート処理
            TemplateInstallHelper.InitializeDefaultStorage();

            InitRPGMakerEditor();
        }

        private static void InitRPGMakerEditor() {
            if (_isStart) return;
            _isStart = true;

            AnalyticsManager.Instance.PostEvent(
                AnalyticsManager.EventName.action,
                AnalyticsManager.EventParameter.initialize);
            AddonManager.Instance.Refresh();

            var instance = RpgMakerEditorParam.instance;
            instance.IsWindowInitialized = false;
            instance.RpgMakerEditor = new RpgMakerEditor();
            instance.RpgMakerEditor.StartRPGMakerAsync();
        }
        
        private static async Task DelayAuth() {
            var isEditorOnline = false;
            while (!isEditorOnline)
            {
                isEditorOnline = Utils.IsOnline();
                await Task.Delay(1);
            }

            await Task.Delay(1);

            var isEditorLoggedIn = false;
            var timeFrom = DateTime.Now.Hour * 60 * 60 * 1000 + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
            while (!isEditorLoggedIn)
            {
                isEditorLoggedIn = Utils.IsLoggedIn();
                await Task.Delay(1);
                
                if (DateTime.Now.Hour * 60 * 60 * 1000 + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond - timeFrom > 5000)
                {
                    // 5秒でログイン状態を確認できなかったらログインモーダルを出す
                    UnitySignInPromptWindow.ShowWindow();
                    throw new Exception("Not logged in.");
                }
            }

            await AuthRpgMaker();
        }
        
        /// <summary>
        ///     RpgMakerを認証。
        /// </summary>
        private static async Task AuthRpgMaker() {
            // 認証
            switch (await Auth.AttemptToAuthenticate())
            {
                case Auth.AuthStatus.AuthenticatedByUnityAssetStore:
                    // Unity Asset Store認証成功
                    break;
                case Auth.AuthStatus.NotAuthenticated:
                    AuthErrorWindow.ShowWindow(EditorLocalize.LocalizeText("WORD_5014"),
                        EditorLocalize.LocalizeText("WORD_5011"));
                    throw new Exception(EditorLocalize.LocalizeText("WORD_5011"));
                case Auth.AuthStatus.NotAuthenticatedWithConnectionError:
                    AuthErrorWindow.ShowWindow(EditorLocalize.LocalizeText("WORD_5014"),
                        EditorLocalize.LocalizeText("WORD_5011"));
                    throw new Exception(EditorLocalize.LocalizeText("WORD_5012"));
                default:
                    throw new ArgumentOutOfRangeException();
            }

            try
            {
                Migration.Migrate();
            }
            catch (Exception)
            {
            }
        }
        
        private void StartRPGMakerAsync() {
            //Unity立ち上げ直後だと、Systemのエラーになるため、コルーチンで実行
            EditorCoroutineUtility.StartCoroutine(StartRpgMaker(), this);
        }
        
        private static IEnumerator StartRpgMaker() {
            var instance = RpgMakerEditorParam.instance;
            _timeSinceStartUp = EditorApplication.timeSinceStartup;

            //StartRpgMakerが実行されてから3秒待つ
            while (3 > EditorApplication.timeSinceStartup - _timeSinceStartUp) yield return null;

            // RepositoryUpdateHelper.ApplyRepositoryUpdates();

            // If first initialization is necessary, do it in here:
            _initialized = RPGMakerDefaultConfigSingleton.InitializeDefaultSettingsForRPGMakerUnite();

            // 初期化未実施の場合は、後続の処理を行わない
            if (_initialized == 0) {
                _isStart = false;
                yield break;
            }

            if (_initialized == 1)
            {
                switch (RPGMakerDefaultConfigSingleton.instance.UniteMode)
                {
                    case RPGMakerDefaultConfigSingleton.RpgMakerUniteModeId:
                        RpgMakerUniteMenu();
                        break;
                    case RPGMakerDefaultConfigSingleton.RpgMakerUniteWindowModeId:
                        RpgMakerUniteWindow();
                        break;
                    case RPGMakerDefaultConfigSingleton.InitializeModeId:
                        RpgMakerUniteMenu();
                        break;
                    default:
                        _isStart = false;
                        break;
                }

                SetEditorModeMenuChecked();
            }
            else
            {
                // 設定を行った後、再起動を行う
                var editorApplicationType = typeof(EditorApplication);
                var restartEditorAndRecompileScripts =
                    editorApplicationType.GetMethod("RestartEditorAndRecompileScripts",
                        BindingFlags.NonPublic | BindingFlags.Static);

                restartEditorAndRecompileScripts.Invoke(null, null);
            }
        }

        private static void ApplyEditorMode(string targetModeId) {
            ModeService.ChangeModeById(targetModeId);
            RPGMakerDefaultConfigSingleton.instance.UniteMode = targetModeId;
            SetEditorModeMenuChecked();
        }

        //----------------------------------------------------------------------------------------------------------------------------------
        // ツールメニューからエディタを開く処理
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     RPG Maker Uniteを開く
        /// </summary>
        [MenuItem(RpgMakerUniteMenuItemPath)]
        private static async void RpgMakerUniteMenu() {
            // If first initialization is necessary, do it in here:
            if (!_isStart && !_isStartMenu)
            {
                _isStartMenu = true;
                _initialized = RPGMakerDefaultConfigSingleton.InitializeDefaultSettingsForRPGMakerUnite();
            }
            else if (_isStartMenu)
            {
                return;
            }

            _isStart = false;
            _isStartMenu = false;

            // 初期化未実施の場合は、後続の処理を行わない
            if (_initialized == 0)
            {
                return;
            }
            else if (_initialized > 1)
            {
                // 設定を行った後、再起動を行う
                var editorApplicationType = typeof(EditorApplication);
                var restartEditorAndRecompileScripts =
                    editorApplicationType.GetMethod("RestartEditorAndRecompileScripts",
                        BindingFlags.NonPublic | BindingFlags.Static);

                restartEditorAndRecompileScripts.Invoke(null, null);
                return;
            }


            // UnityのAssetStoreへのログインが完了するのを待ってから、認証処理を走らせる
            await DelayAuth();

            // モードを保存
            ApplyEditorMode(RPGMakerDefaultConfigSingleton.RpgMakerUniteModeId);
            // rpgmaker.mode内でも指定しているが、適切に適用されないようなので、ここで適用する。
            LayoutUtility.LoadLayout("Packages/com.unity.vsp.rpgmaker/Layouts/RPGMaker.wlt");
            // レイアウトを変更した場合は明示的に再初期化
            RPGMaker.Codebase.Editor.Hierarchy.Hierarchy.IsInitialized = false;
            InitWindows();

            // 初期シーンを開く
            EditorSceneManager.OpenScene("Assets/RPGMaker/Codebase/Runtime/Title/Title.unity");
            // 保存
            EditorApplication.ExecuteMenuItem("File/Save");
        }

        /// <summary>
        ///     RPG Maker Unite Window (開発用)を開く
        /// </summary>
        [MenuItem(RpgMakerUniteWindowMenuItemPath)]
        private static async void RpgMakerUniteWindow() {
            // If first initialization is necessary, do it in here:
            if (!_isStart && !_isStartMenu)
            {
                _isStartMenu = true;
                _initialized = RPGMakerDefaultConfigSingleton.InitializeDefaultSettingsForRPGMakerUnite();
            }
            else if (_isStartMenu)
            {
                return;
            }
            _isStart = false;
            _isStartMenu = false;

            // 初期化未実施の場合は、後続の処理を行わない
            if (_initialized == 0)
            {
                return;
            }
            else if (_initialized > 1)
            {
                // 設定を行った後、再起動を行う
                var editorApplicationType = typeof(EditorApplication);
                var restartEditorAndRecompileScripts =
                    editorApplicationType.GetMethod("RestartEditorAndRecompileScripts",
                        BindingFlags.NonPublic | BindingFlags.Static);

                restartEditorAndRecompileScripts.Invoke(null, null);
                return;
            }

            // UnityのAssetStoreへのログインが完了するのを待ってから、認証処理を走らせる
            await DelayAuth();

            // モードを保存
            ApplyEditorMode(RPGMakerDefaultConfigSingleton.RpgMakerUniteWindowModeId);
#if UNITY_EDITOR_WIN
            LayoutUtility.LoadLayout("Assets/RPGMaker/Codebase/Editor/Layouts/DatabaseLayout2.wlt");
#else
            LayoutUtility.LoadLayout("Assets/RPGMaker/Codebase/Editor/Layouts/DatabaseLayout_Mac.wlt");
#endif
            // レイアウトを変更した場合は明示的に再初期化
            RPGMaker.Codebase.Editor.Hierarchy.Hierarchy.IsInitialized = false;
            InitWindows();
        }

        /// <summary>
        ///     Unity Editor。
        /// </summary>
        [MenuItem(UnityEditorMenuItemPath)]
        private static void UnityEditorMenu() {
            // モードを保存
            ApplyEditorMode(RPGMakerDefaultConfigSingleton.DefaultEditorModeId);
            LayoutUtility.LoadLayout("Packages/com.unity.vsp.rpgmaker/Layouts/Default.wlt");
            // レイアウトを変更した場合は明示的に再初期化
            RPGMaker.Codebase.Editor.Hierarchy.Hierarchy.IsInitialized = false;
        }

        /// <summary>
        ///     メニュー項目のチェック表示/非表示設定。
        /// </summary>
        private static void SetEditorModeMenuChecked() {
            DebugUtil.Log($"ModeService.currentId={ModeService.currentId}");
            Menu.SetChecked(RpgMakerUniteMenuItemPath, ModeService.currentId == RPGMakerDefaultConfigSingleton.RpgMakerUniteModeId);
            Menu.SetChecked(UnityEditorMenuItemPath, ModeService.currentId == RPGMakerDefaultConfigSingleton.DefaultEditorModeId);
            Menu.SetChecked(RpgMakerUniteWindowMenuItemPath, ModeService.currentId == RPGMakerDefaultConfigSingleton.RpgMakerUniteWindowModeId);
        }

        //----------------------------------------------------------------------------------------------------------------------------------
        // ヘルプメニュー
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Help。
        /// </summary>
        [MenuItem("Help/RPG Maker Unite Help...", priority = 10000, editorModes = new[]
        {
            RPGMakerDefaultConfigSingleton.RpgMakerUniteModeId,
            RPGMakerDefaultConfigSingleton.RpgMakerUniteWindowModeId
        })]
        private static void HelpMenu() {
            MenuEditorView.Help();
        }

        /// <summary>
        ///     About。
        /// </summary>
        [MenuItem("Help/About RPG Maker Unite...", priority = 10000, editorModes = new[]
        {
            RPGMakerDefaultConfigSingleton.RpgMakerUniteModeId,
            RPGMakerDefaultConfigSingleton.RpgMakerUniteWindowModeId
        })]
        private static void AboutMenu() {
            MenuEditorView.About();
        }

        //----------------------------------------------------------------------------------------------------------------------------------
        // その他
        //----------------------------------------------------------------------------------------------------------------------------------
        public static void InitWindows() {
            // 現在のモードがUnityモードであれば、Windowを作成せずに終了
            if (RPGMakerDefaultConfigSingleton.instance.UniteMode == RPGMakerDefaultConfigSingleton.DefaultEditorModeId)
            {
                return;
            }

            // 共通メニューウィンドウ
            _menuWindow = WindowLayoutManager.GetOrOpenWindow(WindowLayoutManager.WindowLayoutId.MenuWindow) as MenuWindow;
            _menuWindow.titleContent = new GUIContent(EditorLocalize.LocalizeText("WORD_1560"));
            _menuWindow.Init();

            // ヒエラルキー
            if (!Hierarchy.Hierarchy.Init())
            {
                Hierarchy.Hierarchy.SetInspector();

                //初回又は、BuildSettingsのデータが変わっていた場合に、フォントなどのUIパターンを適用しなおす
                if (RpgMakerEditorParam.instance.ActiveBuildSetting != EditorUserBuildSettings.activeBuildTarget.ToString())
                {
                    RpgMakerEditorParam.instance.ActiveBuildSetting = EditorUserBuildSettings.activeBuildTarget.ToString();
                    //フォント適用しなおしはよばない
                    //FontManager.InitializeFont();
                }
                return;
            }

            // インスペクター
            // Inspector.Inspector.Init();

            // データベースエディタ
            DatabaseEditor.DatabaseEditor.Init();

            // マップエディタ
            MapEditor.MapEditor.Init();

            // アウトラインエディタ
            OutlineEditor.OutlineEditor.Init();

            // インスペクター開いていたもの開く
            Hierarchy.Hierarchy.SetInspector();

            RpgMakerEditorParam.instance.IsWindowInitialized = true;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeExec() {
            if (!MenuWindowParams.instance.IsRpgEditorPlayMode) return;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            //AssetDatabase.Refresh();
            
            FontManager.ResetFonts();

            RuntimeInitWindows();
        }

        private static async void SetWindows() {
            await Task.Delay(2000);
            
            WindowLayoutManager.CloseWindows(new List<WindowLayoutManager.WindowLayoutId>()
            {
                WindowLayoutManager.WindowLayoutId.MapEventExecutionContentsWindow,
                WindowLayoutManager.WindowLayoutId.MapEventRouteWindow,
                WindowLayoutManager.WindowLayoutId.MapEventCommandSettingWindow,
                WindowLayoutManager.WindowLayoutId.MapEventMoveCommandWindow
            });
            
            var assembly = typeof(EditorWindow).Assembly;
            var type = assembly.GetType("UnityEditor.GameView");
            _gameView = EditorWindow.GetWindow(type);
            _gameView.Focus();

            //少しまってからフォーカスをあてなおさないと、タイミングによってタブが切り替わらない
            await Task.Delay(1000);
            _gameView.Focus();
        }

        /// <summary>
        ///     Playmodeの状態が変わった時に実行される
        /// </summary>
        private static async void OnPlayModeStateChanged(PlayModeStateChange state) {
            if (!EditorApplication.isPlaying)
            {
                new MapManagementService().ResetMap();
                AssetDatabase.Refresh();
                await Task.Delay(100);
                InitWindows();
            }
        }

        public static void RuntimeInitWindows() {
            // 現在のモードがUnityモードであれば、Windowを作成せずに終了
            if (RPGMakerDefaultConfigSingleton.instance.UniteMode == RPGMakerDefaultConfigSingleton.DefaultEditorModeId)
            {
                return;
            }

            // 共通メニューウィンドウ
            _menuWindow = WindowLayoutManager.GetOrOpenWindow(WindowLayoutManager.WindowLayoutId.MenuWindow) as MenuWindow;
            _menuWindow.titleContent = new GUIContent(EditorLocalize.LocalizeText("WORD_1560"));
            _menuWindow.Init();

            // ヒエラルキー
            if (!Hierarchy.Hierarchy.Init())
            {
                Hierarchy.Hierarchy.SetInspector();
                SetWindows();
                return;
            }

            // インスペクター
            // Inspector.Inspector.Init();

            // データベースエディタ
            DatabaseEditor.DatabaseEditor.Init();

            // マップエディタ
            MapEditor.MapEditor.Init();

            // アウトラインエディタ
            OutlineEditor.OutlineEditor.Init();

            // インスペクター開いていたもの開く
            Hierarchy.Hierarchy.SetInspector();

            SetWindows();
        }

        /// <summary>
        /// ウィンドウ最大化から復帰時の処理。
        /// </summary>
        public static void WindowMaximizationRecoveringProcess() {
            // 認証済でない場合は何もしない
            if (!Auth.IsAuthenticated)
            {
                return;
            }

            //最大化から復帰した場合には初期化を再実行
            RPGMaker.Codebase.Editor.Hierarchy.Hierarchy.IsInitialized = false;
            RuntimeInitWindows();

            // 以下を呼ばないと、RpgMakerEditor.SetWindows メソッドによって、
            // 『イベント実行内容』枠と『イベントコマンド』枠が閉じられてしまうので、呼ぶ。
            // シーン再生終了時も RuntimeInitWindows の後に呼ばれている。
            InitWindows();
        }

        public static void DataCheckMenuButton(bool flg) {
            _menuWindow?.SetButtonEnabled(flg);
        }
    }
}