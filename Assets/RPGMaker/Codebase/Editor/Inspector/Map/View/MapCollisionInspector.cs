using System;
using System.Collections.Generic;
using System.Linq;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.View;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Inspector.Map.View
{
    public class MapCollisionInspector : VisualElement
    {
        private const string Uxml =
            "Assets/RPGMaker/Codebase/Editor/Inspector/Map/Asset/MapCollisionInspector.uxml";

        private          FloatField _constant_floatField;
        private          Toggle     _constant_toggle;
        private          Toggle     _counter_off_toggle;
        private          Toggle     _counter_on_toggle;
        private          Toggle     _damaged_floor_off_toggle;
        private          Toggle     _damaged_floor_on_toggle;
        private          Toggle     _dive_through_toggle;
        private          Toggle     _down_toggle;
        private          Toggle     _ladder_off_toggle;
        private          Toggle     _ladder_on_toggle;
        private          Toggle     _left_toggle;
        private readonly Action     _onClickRegisterBtn;

        private Toggle _pass_through_toggle;

        private List<string> _passTypeDictionary;
        private FloatField   _percentage_rate_floatField;
        private Toggle       _percentage_rate_toggle;
        private Toggle       _right_toggle;
        private Toggle       _thicket_off_toggle;
        private Toggle       _thicket_on_toggle;

        private readonly TileDataModel      _tileDataModel;
        private          TileImageDataModel _tileImageEntity;
        private          Toggle             _up_toggle;


        public MapCollisionInspector(TileDataModel tileDataModel, Action onClickRegisterBtn) {
            _tileDataModel = tileDataModel;
            _onClickRegisterBtn = onClickRegisterBtn;
            InitUI();
            SetEntityToUI();
        }

        public void SetTileImageEntity(TileImageDataModel tileImageEntity) {
            _tileImageEntity = tileImageEntity;
        }

        private void InitUI() {
            Clear();
            style.flexDirection = FlexDirection.Row;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Uxml);
            var container = visualTree.CloneTree();
            EditorLocalize.LocalizeElements(container);
            container.style.flexGrow = 1;
            Add(container);

            _pass_through_toggle = container.Query<Toggle>("pass_through_toggle");
            _dive_through_toggle = container.Query<Toggle>("dive_through_toggle");
            _up_toggle = container.Query<Toggle>("up_toggle");
            _left_toggle = container.Query<Toggle>("left_toggle");
            _right_toggle = container.Query<Toggle>("right_toggle");
            _down_toggle = container.Query<Toggle>("down_toggle");
            _ladder_on_toggle = container.Query<Toggle>("ladder_on_toggle");
            _ladder_off_toggle = container.Query<Toggle>("ladder_off_toggle");
            _thicket_on_toggle = container.Query<Toggle>("thicket_on_toggle");
            _thicket_off_toggle = container.Query<Toggle>("thicket_off_toggle");
            _counter_on_toggle = container.Query<Toggle>("counter_on_toggle");
            _counter_off_toggle = container.Query<Toggle>("counter_off_toggle");
            _damaged_floor_on_toggle = container.Query<Toggle>("damaged_floor_on_toggle");
            _damaged_floor_off_toggle = container.Query<Toggle>("damaged_floor_off_toggle");
            _constant_toggle = container.Query<Toggle>("constant_toggle");
            _percentage_rate_toggle = container.Query<Toggle>("percentage_rate_toggle");
            _constant_floatField = container.Query<FloatField>("constant_floatField");
            _percentage_rate_floatField = container.Query<FloatField>("percentage_rate_floatField");

            _passTypeDictionary = EditorLocalize.LocalizeTexts(new List<string>
            {
                "WORD_0808",
                "WORD_0809",
                "WORD_0810"
            });
        }

        private static PopupFieldBase<string> MakePopupField(
            Dictionary<string, string> dictionary,
            VisualElement parentContainer,
            string containerName
        ) {
            var container = (VisualElement) parentContainer.Query<VisualElement>(containerName);
            var popupField = new PopupFieldBase<string>(dictionary.Values.ToList(), 0);
            container.Add(popupField);
            return popupField;
        }

        private void SetEntityToUI() {
            if (_passTypeDictionary.IndexOf(_tileDataModel.passType.ToString()) == 0)
                _pass_through_toggle.value = true;
            _pass_through_toggle.RegisterValueChangedCallback(evt =>
            {
                if (_dive_through_toggle.value)
                {
                    _tileDataModel.passType = TileDataModel.PassType.CanPassNormally;
                    _dive_through_toggle.value = false;
                    _pass_through_toggle.SetEnabled(false);
                    _dive_through_toggle.SetEnabled(true);
                    _onClickRegisterBtn?.Invoke();
                }
            });

            if (_passTypeDictionary.IndexOf(_tileDataModel.passType.ToString()) == 1)
                _dive_through_toggle.value = true;
            _dive_through_toggle.RegisterValueChangedCallback(evt =>
            {
                if (_pass_through_toggle.value)
                {
                    _tileDataModel.passType = TileDataModel.PassType.CanPassUnder;
                    _pass_through_toggle.value = false;
                    _pass_through_toggle.SetEnabled(true);
                    _dive_through_toggle.SetEnabled(false);
                    _onClickRegisterBtn?.Invoke();
                }
            });


            _up_toggle.value = _tileDataModel.isPassTop;
            _up_toggle.RegisterValueChangedCallback(evt =>
            {
                _tileDataModel.isPassTop = evt.newValue;
                _onClickRegisterBtn?.Invoke();
            });
            _left_toggle.value = _tileDataModel.isPassLeft;
            _left_toggle.RegisterValueChangedCallback(evt =>
            {
                _tileDataModel.isPassLeft = evt.newValue;
                _onClickRegisterBtn?.Invoke();
            });
            _right_toggle.value = _tileDataModel.isPassRight;
            _right_toggle.RegisterValueChangedCallback(evt =>
            {
                _tileDataModel.isPassRight = evt.newValue;
                _onClickRegisterBtn?.Invoke();
            });
            _down_toggle.value = _tileDataModel.isPassBottom;
            _down_toggle.RegisterValueChangedCallback(evt =>
            {
                _tileDataModel.isPassBottom = evt.newValue;
                _onClickRegisterBtn?.Invoke();
            });

            var againEnterBan = false;

            if (_tileDataModel.isLadder)
                _ladder_on_toggle.value = true;
            else
                _ladder_off_toggle.value = true;
            _ladder_on_toggle.RegisterValueChangedCallback(evt =>
            {
                if (!_ladder_on_toggle.value && !againEnterBan)
                {
                    _ladder_on_toggle.value = false;
                    _ladder_off_toggle.value = true;
                }

                if (_ladder_on_toggle.value)
                {
                    _ladder_on_toggle.value = true;
                    _ladder_off_toggle.value = false;
                    againEnterBan = true;
                    _tileDataModel.isLadder = true;
                }
                else
                {
                    againEnterBan = false;
                }

                _onClickRegisterBtn?.Invoke();
            });
            _ladder_off_toggle.RegisterValueChangedCallback(evt =>
            {
                if (!_ladder_off_toggle.value && !againEnterBan)
                {
                    _ladder_on_toggle.value = true;
                    _ladder_off_toggle.value = false;
                }

                if (_ladder_off_toggle.value)
                {
                    _ladder_on_toggle.value = false;
                    _ladder_off_toggle.value = true;
                    againEnterBan = true;
                    _tileDataModel.isLadder = false;
                }
                else
                {
                    againEnterBan = false;
                }

                _onClickRegisterBtn?.Invoke();
            });


            if (_tileDataModel.isBush)
                _thicket_on_toggle.value = true;
            else
                _thicket_off_toggle.value = true;
            _thicket_on_toggle.RegisterValueChangedCallback(evt =>
            {
                if (!_thicket_on_toggle.value && !againEnterBan)
                {
                    _thicket_on_toggle.value = false;
                    _thicket_off_toggle.value = true;
                }

                if (_thicket_on_toggle.value)
                {
                    _thicket_on_toggle.value = true;
                    _thicket_off_toggle.value = false;
                    againEnterBan = true;
                    _tileDataModel.isBush = true;
                }
                else
                {
                    againEnterBan = false;
                }

                _onClickRegisterBtn?.Invoke();
            });
            _thicket_off_toggle.RegisterValueChangedCallback(evt =>
            {
                if (!_thicket_off_toggle.value && !againEnterBan)
                {
                    _thicket_on_toggle.value = true;
                    _thicket_off_toggle.value = false;
                }

                if (_thicket_off_toggle.value)
                {
                    _thicket_on_toggle.value = false;
                    _thicket_off_toggle.value = true;
                    againEnterBan = true;
                    _tileDataModel.isBush = false;
                }
                else
                {
                    againEnterBan = false;
                }

                _onClickRegisterBtn?.Invoke();
            });


            if (_tileDataModel.isCounter)
                _counter_on_toggle.value = true;
            else
                _counter_off_toggle.value = true;
            _counter_on_toggle.RegisterValueChangedCallback(evt =>
            {
                if (!_counter_on_toggle.value && !againEnterBan)
                {
                    _counter_on_toggle.value = false;
                    _counter_off_toggle.value = true;
                }

                if (_counter_on_toggle.value)
                {
                    _counter_on_toggle.value = true;
                    _counter_off_toggle.value = false;
                    againEnterBan = true;
                    _tileDataModel.isCounter = true;
                }
                else
                {
                    againEnterBan = false;
                }

                _onClickRegisterBtn?.Invoke();
            });
            _counter_off_toggle.RegisterValueChangedCallback(evt =>
            {
                if (!_counter_off_toggle.value && !againEnterBan)
                {
                    _counter_on_toggle.value = true;
                    _counter_off_toggle.value = false;
                }

                if (_counter_off_toggle.value)
                {
                    _counter_on_toggle.value = false;
                    _counter_off_toggle.value = true;
                    againEnterBan = true;
                    _tileDataModel.isCounter = false;
                }
                else
                {
                    againEnterBan = false;
                }

                _onClickRegisterBtn?.Invoke();
            });


            if (_tileDataModel.isDamageFloor)
                _damaged_floor_on_toggle.value = true;
            else
                _damaged_floor_off_toggle.value = true;
            _damaged_floor_on_toggle.RegisterValueChangedCallback(evt =>
            {
                if (!_damaged_floor_on_toggle.value && !againEnterBan)
                {
                    _damaged_floor_on_toggle.value = false;
                    _damaged_floor_off_toggle.value = true;
                }

                if (_damaged_floor_on_toggle.value)
                {
                    _damaged_floor_on_toggle.value = true;
                    _damaged_floor_off_toggle.value = false;
                    againEnterBan = true;
                    _tileDataModel.isDamageFloor = true;
                }
                else
                {
                    againEnterBan = false;
                }

                _onClickRegisterBtn?.Invoke();
            });
            _damaged_floor_off_toggle.RegisterValueChangedCallback(evt =>
            {
                if (!_damaged_floor_off_toggle.value && !againEnterBan)
                {
                    _damaged_floor_on_toggle.value = true;
                    _damaged_floor_off_toggle.value = false;
                }

                if (_damaged_floor_off_toggle.value)
                {
                    _damaged_floor_on_toggle.value = false;
                    _damaged_floor_off_toggle.value = true;
                    againEnterBan = true;
                    _tileDataModel.isDamageFloor = false;
                }
                else
                {
                    againEnterBan = false;
                }

                _onClickRegisterBtn?.Invoke();
            });

            if (_tileDataModel.damageFloorType == TileDataModel.DamageFloorType.Fix)
            {
                _constant_toggle.value = true;
                _constant_floatField.SetEnabled(true);
                _percentage_rate_floatField.SetEnabled(false);
            }
            else
            {
                _percentage_rate_toggle.value = true;
                _constant_floatField.SetEnabled(false);
                _percentage_rate_floatField.SetEnabled(true);
            }

            _constant_toggle.RegisterValueChangedCallback(evt =>
            {
                if (!_constant_toggle.value && !againEnterBan)
                {
                    _constant_toggle.value = false;
                    _percentage_rate_toggle.value = true;
                }

                if (_constant_toggle.value)
                {
                    _constant_toggle.value = true;
                    _percentage_rate_toggle.value = false;
                    againEnterBan = true;
                    _tileDataModel.damageFloorType = TileDataModel.DamageFloorType.Fix;
                }
                else
                {
                    againEnterBan = false;
                }

                _onClickRegisterBtn?.Invoke();
            });
            _percentage_rate_toggle.RegisterValueChangedCallback(evt =>
            {
                if (!_percentage_rate_toggle.value && !againEnterBan)
                {
                    _constant_toggle.value = true;
                    _percentage_rate_toggle.value = false;
                }

                if (_percentage_rate_toggle.value)
                {
                    _constant_toggle.value = false;
                    _percentage_rate_toggle.value = true;
                    againEnterBan = true;
                    _tileDataModel.damageFloorType = TileDataModel.DamageFloorType.Rate;
                }
                else
                {
                    againEnterBan = false;
                }

                _onClickRegisterBtn?.Invoke();
            });


            _constant_floatField.value = _tileDataModel.damageFloorValue;
            _constant_floatField.RegisterValueChangedCallback(evt =>
            {
                _tileDataModel.damageFloorValue = evt.newValue;
                _onClickRegisterBtn?.Invoke();
            });

            _percentage_rate_floatField.value = _tileDataModel.damageFloorValue;
            _percentage_rate_floatField.RegisterValueChangedCallback(evt =>
            {
                _tileDataModel.damageFloorValue = evt.newValue;
                _onClickRegisterBtn?.Invoke();
            });
        }

        private void ToggleProcess(bool data, Toggle toggle1, Toggle toggle2) {
            var againEnterBan = false;
            if (data)
                toggle1.value = true;
            else
                toggle2.value = true;
            toggle1.RegisterCallback<FocusOutEvent>(evt =>
            {
                if (!toggle1.value && !againEnterBan)
                {
                    toggle1.value = false;
                    toggle2.value = true;
                }

                if (toggle1.value)
                {
                    toggle1.value = true;
                    toggle2.value = false;
                    againEnterBan = true;
                    data = true;
                }
                else
                {
                    againEnterBan = false;
                }

                _onClickRegisterBtn?.Invoke();
            });
            toggle2.RegisterCallback<FocusOutEvent>(evt =>
            {
                if (!toggle2.value && !againEnterBan)
                {
                    toggle1.value = true;
                    toggle2.value = false;
                }

                if (toggle2.value)
                {
                    toggle1.value = false;
                    toggle2.value = true;
                    againEnterBan = true;
                    data = false;
                }
                else
                {
                    againEnterBan = false;
                }

                _onClickRegisterBtn?.Invoke();
            });
        }
    }
}