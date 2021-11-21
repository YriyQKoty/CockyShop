namespace CockyShop.Exceptions
{
    public class InvalidInputException : DomainException
    {
        public InvalidInputException(string description) : base(description)
        {
        }
    }
}