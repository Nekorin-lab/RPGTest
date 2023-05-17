using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Class;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Encounter;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Enemy;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventBattle;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Troop;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Vehicle;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Hierarchy.Region.Base.View.Component;
using RPGMaker.Codebase.Editor.Hierarchy.Region.Character.View.Component;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Hierarchy.Region.Character.View
{
    /// <summary>
    /// キャラクターのHierarchyView
    /// </summary>
    public class BattleHierarchyView : AbstractHierarchyView
    {
        // const
        //--------------------------------------------------------------------------------------------------------------
        protected override string MainUxml { get { return "Assets/RPGMaker/Codebase/Editor/Hierarchy/Region/Character/Asset/database_battle.uxml"; } }

        // 利用するデータ
        //--------------------------------------------------------------------------------------------------------------
        private List<EnemyDataModel> _enemyDataModels;
        private List<EventBattleDataModel> _eventBattleDataModels;
        private List<TroopDataModel> _troopDataModels;
        private string _updateData;

        // ヒエラルキー本体クラス
        //--------------------------------------------------------------------------------------------------------------
        private readonly BattleHierarchy _battleHierarchy;

        // UI要素
        //--------------------------------------------------------------------------------------------------------------
        private HierarchyItemListView _enemyCharacterListView;
        private TroopListView _enemyGroupListView;
        private Button _battleSceneButton;
        private int _updateType = 0;

        //--------------------------------------------------------------------------------------------------------------
        //
        // methods
        //
        //--------------------------------------------------------------------------------------------------------------

        // 初期化・更新系
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="battleHierarchy"></param>
        public BattleHierarchyView(BattleHierarchy battleHierarchy) {
            _battleHierarchy = battleHierarchy;
            InitUI();
        }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        override protected void InitContentsData() {
            //戦闘シーン
            _battleSceneButton = UxmlElement.Query<Button>("battleSceneButton");

            //敵
            SetFoldout("enemyCharacterFoldout");
            _enemyCharacterListView = new HierarchyItemListView(ViewName);
            ((VisualElement) UxmlElement.Query<VisualElement>("enemyCharacterListContainer")).Add(_enemyCharacterListView);

            //敵グループ
            SetFoldout("enemyGroupFoldout");
            _enemyGroupListView = new TroopListView(
                _troopDataModels,
                _eventBattleDataModels,
                this
            );
            ((VisualElement) UxmlElement.Query<VisualElement>("enemyGroupListContainer")).Add(_enemyGroupListView);

            //バトルの編集
            SetFoldout("battleSettingFoldout");
            InitEventHandlers();
        }

        /// <summary>
        /// イベントの初期設定
        /// </summary>
        private void InitEventHandlers() {
            EnemyDataModel enemyDataModel = null;
            TroopDataModel troopDataModel = null;
            TroopDataModel troopDataModelWork = null;
            var eventNum = 0;

            // 戦闘シーンボタンクリック時
            Editor.Hierarchy.Hierarchy.AddSelectableElementAndAction(_battleSceneButton,
                () => { _battleHierarchy.OpenBattleSceneInspector(); });
            _battleSceneButton.clickable.clicked += () =>
            {
                Editor.Hierarchy.Hierarchy.InvokeSelectableElementAction(_battleSceneButton);
            };

            // 敵キャラFoldout右クリック時
            BaseClickHandler.ClickEvent(GetFoldout("enemyCharacterFoldout"),
                evt =>
                {
                    if (evt != (int) MouseButton.RightMouse) return;

                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0560")), false,
                        () => { _battleHierarchy.CreateEnemyDataModel(this); });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0561")), false, () =>
                    {
                        if (enemyDataModel != null) _battleHierarchy.PasteEnemyDataModel(this, enemyDataModel);
                    });
                    menu.ShowAsContext();
                });

            // 敵キャラリストアイテムクリック時
            _enemyCharacterListView.SetEventHandler(
                (i, value) => { _battleHierarchy.OpenEnemyInspector(_enemyDataModels[i], this); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0562")), false,
                        () => { enemyDataModel = _enemyDataModels[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0563")), false,
                        () => { _battleHierarchy.DeleteEnemyDataModel(_enemyDataModels[i]); });
                    menu.ShowAsContext();
                });

            // 敵グループFoldout右クリック時
            BaseClickHandler.ClickEvent(GetFoldout("enemyGroupFoldout"),
                evt =>
                {
                    if (evt != (int) MouseButton.RightMouse) return;

                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0566")), false,
                        () => { _battleHierarchy.CreateTroopDataModel(this); });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0567")), false, () =>
                    {
                        if (troopDataModel != null) _battleHierarchy.PasteTroopDataModel(this, troopDataModel);
                    });
                    menu.ShowAsContext();
                });

            // 敵グループアイテムクリック時
            _enemyGroupListView.SetEventHandler(
                (i, value) => { _battleHierarchy.OpenTroopInspector(_troopDataModels[i], this); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0568")), false,
                        () => { troopDataModel = _troopDataModels[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0569")), false,
                        () => { _battleHierarchy.DeleteTroopDataModel(_troopDataModels[i]); });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0570")), false, () =>
                    {
                        _battleHierarchy.CreateTroopEventDataModel(_troopDataModels[i]);
                        _battleHierarchy.Refresh(_troopDataModels[i].id);
                    });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0571")), false, () =>
                    {
                        if (troopDataModelWork != null)
                        {
                            _battleHierarchy.PasteTroopEventDataModel(troopDataModelWork, _troopDataModels[i], eventNum);
                            _battleHierarchy.Refresh(_troopDataModels[i].id);
                        }
                    });
                    menu.ShowAsContext();
                },
                (i, value, num) =>
                {
                    _battleHierarchy.OpenTroopInspector(_troopDataModels[i], this, num);
                },
                (i, value, num) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0572")), false, () =>
                    {
                        troopDataModelWork = _troopDataModels[i];
                        eventNum = num;
                    });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0573")), false, () =>
                    {
                        _battleHierarchy.DeleteTroopEventDataModel(_troopDataModels[i], num);
                        //削除後バトルイベントの最後尾を選択。numが-1になれば通常の敵グループを選択
                        _battleHierarchy.OpenTroopInspector(_troopDataModels[i], this, num-1);
                        // 更新時に開閉状態が初期化されるので修正予定
                        _battleHierarchy.Refresh();
                    });
                    menu.ShowAsContext();
                });
        }

        // データ更新
        //--------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="characterActorDataModels"></param>
        /// <param name="npcCharacterActorDataModels"></param>
        /// <param name="vehiclesDataModels"></param>
        /// <param name="classDataModels"></param>
        /// <param name="enemyDataModels"></param>
        /// <param name="troopDataModels"></param>
        /// <param name="eventBattleDataModels"></param>
        /// <param name="encounterDataModels"></param>
        public void Refresh(
            string updateData = null,
            [CanBeNull] List<EnemyDataModel> enemyDataModels = null,
            [CanBeNull] List<TroopDataModel> troopDataModels = null,
            [CanBeNull] List<EventBattleDataModel> eventBattleDataModels = null,
            int updateType = 0
        ) {
            _updateData = updateData;
            _updateType = updateType;
            if (enemyDataModels != null) _enemyDataModels = enemyDataModels;
            if (troopDataModels != null) _troopDataModels = troopDataModels;
            if (eventBattleDataModels != null) _eventBattleDataModels = eventBattleDataModels;
            base.Refresh();
        }

        /// <summary>
        /// データ更新
        /// </summary>
        protected override void RefreshContents() {
            base.RefreshContents();
            if (_updateData == null)
            {
                SetEnemy();
                SetTroop();
            }
            else
            {
                UpdateEnemy();
                UpdateTroop();
            }
        }

        /// <summary>
        /// 敵の更新
        /// </summary>
        private void SetEnemy() {
            //特徴の先頭が、
            //[0]=命中率
            //[1]=回避率
            //[2]=攻撃時属性
            //となっていない場合には不正データのため、ここで補正する
            foreach (var _enemy in _enemyDataModels)
            {
                TraitCommonDataModel traitWork;
                bool flg = false;
                if (_enemy.traits == null)
                {
                    _enemy.traits = new List<TraitCommonDataModel>();
                }
                if (_enemy.traits.Count < 1 || _enemy.traits[0].categoryId != 2 || _enemy.traits[0].traitsId != 2 || _enemy.traits[0].effectId != 0)
                {
                    //0番目の特徴が命中率ではない
                    flg = false;
                    for (int i = 0; i < _enemy.traits.Count; i++)
                    {
                        if (_enemy.traits[i].categoryId == 2 && _enemy.traits[i].traitsId == 2 && _enemy.traits[i].effectId == 0)
                        {
                            //他のところに命中率があった場合、それを0番目に移動する
                            traitWork = _enemy.traits[i];
                            _enemy.traits.RemoveAt(i);
                            _enemy.traits.Insert(0, traitWork);

                            if (traitWork.value == 0)
                                traitWork.value = 950;

                            flg = true;
                            break;
                        }
                    }
                    if (!flg)
                    {
                        //他のところにも命中率がなかったので追加
                        _enemy.traits.Insert(0, new TraitCommonDataModel(2, 2, 0, 950));
                    }
                }
                if (_enemy.traits.Count < 2 || _enemy.traits[1].categoryId != 2 || _enemy.traits[1].traitsId != 2 || _enemy.traits[1].effectId != 1)
                {
                    //2番目の特徴が回避率ではない
                    flg = false;
                    for (int i = 0; i < _enemy.traits.Count; i++)
                    {
                        if (_enemy.traits[i].categoryId == 2 && _enemy.traits[i].traitsId == 2 && _enemy.traits[i].effectId == 1)
                        {
                            //他のところに回避率があった場合、それを1番目に移動する
                            traitWork = _enemy.traits[i];
                            _enemy.traits.RemoveAt(i);
                            _enemy.traits.Insert(1, traitWork);
                            flg = true;
                            break;
                        }
                    }
                    if (!flg)
                    {
                        //他のところにも回避率がなかったので追加
                        _enemy.traits.Insert(1, new TraitCommonDataModel(2, 2, 1, 50));
                    }
                }
                if (_enemy.traits.Count < 3 || _enemy.traits[2].categoryId != 3 || _enemy.traits[2].traitsId != 1)
                {
                    //3番目の特徴が攻撃時属性ではない
                    flg = false;
                    for (int i = 0; i < _enemy.traits.Count; i++)
                    {
                        if (_enemy.traits[i].categoryId == 3 && _enemy.traits[i].traitsId == 1)
                        {
                            //他のところに攻撃時属性があった場合、それを2番目に移動する
                            traitWork = _enemy.traits[i];
                            _enemy.traits.RemoveAt(i);
                            _enemy.traits.Insert(2, traitWork);
                            flg = true;
                            break;
                        }
                    }
                    if (!flg)
                    {
                        //他のところにも攻撃時属性がなかったので追加
                        _enemy.traits.Insert(2, new TraitCommonDataModel(3, 1, 2, 0));
                    }
                }
            }

            _enemyCharacterListView.Refresh(_enemyDataModels.Select(item => item.name).ToList());
        }

        private void UpdateEnemy() {
            for (int i = 0; i < _enemyDataModels.Count; i++)
            {
                if (_enemyDataModels[i].id == _updateData)
                {
                    _enemyCharacterListView.Refresh(_enemyDataModels.Select(item => item.name).ToList(), i);
                    break;
                }
            }
        }

        /// <summary>
        /// 敵グループの更新
        /// </summary>
        private void SetTroop() {
            _enemyGroupListView.Refresh(_troopDataModels, _eventBattleDataModels);
        }

        private void UpdateTroop() {
            for (int i = 0; i < _troopDataModels.Count; i++)
            {
                if (_troopDataModels[i].id == _updateData)
                {
                    _enemyGroupListView.Refresh(_troopDataModels, _eventBattleDataModels, _updateData, _updateType);
                    break;
                }
            }
        }

        /// <summary>
        /// 最終選択していた敵を返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastEnemyIndex() {
            var elements = new List<VisualElement>();
            _enemyCharacterListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していた敵グループを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastTroopIndex() {
            var elements = new List<VisualElement>();
            _enemyGroupListView.Query<Foldout>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        // イベントハンドラ
        //--------------------------------------------------------------------------------------------------------------
    }
}