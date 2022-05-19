using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _lockTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(15)
        };

        public MainWindow()
        {
            InitializeComponent();
            CaptchaPanel.Visibility = Visibility.Collapsed;
            _lockTimer.Tick += delegate { this.IsEnabled = true; _lockTimer.Stop();  };
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginBox.Text;
            var password = PasswordBox.Text;
            var captchaVisibility = CaptchaPanel.Visibility;
            var enteredCaptcha = CaptchaTB.Text;
            var generatedCaptcha = CaptchaTBl.Text;

            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password) ||
                captchaVisibility == Visibility.Visible && string.IsNullOrWhiteSpace(enteredCaptcha) )
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка");
                return;
            }

            if (captchaVisibility == Visibility.Visible &&
                enteredCaptcha != generatedCaptcha)
            {
                MessageBox.Show("Введенная каптча не совпадает со сгенерированной, повторите попытку через 15 сек", "Ошибка");

                this.IsEnabled = false;
                _lockTimer.Start();

                CaptchaTBl.Text = GenerateCaptcha();

                return;
            }

            var user = App.db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user == null)
            {
                MessageBox.Show("Неверное имя пользователя и/или пароль", "Ошибка");

                CaptchaPanel.Visibility = Visibility.Visible;
                CaptchaTBl.Text = GenerateCaptcha();

                return;
            }

            var window = new Window1(user);
            window.Show();
            this.Close();
        }

        private string GenerateCaptcha()
        {
            const string symbols = "QWERTYUIOPASDFGHJKLZXCVBNM1234567890";
            var result = "";
            var random = new Random();
            for (var i = 0; i < 4; i++)
            {
                result += symbols[random.Next(symbols.Length)];
            }

            return result;
        }

        private void CaptchaRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            CaptchaTBl.Text = GenerateCaptcha();
        }
    }
}
