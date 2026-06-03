using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models; // Sử dụng đúng thư mục Models của WpfApp1

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // LƯU Ý: Nếu tên file DbContext trong thư mục Models của bạn 
        // không phải là "KthpSinhVienContext", hãy đổi tên nó lại cho đúng ở đây.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using var db = new KthpSinhVienContext();

            cboKhoa.ItemsSource = db.KhoaTts.ToList();
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;

            LoadData();
        }

        private void LoadData()
        {
            using var db = new KthpSinhVienContext();

            var list = db.SinhViens
                         .Include(sv => sv.MaKhoaNavigation)
                         .OrderByDescending(sv => sv.SoLanXs)
                         .ToList();

            dgDanhSach.ItemsSource = list.Select(sv => new
            {
                sv.MaSv,
                sv.HoTen,
                sv.SoLanXs,
                sv.MaKhoa,
                TenKhoa = sv.MaKhoaNavigation != null ? sv.MaKhoaNavigation.TenKhoa : "",
                TienThuong = sv.SoLanXs >= 5 ? 500000.0 : (sv.SoLanXs > 1 ? 200000.0 : 0.0)
            }).ToList();
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtMaSv.Text.Trim(), out int maSv))
            {
                MessageBox.Show("Mã SV phải là số nguyên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(txtSoLanXs.Text.Trim(), out int soLanXs))
            {
                MessageBox.Show("Số lần XS phải là số nguyên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (soLanXs < 1)
            {
                MessageBox.Show("Số lần XS phải >= 1!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var db = new KthpSinhVienContext();
            if (db.SinhViens.Any(sv => sv.MaSv == maSv))
            {
                MessageBox.Show($"Mã SV {maSv} đã tồn tại trong hệ thống!", "Trùng dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double tien = soLanXs >= 5 ? 500000.0 : (soLanXs > 1 ? 200000.0 : 0.0);

            SinhVien svMoi = new SinhVien
            {
                MaSv = maSv,
                HoTen = txtHoTen.Text.Trim(),
                SoLanXs = soLanXs,
                TienThuong = tien,
                MaKhoa = cboKhoa.SelectedValue != null ? (int)cboKhoa.SelectedValue : 1
            };

            db.SinhViens.Add(svMoi);
            db.SaveChanges();

            MessageBox.Show("Thêm sinh viên mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadData();
        }

        private void dgDanhSach_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDanhSach.SelectedItem == null) return;

            dynamic row = dgDanhSach.SelectedItem;
            txtMaSv.Text = row.MaSv.ToString();
            txtHoTen.Text = row.HoTen;
            txtSoLanXs.Text = row.SoLanXs.ToString();
            cboKhoa.SelectedValue = row.MaKhoa;
        }

        private void btnTim_Click(object sender, RoutedEventArgs e)
        {
            using var db = new KthpSinhVienContext();

            var filtered = db.SinhViens
                             .Include(sv => sv.MaKhoaNavigation)
                             .Where(sv => sv.MaKhoa == 1)
                             .OrderByDescending(sv => sv.SoLanXs)
                             .ToList();

            Window2 w2 = new Window2(filtered);
            w2.Owner = this;
            w2.ShowDialog();
        }
    }
}