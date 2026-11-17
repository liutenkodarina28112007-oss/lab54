using System.Globalization;
using Lab35.Domain;

namespace Lab35.DAL;

// Простий CSV-формат: LastName,FirstName,Course,StudentCard,Gender,Residence,DormitoryRoom
public sealed class CsvStudentRepository : IStudentRepository
{
    // Файл, з якого читаються та в який записуються дані студентів.
    private readonly string _path;

    // Шлях передається ззовні, щоб місце зберігання можна було визначити
    // під час створення репозиторію.
    public CsvStudentRepository(string path) => _path = path;

    // Завантажує всі студентські записи з CSV-файлу.
    public IReadOnlyList<Student> GetAll()
    {
        // Відсутній файл означає, що записів поки немає.
        if (!File.Exists(_path)) return Array.Empty<Student>();

        var result = new List<Student>();

        // Перший рядок містить заголовки колонок, тому пропускаємо його.
        foreach (var line in File.ReadAllLines(_path).Skip(1))
        {
            // Порожні рядки не містять даних і не створюють об'єктів.
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Для створення Student потрібні всі сім колонок формату.
            var p = line.Split(',');
            if (p.Length != 7) throw new FormatException($"Некоректний рядок CSV: {line}");

            // Перетворюємо текстові значення на типізовані властивості студента.
            // Для чисел використовуємо незмінну культуру, щоб формат не залежав
            // від регіональних налаштувань комп'ютера.
            result.Add(new Student(p[0], p[1], int.Parse(p[2], CultureInfo.InvariantCulture),
                p[3], Enum.Parse<Gender>(p[4], true), Enum.Parse<ResidenceKind>(p[5], true),
                string.IsNullOrWhiteSpace(p[6]) ? null : p[6]));
        }

        // Повертаємо зібрані об'єкти через інтерфейс лише для читання.
        return result;
    }

    // Перезаписує файл заголовком і поточним набором студентів.
    public void SaveAll(IEnumerable<Student> students)
    {
        var lines = new List<string> { "LastName,FirstName,Course,StudentCard,Gender,Residence,DormitoryRoom" };

        // Перетворюємо властивості кожного студента на рядок CSV;
        // для відсутньої кімнати записуємо порожню колонку.
        lines.AddRange(students.Select(s =>
            $"{s.LastName},{s.FirstName},{s.Course},{s.StudentCard},{s.Gender},{s.Residence},{s.DormitoryRoom ?? ""}"));

        // Створюємо папку, якщо її ще немає, перед записом файлу.
        var directory = Path.GetDirectoryName(Path.GetFullPath(_path));
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

        // Записуємо заголовок і сформовані рядки у файл.
        File.WriteAllLines(_path, lines);
    }
}
