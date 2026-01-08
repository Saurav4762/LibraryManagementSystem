using Practice_Project.Entities;
using System;
using System.Collections.Generic;

namespace Practice_Project.Models
{
    public class DashboardViewModel
    {
        /*public string CurrentDate { get; set; } = string.Empty;
        public string CurrentTime { get; set; } = string.Empty;*/

        public int TotalStudents { get; set; }
        public int TotalBooks { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalCategories { get; set; }
        public int CurrentlyIssued { get; set; }
        public int AvailableBooks { get; set; }
        
        public int AvailableBookList { get; set; }
        public int OverdueCount { get; set; }
        public string TotalPendingFine { get; set; } = "₹0.00";

        public List<BookIssue> OverdueBooks { get; set; } = new List<BookIssue>();
    }
}