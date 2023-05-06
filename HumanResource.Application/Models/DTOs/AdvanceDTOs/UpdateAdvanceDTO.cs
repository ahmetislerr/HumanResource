﻿using HumanResource.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResource.Application.Models.DTOs.AdvanceDTOs
{
	public class UpdateAdvanceDTO
	{
		public int Id { get; set; }
        [Required(ErrorMessage = "Amount field cannot be empty!")]
        [Range(0, 99999.99, ErrorMessage = "Please enter between 0-99999.99!")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Installment field cannot be empty!")]
        [Range(0, 20, ErrorMessage = "Please enter between 0-20.")]
        public int NumberOfInstallments { get; set; }

        public DateTime ModifiedDate => DateTime.Now;

        [Required(ErrorMessage = "Update date cannot be blank!"), DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
        public Guid UserId { get; set; }
        [ValidateNever]
        public int StatuId { get; set; }

        //ToDo: Tarih kısıtlaması için attibute yazılacak.
    }
}
