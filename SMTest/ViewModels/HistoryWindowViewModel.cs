using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;
using SMTest.Models;
using SMTest.Models.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SMTest.ViewModels
{
    internal class HistoryWindowViewModel : BindableBase
    {
        private DateTime currentHistoryDate; //Дата, на которую выгружается история
        
        private List<FreePicketsHistory> AllFreePickets; //Лист для выгрузки всех свободных пикетов на указанную дату
        
        private List<BusyPicketsHistory> AllBusyPickets; //Лист для выгрузки всех занятых пикетов на указанную дату
        
        private List<Area> AllAreas; //Лист для выгрузки всех площадок, соответствующих пикетам указанным выше

        #region WareHouse
        public ObservableCollection<WareHouse> WareHouses { get; set; }

        /// <summary>
        /// Выбранный склад
        /// </summary>
        private WareHouse selectedWareHouse;
        public WareHouse SelectedWareHouse
        {
            get => selectedWareHouse;
            set
            {
                SetProperty(ref selectedWareHouse, value);
                BusyPickets.Clear();
                Areas.Clear();
                FreePickets.Clear();
            }
        }
        #endregion
        
        #region Areas
        public ObservableCollection<Area> Areas { get; set; }

        /// <summary>
        /// Выбранная площадка
        /// </summary>
        private Area selectedArea;
        public Area SelectedArea
        {
            get => selectedArea;
            set
            {
                BusyPickets.Clear();
                SetProperty(ref selectedArea, value);
                AreaCargo = AllBusyPickets.Where(p => p.AreaId == value?.AreaId).Sum(p => p.Cargo).ToString();
            }
        }
        /// <summary>
        /// Суммарный вес груза на площадке
        /// </summary>
        private string areaCargo = "";
        public string AreaCargo
        {
            get => areaCargo;
            set
            {
                SetProperty(ref areaCargo, value);
            }
        }
        #endregion

        #region PicketsHistory
        public ObservableCollection<FreePicketsHistory> FreePickets { get; set; }
        public ObservableCollection<BusyPicketsHistory> BusyPickets { get; set; }

        /// <summary>
        /// Выбранный пикет
        /// </summary>
        private IPicket selectedPicket;
        public IPicket SelectedPicket
        {
            get => selectedPicket;
            set => SetProperty(ref selectedPicket, value);
        }

        #endregion

        internal HistoryWindowViewModel(DateTime date)
        {
            currentHistoryDate = date;

            using (SoftMasterDBContext context = new SoftMasterDBContext())
            {
                AllFreePickets = context.FreePicketsHistories.Where(p => p.DateStart < currentHistoryDate && p.DateEnd > currentHistoryDate).ToList();
                AllBusyPickets = context.BusyPicketsHistories.Where(p => p.DateStart < currentHistoryDate && p.DateEnd > currentHistoryDate).ToList();

                var WareHouseId = AllFreePickets.Select(p => p.WareHouseId).Distinct();
                WareHouses = new ObservableCollection<WareHouse>(context.WareHouses.Where(w => WareHouseId.Contains(w.WareHouseId)));

                var AreaId = AllBusyPickets.Select(p => p.AreaId).Distinct();
                AllAreas = context.Areas.Where(a => AreaId.Contains(a.AreaId)).ToList();

                Areas = new ObservableCollection<Area>();
                FreePickets = new ObservableCollection<FreePicketsHistory>();
                BusyPickets = new ObservableCollection<BusyPicketsHistory>();
            }

            #region Create Commands
            LoadAreasCommand = new ActionCommand(OnLoadAreasCommandExecuted, CanLoadAreasCommandExecute);
            LoadFreePicketsCommand = new ActionCommand(OnLoadFreePicketsCommandExecuted, CanLoadFreePicketsCommandExecute);
            LoadBusyPicketsCommand = new ActionCommand(OnLoadBusyPicketsCommandExecuted, CanLoadBusyPicketsCommandExecute);
            #endregion

        }
        #region Commands
        #region LoadAreasCommand
        public ICommand LoadAreasCommand { get; }
        private bool CanLoadAreasCommandExecute(object arg) => !(arg == null);
        private void OnLoadAreasCommandExecuted(object arg)
        {
            Areas.Clear();
            Areas.AddRange(AllAreas.Where(a => a.WareHouseId == SelectedWareHouse.WareHouseId));
        }
        #endregion

        #region LoadFreePicketsCommand
        public ICommand LoadFreePicketsCommand { get; }
        private bool CanLoadFreePicketsCommandExecute(object arg) => !(arg == null);
        private void OnLoadFreePicketsCommandExecuted(object arg)
        {
            FreePickets.Clear();
            FreePickets.AddRange(AllFreePickets.Where(p => p.WareHouseId == SelectedWareHouse.WareHouseId));
        }
        #endregion

        #region LoadBusyPicketsCommand
        public ICommand LoadBusyPicketsCommand { get; }
        private bool CanLoadBusyPicketsCommandExecute(object arg) => !(arg == null);
        private void OnLoadBusyPicketsCommandExecuted(object arg)
        {
            BusyPickets.Clear();
            BusyPickets.AddRange(AllBusyPickets.Where(p => p.AreaId == SelectedArea.AreaId));
        }
        #endregion

        #endregion
    }
}
