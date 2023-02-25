using AutoMapper;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;
using DigitalLibary.WebApi.Payload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        #region Variables
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly SaveToDiary _saveToDiary;
        #endregion

        #region Contructor
        public UserController(JwtService jwtService,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        SaveToDiary saveToDiary,
        IUnitRepository unitRepository,
        IUserRepository userRepository)
        {
            _jwtService = jwtService;
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _userRepository = userRepository;
            _unitRepository = unitRepository;
            _saveToDiary = saveToDiary;

        }
        #endregion

        #region Method CRUD AND LOGIN REGISTER USER
        // GET: api/User/MutiplePrintLibraryCards
        [HttpPost]
        [Route("MutiplePrintLibraryCards")]
        public IActionResult MutiplePrintLibraryCards(List<Guid> IdUsers)
        {
            return Ok(_userRepository.mutiplePrintLibraryCards(IdUsers));
        }
        // GET: api/User/GetListUser
        [HttpPost]
        [Route("GetListUser")]
        public List<UserDTO> GetListUser(SortAndSearchListUsser sortAndSearchListUsser)
        {
            try
            {
                // Initialize a list of UserDTO objects
                var userDTOs = new List<UserDTO>();
                // Assign the result of the getAllUser method to the userDTOs list
                userDTOs = _userRepository.getAllUser(sortAndSearchListUsser);

                // Get a list of all roles
                var allRole = _userRepository.getAllRole();

                // Iterate over each user in the userDTOs list
                for (int i = 0; i < userDTOs.Count; i++)
                {
                    // Get the list of roles for the current user
                    var userRole = _userRepository.getListRoleOfUser(userDTOs[i].Id);
                    // Initialize a list of Role objects
                    var roles = new List<Role>();
                    // Iterate over each role in the userRole list
                    for (int j = 0; j < userRole.Count; j++)
                    {
                        // Find the role in the allRole list that has the same Id as the IdRole of the current role
                        var role = allRole.FirstOrDefault(e => e.Id == new Guid(userRole[j].IdRole));
                        // If the role is not null, add it to the roles list
                        if (role != null)
                        {
                            roles.Add(role);
                        }
                    }
                    // Assign the roles list to the ListRole property of the current userDTO
                    userDTOs[i].ListRole = roles;
                }

                // Return the userDTOs list
                return userDTOs;
            }
            catch (Exception)
            {
                // Catch any exceptions and re-throw them
                throw;
            }
        }
        // GET: api/User/GetListRoleOfUser
        [HttpGet]
        [Route("GetListRoleOfUser")]
        public List<CustomApiRoleOfUser> GetListRoleOfUser(Guid IdUser)
        {
            try
            {
                // get list id in table user table
                List<User_Role> user_Role_List = _userRepository.getListRoleOfUser(IdUser);
                List<CustomApiRoleOfUser> customApiRoleOfUsers = new List<CustomApiRoleOfUser>();

                for (int i = 0; i < user_Role_List.Count; i++)
                {
                    Role roleOfUser = _userRepository.getUserRolebyId(new Guid(user_Role_List[i].IdRole));
                    CustomApiRoleOfUser customApiRoleOfUser = new CustomApiRoleOfUser();
                    customApiRoleOfUser.IdRole = roleOfUser.Id;
                    customApiRoleOfUser.NameRole = roleOfUser.RoleName;
                    customApiRoleOfUser.IdUser = new Guid(user_Role_List[i].IdUser);

                    customApiRoleOfUsers.Add(customApiRoleOfUser);
                }

                return customApiRoleOfUsers;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/User/GetAllUserNotBlocked
        [HttpGet]
        [Route("GetAllUserNotBlocked")]
        public List<UserModel> GetAllUserNotBlocked(int pageNumber, int pageSize)
        {
            try
            {
                List<UserDTO> userDTOs = new List<UserDTO>();
                userDTOs = _userRepository.getAllUserNotBlocked(pageNumber, pageSize);

                List<UserModel> userModels = new List<UserModel>();
                userModels = _mapper.Map<List<UserModel>>(userDTOs);

                for (int i = 0; i < userModels.Count; i++)
                {
                    List<User_Role> user_Role = _userRepository.getListRoleOfUser(userModels[i].Id);
                    Role roleList = new Role();

                    for (int j = 0; j < user_Role.Count; j++)
                    {
                        roleList = _userRepository.getUserRolebyId(new Guid(user_Role[j].IdRole));
                        if (roleList != null)
                        {
                            userModels[i].ListRole.Add(roleList);
                        }
                    }
                }
                return userModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/User/GetUserById?Id
        [HttpGet]
        [Route("GetUserById")]
        public UserModel GetUserById(Guid Id)
        {
            try
            {
                UserModel user = new UserModel();
                UserDTO userDTOs = new UserDTO();

                userDTOs = _userRepository.getUserByID(Id);
                user = _mapper.Map<UserModel>(userDTOs);
                List<User_Role> user_Role = _userRepository.getListRoleOfUser(userDTOs.Id);
                Role roleList = new Role();

                for (int i = 0; i < user_Role.Count; i++)
                {
                    roleList = _userRepository.getUserRolebyId(new Guid(user_Role[i].IdRole));
                    if (roleList != null)
                    {
                        user.ListRole.Add(roleList);
                    }
                }

                return user;

            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/User/GetAllUser?pageNumber=1&pageSize=2
        [HttpGet]
        [Route("GetAllUser")]
        public List<UserModel> GetAllUser(int pageNumber, int pageSize)
        {
            try
            {
                List<UserDTO> userDTOs = new List<UserDTO>();
                userDTOs = _userRepository.getAllUser(pageNumber, pageSize);

                List<UserModel> userModels = new List<UserModel>();
                userModels = _mapper.Map<List<UserModel>>(userDTOs);

                for (int i = 0; i < userModels.Count; i++)
                {
                    List<User_Role> user_Role = _userRepository.getListRoleOfUser(userModels[i].Id);
                    Role roleList = new Role();

                    for (int j = 0; j < user_Role.Count; j++)
                    {
                        roleList = _userRepository.getUserRolebyId(new Guid(user_Role[j].IdRole));
                        if (roleList != null)
                        {
                            userModels[i].ListRole.Add(roleList);
                        }
                    }
                }
                return userModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/User/GetAllRole
        [HttpGet]
        [Route("GetAllRole")]
        public List<Role> GetAllRole()
        {
            try
            {
                List<Role> listRole = new List<Role>();
                listRole = _userRepository.getAllRole();
                return listRole;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/User/GetAllUserType
        [HttpGet]
        [Route("GetAllUserType")]
        public List<UserType> GetAllUserType()
        {
            try
            {
                List<UserType> userTypes = new List<UserType>();
                userTypes = _userRepository.getAllUserType();
                return userTypes;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/User/GetAllUnit
        [HttpGet]
        [Route("GetAllUnit")]
        public List<Unit> GetAllUnit()
        {
            try
            {
                List<Unit> units = new List<Unit>();
                units = _userRepository.getAllUnit();
                return units;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/User/GetUser
        [HttpGet]
        [Route("GetUser")]
        public IActionResult GetUser()
        {
            try
            {
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                var jwt = headerValue.ToString().Split(' ');

                if (jwt[1].Length == 0)
                {
                    return BadRequest(new { message = "Không tìm thấy token !" });
                }

                var token = _jwtService.Verify(jwt[1]);

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken tokenS = handler.ReadToken(jwt[1].ToString()) as JwtSecurityToken;
                string profile = tokenS.Claims.First(claim => claim.Type == "email").Value;
                string expire = tokenS.Claims.First(claim => claim.Type == "exp").Value;


                double doubleVal = Convert.ToDouble(expire);
                DateTime DateAfterConvert = UnixTimeStampToDateTime(doubleVal);

                if (DateAfterConvert < DateTime.Now)
                {
                    return Ok(new
                    {
                        Message = "This token was expired",
                        expire = true
                    });
                }

                var user = _userRepository.getUserByEmail(profile);
                if (user != null && user.IsLocked)
                {
                    return Ok(new
                    {
                        Message = "This account was locked"
                    });
                }

                User_Role user_Role = _userRepository.getRoleOfUser(user.Id);
                Role role = _userRepository.getUserRolebyId(new Guid(user_Role.IdRole));

                // get list id in table user table
                List<User_Role> user_Role_List = _userRepository.getListRoleOfUser(user.Id);
                List<CustomApiRoleOfUser> customApiRoleOfUsers = new List<CustomApiRoleOfUser>();

                for (int i = 0; i < user_Role_List.Count; i++)
                {
                    Role roleOfUser = _userRepository.getUserRolebyId(new Guid(user_Role_List[i].IdRole));
                    CustomApiRoleOfUser customApiRoleOfUser = new CustomApiRoleOfUser();
                    customApiRoleOfUser.IdRole = roleOfUser.Id;
                    customApiRoleOfUser.NameRole = roleOfUser.RoleName;
                    customApiRoleOfUser.IdUser = new Guid(user_Role_List[i].IdUser);

                    customApiRoleOfUsers.Add(customApiRoleOfUser);
                }
                return Ok(new
                {
                    Data = user,
                    RoleName = role.RoleName,
                    listRole = customApiRoleOfUsers
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Message = "This token was expired or error",
                });
                throw;
            }
        }
        // HttpPost: /api/User/Register
        [HttpPost]
        [Route("Register")]
        public IActionResult Register(UserDTO userDto)
        {
            try
            {
                //check email exist
                User userByEmail = _userRepository.getUserByEmail(userDto.Email);
                // get role from database
                Role role = _userRepository.getUserRole(_appSettingModel.RoleDefault);

                // get id Unit and user Type
                Unit unit = _userRepository.getUnit(_appSettingModel.Unit);
                UserType userType = _userRepository.getTypeUser(_appSettingModel.UserTypeDefault);

                if (userByEmail != null && userByEmail.IsActive == true)
                    return BadRequest(new { message = "Tài khoản này đã được kích hoạt" });

                if (userByEmail != null)
                    return BadRequest(new { message = "Tài khoản này chưa được kích hoạt" });

                //generate user code 
                Unit unitEntity = _unitRepository.LoadUnitByID(unit.Id);
                string UnitCode = "";
                int UnitCodeMax = 0;

                if (unitEntity != null)
                {
                    UnitCode = unitEntity.UnitCode;

                    UnitCodeMax = _userRepository.GetMaxUnitCode(UnitCode, unit.Id);
                    UnitCodeMax += 1;
                }

                Random generator = new Random();
                string CodeActive = generator.Next(0, 1000000).ToString("D6");

                if (userByEmail == null)
                {
                    //save user
                    User user = new User()
                    {
                        Id = Guid.NewGuid(),
                        Fullname = userDto.Fullname,
                        Email = userDto.Email,
                        Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                        CreatedDate = DateTime.Now,
                        CreatedBy = Guid.NewGuid(),
                        IsDeleted = false,
                        IsLocked = false,
                        IsActive = false,
                        UserTypeId = userType.Id,
                        UnitId = unit.Id,
                        ActiveCode = CodeActive,
                        UserCode = UnitCode + UnitCodeMax.ToString(),
                        AcitveUser = DateTime.Now,
                        ExpireDayUser = DateTime.Now.AddYears(1)
                    };
                    _userRepository.CreateUser(user);

                    User_Role userRole = new User_Role()
                    {
                        Id = Guid.NewGuid(),
                        IdRole = role.Id.ToString(),
                        IdUser = user.Id.ToString()
                    };
                    _userRepository.CreateUserRole(userRole);
                }

                // prepare content for mail
                string fromMail = _appSettingModel.fromMail;
                string password = _appSettingModel.password;
                string Subject = "Vui lòng xác thực tài khoản !";
                string body =
                $"<div style='max-width: 700px; margin: auto; border: 10px solid #ddd; padding: 50px 20px; font-size: 110%;'>" +
                $"<h2 style='text-align: center; text-transform: uppercase;color: teal;'> Chào mừng bạn đến với thư viện </h2>" +
                $"<p>Chúc mừng bạn đã đăng kí thành công tài khoản. Hãy lấy mã và kích hoạt tài khoản của bạn !</p>" +
                $"<a style='background: crimson; text-decoration: none; color: white; padding: 10px 20px; margin-left: 300px; display: inline-block;text-align:center'>{CodeActive}</a>";

                //send mail confirm
                SendMail sendMail = new SendMail();
                sendMail.SendMailAuto(fromMail, userDto.Email, password, body, Subject);

                return Ok(new
                {
                    message = "Kiểm tra mail của bạn và kích hoạt tài khoản !"
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        // HttpPost: /api/User/Login
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(UserDTO userDto)
        {
            try
            {
                // get one user by email
                User userByEmail = _userRepository.getUserByEmail(userDto.Email);

                if (userByEmail != null && userByEmail.ExpireDayUser <= DateTime.Now)
                {
                    return BadRequest(new { message = "Tài khoản này đã hết hạn không thể đăng nhập !" });
                }

                if (userByEmail != null && userByEmail.IsLocked)
                {
                    return BadRequest(new { message = "This account was locked !" });
                }
                // get id in table user table
                User_Role user_Role = _userRepository.getRoleOfUser(userByEmail.Id);
                // get role of user login current
                Role role = _userRepository.getUserRolebyId(new Guid(user_Role.IdRole));

                if (userByEmail == null) return BadRequest(new { message = "Tài khoản không tồn tại !" });

                if (userByEmail.IsActive == false) return BadRequest(new { message = "Tài khoản này chưa được kich hoạt !" });

                if (!BCrypt.Net.BCrypt.Verify(userDto.Password, userByEmail.Password))
                {
                    return BadRequest(new { message = "Mật khẩu không chính xác" });
                }

                // get list id in table user table
                List<User_Role> user_Role_List = _userRepository.getListRoleOfUser(userByEmail.Id);
                List<CustomApiRoleOfUser> customApiRoleOfUsers = new List<CustomApiRoleOfUser>();

                for (int i = 0; i < user_Role_List.Count; i++)
                {
                    Role roleOfUser = _userRepository.getUserRolebyId(new Guid(user_Role_List[i].IdRole));
                    CustomApiRoleOfUser customApiRoleOfUser = new CustomApiRoleOfUser();
                    customApiRoleOfUser.IdRole = roleOfUser.Id;
                    customApiRoleOfUser.NameRole = roleOfUser.RoleName;
                    customApiRoleOfUser.IdUser = new Guid(user_Role_List[i].IdUser);

                    customApiRoleOfUsers.Add(customApiRoleOfUser);
                }

                //cấp token
                var token = _jwtService.GenerateToken(userByEmail);

                Response.Cookies.Append("jwt", token.AccessToken, new CookieOptions
                {
                    HttpOnly = true
                });

                return Ok(new ResponseLogin
                {
                    Success = true,
                    Message = "Xác thực thành công !",
                    Data = token,
                    Role = role.RoleName,
                    RoleList = customApiRoleOfUsers
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        // HttpPost: /api/User/Logout
        [HttpPost]
        [Route("Logout")]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Delete("jwt");

                return Ok(new
                {
                    message = "Đăng xuất thành công !"
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        // HttpPost: /api/User/ActiveUserByCode
        [HttpPost]
        [Route("ActiveUserByCode")]
        public async Task<IActionResult> ActiveUserByCode(string email, string code)
        {
            try
            {
                User userByEmail = _userRepository.getUserByEmail(email);

                if (userByEmail == null) return BadRequest(new { message = "Tài khoản này chưa tồn tại" });

                if (userByEmail != null && userByEmail.IsActive == true)
                    return Ok(new { message = "Tài khoản này đã được kích hoạt" });

                if (userByEmail.ActiveCode != code) return BadRequest(new { message = "Vui lòng nhập đúng mã code" });

                var status = await _userRepository.ActiveUserByCode(email, code);

                if (status.Success)
                {
                    _saveToDiary.SaveDiary(userByEmail.Id, "Update", "User", true, userByEmail.Id);
                    return Ok(new
                    {
                        message = "Tài khoản của bạn đã được kích hoạt !"
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(userByEmail.Id, "Update", "User", false, userByEmail.Id);
                    return BadRequest(new { message = "Kích hoạt không thành công !" });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // HttpPost: /api/User/SendAgainCode
        [HttpPost]
        [Route("SendAgainCode")]
        public IActionResult SendAgainCode(string email)
        {
            try
            {
                User userByEmail = _userRepository.getUserByEmail(email);

                if (userByEmail == null) return BadRequest(new { message = "Tài khoản này chưa tồn tại" });

                if (userByEmail != null && userByEmail.IsActive == true)
                    return BadRequest(new { message = "Tài khoản này đã được kích hoạt" });

                string CodeActive = userByEmail.ActiveCode;
                string fromMail = _appSettingModel.fromMail;
                string password = _appSettingModel.password;
                string Subject = "Vui lòng xác thực tài khoản !";
                string body =
                $"<div style='max-width: 700px; margin: auto; border: 10px solid #ddd; padding: 50px 20px; font-size: 110%;'>" +
                $"<h2 style='text-align: center; text-transform: uppercase;color: teal;'> Chào mừng bạn đến với thư viện </h2>" +
                $"<p>Chúc mừng bạn đã đăng kí thành công tài khoản. Hãy lấy mã và kích hoạt tài khoản của bạn !</p>" +
                $"<a style='background: crimson; text-decoration: none; color: white; padding: 10px 20px; margin-left: 300px; display: inline-block;text-align:center'>{CodeActive}</a>";

                //send mail confirm
                SendMail sendMail = new SendMail();
                sendMail.SendMailAuto(fromMail, email, password, body, Subject);

                _saveToDiary.ModifyDiary(userByEmail.Id, "Update", "CategorySignV1", true, userByEmail.Id, "yêu cầu gửi lại mã xác thực");

                return Ok(new
                {
                    message = "Kiểm tra mail của bạn !"
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        // HttpPost: /api/User/SendCodeWithAccountActive
        [HttpPost]
        [Route("SendCodeWithAccountActive")]
        public IActionResult SendCodeWithAccountActive(string email)
        {
            try
            {
                User userByEmail = _userRepository.getUserByEmail(email);

                if (userByEmail == null) return BadRequest(new { message = "Tài khoản này chưa tồn tại" });

                if (userByEmail != null && userByEmail.IsActive == true)
                {
                    Random generator = new Random();
                    string CodeActive = generator.Next(0, 1000000).ToString("D6");

                    Response resuilt = _userRepository.UpdateActiveCode(CodeActive, userByEmail.Email);

                    if (resuilt.Success)
                    {
                        string fromMail = _appSettingModel.fromMail;
                        string password = _appSettingModel.password;
                        string Subject = "Vui lòng xác thực tài khoản !";
                        string body =
                        $"<div style='max-width: 700px; margin: auto; border: 10px solid #ddd; padding: 50px 20px; font-size: 110%;'>" +
                        $"<h2 style='text-align: center; text-transform: uppercase;color: teal;'> Chào mừng bạn đến với thư viện </h2>" +
                        $"<p>Chúc mừng bạn đã lấy thành công mã kich hoạt tài khoản. Hãy lấy mã và kích hoạt tài khoản của bạn !</p>" +
                        $"<a style='background: crimson; text-decoration: none; color: white; padding: 10px 20px; margin-left: 300px; display: inline-block;text-align:center'>{CodeActive}</a>";

                        //send mail confirm
                        SendMail sendMail = new SendMail();
                        sendMail.SendMailAuto(fromMail, email, password, body, Subject);

                        return Ok(new
                        {
                            message = "Kiểm tra mail của bạn !"
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            message = "Không thể tạo mã code !"
                        });
                    }

                }
                else
                {
                    return Ok(new
                    {
                        message = "Tài khoản của bạn chưa được kích hoạt hoặc gmail không chính xác !"
                    });
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        // HttpPost: /api/User/VerifyCode
        [HttpPost]
        [Route("VerifyCode")]
        public IActionResult VerifyCode(string code, string email)
        {
            try
            {
                User userByEmail = _userRepository.getUserByEmail(email);

                if (userByEmail == null) return BadRequest(new { message = "Tài khoản này chưa tồn tại" });

                if (userByEmail != null && userByEmail.IsActive == true && userByEmail.ActiveCode == code)
                {
                    return Ok(new
                    {
                        message = "Xác thực thành công !",
                        status = true
                    });
                }
                else
                {
                    return Ok(new
                    {
                        message = "Vui lòng nhập chính xác thông tin !",
                        status = false
                    });
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        // HttpPost: /api/User/ForgotPassWord
        [HttpPost]
        [Route("ForgotPassWord")]
        public IActionResult ForgotPassWord(string email, string newPassword)
        {
            try
            {
                User userByEmail = _userRepository.getUserByEmail(email);

                if (userByEmail == null) return BadRequest(new { message = "Tài khoản này chưa tồn tại" });
                if (userByEmail != null && userByEmail.IsActive == false) return BadRequest(new { message = "Tài khoản này chưa được kich hoạt" });

                Response resuilt = _userRepository.UpdatePassword(email, BCrypt.Net.BCrypt.HashPassword(newPassword));

                if (resuilt.Success)
                {
                    _saveToDiary.ModifyDiary(userByEmail.Id, "Update", "CategorySignV1", true, userByEmail.Id, "quên mật khẩu");
                    return Ok(new
                    {
                        message = resuilt.Message,
                    });
                }
                else
                {
                    return Ok(new
                    {
                        message = resuilt.Message
                    });
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        // HttpPost: /api/User/ChangePassWord
        [HttpPost]
        [Route("ChangePassWord")]
        public IActionResult ChangePassWord(string email, string oldPassword, string newPassword)
        {
            try
            {
                User userByEmail = _userRepository.getUserByEmail(email);

                if (userByEmail == null) return BadRequest(new { message = "Tài khoản này chưa tồn tại" });
                if (userByEmail != null && userByEmail.IsActive == false) return BadRequest(new { message = "Tài khoản này chưa được kich hoạt" });
                if (!BCrypt.Net.BCrypt.Verify(oldPassword, userByEmail.Password))
                    return BadRequest(new { message = "Mật khẩu cũ không chính xác" });

                Response resuilt = _userRepository.UpdatePassword(email, BCrypt.Net.BCrypt.HashPassword(newPassword));

                if (resuilt.Success)
                {
                    _saveToDiary.ModifyDiary(userByEmail.Id, "Update", "CategorySignV1", true, userByEmail.Id, "thay đổi mật khẩu");
                    return Ok(new
                    {
                        message = resuilt.Message,
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = resuilt.Message
                    });
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        // HttpPost: /api/User/RemoveUser
        [HttpPost]
        [Route("RemoveUser")]
        public async Task<IActionResult> RemoveUser(Guid Id)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if (headerValue.Count == 0)
                {
                    return Ok(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return Ok(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                if (checkModel != null) IdUserCurrent = checkModel.Id;

                Response response = new Response();
                response = await _userRepository.RemoveUser(Id);

                if (response.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "User", true, Id);
                    return Ok(new
                    {
                        message = response.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "User", false, Id);
                    return BadRequest(new
                    {
                        message = response.Message,
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "User", false, Id);
                throw;
            }
        }
        // HttpPost: /api/User/LockUserAccount
        [HttpPost]
        [Route("LockUserAccount")]
        public async Task<IActionResult> LockUserAccount(Guid Id, bool isLock)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            string content = isLock ? "khóa tài khoản user" : "bỏ khóa tài khoản user";
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if (headerValue.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                if (checkModel != null) IdUserCurrent = checkModel.Id;

                Response response = new Response();
                response = await _userRepository.LockAccountUser(Id, isLock);

                if (response.Success)
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "User", true, Id, content);
                    return Ok(new
                    {
                        message = response.Message,
                    });
                }
                else
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "User", false, Id, content);
                    return BadRequest(new
                    {
                        message = response.Message,
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.ModifyDiary(IdUserCurrent, "Update", "User", false, Id, content);
                throw;
            }
        }
        // HttpPost: /api/User/UpdateUser
        [HttpPost]
        [Route("UpdateUser")]
        public IActionResult UpdateUser([FromForm] UserModel userModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (checkModel != null) IdUserCurrent = checkModel.Id;

                Response response = new Response();
                UserDTO userDTO = new UserDTO();
                userDTO = _mapper.Map<UserDTO>(userModel);
                userDTO.Phone = userModel.Phone == "undefined" || userModel.Phone == "null" ? null : userModel.Phone;
                userDTO.Address = userModel.Address == "undefined" || userModel.Address == "null" ? null : userModel.Address;
                userDTO.Description = userModel.Description == "undefined" || userModel.Description == "null" ? null : userModel.Description;

                if (userModel.AcitveUser is "null" or null or "undefined")
                {
                    userDTO.AcitveUser = DateTime.Now;
                }
                else
                {
                    DateTime AcitveUser = DateTime.ParseExact(userModel.AcitveUser, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    userDTO.AcitveUser = AcitveUser;
                }

                if (userModel.ExpireDayUser is "null" or null or "undefined")
                {
                    userDTO.ExpireDayUser = DateTime.Now.AddYears(1);
                }
                else
                {
                    DateTime ExpireDayUser = DateTime.ParseExact(userModel.ExpireDayUser, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    userDTO.ExpireDayUser = ExpireDayUser;
                }

                //delete file image user
                if (userModel.idFile is null)
                {
                    string IdFile = userDTO.Id.ToString() + ".jpg";

                    // set file path to save file
                    var filename = Path.Combine(_appSettingModel.ServerFileAvartar, Path.GetFileName(IdFile));
                    //delete file before save
                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }
                }

                // check file exits
                if (Request.Form.Files.Count > 0)
                {
                    foreach (var file in Request.Form.Files)
                    {
                        var fileContentType = file.ContentType;

                        if (fileContentType == "image/jpeg" || fileContentType == "image/png"
                            || fileContentType == "image/jpg")
                        {
                            // prepare path to save file image
                            string pathTo = _appSettingModel.ServerFileAvartar;
                            // get extention form file name
                            string IdFile = userDTO.Id.ToString() + ".jpg";

                            // set file path to save file
                            var filename = Path.Combine(pathTo, Path.GetFileName(IdFile));

                            //delete file before save
                            if (System.IO.File.Exists(filename))
                            {
                                System.IO.File.Delete(filename);
                            }

                            // save file
                            using (var stream = System.IO.File.Create(filename))
                            {
                                file.CopyTo(stream);
                            }
                            // set data document avatar
                            userDTO.Avatar = IdFile;
                        }
                    }
                }

                response = _userRepository.UpdateUser(userDTO);

                if (response.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "User", true, userModel.Id);
                    return Ok(new
                    {
                        message = response.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "User", false, userModel.Id);

                    return BadRequest(new
                    {
                        message = response.Message,
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "User", false, userModel.Id);
                throw;
            }
        }
        // HttpPost: /api/User/InsertUser
        [HttpPost]
        [Route("InsertUser")]
        public IActionResult InsertUser([FromForm] UserModel userModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid IdUser = Guid.NewGuid();
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if (headerValue.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                if (checkModel != null) IdUserCurrent = checkModel.Id;


                //check exits email
                User userByEmail = _userRepository.getUserByEmail(userModel.Email);
                if (userByEmail != null) return BadRequest(new { message = "Email đã tồn tại !" });

                UserDTO userDTO = new UserDTO();
                userDTO = _mapper.Map<UserDTO>(userModel);

                //generate code user
                Random generator = new Random();
                string CodeActive = generator.Next(0, 1000000).ToString("D6");

                // define some col with data concrete
                userDTO.Id = Guid.NewGuid();
                userDTO.Status = 0;
                userDTO.CreatedDate = DateTime.Now;
                userDTO.IsLocked = false;
                userDTO.IsDeleted = false;
                userDTO.IsActive = false;
                userDTO.ActiveCode = CodeActive;
                userDTO.CreatedBy = checkModel.Id;
                userDTO.Password = BCrypt.Net.BCrypt.HashPassword(userModel.Password);
                userDTO.Phone = userModel.Phone == "undefined" ? null : userModel.Phone;
                userDTO.Address = userModel.Address == "undefined" ? null : userModel.Address;
                userDTO.Email = userDTO.Email.Trim();

                if (userModel.AcitveUser is "null" or null or "undefined")
                {
                    userDTO.AcitveUser = DateTime.Now;
                }
                else
                {
                    DateTime AcitveUser = DateTime.ParseExact(userModel.AcitveUser, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    userDTO.AcitveUser = AcitveUser;
                }

                if (userModel.ExpireDayUser is "null" or null or "undefined")
                {
                    userDTO.ExpireDayUser = DateTime.Now.AddYears(1);
                }
                else
                {
                    DateTime ExpireDayUser = DateTime.ParseExact(userModel.ExpireDayUser, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    userDTO.ExpireDayUser = ExpireDayUser;
                }

                IdUser = userDTO.Id;

                //generate unit code user
                Unit unit = _unitRepository.LoadUnitByID(userDTO.UnitId);
                if (unit != null)
                {
                    string UnitCode = unit.UnitCode;

                    int UnitCodeMax = _userRepository.GetMaxUnitCode(UnitCode, userDTO.Id);
                    UnitCodeMax += 1;
                    userDTO.UserCode = UnitCode + UnitCodeMax.ToString();
                }

                //save image user
                if (Request.Form.Files.Count > 0)
                {
                    foreach (var file in Request.Form.Files)
                    {
                        var fileContentType = file.ContentType;

                        if (fileContentType == "image/jpeg"
                        || fileContentType == "image/png" || fileContentType == "image/jpg")
                        {

                            // prepare path to save file image
                            string pathTo = _appSettingModel.ServerFileAvartar;
                            // get extention form file name
                            string IdFile = userDTO.Id.ToString() + ".jpg";

                            // set file path to save file
                            var filename = Path.Combine(pathTo, Path.GetFileName(IdFile));

                            // save file
                            using (var stream = System.IO.File.Create(filename))
                            {
                                file.CopyTo(stream);
                            }
                            userDTO.Avatar = IdFile;
                        }
                    }
                }
                else userDTO.Avatar = null;

                //save to table user
                Response result = _userRepository.InsertUser(userDTO);

                if (!result.Success)
                {
                    return BadRequest(new { message = "Thêm tài khoản không thành công !" });
                }

                //save to table user_role
                result = _userRepository.InsertRoleInUserRole(userDTO.Id.ToString(), "Admin");

                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "User", true, IdUser);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "User", false, IdUser);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "User", false, IdUser);
                throw;
            }
        }
        // HttpPost: /api/User/AddRoleUser
        [HttpPost]
        [Route("AddRoleUser")]
        public IActionResult AddRoleUser(User_RoleModel user_RoleModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if (headerValue.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }

                if (checkModel != null) IdUserCurrent = checkModel.Id;

                //save to table user_role
                Response result = _userRepository.InsertUser_Role(user_RoleModel.IdRole, user_RoleModel.IdUser);

                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "User_Role", true, new Guid(user_RoleModel.IdRole));
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "User_Role", false, new Guid(user_RoleModel.IdRole));
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "User_Role", false, new Guid(user_RoleModel.IdRole));
                throw;
            }
        }
        // HttpPost: /api/User/DeleteUserRole
        [HttpPost]
        [Route("DeleteUserRole")]
        public IActionResult DeleteUserRole(User_RoleModel user_RoleModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if (headerValue.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                if (checkModel != null) IdUserCurrent = checkModel.Id;


                //delete to table user_role
                Response result = _userRepository.DeleteUser_Role(user_RoleModel.IdUser, user_RoleModel.IdRole);

                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "User_Role", true, new Guid(user_RoleModel.IdRole));
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "User_Role", false, new Guid(user_RoleModel.IdRole));
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "User_Role", false, new Guid(user_RoleModel.IdRole));
                throw;
            }
        }
        // HttpPost: /api/User/InsertUserByExcel
        [HttpPost]
        [Route("InsertUserByExcel")]
        public IActionResult ReadFileExcel()
        {
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if (headerValue.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }

                bool checkAddUser = false;
                var addUserErrorByExcels = new List<UserDTO>();

                using (var package = new ExcelPackage(Request.Form.Files[0].OpenReadStream()))
                {
                    // Get the first worksheet in the workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    // Read the cell values in the worksheet
                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var userDTO = new UserDTO();
                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        {
                            string cellValue = worksheet.Cells[row, col].Value?.ToString();

                            if(cellValue != null)
                            {
                                if (col == 1) userDTO.Fullname = cellValue;
                                else if (col == 2) userDTO.Description = cellValue;
                                else if (col == 3) userDTO.Password = cellValue;
                                else if (col == 4) userDTO.Email = cellValue.Trim();
                                else if (col == 5) userDTO.Phone = cellValue;
                                else if (col == 6) userDTO.UserTypeId = new Guid(cellValue);
                                else if (col == 7) userDTO.Address = cellValue;
                                else if (col == 8) userDTO.UnitId = new Guid(cellValue);
                                else if (col == 9)
                                {
                                    var dt = new DateTime();
                                    var culutreInfo = new CultureInfo("vi-VN");
                                    string[] formats = { "dd/MM/yyyy" };
                                    if (!DateTime.TryParseExact(cellValue, formats,
                                                    culutreInfo,
                                                    DateTimeStyles.None, out dt))
                                    {
                                        userDTO.AcitveUser = DateTime.Now;
                                    }
                                    else
                                    {
                                        if (IsValidDate(cellValue))
                                        {
                                            // Use the ParseExact method to convert the string to a DateTime object
                                            DateTime dateTime = DateTime.ParseExact(cellValue, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                            // Use the Now property to get the current time
                                            DateTime now = DateTime.Now;

                                            // Create a new DateTime object with the date from the input string and the time from the current time
                                            var result = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, now.Hour, now.Minute, now.Second);
                                            userDTO.AcitveUser = result;
                                        }
                                        if (!IsValidDate(cellValue))
                                        {
                                            userDTO.AcitveUser = DateTime.Now;
                                        }
                                    }
                                }
                                else if (col == 10)
                                {
                                    var dt = new DateTime();
                                    var culutreInfo = new CultureInfo("vi-VN");
                                    string[] formats = { "dd/MM/yyyy" };
                                    if (!DateTime.TryParseExact(cellValue, formats,
                                                    culutreInfo,
                                                    DateTimeStyles.None, out dt))
                                    {
                                        userDTO.ExpireDayUser = DateTime.Now.AddYears(1);
                                    }
                                    else
                                    {
                                        if (IsValidDate(cellValue))
                                        {
                                            // Use the ParseExact method to convert the string to a DateTime object
                                            DateTime dateTime = DateTime.ParseExact(cellValue, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                            // Use the Now property to get the current time
                                            DateTime now = DateTime.Now;

                                            // Create a new DateTime object with the date from the input string and the time from the current time
                                            var result = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, now.Hour, now.Minute, now.Second);

                                            userDTO.ExpireDayUser = result;
                                        }
                                        if (!IsValidDate(cellValue))
                                        {
                                            userDTO.ExpireDayUser = DateTime.Now.AddYears(1);
                                        }
                                    }
                                }
                            }
                        }
                        if (userDTO.Email == null || userDTO.Password == null)
                        {
                            continue;
                        }
                        //generate unit code user
                        Unit unit = _unitRepository.LoadUnitByID(userDTO.UnitId);
                        string userCode = "";
                        if (unit != null)
                        {
                            string UnitCode = unit.UnitCode;

                            int UnitCodeMax = _userRepository.GetMaxUnitCode(UnitCode, userDTO.UnitId);
                            UnitCodeMax += 1;
                            userCode = UnitCode + UnitCodeMax.ToString();
                        }

                        // Create a new System.Random random number generator
                        var generator = new Random();

                        // Generate a random six-digit code
                        string codeActive = generator.Next(0, 1000000).ToString("D6");

                        userDTO.Id = Guid.NewGuid();
                        userDTO.Status = 0;
                        userDTO.CreatedDate = DateTime.Now;
                        userDTO.UserCode = userCode;
                        userDTO.IsLocked = false;
                        userDTO.IsDeleted = false;
                        userDTO.IsActive = true;
                        userDTO.CreatedBy = checkModel.Id;
                        userDTO.ActiveCode = codeActive;

                        //check exits email
                        var userByEmail = _userRepository.getUserByEmail(userDTO.Email);
                        var response = new Response();
                        if (userByEmail == null)
                        {
                            userDTO.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
                            response = _userRepository.InsertUser(userDTO);
                            //insert user role
                            _userRepository.InsertRoleInUserRole(userDTO.Id.ToString(), "User");
                        }
                        if (!response.Success || userByEmail is not null)
                        {
                            checkAddUser = true;
                            addUserErrorByExcels.Add(userDTO);
                        }
                    }
                }

                if (checkAddUser)
                {
                    var pathGetFileImportUser = string.Concat(_appSettingModel.ServerFileExcel, "\\", "Mau_ImportUser.xlsx");
                    var fIfor = new FileInfo(pathGetFileImportUser);

                    using (var excelPackage = new ExcelPackage(fIfor))
                    {
                        //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                        ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[0];

                        namedWorksheet.Cells["A2:J2000"].Clear();

                        // get all table unit
                        int row = 2;
                        for (int i = 0; i < addUserErrorByExcels.Count; i++)
                        {
                            namedWorksheet.Cells[row, 1].Value = addUserErrorByExcels[i].Fullname;
                            namedWorksheet.Cells[row, 2].Value = addUserErrorByExcels[i].Description;
                            namedWorksheet.Cells[row, 3].Value = addUserErrorByExcels[i].Password;
                            namedWorksheet.Cells[row, 4].Value = addUserErrorByExcels[i].Email;
                            namedWorksheet.Cells[row, 5].Value = addUserErrorByExcels[i].Phone;
                            namedWorksheet.Cells[row, 6].Value = addUserErrorByExcels[i].UserTypeId;
                            namedWorksheet.Cells[row, 7].Value = addUserErrorByExcels[i].Address;
                            namedWorksheet.Cells[row, 8].Value = addUserErrorByExcels[i].UnitId;
                            namedWorksheet.Cells[row, 9].Value = addUserErrorByExcels[i].AcitveUser?.ToString("dd/MM/yyyy");
                            namedWorksheet.Cells[row, 10].Value = addUserErrorByExcels[i].ExpireDayUser?.ToString("dd/MM/yyyy");
                            row++;
                        }

                        FileInfo fiToSave = new FileInfo(pathGetFileImportUser);
                        //Save your file
                        excelPackage.SaveAs(fiToSave);
                    }
                    //download file excel
                    byte[] fileBytes = System.IO.File.ReadAllBytes(pathGetFileImportUser);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Danh sách thêm không thành công.xlsx");
                }
                else
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Thêm mới thành công"
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/User/GetFileExcelImportExcel
        [HttpGet]
        [Route("GetFileExcelImportExcel")]
        public IActionResult GetFileExcelImportExcel()
        {
            try
            {
                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", "Mau_ImportUser.xlsx");
                FileInfo fi = new FileInfo(path);

                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {
                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheetUser = excelPackage.Workbook.Worksheets[0];
                    namedWorksheetUser.Cells["A2:J2000"].Clear();

                    namedWorksheetUser.Cells[2, 9].Value = DateTime.Now.ToString("dd/MM/yyyy");
                    namedWorksheetUser.Cells[2, 10].Value = DateTime.Now.AddYears(1).ToString("dd/MM/yyyy");


                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[1];
                    namedWorksheet.Cells["A2:B200"].Clear();

                    // get all table unit
                    List<Unit> units = new List<Unit>();
                    units = _userRepository.getAllUnit();
                    int row = 2;
                    for (int i = 0; i < units.Count; i++)
                    {
                        namedWorksheet.Cells[row, 1].Value = units[i].Id;
                        namedWorksheet.Cells[row, 2].Value = units[i].UnitName;
                        row++;
                    }

                    // load data from table user type
                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet nameTypeSheet = excelPackage.Workbook.Worksheets[2];

                    // get all table unit
                    List<UserType> userTypes = new List<UserType>();
                    userTypes = _userRepository.getAllUserType();
                    row = 2;
                    for (int i = 0; i < userTypes.Count; i++)
                    {
                        nameTypeSheet.Cells[row, 1].Value = userTypes[i].Id;
                        nameTypeSheet.Cells[row, 2].Value = userTypes[i].TypeName;
                        row++;
                    }
                    FileInfo fiToSave = new FileInfo(path);
                    //Save your file
                    excelPackage.SaveAs(fiToSave);
                }
                //download file excel
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Mau_ThemMoiNguoiDung.xlsx");
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/User/GetFileExcelUserByIdUnit
        [HttpGet]
        [Route("GetFileExcelUserByIdUnit")]
        public IActionResult GetFileExcelUserByIdUnit(Guid IdUnit)
        {
            try
            {
                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", "Mau_UserByUnitId.xlsx");
                FileInfo fi = new FileInfo(path);

                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {
                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheetUser = excelPackage.Workbook.Worksheets[0];
                    namedWorksheetUser.Cells["A2:E2000"].Clear();

                    // get all user by id unit
                    List<UserDTO> userDTOs = _userRepository.getAllUserByIdUnit(IdUnit);
                    int row = 2;
                    for (int i = 0; i < userDTOs.Count; i++)
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            if (i % 2 == 0)
                            {
                                Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#E5DFDF");
                                namedWorksheetUser.Cells[row, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                namedWorksheetUser.Cells[row, j].Style.Fill.BackgroundColor.SetColor(colFromHex);
                            }
                            namedWorksheetUser.Cells[row, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            namedWorksheetUser.Cells[row, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            namedWorksheetUser.Cells[row, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            namedWorksheetUser.Cells[row, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            namedWorksheetUser.Cells[row, j].Style.Font.Size = 13;
                            namedWorksheetUser.Cells[row, j].Style.Font.Name = "Times New Roman";
                        }

                        namedWorksheetUser.Cells[row, 1].Value = userDTOs[i].Fullname;
                        namedWorksheetUser.Cells[row, 2].Value = userDTOs[i].Email;
                        namedWorksheetUser.Cells[row, 3].Value = userDTOs[i].Phone;
                        namedWorksheetUser.Cells[row, 4].Value = userDTOs[i].AcitveUser?.ToString("dd/MM/yyyy");
                        namedWorksheetUser.Cells[row, 5].Value = userDTOs[i].ExpireDayUser?.ToString("dd/MM/yyyy");
                        row++;
                    }

                    //FileInfo fiToSave = new FileInfo(path);
                    //Save your file
                    excelPackage.SaveAs(fi);
                }
                //download file excel
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Mau_Chỉnh sửa theo phòng ban.xlsx");
            }
            catch (Exception)
            {
                throw;
            }
        }
        // HttpPost: /api/User/UpdateActiveAndExpireDateUser
        [HttpPost]
        [Route("UpdateActiveAndExpireDateUser")]
        public async Task<IActionResult> UpdateActiveAndExpireDateUser()
        {
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if (headerValue.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }

                string IdFile = "";
                // check file exit and save file to sevrer
                if (Request.Form.Files.Count > 0)
                {
                    foreach (var file in Request.Form.Files)
                    {
                        string pathTo = _appSettingModel.ServerFileExcel;
                        IdFile = DateTimeOffset.Now.ToUnixTimeSeconds().ToString() + ".xlsx";

                        // set file path to save file
                        var filename = Path.Combine(pathTo, Path.GetFileName(IdFile));

                        // save file
                        using (var stream = System.IO.File.Create(filename))
                        {
                            file.CopyTo(stream);
                        }
                    }
                }

                // path to open file excel
                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", IdFile);
                FileInfo fi = new FileInfo(path);

                bool checkUpdateUser = false;
                List<UpdateDateUserErrorByExcel> addUserErrorByExcels = new List<UpdateDateUserErrorByExcel>();

                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {
                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[0];

                    int rowCount = namedWorksheet.Dimension.Rows;
                    int colCount = namedWorksheet.Dimension.Columns;
                    for (int i = 2; i <= rowCount; i++)
                    {
                        UserDTO userDTO = new UserDTO();
                        bool checkValidDate = false;
                        for (int j = 1; j <= colCount; j++)
                        {
                            string cells = "";
                            if (namedWorksheet.Cells[i, j].Value != null)
                            {
                                cells = namedWorksheet.Cells[i, j].Value.ToString();

                                if (j == 1) userDTO.Fullname = cells;
                                if (j == 2) userDTO.Email = cells;
                                if (j == 3) userDTO.Phone = cells;
                                if (j == 4)
                                {
                                    DateTime dt;
                                    CultureInfo culutreInfo = new CultureInfo("vi-VN");
                                    string[] formats = { "dd/MM/yyyy" };
                                    if (!DateTime.TryParseExact(cells, formats,
                                                    culutreInfo,
                                                    DateTimeStyles.None, out dt))
                                    {
                                        checkValidDate = true;
                                    }
                                    else
                                    {
                                        if (IsValidDate(cells))
                                        {
                                            //success code
                                            DateTime ActiveUser = DateTime.ParseExact(cells, "dd/MM/yyyy", culutreInfo);
                                            userDTO.AcitveUser = ActiveUser;
                                        }
                                        if (!IsValidDate(cells))
                                        {
                                            checkValidDate = true;
                                        }
                                    }
                                }
                                if (j == 5)
                                {
                                    DateTime dt;
                                    CultureInfo culutreInfo = new CultureInfo("vi-VN");
                                    string[] formats = { "dd/MM/yyyy" };
                                    if (!DateTime.TryParseExact(cells, formats,
                                                    culutreInfo,
                                                    DateTimeStyles.None, out dt))
                                    {
                                        checkValidDate = true;
                                    }
                                    else
                                    {
                                        if (IsValidDate(cells))
                                        {
                                            //success code
                                            DateTime ExpireDayUser = DateTime.ParseExact(cells, "dd/MM/yyyy", culutreInfo);
                                            userDTO.ExpireDayUser = ExpireDayUser;
                                        }
                                        if (!IsValidDate(cells))
                                        {
                                            checkValidDate = true;
                                        }
                                    }                              
                                }
                            }
                        }
                        if (userDTO.Email == null)
                        {
                            continue;
                        }
                        if (!checkValidDate)
                        {
                            var result = await _userRepository.UpdateActiveAndExpireDateUser(userDTO);
                            if (!result.Success)
                            {
                                checkUpdateUser = true;
                                var updateDateUserErrorByExcel = new UpdateDateUserErrorByExcel()
                                {
                                    Fullname = userDTO.Fullname,
                                    Email = userDTO.Email,
                                    Phone = userDTO.Phone,
                                    AcitveUser = namedWorksheet.Cells[i, 4].Value.ToString(),
                                    ExpireDayUser = namedWorksheet.Cells[i, 5].Value.ToString(),
                                };
                                addUserErrorByExcels.Add(updateDateUserErrorByExcel);
                            }
                        }
                        else
                        {
                            checkUpdateUser = true;
                            var updateDateUserErrorByExcel = new UpdateDateUserErrorByExcel()
                            {
                                Fullname = userDTO.Fullname,
                                Email = userDTO.Email,
                                Phone = userDTO.Phone,
                                AcitveUser = namedWorksheet.Cells[i, 4].Value.ToString(),
                                ExpireDayUser = namedWorksheet.Cells[i, 5].Value.ToString(),
                            };
                            addUserErrorByExcels.Add(updateDateUserErrorByExcel);
                        }
                    }
                }

                if (checkUpdateUser)
                {
                    var pathGetFileUpdateUser = string.Concat(_appSettingModel.ServerFileExcel, "\\", "Mau_UserByUnitId.xlsx");
                    FileInfo fIfor = new FileInfo(pathGetFileUpdateUser);

                    using (ExcelPackage excelPackage = new ExcelPackage(fIfor))
                    {
                        //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                        ExcelWorksheet namedWorksheetUser = excelPackage.Workbook.Worksheets[0];
                        namedWorksheetUser.Cells["A2:E2000"].Clear();

                        int row = 2;
                        for (int i = 0; i < addUserErrorByExcels.Count; i++)
                        {
                            for (int j = 1; j <= 5; j++)
                            {
                                if (i % 2 == 0)
                                {
                                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#E5DFDF");
                                    namedWorksheetUser.Cells[row, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    namedWorksheetUser.Cells[row, j].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                }
                                namedWorksheetUser.Cells[row, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                namedWorksheetUser.Cells[row, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                namedWorksheetUser.Cells[row, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                namedWorksheetUser.Cells[row, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                                namedWorksheetUser.Cells[row, j].Style.Font.Size = 13;
                                namedWorksheetUser.Cells[row, j].Style.Font.Name = "Times New Roman";
                            }

                            namedWorksheetUser.Cells[row, 1].Value = addUserErrorByExcels[i].Fullname;
                            namedWorksheetUser.Cells[row, 2].Value = addUserErrorByExcels[i].Email;
                            namedWorksheetUser.Cells[row, 3].Value = addUserErrorByExcels[i].Phone;
                            namedWorksheetUser.Cells[row, 4].Value = addUserErrorByExcels[i].AcitveUser;
                            namedWorksheetUser.Cells[row, 5].Value = addUserErrorByExcels[i].ExpireDayUser;
                            row++;
                        }

                        FileInfo fiToSave = new FileInfo(pathGetFileUpdateUser);
                        //Save your file
                        excelPackage.SaveAs(fiToSave);
                    }
                    //download file excel
                    byte[] fileBytes = System.IO.File.ReadAllBytes(pathGetFileUpdateUser);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Danh sách chỉnh sửa không thành công.xlsx");
                }
                else
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Cập nhật thành công"
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // HttpPost: /api/User/UpdateImageUsers
        [HttpPost]
        [Route("UpdateImageUsers")]
        public async Task<IActionResult> UpdateImageUsers([FromForm]string lstIdUser)
        {
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if (headerValue.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                string IdFile = Guid.NewGuid().ToString() + ".jpg";
                var result = await _userRepository.UpdateImageUsers(lstIdUser, IdFile);

                //if update success then delete old file in folder and replace by new file
                if(result.Success)
                {
                    //delete old file
                    var userId = lstIdUser.Split(',');
                    for (int i = 0; i < userId.Count(); i++)
                    {
                        var userAvatar = _userRepository.NameFileAvatarUser(new Guid(userId[i]));
                        if (userAvatar != null)
                        {
                            var filename = Path.Combine(_appSettingModel.ServerFileAvartar, Path.GetFileName(userAvatar));
                            //delete file before save
                            if (System.IO.File.Exists(filename))
                            {
                                System.IO.File.Delete(filename);
                            }
                        }
                    }

                    //save new image
                    if (Request.Form.Files.Count > 0)
                    {
                        foreach (var file in Request.Form.Files)
                        {
                            var fileContentType = file.ContentType;

                            if (fileContentType == "image/jpeg"
                            || fileContentType == "image/png" || fileContentType == "image/jpg")
                            {

                                // prepare path to save image file
                                string pathTo = _appSettingModel.ServerFileAvartar;

                                // set file path to save file
                                var filename = Path.Combine(pathTo, Path.GetFileName(IdFile));

                                // save file
                                using (var stream = System.IO.File.Create(filename))
                                {
                                    file.CopyTo(stream);
                                }
                            }
                        }
                    }
                    return Ok(result);
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // HttpPost: /api/User/UpdateUserExpireDateByUnit
        [HttpPost]
        [Route("UpdateUserExpireDateByUnit")]
        public async Task<IActionResult> UpdateUserExpireDateByUnit(UpdateUserExpireDateByUnitModel updateUserExpireDateByUnitModel)
        {
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if (headerValue.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                DateTime effectiveDate = DateTime.ParseExact(updateUserExpireDateByUnitModel.effectiveDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime expirationDate = DateTime.ParseExact(updateUserExpireDateByUnitModel.expirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                var result = await _userRepository.UpdateDateUser(effectiveDate, expirationDate, updateUserExpireDateByUnitModel.idUnit);
                return Ok(result);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region METHOD
        private bool IsValidDate(string date)
        {
            var tempDate = date.Split('/');
            int day = int.Parse(tempDate[0]);
            int month = int.Parse(tempDate[1]);
            int year = int.Parse(tempDate[2]);

            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
                return false;

            if (month < 1 || month > 12)
                return false;

            if (day < 0 || day > 0 && day > DateTime.DaysInMonth(year, month))
                return false;

            return true;
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        #endregion
    }
}
