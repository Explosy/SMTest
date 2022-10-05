using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using SMTest.Models.DB;

namespace SMTest.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        #region WareHouse
        public ObservableCollection<WareHouse> WareHouses { get; set; }
        
        private WareHouse selectedWareHouse;
        public WareHouse SelectedWareHouse
        {
            get => selectedWareHouse;
            set
            {
                SetProperty(ref selectedWareHouse, value);
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            using (SoftMasterDBContext context = new SoftMasterDBContext())
            {
                WareHouses = new ObservableCollection<WareHouse>(context.WareHouses);
            }
        }
    }
}
