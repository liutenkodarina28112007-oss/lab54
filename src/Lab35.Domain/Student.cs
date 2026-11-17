namespace Lab35.Domain;

// Перелік статей, які використовуються для фільтрації та поселення.
public enum Gender { Female, Male }

// Місце проживання студента: гуртожиток або інший варіант.
public enum ResidenceKind { Dormitory, Other }

// Модель студента з перевіркою обов'язкових даних під час створення.
public sealed class Student
{
    // Основні дані студента після нормалізації пробілів.
    public string LastName { get; }
    public string FirstName { get; }
    public int Course { get; }
    public string StudentCard { get; }

    // Дані, що визначають стать і тип проживання.
    public Gender Gender { get; }
    public ResidenceKind Residence { get; }

    // Кімната потрібна лише для мешканців гуртожитку.
    public string? DormitoryRoom { get; }

    // Зручна похідна властивість для перевірки типу проживання.
    public bool LivesInDormitory => Residence == ResidenceKind.Dormitory;

    // Конструктор перевіряє вхідні значення та формує незмінний об'єкт.
    public Student(string lastName, string firstName, int course, string studentCard,
        Gender gender, ResidenceKind residence, string? dormitoryRoom = null)
    {
        // Перевіряємо обов'язкові текстові поля, перш ніж зберігати їх.
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Прізвище не може бути порожнім.");
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("Ім'я не може бути порожнім.");

        // У цій моделі підтримуються курси від першого до шостого.
        if (course is < 1 or > 6) throw new ArgumentOutOfRangeException(nameof(course), "Курс має бути від 1 до 6.");
        if (string.IsNullOrWhiteSpace(studentCard)) throw new ArgumentException("Номер студентського квитка обов'язковий.");

        // Студент не може бути позначений мешканцем гуртожитку без кімнати.
        if (residence == ResidenceKind.Dormitory && string.IsNullOrWhiteSpace(dormitoryRoom))
            throw new ArgumentException("Для мешканця гуртожитку потрібно вказати кімнату.");

        // Обрізаємо зайві пробіли в текстових значеннях перед збереженням.
        LastName = lastName.Trim();
        FirstName = firstName.Trim();
        Course = course;
        StudentCard = studentCard.Trim();
        Gender = gender;
        Residence = residence;
        DormitoryRoom = residence == ResidenceKind.Dormitory ? dormitoryRoom!.Trim() : null;
    }

    // Формуємо читабельне представлення студента для консольного виводу.
    public override string ToString() =>
        $"{LastName} {FirstName}, курс {Course}, квиток {StudentCard}, {Gender}, " +
        (LivesInDormitory ? $"гуртожиток, кімната {DormitoryRoom}" : "поза гуртожитком");
}
