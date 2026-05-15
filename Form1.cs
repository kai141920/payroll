using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Text.Json;

namespace payroll
{
    public partial class Form1 : Form
    {
        private HttpClient _httpClient = new HttpClient();
        private string _apiUrl = "http://localhost:3000/api";

        public Form1()
        {
            InitializeComponent();

            // ✅ Attach event
            dataGridViewEmployees.CellContentClick += dataGridViewEmployees_CellContentClick;

            // ✅ Setup button column
            SetupRemoveButton();

            _ = LoadEmployeeData();
        }

        // =========================
        // ADD REMOVE BUTTON COLUMN
        // =========================
        private void SetupRemoveButton()
        {
            // Prevent duplicate column
            if (dataGridViewEmployees.Columns["colRemove"] == null)
            {
                DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
                btn.Name = "colRemove";
                btn.HeaderText = "Action";
                btn.Text = "Remove";
                btn.UseColumnTextForButtonValue = true;

                dataGridViewEmployees.Columns.Add(btn);
            }
        }

        // =========================
        // LOAD DATA FROM API
        // =========================
        private async Task LoadEmployeeData()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };

                var employees = await _httpClient.GetFromJsonAsync<List<Employee>>(
                    $"{_apiUrl}/employees", options);

                dataGridViewEmployees.Rows.Clear();

                foreach (var emp in employees)
                {
                    dataGridViewEmployees.Rows.Add(
                        emp.Id,
                        emp.Name,
                        emp.DateOfBirth,
                        emp.Mobile,
                        emp.DepartmentName ?? "(Unknown Department)",
                        emp.GradeName ?? "(Unknown Grade)",
                        emp.Designation
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // REMOVE BUTTON CLICK
        // =========================
        private async void dataGridViewEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Check if Remove button clicked
            if (dataGridViewEmployees.Columns[e.ColumnIndex].Name == "colRemove")
            {
                var idCell = dataGridViewEmployees.Rows[e.RowIndex].Cells["colId"].Value;

                if (idCell == null)
                {
                    MessageBox.Show("Invalid employee ID.");
                    return;
                }

                string id = idCell.ToString();

                var confirm = MessageBox.Show("Delete this employee?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"{_apiUrl}/employees/{id}");

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Deleted successfully!");
                            await LoadEmployeeData();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete. Check API.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting: {ex.Message}");
                    }
                }
            }
        }

        // =========================
        // REFRESH BUTTON
        // =========================
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadEmployeeData();
        }

        // =========================
        // ADD EMPLOYEE FORM
        // =========================
        private void btnAddEmployee_Click_1(object sender, EventArgs e)
        {
            option op = new option();
            this.Hide();
            op.Show();
        }

        // =========================
        // MODEL CLASS
        // =========================
        public class Employee
        {
            public object Id { get; set; }
            public string Name { get; set; }
            public string DateOfBirth { get; set; }
            public string Mobile { get; set; }
            public string Designation { get; set; }
            public string DepartmentName { get; set; }
            public string GradeName { get; set; }
        }
    }
}