using DigitalLibary.Data.Entity;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiCountUserByUserType
    {
        public CustomApiCountUserByUserType()
        {

        }
        public UserType UserType { get; set; }
        public int NumberUser { get; set; }
    }
}
