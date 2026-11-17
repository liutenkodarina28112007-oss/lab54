using Lab35.BLL;
using Lab35.DAL;
using Lab35.Domain;

// Зберігаємо шлях до CSV поруч із виконуваною програмою,
// щоб застосунок знаходив файл незалежно від поточної папки запуску.
var dataPath = Path.Combine(AppContext.BaseDirectory, "students.csv");

// Консольна програма використовує CSV-репозиторій для роботи з даними,
// а сервіс — для виконання бізнес-операцій над студентами.
var repository = new CsvStudentRepository(dataPath);
var service = new StudentService(repository);

// Вмикаємо коректне відображення українських символів у консолі.
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Лабораторна робота 3.5 — варіант 3");

// Показуємо загальну кількість записів та кількість студенток,
// які відповідають умові варіанта.
Console.WriteLine($"Усього студентів у файлі: {service.GetAll().Count}");
Console.WriteLine($"Першокурсниць у гуртожитку: {service.CountFirstYearFemaleDormitoryStudents()}");
Console.WriteLine("\nСписок студенток, які відповідають умові:");

// Виводимо повний список студенток, відібраний бізнес-сервісом.
foreach (var student in service.GetFirstYearFemaleDormitoryStudents())
    Console.WriteLine(student);

// Демонструємо створення додаткових сутностей із предметної області.
Console.WriteLine("\nДодаткові сутності варіанта:");
var musician = new Musician("Олена", "фортепіано");
var pilot = new Pilot("Андрій", 1250);
Console.WriteLine($"Музикант: {musician.Name}, інструмент: {musician.Instrument}");
Console.WriteLine($"Пілот: {pilot.Name}, наліт: {pilot.FlightHours} год.");

// Очікуємо Enter, щоб консольне вікно не закрилося відразу після виведення.
Console.WriteLine("\nНатисніть Enter для завершення.");
Console.ReadLine();
