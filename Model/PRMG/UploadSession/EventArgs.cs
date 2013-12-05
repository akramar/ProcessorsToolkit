using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessorsToolkit.Model.PRMG.UploadSession
{
    public class PRMGSessionLoggedIn : EventArgs
    {
        public string ImgFlowContainerKey { get; set; }
        public string ImgFlowSessionKey { get; set; }
    } 
}
