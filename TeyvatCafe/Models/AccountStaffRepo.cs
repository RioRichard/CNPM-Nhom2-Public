using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace TeyvatCafe.Models
{
    public class AccountStaffRepo
    {
        private IConfiguration configuration;

        public AccountStaffRepo(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public static List<AccountStaff> GetStaffID(DataContext context)
        {
            var Role = context.Roles.ToList();
            var StaffAccount = context.AccountStaffs.ToList();
            var StaffRole = context.StaffRoles.ToList();
            foreach(var item in StaffAccount)
            {
                GetRole(item, Role, StaffRole);

            }
            return StaffAccount;

        }
        static AccountStaff GetRole(AccountStaff staff, List<Role> roles, List<StaffRole> staffRoles)
        {
            if (staff.ListRole is null)
            {
                staff.ListRole = new List<Role>();
            }
            staffRoles = staffRoles.Where(p => p.IsDelete == false && p.IDStaff == staff.IDStaff).ToList();
            staff.ListRole = (from a in staffRoles
                              
                              join b in roles
                             
                              on a.IdRole equals b.IDRole
                              select b).ToList();
            return staff;
        }
        public static AccountStaff GetAccountStaff(DataContext context, string idStaff)
        {
            var staff = context.AccountStaffs.FirstOrDefault(p => p.IDStaff == idStaff);
            var Role = context.Roles.ToList();
            var StaffRole = context.StaffRoles.ToList();
            staff = GetRole(staff, Role, StaffRole);
            return staff;
        }
        #region SendMail
        public void SendConfirmMail(string email, string Token)
        {
            string content = "Chào bạn, \n\n" +
                "Đây là email xác nhận tài khoản của nhân viên Teyvat cafe. \n" +
                $"Hãy bấm vào địa chỉ sau để xác nhận tài khoản của bạn: \n" +
                $"{Helper.URLWebsite}/adminauth/ResetPassword/1/{Token}\n" +
                $"Địa chỉ này tồn tại trong 15 phút\n" +
                $"Trong trường hợp bạn muốn xác nhận lại sau khi đường dẫn hết hạn, vui lòng liên hệ với quản trị viên.\n" +
                $"Trân trọng \n" +
                $"Teyvat Caphe Team";
            string subject = "Xác nhận địa chỉ email";

            Helper.SendMail(configuration, email, content, subject);
        }
        public void SendForgotPassMail(string email, string Token)
        {
            string content = "Chào bạn, \n\n" +
                "Đây là email để lập lại mật khẩu cho tài khoản của nhân viên Teyvat cafe. \n" +
                $"Hãy bấm vào địa chỉ sau để xác nhận tài khoản của bạn: \n" +
                $"{Helper.URLWebsite}/adminauth/resetpassword/2/{Token}\n" +
                $"Địa chỉ này tồn tại trong 15 phút\n" +
                $"Trong trường hợp bạn muốn nhận lại đường dẫn sau khi đường dẫn hết hạn, vui lòng liên hệ với quản trị viên.\n" +

                $"Trân trọng \n" +
                $"Teyvat Caphe Team";
            string subject = "Lấy lại mật khẩu";

            Helper.SendMail(configuration, email, content, subject);
        }
        #endregion
        #region Login

        public static Account StaffToAccount (AccountStaff staff)
        {
            var result = new Account()
            {
                IdAccount = staff.IDStaff,
                Email = staff.Email,
                ExpiredTokenTime = staff.ExpiredTokenTime,
                FullName = staff.FullName,
                Gender = staff.Gender,
                IsBanned = staff.IsBanned,
                IsConfirmed = staff.IsConfirmed,
                Password = staff.Password,
                Token = staff.Token,
                UserName = staff.UserName
            };
            return result;
        }
        
        static AccountRepo.ReturnAuthentication AddClaims(AccountStaff account)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,account.IDStaff),
                new Claim(ClaimTypes.Name, account.UserName),
            };
            foreach (var item in account.ListRole)
            {
                var newClaim = new Claim(ClaimTypes.Role, item.RoleName);
                claims.Add(newClaim);
            }
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Staff_Scheme");
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
            AuthenticationProperties properties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTime.UtcNow.AddDays(15)
            };
            AccountRepo.ReturnAuthentication returnObj = new AccountRepo.ReturnAuthentication
            {
                principal = principal,
                properties = properties,

            };
            return returnObj;
        }
        public static bool CheckAccount(AccountStaff account, string objPassword)
        {

            if (account is null)
                return false;
            var password = Helper.Hash(account.IDStaff + objPassword);
            var passwordEqualCheck = password.SequenceEqual(account.Password);
            if (!passwordEqualCheck)
                return false;
            return true;
        }

        public static bool CheckConfirm(AccountStaff account)
        {
            if (account.IsConfirmed == false)
                return false;
            return true;
        }

        public static int LoginAccount(DataContext context, AuthModel obj, out AccountRepo.ReturnAuthentication claim)
        {
            int wrongUserOrPassword = -1;
            int accountNotConfirmed = 0;
            int loginSuccess = 1;
            int bannedStaff = -2;
            var account = context.AccountStaffs.FirstOrDefault(p => p.UserName == obj.UserName);
            var checkAccount = CheckAccount(account, obj.Pass);
            if (!checkAccount)
            {
                claim = null;
                return wrongUserOrPassword;

            }
            if(account.IsBanned == true)
            {
                claim = null;
                return bannedStaff;
            }
            var checkConfirm = CheckConfirm(account);
            if (!checkConfirm)
            {
                claim = new AccountRepo.ReturnAuthentication
                {
                    Email = account.Email,
                    IdUser = account.IDStaff,
                    expiredTokenTime = account.ExpiredTokenTime

                };
                return accountNotConfirmed;

            }
            var role = context.Roles.ToList();
            var staffRole = context.StaffRoles.ToList();
            account = GetRole(account, role, staffRole);
            claim = AddClaims(account);
            return loginSuccess;

        }
        #endregion
        #region AddStaff
        public void AddStaffToDatabase(DataContext context, StaffModel obj)
        {

            var IdAccount = Helper.GerenerateIdStaff(context);
            var password = IdAccount + Helper.RandomString(32);
            var HashPassword = Helper.Hash(password);

            var Token = Helper.GenerateToken(IdAccount);
            AccountStaff output = new AccountStaff
            {
                IDStaff = IdAccount,
                UserName = obj.UserName,
                Password = HashPassword,
                Email = obj.Email,
                Token = Token,
                ExpiredTokenTime = DateTime.UtcNow.AddMinutes(15),
                IsBanned = false,
                IsConfirmed = false,
            };
            context.AccountStaffs.Add(output);
            context.SaveChanges();

            AddRoleForStaff(context, IdAccount, obj.RoleId);
            SendConfirmMail(obj.Email, Token);

        }
        static void AddRoleForStaff(DataContext context, string idAccount, List<Guid> Roles)
        {
            if(Roles.Count == 0)
            {
                var staff = context.Roles.FirstOrDefault(p => p.RoleName == "Staff");
                Roles.Add(staff.IDRole);
            }
            foreach (var item in Roles)
            {
                var newStaffRole = new StaffRole()
                {
                    IdRole = item,
                    IDStaff = idAccount,
                    IsDelete = false
                };
                context.StaffRoles.Add(newStaffRole);
                context.SaveChanges();
            }
        }
        static bool CheckSameUserOrEmail(DataContext context, StaffModel obj)
        {
            var check = context.AccountStaffs.FirstOrDefault(p => p.Email == obj.Email || p.UserName == obj.UserName);
            if (check == null)
                return true;
            else
                return false;
        }
        public int AddStaff(DataContext context, StaffModel obj)
        {
            int UserOrEmailExisted = -1;
            
            int AddComplete = 1;
            var check = CheckSameUserOrEmail(context, obj);
            if (check == false)
                return UserOrEmailExisted;
            AddStaffToDatabase(context, obj);
            return AddComplete;
        }
        #endregion
        #region Confirm and ResetPass
        public static int CheckToken(AccountStaff account)
        {
            int invalidAccount = -1;
            int validAccountButTokenExpired = 0;
            int validAccount = 1;
            if (account == null)
            {
                return invalidAccount;
            }
            var compareExpiredTime = DateTime.Compare(DateTime.UtcNow, account.ExpiredTokenTime);
            if (compareExpiredTime > 0)
            {
                return validAccountButTokenExpired;
            }
            else
                return validAccount;
        }
        public string ConfirmMail(DataContext context, string Token)
        {
            var urlAccountConfirmed = $"/adminauth/ResetPassword/1/{Token}";
            var urlSendEmailAgain = "/adminauth/sendmailagain";
            var urlInvalidAccount = Helper.url404;
            var account = context.AccountStaffs.FirstOrDefault(p => p.Token == Token);
            var result = CheckToken(account);
            switch (result)
            {
                case -1:
                    return urlInvalidAccount;

                case 1:
                    //account.Token = "";
                    //account.ExpiredTokenTime = DateTime.UnixEpoch;
                    //account.IsConfirmed = true;
                    //context.AccountStaffs.Update(account);
                    //context.SaveChanges();
                    return urlAccountConfirmed;
                case 0:
                    return urlSendEmailAgain;
                default:
                    return Helper.url404;

            }
        }
        public bool ResendEmail(DataContext context, AccountStaff account)
        {
            double timeBeforeResend = 10;
            var resendTooSoon = false;
            var resendGood = true;
            var expiredToken = account.ExpiredTokenTime;
            var checkTimeResendTooSoon = DateTime.Compare(DateTime.UtcNow, expiredToken.AddMinutes(-timeBeforeResend));
            if (checkTimeResendTooSoon >= 0)
            {
                
                var newToken = Helper.GenerateToken(account.IDStaff);

                account.Token = newToken;
                account.ExpiredTokenTime = DateTime.UtcNow.AddMinutes(15);
                context.AccountStaffs.Update(account);
                context.SaveChanges();
                SendConfirmMail(account.Email, newToken);
                return resendGood;

            }
            return resendTooSoon;

        }
        public static bool ResetPassword(DataContext context, AccountStaff account, string pass)
        {
            var checkTime = DateTime.Compare(DateTime.UtcNow, account.ExpiredTokenTime);

            if (checkTime > 0)
            {
                return false;
            }
            var newPass = account.IDStaff + pass;
            var hashNewPass = Helper.Hash(newPass);
            account.Password = hashNewPass;
            account.ExpiredTokenTime = DateTime.UnixEpoch;
            account.Token = string.Empty;
            context.AccountStaffs.Update(account);
            context.SaveChanges();
            return true;
        }
        public bool ForgotPassword(DataContext context, string userId)
        {
            int minutesBeforeResend = 10;
            int minutesExpireToken = 15;
            

            var account = context.AccountStaffs.FirstOrDefault(p => p.IDStaff == userId);

            
            if (DateTime.Compare(DateTime.UtcNow, account.ExpiredTokenTime.AddMinutes(-minutesBeforeResend)) < 0)
                return false;

            var newToken = Helper.GenerateToken(account.IDStaff);
            var TokenExpired = DateTime.UtcNow.AddMinutes(minutesExpireToken);
            account.Token = newToken;
            account.ExpiredTokenTime = TokenExpired;
            context.AccountStaffs.Update(account);
            context.SaveChanges();
            SendForgotPassMail(account.Email, newToken);
            return true;

        }

        #endregion
        public static void ChangeUserInfo(DataContext context, string userId, string fullName, bool? gender)
        {
            var account = context.AccountStaffs.FirstOrDefault(p => p.IDStaff == userId);
            account.FullName = fullName;
            account.Gender = gender;
            context.AccountStaffs.Update(account);
            context.SaveChanges();
        }
        static void UpdatePassword(DataContext context, AccountStaff account, string newPass)
        {
            var newPassBeforeHash = account.IDStaff + newPass;
            var hashNewPass = Helper.Hash(newPassBeforeHash);
            account.Password = hashNewPass;
            context.AccountStaffs.Update(account);
            context.SaveChanges();
        }
        public static bool ChangePassword(DataContext context, string userId, string currentPass, string newPass)
        {
            AccountStaff account = context.AccountStaffs.FirstOrDefault(p => p.IDStaff == userId);

            var check = CheckAccount(account, currentPass);
            if (check == false)
            {
                return false;
            }
            UpdatePassword(context, account, newPass);
            return true;
        }
        public static void ChangeRole(DataContext context, string userId, List<Guid> roleId)
        {
            var staffRole = context.StaffRoles.Where(p => p.IDStaff == userId);
            var allRole = context.Roles.ToList();
            foreach (var item in allRole)
            {
                var checkStaff = staffRole.FirstOrDefault(p => p.IdRole.Equals(item.IDRole));

                Guid checkNewRole = roleId.FirstOrDefault(p => p.Equals(item.IDRole));
                var check = checkStaff == null;
                var check2 = checkNewRole.Equals(Guid.Empty);
                if (checkStaff == null && !checkNewRole.Equals(Guid.Empty))
                {
                    var newStaffRole = new StaffRole()
                    {
                        IdRole = item.IDRole,
                        IDStaff = userId,
                        IsDelete = false,
                    };
                    context.StaffRoles.Add(newStaffRole);
                    context.SaveChanges();
                    Console.WriteLine(1);

                }
                else if (checkStaff != null && checkNewRole.Equals(Guid.Empty))
                {
                    if (item.RoleName != "Staff" && item.RoleName !="SuperAdmin")
                    {
                        checkStaff.IsDelete = true;
                        context.StaffRoles.Update(checkStaff);
                        context.SaveChanges();
                        Console.WriteLine(2);

                    }
                    Console.WriteLine(3);


                }
                else if (checkStaff != null && !checkNewRole.Equals(Guid.Empty) && checkStaff.IsDelete == true)
                {
                    checkStaff.IsDelete = false;
                    context.StaffRoles.Update(checkStaff);
                    context.SaveChanges();
                    Console.WriteLine(4);

                }
                Console.WriteLine(5);


            }
        }
    }
}
