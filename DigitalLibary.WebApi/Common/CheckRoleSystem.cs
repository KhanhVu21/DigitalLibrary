using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.WebApi.Helper;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace DigitalLibary.WebApi.Common
{
    public class CheckRoleSystem
    {
        #region Variables
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        #endregion

        #region Contructor
        public CheckRoleSystem(JwtService jwtService, IUserRepository userRepository)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
        }
        #endregion

        #region FUNCTION
        public CheckAdminModel CheckAdmin(string headerValue)
        {
            try
            {
                // init checkadmin model
                CheckAdminModel checkAdminModel = new CheckAdminModel();
                var jwt = headerValue.ToString().Split(' ');
                // check jwt exits
                if (jwt[1].Length == 0)
                {
                    checkAdminModel.check = false;
                }
                // verify json web token
                var token = _jwtService.Verify(jwt[1]);

                // check security of jwt
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken tokenS = handler.ReadToken(jwt[1].ToString()) as JwtSecurityToken;
                string profile = tokenS.Claims.First(claim => claim.Type == "email").Value;
                //get user from database by email
                var user = _userRepository.getUserByEmail(profile);
                // get list role of user
                List<User_Role> user_Role = _userRepository.getListRoleOfUser(user.Id);
                Role role = new Role();
                // init variable check in list role of user
                bool checkListRole = false;
                // loop throught list role and check exits role admin
                for(int i = 0; i < user_Role.Count; i++)
                {
                    role = _userRepository.getUserRolebyId(new Guid(user_Role[i].IdRole));
                    if(role != null)
                    {
                        if(role.RoleName == "Admin")
                        {
                            checkListRole = true;
                            break;
                        }
                    }
                }

                if (checkListRole)
                {
                    checkAdminModel.check = true;
                    checkAdminModel.Id = user.Id;
                }
                else
                {
                    checkAdminModel.check = false;
                }
                return checkAdminModel;
            }
            catch (Exception)
            {
                throw;
            }
        }   
        #endregion
    }
}
