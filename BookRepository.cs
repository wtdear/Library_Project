using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            var name = fileName ?? "Books.json";
            _filePath = ResolveJsonPath(name);
        }

        private static string ResolveJsonPath(string fileName)
        {
            var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            var candidates = new[]
            {
                Path.Combine(exeDir, fileName),
                Path.Combine(Directory.GetCurrentDirectory(), fileName),
                Path.Combine(AppContext.BaseDirectory, fileName),
            };
            foreach (var p in candidates)
            {
                if (File.Exists(p))
                    return p;
            }

            return string.IsNullOrEmpty(exeDir)
                ? Path.Combine(Directory.GetCurrentDirectory(), fileName)
                : Path.Combine(exeDir, fileName);
        }

        public IReadOnlyList<Book> LoadAll()
        {
            if (!File.Exists(_filePath))
                return Array.Empty<Book>();

            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<Book>();

            var trimmed = json.TrimStart();
            if (trimmed.StartsWith("["))
            {
                return JsonSerializer.Deserialize<List<Book>>(json, JsonOptions) ?? new List<Book>();
            }

            if (trimmed.StartsWith("{"))
            {
                var migrated = TryLoadLegacyDictionary(json);
                if (migrated.Count > 0)
                {
                    SaveAll(migrated);
                    return migrated;
                }
            }

            return new List<Book>();
        }

        public void SaveAll(IReadOnlyList<Book> books)
        {
            var list = books.ToList();
            var json = JsonSerializer.Serialize(list, JsonOptions);
            File.WriteAllText(_filePath, json);
        }

        public static int NextId(IReadOnlyList<Book> books) =>
            books.Count == 0 ? 1 : books.Max(b => b.Id) + 1;

        private static List<Book> TryLoadLegacyDictionary(string json)
        {
            var result = new List<Book>();
            try
            {
                var root = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, JsonOptions);
                if (root == null)
                    return result;

                foreach (var pair in root)
                {
                    if (pair.Value.ValueKind != JsonValueKind.Object)
                        continue;

                    var o = pair.Value;
                    var book = new Book
                    {
                        Id = o.TryGetProperty("id", out var idEl) ? idEl.GetInt32()
                            : o.TryGetProperty("Id", out var idP) ? idP.GetInt32() : 0,
                        Title = GetString(o, "title", "Title"),
                        Author = GetString(o, "author", "Author"),
                        Year = o.TryGetProperty("year", out var y) ? y.GetInt32()
                            : o.TryGetProperty("Year", out var y2) ? y2.GetInt32() : 0,
                        Pages = o.TryGetProperty("pages", out var p) ? p.GetInt32()
                            : o.TryGetProperty("Pages", out var p2) ? p2.GetInt32() : 0,
                    };
                    if (book.Id != 0 && !string.IsNullOrWhiteSpace(book.Title))
                        result.Add(book);
                }
            }
            catch (JsonException)
            {
                // ignore
            }

            return result;
        }

        private static string GetString(JsonElement o, string camel, string pascal)
        {
            if (o.TryGetProperty(camel, out var e))
                return e.GetString() ?? string.Empty;
            if (o.TryGetProperty(pascal, out var e2))
                return e2.GetString() ?? string.Empty;
            return string.Empty;
        }
    }
}
