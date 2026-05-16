using DAL;
using System;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace QLquannet.Model
{
    public partial class frmPrintRecipe : Form
    {

        private PrintRecipeDAL prDAL;
        public frmPrintRecipe()
        {

            prDAL = new PrintRecipeDAL();
            InitializeComponent();
            LoadBillingData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadBillingData()
        {
            DataTable billingData = prDAL.GetAllBillings();
            dgvBilling.DataSource = billingData;
        }

        private void btnPrintRecipe_Click(object sender, EventArgs e)
        {

            if (dgvBilling.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để xuất!");
                return;
            }

            // 1. Lấy thông tin hóa đơn được chọn
            DataGridViewRow selectedRow = dgvBilling.SelectedRows[0];
            int billingID = Convert.ToInt32(selectedRow.Cells["BillingID"].Value);

            // 2. Lấy dữ liệu chi tiết từ Database (giữ nguyên logic DAL của bạn)
            DataTable billingInfo = prDAL.GetBillingInfo(billingID);
            DataTable foodDetails = prDAL.GetFoodDetails(billingID);

            if (billingInfo.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy dữ liệu cho hóa đơn này.");
                return;
            }

            // 3. Sử dụng SaveFileDialog để người dùng chọn nơi lưu file
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV Files (*.csv)|*.csv";
            sfd.FileName = "HoaDon_" + billingID + ".csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    // --- Phần đầu hóa đơn ---
                    sb.AppendLine("HOA DON THANH TOAN");
                    sb.AppendLine("Cua hang: NET CO");
                    sb.AppendLine("Ma hoa don: " + billingID);
                    sb.AppendLine("Ngay: " + billingInfo.Rows[0]["Date"].ToString());
                    sb.AppendLine("Nhan vien: " + billingInfo.Rows[0]["LastName"].ToString());
                    sb.AppendLine(); // Dòng trống

                    // --- Tiêu đề bảng hàng hóa (Ngăn cách bởi dấu phẩy) ---
                    sb.AppendLine("STT,Ten Mon,So Luong,Don Gia,Thanh Tien");

                    // --- Chi tiết các món ăn ---
                    for (int i = 0; i < foodDetails.Rows.Count; i++)
                    {
                        string stt = (i + 1).ToString();
                        string tenMon = foodDetails.Rows[i]["FoodName"].ToString();
                        string soLuong = foodDetails.Rows[i]["Count"].ToString();
                        string donGia = foodDetails.Rows[i]["Price"].ToString();
                        string thanhTien = foodDetails.Rows[i]["Total"].ToString();

                        // Nối các cột lại bằng dấu phẩy
                        sb.AppendLine($"{stt},{tenMon},{soLuong},{donGia},{thanhTien}");
                    }

                    // --- Tổng kết ---
                    sb.AppendLine();
                    sb.AppendLine($",,,Tong tien:,{billingInfo.Rows[0]["Amount"]}");

                    // 4. Ghi file với mã hóa UTF-8 để hiển thị được tiếng Việt
                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);

                    MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tùy chọn: Mở file ngay sau khi lưu
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu file: " + ex.Message);
                }
            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
