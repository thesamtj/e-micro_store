using System;
using System.Collections.Generic;

namespace Ordering.Infrastructure.Test.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string? StudentName { get; set; }

    public int DepartmentId { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public virtual Department Department { get; set; } = null!;
}
