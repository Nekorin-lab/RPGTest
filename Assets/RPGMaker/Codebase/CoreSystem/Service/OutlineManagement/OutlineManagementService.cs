using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Outline;
using RPGMaker.Codebase.CoreSystem.Lib.Auth;
using RPGMaker.Codebase.CoreSystem.Service.OutlineManagement.Repository;

namespace RPGMaker.Codebase.CoreSystem.Service.OutlineManagement
{
    public class OutlineManagementService
    {
        private readonly MapRepository     _mapRepository;
        private readonly OutlineRepository _outlineRepository;

        public OutlineManagementService() {
            _outlineRepository = new OutlineRepository();
            _mapRepository = new MapRepository();
        }

        public List<MapSubDataModel> LoadMaps() {
            return _mapRepository.GetMaps();
        }

        public OutlineDataModel LoadOutline() {
            return _outlineRepository.GetOutline();
        }

        public void SaveOutline(OutlineDataModel outlineDataModel) {
            _outlineRepository.StoreOutline(outlineDataModel);
        }
    }
}