namespace jjodel_persistence.Models.Dto {

    // lightweight project row for the dashboard (the State is not loaded: it can weigh hundreds of KB).
    public class LatestProjectResponse {

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Author { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
    }
}
