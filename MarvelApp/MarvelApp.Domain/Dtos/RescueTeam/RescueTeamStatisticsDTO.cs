namespace MarvelApp.Domain.Dtos.RescueTeam
{
    public class RescueTeamStatisticsDTO
    {
        public string Name { get; set; } = null!;
        public int MembersCount { get; set; }
        public string StrongestMember { get; set; } = null!;
        public string SmartestMember { get; set; } = null!;
        public string FastestMember { get; set; } = null!;
    }
}
