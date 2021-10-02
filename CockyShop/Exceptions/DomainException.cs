using System;

namespace CockyShop.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string description)
        {
            Description = description;
        }

        public string Description { get;}
        
        
    }
}