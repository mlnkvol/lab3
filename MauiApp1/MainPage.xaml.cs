using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public List<Scientist> scientistsEntries;
        public FileResult file;
        public List<int> ids = new List<int>();

        public MainPage()
        {
            InitializeComponent();
            ChangehMenuItem.IsEnabled = false;
            SaveMenuItem.IsEnabled = false;
            SearchMenuItem.IsEnabled = false;
            DeleteMenuItem.IsEnabled = false;
            MessagingCenter.Subscribe<ChangeDataPage, List<Scientist>>(this, "UpdateScientists", (sender, updatedScientists) =>
            {
                Show(updatedScientists);
            });
        }

        private void OnSearchButtonClicked(object sender, EventArgs e)
        {
            bool searchByFullName = FullNameCheckBox.IsChecked;
            bool searchByFaculty = FacultyCheckBox.IsChecked;
            bool searchByPosition = PositionCheckBox.IsChecked;

            string selectedFullName = FullNameEntry.Text;
            string selectedFaculty = FacultyEntry.Text;
            string selectedPosition = PositionEntry.Text;

            if (!searchByFullName && !searchByFaculty && !searchByPosition)
            {
                Show(scientistsEntries);
                return;
            }

            var filteredEntries = scientistsEntries;

            if (searchByFullName && !string.IsNullOrEmpty(selectedFullName))
            {
                filteredEntries = filteredEntries.Where(entry => entry.FullName == selectedFullName).ToList();
            }

            if (searchByFaculty && !string.IsNullOrEmpty(selectedFaculty))
            {
                filteredEntries = filteredEntries.Where(entry => entry.Faculty == selectedFaculty).ToList();
            }

            if (searchByPosition && !string.IsNullOrEmpty(selectedPosition))
            {
                filteredEntries = filteredEntries.Where(entry => entry.Position == selectedPosition).ToList();
            }

            Show(filteredEntries);
        }

        private void OnResetButtonClicked(object sender, EventArgs e)
        {
            FullNameCheckBox.IsChecked = false;
            FacultyCheckBox.IsChecked = false;
            PositionCheckBox.IsChecked = false;
            DeleteEntry.Text = string.Empty;
            FullNameEntry.Text = string.Empty;
            FacultyEntry.Text = string.Empty;
            PositionEntry.Text = string.Empty;
            scientistsGrid.Children.Clear();
            ChangehMenuItem.IsEnabled = false;
            SaveMenuItem.IsEnabled = false;
            SearchMenuItem.IsEnabled = false;
            DeleteMenuItem.IsEnabled = false;
            file = null;
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (int.TryParse(DeleteEntry.Text, out int selectedId))
            {
                Scientist entryToRemove = scientistsEntries.FirstOrDefault(entry => entry.Id == selectedId);

                if (entryToRemove != null)
                {
                    scientistsEntries.Remove(entryToRemove);
                    ids.Remove(selectedId);
                    Show(scientistsEntries);
                    DeleteEntry.Text = string.Empty;
                }
                else if (!scientistsEntries.Any(entry => entry.Id == selectedId))
                {
                    await DisplayAlert("notification", "Scientist with this Id doesn't exist", "Ok");
                }
                else
                {
                    await DisplayAlert("notification", "Error deleting scientist", "Ok");
                }
            }
            else
            {
                await DisplayAlert("notification", "Please enter valid Id", "Ok");
            }
        }

        private async void OnOpenFileButtonClicked(object sender, EventArgs e)
        {
            file = await FilePicker.PickAsync();

            if (file != null)
            {
                if (!file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    await DisplayAlert("Notification", "Selected file is not JSON file", "Ok");
                    return;
                }

                using Stream stream = await file.OpenReadAsync();

                if (stream.Length == 0)
                {
                    await DisplayAlert("Notification", "File is empty", "Ok");
                    return;
                }

                ChangehMenuItem.IsEnabled = true;
                SaveMenuItem.IsEnabled = true;
                SearchMenuItem.IsEnabled = true;
                DeleteMenuItem.IsEnabled = true;

                using StreamReader reader = new(stream);
                string json = await reader.ReadToEndAsync();

                JsonSerializerOptions jsonOptions = new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                };

                var wrapper = JsonSerializer.Deserialize<ScientistWrapper>(json, jsonOptions);

                scientistsEntries = wrapper?.Scientists;

                if (scientistsEntries != null)
                {
                    ids = scientistsEntries.Select(scientist => scientist.Id).ToList();
                }
            }
            else
            {
                await DisplayAlert("Notification", "Failed to open file", "Ok");
            }

            Show(scientistsEntries);
        }

        private async void OnSaveFileButtonClicked(object sender, EventArgs e)
        {
            if (scientistsEntries != null && scientistsEntries.Any())
            {
                if (file != null)
                {
                    scientistsEntries = scientistsEntries.OrderBy(scientist => scientist.Id).ToList();

                    string filePath = file.FullPath;
                    using StreamWriter writer = new StreamWriter(filePath);

                    JsonSerializerOptions jsonOptions = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                        AllowTrailingCommas = true,
                        WriteIndented = true,
                    };

                    string json = JsonSerializer.Serialize(scientistsEntries, jsonOptions);
                    await writer.WriteAsync(json);
                }
            }
            else
            {
                await DisplayAlert("Notification", "Table is empty", "Ok");
            }
        }

        private async void OnAboutButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutProgramPage());
        }

        private async void OnChangeButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChangeDataPage(scientistsEntries, ids));
        }

        private async void OnExitButtonClicked(object sender, EventArgs e)
        {
            var result = await Application.Current.MainPage.DisplayAlert("Вихід", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
            if (result)
            {
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        private void Show(List<Scientist> scientists)
        {
            scientistsGrid.Children.Clear();
            int row = 0;
            if (file != null && scientists != null && scientists.Any())
            {
                List<Scientist> sortedScientists = scientists.OrderBy(scientist => scientist.Id).ToList();
                foreach (var scientistEntry in sortedScientists)
                {
                    scientistsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    Label idLabel = new Label { Text = scientistEntry.Id.ToString() };
                    Label fullNameLabel = new Label { Text = scientistEntry.FullName };
                    Label facultyLabel = new Label { Text = scientistEntry.Faculty };
                    Label departmentLabel = new Label { Text = scientistEntry.Department };
                    Label positionLabel = new Label { Text = scientistEntry.Position };
                    Label salaryLabel = new Label { Text = scientistEntry.Salary };
                    Label jobExperienceLabel = new Label { Text = scientistEntry.JobExperience };

                    Grid.SetRow(idLabel, row);
                    Grid.SetColumn(idLabel, 0);

                    Grid.SetRow(fullNameLabel, row);
                    Grid.SetColumn(fullNameLabel, 1);

                    Grid.SetRow(facultyLabel, row);
                    Grid.SetColumn(facultyLabel, 2);

                    Grid.SetRow(departmentLabel, row);
                    Grid.SetColumn(departmentLabel, 3);

                    Grid.SetRow(positionLabel, row);
                    Grid.SetColumn(positionLabel, 4);

                    Grid.SetRow(salaryLabel, row);
                    Grid.SetColumn(salaryLabel, 5);

                    Grid.SetRow(jobExperienceLabel, row);
                    Grid.SetColumn(jobExperienceLabel, 6);

                    scientistsGrid.Children.Add(idLabel);
                    scientistsGrid.Children.Add(fullNameLabel);
                    scientistsGrid.Children.Add(facultyLabel);
                    scientistsGrid.Children.Add(departmentLabel);
                    scientistsGrid.Children.Add(positionLabel);
                    scientistsGrid.Children.Add(salaryLabel);
                    scientistsGrid.Children.Add(jobExperienceLabel);

                    row++;
                }
            }
        }
    }
}
