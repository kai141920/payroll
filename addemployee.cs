using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace payroll
{
    public partial class addemployee : Form
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiBaseUrl = "http://localhost:3000/api";

        public addemployee()
        {
            InitializeComponent();
            _ = LoadDropdownDataAsync();
        }

        private async Task LoadDropdownDataAsync()
        {
            try
            {
                // Configure JSON options to handle number/string conversion
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };

                // Load departments
                var deptResponse = await _httpClient.GetAsync($"{ApiBaseUrl}/departments");
                deptResponse.EnsureSuccessStatusCode();
                var jsonString = await deptResponse.Content.ReadAsStringAsync();
                var departments = JsonSerializer.Deserialize<List<Department>>(jsonString, options);

                cmbDepartment.DataSource = departments;
                cmbDepartment.DisplayMember = "Name";
                cmbDepartment.ValueMember = "Id";

                // Load grades - with debug logging
                var gradeResponse = await _httpClient.GetAsync($"{ApiBaseUrl}/grades");
                gradeResponse.EnsureSuccessStatusCode();
                var gradeJson = await gradeResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Grade API Response: " + gradeJson); // Debug output

                var grades = JsonSerializer.Deserialize<List<Grades>>(gradeJson, options);
                Console.WriteLine("Deserialized Grades Count: " + grades?.Count); // Debug output

                cmbGrade.DataSource = grades;
                cmbGrade.DisplayMember = "Grade";
                cmbGrade.ValueMember = "Id";

                // Force UI update
                cmbGrade.Refresh();
                cmbDepartment.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dropdown data: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAddEmployee_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                var newEmployee = new
                {
                    name = txtName.Text.Trim(),
                    dob = dtpDOB.Value.ToString("yyyy-MM-dd"),
                    mobile = txtMobile.Text.Trim(),
                    doj = dtpDOJ.Value.ToString("yyyy-MM-dd"),
                    designation = txtDesignation.Text.Trim(),
                    department = cmbDepartment.SelectedValue?.ToString(),
                    grade = cmbGrade.SelectedValue?.ToString()
                };

                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/employees", newEmployee);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Employee added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Form1 f1 = new Form1();
                    this.Hide();
                    f1.Show();
                    Close();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to add employee: {error}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter employee name", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMobile.Text) || txtMobile.Text.Length < 10)
            {
                MessageBox.Show("Please enter a valid 10-digit mobile number", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbDepartment.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a department", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbGrade.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a grade", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Model classes with proper JSON property names
        private class Department
        {
            [JsonPropertyName("id")]
            public object Id { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        private class Grades
        {
            [JsonPropertyName("id")]
            public object Id { get; set; }
            [JsonPropertyName("grade")]
            public string Grade { get; set; }

            [JsonPropertyName("salary")]
            public string Salary { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            option op = new option();
            this.Hide();
            op.Show();
        }

        private void addemployee_Load(object sender, EventArgs e)
        {

        }
    }
}