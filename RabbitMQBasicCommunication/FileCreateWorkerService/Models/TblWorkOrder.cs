using System;
using System.Collections.Generic;

namespace FileCreateWorkerService.Models
{
    public partial class TblWorkOrder
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string TaskId { get; set; } = null!;
        public int StatusId { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsOpen { get; set; }
    }
}
