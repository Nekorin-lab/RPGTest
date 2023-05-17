using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Common.View
{
    /// <summary>
    /// HierarchyViewの基底クラス
    /// </summary>
    public class AbstractHierarchyView : VisualElement
    {
        /// <summary>
        /// 初期化中かどうか
        /// </summary>
        protected bool isInitialize = false;
        /// <summary>
        /// 再読込中かどうか
        /// </summary>
        protected bool isRefresh = false;
        /// <summary>
        /// View内の状態を保持するための ScriptableSingleton
        /// </summary>
        public class HierarchyParams : ScriptableSingleton<HierarchyParams>
        {
            /// <summary>
            /// Foldoutの開閉状態を保持(名前)
            /// </summary>
            public List<string> FoldoutsName;
            /// <summary>
            /// Foldoutの開閉状態を保持(フラグ)
            /// </summary>
            public List<bool> FoldoutsSetting;
        }

        /// <summary>
        /// UIに配置するFoldout
        /// </summary>
        protected Dictionary<string, Foldout> foldout = new Dictionary<string, Foldout>();
        protected string ThisViewName;

        /// <summary>
        /// Hierarchy名
        /// </summary>
        public virtual string ViewName { 
            get
            {
                if (ThisViewName == null)
                    ThisViewName = this.GetType().Name;
                return ThisViewName;
            }
        }
        /// <summary>
        /// UXML定義
        /// </summary>
        protected virtual string MainUxml { get; }

        /// <summary>
        /// USS定義
        /// </summary>
        protected const string MainUssLight = "Assets/RPGMaker/Codebase/Editor/Hierarchy/Region/Base/Asset/hierarchyLight.uss";
        protected const string MainUssDark = "Assets/RPGMaker/Codebase/Editor/Hierarchy/Region/Base/Asset/hierarchyDark.uss";
        
        /// <summary>
        /// TOPのVisualElement
        /// </summary>
        protected VisualElement UxmlElement { get; set; }

        /// <summary>
        /// UI初期化処理
        /// </summary>
        protected virtual void InitUI() {
            if (isInitialize) return;
            isInitialize = true;

            //Foldout用の初期化
            if (HierarchyParams.instance.FoldoutsName == null)
            {
                HierarchyParams.instance.FoldoutsName = new List<string>();
                HierarchyParams.instance.FoldoutsSetting = new List<bool>();
            }

            //初期化
            Clear();

            //UXMLの読込
            if (MainUxml != "")
            {
                UxmlElement = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MainUxml).CloneTree();
                EditorLocalize.LocalizeElements(UxmlElement);
                UxmlElement.style.flexGrow = 1;
                Add(UxmlElement);
            }
            else
            {
                UxmlElement = this;
            }
            
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(MainUssDark);
            if (!EditorGUIUtility.isProSkin)
            {
                styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(MainUssLight);
            }
            UxmlElement.styleSheets.Clear();
            UxmlElement.styleSheets.Add(styleSheet);
            //各コンテンツデータの初期化
            InitContentsData();

            isInitialize = false;
        }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        protected virtual void InitContentsData() {}

        /// <summary>
        /// リフレッシュ処理
        /// </summary>
        protected bool Refresh() {
            if (isRefresh) return false;
            isRefresh = true;
            RefreshContents();
            isRefresh = false;
            return true;
        }

        /// <summary>
        /// コンテンツのリフレッシュ
        /// </summary>
        protected virtual void RefreshContents() {}

        /// <summary>
        /// Foldout部品の登録
        /// 既に開閉状態を保持していた場合には、そのデータを復元する
        /// </summary>
        /// <param name="foldout"></param>
        public void SetFoldout(string key) {
            //keyに対して、Viewの名称を付加する
            //これにより、各画面単位で一意に定まる名称に置き換える
            string keyWork = ViewName + "_" + key;

            if (foldout.ContainsKey(keyWork))
                foldout.Remove(keyWork);

            //ScriptableSingleton に値を保持していなければ初期化処理
            if (!HierarchyParams.instance.FoldoutsName.Contains(keyWork))
            {
                HierarchyParams.instance.FoldoutsName.Add(keyWork);
                HierarchyParams.instance.FoldoutsSetting.Add(false);
            }

            //対象のFoldout部品
            Foldout foldoutData = UxmlElement.Query<Foldout>(key);
            int foldoutIndex = HierarchyParams.instance.FoldoutsName.IndexOf(keyWork);

            //Foldoutの開閉状態を取得し、最終の設定値を ScriptableSingleton に保持
            foldoutData.RegisterValueChangedCallback(evt =>
            {
                HierarchyParams.instance.FoldoutsSetting[foldoutIndex] = foldoutData.value;
            });

            //Foldoutを管理する
            foldout.Add(keyWork, foldoutData);

            //最終の設定値を、初期値として設定
            foldoutData.value = HierarchyParams.instance.FoldoutsSetting[foldoutIndex];
        }

        /// <summary>
        /// Foldout部品を返却
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Foldout GetFoldout(string key) {
            return foldout[ViewName + "_" + key];
        }
    }
}