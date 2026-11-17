using Lab35.Domain;

namespace Lab35.DAL;

// Контракт сховища студентів. Бізнес-логіка залежить від цього інтерфейсу,
// а не від конкретного способу зберігання (наприклад, CSV).
public interface IStudentRepository
{
    // Повертає всі студентські записи.
    IReadOnlyList<Student> GetAll();

    // Зберігає переданий набір студентів.
    void SaveAll(IEnumerable<Student> students);
}
