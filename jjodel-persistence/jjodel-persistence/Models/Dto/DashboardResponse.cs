namespace jjodel_persistence.Models.Dto {

    // view model of the backoffice dashboard (Home/Index).
    public class DashboardResponse {

        // first day (UTC) of the oldest month shown in the growth charts.
        public DateTime FromMonth { get; set; }

        // labels of the 12 months, e.g. "Oct 25".
        public string[] MonthLabels { get; set; } = Array.Empty<string>();

        public UserStatsResponse Users { get; set; } = new();
        public ProjectStatsResponse Projects { get; set; } = new();
    }
}
