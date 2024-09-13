using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XamlAnimatedGif;
using MaterialDesignThemes.Wpf;

namespace Mini_Store
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateButtons();
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void StoreButton(object sender, RoutedEventArgs e)
        {
            Settings_Border.Visibility = (Settings_Border.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Donate_Button(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.donationalerts.com/r/skey4ik") { UseShellExecute = true });
        }

        private static void Show_Message(string text)
        {
            Message addWindow = new Message();
            addWindow.Message_Block.Text = text;
            addWindow.Show();
        }

        private void Clear_Cache_Button(object sender, RoutedEventArgs e)
        {
            string folderPath = @"tmp/";
            try
            {
                if (Directory.Exists(folderPath))
                {
                    // Получаем все файлы из указанной папки
                    string[] files = Directory.GetFiles(folderPath);

                    // Удаляем каждый файл
                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }

                    Show_Message("Все файлы были удалены.");
                }
                else
                {
                    Show_Message("Указанная папка не существует");
                }
            }
            catch (Exception ex)
            {
                Show_Message($"Произошла ошибка: {ex.Message}");
            }
        }
        private void CreateButtons()
        {
            try
            {
                string json = File.ReadAllText("Data.json");
                var items = JsonConvert.DeserializeObject<List<MyItem>>(json) ?? new List<MyItem>();

                foreach (var item in items)
                {
                    Grid grid = new()
                    {
                        Height = 100,
                        Width = 100,
                        Margin = new Thickness(15),
                        Background = new SolidColorBrush(Color.FromRgb(50, 56, 69)),
                        Cursor = Cursors.Hand,
                        Clip = new RectangleGeometry(new Rect(0, 0, 100, 100), 8, 8)
                    };

                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    if (item.data.Icon != null && File.Exists(item.data.Icon))
                    {
                        Image img = new()
                        {
                            Source = new BitmapImage(new Uri(Path.Combine(Directory.GetCurrentDirectory(), item.data.Icon))),                           
                            Width = 40,
                            Margin = new Thickness(10),
                            HorizontalAlignment = HorizontalAlignment.Center
                        };
                        Grid.SetRow(img, 0);
                        grid.Children.Add(img);
                    }
                    else
                    {
                        TextBlock NoImage = new()
                        {
                            Text = "Empty Image",
                            Background = Brushes.White,
                            Padding = new Thickness(8),
                            FontSize = 10,
                            Margin = new Thickness(0, 20, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                        Grid.SetRow(NoImage, 0);
                        grid.Children.Add(NoImage);
                    }
                    Image gif = new()
                    {
                        Height = 100,
                        Width = 100,
                        Visibility = Visibility.Visible,
                    };
                    AnimationBehavior.SetSourceUri(gif, new Uri("pack://application:,,,/Resources/gif.gif"));

                    TextBlock label = new()
                    {
                        Text = item.data.Name,
                        Foreground = Brushes.White,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0),
                        FontSize = 14,
                        TextWrapping = TextWrapping.Wrap
                    };
                    Grid.SetRow(label, 1);
                    grid.Children.Add(label);

                    grid.MouseDown += async (sender, e) =>
                    {
                        grid.Children.Add(gif);
                        string filePath = "tmp\\" + item.data.Name + "_setup.exe";
                        await DownloadFileAsync(item.data.Link, filePath, gif, grid);
                    };
                    ButtonPanel.Children.Add(grid);
                }
                Grid grid_add = new()
                {
                    Height = 100,
                    Width = 100,
                    Margin = new Thickness(15),
                    Background = new SolidColorBrush(Color.FromRgb(50, 56, 69)),
                    Cursor = Cursors.Hand,
                    Clip = new RectangleGeometry(new Rect(0, 0, 100, 100), 8, 8)
                };

                PackIcon img_add = new()
                {
                    Kind = PackIconKind.Plus,
                    Width = 75,
                    Height = 75,
                    Foreground = new SolidColorBrush(Color.FromRgb(38, 129, 218)),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                grid_add.Children.Add(img_add);

                grid_add.MouseDown += (sender, e) =>
                {
                    AddWindow addWindow = new AddWindow();
                    addWindow.ShowDialog();
                };

                ButtonPanel.Children.Add(grid_add);
            }
            catch (Exception ex)
            {
                Show_Message($"Произошла ошибка: {ex.Message}");
            }
        }
        private static async Task DownloadFileAsync(string fileUrl, string filePath, Image gif, Grid grid)
        {
            if (!Directory.Exists("tmp"))
            {
                Directory.CreateDirectory("tmp");
            }
            try
            {
                using (HttpClient httpClient = new())
                using (Stream contentStream = await httpClient.GetStreamAsync(fileUrl))
                using (FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write))
                {
                    await contentStream.CopyToAsync(fileStream);
                }
                grid.Children.Remove(gif);
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                grid.Children.Remove(gif);
                
                Show_Message($"Произошла ошибка: {ex.Message}");
            }
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
    }
}