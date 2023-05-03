﻿using AutoMapper;
using HumanResource.Application.Models.DTOs.LeaveDTO;
using HumanResource.Application.Models.VMs.LeaveVM;
using HumanResource.Application.Services.PersonelService;
using HumanResource.Domain.Entities;
using HumanResource.Domain.Enums;
using HumanResource.Domain.Repositries;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace HumanResource.Application.Services.LeaveServices
{
    public class LeaveService : ILeaveService
    {
        private readonly IPersonelService _personelService;
        private readonly ILeaveRepository _leaveRepository;
        private readonly IMapper _mapper;


        public LeaveService(ILeaveRepository leaveRepository, IMapper mapper, IPersonelService personelService)
        {
            _leaveRepository = leaveRepository;
            _mapper = mapper;
            _personelService = personelService;
        }

        public async Task<bool> Create(CreateLeaveDTO model, string userName)
        {
            model.Statu.Name = Status.AwatingApproval.ToString();
            model.Statu.StatuEnumId = Status.AwatingApproval.GetHashCode();
            Leave leave = _mapper.Map<Leave>(model);
            leave.UserId = await _personelService.GetPersonelId(userName);
            return await _leaveRepository.Add(leave);
        }

        public async Task Delete(int id)
        {
            Leave leave = await _leaveRepository.GetDefault(x => x.Id == id);
            leave.StatuId =Status.Deleted.GetHashCode();
            leave.DeletedDate = DateTime.Now;
            await _leaveRepository.Delete(leave);
        }

        public async Task<UpdateLeaveDTO> GetById(int id)
        {
            Leave leave = await _leaveRepository.GetDefault(x => x.Id == id);
            return _mapper.Map<UpdateLeaveDTO>(leave);
        }

        public async Task<List<LeaveVM>> GetLeavesForPersonel(Guid id)
        {
            var comments = await _leaveRepository.GetFilteredList(
                select: x => new LeaveVM()
                {
                    Id = x.Id,
                    StartDate = x.StartDate.ToShortDateString(),
                    CreatedDate = x.CreatedDate.ToShortDateString(),
                    EndDate = x.EndDate.ToShortDateString(),
                    ReturnDate = x.ReturnDate.ToShortDateString(),
                    LeavePeriod = x.LeavePeriod,
                    LeaveType = x.LeaveType.Name
                },
                
                where: x => x.User.Id == id && x.StatuId != Status.Deleted.GetHashCode(),
                orderby: x => x.OrderByDescending(x => x.CreatedDate),
                include: x => x.Include(x => x.User).Include(x => x.LeaveType).Include(x=>x.Statu)
                );

            return comments;
        }

        public async Task<bool> Update(UpdateLeaveDTO model)
        {
            Leave leave = _mapper.Map<Leave>(model);
            return await _leaveRepository.Update(leave);
        }
    }
}
