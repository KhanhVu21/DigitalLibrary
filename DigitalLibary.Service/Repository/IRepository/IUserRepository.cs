using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IUserRepository
    {
        #region CURD TABLE USER_ROLE
        User_Role CreateUserRole(User_Role user_Role);
        User_Role getRoleOfUser(Guid Id);
        List<User_Role> getListRoleOfUser(Guid Id);
        Response InsertRoleInUserRole(string IdUser, string role);
        Response InsertUser_Role(string IdRole, string IdUser);
        Response DeleteUser_Role(string IdUser, string IdRole);
        #endregion

        #region CRUD TABLE ROLE
        Role getUserRole(string type);
        Role getUserRolebyId(Guid Id);
        List<Role> getListUserRolebyId(Guid Id);
        List<Role> getAllRole();
        #endregion

        #region CRUD TABLE USER_TYPE
        UserType getTypeUser(string type);
        List<UserType> getAllUserType();
        UserType getUserType(Guid id);
        #endregion

        #region CRUD TABLE UNIT
        Unit getUnit(string type);
        Unit getUnit(Guid Id);
        List<Unit> getAllUnit();
        #endregion

        #region CRUD TABLE USER
        List<MutiplePrintLibraryCard> mutiplePrintLibraryCards(List<Guid> IdUsers);
        Task<Response> UpdateDateUser(DateTime AcitveUser, DateTime ExpireDayUser, Guid IdUnit);
        String NameFileAvatarUser (Guid Id);
        Task<Response> UpdateImageUsers(string lstIdUser, string IdImage);
        Task<Response> UpdateActiveAndExpireDateUser(UserDTO userDTOs);
        List<UserDTO> getAllUserByIdUnit(Guid IdUnit);
        List<UserDTO> getAllUser(SortAndSearchListUsser sortAndSearchListUsser);
        Response UpdateUser(UserDTO userDTO);
        Response InsertUser(UserDTO userDTO);
        Task<Response> LockAccountUser(Guid Id, bool isLock);
        UserDTO getUserByID (Guid Id);
        List<UserDTO> getAllUser(int pageNumber, int pageSize);
        List<UserDTO> getAllUserNotBlocked(int pageNumber, int pageSize);
        User CreateUser(User userInfor);
        User getUserByEmail(string email);
        Task<Response> RemoveUser(Guid Id);
        Task<Response> ActiveUser(UserDTO userInfor);
        Task<Response> ActiveUserByCode(string email, string code);
        Response UpdateActiveCode(string code, string email);
        Response UpdatePassword(string email, string newPassword);
        public int GetMaxUnitCode(string code, Guid IdUser);
        #endregion
    }
}
