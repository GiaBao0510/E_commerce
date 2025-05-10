namespace E_commerce.Core.Entities
{
    public class _EmailModel
    {
        public string Subject {get; set;} = String.Empty;
        public string htmlMessage {get; set;} = String.Empty;
        public string EmailTo {get; set;} = String.Empty;
        public string EmailFrom {get; set;} = String.Empty;
        public IEnumerable<MyAttachment>? Attachments {get; set;} = null;
    }

    public class MyAttachment{
        public string ContentTyoe {get; set;} = String.Empty;
        public string Filename {get; set;} = String.Empty;
        public byte[] Data {get; set;} = new byte[0];
    }
}