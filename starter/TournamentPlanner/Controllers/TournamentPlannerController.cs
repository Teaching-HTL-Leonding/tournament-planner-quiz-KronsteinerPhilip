using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TournamentPlanner.Data;

namespace TournamentPlanner.Controllers
{
    [ApiController]
    [Route("api")]
    public class TournamentPlannerController:ControllerBase
    {
        private readonly TournamentPlannerDbContext context;

        public TournamentPlannerController(TournamentPlannerDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("players")]
        public IEnumerable<Player> GetAllPlayers()
        {
            return context.Players;
        }

        [HttpGet]
        [Route("matches/open")]
        public IEnumerable<Match> GetAllOpenMatches() => context.Matches.Where(i => i.Winner == null).ToList();

        [HttpPost]
        [Route("players/{player}")]
        public async Task<Player> PostPlayers(Player player)
        {
            context.Add(player);
            await context.SaveChangesAsync();
            return player;
        }
    }
}
