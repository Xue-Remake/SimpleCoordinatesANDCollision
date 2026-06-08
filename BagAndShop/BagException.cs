using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop
{
    internal class BagException : Exception
    {
        public int RemainCount = -1;
        public BagException() { }
        public BagException(string message) : base(message)
        { }
        public BagException(int remainCount)
        {
            this.RemainCount = remainCount;
        }
        public BagException(string message, int remainCount) : base(message)
        {
            this.RemainCount = remainCount;
        }
    }
    internal class RemoveException : Exception
    {
        public int RemainRequirements = -1;
        public int EmptyStateCount = -1;
        public RemoveException(int remainRequirements, int emptyStateCount)
        {
            RemainRequirements = remainRequirements;
            EmptyStateCount = emptyStateCount;
        }
        public RemoveException(string message) : base(message) { }
        public RemoveException(string message, int remainRequirements, int emptyStateCount) : base(message)
        {
            RemainRequirements = remainRequirements;
            EmptyStateCount = emptyStateCount;
        }
    }
}
