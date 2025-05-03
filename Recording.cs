


namespace Recording;

class GameHistory
{
    public Round[] Rounds = [];
    public void AppendToRounds(Round round)
    {
        Rounds = Rounds.Append(round).ToArray();
    }
    public void SaveToFile(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var round in Rounds)
            {
                writer.WriteLine($"Round {round.Id} Number of Time Slices: {round.History.Length}");
                foreach (var timeSlice in round.History)
                {
                    writer.WriteLine($"Time: {timeSlice.Time}, Player Position: ({timeSlice.PlayerPositionX}, {timeSlice.PlayerPositionY})");
                }
            }
        }
    }
}
class TimeSlice
{
    public TimeSlice(int playerPositionX, int playerPositionY, float time)
    {
        PlayerPositionX = playerPositionX;
        PlayerPositionY = playerPositionY;
        Time = time;
    }
    public float Time { get; set; }
    public int PlayerPositionX { get; set; }
    public int PlayerPositionY { get; set; }
}
class Round
{
    public Round(int pastId = 0)
    {
        Id = pastId + 1;
    }
    public int Id;
    public TimeSlice[] History = [];
    public void AppendToHistory(TimeSlice timeSlice)
    {
        History = History.Append(timeSlice).ToArray();
    }
}
