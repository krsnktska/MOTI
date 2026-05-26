using AirlineGame.Models;

namespace AirlineGame.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.GameConfigs.Any()) return;

        var game = new GameConfig
        {
            Name = "Конкуренція авіакомпаній",
            Description = "Дві авіакомпанії одночасно обирають цінові стратегії на одному маршруті. " +
                          "Кожна прагне захопити максимальну частку ринку. " +
                          "Гра є антагоністичною з нульовою сумою: те, що одна авіакомпанія виграє у частці ринку, " +
                          "інша втрачає (в одиницях відсотків).",
            Player1Name = "Alpha Airlines",
            Player2Name = "Beta Airlines",
            Strategies =
            [
                new() { PlayerNumber = 1, Index = 0, Name = "Преміум",
                    Description = "Висока ціна, покращений сервіс, преміум-клас" },
                new() { PlayerNumber = 1, Index = 1, Name = "Економ",
                    Description = "Середня ціна, стандартний сервіс" },
                new() { PlayerNumber = 1, Index = 2, Name = "Бюджет",
                    Description = "Низька ціна, мінімальний сервіс, лоукост" },
                new() { PlayerNumber = 2, Index = 0, Name = "Преміум",
                    Description = "Висока ціна, покращений сервіс, преміум-клас" },
                new() { PlayerNumber = 2, Index = 1, Name = "Економ",
                    Description = "Середня ціна, стандартний сервіс" },
                new() { PlayerNumber = 2, Index = 2, Name = "Бюджет",
                    Description = "Низька ціна, мінімальний сервіс, лоукост" }
            ],
            MatrixCells =
            [
                new() { Row = 0, Col = 0, Value =  0 },
                new() { Row = 0, Col = 1, Value = -2 },
                new() { Row = 0, Col = 2, Value = -4 },
                new() { Row = 1, Col = 0, Value =  3 },
                new() { Row = 1, Col = 1, Value =  1 },
                new() { Row = 1, Col = 2, Value = -1 },
                new() { Row = 2, Col = 0, Value =  5 },
                new() { Row = 2, Col = 1, Value =  3 },
                new() { Row = 2, Col = 2, Value =  0 }
            ]
        };

        db.GameConfigs.Add(game);
        db.SaveChanges();
    }
}
