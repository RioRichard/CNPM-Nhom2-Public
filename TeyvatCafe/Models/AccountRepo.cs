using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace TeyvatCafe.Models
{
    public class AccountRepo
    {
        

        IConfiguration configuration;
        

        public AccountRepo(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        

        #region register

        public int RegisterAccount(DataContext context,AuthModel obj)
        {
            int registerSuccess = 1;

            var check = CheckExistedAccount(context, obj);
            if (check == registerSuccess)
            {
                AddAccountToDatabase(context, obj);
            }
            return check;
        }

        private void AddAccountToDatabase(DataContext context, AuthModel obj)
        {
            
            var IdAccount = Helper.GerenerateIdAccount(context);
            var password = IdAccount + obj.Pass;
            var HashPassword = Helper.Hash(password);

            var Token = Helper.GenerateToken(IdAccount);
            Account output = new Account
            {
                IdAccount = IdAccount,
                UserName = obj.UserName,
                Password = HashPassword,
                Email = obj.Email,
                Token = Token,
                ExpiredTokenTime = DateTime.UtcNow.AddMinutes(15),
                IsBanned = false,
                IsConfirmed = false,
            };
            context.Accounts.Add(output);
            this.SendConfirmMail(obj.Email, Token);

            context.SaveChanges();

        }
        public string ConfirmMail(DataContext context,string Token)
        {
            var urlAccountConfirmed = "/auth/EmailConfirmCompleted";
            var urlSendEmailAgain = "/auth/sendmailagain";
            var urlInvalidAccount = Helper.url404;
            var account = context.Accounts.FirstOrDefault(p => p.Token == Token);
            var result = CheckToken(account);
            switch (result)
            {
                case -1:
                    return urlInvalidAccount;

                case 1:
                    account.Token = "";
                    account.ExpiredTokenTime = DateTime.UnixEpoch;
                    account.IsConfirmed = true;
                    context.Accounts.Update(account);
                    context.SaveChanges();
                    return urlAccountConfirmed;
                case 0:
                    ResendEmail(context,account.IdAccount,account.Email,account.ExpiredTokenTime);
                    return urlSendEmailAgain;
                default:
                    return Helper.url404;
                    
            }
            
        }

        
        
        public static int CheckToken(Account account)
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

        public static int CheckExistedAccount(DataContext context, AuthModel obj)
        {
            int userNameExisted = -1;
            int emailExisted = 0;
            int noExisted = 1;
            var resultUserName = context.Accounts.FirstOrDefault(p => p.UserName == obj.UserName);
            if (resultUserName != null)
                return userNameExisted;
            var resultEmail = context.Accounts.FirstOrDefault(p => p.Email == obj.Email);
            if (resultEmail != null)
                return emailExisted;
            return noExisted;

        }
        #endregion
        #region login
        public static bool CheckAccount(Account account, string objPassword)
        {
            
            if (account is null)
                return false;
            var password = Helper.Hash(account.IdAccount + objPassword);
            var passwordEqualCheck = password.SequenceEqual(account.Password);
            if (!passwordEqualCheck)
                return false;
            return true;
        }

        public static bool CheckConfirm(Account account)
        {
            if (account.IsConfirmed == false)
                return false;
            return true;
        }
        
        public static int LoginAccount(DataContext context,AuthModel obj,out ReturnAuthentication claim)
        {
            int wrongUserOrPassword = -1;
            int accountNotConfirmed = 0;
            int loginSuccess = 1;
            var account = context.Accounts.FirstOrDefault(p => p.UserName == obj.UserName);
            var checkAccount = CheckAccount(account, obj.Pass);
            if (!checkAccount)
            {
                claim = null;
                return wrongUserOrPassword;

            }
            var checkConfirm = CheckConfirm(account);
            if (!checkConfirm)
            {
                claim = new ReturnAuthentication
                {
                    Email = account.Email,
                    IdUser = account.IdAccount,
                    expiredTokenTime = account.ExpiredTokenTime
                    
                };
                return accountNotConfirmed;

            }

            claim = AddClaims(account, obj.RememberMe);
            return loginSuccess;

        }
        
        static ReturnAuthentication AddClaims (Account account, bool remeberMe)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,account.IdAccount),
                new Claim(ClaimTypes.Name, account.UserName),
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "User_Scheme");
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
            AuthenticationProperties properties = new AuthenticationProperties
            {
                IsPersistent = remeberMe,
                ExpiresUtc = DateTime.UtcNow.AddDays(15)
            };
            ReturnAuthentication returnObj = new ReturnAuthentication
            {
                principal = principal,
                properties = properties,
                
                
            };
            return returnObj;
        }


        public class ReturnAuthentication
        {
            public ClaimsPrincipal principal { get; set; }
            public AuthenticationProperties properties { get; set; }
            public string IdUser { get; set; }
            public string Email { get; set; }
            public DateTime expiredTokenTime { get; set; }
        }

        #endregion
        #region Send email and Reconfirm email
        public void SendConfirmMail(string email, string Token)
        {
            string content = "Chào bạn, \n\n" +
                "Đây là email xác nhận tài khoản của Teyvat cafe. \n" +
                $"Hãy bấm vào địa chỉ sau để xác nhận tài khoản của bạn: \n" +
                $"{Helper.URLWebsite}/auth/ConfirmAccount/{Token}\n" +
                $"Địa chỉ này tồn tại trong 15 phút\n" +
                $"Trân trọng \n" +
                $"Teyvat Caphe Team";
            string subject = "Xác nhận địa chỉ email";

            Helper.SendMail(configuration, email, content, subject);
        }
        public void SendForgotPassMail(string email, string Token)
        {
            string content = "Chào bạn, \n\n" +
                "Đây là email để lập lại mật khẩu cho tài khoản của Teyvat cafe. \n" +
                $"Hãy bấm vào địa chỉ sau để xác nhận tài khoản của bạn: \n" +
                $"{Helper.URLWebsite}/auth/resetpassword/{Token}\n" +
                $"Địa chỉ này tồn tại trong 15 phút\n" +
                $"Trân trọng \n" +
                $"Teyvat Caphe Team";
            string subject = "Lấy lại mật khẩu";

            Helper.SendMail(configuration, email, content, subject);
        }


        public bool ResendEmail(DataContext context, string userId,string email,DateTime expireTokenTime)
        {
            double timeBeforeResend = 10;
            var resendTooSoon = false;
            var resendGood = true;
            var checkTimeResendTooSoon = DateTime.Compare(DateTime.UtcNow, expireTokenTime.AddMinutes(-timeBeforeResend));
            if(checkTimeResendTooSoon >= 0)
            {
                var account = context.Accounts.FirstOrDefault(p => p.IdAccount == userId);
                var check = account == null;
                var newToken = Helper.GenerateToken(userId);

                account.Token = newToken;
                account.ExpiredTokenTime = DateTime.UtcNow.AddMinutes(15);
                context.Accounts.Update(account);
                context.SaveChanges();
                SendConfirmMail(email, newToken);
                return resendGood;

            }
            return resendTooSoon;
            
        }
        #endregion
        #region changePasswordAndUserInfo
        public static void ChangeUserInfo(DataContext context,string userId, string fullName, bool? gender)
        {
            var account = context.Accounts.FirstOrDefault(p => p.IdAccount == userId);
            account.FullName = fullName;
            account.Gender = gender;
            context.Accounts.Update(account);
            context.SaveChanges();
        }
        
        static void UpdatePassword (DataContext context, Account account, string newPass)
        {
            var newPassBeforeHash = account.IdAccount + newPass;
            var hashNewPass = Helper.Hash(newPassBeforeHash);
            account.Password = hashNewPass;
            context.Accounts.Update(account);
            context.SaveChanges();
        }
        public static bool ChangePassword(DataContext context, string userId, string currentPass, string newPass)
        {
            Account account = context.Accounts.FirstOrDefault(p=>p.IdAccount == userId);

            var check = CheckAccount(account, currentPass);
            if (check == false)
            {
                return false;
            }
            UpdatePassword(context, account, newPass);
            return true;
        }
        #endregion
        #region ForgotPassword
        Account CheckEmail (DataContext context, string email)
        {
            var account = context.Accounts.FirstOrDefault(p => p.Email == email);
            if (account == null)
            {
                return null;
            }
            return account;
        }
        public int ForgotPassword(DataContext context, string email)
        {
            int minutesBeforeResend = 10;
            int minutesExpireToken = 15;
            int invalidEmail = -1;
            int sendEmailTooSoon = 0;
            int validEmail = 1;

            var account = CheckEmail(context, email);
            
            if (account == null)
                return invalidEmail;
            if (DateTime.Compare(DateTime.UtcNow, account.ExpiredTokenTime.AddMinutes(-minutesBeforeResend)) < 0)
                return sendEmailTooSoon;

            var newToken = Helper.GenerateToken(account.IdAccount);
            var TokenExpired = DateTime.UtcNow.AddMinutes(minutesExpireToken);
            account.Token = newToken;
            account.ExpiredTokenTime = TokenExpired;
            context.Accounts.Update(account);
            context.SaveChanges();
            SendForgotPassMail(email, newToken);
            return validEmail;

        }
        public static bool ResetPassword(DataContext context, Account account, string password)
        {
            var checkTime = DateTime.Compare(DateTime.UtcNow, account.ExpiredTokenTime);
            
            if (checkTime > 0)
            {
                return false;
            }
            var newPass = account.IdAccount + password;
            var hashNewPass = Helper.Hash(newPass);
            account.Password = hashNewPass;
            account.ExpiredTokenTime = DateTime.UnixEpoch;
            account.Token = string.Empty;
            context.Accounts.Update(account);
            context.SaveChanges();
            return true;

        }



        #endregion

    }
}
