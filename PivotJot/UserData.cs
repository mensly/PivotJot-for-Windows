using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;

namespace PivotJot
{
    
    class UserData
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private DataProtectionProvider protectionProvider = new DataProtectionProvider("LOCAL=user");
        // TODO: Cache selected

        private string token;
        public async Task<string> GetToken()
        {
            if (token == null)
            {
                string encryptedToken = localSettings.Values["token"] as string;
                if (encryptedToken != null)
                {
                    var encoded = CryptographicBuffer.DecodeFromBase64String(encryptedToken);
                    var decrypted = await protectionProvider.UnprotectAsync(encoded);
                    token = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, decrypted);
                }
            }
            return token;
        }

        public async void SetToken(string token)
        {
            this.token = token;
            if (token == null)
            {
                localSettings.Values.Remove("token");
            }
            else
            {
                var encoded = CryptographicBuffer.ConvertStringToBinary(token, BinaryStringEncoding.Utf8);
                var encrypted = await protectionProvider.ProtectAsync(encoded);
                localSettings.Values["token"] = CryptographicBuffer.EncodeToBase64String(encrypted);
            }
        }

        public List<Project> Projects
        {
            get
            {
                string json = localSettings.Values["projects"] as string;
                if (json != null)
                {
                    return JsonConvert.DeserializeObject<List<Project>>(json);
                }
                else
                {
                    return new List<Project>();
                }
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    localSettings.Values["projects"] = JsonConvert.SerializeObject(value);
                }
                else
                {
                    localSettings.Values.Remove("projects");
                }
            }
        }
        
    }
}
