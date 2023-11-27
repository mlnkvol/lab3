namespace MauiApp1;

public partial class ChangeDataPage : ContentPage
{
    private List<Scientist> scientistsEntries;
    private List<int> ids;
    public ChangeDataPage(List<Scientist> scientistsEntries, List<int> ids)
    {
        InitializeComponent();
        applyButton.IsEnabled = false;
        this.scientistsEntries = scientistsEntries; 
        this.ids = ids;
    }

    private async void OnAddButtonClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(IdEntry.Text, out int id) || id < 0)
        {
            await DisplayAlert("Notification", "Enter valid id", "Ok");
            return;
        }
        if (ids.Contains(id))
        {
            await DisplayAlert("Notification", "Scientist with this ID already exists", "Ok");
            return;
        }
        string fullName = FullNameEntry.Text;
        if (string.IsNullOrWhiteSpace(fullName))
        {
            await DisplayAlert("Notification", "Enter full name of scientist", "Ok");
            return;
        }

        string faculty = FacultyEntry.Text;
        if (string.IsNullOrEmpty(faculty))
        {
            await DisplayAlert("Notification", "Enter faculty of scientist", "Ok");
            return;
        }
        string department = DepartmentEntry.Text;
        string position = PositionEntry.Text;
        string salary = SalaryEntry.Text;
        string jobExperience = JobExperienceEntry.Text;

        Scientist newScientist = new Scientist
        {
            Id = id,
            FullName = fullName,
            Faculty = faculty,
            Department = department,
            Position = position,
            Salary = salary,
            JobExperience = jobExperience
        };

        scientistsEntries.Add(newScientist);

        IdEntry.Text = string.Empty;
        FullNameEntry.Text = string.Empty;
        FacultyEntry.Text = string.Empty;
        DepartmentEntry.Text = string.Empty;
        PositionEntry.Text = string.Empty;
        SalaryEntry.Text = string.Empty;
        JobExperienceEntry.Text = string.Empty;
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        if (int.TryParse(IdEntry.Text, out int enteredId))
        {
            if (!ids.Contains(enteredId))
            {
                await DisplayAlert("Notification", "Scientist with this ID does not exist", "Ok");
            }
            else
            {
                int idToEdit = int.Parse(IdEntry.Text);

                Scientist scientistToEdit = scientistsEntries.FirstOrDefault(scientist => scientist.Id == idToEdit);

                if (scientistToEdit != null)
                {
                    FullNameEntry.Text = scientistToEdit.FullName;
                    FacultyEntry.Text = scientistToEdit.Faculty;
                    DepartmentEntry.Text = scientistToEdit.Department;
                    PositionEntry.Text = scientistToEdit.Position;
                    SalaryEntry.Text = scientistToEdit.Salary;
                    JobExperienceEntry.Text = scientistToEdit.JobExperience;
                }
                applyButton.IsEnabled = true;
            }
        }
        else
        {
            await DisplayAlert("Notification", "Enter valid Id", "Ok");
        }
    }

    private void OnApplyButtonClicked(object sender, EventArgs e)
    {
        int idToEdit = int.Parse(IdEntry.Text);

        Scientist scientistToEdit = scientistsEntries.FirstOrDefault(scientist => scientist.Id == idToEdit);

        if (scientistToEdit != null)
        {
            scientistToEdit.FullName = FullNameEntry.Text;
            scientistToEdit.Faculty = FacultyEntry.Text;
            scientistToEdit.Department = DepartmentEntry.Text;
            scientistToEdit.Position = PositionEntry.Text;
            scientistToEdit.Salary = SalaryEntry.Text;
            scientistToEdit.JobExperience = JobExperienceEntry.Text;
        }

        IdEntry.Text = string.Empty;
        FullNameEntry.Text = string.Empty;
        FacultyEntry.Text = string.Empty;
        DepartmentEntry.Text = string.Empty;
        PositionEntry.Text = string.Empty;
        SalaryEntry.Text = string.Empty;
        JobExperienceEntry.Text = string.Empty;
        applyButton.IsEnabled = false;
    }

    private async void OnMainPageButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
        MessagingCenter.Send(this, "UpdateScientists", scientistsEntries);
    }
}