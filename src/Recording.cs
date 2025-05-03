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
    public TimeSlice(float playerPositionX, float playerPositionY, int playerDirection, bool playerShoot, float time)
    {
        PlayerPositionX = playerPositionX;
        PlayerPositionY = playerPositionY;
        PlayerDirection = playerDirection;
        PlayerShoot = playerShoot;

        Time = time;
    }
    public float Time { get; set; }
    public bool PlayerShoot { get; set; }
    public float PlayerPositionX { get; set; }
    public float PlayerPositionY { get; set; }
    public int PlayerDirection { get; set; }
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
        Console.WriteLine($"TimeSlice: {timeSlice.Time}");
        History = History.Append(timeSlice).ToArray();
    }
}
