namespace lab2
{
    public class FilterService
    {
        public FilterService() { }

        public bool IsMatchingFilters(Publication publication,
                               string? titleFilter,
                               List<string>? authorsFilter,
                               int? publishedYearFilter,
                               string? facultyFilter,
                               string? departmentFilter)
        {
            if (publication == null)
                throw new ArgumentNullException(nameof(publication));

            if (!string.IsNullOrEmpty(titleFilter) && !(publication.Title?.Contains(titleFilter, StringComparison.OrdinalIgnoreCase) ?? false))
            {
                return false;
            }

            if (authorsFilter != null && authorsFilter.Count > 0 && !authorsFilter.All(a => publication.Authors.Contains(a)))
            {
                return false;
            }

            if (publishedYearFilter.HasValue && publication.PublishedYear != publishedYearFilter.Value)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(facultyFilter) &&
                !string.Equals(publication.Faculty, facultyFilter, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(departmentFilter) &&
                !string.Equals(publication.Department, departmentFilter, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

    }
}