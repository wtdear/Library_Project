using System.Collections.Generic;

namespace Library_Project
{
    /// <summary>Доступ к хранилищу книг (удобно подменять в тестах).</summary>
    public interface IBookRepository
    {
        IReadOnlyList<Book> LoadAll();

        void SaveAll(IReadOnlyList<Book> books);
    }
}
