using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для TableEditorWindow.xaml
    /// </summary>
    public partial class TableEditorWindow : Window
    {
        public TableEditorWindow(string tableName, DataTable tableData)
        {
            InitializeComponent();
            // Создаем сервисы с явным приведением к интерфейсам
            IDatabaseService dataService = (IDatabaseService)new DataService();
            IJsonExportService exportService = (IJsonExportService)new JsonExportService();

            var editorVM = new TableEditorViewModel(tableName, tableData, dataService, exportService);
            DataContext = editorVM;
        }
    }
}