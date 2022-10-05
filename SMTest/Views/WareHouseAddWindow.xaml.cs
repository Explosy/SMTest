using System.Windows;
using System.Windows.Input;


namespace SMTest.Views
{
    /// <summary>
    /// Логика взаимодействия для WareHouseAddWindow.xaml
    /// </summary>
    public partial class WareHouseAddWindow : Window
    {
        public WareHouseAddWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        /// <summary>
        /// Обработчик, запрещающий вводить в текстовые поля не цифры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back) return;
            else
                e.Handled = true;
        }

        /// <summary>
        /// Свойство, возвращающее введенный пользователем номер первого пикета (Если оно пустое, то возвращает 0)
        /// </summary>
        public int FirstPicketNumber
        {
            get { return FirstPicketNumberBox.Text.Equals("") ? 0 : int.Parse(FirstPicketNumberBox.Text); }
        }
        /// <summary>
        /// Свойство, возвращающее введенное пользователем количество пикетов (Если оно пустое, то возвращает 0)
        /// </summary>
        public int PicketsCount
        {
            get { return PicketsCountNumberBox.Text.Equals("") ? 0 : int.Parse(PicketsCountNumberBox.Text); }
        }

    }
}
