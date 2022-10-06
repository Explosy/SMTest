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
            DataContext = new HistoryWindowViewModel(date);
        }
    }
}
