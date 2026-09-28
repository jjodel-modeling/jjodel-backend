namespace jjodel_persistence.Models.Dto {

    // project statistics for the backoffice dashboard.
    public class ProjectStatsResponse {

        public int Total { get; set; }
        public int NewLast30Days { get; set; }

        // new projects per month: 12 values, from the oldest month to the current one.
        public int[] NewPerMonth { get; set; } = Array.Empty<int>();

        public int Public { get; set; }
        public int Private { get; set; }

        public List<LatestProjectResponse> Latest { get; set; } = new();
    }
}
