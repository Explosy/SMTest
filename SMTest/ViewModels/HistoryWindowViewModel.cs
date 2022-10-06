using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;
using SMTest.Models.DB;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SMTest.ViewModels
{
    internal class HistoryWindowViewModel : BindableBase
    {
        private DateTime currentHistoryDate;
        
        #region WareHouse
        public ObservableCollection<WareHouse> WareHouses { get; set; }

        private WareHouse selectedWareHouse;
        /// <summary>
        /// Свойство, содержащее информацию о выбранном складе
        /// </summary>
        public WareHouse SelectedWareHouse
        {
            get => selectedWareHouse;
            set
            {
                SetProperty(ref selectedWareHouse, value);
            }
        }
        #endregion
        
        #region Areas
        public ObservableCollection<Area> Areas { get; set; }

        private Area selectedArea;
        /// <summary>
        /// Свойство, содержащее информацию о выбранной площадке
        /// </summary>
        public Area SelectedArea
        {
            get => selectedArea;
            set
            {
                SetProperty(ref selectedArea, value);
                BusyPickets.Clear();
            }
        }
        #endregion

        #region PicketsHistory
        public ObservableCollection<FreePicketsHistory> FreePickets { get; set; }
        public ObservableCollection<BusyPicketsHistory> BusyPickets { get; set; }
        #endregion

        internal HistoryWindowViewModel(DateTime date)
        {
            currentHistoryDate = date;
            using (SoftMasterDBContext context = new SoftMasterDBContext())
            {
                FreePickets = new ObservableCollection<FreePicketsHistory>(context.FreePicketsHistories.Where(p => p.DateStart < currentHistoryDate && 
                                                                                                                    p.DateEnd > currentHistoryDate));
                BusyPickets = new ObservableCollection<BusyPicketsHistory>(context.BusyPicketsHistories.Where(p => p.DateStart < currentHistoryDate &&
                                                                                                                    p.DateEnd > currentHistoryDate));
                var WareHouseId = FreePickets.Select(p => p.WareHouseId).Distinct();
                WareHouses = new ObservableCollection<WareHouse>(context.WareHouses.Where(w => WareHouseId.Contains(w.WareHouseId)));
                var AreaId = BusyPickets.Select(p => p.AreaId).Distinct();
                Areas = new ObservableCollection<Area>(context.Areas.Where(a => AreaId.Contains(a.AreaId)));
            }
        }
    }
}
