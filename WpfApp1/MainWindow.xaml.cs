using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            IDataService dataService = new DataService();
            IUserService userService = new UserService();

            // Назначаем DataContext — связываем окно с ViewModel
            this.DataContext = new MainViewModel(dataService, userService);


        }
    }
}