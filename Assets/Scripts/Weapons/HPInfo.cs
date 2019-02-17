public struct HPInfo
{
	public int current;
	public int max;

	public float RateToFull { get { return (float)current / max; } }
}