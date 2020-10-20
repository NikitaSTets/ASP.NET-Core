using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASP.NET_Core_Check.Models
{
    [Bind("Test1,FirstMidName,HireDate")]
    [Serializable]
    public class TestBindAttributeModel
    {
        [BindNever]
        public int Id { get; set; }

        public string FirstMidName { get; set; }

        [BindRequired]
        public int HireDate { get; set; }

        public string LastName { get; set; }
    }
}