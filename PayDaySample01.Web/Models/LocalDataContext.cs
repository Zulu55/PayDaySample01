﻿using PayDaySample01.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayDaySample01.Web.Models
{
    public class LocalDataContext : DataContext
    {
        public System.Data.Entity.DbSet<PayDaySample01.Domain.Models.Employee> Employees { get; set; }
    }
}