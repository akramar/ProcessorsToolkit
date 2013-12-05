using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ProcessorsToolkit.Model
{
    /*
    internal class FormReference
    {

        // [XmlAttribute("name")]
        //   public string Name { get; set; }

        public List<Lender> Lenders { get; set; }
    }*/

    [XmlRoot("FormReference")]
    public class FormReference
    {
        [XmlArray("Lenders")]
        [XmlArrayItem("Lender")]
        public List<Lender> Lenders { get; set; }
    }

    public class Lender
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        public List<Form> Forms { get; set; }
    }

    public class Form
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        public string Filename { get; set; }
    }

    
}

    
