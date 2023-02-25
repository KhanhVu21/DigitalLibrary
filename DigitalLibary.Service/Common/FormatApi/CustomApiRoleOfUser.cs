using System;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiRoleOfUser
    {
        public CustomApiRoleOfUser()
        {

        }
        public Guid IdRole { get; set; }
        public string NameRole { get; set; }
        public Guid IdUser { get; set; }
    }
}
