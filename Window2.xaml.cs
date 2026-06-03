using System.Collections.Generic;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class Window2 : Window
    {
        public Window2(List<SinhVien> danhSach)
        {
            InitializeComponent();
            dgDanhSach2.ItemsSource = danhSach;
        }
    }
}