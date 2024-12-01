using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using CommunityToolkit.Maui.Storage;

namespace lab2
{
    public partial class MainPage : ContentPage
    {
        private string? selectedStrategy;
        private List<Publication> publications;
        private XmlAnalyzerContext analyzerContext;
        private string? filePath;

        public MainPage()
        {
            InitializeComponent();

            var LinqAnalyzer = new LinqAnalyzer();
            analyzerContext = new XmlAnalyzerContext(LinqAnalyzer);

            publications = new List<Publication>();
        }

        private async void OnStartBtnClicked(object sender, EventArgs e)
        {
            await ShowStrategySelection();
        }

        private async Task ShowStrategySelection()
        {
            string action = await DisplayActionSheet("Choose strategy to process file", "Cancel", null, "DOM", "SAX", "LINQ");

            if (action != "Cancel")
            {
                selectedStrategy = action;
                await ShowNextStep();
            }
            else
            {
                await DisplayAlert("Nothing has been chosen", "If you want to work with new data - choose strategy", "OK");
            }
        }

        private async Task ShowNextStep()
        {
            bool chooseFile = await DisplayAlert("Next Step",
                $"Strategy Selected: {selectedStrategy}",
                "Choose File", "Back");

            if (chooseFile)
            {
                await OpenFilePicker();
            }
            else
            {
                await ShowStrategySelection();
            }
        }

        private async Task OpenFilePicker()
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Please select an XML file",
            });

            if (result != null)
            {
                this.filePath = result.FullPath;
                ProcessFile(filePath, selectedStrategy);
            }
        }

        private void ProcessFile(string filePath, string strategy)
        {
            switch (strategy)
            {
                case "DOM":
                    analyzerContext.SetAnalyzer(new DomAnalyzer());
                    break;
                case "SAX":
                    analyzerContext.SetAnalyzer(new SaxAnalyzer());
                    break;
                case "LINQ":
                    analyzerContext.SetAnalyzer(new LinqAnalyzer());
                    break;
            }

            publications = analyzerContext.AnalyzeXml(filePath);

            PopulateDropdowns(publications);

            DisplayResults(publications);
        }

        private void PopulateDropdowns(List<Publication> publications)
        {
            var faculties = publications.Select(p => p.Faculty).Distinct().ToList();
            var departments = publications.Select(p => p.Department).Distinct().ToList();

            FacultyPicker.ItemsSource = faculties;
            DepartmentPicker.ItemsSource = departments;
        }

        private void DisplayResults(IEnumerable<Publication> results)
        {
            ResultsGrid.ItemsSource = new ObservableCollection<Publication>(results);
            ResultsGrid.IsVisible = results.Any();
        }

        private async void OnInfoBtnClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Information", "Project was made by Savycheva Kateryna", "OK");
        }

        private async void OnExitBtnClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Confirmation", "Are you sure you want to leave?", "Leave", "Back");
            if (answer)
            {
                System.Environment.Exit(0);
            }
        }

        private void OnFiltersBtnClicked(object sender, EventArgs e)
        {
            FiltersPanel.IsVisible = !FiltersPanel.IsVisible;
        }

        private int? publishedYear;
        private string? titleFilter;
        private List<string>? authors;
        private string? selectedFaculty;
        private string? selectedDepartment;
        private List<Publication>? filteredPublications;

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (YearFilterCheckBox.IsChecked)
            {
                var selectedYear = (int)e.NewValue;
                SelectedYearLabel.Text = selectedYear.ToString();
                publishedYear = selectedYear;
            }
        }

        private void OnYearFilterCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                YearSlider.IsEnabled = true;
                var selectedYear = (int)YearSlider.Value;
                SelectedYearLabel.Text = selectedYear.ToString();
                publishedYear = selectedYear;
            }
            else
            {
                YearSlider.IsEnabled = false;
                publishedYear = null;
                SelectedYearLabel.Text = "N/A";
            }
        }

        private void OnTitleTextChanged(object sender, TextChangedEventArgs e)
        {
            titleFilter = TitleEntry.Text;
        }

        private void OnAuthorsTextChanged(object sender, TextChangedEventArgs e)
        {
            var authorsInput = AuthorsEntry.Text;
            authors = string.IsNullOrWhiteSpace(authorsInput)
                            ? new List<string>()
                            : authorsInput.Split(',')
                                          .Select(a => a.Trim())
                                          .ToList();
        }

        private void OnFacultySelected(object sender, EventArgs e)
        {
            selectedFaculty = FacultyPicker.SelectedItem as string;
        }

        private void OnDepartmentSelected(object sender, EventArgs e)
        {
            selectedDepartment = DepartmentPicker.SelectedItem as string;
        }


        private async void OnClearFiltersBtnClicked(object sender, EventArgs e)
        {
            if (filteredPublications != null)
            {
                filteredPublications.Clear();

                YearSlider.Value = 1975;
                SelectedYearLabel.Text = "1975";
                YearFilterCheckBox.IsChecked = false;

                FacultyPicker.SelectedIndex = -1;
                DepartmentPicker.SelectedIndex = -1;

                TitleEntry.Text = string.Empty;
                AuthorsEntry.Text = string.Empty;

                DisplayResults(publications);
            }
            else
            {
                await DisplayAlert("Error", "No filters were chosen", "OK");
            }
        }


        private async void OnSearchBtnClicked(object sender, EventArgs e)
        {
            if (publications.Count != 0)
            {
                FilterPublications();
            }
            else
            {
                await DisplayAlert("Error", $"There is no publications to apply filters", "OK");
            }
        }


        private void FilterPublications()
        {
            if(filePath == null)
            {
                throw new Exception("There is some problems with filepath");
            }
            else
            {
                this.filteredPublications = analyzerContext.ApplyFilters(filePath,
                                                                   titleFilter ?? string.Empty,
                                                                   authors ?? new List<string>(),
                                                                   publishedYear,
                                                                   selectedFaculty ?? string.Empty,
                                                                   selectedDepartment ?? string.Empty);
                DisplayResults(filteredPublications);
            }
        }


        private async void OnTransformBtnClicked(object sender, EventArgs e)
        {
            try
            {
                string choice = await DisplayActionSheet(
                    "Select Publications to Transform",
                    "Cancel",
                    null,
                    "All Publications",
                    "Filtered Publications"
                );

                switch (choice)
                {
                    case "All Publications":
                        if (filteredPublications == null)
                        {
                            await DisplayAlert("Error", $"There is no publications.", "OK");
                        }
                        else
                        {
                            await TransformToHtmlAsync(publications, "all");
                        }
                        break;

                    case "Filtered Publications":
                        if(filteredPublications == null)
                        {
                            await DisplayAlert("Error", $"There is no filtered publications.", "OK");
                        }
                        else
                        {
                           await TransformToHtmlAsync(filteredPublications, "filtered");
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error!", $"Error transforming XML to HTML: {ex.Message}", "OK");
            }
        }

        private async Task TransformToHtmlAsync(List<Publication> publications, string source)
        {
            string xmlData = SerializePublicationsToXml(publications);

            string xslFileContent;
            try
            {
                using (var stream = await FileSystem.Current.OpenAppPackageFileAsync("Assets/Transform.xsl"))
                using (var reader = new StreamReader(stream))
                {
                    xslFileContent = await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load XSL file: {ex.Message}", "OK");
                return;
            }

            string htmlResult;
            try
            {
                htmlResult = TransformXmlToHtml(xmlData, xslFileContent);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error transforming XML to HTML", ex.Message, "OK");
                return;
            }

            string fileName = $"{source}_Publications.html";
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlResult)))
            {
                await FileSaver.Default.SaveAsync(fileName, stream);
            }

            await DisplayAlert("Success", $"{source} HTML file has been saved!", "OK");
        }
        private string SerializePublicationsToXml(List<Publication> publications)
        {
            var xml = new XElement("Publications",
                publications.Select(p => new XElement("Publication",
                    new XElement("Title", p.Title),
                    new XElement("Authors", string.Join(", ", p.Authors)),
                    new XElement("PublishedYear", p.PublishedYear),
                    new XElement("Faculty", p.Faculty),
                    new XElement("Department", p.Department)
                ))
            );

            return xml.ToString();
        }

        public string TransformXmlToHtml(string xmlContent, string xslContent)
        {
            try
            {
                XslCompiledTransform xslTransform = new XslCompiledTransform();
                using (var xslReader = XmlReader.Create(new StringReader(xslContent)))
                {
                    xslTransform.Load(xslReader);
                }

                using (var xmlReader = XmlReader.Create(new StringReader(xmlContent)))
                using (var stringWriter = new StringWriter())
                {
                    xslTransform.Transform(xmlReader, null, stringWriter);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error transforming XML to HTML: {ex.Message}", ex);
            }
        }

    }
}