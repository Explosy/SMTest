using System;
using System.Windows;
using SMTest.ViewModels;

namespace SMTest.Views
{
    /// <summary>
    /// Логика взаимодействия для HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(DateTime date)
        {
            InitializeComponent();
            DataContext = new HistoryWindowViewModel(new DateTime(2022, 10, 4, 14, 0, 0));
        }
    }
}
