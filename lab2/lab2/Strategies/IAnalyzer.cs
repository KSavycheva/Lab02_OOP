using System.Collections.Generic;

namespace lab2
{
    public interface IAnalyzer
    {
        List<Publication> Analyze(string filePath,
                                  string? titleFilter,
                                  List<string>? authorsFilter,
                                  int? publishedYearFilter,
                                  string? facultyFilter,
                                  string? departmentFilter);
    }
}