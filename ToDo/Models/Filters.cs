namespace ToDoApp.Models
{
    public class Filters
    {
        public Filters(string filterString)
        {
            Filterstring = filterString ?? "all-all-all"; //null-coalescing operator - this statement signifies that Filterstring is set to filterString, unless filterString is null, then it's assigned to "all-all-all"
            string[] filters = Filterstring.Split('-');
            CategoryId = filters[0];
            Due = filters[1];
            StatusId = filters[2];
        }

        public string Filterstring { get; }

        public string CategoryId { get; }

        public string Due { get; }

        public string StatusId { get; } //these "get" only properties can only be set in a constructor (above)

        public bool HasCategory => CategoryId.ToLower() != "all"; //is there any categoryId not equal to all 

        public bool HasDue => Due.ToLower() != "all";

        public bool HasStatus => StatusId.ToLower() != "all";

        public static Dictionary<string, string> DueFilterValues =>   //this property returns a dictionary
            new Dictionary<string, string>
            {
                {"future", "future"},
                {"past", "past"},
                {"today","today"}
            };
        public bool IsPast => Due.ToLower() == "past";

        public bool IsFuture => Due.ToLower() == "future";

        public bool IsToday => Due.ToLower() == "today";

    }

}
