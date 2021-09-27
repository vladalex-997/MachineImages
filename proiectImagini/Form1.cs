using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;


namespace proiectImagini
{
    public partial class Form1 : Form
    {
        public string Cod;
        public Timer timer1;
        public Timer timer2;
        public Image current;
        public Image previous;
        public Image Eroare=new Bitmap(proiectImagini.Properties.Resources.Eroare);
        public Form1()
        {
            InitializeComponent();
            labelImagine.Visible = false;
            textBoxId.Visible = false;
            buttonUpload.Visible = false;
            labelError.Visible = false;
           // GetImage();
            InitTimer1();
            InitTimer2();
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK&&int.TryParse(textBoxId.Text, out int docu))
            {
                string filename = openFileDialog1.FileName;
                string contentType = "image/jpeg";
                byte[] bytes = File.ReadAllBytes(filename);
                Database databaseObject = new Database();
                string query = "UPDATE PLCImages set Nume=@Nume, ContentType=@ContentType, DataImagine=@DataImagine WHERE Id=@Id";
                SqlCommand mycommand = new SqlCommand(query, databaseObject.myConnection);
                mycommand.Parameters.AddWithValue("@Nume",filename);
                mycommand.Parameters.AddWithValue("@ContentType", contentType);
                mycommand.Parameters.AddWithValue("DataImagine",bytes);
                mycommand.Parameters.AddWithValue("@Id",int.Parse(textBoxId.Text));

                databaseObject.OpenConnection();
                var result = mycommand.ExecuteNonQuery();
                databaseObject.CloseConnection();

                MessageBox.Show("Incarcare imagine reusita");

            }
            else
            {
                MessageBox.Show("Selectati fisier si scrieti Id Corect");
            }

          


        }

        public void InitTimer1()
        {
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 100; // nr sec in mili
            timer1.Start();




        }

        public void InitTimer2()
        {
            timer2 = new Timer();
            timer2.Tick += new EventHandler(timer2_Tick);
            timer2.Interval = 1500; // nr sec in mili
            timer2.Start();




        }

     


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (textBoxCod.Text != "")
            {
                GetImage();
                previous = current;
                Display();
            }
           

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            textBoxCod.Text = "";



        }

        public void Display()
        {
            pictureBox1.Image = previous;
        }

        public void GetImage()
        {
            try
            {
                Database databaseObject = new Database();
                databaseObject.OpenConnection();

                Cod = textBoxCod.Text;

                string query = "SELECT DataImagine from PLCImages WHERE Cod=@Cod";
                SqlCommand mycommand = new SqlCommand(query, databaseObject.myConnection);
                mycommand.Parameters.AddWithValue("@Cod", Cod);

                SqlDataReader reader = mycommand.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    current = ByteArrayToImage((byte[])(reader.GetValue(0)));

                    labelError.Visible = false;
                }
                else
                {

                    

                    current = Eroare;
                 //   labelError.Visible = true;

                }
                reader.Close();


                databaseObject.CloseConnection();

               

            }
            catch (Exception)
            {
             //   labelError.Visible = true;
            }
            

        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
        }
    }
}
