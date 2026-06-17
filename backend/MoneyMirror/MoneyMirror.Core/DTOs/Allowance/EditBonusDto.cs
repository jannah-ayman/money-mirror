using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

﻿namespace MoneyMirror.Core.DTOs.Allowance
{
    public class EditBonusDto
    {
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
    }
}
