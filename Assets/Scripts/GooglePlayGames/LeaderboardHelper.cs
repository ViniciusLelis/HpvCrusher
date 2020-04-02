using System.Collections.Generic;

public class LeaderboardHelper
{
    private static readonly Dictionary<int, string> _episodesAndLeaderboards = new Dictionary<int, string>
    {
        {1, GPGSIds.leaderboard_leaderboard_episode_1},
        {2, GPGSIds.leaderboard_leaderboard_episode_2},
        {3, GPGSIds.leaderboard_leaderboard_episode_3},
        {4, GPGSIds.leaderboard_leaderboard_episode_4},
        {5, GPGSIds.leaderboard_leaderboard_episode_5},
        {6, GPGSIds.leaderboard_leaderboard_episode_6},
        {7, GPGSIds.leaderboard_leaderboard_episode_7},
        {8, GPGSIds.leaderboard_leaderboard_episode_8},
        {9, GPGSIds.leaderboard_leaderboard_episode_9},
    };

    public static string GetLeaderboardIdForEpisode(int episodeNumber)
    {
        if (_episodesAndLeaderboards.ContainsKey(episodeNumber))
        {
            return _episodesAndLeaderboards[episodeNumber];
        }

        return null;
    }

}
