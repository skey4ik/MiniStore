using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Mini_Store
{
    public partial class AddWindow : Window
    {
        private List<MyItem> dataList = new List<MyItem>();
        public AddWindow()
        {
            InitializeComponent();
            LoadData();
        }

        public class MyItem
        {
            public Data? data { get; set; }
        }

        public class Data
        {
            public string? Name { get; set; }
            public string? Icon { get; set; }
            public string? Link { get; set; }

        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            var newData = new Data
            {
                Name = TextBoxName.Text,
                Icon = TextBoxIcon.Text,
                Link = TextBoxLink.Text
            };

            var newItem = new MyItem { data = newData };

            dataList.Add(newItem);

            SaveData();
            Restart();


        }

        private void SaveData()
        {
            string jsonString = JsonConvert.SerializeObject(dataList, Formatting.Indented);

            File.WriteAllText("Data.json", jsonString);
        }
        private void LoadData()
        {
            if (File.Exists("Data.json"))
            {
                string jsonString = File.ReadAllText("Data.json");
                dataList = JsonConvert.DeserializeObject<List<MyItem>>(jsonString) ?? new List<MyItem>();
            }
        }

        private void Restart()
        {
            var currentWindow = Application.Current.MainWindow;
            currentWindow.Close();
            var newWindow = new MainWindow();
            newWindow.Show();
            Application.Current.MainWindow = newWindow;
            Close();
            AddWindow addWindow = new AddWindow();
            addWindow.ShowDialog();
        }
    }
}
