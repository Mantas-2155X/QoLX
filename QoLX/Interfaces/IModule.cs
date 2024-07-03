namespace QoLX.Interfaces
{
	public interface IModule
	{
		public bool Loaded { get; }
		
		public void Load();

		public void Unload();
	}
}