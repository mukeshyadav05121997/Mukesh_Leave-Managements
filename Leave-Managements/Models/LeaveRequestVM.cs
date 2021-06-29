
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Managements.Models
{
    public class LeaveRequestVM
    {

        public int Id { get; set; }

        public EmployeeVM RequestingEmployee { get; set; }
        public string RequestingEmployeeId { get; set; }
        [Display(Name = "Start Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public LeaveTypeVM LeaveType { get; set; }
        public int LeaveId { get; set; }
        [Display(Name = "Date Requested")]
        public DateTime DateRequested { get; set; }
        [Display(Name = "Data Actioned")]
        public DateTime DateActioned { get; set; }
        public bool? Approved { get; set; }

        public EmployeeVM ApprovedBy { get; set; }
        public string ApprovedById { get; set; }
    }

    public class AdminLeaveRequestViewVM
    {
        [Display(Name = "Total Request")]
        public int TotalRequest { get; set; }
        [Display(Name = "Approved Request")]
        public int ApprovedRequest { get; set; }
        [Display(Name = " Pending Request")]
        public int PendingRequest { get; set; }
        [Display(Name = "Rejected Request")]
        public int RejectedRequest { get; set; }
        public List<LeaveRequestVM> LeaveRequests { get; set; }

    }
    public class CreateLeaveRequestVM
    {
        [Display(Name = "Leave Type")]
        public int LeaveId { get; set; }
        [Display(Name = "Start Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public IEnumerable<SelectListItem> LeaveTypes { get; set; }

    }

    public class EmployeeLeaveRequestViewVM
        {
        public List<LeaveAllocationVM> LeaveAllocations { get; set; }
        public List<LeaveRequestVM> LeaveRequests { get; set; }
        }

}
