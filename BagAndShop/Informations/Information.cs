using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop.Informations
{
    public class Information
    {
        public string? message { get; set; }
        public Information(string message)
        {
            this.message = message;
        }
        public Information()
        {
            message = null;
        }
    }
}
