using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Drawing;
using System.Data.SqlClient;
using System.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Globalization;
using Color = System.Drawing.Color;


namespace QLBH
{
    public partial class Form1 : Form
    {

        private bool Dragging = false;
        private Point startPoint = new Point(0, 0);
        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);

        }


        private void Form1_Load(object sender, EventArgs e)
        {

            //Panel pnlRight = new Panel() { Width = 2, Dock = DockStyle.Right, BackColor = System.Drawing.Color.GreenYellow };
            //this.Controls.Add(pnlRight);

            //Panel pnlBottom = new Panel() { Height = 2, Dock = DockStyle.Bottom, BackColor = System.Drawing.Color.GreenYellow };
            //this.Controls.Add(pnlBottom);

            //Panel pnlLeft = new Panel() { Width = 2, Dock = DockStyle.Left, BackColor = System.Drawing.Color.GreenYellow };
            //this.Controls.Add(pnlLeft);
        }

        // định nghĩa sql connect và thực hiện truyền vào C#
        SqlConnection conn = null;
        SqlCommand cmd = null;
        ProcessData processData = new ProcessData();




        private void Btn_signUp_Click(object sender, EventArgs e)
        {
            // khi nhấn nút đăng ký thì sẽ xữ lý những tác vụ.k dc để trống khi nhấn button ,nếu rỗng thì thông báo lỗi


            // ckeck các điều kiện ,nếu không thoả thì dừng và bắt nhập lại

            if (txtUser.Text == "" || txtPsw.Text == "" || txtEmail.Text == ""
                || txtName.Text == "" || txtDOB.Text == ""||lberror_user.Text !="" ||lberror_pass.Text!=""||lberror_name.Text!=""
                ||lberror_email.Text !=""||lberror_date.Text!="")
            {
                MessageBox.Show("Nội dung bị rỗng! Nhập đúng định dạng mới được đăng ký nhé!!!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }


            // Còn không thì
            else
            {

                // sử dụng hàm băm md5 Encrypt
                MD5 md5 = MD5.Create();
                byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(txtPsw.Text);
                byte[] hash = md5.ComputeHash(bytes);
                // bây giờ sẽ thực thi băm txt Pwd
                StringBuilder sb = new StringBuilder();
                for (int b = 0; b < hash.Length; b++)
                {
                    sb.Append(hash[b].ToString("X2")); // format theo yêu cầu 
                }


                var pwd = sb.ToString();
                // thêm dữ liệu từ data vào

                 conn = new SqlConnection(processData.path);
                string sql = "insert into Users(f_Username,f_Password,f_Name,f_Email,f_DOB,f_Permission) values(@a,@b,@c,@d,@e,@f)";
                cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@a", txtUser.Text);
                cmd.Parameters.AddWithValue("@b", pwd); // tại đây password sẽ băm và chuyển sang chế độ bảo mật
                cmd.Parameters.AddWithValue("@c", txtName.Text);
                cmd.Parameters.AddWithValue("@d", txtEmail.Text);
                cmd.Parameters.AddWithValue("@e", txtDOB.Text);
                cmd.Parameters.AddWithValue("@f", 0);

                conn.Open();
                cmd.ExecuteNonQuery();

                // thông báo khi thêm thành công!
                MessageBox.Show("Xin chúc mừng bạn đã đăng ký thành công User.Thông tin đã được lưu trong Database với Password không hiện rõ trong Database!!!! ", "Đăng ký User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                conn.Close();
                // khi mà đăng kí xong thì các textbox trả về rỗng hết
                txtEmail.Text = txtName.Text = txtPsw.Text = txtUser.Text = txtDOB.Text = "";



            }
        }

        //xữ lý focus
        private void txtUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPsw.Focus();
            }
        }

        private void txtPsw_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtName.Focus();
            }
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtEmail.Focus();
            }
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtDOB.Focus();
            }

        }

        private void txtDOB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtUser.Focus();
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_max_Click(object sender, EventArgs e)
        {
            if (WindowState.ToString() == "Normal") {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }


        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            startPoint = new Point(e.X, e.Y);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (Dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this.startPoint.X, p.Y - this.startPoint.Y);
            }
        }
        private bool CheckUser()
        {
            DataTable dt = processData.GetDataTable("Select * from Users");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][1].ToString().Trim() == txtUser.Text.Trim())
                    return true;
            }
            return false;
        }
        public bool IsUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsUpper(input[i]))
                    return true;
            }

            return false;
        }
        public bool isSpace(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == ' ')
                    return true;
            }

            return false;
        }
        public bool KiemTraKyTuDacBiet(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '*' || input[i] == '!' || input[i] == '`' || input[i] == '~' || input[i] == '@' || input[i] == '$' ||
                    input[i] == '%' || input[i] == '^' || input[i] == '&' || input[i] == '(' || input[i] == ')' || input[i] == '#' ||
                    input[i] == '+' || input[i] == '=' || input[i] == '{' || input[i] == '}' || input[i] == '|' || input[i] == '\\' ||
                    input[i] == ':' || input[i] == '"' || input[i] == ';' || input[i] == '\'' || input[i] == '<' || input[i] == '>' ||
                    input[i] == ',' || input[i] == '?' || input[i] == '/')
                    return true;
            }

            return false;
        }
        public bool KiemTraKyTuDacBiet1(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '*' || input[i] == '!' || input[i] == '`' || input[i] == '~' || input[i] == '@' || input[i] == '$' ||
                    input[i] == '%' || input[i] == '^' || input[i] == '&' || input[i] == '(' || input[i] == ')' || input[i] == '#' ||
                    input[i] == '+' || input[i] == '=' || input[i] == '{' || input[i] == '}' || input[i] == '|' || input[i] == '\\' ||
                    input[i] == ':' || input[i] == '"' || input[i] == ';' || input[i] == '\'' || input[i] == '<'|| input[i] == '>' ||
                    input[i] == ',' || input[i] == '?' || input[i] == '/' || input[i] == '-' || input[i] == '.' || input[i] == '_')
                    return true;
            }

            return false;
        }
        private static Regex a_z = new Regex(@"^[a-z]", RegexOptions.Compiled);
        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            // kiem tra null
            if (txtUser.Text == "")
            {
                lberror_user.Text = "*Bắt buộc phải có dữ liệu";
            }
            else
            {
                // kiem tra user tôn tại
                if (CheckUser())
                {
                    lberror_user.Text = "User đã tồn tại trong Database!";
                }
                else
                {
                    // Không dược viết hoa
                    if (IsUpper(txtUser.Text))
                    {
                        lberror_user.Text = "Không được viết hoa.";
                    }
                    else
                    {
                        // bat đâu là chữ a-z
                       if(!a_z.IsMatch(txtUser.Text))
                        {
                            lberror_user.Text = "Kí tự phải bắt đầu từ a-z.";
                        }
                       else
                        {
                            // kiem tra khoang trang
                            if(isSpace(txtUser.Text))
                            {
                                lberror_user.Text = "không được có khoảng trắng.";
                            }
                            else
                            {
                                // kiểm tra ký tự đặc biệt
                                if(KiemTraKyTuDacBiet(txtUser.Text))
                                {
                                    lberror_user.Text = "Không được có ký tự đặc biệt.";
                                }
                                else
                                {
                                    lberror_user.Text = "";
                                }
                            }
                        }
                    }
                }
            }
           
        }

        // kiểm tra Password chỉ gồm các kí tự a-z và 0-9 cùng tối thiểu 3 ký tự
        private static Regex pwd = new Regex(@"[a-z-0-9]", RegexOptions.Compiled);
        private void txtPsw_TextChanged(object sender, EventArgs e)
        {

            // kiem tra null
            if (txtPsw.Text == "")
            {
                lberror_pass.Text = "*Bắt buộc phải có dữ liệu";
            }
            else
            {
                // Không dược viết hoa
                if (IsUpper(txtUser.Text))
                {
                    lberror_pass.Text = "Không được viết hoa.";
                }
                else
                {
                    // chữ a-z và 0-9
                    if (!pwd.IsMatch(txtPsw.Text))
                    {
                        lberror_pass.Text = "Kí tự phải từ a-z và 0-9.";
                    }
                    else if (txtPsw.Text.Length <= 2)
                    {
                        lberror_pass.Text = "Độ dài Password phải lớn hơn 2";
                    }
                    else
                    {
                        
                       
                            // kiểm tra ký tự đặc biệt
                            if (KiemTraKyTuDacBiet1(txtPsw.Text))
                            {
                                lberror_pass.Text = "Không được có ký tự đặc biệt.";
                            }
                            else
                            {
                                lberror_pass.Text = "";
                            }
                        
                    }
                }

            }
        }


        // kiểm tra Name không được bắt đầu ký số hay kí tự đặc biệt 
        private static Regex name = new Regex(@"^[a-z]", RegexOptions.Compiled);
        private static Regex name1 = new Regex(@"^[A-Z]", RegexOptions.Compiled);
        private void txtName_TextChanged(object sender, EventArgs e)
        {
            // kiem tra null
            if (txtName.Text == "")
            {
                lberror_name.Text = "*Bắt buộc phải có dữ liệu";
            }
            else
            {
                if (!name.IsMatch(txtName.Text) && !name1.IsMatch(txtName.Text))
                {
                    lberror_name.Text = "Name không được bắt đầu bằng số hay ký tự đặt biệt";
                }
                else
                {
                    lberror_name .Text= "";
                }
            }
        }
        private static Regex checkemail = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.Compiled);
        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            if (txtEmail.Text == "")
            {
                lberror_email.Text = "*Bắt buộc phải có dữ liệu";
            }
            else
            {
                if (!checkemail.IsMatch(txtEmail.Text))
                {
                    lberror_email.Text = "*Email không hợp lệ.";
                }
                else
                {
                    lberror_email.Text = "";
                }
            }
        }

        private void txtDOB_TextChanged(object sender, EventArgs e)
        {
            if (txtDOB.Text == "" || txtDOB.Text == "dd/mm/yyyy")
            {
                lberror_date.Text = "*Bắt buộc phải có dữ liệu";
            }
            else
            {
                try
                {
                    DateTime.ParseExact(txtDOB.Text, "dd/MM/yyyy", null);
                    lberror_date.Text = "";
                }
                catch
                {
                    lberror_date.Text = "Ngày/Tháng/Năm format không đúng định dạng!!";
                }
            }
        }

        private void txtDOB_Enter(object sender, EventArgs e)
        {
            if (txtDOB.Text == "__/__/____")
            {
                txtDOB.Text = "";
                txtDOB.ForeColor = Color.Black;
            }
        }

        private void txtDOB_Leave(object sender, EventArgs e)
        {
            if (txtDOB.Text.Trim() == "")
            {
                txtDOB.Text = "__/__/____";
                txtDOB.ForeColor = Color.Gray;
            }
        }
    }
}

