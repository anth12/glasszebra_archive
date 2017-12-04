using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GlassZebra
{
    public class Game
    {
        public static Game Current { get; set; } = new Game();

        public string CurrentPlayer { get; set; }
        public string CurrentQuestion { get; set; }

        public Dificulty CurrentDificulty { get; set; }

        public Player[] Players { get; set; }
        public Dificulty[] Dificulties { get; set; }
         = new[]
         {
            Dificulty.Easy,
            Dificulty.Moderate,
            Dificulty.Hard,
            Dificulty.VeryHard,
            Dificulty.Animal,
            Dificulty.Characters,
            Dificulty.Movies,
            Dificulty.TvShows,
            Dificulty.Books
         };
    }

    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Dificulty
    {
        Easy,
        Moderate,
        Hard,
        VeryHard,
        Animal,
        Characters,
        Movies,
        TvShows,
        Books
    }
}
