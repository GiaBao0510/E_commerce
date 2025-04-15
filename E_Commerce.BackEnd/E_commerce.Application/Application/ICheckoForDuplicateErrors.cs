namespace E_commerce.Application.Application
{
    public interface ICheckoForDuplicateErrors
    {
        public Task CheckForDuplicatePhonenumbers(string phonenum);
        public Task CheckForDuplicateEmails(string email);
        public Task CheckForDuplicateUIDs(string uid);
        public Task CheckForDuplicateWhenUpdatingEmail(string email);
        public Task CheckForDuplicateWhenUpdatingPhoneNumber(string phoneNumber);
    }
}