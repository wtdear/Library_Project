using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
            MessageBox.Show("Devoloper - Johhny Kolbaya\nGroup - 106-Д9-2ИСП", "Author", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
