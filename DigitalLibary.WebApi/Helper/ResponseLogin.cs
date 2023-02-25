using DigitalLibary.Service.Common.FormatApi;
using System.Collections.Generic;

namespace DigitalLibary.WebApi.Helper
{
    public class ResponseLogin
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TokenModel Data { get; set; }
        public string Role { get; set; }
        public List<CustomApiRoleOfUser> RoleList { get; set; }
    }
}
