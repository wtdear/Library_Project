using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Library_Project
{
    public sealed class LibraryBookUiService
    {
        private readonly IBookRepository _repository;
        private readonly Window _owner;
        private readonly Action _reloadBooks;

        public LibraryBookUiService(IBookRepository repository, Window owner, Action reloadBooks)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _reloadBooks = reloadBooks ?? throw new ArgumentNullException(nameof(reloadBooks));
        }

        public void AddBook()
        {
            var title = WpfInputHelper.Prompt(_owner, "Add Book", "Book title:");
            if (title == null)
                return;
            title = title.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show(_owner, "Title cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var author = WpfInputHelper.Prompt(_owner, "Add Book", "Author:");
            if (author == null)
                return;
            author = author.Trim();
            if (string.IsNullOrEmpty(author))
            {
                MessageBox.Show(_owner, "Author cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var yearText = WpfInputHelper.Prompt(_owner, "Add Book", "Publication year:");
            if (yearText == null)
                return;
            if (!int.TryParse(yearText.Trim(), out var year))
            {
                MessageBox.Show(_owner, "Invalid year format.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var pagesText = WpfInputHelper.Prompt(_owner, "Add Book", "Number of pages:");
            if (pagesText == null)
                return;
            if (!int.TryParse(pagesText.Trim(), out var pages) || pages < 0)
            {
                MessageBox.Show(_owner, "Invalid page count format.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var list = _repository.LoadAll().ToList();
            var newBook = new Book(title, author, year, pages)
            {
                Id = BookRepository.NextId(list),
            };
            list.Add(newBook);
            _repository.SaveAll(list);
            _reloadBooks();
            MessageBox.Show(_owner, $"Book \"{title}\" added (ID: {newBook.Id}).", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void DeleteBook()
        {
            var idText = WpfInputHelper.Prompt(_owner, "Delete Book", "Book ID to delete:");
            if (idText == null)
                return;
            if (!int.TryParse(idText.Trim(), out var id))
            {
                MessageBox.Show(_owner, "Invalid ID format.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var list = _repository.LoadAll().ToList();
            var book = list.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                MessageBox.Show(_owner, $"Book with ID {id} not found.", "Delete Book", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            list.Remove(book);
            _repository.SaveAll(list);
            _reloadBooks();
            MessageBox.Show(_owner, $"Book \"{book.Title}\" deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void EditBook()
        {
            var idText = WpfInputHelper.Prompt(_owner, "Edit Book", "Book ID:");
            if (idText == null)
                return;
            if (!int.TryParse(idText.Trim(), out var id))
            {
                MessageBox.Show(_owner, "Invalid ID format.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var list = _repository.LoadAll().ToList();
            var book = list.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                MessageBox.Show(_owner, $"Book with ID {id} not found.", "Edit Book", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var newTitle = WpfInputHelper.Prompt(_owner, "Edit Book",
                $"Title (leave empty to keep current):\nCurrent: {book.Title}", book.Title);
            if (newTitle == null)
                return;
            if (!string.IsNullOrWhiteSpace(newTitle))
                book.Title = newTitle.Trim();

            var newAuthor = WpfInputHelper.Prompt(_owner, "Edit Book", $"Author:\nCurrent: {book.Author}", book.Author);
            if (newAuthor == null)
                return;
            if (!string.IsNullOrWhiteSpace(newAuthor))
                book.Author = newAuthor.Trim();

            var yearStr = WpfInputHelper.Prompt(_owner, "Edit Book", $"Year (leave empty to keep current). Current: {book.Year}", "");
            if (yearStr == null)
                return;
            if (!string.IsNullOrWhiteSpace(yearStr) && int.TryParse(yearStr.Trim(), out var newYear))
                book.Year = newYear;

            var pagesStr = WpfInputHelper.Prompt(_owner, "Edit Book", $"Pages (leave empty to keep current). Current: {book.Pages}", "");
            if (pagesStr == null)
                return;
            if (!string.IsNullOrWhiteSpace(pagesStr) && int.TryParse(pagesStr.Trim(), out var newPages))
                book.Pages = newPages;

            _repository.SaveAll(list);
            _reloadBooks();
            MessageBox.Show(_owner, "Book updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowBooksDialog()
        {
            var list = _repository.LoadAll().ToList();
            if (list.Count == 0)
            {
                MessageBox.Show(_owner, "The library is empty!", "Book List", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var sb = new StringBuilder("Book list:\n\n");
            foreach (var book in list.OrderBy(b => b.Id))
                sb.AppendLine(book.ToLineSummary());

            MessageBox.Show(_owner, sb.ToString(), "Library Books", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}