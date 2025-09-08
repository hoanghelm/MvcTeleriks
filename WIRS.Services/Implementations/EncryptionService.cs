using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIRS.Shared.Configurations;
using WIRS.Shared;
using WIRS.Services.Interfaces;
using Microsoft.Extensions.Options;
using WIRS.Shared.Helpers;

namespace WIRS.Services.Implementations
{
    public class EncryptionService : IEncryptionService
    {
        private readonly AppSettings _appSettings;

        public EncryptionService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string Encrypt(string plainText)
        {
            try
            {
                return EncrytionHelper.Encrypt(plainText, _appSettings.PasswordHash, _appSettings.SaltKey, _appSettings.VIKey);
            }
            catch
            {
                return plainText;
            }
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                return EncrytionHelper.Decrypt(cipherText, _appSettings.PasswordHash, _appSettings.SaltKey, _appSettings.VIKey);
            }
            catch
            {
                return cipherText;
            }
        }
    }
}