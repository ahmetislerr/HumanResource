﻿using HumanResource.Application.Models.DTOs.LeaveDTO;
using HumanResource.Application.Models.VMs.CompanyManagerVMs;
using HumanResource.Application.Models.VMs.LeaveVM;

namespace HumanResource.Application.Services.LeaveServices
{
    public interface ILeaveService
    {

        Task<bool> Create(CreateLeaveDTO model, string userName);
        Task<bool> Update(UpdateLeaveDTO model);
        Task Delete(int id);
        Task<UpdateLeaveDTO> GetById(int id);
        Task<LeaveDetailVM> LeaveDetail(int id);
        Task<List<LeaveVM>> GetLeavesForPersonel(Guid id);
        Task<ProcessVM> Approve(int id);
        Task<ProcessVM> Reject(int id);



    }
}
