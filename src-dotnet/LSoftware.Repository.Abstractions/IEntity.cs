namespace LSoftware.Repository.Abstractions
{
	public interface IEntity
	{
		string Id { get; set; }
		void SetAsNew();
	}
}
