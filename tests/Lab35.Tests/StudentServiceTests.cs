using Lab35.BLL;
using Lab35.DAL;
using Lab35.Domain;
using Xunit;

namespace Lab35.Tests;

// Перевірки бізнес-логіки та правил предметної області.
public sealed class StudentServiceTests
{
    // Атрибут [Fact] позначає окремий тест, який запускає xUnit.
    // Кожен тест нижче перевіряє один конкретний сценарій.

    // Тестова реалізація сховища тримає дані в пам'яті замість роботи з файлом.
    // Такі тести ізольовані від диска та зовнішніх даних.
    private sealed class StubStudentRepository : IStudentRepository
    {
        private readonly List<Student> _students;

        // Копіюємо вхідну послідовність у внутрішній змінюваний список.
        public StubStudentRepository(IEnumerable<Student> students) => _students = students.ToList();

        // Повертаємо підготовлені тестові дані.
        public IReadOnlyList<Student> GetAll() => _students;

        // Імітуємо збереження: очищаємо список і додаємо новий набір.
        public void SaveAll(IEnumerable<Student> students) { _students.Clear(); _students.AddRange(students); }
    }

    // Допоміжний метод скорочує створення тестових студентів,
    // а випадковий номер квитка забезпечує окреме значення для кожного об'єкта.
    private static Student S(string last, int course, Gender gender, ResidenceKind residence, string? room = null) =>
        new(last, "Тест", course, Guid.NewGuid().ToString("N"), gender, residence, room);

    [Fact]
    public void Count_ReturnsOnlyFirstYearWomenLivingInDormitory()
    {
        // Arrange: додаємо відповідних студенток і записи, які мають бути відсіяні.
        var students = new[]
        {
            S("А", 1, Gender.Female, ResidenceKind.Dormitory, "1.01"),
            S("Б", 1, Gender.Female, ResidenceKind.Dormitory, "1.02"),
            S("В", 1, Gender.Male, ResidenceKind.Dormitory, "1.03"),
            S("Г", 2, Gender.Female, ResidenceKind.Dormitory, "1.04"),
            S("Д", 1, Gender.Female, ResidenceKind.Other)
        };
        var service = new StudentService(new StubStudentRepository(students));

        // Act: просимо сервіс порахувати записи, що відповідають усім умовам.
        var count = service.CountFirstYearFemaleDormitoryStudents();

        // Assert: лише дві студентки відповідають умовам фільтра.
        Assert.Equal(2, count);
    }

    [Fact]
    public void Filter_ReturnsMatchingStudents()
    {
        // Arrange: в тестовому сховищі є студентки першого та другого курсів.
        var service = new StudentService(new StubStudentRepository(new[]
        {
            S("А", 1, Gender.Female, ResidenceKind.Dormitory, "1.01"),
            S("Б", 2, Gender.Female, ResidenceKind.Dormitory, "1.02")
        }));

        // Act: отримуємо список студенток, які відповідають умовам сервісу.
        var result = service.GetFirstYearFemaleDormitoryStudents();

        // Assert: у списку одна людина, її прізвище — "А".
        Assert.Single(result);
        Assert.Equal("А", result[0].LastName);
    }

    [Fact]
    public void Count_ReturnsZeroWhenNoStudentsMatch()
    {
        // Arrange: створюємо набір, у якому немає студенток, що відповідають умові.
        var service = new StudentService(new StubStudentRepository(new[]
        {
            S("А", 1, Gender.Male, ResidenceKind.Dormitory, "1.01")
        }));

        // Act: викликаємо підрахунок; Assert: перевіряємо, що результат дорівнює нулю.
        Assert.Equal(0, service.CountFirstYearFemaleDormitoryStudents());
    }

    [Fact]
    public void AssignToRoom_AssignsStudentsOfSameGender()
    {
        // Arrange: готуємо сервіс і студенток, які ще не живуть у гуртожитку.
        var service = new StudentService(new StubStudentRepository(Array.Empty<Student>()));
        var students = new[] { S("А", 1, Gender.Female, ResidenceKind.Other), S("Б", 2, Gender.Female, ResidenceKind.Other) };

        // Act: поселяємо двох студенток в одну жіночу кімнату.
        var assigned = service.AssignToRoom(students, "5.10", Gender.Female);

        // Assert: перевіряємо кількість, тип проживання, кімнату та стать результатів.
        Assert.Equal(2, assigned.Count);
        Assert.All(assigned, s =>
        {
            Assert.True(s.LivesInDormitory);
            Assert.Equal("5.10", s.DormitoryRoom);
            Assert.Equal(Gender.Female, s.Gender);
        });
    }

    [Fact]
    public void AssignToRoom_RejectsMixedGender()
    {
        // Arrange: у наборі є студентка і студент, але кімната призначена для жінок.
        var service = new StudentService(new StubStudentRepository(Array.Empty<Student>()));
        var students = new[] { S("А", 1, Gender.Female, ResidenceKind.Other), S("Б", 1, Gender.Male, ResidenceKind.Other) };

        // Act: викликаємо поселення; Assert: очікуємо InvalidOperationException.
        Assert.Throws<InvalidOperationException>(() => service.AssignToRoom(students, "5.10", Gender.Female));
    }

    [Fact]
    public void Student_RejectsInvalidCourse()
    {
        // Arrange: курс 7 виходить за дозволений діапазон від 1 до 6.
        // Act: створюємо студента з некоректним курсом.
        // Assert: конструктор має відхилити значення через ArgumentOutOfRangeException.
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Student("Тест", "Тест", 7, "ST007", Gender.Female, ResidenceKind.Other));
    }

    [Fact]
    public void Student_RequiresRoomForDormitoryResident()
    {
        // Arrange: задаємо проживання в гуртожитку, але не вказуємо кімнату.
        // Act: створюємо студента з неповними даними проживання.
        // Assert: конструктор має повідомити про некоректний аргумент.
        Assert.Throws<ArgumentException>(() =>
            new Student("Тест", "Тест", 1, "ST008", Gender.Female, ResidenceKind.Dormitory));
    }

    [Fact]
    public void Pilot_RejectsNegativeFlightHours()
    {
        // Arrange: задаємо від'ємне значення нальоту.
        // Act: створюємо пілота з некоректними даними.
        // Assert: очікуємо ArgumentOutOfRangeException.
        Assert.Throws<ArgumentOutOfRangeException>(() => new Pilot("Пілот", -1));
    }

    [Fact]
    public void Musician_RequiresInstrument()
    {
        // Arrange: передаємо пробіли замість назви інструмента.
        // Act: створюємо музиканта з некоректними даними.
        // Assert: очікуємо ArgumentException.
        Assert.Throws<ArgumentException>(() => new Musician("Музикант", " "));
    }
}
