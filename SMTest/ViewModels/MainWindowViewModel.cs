using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using SMTest.Models;
using SMTest.Models.DB;
using System.Linq;
using SMTest.Views;
using System.Windows;
using System.Windows.Input;

namespace SMTest.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
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
                clearAllCollection();
                SetProperty(ref selectedWareHouse, value);
                if (value == null) return;
                UpdateWareHouseInfo(value);
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
                SetProperty(ref selectedArea, value);
                if (value == null)
                {
                    AreaCargo = "";
                    return;
                }
                UpdateAreaInfo(value);
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

        #region Pickets
        public ObservableCollection<FreePicket> FreePickets { get; set; }
        public ObservableCollection<BusyPicket> BusyPickets { get; set; }

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

        #region DatePicker
        /// <summary>
        /// Выбранная пользователем дата для просмотра истории
        /// </summary>
        private DateTime selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get => selectedDate;
            set => SetProperty(ref selectedDate, value);
        }
        #endregion
        public MainWindowViewModel()
        {
            #region CreateCollection
            //Создание коллекций для отображения складов, площадок, пикетов.
            WareHouses = new ObservableCollection<WareHouse>();
            Areas = new ObservableCollection<Area>();
            BusyPickets = new ObservableCollection<BusyPicket>();
            FreePickets = new ObservableCollection<FreePicket>();
            #endregion

            #region Create Commands

            //Команды загрузки данных о складах из БД
            LoadWareHouseCommand = new ActionCommand(OnLoadWareHouseCommandExecuted, CanLoadWareHouseCommandExecute);
            
            #region Add/Delete Commands
            //Команды добавление/удаления объектов из БД
            AddWareHouseCommand = new ActionCommand(OnAddWareHouseCommandExecuted, CanAddWareHouseCommandExecute);
            DeleteWareHouseCommand = new ActionCommand(OnDeleteWareHouseCommandExecuted, CanDeleteWareHouseCommandExecute);
            AddAreaCommand = new ActionCommand(OnAddAreaCommandExecuted, CanAddAreaCommandExecute);
            DeleteAreaCommand = new ActionCommand(OnDeleteAreaCommandExecuted, CanDeleteAreaCommandExecute);
            #endregion

            ChangePicketCargoCommand = new ActionCommand(OnChangePicketCargoCommandExecuted, CanChangePicketCargoCommandExecute);

            FreeToBusyPicketMoveCommand = new ActionCommand(OnFreeToBusyPicketMoveCommandExecuted, CanFreeToBusyPicketMoveCommandExecute);

            BusyToFreePicketMoveCommand = new ActionCommand(OnBusyToFreePicketMoveCommandExecuted, CanBusyToFreePicketMoveCommandExecute);

            ShowHistoryCommand = new ActionCommand(OnShowHistoryCommandExecuted, CanShowHistoryCommandExecute);
            #endregion
        }

        #region Commands

        #region LoadWareHouseCommand
        public ICommand LoadWareHouseCommand { get; }
        private bool CanLoadWareHouseCommandExecute(object arg) => true;
        private void OnLoadWareHouseCommandExecuted(object arg)
        {
            WareHouses.Clear();
            using (SoftMasterDBContext context = new SoftMasterDBContext())
            {
                WareHouses.AddRange(context.WareHouses);
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
            else MessageBox.Show("Операция добавления склада отменена", "Результат");

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
                MessageBox.Show("Склад успешно удален", "Результат");
            }
            catch (Exception e)
            {
                MessageBox.Show("В процессе удаления склада произошли ошибки.", "Результат");
                MessageBox.Show(e.Message, "Результат");
            }
        }
        #endregion

        #region AddAreaCommand
        public ICommand AddAreaCommand { get; }
        private bool CanAddAreaCommandExecute(object arg) => !Equals((string)arg, "");
        private void OnAddAreaCommandExecuted(object arg)
        {
            Area newArea = new Area() { Title = (string)arg, WareHouseId = selectedWareHouse.WareHouseId };
            AreaName = "";
            try
            {
                using (SoftMasterDBContext context = new SoftMasterDBContext())
                {
                    context.Areas.Add(newArea);
                    context.SaveChanges();
                }
                clearAllCollection();
                MessageBox.Show("Площадка успешно добавлена", "Результат");
            }
            catch (Exception e)
            {
                MessageBox.Show("В процессе добавления площадки произошли ошибки.", "Результат");
                MessageBox.Show(e.Message, "Ошибка");
            }
        }
        #endregion

        #region DeleteAreaCommand
        public ICommand DeleteAreaCommand { get; }
        private bool CanDeleteAreaCommandExecute(object arg) => !(selectedArea == null);
        private void OnDeleteAreaCommandExecuted(object arg)
        {
            try
            {
                using (SoftMasterDBContext context = new SoftMasterDBContext())
                {
                    context.Areas.Remove((Area)arg);
                    context.SaveChanges();
                }

                clearAllCollection();
                MessageBox.Show("Площадка успешно удалена", "Результат");
            }
            catch (Exception e)
            {
                MessageBox.Show("В процессе удаления площадки произошли ошибки.", "Результат");
                MessageBox.Show(e.Message, "Результат");
            }
        }
        #endregion

        #region ChangePicketCargoCommand
        public ICommand ChangePicketCargoCommand { get; }
        private bool CanChangePicketCargoCommandExecute(object arg) => !(SelectedPicket == null);
        private void OnChangePicketCargoCommandExecuted(object arg)
        {
            try
            {
                using (SoftMasterDBContext context = new SoftMasterDBContext())
                {
                    if (SelectedPicket is BusyPicket)
                    {
                        IPicket picket = context.BusyPickets.Find(SelectedPicket.PicketId);
                        BusyPicketsHistory oldPicket = new BusyPicketsHistory()
                        {
                            Cargo = picket.Cargo,
                            PicketNumber = picket.PicketNumber,
                            AreaId = ((BusyPicket)picket).AreaId,
                            DateStart = picket.DateStart
                        };
                        context.BusyPicketsHistories.Add(oldPicket);
                        picket.Cargo = int.Parse((string)arg);
                        picket.DateStart = DateTime.Now;
                    }
                    else if (SelectedPicket is FreePicket)
                    {
                        IPicket picket = context.FreePickets.Find(SelectedPicket.PicketId);
                        FreePicketsHistory oldPicket = new FreePicketsHistory()
                        {
                            Cargo = picket.Cargo,
                            PicketNumber = picket.PicketNumber,
                            WareHouseId = ((FreePicket)picket).WareHouseId,
                            DateStart = picket.DateStart
                        };
                        context.FreePicketsHistories.Add(oldPicket);
                        picket.Cargo = int.Parse((string)arg);
                        picket.DateStart = DateTime.Now;
                    }
                    context.SaveChanges();
                    SelectedPicket = null;
                }
                MessageBox.Show("Вес успешно изменен", "Результат");
            }
            catch (Exception e)
            {
                MessageBox.Show("В процессе изменения веса произошли ошибки.", "Результат");
                MessageBox.Show(e.Message, "Результат");
            }
        }
        #endregion

        #region FreeToBusyPicketMoveCommand
        public ICommand FreeToBusyPicketMoveCommand { get; }
        private bool CanFreeToBusyPicketMoveCommandExecute(object arg)
        {
            if (SelectedArea == null) return false;
            if (BusyPickets.Count == 0) return true;
            if (SelectedPicket is FreePicket)
            {
                var lastPicket = BusyPickets.Max(p => p.PicketNumber);
                var firstPicket = BusyPickets.Min(p => p.PicketNumber);
                if (SelectedPicket.PicketNumber - lastPicket == 1 ||
                    firstPicket - SelectedPicket.PicketNumber == 1) return true;
            }
            return false;
        }
        private void OnFreeToBusyPicketMoveCommandExecuted(object arg)
        {
            try
            {
                using (SoftMasterDBContext context = new SoftMasterDBContext())
                {
                    FreePicket picket = context.FreePickets.Find(SelectedPicket.PicketId);
                    var newBusyPicket = new BusyPicket()
                    {
                        AreaId = selectedArea.AreaId,
                        Cargo = picket.Cargo,
                        PicketNumber = picket.PicketNumber
                    };
                    context.BusyPickets.Add(newBusyPicket);

                    FreePicketsHistory oldPicket = new FreePicketsHistory()
                    {
                        Cargo = picket.Cargo,
                        PicketNumber = picket.PicketNumber,
                        WareHouseId = picket.WareHouseId,
                        DateStart = picket.DateStart
                    };
                    context.FreePicketsHistories.Add(oldPicket);

                    context.FreePickets.Remove(picket);
                    context.SaveChanges();
                }
                MessageBox.Show("Пикет успешно перемещен", "Результат");
                SelectedPicket = null;
                clearAllCollection();
                UpdateWareHouseInfo(SelectedWareHouse);
            }
            catch (Exception e)
            {
                MessageBox.Show("В процессе перемещения пикета произошли ошибки.", "Результат");
                MessageBox.Show(e.Message, "Результат");
            }
        }

        #endregion

        #region BusyToFreePicketMove
        public ICommand BusyToFreePicketMoveCommand { get; }
        private bool CanBusyToFreePicketMoveCommandExecute(object arg)
        {
            if (SelectedPicket is BusyPicket && SelectedWareHouse != null)
            {
                var lastPicket = BusyPickets.Max(p => p.PicketNumber);
                var firstPicket = BusyPickets.Min(p => p.PicketNumber);
                if (SelectedPicket.PicketNumber == lastPicket ||
                    SelectedPicket.PicketNumber == firstPicket) return true;
            }
            return false;
        }
        private void OnBusyToFreePicketMoveCommandExecuted(object arg)
        {
            try
            {
                using (SoftMasterDBContext context = new SoftMasterDBContext())
                {
                    BusyPicket picket = context.BusyPickets.Find(SelectedPicket.PicketId);
                    var newFreePicket = new FreePicket()
                    {
                        WareHouseId = selectedArea.WareHouseId,
                        Cargo = picket.Cargo,
                        PicketNumber = picket.PicketNumber
                    };
                    context.FreePickets.Add(newFreePicket);

                    BusyPicketsHistory oldPicket = new BusyPicketsHistory()
                    {
                        Cargo = picket.Cargo,
                        PicketNumber = picket.PicketNumber,
                        AreaId = picket.AreaId,
                        DateStart = picket.DateStart
                    };
                    context.BusyPicketsHistories.Add(oldPicket);

                    context.BusyPickets.Remove(picket);
                    context.SaveChanges();
                }
                MessageBox.Show("Пикет успешно перемещен", "Результат");
                SelectedPicket = null;
                clearAllCollection();
                UpdateWareHouseInfo(SelectedWareHouse);
            }
            catch (Exception e)
            {
                MessageBox.Show("В процессе перемещения пикета произошли ошибки.", "Результат");
                MessageBox.Show(e.Message, "Результат");
            }
        }
        #endregion

        #region ShowHistoryCommand
        public ICommand ShowHistoryCommand { get; }
        private bool CanShowHistoryCommandExecute(object arg) => !(arg == null);
        private void OnShowHistoryCommandExecuted(object arg)
        {
            HistoryWindow historyWindow = new HistoryWindow(SelectedDate);
            historyWindow.Show();
        }
        #endregion
        
        #endregion

        #region Support Function

        /// <summary>
        /// Функция обнуления содержимого всех коллекция
        /// </summary>
        private void clearAllCollection()
        {
            Areas.Clear();
            FreePickets.Clear();
            BusyPickets.Clear();
        }
        
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
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка");
                return false;
            }
        }

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
                for (int count = 0; count < picketsCount; count++)
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
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка");
                return false;
            }
        }
        /// <summary>
        /// Функция обновления данных о складе
        /// </summary>
        /// <param name="wareHouse">Объект склада, о котором нужно обновить информацию</param>
        private void UpdateWareHouseInfo(WareHouse wareHouse)
        {
            try
            {
                using (SoftMasterDBContext context = new SoftMasterDBContext())
                {
                    FreePickets.AddRange(context.FreePickets.Where(p => p.WareHouseId.Equals(wareHouse.WareHouseId)).OrderBy(p => p.PicketNumber));
                    Areas.AddRange(context.Areas.Where(a => a.WareHouseId.Equals(wareHouse.WareHouseId)));
                }
            }
            catch (Exception e)
            {
                FreePickets.Add(new FreePicket() { PicketNumber = 0 });
                Areas.Add(new Area() { Title = "Данные не загружены" });
                MessageBox.Show(e.Message, "Ошибка");
            }
            
        }
        /// <summary>
        /// Функция обновления данных о площадке
        /// </summary>
        /// <param name="area">Объект площадки, о которой нужно обновить информацию</param>
        private void UpdateAreaInfo(Area area)
        {
            try
            {
                using (SoftMasterDBContext context = new SoftMasterDBContext())
                {
                    BusyPickets.Clear();
                    BusyPickets.AddRange(context.BusyPickets.Where(p => p.AreaId.Equals(area.AreaId)).OrderBy(p => p.PicketNumber));
                    AreaCargo = BusyPickets.Sum(p => p.Cargo).ToString();
                }
            }
            catch (Exception e)
            {
                BusyPickets.Add(new BusyPicket() { PicketNumber = 0 });
                MessageBox.Show(e.Message, "Ошибка");
            }
        }

        #endregion

        #region TextBoxValue
        private string wareHouseName = "";
        /// <summary>
        /// Введенное пользователем имя для нового склада
        /// </summary>
        public string WareHouseName
        {
            get => wareHouseName;
            set => SetProperty(ref wareHouseName, value);
        }

        private string areaName = "";
        /// <summary>
        /// Введенное пользователем имя для новой площадки
        /// </summary>
        public string AreaName
        {
            get => areaName;
            set => SetProperty(ref areaName, value);
        }
        #endregion
    }
}
