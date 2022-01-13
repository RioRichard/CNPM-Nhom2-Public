using System.Collections.Generic;
using System.Linq;

namespace TeyvatCafe.Models
{
    public class AddressRepo
    {
        public static List<AccountAddress> GetAddressByUser(DataContext context,string userID)
        {
            var addressAccount = context.AccountAddresses.Where(p => p.IdAccount == userID).ToList();
            var address = context.Addresses.ToList();
            var result = (from addressAcc in addressAccount
                          join add in address
                          on addressAcc.IdAddress equals (int)add.IdAddress
                          select new AccountAddress
                          {
                              IdAddress = addressAcc.IdAddress,
                              IdAccount = addressAcc.IdAccount,
                              IsDefault = addressAcc.IsDefault,
                              Adresss = add.Addressed,
                              PhoneNumber = add.Phone,
                              Receiver = add.Receiver,

                          }).ToList();
            return result;
        }

        public static void AddAccount(DataContext context, string userId, string address, string phone, string receiver, bool isDefault)
        {
            var newAddress = AddNewAddress(context, address, phone, receiver);
            AddNewAccountAddress(context, newAddress, userId, isDefault);
        }
        static Address AddNewAddress(DataContext context, string address, string phone, string receiver)
        {
            var newAddress = new Address()
            {
                Addressed = address,
                Phone = phone,
                Receiver = receiver
            };
            context.Addresses.Add(newAddress);
            context.SaveChanges();
            return newAddress;
        }
        static AccountAddress AddNewAccountAddress(DataContext context,  Address address, string userId, bool isDefalut)
        {
            var checkHaveAddress = context.AccountAddresses.FirstOrDefault(p => p.IdAccount == userId);
            if (checkHaveAddress == null)
                isDefalut = true;
            if(isDefalut == true)
            {
                SetDefaultAnotherAddress(context, userId);
            }
            var newAccountAddress = new AccountAddress()
            {
                IdAccount = userId,
                IdAddress = (int)address.IdAddress,
                IsDefault = isDefalut
            };
            context.AccountAddresses.Add(newAccountAddress);
            context.SaveChanges();
            return newAccountAddress;
        }
        static void SetDefaultAnotherAddress(DataContext context, string userId)
        {
            var accountAddressDefault = context.AccountAddresses.FirstOrDefault(p => p.IsDefault == true && p.IdAccount == userId);
            if (accountAddressDefault != null)
            {
                accountAddressDefault.IsDefault = false;
                context.AccountAddresses.Update(accountAddressDefault);
                context.SaveChanges();
            }
        }
        public static void SetDefault(DataContext context, string userId, int addId)
        {
            SetDefaultAnotherAddress(context, userId);
            var addressAccount = context.AccountAddresses.FirstOrDefault(p => p.IdAccount == userId && p.IdAddress == addId);
            addressAccount.IsDefault = true;
            context.AccountAddresses.Update(addressAccount);
            context.SaveChanges();
        }
    }
}
