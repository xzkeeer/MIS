using System;
using System.Windows;
using WpfApp1;

namespace MedicalSystem
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Создаем экземпляры сервисов
                IDataService dataService = new DataService(); // Явное приведение к интерфейсу
                IUserService userService = new UserService(); // Явное приведение к интерфейсу

                // Проверка подключения
               

                // Создание главного окна
                var mainViewModel = new MainViewModel(dataService, userService);

                // Запускаем загрузку таблиц вручную
                mainViewModel.LoadTablesCommand.Execute(null);
                var mainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };

                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        
    }
}