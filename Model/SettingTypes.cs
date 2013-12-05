using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ProcessorsToolkit.Model
{
    public class SettingTypes
    {
        //[SettingsSerializeAs(SettingsSerializeAs.Xml)]
        [Serializable]
        public class LenderLogin
        {
            public string CompanyId { get; set; }
            public string LenderId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        //We're using List<ProcessorsToolkit.Model.SettingTypes.LenderLogin> instead
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public class LenderLogins : List<LenderLogin>
        {
        }

        /*
        [Serializable]
        public class LenderLogin2
        {
            public LenderLogin2()
            {
            }

            public string LenderId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }
        */
        [Serializable()] //We're using a string collection for this right now instead
        public class AvailableLoanFolders : ApplicationSettingsBase
        {

        }
        /*
        [Serializable()]
        public class TestSetting : ApplicationSettingsBase
        {
            [UserScopedSetting]
            public string Name
            {
                get { return (string)this["name"]; }
                set { this["name"] = value; }
            }
        }*/
    }
}
