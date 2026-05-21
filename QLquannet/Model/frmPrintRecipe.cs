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

            DataGridViewRow selectedRow = dgvBilling.SelectedRows[0];
            int billingID = Convert.ToInt32(selectedRow.Cells["BillingID"].Value);

            //Lấy dữ liệu chi tiết từ Database 
            DataTable billingInfo = prDAL.GetBillingInfo(billingID);
            DataTable foodDetails = prDAL.GetFoodDetails(billingID);

            if (billingInfo.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy dữ liệu cho hóa đơn này.");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV Files (*.csv)|*.csv";
            sfd.FileName = "HoaDon_" + billingID + ".csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    //Phần đầu hóa đơn
                    sb.AppendLine("HOA DON THANH TOAN");
                    sb.AppendLine("Cua hang: NET CO");
                    sb.AppendLine("Ma hoa don: " + billingID);
                    sb.AppendLine("Ngay: " + billingInfo.Rows[0]["Date"].ToString());
                    sb.AppendLine("Nhan vien: " + billingInfo.Rows[0]["LastName"].ToString());
                    sb.AppendLine();

                    sb.AppendLine("STT,Ten Mon,So Luong,Don Gia,Thanh Tien");

                    // Chi tiết các món ăn
                    for (int i = 0; i < foodDetails.Rows.Count; i++)
                    {
                        string stt = (i + 1).ToString();
                        string tenMon = foodDetails.Rows[i]["FoodName"].ToString();
                        string soLuong = foodDetails.Rows[i]["Count"].ToString();
                        string donGia = foodDetails.Rows[i]["Price"].ToString();
                        string thanhTien = foodDetails.Rows[i]["Total"].ToString();
                        sb.AppendLine($"{stt},{tenMon},{soLuong},{donGia},{thanhTien}");
                    }
                    sb.AppendLine();
                    sb.AppendLine($",,,Tong tien:,{billingInfo.Rows[0]["Amount"]}");
                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
