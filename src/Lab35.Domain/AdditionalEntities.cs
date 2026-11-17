namespace Lab35.Domain;

// Додаткові сутності згідно з варіантом 3.
public sealed class Musician
{
    // Ім'я та музичний інструмент, яким володіє музикант.
    public string Name { get; }
    public string Instrument { get; }

    // Створюємо музиканта лише за наявності імені й назви інструмента.
    public Musician(string name, string instrument)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Ім'я музиканта обов'язкове.");
        if (string.IsNullOrWhiteSpace(instrument)) throw new ArgumentException("Інструмент обов'язковий.");

        // Зберігаємо значення без пробілів на початку та в кінці.
        Name = name.Trim();
        Instrument = instrument.Trim();
    }
}

public sealed class Pilot
{
    // Ім'я пілота та загальна кількість годин нальоту.
    public string Name { get; }
    public int FlightHours { get; }

    // Наліт не може бути від'ємним, а ім'я є обов'язковим.
    public Pilot(string name, int flightHours)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Ім'я пілота обов'язкове.");
        if (flightHours < 0) throw new ArgumentOutOfRangeException(nameof(flightHours));

        // Нормалізуємо ім'я перед збереженням у властивості.
        Name = name.Trim();
        FlightHours = flightHours;
    }
}
