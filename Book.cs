using System;

namespace Library_Project
{
    /// <summary>Модель книги (только данные).</summary>
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Pages { get; set; }


        /// <summary>
        ///  <remarks> Добавить архивацию </remarks>
        /// </summary>
        public bool IsArchived { get; set; } = false;
        public DateTime? ArchivedAt { get; set; }

        public Book() { }

        public Book(string title, string author, int year, int pages)
        {
            Title = title;
            Author = author;
            Year = year;
            Pages = pages;
        }

        public string ToLineSummary() =>
            $"ID: {Id} | {Title} | {Author} | {Year} | {Pages} стр.";

        /// <summary>Для привязки в списке.</summary>
        public string Summary => ToLineSummary();
    }
}
