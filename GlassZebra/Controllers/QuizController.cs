using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GlassZebra.Controllers
{
    [Route("api/[controller]")]
    public class QuizController : Controller
    {
        // GET api/ping
        [HttpGet]
        public string Get()
        {
            return "";
        }

        // POST api/quiz/score
        [HttpPost("score")]
        public async Task<Player[]> PostScore(string player)
        {
            Words.AnsweredItems.Add(Game.Current.CurrentQuestion);

            Game.Current.CurrentPlayer = player;
            Game.Current.Players.First(p => p.Name == player).Score++;
            // Re-order
            Game.Current.Players = Game.Current.Players.OrderByDescending(p => p.Score).ThenBy(p => p.Name).ToArray();
            await WebSocketHandler.BroadcastUpdate();
            return Game.Current.Players;
        }

        // POST api/quiz/players
        [HttpPost("players")]
        public Player[] PostPlayers([FromBody] string[] players)
        {
            Game.Current.Players = players.Select(p=> new Player { Name = p }).ToArray();
            Game.Current.CurrentPlayer = Game.Current.Players[new Random().Next(0, Game.Current.Players.Length - 1)].Name;

            foreach (var player in Game.Current.Players)
                Words.SeenItems.Add(player.Name, new System.Collections.Generic.List<string>());

            return Game.Current.Players;
        }

        // GET api/quiz/dificulties
        [HttpPost("dificulties")]
        public Game PostDificulties([FromBody] Dificulty[] dificulties)
        {
            Game.Current.Dificulties = dificulties;
            return Game.Current;
        }

        // GET api/quiz/players
        [HttpGet("players")]
        public Player[] GetPlayers()
        {
            return Game.Current.Players;
        }

        // GET api/quiz/game
        [HttpGet("game")]
        public Game GetGame()
        {
            return Game.Current;
        }

        // GET api/quiz/question
        [HttpGet("question")]
        public async Task<Game> GetQuestion()
        {
            Words.NextQuestion();

            await WebSocketHandler.BroadcastUpdate();

            return Game.Current;
        }
    }

}
