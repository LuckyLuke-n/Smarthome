using LSoftware.Repository.Abstractions;

namespace Smarthome.Core.DomainObjects
{
	[Serializable]
	public class Location : IEntity
	{
		public string Id { get; set; } = string.Empty;

		public string City { get; set; } = string.Empty;
		public double Latitude { get; set; }
		public double Longitude { get; set; }

		public void SetAsNew()
		{
			Id = Guid.NewGuid().ToString();
		}
	}
}
