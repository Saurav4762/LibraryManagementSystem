namespace Practice_Project.Models
{
    public static class Constants
    {
        public static readonly List<string> Semesters = new List<string>
        {
            "First",
            "Second",
            "Third", 
            "Fourth",
            "Fifth",
            "Sixth",
            "Seventh",
            "Eighth"
        };
        
        // You can also add other constants here
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string Librarian = "Librarian";
            public const string Student = "Student";
        }
        
        public static class Status
        {
            public const string Active = "Active";
            public const string Inactive = "Inactive";
        }
        
    }
}