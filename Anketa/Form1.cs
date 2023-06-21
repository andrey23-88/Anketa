using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace AnketaDZ
{

    public partial class Form1 : Form
    {
        BindingList<Anketes> ankets = new BindingList<Anketes>();
        public Form1()
        {
            InitializeComponent();

            listBox1.DataSource = ankets;
            listBox1.DisplayMember = "fulName";
        }
        public class Anketes
        {
            public string name { get; set; }
            public string lastName { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string fulName { get; set; }
        }

        //обработка добавления в базу
        private void button1_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            if (IsTxtBoxEmpty())
            {
                MessageBox.Show("Нельзя добавить пустые значения!",
                "Внимание",
                MessageBoxButtons.OK,
                MessageBoxIcon.Hand);
            }
            else if (CheckEmail())
            {
                MessageBox.Show("Некорректный email-адрес");
                txtEmail.Focus();
            }
            else if (IsCheckEmail())
            {
                MessageBox.Show("Пользователь с данным email-адресом уже имеется в базе!");
                txtEmail.Focus();
            }
            else
            {
                Anketes anketes = new Anketes();
                anketes.name = txtName.Text;
                anketes.lastName = txtLname.Text;
                anketes.email = txtEmail.Text;
                anketes.phone = msktxtPhone.Text;
                anketes.fulName = $"{txtName.Text} {txtLname.Text} {txtEmail.Text} {msktxtPhone.Text}";
                ankets.Add(anketes);
                listBox1.SelectedIndex = -1;
                ItemIndexText();
            }
        }

        //обработка на клик по анкете в listBox
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                Anketes anketes = (Anketes)listBox1.SelectedItem;

                txtName.Text = anketes.name;
                txtName.Select();
                txtLname.Text = anketes.lastName;
                txtEmail.Text = anketes.email;
                msktxtPhone.Text = anketes.phone;
            }
        }

        //действия при загрузки формы
        private void Form1_Load(object sender, EventArgs e)
        {
            ankets.Add(new Anketes// для образца
            {
                name = "Иван",
                lastName = "Иванов",
                email = "ivan@mail.ru",
                phone = "+7(899)912-3456",
                fulName = $"Иван Иванов ivan@mail.ru +7(899)912-3456"
            });
            listBox1.SelectedIndex = -1;
            lblCount.Text = ankets.Count().ToString();
        }

        //обработка кнопки открытия файла
        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                lblProv.Text = fileName;
                ankets.Clear();
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string a = line;
                        string b = reader.ReadLine();
                        string c = reader.ReadLine();
                        string d = reader.ReadLine();
                        ankets.Add(new Anketes
                        {
                            name = a,
                            lastName = b,
                            email = c,
                            phone = d,
                            fulName = $"{a} {b} {c} {d}"
                        });
                    }
                }
                lblCount.Text = ankets.Count().ToString();
            }
        }

        //обработка кнопки сохранения в файл
        private void button4_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
                lblProv.Text = fileName;

                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    foreach (var item in ankets)
                    {
                        sw.WriteLine(item.name);
                        sw.WriteLine(item.lastName);
                        sw.WriteLine(item.email);
                        sw.WriteLine(item.phone);
                    }
                }
            }
        }

        //обработка удаления записи
        private void button3_Click(object sender, EventArgs e)
        {
            if (lblCount.Text == "0")
            {
                MessageBox.Show("Нет записей для удаления!");
            }
            else if (MessageBox.Show("Вы точно хотите удалить запись?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Anketes ank = (Anketes)listBox1.SelectedItem;
                int selectedIndex = listBox1.SelectedIndex;
                ankets.Remove(ank);
                ItemIndexText();
            }

        }
        // обработка изменения записи
        private void btnChange_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите нужную запись в списке!",
                  "Внимание",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Hand);
            }
            else if (CheckEmail())
            {
                MessageBox.Show("Некорректный email-адрес");
                txtEmail.Focus();
            }
            else if (IsCheckEmail())
            {
                MessageBox.Show("Пользователь с данным email-адресом уже имеется в базе!");
                txtEmail.Focus();
            }
            else
            {
                Anketes ank = (Anketes)listBox1.SelectedItem;
                int selectedIndex = listBox1.SelectedIndex;

                ank.name = txtName.Text;
                ank.lastName = txtLname.Text;
                ank.email = txtEmail.Text;
                ank.phone = msktxtPhone.Text;
                ank.fulName = $"{txtName.Text} {txtLname.Text} {txtEmail.Text} {msktxtPhone.Text}";
                ankets.Remove(ank);
                ankets.Insert(selectedIndex, ank);
                lblCount.Text = ankets.Count().ToString();
                listBox1.SelectedIndex = -1;
                ItemIndexText();
            }
        }
        //метод проверки правильности заполнения email
        private bool CheckEmail()
        {
            string email = txtEmail.Text;
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            { return true; }
            else return false;
        }

        //метод проверки на дублирования email в базе 
        private bool IsCheckEmail()
        {
            foreach (var item in ankets)
            {
                if (item.email == txtEmail.Text)
                    return true;
            }
            return false;
        }
        //метод очистки текстовых полей, если ничего не выделено в listBox
        private void ItemIndexText()
        {
            if (listBox1.SelectedIndex == -1)
            {
                txtName.Text = String.Empty; txtName.Select();
                txtLname.Text = String.Empty;
                txtEmail.Text = String.Empty;
                msktxtPhone.Text = String.Empty;
            }
            lblCount.Text = ankets.Count().ToString();
        }

        //метод проверки на пустые поля при добавлении или редактировании 
        private bool IsTxtBoxEmpty()
        {
            if (txtName.Text == String.Empty ||
                txtLname.Text == String.Empty ||
                txtEmail.Text == String.Empty ||
                msktxtPhone.Text == "+7(   )    -")
                return true;
            else return false;
        }

        //метод перекидывающий активный статус другому элементу при нажатии Tab
        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                this.SelectNextControl((Control)sender, false, true, true, true);
            }
        }

        //метод позволяющий в поля вводить только буквы и backspace
        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        ////метод позволяющий в поля вводить только буквы и backspace
        private void txtLname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }
    }
}
