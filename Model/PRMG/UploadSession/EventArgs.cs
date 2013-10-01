using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessorsToolkit.Model.UploadSession.PRMG
{
    public class PRMGSessionLoggedIn : EventArgs
    {
        public string ImgFlowContainerKey { get; set; }
        public string ImgFlowSessionKey { get; set; }
    } 
}
