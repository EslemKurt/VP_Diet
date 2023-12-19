﻿using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp1.Properties;
using static System.ComponentModel.Design.ObjectSelectorEditor;


// Şevval's database
//Data Source=LAPTOP-9HENLSU2;Initial Catalog=VP_diet;Integrated Security=True;Encrypt=True;Trust Server Certificate=True
//esma db   Data Source=localhost;Initial Catalog=VP_diet;Integrated Security=True


namespace WinFormsApp1
{
    public partial class DanisanPanel : Form
    {
        public int Id;
        private Panel myPanel;


        public DanisanPanel(int id)
        {
            InitializeComponent();
            //this.Id = id;
            LoadConsultantData(this.Id = id);
            // UpdateCompleted olayına abone ol
            UpdateKg updateKgForm = new UpdateKg(Id);
            updateKgForm.UpdateCompleted += UpdateKgForm_UpdateCompleted;

            foreach (var listItemControl in flowLayoutPanel1.Controls)
            {
                if (listItemControl is listItem listItem)
                {
                    listItem.btnInceleClicked += ListItem_InceleClicked;
                }
            }
        }

        /*private void DanisanPanel_Load(object sender, EventArgs e)
        {
            DrawLines();
        }

        private void DrawLines()
        {
            using (Graphics g = this.CreateGraphics())
            {
                 using (Pen p = new Pen(Color.Black, 1))
                 {
                     // Burada label'ın üstündeki ve altındaki çizgileri çizebilirsiniz.
                     // Örneğin, eğer label adı "label9" ise:
                     int label9Top = label9.Top;
                     int label9Bottom = label9.Bottom;

                    g.DrawLine(p, new Point(0, label9Top), new Point(this.Width, label9Top));
                     g.DrawLine(p, new Point(0, label9Bottom), new Point(this.Width, label9Bottom)); 

                 }
                
            }
        }
        */

        //SqlConnection baglanti = new SqlConnection(@"Data Source=LAPTOP-9HENLSU2;Initial Catalog=VP_diet;Integrated Security=True;TrustServerCertificate=True");
        SqlConnection baglanti = new SqlConnection(@"Data Source=localhost;Initial Catalog=VP_diet;Integrated Security=True");

        private void UpdateKgForm_UpdateCompleted(object sender, EventArgs e)
        {
            // Güncelleme tamamlandığında yapılacak işlemler
            LoadConsultantData(Id); // veya başka güncelleme işlemleri
        }
        private void LoadConsultantData(int id)
        {
            using (baglanti) // connectionString'i uygun şekilde değiştirin
            {
                baglanti.Open();


                // 1. SqlCommand için SqlDataReader
                using (SqlCommand komut1 = new SqlCommand("SELECT * FROM Users WHERE Id = @id", baglanti))
                {
                    komut1.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader dataReader2 = komut1.ExecuteReader())
                    {
                        if (dataReader2.Read())
                        {
                            string kullaniciAdi = dataReader2["userName"].ToString();
                            lblUser.Text = kullaniciAdi;
                            DateTime registerDate = Convert.ToDateTime(dataReader2["registerDate"]);

                            // Gün sayısını hesaplayarak label'a atama yapın
                            TimeSpan difference = DateTime.Now - registerDate;
                            int gunSayisi = (int)difference.TotalDays;

                            lblGun.Text = gunSayisi.ToString();
                        }
                    }
                }

                // 2. SqlCommand için SqlDataReader
                using (SqlCommand komut2 = new SqlCommand("SELECT * FROM Consultant WHERE consultantId = @id", baglanti))
                {
                    komut2.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader dataReader = komut2.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            string email = dataReader["email"].ToString();
                            lblMail.Text = email;
                            string boyy = dataReader["height"].ToString();
                            lblBoy.Text = boyy;
                            int boy1 = Convert.ToInt32(dataReader["height"]);
                            string target = dataReader["targetWeight"].ToString();
                            lblTarget.Text = target;


                        }
                    }
                }

                // 3. SqlCommand için SqlDataReader
                using (SqlCommand komut3 = new SqlCommand("SELECT TOP 1 * FROM UpdateTbl WHERE userId = @id ORDER BY updateTime DESC", baglanti))
                {
                    komut3.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader dataReader3 = komut3.ExecuteReader())
                    {
                        if (dataReader3.Read())
                        {
                            string newwWeight = dataReader3["newWeight"].ToString();
                            kiloLbl.Text = newwWeight;

                            string newWaist = dataReader3["newWaist"].ToString();
                            lblBel.Text = newWaist;

                            string newHip = dataReader3["newHip"].ToString();
                            lblKalca.Text = newHip;

                            string newChest = dataReader3["newChest"].ToString();
                            lblGogus.Text = newChest;


                        }
                    }
                }

                //4.SqlCommand için SqlDataReader
                using (SqlCommand komut4 = new SqlCommand("SELECT D.nameSurname AS DietitianNameSurname, D.email AS DietitianEmail FROM Partner P JOIN Dietitian D ON P.dietitian = D.dietitianId WHERE P.consultant = @id ", baglanti))
                {
                    komut4.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader dataReader4 = komut4.ExecuteReader())
                    {
                        if (dataReader4.Read())
                        {
                            string dietitianNameSurname = dataReader4["DietitianNameSurname"].ToString();
                            string dietitianEmail = dataReader4["DietitianEmail"].ToString();

                            // Elde edilen değerleri kullanabilirsiniz
                            diyetisyenLbl.Text = dietitianNameSurname;
                            diyetisyenMailLbl.Text = dietitianEmail;
                        }
                        else
                        {
                            // Belirtilen consultantId ile eşleşen bir kayıt bulunamadı
                            diyetisyenLbl.Text = "Eşleşen diyetisyen bulunamadı";
                            diyetisyenMailLbl.Text = "";
                        }
                    }
                }

                using (SqlCommand komut7 = new SqlCommand("SELECT TOP 1 U.newWeight, C.height FROM UpdateTbl U INNER JOIN Consultant C ON U.userId = C.consultantId WHERE U.userId = @id ORDER BY U.updateTime DESC", baglanti))
                {
                    komut7.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader dataReader7 = komut7.ExecuteReader())
                    {
                        if (dataReader7.Read())
                        {
                            double newWeight = Convert.ToDouble(dataReader7["newWeight"]);
                            double height = Convert.ToDouble(dataReader7["height"]) / 100;

                            // BMI hesapla ve göster
                            CalculateAndShowBMI(newWeight, height);
                        }

                    }




                }

                baglanti.Close();
            }
        }
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            UpdateKg frm = new UpdateKg(Id);
            this.Opacity = 0.5;
            frm.FormClosed += (s, args) =>
            {
                // Guncelle formu kapatıldığında ana formun saydamlığını 1.0 olarak ayarla
                //this.Update();
                this.Opacity = 1.0;

            };
            frm.Show();

        }

        // Veritabanından diyetisyen kullanıcı adlarını getiren bir metod
        private List<string> GetDietitianUsernames()
        {
            List<string> usernames = new List<string>();

            try
            {
                // Veritabanına bağlantı sağla
                using (baglanti)
                {
                    baglanti.Open();

                    // SQL sorgusunu oluştur
                    string query = "SELECT nameSurname FROM Dietitian"; // DietitianTable tablosunda bulunan kullanıcı adlarını çek
                    using (SqlCommand command = new SqlCommand(query, baglanti))
                    {
                        // Veritabanından verileri oku
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string username = reader["nameSurname"].ToString();
                                usernames.Add(username);
                            }
                        }
                    }

                    baglanti.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving dietitian usernames: " + ex.Message);
            }

            return usernames;
        }

        private void populateItems()
        {
            SqlConnection connection = new SqlConnection("Data Source=localhost; Initial Catalog=VP_diet; Integrated Security=True");
            using (connection)
            {
                connection.Open();

                string query = "SELECT * FROM Dietitian";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listItem listItem = new listItem(Convert.ToInt32(reader["dietitianId"]));
                            listItem.DietitianId = Convert.ToInt32(reader["dietitianId"]); // Bu satırın eklenmiş olması önemli
                            listItem.Name = reader["nameSurname"].ToString();
                            listItem.Puan = "süper";
                            listItem.InceleClicked += ListItem_InceleClicked;
                            flowLayoutPanel1.Controls.Add(listItem);
                        }
                    }
                }
            }

        }
        private void btnParola_Click(object sender, EventArgs e)
        {
            PassUpdate frm = new PassUpdate(Id);
            this.Opacity = 0.5;
            frm.FormClosed += (s, args) =>
            {
                // Guncelle formu kapatıldığında ana formun saydamlığını 1.0 olarak ayarla
                this.Opacity = 1.0;
            };
            frm.Show();
        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void leftPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DanisanPanel form = new DanisanPanel(Id);
            form.Show();
            this.Close();
        }

        private void btnSayfaYenile_Click(object sender, EventArgs e)
        {
            DanisanPanel form = new DanisanPanel(Id);
            form.Show();
            this.Close();
        }

        private void DanisanPanel_Load(object sender, EventArgs e)
        {
            populateItems();
        }
        private void ListItem_InceleClicked(object sender, int dietitianId)
        {
            // InceleClicked etkinliği tetiklendiğinde infoFrm formunu aç
            infoFrm form = new infoFrm(dietitianId);
            form.Show();
        }

        private void CalculateAndShowBMI(double weight, double height)
        {
            // BMI hesapla
            double bmi = weight / (height * height);

            // BMI'ı ekrana yazdır
            lblBmi.Text = $"BMI: {bmi:F2}";

            // BMI değerlendirmesi
            if (bmi < 18.5)
            {
                lblBmi.Text += " (Zayıf)";
            }
            else if (bmi < 24.9)
            {
                lblBmi.Text += " (Normal)";
            }
            else if (bmi < 29.9)
            {
                lblBmi.Text += " (Fazla Kilolu)";
            }
            else
            {
                lblBmi.Text += " (Obez)";
            }
        }

    }


}