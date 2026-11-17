using Lab35.DAL;
using Lab35.Domain;

namespace Lab35.BLL;

// Бізнес-сервіс містить операції над студентами та не залежить
// від конкретного формату зберігання даних.
public sealed class StudentService
{
    // Репозиторій постачає дані та зберігає їх за запитом сервісу.
    private readonly IStudentRepository _repository;

    // Перевіряємо залежність від репозиторію під час створення сервісу.
    public StudentService(IStudentRepository repository) =>
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    // Повертає всі записи, отримані зі сховища.
    public IReadOnlyList<Student> GetAll() => _repository.GetAll();

    // Основна операція варіанта 3: першокурсниці, які мешкають у гуртожитку.
    public IReadOnlyList<Student> GetFirstYearFemaleDormitoryStudents() =>
        // Для результату одночасно мають виконуватися всі три умови:
        // перший курс, жіноча стать і проживання в гуртожитку.
        _repository.GetAll()
            .Where(s => s.Course == 1 && s.Gender == Gender.Female && s.LivesInDormitory)
            .ToList();

    // Кількість визначається за тим самим відфільтрованим списком,
    // щоб умови підрахунку та вибірки не розходилися.
    public int CountFirstYearFemaleDormitoryStudents() =>
        GetFirstYearFemaleDormitoryStudents().Count;

    // Поселяє студентів у кімнати; в одній кімнаті можуть бути лише особи однієї статі.
    public IReadOnlyList<Student> AssignToRoom(IEnumerable<Student> students, string room, Gender roomGender)
    {
        // Не дозволяємо створювати поселення без номера кімнати.
        if (string.IsNullOrWhiteSpace(room)) throw new ArgumentException("Номер кімнати обов'язковий.");

        // Матеріалізуємо перелік один раз і окремо перевіряємо null.
        var list = students?.ToList() ?? throw new ArgumentNullException(nameof(students));

        // Усі мешканці мають відповідати статі, заданій для кімнати.
        if (list.Any(s => s.Gender != roomGender))
            throw new InvalidOperationException("У кімнаті не можна поселяти студентів різної статі.");

        // Створюємо нові об'єкти студентів із проживанням у вказаній кімнаті;
        // початкові об'єкти не змінюються.
        return list.Select(s => new Student(s.LastName, s.FirstName, s.Course, s.StudentCard,
            s.Gender, ResidenceKind.Dormitory, room)).ToList();
    }

    // Передає збереження репозиторію, не прив'язуючи сервіс до CSV чи іншого сховища.
    public void Save(IEnumerable<Student> students) => _repository.SaveAll(students);
}
