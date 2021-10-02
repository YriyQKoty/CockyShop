namespace CockyShop.Exceptions
{
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string description) : base(description)
        {
        }
    }
}