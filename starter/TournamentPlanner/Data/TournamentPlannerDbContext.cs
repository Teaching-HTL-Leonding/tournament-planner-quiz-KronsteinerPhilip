using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TournamentPlanner.Data
{
    public enum PlayerNumber { Player1 = 1, Player2 = 2 };

    public class TournamentPlannerDbContext : DbContext
    {
        public TournamentPlannerDbContext(DbContextOptions<TournamentPlannerDbContext> options)
            : base(options)
        { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }

        // This class is NOT COMPLETE.
        // Todo: Complete the class according to the requirements
        
        /// <summary>
        /// Adds a new player to the player table
        /// </summary>
        /// <param name="newPlayer">Player to add</param>
        /// <returns>Player after it has been added to the DB</returns>
        public async Task<Player> AddPlayer(Player newPlayer)
        {
            await Players.AddAsync(newPlayer);
            await SaveChangesAsync();
            return newPlayer;
        }

        /// <summary>
        /// Adds a match between two players
        /// </summary>
        /// <param name="player1Id">ID of player 1</param>
        /// <param name="player2Id">ID of player 2</param>
        /// <param name="round">Number of the round</param>
        /// <returns>Generated match after it has been added to the DB</returns>
        public async Task<Match> AddMatch(int player1Id, int player2Id, int round)
        {
            Match match = new Match { Player1 = Players.Find(player1Id), Player2 = Players.Find(player2Id), Round = round };
            await Matches.AddAsync(match);
            await SaveChangesAsync();
            return match;
        }

        /// <summary>
        /// Set winner of an existing game
        /// </summary>
        /// <param name="matchId">ID of the match to update</param>
        /// <param name="player">Player who has won the match</param>
        /// <returns>Match after it has been updated in the DB</returns>
        public async Task<Match> SetWinner(int matchId, PlayerNumber player)
        {
            if (player.Equals(1))
                Matches.Find(matchId).Winner = Matches.Find(matchId).Player1;
            else if(player.Equals(2))
                Matches.Find(matchId).Winner = Matches.Find(matchId).Player2;
            return Matches.Find(matchId);
        }

        /// <summary>
        /// Get a list of all matches that do not have a winner yet
        /// </summary>
        /// <returns>List of all found matches</returns>
        public async Task<IList<Match>> GetIncompleteMatches()
        {
            return Matches.ToList();
        }

        /// <summary>
        /// Delete everything (matches, players)
        /// </summary>
        public async Task DeleteEverything()
        {
            using var transaction = await Database.BeginTransactionAsync();
            foreach (var item in Players)
            {
                Players.Remove(item);
            }
            foreach (var item in Matches)
            {
                Matches.Remove(item);
            }
            await SaveChangesAsync();
            await transaction.CommitAsync();
            return;
        }

        /// <summary>
        /// Get a list of all players whose name contains <paramref name="playerFilter"/>
        /// </summary>
        /// <param name="playerFilter">Player filter. If null, all players must be returned</param>
        /// <returns>List of all found players</returns>
        public async Task<IList<Player>> GetFilteredPlayers(string playerFilter = null)
        {
            List<Player> list = new List<Player>();
            if (playerFilter == null)
                return Players.ToList();

            foreach (var item in Players)
            {
                if (item.Name.Contains(playerFilter))
                    list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// Generate match records for the next round
        /// </summary>
        /// <exception cref="InvalidOperationException">Error while generating match records</exception>
        public async Task GenerateMatchesForNextRound()
        {
            using var transaction = await Database.BeginTransactionAsync();
            foreach (var item in Matches)
                if (item.Winner == null)
                    throw new InvalidOperationException();
            if(Players.ToList().Count != 32)
                throw new InvalidOperationException();

            //TODO

            await transaction.CommitAsync();
        }
    }
}
