using System;
using System.Collections.Generic;

namespace Ordering.Infrastructure.Test.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string? DepartmentName { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
