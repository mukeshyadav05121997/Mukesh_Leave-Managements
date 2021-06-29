using Leave_Managements.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Managements.Contracts
{
  public  interface ILeaveRequestRepository: IRepositoryBase<LeaveRequest>
    {
        ICollection<LeaveRequest> GetLeaveRequestByEmployee(string id);
    }
}
