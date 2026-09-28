namespace jjodel_persistence.Models.Dto {

    // user statistics for the backoffice dashboard.
    public class UserStatsResponse {

        public int Total { get; set; }
        public int NewLast30Days { get; set; }
        public int NewLast7Days { get; set; }
        public int ActiveLast30Days { get; set; }

        // new users per month: 12 values, from the oldest month to the current one.
        public int[] NewPerMonth { get; set; } = Array.Empty<int>();

        // users by last access: < 7 days, 7-30 days, 30-90 days, 90 days - 1 year, > 1 year, never.
        public int[] LastAccessBuckets { get; set; } = Array.Empty<int>();

        // top affiliations / countries, followed by "Other" and "Not specified".
        public List<KeyValuePair<string, int>> ByAffiliation { get; set; } = new();
        public List<KeyValuePair<string, int>> ByCountry { get; set; } = new();
    }
}
