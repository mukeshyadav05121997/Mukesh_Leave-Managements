using AutoMapper;
using Leave_Managements.Contracts;
using Leave_Managements.Data;
using Leave_Managements.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Managements.Controllers
{

    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepository _leaverequestRepo;
        private readonly ILeaveAlloactionRepository _leaveallocationrepo;
        private readonly ILeaveTypeRepository _leavetypeRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;
        // GET: LeaveRequestController
        public LeaveRequestController
    (
        ILeaveRequestRepository leaverequestRepo,
        ILeaveAlloactionRepository leaveallocationrepo,
        ILeaveTypeRepository leavetypeRepo,
        IMapper mapper,
        UserManager<Employee> userManager
    )
        {
            _leaverequestRepo = leaverequestRepo;
            _leaveallocationrepo = leaveallocationrepo;
            _leavetypeRepo = leavetypeRepo;
            _mapper = mapper;
            _userManager = userManager;

        }
       


        public ActionResult Index()
        {
            var leaveRequests = _leaverequestRepo.FindAll();
            var leaveRequestsModel = _mapper.Map < List<LeaveRequestVM>>(leaveRequests);
            var model = new AdminLeaveRequestViewVM
            {
                TotalRequest = leaveRequestsModel.Count,
                ApprovedRequest = leaveRequestsModel.Count(q => q.Approved == true),
                PendingRequest = leaveRequestsModel.Count(q => q.Approved == null),
                RejectedRequest = leaveRequestsModel.Count(q => q.Approved == false),
                LeaveRequests=leaveRequestsModel
            };
            return View(model);
        }

        // GET: LeaveRequestController/Details/5
        public ActionResult Details(int id)
        {
            var leaverequest = _leaverequestRepo.FindById(id);
            var model = _mapper.Map<LeaveRequestVM>(leaverequest);
            return View(model);
        }

        public ActionResult ApproveRequest(int id)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var leaverequest = _leaverequestRepo.FindById(id);
                leaverequest.Approved = true;
                leaverequest.ApprovedById = user.Id;
                leaverequest.DateActioned = DateTime.Now;

                var isSuccess = _leaverequestRepo.Update(leaverequest);
                return RedirectToAction(nameof(Index));

            }
            catch(Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
           
        }
        public ActionResult RejectRequest(int id)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var leaverequest = _leaverequestRepo.FindById(id);
                leaverequest.Approved = false;
                leaverequest.ApprovedById = user.Id;
                leaverequest.DateActioned = DateTime.Now;

                var isSuccess = _leaverequestRepo.Update(leaverequest);
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }
        [Authorize(Roles = "Employee")]
        public ActionResult MyLeave()
        {
            var employee = _userManager.GetUserAsync(User).Result;
            var employeeid = employee.Id;
            var employeeallocation = _leaveallocationrepo.GetLeaveAllocationsByEmployee(employeeid);
            var employeerequest = _leaverequestRepo.GetLeaveRequestByEmployee(employeeid);
            var employeeallocationmodel = _mapper.Map<List<LeaveAllocationVM>>(employeeallocation);
            var employeerequestmodel = _mapper.Map<List<LeaveRequestVM>>(employeerequest);

            var model = new EmployeeLeaveRequestViewVM
            {
                LeaveAllocations = employeeallocationmodel,
                LeaveRequests=employeerequestmodel

            };
            return View(model);
               


            
        }

        // GET: LeaveRequestController/Create
        [Authorize(Roles = "Employee")]
        public ActionResult Create()
        {
            var leavetype = _leavetypeRepo.FindAll();
            var leavetypeitem = leavetype.Select(
                q=>new SelectListItem
                {
                    Text=q.Name,
                    Value=q.Id.ToString()

                }
                );
            var model = new CreateLeaveRequestVM
            {
               LeaveTypes=leavetypeitem
            };
            return View(model);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestVM model)
        {
           
            try
            {
                var leavetype = _leavetypeRepo.FindAll();
                var leavetypeitem = leavetype.Select(
                    q => new SelectListItem
                    {
                        Text = q.Name,
                        Value = q.Id.ToString()

                    }
                    );
                model.LeaveTypes = leavetypeitem;
                if(!ModelState.IsValid)
                {
                    return View(model);

                }
                if(DateTime.Compare(model.StartDate,model.EndDate)>1)
                {
                    ModelState.AddModelError("","StartDate is greater then EndDate");
                    return View(model);

                }
                var employee = _userManager.GetUserAsync(User).Result;
                var allocation = _leaveallocationrepo.GetLeaveAllocationsByEmployeeandtype(employee.Id,model.LeaveId);
                int DayRequested =(int)(model.EndDate - model.StartDate).TotalDays;

                if(DayRequested>allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "You do not have sufficient days");
                    return View(model);
                }
                else
                {
                    if (DayRequested==0)
                    {
                        ModelState.AddModelError("", "You do not have sufficient days");
                        return View(model);
                    }

                }


                var leaverequested = new LeaveRequestVM
                {
                    RequestingEmployeeId=employee.Id,
                    StartDate=model.StartDate,
                    EndDate=model.EndDate,
                    Approved=null,
                    DateRequested=DateTime.Now,
                    DateActioned=DateTime.Now,
                    LeaveId=model.LeaveId
                    
                };
                var leaveRequest = _mapper.Map<LeaveRequest>(leaverequested);
                var isSuccess = _leaverequestRepo.Create(leaveRequest);
                if(!isSuccess)
                {
                    ModelState.AddModelError("","Something went wrong during entering the value");
                    return View(model);

                }

                return RedirectToAction("MyLeave");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("","Something went wrong");
                return View(model);
            }
        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

