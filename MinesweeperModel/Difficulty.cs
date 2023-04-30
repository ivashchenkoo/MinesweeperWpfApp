namespace MinesweeperModel
{
	public class Difficulty
	{
		public string Description { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int MinesNumber { get; set; }

		public static Difficulty[] GetDefaultDifficulties()
		{
			return new Difficulty[]
			{
				new Difficulty
				{
					Description = "Beginner",
					Width = 12,
					Height = 12,
					MinesNumber = 20
				},
				new Difficulty
				{
					Description = "Intermediate",
					Width = 16,
					Height = 16,
					MinesNumber = 40
				},
				new Difficulty
				{
					Description = "Expert",
					Width = 25,
					Height = 16,
					MinesNumber = 160
				}
			};
		}
	}
}
