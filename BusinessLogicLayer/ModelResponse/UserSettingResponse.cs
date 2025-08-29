using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ModelResponse
{
    public class UserSettingResponse
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string TwoFactorStatus { get; set; }
    }
}
