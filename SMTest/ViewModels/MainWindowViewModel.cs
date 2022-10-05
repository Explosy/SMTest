using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;
using SMTest.Models.DB;
using SMTest.Views;

namespace SMTest.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
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
            }
        }

        #endregion
        #region Pickets
        public ObservableCollection<FreePicket> FreePickets { get; set; }
        #endregion
        public MainWindowViewModel()
        {
            WareHouses = new ObservableCollection<WareHouse>();
            Areas = new ObservableCollection<Area>();
            FreePickets = new ObservableCollection<FreePicket>();

            #region Create Commands
            LoadWareHouseCommand = new ActionCommand(OnLoadWareHouseCommandExecuted, CanLoadWareHouseCommandExecute);
            LoadAreasCommand = new ActionCommand(OnLoadAreasCommandExecuted, CanLoadAreasCommandExecute);
            LoadFreePicketsCommand = new ActionCommand(OnLoadFreePicketsCommandExecuted, CanLoadFreePicketsCommandExecute);

            AddWareHouseCommand = new ActionCommand(OnAddWareHouseCommandExecuted, CanAddWareHouseCommandExecute);
            DeleteWareHouseCommand = new ActionCommand(OnDeleteWareHouseCommandExecuted, CanDeleteWareHouseCommandExecute);
            
            #endregion

        }

        #region Commands

        #region LoadWareHouseCommand
        public ICommand LoadWareHouseCommand { get; }
        private bool CanLoadWareHouseCommandExecute(object arg) => true;
        private void OnLoadWareHouseCommandExecuted(object arg)
        {
            //Обнуление поля имени нового склада
            WareHouseName = "";
            WareHouses.Clear();
            using (SoftMasterDBContext context = new SoftMasterDBContext())
            {
                WareHouses.AddRange(context.WareHouses);
            }
        }
        #endregion

        #region LoadAreasCommand
        public ICommand LoadAreasCommand { get; }
        private bool CanLoadAreasCommandExecute(object arg) => !(arg==null);
        private void OnLoadAreasCommandExecuted(object arg)
        {
            Areas.Clear();
            using (SoftMasterDBContext context = new SoftMasterDBContext())
            {
                Areas.AddRange(context.Areas.Where(a => a.WareHouseId.Equals(selectedWareHouse.WareHouseId)));
            }
        }
        #endregion

        #region LoadFreePicketsCommand
        public ICommand LoadFreePicketsCommand { get; }
        private bool CanLoadFreePicketsCommandExecute(object arg) => !(arg == null);
        private void OnLoadFreePicketsCommandExecuted(object arg)
        {
            using (SoftMasterDBContext context = new SoftMasterDBContext())
            {
                FreePickets.AddRange(context.FreePickets.Where(a => a.WareHouseId.Equals(selectedWareHouse.WareHouseId)));
            }
        }
        #endregion

        #region AddWareHouseCommand
        public ICommand AddWareHouseCommand { get; }
        private bool CanAddWareHouseCommandExecute(object arg) => !Equals((string)arg, "");
        private void OnAddWareHouseCommandExecuted(object arg)
        {
            //Создание диалогового окна добавления нового склада
            WareHouseAddWindow wareHouseAddWindow = new WareHouseAddWindow();
            if (wareHouseAddWindow.ShowDialog() == true)
            {
                int idNewWareHouse;
                bool resultFlag = AddWareHouse((string)arg, out idNewWareHouse);
                if (resultFlag)
                {
                    resultFlag = AddFreePickets(idNewWareHouse, wareHouseAddWindow.FirstPicketNumber, wareHouseAddWindow.PicketsCount);
                }
                if (resultFlag) MessageBox.Show("Склад успешно добавлен", "Результат");
                else MessageBox.Show("В процессе добавления склада произошли ошибки.", "Результат");
            }
            else MessageBox.Show("Операция добавления склада отменена","Результат");

            clearAllCollection(); 
        }
        #endregion

        #region DeleteWareHouseCommand
        public ICommand DeleteWareHouseCommand { get; }
        private bool CanDeleteWareHouseCommandExecute(object arg) => !(selectedWareHouse == null);
        private void OnDeleteWareHouseCommandExecuted(object arg)
        {
            try
            {
                //Попытка удаления склада из БД
                using (SoftMasterDBContext context = new SoftMasterDBContext())
                {
                    context.WareHouses.Remove((WareHouse)arg);
                    context.SaveChanges();
                }
                //Обновление данных о складах
                clearAllCollection();            
            }
            catch (Exception e)
            {
                MessageBox.Show("В процессе удаления склада произошли ошибки.", "Результат");
                MessageBox.Show(e.Message, "Результат");
            }
        }
        #endregion

        #endregion

        #region Support Function
        
        /// <summary>
        /// Функция обнуления содержимого всех коллекция
        /// </summary>
        private void clearAllCollection()
        {
            WareHouses.Clear();
            Areas.Clear();
            FreePickets.Clear();
        }
        
        #region AddWareHouse
        /// <summary>
        /// Функция добавления в БД нового склада
        /// </summary>
        /// <param name="wareHouseName">Имя нового склада</param>
        /// <returns>Возвращает True в случае успеха добавления, иначе False</returns>
        private bool AddWareHouse(string wareHouseName, out int idNewWareHouse)
        {
            idNewWareHouse = 0;
            try
            {
                WareHouse newWareHouse = new WareHouse() { Title = wareHouseName };
                using (SoftMasterDBContext context = new SoftMasterDBContext())
                {
                    context.WareHouses.Add(newWareHouse);
                    context.SaveChanges();
                }
                using (SoftMasterDBContext context = new SoftMasterDBContext())
                {
                    idNewWareHouse = context.WareHouses.Where(w => w.Title.Equals(wareHouseName))
                                      .FirstOrDefault()!.WareHouseId;
                }
                return true;
            }
            catch (DbUpdateException e)
            {
                MessageBox.Show(e.Message, "Ошибка");
                return false;
            }
            
        }
        #endregion

        #region AddFreePicket
        /// <summary>
        /// Функция добавления в БД свободных пикетов при создании нового склада
        /// </summary>
        /// <param name="wareHouseId">Id нового склада</param>
        /// <param name="firstPicketNumber">Порядковый номер первого пикета</param>
        /// <param name="picketsCount">Количество пикетов в новом складе</param>
        /// <returns></returns>
        private bool AddFreePickets(int wareHouseId, int firstPicketNumber, int picketsCount)
        {
            try
            {
                FreePicket[] newFreePickets = new FreePicket[picketsCount];
                for (int count=0; count < picketsCount; count++)
                {
                    newFreePickets[count] = new FreePicket()
                    {
                        PicketNumber = firstPicketNumber + count,
                        WareHouseId = wareHouseId
                    };
                }
                using (SoftMasterDBContext context = new SoftMasterDBContext())
                {
                    context.FreePickets.AddRange(newFreePickets);
                    context.SaveChanges();
                }
                return true;
            }
            catch (DbUpdateException e)
            {
                MessageBox.Show(e.Message, "Ошибка");
                return false;
            }
        }
        #endregion

        #endregion

        #region TextBoxValue
        private string wareHouseName = "";
        public string WareHouseName
        {
            get => wareHouseName;
            set => SetProperty(ref wareHouseName, value);
        }

        private string areaName = "";
        public string AreaName
        {
            get => areaName;
            set => SetProperty(ref areaName, value);
        }
        #endregion
    }
}
