using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace payroll
{
    internal class Employee
    {
        [JsonPropertyName("id")]
        public object Id { get; set; }

        public string EmpId => Id?.ToString();

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("dob")]
        public string DateOfBirth { get; set; }

        [JsonPropertyName("mobile")]
        public string Mobile { get; set; }

        [JsonPropertyName("doj")]
        public string DateOfJoining { get; set; }

        [JsonPropertyName("designation")]
        public string Designation { get; set; }

        [JsonPropertyName("department")]
        public object DepartmentId { get; set; }

        [JsonPropertyName("grade")]
        public object GradeId { get; set; }
    }

    public class Department
    {
        [JsonPropertyName("id")]
        public object Id { get; set; }

        public string DeptId => Id?.ToString();

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Grade
    {
        [JsonPropertyName("id")]
        public object Id { get; set; }

        public string GradeId => Id?.ToString();

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}

