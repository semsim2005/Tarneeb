using System.Collections.Generic;

namespace Tarneeb.Models
{
    /// <summary>
    /// Holds the Scores of the games of the match and the total score
    /// Also include the Winning team in case the match is finished
    /// </summary>
    public class MatchScore
    {
        private const int MatchScoreUpper = 31, MatchScoreLower = -31;

        public List<GameScore> ScoresList { get; private set; }
        public Dictionary<TeamPosition, int> Scores;

        public TeamPosition? Winner
        {
            get
            {
                if (Scores[TeamPosition.NorthSouth] >= MatchScoreUpper || Scores[TeamPosition.EastWest] <= MatchScoreLower)
                {
                    return TeamPosition.NorthSouth;
                }
                else if (Scores[TeamPosition.EastWest] >= MatchScoreUpper || Scores[TeamPosition.NorthSouth] <= MatchScoreLower)
                {
                    return TeamPosition.EastWest;
                }
                else
                {
                    return null;
                }
            }
        }

        public MatchScore()
        {
            ScoresList = new List<GameScore>();

            Scores = new Dictionary<TeamPosition, int>();
            Scores.Add(TeamPosition.NorthSouth, 0);
            Scores.Add(TeamPosition.EastWest, 0);
        }
        public void AddGameScore(GameScore gameScore)
        {
            ScoresList.Add(gameScore);
            Scores[TeamPosition.NorthSouth] += gameScore.Score[TeamPosition.NorthSouth];
            Scores[TeamPosition.EastWest] += gameScore.Score[TeamPosition.EastWest];
        }
    }

    /// <summary>
    /// Holds the score of a single game in the match including
    /// the bid and the scores of the two teams
    /// </summary>
    public class GameScore
    {
        public Dictionary<TeamPosition, int> Score { get; set; }
        public Bid GameBid { get; set; }

        public GameScore(Bid gameBid, int nsScore, int ewScore)
        {
            this.GameBid = gameBid;
            Score = new Dictionary<TeamPosition, int>();
            Score[TeamPosition.NorthSouth] = nsScore;
            Score[TeamPosition.EastWest] = ewScore;
        }
    }
}