using RPGMaker.Codebase.CoreSystem.Service.MapManagement.Repository;
using RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement.Repository;
using RPGMaker.Codebase.CoreSystem.Service.EventManagement.Repository;
using RPGMaker.Codebase.CoreSystem.Service.OutlineManagement.Repository;

namespace RPGMaker.Codebase.Editor.Common
{
    /// <summary>
    /// 本処理はリリース時には、クラスごと不要となる
    /// </summary>
    internal static class RepositoryUpdateHelper {

        /// <summary>
        ///     Repository Update  (for Development)
        /// </summary>
        internal static void ApplyRepositoryUpdates() {

        // script defineでやっても再コンパイルが走って遅くなるだけなので外す
#if ENABLE_DEVELOPMENT_FIX
            new EventRepository().OldEvent();

            new SkillCustomRepository().OldSkill();

            new ItemRepository().OldItem();

            new CoreSystem.Service.MapManagement.Repository.MapRepository().MapJsonFix();
            new CoreSystem.Service.MapManagement.Repository.MapRepository().MapSampleJsonFix();
            
            //以下、各種翻訳データの適用
            //MAP
            new CoreSystem.Service.MapManagement.Repository.MapRepository().MapJsonTranslation();
            //アクター
            new CharacterActorRepository().JsonTranslation();
            //敵
            new EnemyRepository().JsonTranslation();
            new EnemyRepository().EnemyRatingFix();
            //敵グループ
            new TroopRepository().JsonTranslation();
            //スキル
            new SkillCustomRepository().JsonTranslation();
            //職業
            new ClassRepository().JsonTranslation();
            //乗り物
            new VehicleRepository().JsonTranslation();
            //アイテム
            new ItemRepository().JsonTranslation();
            //武器
            new WeaponRepository().JsonTranslation();
            //武器の装備タイプ
            new WeaponRepository().SetWeaponEquipType();
            //防具
            new ArmorRepository().JsonTranslation();
            //ステート
            new StateRepository().JsonTranslation();
            //ステートの重ね合わせ
            new StateRepository().OverRayConvert();
            //Animation
            new AnimationRepository().JsonTranslation();
            //Event
            //イベントはファイルを書き換えるのみのため、本作業実施時後にUnity自体を再起動すること
            new EventCommonRepository().JsonTranslation();
            new EventMapRepository().JsonTranslation();
            new EventRepository().JsonTranslation();
            //AssetManage
            //System
            //Title
            new TileRepository().JsonTranslation();
            //Outline
            new OutlineRepository().JsonTranslation();

            //new MapRepository().MapFixForEditor();
            //new MapRepository().MapTileFixForEditor();

            TileRepository.FixImageSize();
#endif
        }
    }
}