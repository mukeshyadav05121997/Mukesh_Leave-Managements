﻿using Leave_Managements.Contracts;
using Leave_Managements.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Managements.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool Create(LeaveRequest entity)
        {
             _db.LeaveRequests.Add(entity);
            return Save();
        }

        public bool Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return Save();
        }

        public ICollection<LeaveRequest> FindAll()
        {
            var leaverequest = _db.LeaveRequests
                .Include(q=>q.RequestingEmployee)
                .Include(q=>q.ApprovedBy)
                .Include(q=>q.LeaveType)
                .ToList();
            return leaverequest;
        }

        public LeaveRequest FindById(int id)
        {
            var leaverequest = _db.LeaveRequests
                .Include(q => q.RequestingEmployee)
                .Include(q => q.ApprovedBy)
                .Include(q => q.LeaveType)
                .FirstOrDefault(q=>q.Id==id);
                
            return leaverequest;
        }

        public ICollection<LeaveRequest> GetLeaveRequestByEmployee(string id)
        {
            var leaverequest = _db.LeaveRequests
                 .Include(q => q.RequestingEmployee)
                 .Include(q => q.ApprovedBy)
                 .Include(q => q.LeaveType)
                 
                .Where(q => q.RequestingEmployeeId == id).ToList();
            return leaverequest;
        }

        public bool IsExist(int id)
        {
            var exists = _db.LeaveRequests.Any(q => q.Id == id);
            return exists;
        }

        public bool Save()
        {
            var change = _db.SaveChanges();
            return change > 0;
          
        }

        public bool Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return Save();
        }
    }
}
