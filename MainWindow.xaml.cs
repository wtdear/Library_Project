using System;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace Library_Project
{
    public partial class MainWindow : Window
    {
        private readonly BookRepository _books = new BookRepository();
        private readonly LibraryBookUiService _libraryUi;

        public ObservableCollection<Book> Books { get; } = new ObservableCollection<Book>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _libraryUi = new LibraryBookUiService(_books, this, ReloadBooksFromStore);
            ReloadBooksFromStore();
        }

        private void ReloadBooksFromStore()
        {
            Books.Clear();
            foreach (var book in _books.LoadAll().OrderBy(b => b.Id))
                Books.Add(book);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void CloseButton_Click(object sender, RoutedEventArgs e) =>
            Close();

        private void ShowListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ReloadBooksFromStore();
            _libraryUi.ShowBooksDialog();
        }

        private void AddMenuItem_Click(object sender, RoutedEventArgs e) =>
            _libraryUi.AddBook();

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e) =>
            _libraryUi.DeleteBook();

        private void EditMenuItem_Click(object sender, RoutedEventArgs e) =>
            _libraryUi.EditBook();

        private void TakeMenuItem_Click(object sender, RoutedEventArgs e) =>
            _libraryUi.TakeBook();
        private void ShowTakenMenuItem_Click(object sender, RoutedEventArgs e) =>
            _libraryUi.ShowTakenBooks();

        private void ReturnMenuItem_Click(object sender, RoutedEventArgs e) =>
            _libraryUi.ReturnBook();


        private void AddBook_button(object sender, RoutedEventArgs e) =>
            _libraryUi.AddBook();

        private void ShowList_button(object sender, RoutedEventArgs e)
        {
            ReloadBooksFromStore();
            _libraryUi.ShowBooksDialog();
        }

        private void Delete_button(object sender, RoutedEventArgs e) =>
            _libraryUi.DeleteBook();

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // ignore
        }

        private void Author_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Devoloper - Johhny Kolbaya\nGroup - 106-Д9-2ИСП\nTask - Тема 1. Учёт книг (мини-библиотека)\nРеализуются добавление книги, просмотр всех книг, редактирование информации о книге, помещение книги в архив, загрузка списка книг из файла и сохранение изменённого списка в файл.\n", "Author", MessageBoxButton.OK, MessageBoxImage.Information);

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // ignore
        }
        private void ToArchive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetDirectory = @"D:\\library\\Data\\appData";
                string sourceFilePath = Path.Combine(targetDirectory, "Books.json");
                string zipPath = Path.Combine(targetDirectory, "Books.zip");

                if (!File.Exists(sourceFilePath))
                {
                    MessageBox.Show("Файл Books.json не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (File.Exists(zipPath)) File.Delete(zipPath);

                string arguments = $"-NoProfile -Command \"Compress-Archive -Path '{sourceFilePath}' -DestinationPath '{zipPath}' -Force\"";

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = arguments,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };

                using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo))
                {
                    process?.WaitForExit();
                }

                MessageBox.Show("Архив успешно создан средствами Windows!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
