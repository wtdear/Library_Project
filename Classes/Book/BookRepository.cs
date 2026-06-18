using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Library_Project
{
    public sealed class BookRepository : IBookRepository
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
        };

        private readonly string _filePath;

        public BookRepository(string? fileName = null)
        {
            var appDirectory = @"D:\\library\\Data";
            _filePath = Path.Combine(appDirectory, fileName ?? "Books.json");

            Console.WriteLine($"[BookRepository] File path: {_filePath}");
            System.Diagnostics.Debug.WriteLine($"[BookRepository] File path: {_filePath}");
        }

        public IReadOnlyList<Book> LoadAll()
        {
            Console.WriteLine($"[LoadAll] Attempting to load from: {_filePath}");

            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"[LoadAll] File does not exist: {_filePath}");
                return new List<Book>();
            }

            var json = File.ReadAllText(_filePath);
            Console.WriteLine($"[LoadAll] Read {json.Length} characters");

            if (string.IsNullOrWhiteSpace(json))
            {
                Console.WriteLine($"[LoadAll] File is empty");
                return new List<Book>();
            }

            try
            {
                var books = JsonSerializer.Deserialize<List<Book>>(json, JsonOptions);
                Console.WriteLine($"[LoadAll] Deserialized {books?.Count ?? 0} books");
                return books ?? new List<Book>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[LoadAll] JSON Error: {ex.Message}");
                return new List<Book>();
            }
        }

        public void SaveAll(IReadOnlyList<Book> books)
        {
            Console.WriteLine($"[SaveAll] Saving {books.Count} books to: {_filePath}");

            try
            {
                var list = books.ToList();
                var json = JsonSerializer.Serialize(list, JsonOptions);
                Console.WriteLine($"[SaveAll] Serialized JSON length: {json.Length} characters");

                File.WriteAllText(_filePath, json);
                Console.WriteLine($"[SaveAll] File written successfully");

                if (File.Exists(_filePath))
                {
                    var fileInfo = new FileInfo(_filePath);
                    Console.WriteLine($"[SaveAll] File size: {fileInfo.Length} bytes");
                }
                else
                {
                    Console.WriteLine($"[SaveAll] ERROR: File was not created!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveAll] ERROR: {ex.Message}");
                Console.WriteLine($"[SaveAll] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public static int NextId(IReadOnlyList<Book> books) =>
            books.Count == 0 ? 1 : books.Max(b => b.Id) + 1;
    }
}