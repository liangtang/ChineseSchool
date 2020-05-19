using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChineseSchool.Entities;

namespace ChineseSchool.Models
{
    public class InvoiceViewModel
    {
        public Parent Parent { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }
        public Invoice Invoice { get; set; }
    }
}